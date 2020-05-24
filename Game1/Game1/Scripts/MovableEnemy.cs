using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class MovableEnemy : EnemyObject
    {
        Vector2 source;
        public Vector2 currentPosition;
        int distance;
        public string tag;

        public MovableEnemy(Vector2 source, int distance, string tag, bool xAxis)
        {
            this.source = source;
            currentPosition = source;
            this.distance = distance;
            this.tag = tag;
        }

        public override void Action(Board board)
        {
            Vector2 destination = currentPosition;

            Move(board);
        }

        public bool Move(Board board)
        {
            Vector2 dir;
            Vector2 targetPosition;

            if (currentPosition.Y + 1 <= source.Y)
            {
                targetPosition = new Vector2(currentPosition.X, currentPosition.Y + 1);
                dir = new Vector2(0, 1);
            }
            else
            {
                targetPosition = new Vector2(currentPosition.X, currentPosition.Y - 1);
                dir = new Vector2(0, -1);
            }
          //  targetPosition = currentPosition.Y + 1 <= source.Y + distance ? new Vector2(currentPosition.X, currentPosition.Y + 1) : new Vector2(currentPosition.X, currentPosition.Y - 1);
            Node currentNode = board.Node(position);
            Node targetNode = board.Node(targetPosition).isEmpty ? board.Node(targetPosition) : board.Node(targetPosition - 2 * dir);

            if (!targetNode.isEmpty)
            {
                return false;
            }

            position = targetNode.position;

            //laser toggle ahead!
            foreach (KeyValuePair<LaserToggle, Wall> laser in board.lasers)
            {
                //Trigger the laser's toggle
                if (position == laser.Key.position)
                    laser.Key.Action(laser.Value);
            }

            return true;
        }
    }
}
