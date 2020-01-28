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
        public Node[,] nodes;
        BoardInfo boardInfo;

        public Board(int width, int height, int nHoles)
        {
            boardInfo = new BoardInfo();
            //set board info params
            boardInfo.width = width;
            boardInfo.height = height;
            boardInfo.nHoles = nHoles;

            nodes = new Node[boardInfo.width, boardInfo.height];

            //create the board graph with all the information
            CreateBoard(boardInfo);
        }

        //Creates all board nodes and their neighbor dictionaries
        public void CreateBoard(BoardInfo boardInfo)
        {
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
                        //the 1st and 2nd nodes can't both be holes
                        if (nodes[0, 0] != null ||
                            (nodes[0, 0] == null && nodes[x, y] != nodes[1, 0]))
                        {
                            x++;
                            //if the current line is already full, pass on to the next line
                            if (x == boardInfo.width && y < boardInfo.height)
                            {
                                x = 0;
                                y++;
                            }
                            holesCount++;
                        }
                    }

                    nodes[x, y] = new Node();
                    //positions are filled from (0,0) to (width-1, height-1)
                    nodes[x, y].position = new Vector2(x, y);
                }
            }

            //create the neighbors after creating the nodes
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    //if not a hole
                    if (nodes[x,y] != null)
                    {
                        //create neighbors
                        CreateNeighbors(nodes[x, y], this);
                    }
                }
            }
        }

        //Creates a neighbor dictionary for each valid (non-hole) node
        private void CreateNeighbors(Node currentNode, Board board)
        {
            //create a neighbor list
            currentNode.neighbors = new Dictionary<Node, Direction>();

            float x = currentNode.position.X;
            float y = currentNode.position.Y;
            Node node = new Node();

            //identify neighbors and fill the neighbor list
            for (int y1 = 0; y1 < board.boardInfo.height; y1++)
            {
                for (int x1 = 0; x1 < board.boardInfo.width; x1++)
                {
                    node = board.nodes[x1, y1];
                    //if it's a neighbor and not a hole
                    if (node != null &&
                        (node.position == new Vector2(x - 1, y - 1)
                    || node.position == new Vector2(x, y - 1)
                    || node.position == new Vector2(x + 1, y - 1)
                    || node.position == new Vector2(x - 1, y + 1)
                    || node.position == new Vector2(x, y + 1)
                    || node.position == new Vector2(x + 1, y + 1)
                    || node.position == new Vector2(x - 1, y)
                    || node.position == new Vector2(x + 1, y)))
                    {
                        //calculate the direction from the current node to the neighbor
                        Vector2 direction = node.position - currentNode.position;
                        //direction is defined for each neighbor
                        currentNode.neighbors.Add(node, Functions.GetDirection(direction));
                    }
                }
            }
        }
    }
}