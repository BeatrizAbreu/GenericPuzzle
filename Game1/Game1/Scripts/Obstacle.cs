using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public abstract class Obstacle
    {
        public Vector2 position;
        internal Board board;
        public Texture2D texture;

        public Obstacle(Board board)
        {
            this.board = board;
        }

        public abstract bool Move(Direction direction);
    }
}
