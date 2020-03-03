using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class Obstacle
    {
        public Vector2 position;
        public string tag;

        public virtual void Action(Vector2 direction)
        {
            this.Action(direction);
        }
    }
}
