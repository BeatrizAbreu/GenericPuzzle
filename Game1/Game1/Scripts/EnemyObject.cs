using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public abstract class EnemyObject
    {
        public Vector2 position;

        public abstract void Action();
    }
}
