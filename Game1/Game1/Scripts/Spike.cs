using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Spike : EnemyObject
    {
        public Spike(Game1 game)
        {
            texture = game.Content.Load<Texture2D>("assets/spike");
        }

        public override void Action()
        {
            Player.hasLost = true;        
        }
    }
}
