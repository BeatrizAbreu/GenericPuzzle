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
        public Wall(Vector2 position, Game1 game) : base(position)
        {
            if (Game1.isOctaboard)
                texture = game.Content.Load<Texture2D>("assets/octaboard/wallOcta");
            else
                texture = game.Content.Load<Texture2D>("assets/wall");
        }
    }
}
