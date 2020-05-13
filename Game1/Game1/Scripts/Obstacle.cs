using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public abstract class Obstacle
    {
        public Vector2 position;
        public Texture2D texture;
        internal Board board;

        public Obstacle(Board board)
        {
            this.board = board;
        }

        public Obstacle(Vector2 position)
        {
            this.position = position;
        }

        public virtual bool Move(Direction direction)
        {
            return false;
        }
    }
}
