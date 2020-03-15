using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class Player
    {
        public Vector2 position;
        public static int nMoves;
        public static bool hasLost;
        Board board;
        public Player(Board board, Vector2 position)
        {
            this.board = board;
            this.position = position;
            nMoves = 0;
        }

        public GameState AutoPlay(List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects)
        {
            Move(board.Node(position).neighbors.Keys.Shuffle().First());
            // FIXME: mais cedo ou mais tarde, validar se o Move retornou false, e nesse caso, descartar o movimento
            //        talvez colocar a funcao a retornar o GameState como um parametro out, e retornar um booleano tb
            //        se booleano é falso, o GameState é inexistente...
            GameState gameState = new GameState(board.nodes, obstacles, enemyObjects, winObjects, this);
            return gameState;
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
                if (obstacle.position == targetNode.position)
                {
                    // if the object is a box 
                    if (!targetNode.isEmpty)
                    {
                       if (obstacle.position == new Vector2(0, 2))
                           Console.Write(direction + " " + targetNode.isEmpty);
                        if (!obstacle.Move(direction))
                        {
                            return false;                        
                        }
                    }
                }              
            }
            
            // First we move
            Player.nMoves++;
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
                }
            }
       
            
            return true;
        }
    }
}
