﻿using Game1.Scripts;
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

        public Board(int width, int height, int nHoles, int nBoxes, int nEnemies, int nCollectibles)
        {
            //set board info params
            boardInfo = new BoardInfo();
            boardInfo.width = width;
            boardInfo.height = height;
            boardInfo.nHoles = nHoles;
            boardInfo.nBoxes = nBoxes;
            boardInfo.nEnemies = nEnemies;
            boardInfo.nCollectibles = nCollectibles;

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

        public Board(int width, int height, Vector2[] holesPosition, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects)
        {
            //set board info params
            boardInfo = new BoardInfo();
            boardInfo.width = width;
            boardInfo.height = height;

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

                        foreach (WinObject winObj in winObjects)
                        {
                            //if the node is taken by a winObject, jump to the next node
                            if (nodes[x, y].position == winObj.position)
                                break;

                            //if the node is empty and is not the first, second or third position
                            if (nodes[x, y].isEmpty
                                && rand > placementChance
                                && (x != 0 || y != 0)
                                && !(x == 1 || y == 0)
                                && !(x == 1 || y == 1))
                            {
                                //create and place the spikes
                                Spike spike = new Spike();
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

                    if(nodes[x, y] != null)
                    {
                        //place Collectible
                        if (CollectibleCount < boardInfo.nCollectibles && rand > 50)
                        {
                            if (nodes[x, y].isEmpty)
                            {
                                Collectible winObject = new Collectible();
                                winObject.position = nodes[x, y].position;
                                winObjects.Add(winObject);
                                CollectibleCount++;
                            }
                        }
                        //while there's still Toggles to place
                        else if (nodes[x, y] != null
                        && objCount < boardInfo.nBoxes)
                        {
                            placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nBoxes);
                            rand = RNG.Next(100);

                            foreach (Obstacle obstacle in obstacles)
                            {
                                //if the node is not a hole and the random rolls over 30
                                //and there's a box in the same line or column but not on the same cell
                                if (nodes[x, y].isEmpty
                                    && !(x == 0 && y == 0)
                                    && (x == obstacle.position.X || y == obstacle.position.Y)
                                    && rand > placementChance)
                                {
                                    //create and place the object
                                    Toggle winObject = new Toggle();
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

        public void CreateObstacles()
        {
            int boxCount = 0;
            int rand;

            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    if (nodes[x, y] != null
                        &&boxCount < boardInfo.nBoxes)
                    {
                        placementChance = Functions.GetPlacementChance(x, y, boardInfo.width, boardInfo.height, boardInfo.nBoxes);
                        rand = RNG.Next(100);

                        if (x != boardInfo.width - 1 && x != 0
                            && rand > placementChance)
                        {
                            //if there's 2 free nodes next to the box (above and below)
                            if (nodes[x + 1, y] != null && nodes[x - 1, y] != null
                                 && nodes[x + 1, y].isEmpty && nodes[x - 1, y].isEmpty)
                            {
                                //create and place the box
                                Box box = new Box(this);
                                box.position = nodes[x, y].position;
                                nodes[x, y].isEmpty = false;
                                obstacles.Add(box);
                                boxCount++;
                            }
                        }
                        else if (y != boardInfo.height - 1 && y != 0
                            && rand > placementChance)
                        {
                            //if there's 2 free nodes next to the box (to the right and left)
                            if (nodes[x, y + 1] != null && nodes[x, y - 1] != null
                                 && nodes[x, y + 1].isEmpty && nodes[x, y - 1].isEmpty)
                            {
                                //create and place the box
                                Box box = new Box(this);
                                box.position = nodes[x, y].position;
                                nodes[x, y].isEmpty = false;
                                obstacles.Add(box);
                                boxCount++;
                            }
                        }
                    }
                }
            }
            //Update the nBoxes var so there aren't too many win objects for the amount of boxes
            boardInfo.nBoxes = boxCount;
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

        //Creates all board nodes and their neighbor dictionaries
        public void CreateBoard()
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

        //verifies if any box is unreachable -> lost game (returns false)
        private bool BoxCanMove()
        {
            foreach (Obstacle obstacle in obstacles)
            {
                //if the box is on one of the four corners
                if ((obstacle.position.X == 0 && obstacle.position.Y == 0)
                    || (obstacle.position.X == boardInfo.width - 1 && obstacle.position.Y == boardInfo.height - 1))
                {
                    //is there a winObject in that position?
                    foreach (WinObject winObj in winObjects)
                    {
                        if (winObj.position == obstacle.position)
                            return true; //box is reachable
                    }
                    return false; //the box is stuck in a corner without a pressure plate
                }

                //if the nodes are squares and the box is in one of the four extreme lines or columns
                //OR if the nodes are hexagons OR octagons AND the box is on the 2 extreme columns (x == 0 || x == width-1)
                if (obstacle.position.X == boardInfo.width - 1
                   || obstacle.position.X == 0
                   || (boardInfo.nDirections == 4
                       && (obstacle.position.Y == boardInfo.height - 1
                       || obstacle.position.Y == 0))
                   || obstacle.position.Y == 0 || obstacle.position.Y == boardInfo.height - 1)
                {
                    if (obstacle.position.X % 2 == 0 || obstacle.position.X == boardInfo.width - 1)
                    {
                        return false; //box is unpushable
                    }
                    WinObject[] nearbyToggles = GetClosestWinObjects(obstacle.position, 3);
                    foreach (WinObject toggle in nearbyToggles)
                    {
                        if (ValidPath(nodes[(int)obstacle.position.X, (int)obstacle.position.Y], toggle.position))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true; //box is reachable
            }
            return true;
        }

        public bool ValidPath(Node currentNode, Vector2 destination)
        {
            foreach (KeyValuePair<Direction, Node> neighbor in currentNode.neighbors)
            {
                //if the current neighbor has a box 
                if (!neighbor.Value.isEmpty)
                    return false;

                //if the current neighbor has the closest pressure plate
                if (destination == neighbor.Value.position)
                {
                    return true;
                }

                //if the current neighbor has an enemy 
                foreach (EnemyObject enemyObj in enemyObjects)
                {
                    if (enemyObj.position == neighbor.Value.position)
                        return false;
                }

                //if the current neighbor is reachable 
                ValidPath(neighbor.Value, destination);
            }
            return false;
        }

        public WinObject[] GetClosestWinObjects(Vector2 position, int count)
        {
            int[] minDistance = new int[count];
            int distance;
            int x = 0, y = 0;
            WinObject[] minWinObjects = new WinObject[count];

            foreach (WinObject winObject in winObjects)
            {
                x = (int)Math.Abs(winObject.position.X - position.X);
                y = (int)Math.Abs(winObject.position.Y - position.Y);
                distance = x > y ? x : y;

                if (minDistance[0] == 0)
                {
                    minDistance[0] = distance;
                    minWinObjects[0] = winObject;
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (minDistance[i] > distance)
                        {
                            minDistance[i] = distance;
                            minWinObjects[i] = winObject;
                            break;
                        }
                    }
                }
            }

            return minWinObjects;
        }

        //Dictionary<Direction, NodeMCTS> CreateTree(NodeMCTS root)
        //{
        //    Dictionary<Direction, NodeMCTS> children = new Dictionary<Direction, NodeMCTS>();

        //    /* FIX ME: ONLY N TIMES */

        //    foreach (KeyValuePair<Direction, Node> neighbor in nodes[0, 0].neighbors)
        //    {
        //        NodeMCTS child = new NodeMCTS(CreateTree(root));
        //        children.Add(neighbor.Key, child);
        //    }
        //    return children;
        //}

        //Returns the targetNode for each node-direction pair
        public abstract Node Move(Node currentNode, Direction direction);
        //Creates a neighbor dictionary for each valid (non-hole) node
        internal abstract void CreateNeighbors();
        public abstract Vector2 DrawPosition(Vector2 cellPos);
    }
}