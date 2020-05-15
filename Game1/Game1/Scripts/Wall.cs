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
        public Texture2D[] textures;
        public Color color;

        public Wall(Board board, Game1 game) : base(board)
        {
            if (Game1.isOctaboard)
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/octaboard/laserOnOcta");
                textures[1] = game.Content.Load<Texture2D>("assets/octaboard/laserOffOcta");
                textures[2] = game.Content.Load<Texture2D>("assets/octaboard/laserOnOctaQuad");
                textures[3] = game.Content.Load<Texture2D>("assets/octaboard/laserOffOctaQuad");
            }

            else
            {
                textures = new Texture2D[4];
                textures[0] = game.Content.Load<Texture2D>("assets/laserOn");
                textures[1] = game.Content.Load<Texture2D>("assets/laserOff");
            }

            texture = textures[0];
        }
    }
}
