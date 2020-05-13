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
        private Texture2D[] textures;

        public LaserToggle(Game1 game)
        {
            if (Game1.isOctaboard)
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaOff");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaOn");
                textures[2] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaQuadOff");
                textures[3] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOctaQuadOn");
            }

            else
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOff");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/lasertoggleOn");
            }

            texture = textures[0];
        }

        public override void Action(Wall wall)
        {
            if(isTriggered == wall.board.Node(wall.position).isEmpty)
            {
                isTriggered = !isTriggered;
                wall.board.Node(wall.position).isEmpty = isTriggered;
            }
        }
    }
}

