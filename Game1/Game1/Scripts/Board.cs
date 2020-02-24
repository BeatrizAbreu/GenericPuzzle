using Game1.Scripts;
using Microsoft.Xna.Framework;
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
        public Dictionary<Node, Direction> neighbors;
        //shows if the node is occupied by a box object
        public bool isEmpty;
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
    }

    //Creates and manages the game's board
    public abstract class Board
    {
        Node[,] nodes;
        public virtual Node this[int i, int j] { 
            set { nodes[i,j] = value; }
            get { return nodes[i,j];  } 
        }

        internal BoardInfo boardInfo;

        public List<Obstacle> obstacles;
        public List<WinObject> winObjects;

        public Board(Game1 game, int width, int height, int nHoles, int nBoxes)
        {
            //set board info params
            boardInfo = new BoardInfo();          
            boardInfo.width = width;
            boardInfo.height = height;
            boardInfo.nHoles = nHoles;
            boardInfo.nBoxes = nBoxes;

            nodes = new Node[boardInfo.width, boardInfo.height];

            obstacles = new List<Obstacle>();
            winObjects = new List<WinObject>();
            
            //create the board graph with all the information
            CreateBoard(boardInfo);
            CreateObstacles();
            CreateWinObjects();
        }

        public void CreateWinObjects()
        {
            Random random = new Random();
            int objCount = 0;
            int rand;

            //go through the nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    //while there's still objects to place
                    if (objCount < boardInfo.nBoxes)
                    {
                        rand = random.Next(100);

                        foreach (Obstacle obstacle in obstacles)
                        {
                            //if the node is not a hole and the random rolls over 30
                            //and there's a box in the same line or column but not on the same cell
                            if (nodes[x, y] != null
                                && nodes[x, y].isEmpty
                                && (x == obstacle.position.X || y == obstacle.position.Y)
                                && rand > 30)
                            {
                                //create and place the object
                                WinObject winObject = new WinObject();
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

        public void CreateObstacles()
        {
            Random random = new Random();
            int boxCount = 0;
            int rand;

            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    if (boxCount < boardInfo.nBoxes)
                    {
                        rand = random.Next(100);

                        if (nodes[x, y] != null
                            && x != boardInfo.width - 1 && x != 0
                            && rand > 30)
                        {
                            //if there's 2 free nodes next to the box (above and below)
                            if (nodes[x + 1, y] != null && nodes[x - 1, y] != null
                                 && nodes[x + 1, y].isEmpty && nodes[x - 1, y].isEmpty)
                            {
                                //create and place the box
                                Box box = new Box();
                                box.position = nodes[x, y].position;
                                nodes[x, y].isEmpty = false;
                                obstacles.Add(box);
                                boxCount++;
                                //break;
                            }
                        }
                        else if (nodes[x, y] != null
                            && y != boardInfo.height - 1 && y != 0
                            && rand > 30)
                        {
                            //if there's 2 free nodes next to the box (to the right and left)
                            if (nodes[x, y + 1] != null && nodes[x, y - 1] != null
                                 && nodes[x, y + 1].isEmpty && nodes[x, y - 1].isEmpty)
                            {
                                //create and place the box
                                Box box = new Box();
                                box.position = nodes[x, y].position;
                                nodes[x, y].isEmpty = false;
                                obstacles.Add(box);
                                boxCount++;
                                //break;
                            }
                        }
                    }
                }
            }

            //Update the nBoxes var so there aren't too many win objects for the amount of boxes
            boardInfo.nBoxes = boxCount;
        }

        //Creates all board nodes and their neighbor dictionaries
        public void CreateBoard(BoardInfo boardInfo)
        {
            Vector2 lastHolePosition = new Vector2(0,0);
            Random random = new Random();
            int holesCount = 0;
            int rand;

            //create the matrix of nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                //create each node cell
                for (int x = 0; x < boardInfo.width; x++)
                {
                    rand = random.Next(100);
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
                            lastHolePosition = new Vector2(x,y);
                            holesCount++;
                        }
                    }

                    nodes[x, y] = new Node();
                    //positions are filled from (0,0) to (width-1, height-1)
                    nodes[x, y].position = new Vector2(x, y);
                    nodes[x, y].isEmpty = true;
                }
            }

            //create the neighbors after creating the nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    //if not a hole
                    if (nodes[x, y] != null)
                    {
                        //create neighbors
                        CreateNeighbors(nodes[x, y]);
                    }
                }
            }
        }

        public Dictionary<Keys, Vector2> GetKeysDirection(int nDirections)
        {
            Dictionary<Keys, Vector2> moves = new Dictionary<Keys, Vector2>();

            moves.Add(Keys.W, new Vector2(0, -1)); //UP
            moves.Add(Keys.S, new Vector2(0, 1)); //DOWN
          
            if (nDirections != 6)
            {
                //4 directions
                moves.Add(Keys.A, new Vector2(-1, 0)); //LEFT
                moves.Add(Keys.D, new Vector2(1, 0)); //RIGHT

                //8 directions
                if (nDirections == 8)
                {
                    moves.Add(Keys.Z, new Vector2(-1, 1)); //DOWNLEFT
                    moves.Add(Keys.X, new Vector2(1, 1)); //DOWNRIGHT
                    moves.Add(Keys.Q, new Vector2(-1, -1)); //UPLEFT
                    moves.Add(Keys.E, new Vector2(1, -1)); //UPRIGHT
                }
            }
            else
            {
                moves.Add(Keys.A, new Vector2(-1, 1)); //DOWNLEFT
                moves.Add(Keys.D, new Vector2(1, 1)); //DOWNRIGHT
                moves.Add(Keys.Q, new Vector2(-1, -1)); //UPLEFT
                moves.Add(Keys.E, new Vector2(1, -1)); //UPRIGHT
            }

            return moves;
        }

        public Node Move(Node currentNode, Vector2 direction)
        {
            //indicates the box's movement direction
            Vector2 futureDirection = direction;

            //if x is pair and the direction is DOWN or if x is not pair and the direction is UP and direction.X is NOT ZERO
            if (direction.X != 0
                && ((currentNode.position.X % 2 == 0 && direction.Y > 0)
                || (currentNode.position.X % 2 != 0 && direction.Y < 0)))
            {
                futureDirection = direction;
                direction = new Vector2(direction.X, 0);
            }
            else if (direction.X != 0)
                futureDirection = new Vector2(direction.X, 0);

            //finding the next node through the current node's neighbors
            foreach (KeyValuePair<Node, Direction> neighbor in currentNode.neighbors)
            {
                Direction dir = Functions.GetDirection(direction);
                Direction futureDir = Functions.GetDirection(futureDirection);

                if (neighbor.Value == dir)
                {
                    //find the next node's obstacle
                    foreach (var obstacle in obstacles)
                    {
                        //find the obstacle that's in the neighbor
                        if (obstacle.position == neighbor.Key.position)
                        {
                            //if the object is a box
                            if (!neighbor.Key.isEmpty)
                            {
                                //find the box's next position once it's pushed
                                foreach (KeyValuePair<Node, Direction> futureNeighbor in neighbor.Key.neighbors)
                                {
                                    //if that position is found and the node is empty
                                    if (futureNeighbor.Value == futureDir
                                        && futureNeighbor.Key.isEmpty)
                                    {
                                        //cast the box's action
                                        obstacle.Action(futureDirection);
                                        //update the neighbor node's state to empty as the box is pushed
                                        neighbor.Key.isEmpty = true;
                                        //update the future neighbor's state to not empty
                                        futureNeighbor.Key.isEmpty = false;

                                        foreach (WinObject winObject in winObjects)
                                        {
                                            //if a pressure plate is found in the same position, the box is placed on the pressure plate
                                            if (winObject.position == futureNeighbor.Key.position)
                                            {
                                                winObject.Action();
                                                //inObject.isTriggered = true;
                                            }
                                            //if the box was in a pressure plate
                                            if (winObject.position == neighbor.Key.position)
                                            {
                                                winObject.Deactivate();
                                            }
                                        }

                                       // nMoves++;
                                        //update the player's position
                                        return neighbor.Key;
                                    }
                                }
                                return currentNode;
                            }

                            ////if the object is a spike
                            //else
                            //{

                            //}
                        }
                    }

                  //  nMoves++;
                    //the current node is updated
                    return neighbor.Key;
                }
            }

            return currentNode;
        }
    

        //Creates a neighbor dictionary for each valid (non-hole) node
        internal abstract void CreateNeighbors(Node currentNode);
        public abstract void Draw(GameTime gameTime);

        public abstract Vector2 DrawPosition(Vector2 cellPos); 
    }
}