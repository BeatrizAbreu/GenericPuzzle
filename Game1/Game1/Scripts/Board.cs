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
    class Node
    {
        //node's position on the board
        public Vector2 position;
        //each neighbor is defined by its (negative) direction towards the current node
        public Dictionary<Node, Direction> neighbors;
    }

    //Contains the board' information
    public class BoardInfo
    {
        public int width;
        public int height;
        //number of holes/walls/non-walkable tiles
        public int nHoles;
    }

    //Creates and manages the game's board
    public class Board
    {
        BoardInfo boardInfo;

        public Board(int width, int height, int nHoles)
        {
            //set board info params
            boardInfo.width = width;
            boardInfo.height = height;
            boardInfo.nHoles = nHoles;

            //create the board graph with all the information
            CreateBoard(boardInfo);
        }

        //Creates all board nodes and their neighbor dictionaries
        public void CreateBoard(BoardInfo boardInfo)
        {
            Node[,] nodes = new Node[boardInfo.width, boardInfo.height];
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
                    if (holesCount <= boardInfo.nHoles && rand > 50)
                    {
                        x++;
                        //if the current line is already full, pass on to the next line
                        if(x == boardInfo.width)
                        {
                            x = 0;
                            y++;
                        }
                        holesCount++;
                    }

                    //positions are filled from (0,0) to (width-1, height-1)
                    nodes[x, y].position = new Vector2(x, y);
                    //create neighbors
                    CreateNeighbors(nodes[x, y], nodes);
                }
            }
        }

        //Creates a neighbor dictionary for each valid (non-hole) node
        private void CreateNeighbors(Node currentNode, Node[,] nodes)
        {
            //create a neighbor list
            currentNode.neighbors = new Dictionary<Node, Direction>();

            //identify neighbors and fill the neighbor list
            foreach (var node in nodes)
            {
                float x = currentNode.position.X;
                float y = currentNode.position.Y;

                //if it's a neighbor
                if (node.position == new Vector2(x - 1, y - 1)
                    || node.position == new Vector2(x, y - 1)
                    || node.position == new Vector2(x + 1, y - 1)
                    || node.position == new Vector2(x - 1, y + 1)
                    || node.position == new Vector2(x, y + 1)
                    || node.position == new Vector2(x + 1, y + 1)
                    || node.position == new Vector2(x - 1, y)
                    || node.position == new Vector2(x + 1, y))
                {
                    //calculate the direction from the current node to the neighbor
                    Vector2 direction = node.position - currentNode.position;
                    //direction is defined for each neighbor
                    currentNode.neighbors.Add(node, GetDirection(direction));
                }
            }
        }

        //Sets each node's neighbor with the right direction name (UP/DOWN/LEFT/RIGHT/etc.) 
        private Direction GetDirection(Vector2 direction)
        {
            Direction dir;

            //RIGHT
            if (direction.X > 0)
            {
                //UP
                if (direction.Y > 0)
                    dir = Direction.UpRight;
                //DOWN
                if (direction.Y < 0)
                    dir = Direction.DownRight;
                //NEUTRAL
                else
                    dir = Direction.Right;
            }
            //LEFT
            else if (direction.X < 0)
            {
                //UP
                if (direction.Y > 0)
                    dir = Direction.UpLeft;
                //DOWN
                if (direction.Y < 0)
                    dir = Direction.DownLeft;
                //NEUTRAL
                else
                    dir = Direction.Left;
            }
            //NEUTRAL
            else
            {
                if (direction.Y > 0)
                    dir = Direction.Up;
                else 
                    dir = Direction.Down;
            }

            return dir;
        }       
    }
}