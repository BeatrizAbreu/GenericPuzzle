using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Toggle : WinObject
    {
        public static int activeObjectCount;
        public static int objectCount;

        public Toggle(Game1 game)
        {
            tag = "Toggle";
            objectCount++;

            if (Game1.isOctaboard)
                texture = game.Content.Load<Texture2D>("assets/octaboard/toggleOcta");
            else
                texture = game.Content.Load<Texture2D>("assets/toggle");
        }

        public override void Action()
        {
            isTriggered = true;
            activeObjectCount++;
        }

        public override void Deactivate()
        {
            isTriggered = false;
            activeObjectCount--;
        }
    }
}
