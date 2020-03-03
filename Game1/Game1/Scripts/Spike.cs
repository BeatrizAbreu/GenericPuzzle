using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Spike : EnemyObject
    {
        public override void Action()
        {
            Player.hasLost = true;
        }
    }
}
