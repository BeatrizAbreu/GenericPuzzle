using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Box : Obstacle
    {
        public Box()
        {
            this.tag = "box";
        }

        public override void Action(Vector2 direction)
        {
            //the box is pushed in the player's direction if possible
            this.position = position + direction;
        }
    }
}
