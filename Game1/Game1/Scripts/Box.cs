using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Box : Obstacle
    {
        public Box(Board board, Game1 game) : base(board)
        {
            texture = game.Content.Load<Texture2D>("assets/box");
        }

        public override bool Move(Direction direction)
        {
            Node currentNode = board.Node(position);
            Node targetNode = board.Move(currentNode, direction);

            if (currentNode.position == targetNode.position) return false;

            if (targetNode.isEmpty)
            {
                targetNode.isEmpty = false;
                currentNode.isEmpty = true;
                position = targetNode.position;

                foreach (WinObject winObject in board.winObjects)
                {                                    
                    if (winObject.tag == "Toggle")
                    {
                        //if a pressure plate is found in the same position, the box is placed on the pressure plate
                        if (winObject.position == position)
                            winObject.Action();

                        //if the box was in a pressure plate
                        if (winObject.position == currentNode.position)
                        {
                            winObject.Deactivate();
                        }
                    }


                }
                return true;
            }
            return false;
        }
    }
}
