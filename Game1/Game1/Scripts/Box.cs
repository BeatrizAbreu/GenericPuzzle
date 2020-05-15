using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class Box : Obstacle
    {
        public Box(Board board, Game1 game) : base(board)
        {
            if (Game1.isOctaboard)
                texture = game.Content.Load<Texture2D>("assets/octaboard/boxOcta");
            else
                texture = game.Content.Load<Texture2D>("assets/box");
        }

        public override bool Move(Direction direction)
        {
            Node currentNode = board.Node(position);
            Node targetNode = board.Move(currentNode, direction);

            foreach (Portal portal in board.portals)
            {
                // portal ahead!
                if (portal.pos1 == targetNode.position || portal.pos2 == targetNode.position)
                {
                    Vector2 dir = targetNode.position - currentNode.position;

                    if (BoardInfo.nDirections == 6)
                    {
                        if ((targetNode.position.X % 2 == 0 && currentNode.position.X % 2 != 0)
                            || (targetNode.position.X % 2 != 0 && currentNode.position.X % 2 == 0))
                            dir.Y += 1;
                    }

                    Vector2 pos = portal.Trigger(this.position, dir, targetNode.position);
                    targetNode = board.nodes[(int)pos.X, (int)pos.Y];
                    break;
                }
            }

            if (currentNode.position == targetNode.position) return false;

            //Can't move through lasers!
            foreach (var laser in board.lasers)
            {
                if(targetNode.position == laser.Value.position)
                {
                    targetNode.isEmpty = laser.Key.isTriggered;
                    break;
                }
            }

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
