using Microsoft.Xna.Framework;

namespace Game1
{
    public abstract class Obstacle
    {
        public Vector2 position;
        internal Board board;
        public Obstacle(Board board)
        {
            this.board = board;
        }

        public abstract bool Move(Direction direction);
    }
}
