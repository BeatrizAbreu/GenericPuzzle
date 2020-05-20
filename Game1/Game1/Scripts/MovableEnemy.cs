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
        bool xAxis;
        public string tag;

        public MovableEnemy(Vector2 source, int distance, string tag, bool xAxis)
        {
            this.source = source;
            currentPosition = source;
            this.distance = distance;
            this.tag = tag;
            this.xAxis = xAxis;
        }

        public override void Action(Board board)
        {
            Vector2 destination = currentPosition;

            Move(board);
        }

        public bool Move(Board board)
        {
            Node currentNode = board.Node(position);
            Node targetNode = board.Move(currentNode, direction);
            bool isTeleporting = false;
            Vector2 teleportPos = new Vector2();

            if (currentNode == targetNode)
                return false;

            foreach (Portal portal in board.portals)
            {
                // Portal ahead!
                if (portal.pos1 == targetNode.position || portal.pos2 == targetNode.position)
                {
                    Vector2 dir = targetNode.position - currentNode.position;

                    if (BoardInfo.nDirections == 6)
                    {
                        if ((targetNode.position.X % 2 == 0 && currentNode.position.X % 2 != 0)
                            || (targetNode.position.X % 2 != 0 && currentNode.position.X % 2 == 0))
                            dir.Y += 1;
                    }

                    teleportPos = portal.Trigger(this.position, dir, targetNode.position);

                    if (board.Node(teleportPos) == null)
                        return false;

                    targetNode = board.Node(teleportPos);
                    isTeleporting = true;
                    break;
                }
            }

            if (!targetNode.isEmpty)
            {
                return false;
            }

            if (isTeleporting)
                position = teleportPos;
            else
                position = targetNode.position;

            //laser toggle ahead!
            foreach (KeyValuePair<LaserToggle, Wall> laser in board.lasers)
            {
                //Trigger the laser's toggle
                if (position == laser.Key.position)
                    laser.Key.Action(laser.Value);
            }

            // then we might die
            // find the next node's enemy
            foreach (EnemyObject enemyObj in board.enemyObjects)
            {
                //find the obstacle that's in the neighbor
                if (enemyObj.position == position)
                {
                    // kill the player
                    enemyObj.Action();
                    break;
                }
            }

            foreach (WinObject winObject in board.winObjects)
            {
                //Collectible ahead!
                if (winObject.tag == "Collectible" && winObject.position == position)
                {
                    winObject.Action();
                }
            }

            //we're in a octaboard
            if (Game1.isOctaboard)
            {
                //change nDirections if needed
                if ((currentNode.position.X + currentNode.position.Y) % 2 != targetNode.position.X + targetNode.position.Y % 2)
                {
                    BoardInfo.nDirections = BoardInfo.nDirections == 8 ? 4 : 8;
                }
            }

            return true;
        }
    }
}
