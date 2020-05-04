using Game1.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    //Names all possible neighbor directions
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    //Contains the information about each node
    public class Node
    {
        //node's position on the board
        public Vector2 position;
        //each neighbor is defined by its (negative) direction towards the current node
        public Dictionary<Direction, Node> neighbors;
        //shows if the node is occupied by a box object
        public bool isEmpty;

        public Node() { neighbors = new Dictionary<Direction, Node>(); }
    }

    //Contains the board' information
    public class BoardInfo
    {
        public int nDirections;

        public int width;
        public int height;

        //number of holes/walls/non-walkable tiles
        public int nHoles;
        public int nBoxes;
        public int nEnemies;
        public int nCollectibles;
    }

    //Creates and manages the game's board
    public abstract class Board
    {
        public Texture2D nodeTexture;
        private Game1 game;
        public Node[,] nodes;
        public virtual Node this[int i, int j]
        {
            set { nodes[i, j] = value; }
            get { return nodes[i, j]; }
        }

        internal BoardInfo boardInfo;

        public List<Obstacle> obstacles;
        public List<WinObject> winObjects;
        public List<EnemyObject> enemyObjects;
        private int placementChance;

        private Vector2[] holesPosition;

        public Board(int width, int height, int nHoles, int nBoxes, int nEnemies, int nCollectibles, int nDirections, Game1 game)
        {
            //set board info params
            boardInfo = new BoardInfo();
            boardInfo.width = width;
            boardInfo.height = height;
            boardInfo.nHoles = nHoles;
            boardInfo.nBoxes = nBoxes;
            boardInfo.nEnemies = nEnemies;
            boardInfo.nDirections = nDirections;
            boardInfo.nCollectibles = nCollectibles;

            this.game = game;
            nodes = new Node[boardInfo.width, boardInfo.height];

            obstacles = new List<Obstacle>();
            winObjects = new List<WinObject>();
            enemyObjects = new List<EnemyObject>();

            //create the board graph with all the information
            CreateBoard();
            CreateObstacles();
            CreateWinObjects();
            CreateEnemyObjects();
        }

        public Board(int width, int height, Vector2[] holesPosition, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, Game1 game)
        {
            //set board info params
            boardInfo = new BoardInfo();
            boardInfo.width = width;
            boardInfo.height = height;

            this.game = game;
            nodes = new Node[boardInfo.width, boardInfo.height];
            this.holesPosition = holesPosition;

            this.obstacles = obstacles;
            this.winObjects = winObjects;
            this.enemyObjects = enemyObjects;

            //create the board graph with all the information
            CreateBoardFromFile();
        }

        public Node Node(Vector2 pos)
        {
            return this[(int)pos.X, (int)pos.Y];
        }

        public void CreateEnemyObjects()
        {
            int objCount = 0;
            int rand;

            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    if (nodes[x, y] != null
                        && objCount < boardInfo.nEnemies)
                    {
                        placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nEnemies);
                        rand = RNG.Next(100);

                        int winObjCount = 0;

                        foreach (WinObject winObj in winObjects)
                        {
                            //if the node is taken by a winObject, jump to the next node
                            if (nodes[x, y].position == winObj.position)
                            {
                                break;
                            }

                            winObjCount++;

                            //if the node is empty and is not the first, second or third position
                            if (winObjCount == winObjects.Count
                                && nodes[x, y].isEmpty
                                && rand > placementChance
                                && (x != 0 || y != 0)
                                && !(x == 1 || y == 0)
                                && !(x == 1 || y == 1))
                            {
                                //create and place the spikes
                                Spike spike = new Spike(game);
                                spike.position = nodes[x, y].position;
                                //nodes[x, y].isEmpty = false;
                                enemyObjects.Add(spike);
                                objCount++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void CreateWinObjects()
        {
            int objCount = 0;
            int CollectibleCount = 0;
            int rand;

            //go through the nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    rand = RNG.Next(100);

                    if (nodes[x, y] != null && !(x == 0 && y == 0))
                    {
                        //place Collectible
                        if (CollectibleCount < boardInfo.nCollectibles && rand > 50)
                        {
                            if (nodes[x, y].isEmpty)
                            {
                                bool placeCollectible = true;

                                foreach (WinObject c in winObjects)
                                {
                                    if(c.tag == "Collectible" 
                                        && (c.position.X == x || c.position.Y == y))
                                    {
                                        placeCollectible = false;
                                        break;
                                    }
                                }

                                if(placeCollectible)
                                {
                                    Collectible winObject = new Collectible(game);
                                    winObject.position = nodes[x, y].position;
                                    winObjects.Add(winObject);
                                    CollectibleCount++;
                                }

                            }
                        }
                        //while there's still Toggles to place
                        else if (nodes[x, y] != null && objCount < boardInfo.nBoxes)
                        {
                            placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nBoxes);
                            rand = RNG.Next(100);

                            foreach (Obstacle obstacle in obstacles)
                            {
                                //if the node is not a hole and the random rolls over 30
                                //and there's a box in the same line or column but not on the same cell
                                if (nodes[x, y].isEmpty
                                    && ((x == obstacle.position.X && x != 0) || (y == obstacle.position.Y && y != 0))
                                    && rand > placementChance)
                                {
                                    //create and place the object
                                    Toggle winObject = new Toggle(game);
                                    winObject.position = nodes[x, y].position;
                                    winObjects.Add(winObject);
                                    objCount++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void NEWCreateWinObjects()
        {
            int objCount = 0;
            int CollectibleCount = 0;
            int rand;

            //go through the nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    rand = RNG.Next(100);

                    if (nodes[x, y] != null)
                    {
                        //place Collectible while there's still collectibles to place and the node is empty
                        if (CollectibleCount < boardInfo.nCollectibles
                            && rand > 70
                            && nodes[x, y].isEmpty)
                        {
                            //Create collectible
                            Collectible winObject = new Collectible(game);
                            winObject.position = nodes[x, y].position;
                            winObjects.Add(winObject);
                            CollectibleCount++;
                        }

                        //while there's still Toggles to place
                        else if (nodes[x, y] != null && objCount < boardInfo.nBoxes)
                        {
                            placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nBoxes);
                            rand = RNG.Next(100);

                            //go through every obstacle
                            foreach (Obstacle obstacle in obstacles)
                            {
                                //if the node is not a hole and the random rolls over 30
                                //and there's a box in the same line or column but not on the same cell
                                if ((x != 0 || y != 0)
                                    && nodes[x, y].isEmpty
                                    && nodes[x, y].position != obstacle.position)
                                {
                                    //create and place the object
                                    Toggle winObject = new Toggle(this.game);

                                    if (boardInfo.nDirections == 4 || (boardInfo.nDirections == 6 && x % 2 == obstacle.position.X % 2) && rand > placementChance)
                                    {
                                        //the box is in an extreme collumn - the win object must be in that same column
                                        if ((x == 0 || x == boardInfo.width - 1) && (obstacle.position.X == 0 || obstacle.position.X == boardInfo.width - 1))
                                        {
                                            winObject.position.X = obstacle.position.X;
                                            winObject.position.Y = y;
                                        }
                                        //the box is in an extreme line - the win object must be in that same line
                                        else if ((y == 0 || y == boardInfo.height - 1) && (obstacle.position.Y == 0 || obstacle.position.Y == boardInfo.height - 1))
                                        {
                                            winObject.position.Y = obstacle.position.Y;
                                            winObject.position.X = x;
                                        }
                                    }

                                    else if (x != 0 && y != 0
                                        && (x == obstacle.position.X || y == obstacle.position.Y))
                                    {
                                        winObject.position = nodes[x, y].position;
                                    }

                                    winObjects.Add(winObject);
                                    objCount++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateObstacles()
        {
            int boxCount = 0;
            int rand;

            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    if (nodes[x, y] != null
                        && boxCount < boardInfo.nBoxes)
                    {
                        placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nBoxes);
                        rand = RNG.Next(100);

                        //it's not the first or last line/collumn
                        if (y != boardInfo.height - 1 && y > 0
                            && x > 0 && x != boardInfo.width - 1)
                        {
                            //if there's 2 free nodes next to the box (above and below)
                            if (nodes[x, y + 1] != null && nodes[x, y - 1] != null
                                 && nodes[x, y + 1].isEmpty && nodes[x, y - 1].isEmpty
                                 && rand > placementChance)
                            {
                                CreateBox(x, y, ref boxCount);
                            }

                            else if (boardInfo.nDirections == 6)
                            {
                                rand = RNG.Next(100);

                                if (rand > placementChance)
                                {
                                    //check the first diagonals - free nodes to the upper right/lower left
                                    //&& check the second diagonals - free nodes to the lower right/upper left          
                                    if ((nodes[x + 1, y] != null && nodes[x - 1, y - 1] != null
                                     && nodes[x + 1, y].isEmpty && nodes[x - 1, y - 1].isEmpty)
                                     || (nodes[x + 1, y - 1] != null && nodes[x - 1, y] != null
                                     && nodes[x + 1, y - 1].isEmpty && nodes[x - 1, y].isEmpty))
                                    {
                                        CreateBox(x, y, ref boxCount);
                                    }
                                }
                            }
                        }

                        //if this is a 4 direction board, the box can be placed on a node as long as it can be pushed to the right/left
                        else if ((x != 0 || y != 0) && boardInfo.nDirections == 4)
                        {
                            rand = RNG.Next(100);

                            //it's not the first or last column
                            if (x != boardInfo.width - 1 && x != 0
                                && rand > placementChance)
                            {
                                //if there's 2 free nodes next to the box (to the right and left)
                                if (nodes[x + 1, y] != null && nodes[x - 1, y] != null
                                     && nodes[x + 1, y].isEmpty && nodes[x - 1, y].isEmpty)
                                {
                                    CreateBox(x, y, ref boxCount);
                                }
                            }
                        }

                        //we're in the 4 extreme lines/collumns
                        else
                        {
                            rand = RNG.Next(100);

                            CreateBox(x, y, ref boxCount);
                        }
                    }
                }
            }

            //Update the nBoxes var so there aren't too many win objects for the amount of boxes
            boardInfo.nBoxes = boxCount;
        }

        private bool CreateBox(int x, int y, ref int boxCount)
        {
            //create and place the box
            Box box = new Box(this, game);
            box.position = nodes[x, y].position;

            //Check all the previously created boxes
            foreach (Obstacle obstacle in obstacles)
            {
                //Check the neighbors to said box
                foreach (var neighbor in nodes[(int)box.position.X, (int)box.position.Y].neighbors)
                {
                    //there's already a box in a neighbor
                    if (!neighbor.Value.isEmpty)
                        return false;
                }
            }

            if (BoxRules(box))
            {
                nodes[x, y].isEmpty = false;
                obstacles.Add(box);
                boxCount++;
                return true;
            }
            return false;
        }

        //Checks if a newly created box is ready for placement (not in a bad position)
        private bool BoxRules(Obstacle box)
        {
            //if the box is on one of the four corners
            if ((box.position.X == 0 && box.position.Y == 0)
                || (box.position.X == boardInfo.width - 1 && box.position.Y == boardInfo.height - 1))
            {
                //is there a winObject in that position?
                foreach (WinObject winObj in winObjects)
                {
                    if (winObj.position == box.position)
                        return true; //box is reachable
                }
                return false; //the box is stuck in a corner without a pressure plate
            }

            //if the nodes are squares and the box is in one of the four extreme lines or columns
            //OR if the nodes are hexagons OR octagons AND the box is on the 2 extreme columns (x == 0 || x == width-1)
            if (box.position.X == boardInfo.width - 1
               || box.position.X == 0
               || (boardInfo.nDirections == 4
                   && (box.position.Y == boardInfo.height - 1
                   || box.position.Y == 0))
               || box.position.Y == 0 || box.position.Y == boardInfo.height - 1)
            {
                if (box.position.X % 2 == 0 || box.position.X == boardInfo.width - 1)
                {
                    return false; //box is unpushable
                }
            }
            return true; //box is reachable
        }

        //Creates the board read from file
        public void CreateBoardFromFile()
        {
            int counter = 0;
            //create the matrix of nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                //create each node cell
                for (int x = 0; x < boardInfo.width; x++)
                {
                    //create a hole: skip the current position
                    if (holesPosition.Length > counter
                        && x == holesPosition[counter].X
                        && y == holesPosition[counter].Y)
                    {
                        x++;
                        //if the current line is already full, pass on to the next line
                        if (x == boardInfo.width && y < boardInfo.height)
                        {
                            x = 0;
                            y++;
                        }
                        counter++;
                    }

                    nodes[x, y] = new Node();
                    //positions are filled from (0,0) to (width-1, height-1)
                    nodes[x, y].position = new Vector2(x, y);
                    nodes[x, y].isEmpty = true;
                }
            }

            CreateNeighbors();
        }

        //private void CheckEachNewBox()
        //{
        //    foreach (Obstacle box in obstacles)
        //    {
        //        int i = 0;
        //        if (!BoxCanMove(box))
        //        {
        //            int xRand, yRand;
        //            xRand = RNG.Next(boardInfo.width - 1) + 1;
        //            yRand = RNG.Next(boardInfo.height - 1) + 1;
        //            if (nodes[xRand, yRand] != null && nodes[xRand, yRand].isEmpty)
        //            {
        //                nodes[(int)box.position.X, (int)box.position.Y].isEmpty = true;
        //                obstacles[i].position.X = xRand;
        //                obstacles[i].position.Y = yRand;
        //                nodes[xRand, yRand].isEmpty = false;
        //                CheckEachNewBox();
        //            }
        //            else
        //                CheckEachNewBox();
        //        }
        //        i++;
        //    }
        //}

        //Creates all board nodes and their neighbor dictionaries
        private void CreateBoard()
        {
            Vector2 lastHolePosition = new Vector2(0, 0);
            int holesCount = 0;
            int rand;

            //create the matrix of nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                //create each node cell
                for (int x = 0; x < boardInfo.width; x++)
                {
                    rand = RNG.Next(100);
                    //create a hole: skip the current position
                    if (holesCount < boardInfo.nHoles && rand > 50)
                    {
                        //there can't be two adjacent holes
                        if (!(x == lastHolePosition.X + 1 && y == lastHolePosition.Y
                           || x == lastHolePosition.X - 1 && y == lastHolePosition.Y + 1
                           || x == lastHolePosition.X && y == lastHolePosition.Y + 1
                           || x == lastHolePosition.X + 1 && y == lastHolePosition.Y + 1)
                           && !(x == 0 && y == 0))
                        {
                            x++;
                            //if the current line is already full, pass on to the next line
                            if (x == boardInfo.width && y < boardInfo.height)
                            {
                                x = 0;
                                y++;
                            }
                            lastHolePosition = new Vector2(x, y);
                            holesCount++;
                        }
                    }

                    nodes[x, y] = new Node();
                    //positions are filled from (0,0) to (width-1, height-1)
                    nodes[x, y].position = new Vector2(x, y);
                    nodes[x, y].isEmpty = true;
                }
            }

            CreateNeighbors();
        }

        public Dictionary<Keys, Direction> GetKeysDirection(int nDirections)
        {
            Dictionary<Keys, Direction> moves = new Dictionary<Keys, Direction>();

            moves[Keys.W] = Direction.Up;
            moves[Keys.S] = Direction.Down;

            if (nDirections != 6)
            {
                //4 directions
                moves[Keys.A] = Direction.Left;
                moves[Keys.D] = Direction.Right;

                //8 directions
                if (nDirections == 8)
                {
                    moves[Keys.Z] = Direction.DownLeft;
                    moves[Keys.X] = Direction.DownRight;
                    moves[Keys.Q] = Direction.UpLeft;
                    moves[Keys.E] = Direction.UpRight;
                }
            }
            else
            {
                moves[Keys.A] = Direction.DownLeft;
                moves[Keys.D] = Direction.DownRight;
                moves[Keys.Q] = Direction.UpLeft;
                moves[Keys.E] = Direction.UpRight;
            }
            return moves;
        }

        public int EvaluateVictory(GameState gameState)
        {
            int triggeredCount = 0;
            int r = 0;

            if (Player.hasLost)
                return -1;

            foreach (WinObject winObj in gameState.winObjects)
            {
                if (winObj.isTriggered)
                    triggeredCount++;
            }

            return r = triggeredCount == gameState.winObjects.Count ? 1 : 0;
        }

        ////verifies if any box is unreachable -> lost game (returns false)
        //private bool BoxCanMove(Obstacle box)
        //{
        //    //if the box is on one of the four corners
        //    if ((box.position.X == 0 && box.position.Y == 0)
        //        || (box.position.X == boardInfo.width - 1 && box.position.Y == boardInfo.height - 1))
        //    {
        //        //is there a winObject in that position?
        //        foreach (WinObject winObj in winObjects)
        //        {
        //            if (winObj.position == box.position)
        //                return true; //box is reachable
        //        }
        //        return false; //the box is stuck in a corner without a pressure plate
        //    }

        //    //if the nodes are squares and the box is in one of the four extreme lines or columns
        //    //OR if the nodes are hexagons OR octagons AND the box is on the 2 extreme columns (x == 0 || x == width-1)
        //    if (box.position.X == boardInfo.width - 1
        //       || box.position.X == 0
        //       || (boardInfo.nDirections == 4
        //           && (box.position.Y == boardInfo.height - 1
        //           || box.position.Y == 0))
        //       || box.position.Y == 0 || box.position.Y == boardInfo.height - 1)
        //    {
        //        if (box.position.X % 2 == 0 || box.position.X == boardInfo.width - 1)
        //        {
        //            return false; //box is unpushable
        //        }

        //        foreach (WinObject toggle in winObjects)
        //        {
        //            if (toggle.tag == "Toggle" && ValidPath(nodes[(int)box.position.X, (int)box.position.Y], toggle.position, nodes[(int)box.position.X, (int)box.position.Y]))
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    return true; //box is reachable
        //}

        //private bool ValidPath(Node currentNode, Vector2 destination, Node previousNode)
        //{
        //    int c = 1;
        //    foreach (KeyValuePair<Direction, Node> neighbor in currentNode.neighbors)
        //    {
        //        bool enemy = false;

        //        if (neighbor.Value != previousNode
        //            && neighbor.Value.isEmpty)
        //        {
        //            //if it has a
        //            if (destination == neighbor.Value.position)
        //                return true;

        //            //if the current neighbor doesn't have an enemy 
        //            foreach (EnemyObject enemyObj in enemyObjects)
        //            {
        //                if (enemyObj.position == neighbor.Value.position)
        //                {
        //                    enemy = true;
        //                    break;
        //                }
        //            }

        //            if(c > currentNode.neighbors.Count)
        //            if (!enemy
        //                && neighbor.Value.neighbors.Count >0 && ValidPath(neighbor.Value, destination, currentNode))
        //                return true;

        //            c++;
        //        }
        //    }
        //    return false;
        //}

        //private WinObject[] GetClosestWinObjects(Vector2 position, int count)
        //{
        //    int[] minDistance = new int[count];
        //    int distance;
        //    int x = 0, y = 0;
        //    WinObject[] minWinObjects = new WinObject[count];

        //    foreach (WinObject winObject in winObjects)
        //    {
        //        x = (int)Math.Abs(winObject.position.X - position.X);
        //        y = (int)Math.Abs(winObject.position.Y - position.Y);
        //        distance = x > y ? x : y;

        //        if (minDistance[0] == 0)
        //        {
        //            minDistance[0] = distance;
        //            minWinObjects[0] = winObject;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                if (minDistance[i] > distance)
        //                {
        //                    minDistance[i] = distance;
        //                    minWinObjects[i] = winObject;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    return minWinObjects;
        //}


        //Returns the targetNode for each node-direction pair
        public abstract Node Move(Node currentNode, Direction direction);
        //Creates a neighbor dictionary for each valid (non-hole) node
        internal abstract void CreateNeighbors();
        public abstract Vector2 DrawPosition(Vector2 cellPos);
    }
}