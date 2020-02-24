using Game1.Scripts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
  
    //Creates and manages the game's board
    public class HexaBoard : Board
    {
    
        public HexaBoard(int width, int height, int nHoles, int nBoxes) : base(width, height, nHoles, nBoxes)
        {
            /* nothing specific to hexaboard for now */
        }

        internal override void CreateNeighbors(Node currentNode, Board board)
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
                    node = this[x1, y1];
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