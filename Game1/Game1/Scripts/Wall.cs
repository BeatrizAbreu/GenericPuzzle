using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class Wall : Obstacle
    {
        private Texture2D[] textures;

        public Wall(Board board, Game1 game) : base(board)
        {
            if (Game1.isOctaboard)
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/wallOctaOn");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/wallOctaOff");
                textures[2] = game.Content.Load<Texture2D>("assets/octaboard/wallOctaQuadOn");
                textures[3] = game.Content.Load<Texture2D>("assets/octaboard/wallOctaQuadOff");
            }

            else
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/wallOn");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/wallOff");
            }

            texture = textures[0];
        }
    }
}
