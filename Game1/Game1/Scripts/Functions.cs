using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Functions
    {
        //Sets each node's neighbor with the right direction name (UP/DOWN/LEFT/RIGHT/etc.) 
        public static Direction GetDirection(Vector2 direction)
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
