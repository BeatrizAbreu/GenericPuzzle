using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Scripts
{
    public class Player
    {
        public Vector2 position;
        public int nMoves;
        public static bool hasLost;
        Board board;

        public Player(Board board, Vector2 position)
        {
            this.board = board;
            this.position = position;
            nMoves = 0;
        }

        public bool AutoPlay(List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, GameState gameState)
        {
            var keys = board.Node(position).neighbors.Keys.Shuffle().ToList();
            // Useful for debug
            // System.Console.WriteLine("Available movements: {0}", String.Join(",", keys.Select((k)=>k.ToString())));

            // Bangs the wall until a movement succeeds
            foreach (var movement in keys)
            {
                if (Move(movement)) return true;
            }
            return false;
        }

        public bool Move(Direction direction)
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

            if(!targetNode.isEmpty)
            {
                bool hasWall = true;

                foreach (Obstacle obstacle in board.obstacles)
                {
                    // Obstacle ahead!
                    if (obstacle.position == targetNode.position)
                    {
                        if(!obstacle.Move(direction))
                        return false;

                        hasWall = false;
                    }
                }

                if (hasWall)
                    return false;
            }

            // First we move
            nMoves++;

            //update position
            if (isTeleporting)
                position = teleportPos;
            else
                position = targetNode.position;

            //trigger movable enemy's movement
            foreach (var enemy in board.enemyObjects)
            {
                if (enemy.tag == "Movable")
                {
                    enemy.Action(board);
                    if (enemy.position == position)
                        enemy.Action();
                }
            }

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
