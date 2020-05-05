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
            foreach (var movement in keys) {
                if (Move(movement)) return true;
            }
            return false;
        }

        public bool Move(Direction direction)
        {                        
            Node currentNode = board.Node(position);
            Node targetNode = board.Move(currentNode, direction);

            if (currentNode == targetNode) 
                return false;

            foreach (Obstacle obstacle in board.obstacles)
            {
                // Obstacle ahead!
                if (obstacle.position == targetNode.position && !obstacle.Move(direction))
                {
                    return false;                        
                }              
            }
            
            // First we move
            nMoves++;
            position = targetNode.position;

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
                if((currentNode.position.X + currentNode.position.Y) % 2 != targetNode.position.X + targetNode.position.Y % 2)
                {
                    BoardInfo.nDirections = BoardInfo.nDirections == 8 ? 4 : 8;
                }
            }

            return true;
        }
    }
}
