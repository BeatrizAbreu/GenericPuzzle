using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class Portal : TriggerObject
    {
        public Color color;

        public Portal(Vector2 pos1, Vector2 pos2, Color color, Game1 game)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
            this.color = color;

            if (Game1.isOctaboard)
                texture1 = texture2 = game.Content.Load<Texture2D>("assets/octaboard/portalOcta");
            else
                texture1 = texture2 = game.Content.Load<Texture2D>("assets/portal");
        }

        //Trigger function - Player teleport
        public override Vector2 Trigger(Vector2 position, Vector2 direction, Vector2 pos)
        {
            Vector2 endPos = position;

            if (pos == pos1)
                endPos = pos2 + direction;
            else
                endPos = pos1 + direction;

            return endPos;
        }
    }
}
