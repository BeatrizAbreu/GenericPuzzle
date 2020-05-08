using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Collectible : WinObject
    {
        public static int activeObjectCount;
        public static int objectCount;

        public Collectible(Game1 game)
        {
            tag = "Collectible";
            objectCount++;

            if (Game1.isOctaboard)
                texture = game.Content.Load<Texture2D>("assets/octaboard/collectibleOcta");
            else
                texture = game.Content.Load<Texture2D>("assets/collectible");
        }

        public override void Action()
        {
            isTriggered = true;
            activeObjectCount++;
        }
    }
}
