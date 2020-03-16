using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class GameState
    {
        public Board board  {get; private set;}
        public Player player {get; private set;}
        public List<Obstacle>    obstacles     {get; private set;}
        public List<EnemyObject> enemyObjects  {get; private set;}
        public List<WinObject>   winObjects    {get; private set;}

        public GameState(Board board, Player player)
        {
            this.player = player;
            this.board  = board;
            this.obstacles    = board.obstacles;
            this.enemyObjects = board.enemyObjects;
            this.winObjects   = board.winObjects;
        }

        public int PlayTest(int plays)
        {
            int victory = 0;
            for (int i = 0; i < plays; i++)
            {
                if (!player.AutoPlay(obstacles, enemyObjects, winObjects, this))
                    break; // no possible movement

                victory = board.EvaluateVictory(this); 
                if (victory != 0)
                    break; // win
            }

            return victory; 
        }
         

            //    public void MonteCarloTreeSearch(GameTime gameTime, NodeMCTS root, Board board)
            //{
            //    //go through the three's nodes
            //    foreach (NodeMCTS node in root.children)
            //    {
            //        //if the node is not a leaf, keep going through the children
            //        if (node.children != null)
            //        {
            //            MonteCarloTreeSearch(gameTime, node, board);
            //        }

            //        //found a leaf
            //        else
            //        {
            //            //Respawn the player when he loses/wins and give the info to the father node
            //            if (Math.Abs(board.EvaluateVictory(this)) == 1)
            //            {
            //                if (board.EvaluateVictory(this) == -1)
            //                {
            //                    Game1.lossCount++;
            //                }
            //                else
            //                {
            //                    Game1.winCount++;
            //                }
            //                //drawCount = root.children.Count - root.lossCount - root.winCount;

            //                Respawn(gameTime);
            //            }

            //            else
            //            {
            //                GameState tempGameState = this;
            //                //plays 500 times or until it loses/wins
            //                for (int i = 0; i < 500; i++)
            //                {
            //                    //Autoplay
            //                    // if (timerStartTime + timer < gameTime.TotalGameTime)
            //                    {
            //                        //if the player moved
            //                        if (player.AutoPlay(obstacles, enemyObjects, winObjects, ref tempGameState))
            //                        {
            //                            this.nodes = tempGameState.nodes;
            //                            this.player = tempGameState.player;
            //                            this.obstacles = tempGameState.obstacles;
            //                            this.enemyObjects = tempGameState.enemyObjects;
            //                            this.winObjects = tempGameState.winObjects;
            //                            this.beenVisited = tempGameState.beenVisited;

            //                            //  timerStartTime = gameTime.TotalGameTime;
            //                            if (Math.Abs(board.EvaluateVictory(this)) == 1)
            //                            {
            //                                break;
            //                            }
            //                            Game1.movesCount++;
            //                        }
            //                    }
            //                }

            //                if (Game1.movesCount == 500 || Math.Abs(board.EvaluateVictory(this)) == 1)
            //                {
            //                    Game1.playsCount++;
            //                    Game1.movesCount = 0;
            //                }

            //                if (Game1.playsCount == 100)
            //                {
            //                    Console.WriteLine(Game1.playsCount + "  :" + Game1.winCount + " vs " + Game1.lossCount);
            //                }

            //                //plays 500 times or until it loses / wins
            //                //for (int i = 0; i < 500; i++)
            //                //{
            //                //    Autoplay
            //                //if (timerStartTime + timer < gameTime.TotalGameTime)
            //                //    {
            //                //        Respawn the player when he loses/ wins and give the info to the father node
            //                //    if (Math.Abs(board.EvaluateVictory(currentGameState)) == 1)
            //                //        {
            //                //            if (board.EvaluateVictory(currentGameState) == -1)
            //                //            {
            //                //                root.lossCount++;
            //                //            }
            //                //            else
            //                //            {
            //                //                root.winCount++;
            //                //            }
            //                //            drawCount = root.children.Count - root.lossCount - root.winCount;

            //                //            Respawn(gameTime);
            //                //            node.gameState = currentGameState;
            //                //            break;
            //                //        }

            //                //        currentGameState = player.AutoPlay(obstacles, enemyObjects, winObjects);
            //                //        timerStartTime = gameTime.TotalGameTime;
            //                //    }
            //                //}
            //            }
            //        }
            //    }
        }
    }
