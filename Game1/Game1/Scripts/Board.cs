using Game1.Scripts;
using Microsoft.Xna.Framework;
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

        public Board(int width, int height, int nHoles, int nBoxes)
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
                        CreateNeighbors(nodes[x, y], this);
                    }
                }
            }
        }

        //Creates a neighbor dictionary for each valid (non-hole) node
        internal abstract void CreateNeighbors(Node currentNode, Board board);
    }
}