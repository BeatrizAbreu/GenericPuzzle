using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class LaserToggle : WinObject
    {
        public Texture2D[] textures;

        public LaserToggle(Game1 game)
        {
            if (Game1.isOctaboard)
            {
                textures = new Texture2D[2];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaOff");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaOn");
            }

            else
            {
                textures = new Texture2D[2];
                textures[0] = game.Content.Load<Texture2D>("assets/lasertoggleOff");
                textures[1] = game.Content.Load<Texture2D>("assets/lasertoggleOn");
            }

            texture = textures[0];
        }

        public override void Action(Wall wall)
        {
            if(isTriggered == wall.board.Node(wall.position).isEmpty)
            {
                isTriggered = !isTriggered;
                wall.board.Node(wall.position).isEmpty = isTriggered;

                int index = isTriggered ? 1 : 0;

                wall.texture = wall.textures[index];
                texture = textures[index];
            }
        }
    }
}

