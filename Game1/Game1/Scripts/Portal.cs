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
        bool isUsed;
        public Color color;

        public Portal(Vector2 pos1, Vector2 pos2, Color color, Game1 game)
        {
            isUsed = false;

            this.pos1 = pos1;
            this.pos2 = pos2;
            this.color = color;

            if (Game1.isOctaboard)
                texture1 = texture2 = game.Content.Load<Texture2D>("assets/octaboard/portalOcta");
            else
                texture1 = texture2 = game.Content.Load<Texture2D>("assets/portal");           
        }

        //Trigger function - Player teleport
        public override void Trigger(Player player, Direction dir)
        {
            isUsed = !isUsed;
            Vector2 playerPos = player.position;

            if (!isUsed)
                player.position = pos2;
            else
                player.position = pos1;

            if(!player.Move(dir))
            {
                isUsed = !isUsed;
                player.position = playerPos;
            }
        }
    }
}
