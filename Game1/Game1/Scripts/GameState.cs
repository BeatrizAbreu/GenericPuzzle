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
        Node[,] nodes;
        Player player;
        List<Obstacle> obstacles;
        List<EnemyObject> enemyObjects;
        public List<WinObject> winObjects;
        public bool beenVisited;

        public GameState(Node[,] nodes, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, Player player)
        {
            this.nodes = nodes;
            this.obstacles = obstacles;
            this.enemyObjects = enemyObjects;
            this.winObjects = winObjects;
            this.player = player;
            beenVisited = false;
        }

        public void PlayTest(GameTime gameTime, Board board)
        {
            while (true)
            {
                //Respawn the player when he loses/wins and give the info to the father node
                if (board.EvaluateVictory(this) != 0)
                {
                    if (board.EvaluateVictory(this) == -1)
                    {
                        Game1.lossCount++;
                    }
                    else
                    {
                        Game1.winCount++;
                    }
                    //drawCount = root.children.Count - root.lossCount - root.winCount;

                    Game1.Respawn(gameTime);
                }

                if (Game1.playsCount == 1000)
                {
                    Console.WriteLine(Game1.playsCount + "  :" + Game1.winCount + " vs " + Game1.lossCount + " / player: " + player.position);
                    break;
                }

                else
                {
                    GameState tempGameState = this;
                    //plays 500 times or until it loses/wins
                    for (int i = 0; i < 500; i++)
                    {
                        //  timerStartTime = gameTime.TotalGameTime;
                        if (board.EvaluateVictory(this) != 0 || Game1.movesCount >= 500)
                        {
                            break;
                        }

                        //Autoplay
                        // if (timerStartTime + timer < gameTime.TotalGameTime)
                        //{
                            //if the player moved
                            if (player.AutoPlay(obstacles, enemyObjects, winObjects, ref tempGameState))
                            {
                                this.nodes = tempGameState.nodes;
                                this.player = tempGameState.player;
                                this.obstacles = tempGameState.obstacles;
                                this.enemyObjects = tempGameState.enemyObjects;
                                this.winObjects = tempGameState.winObjects;
                                this.beenVisited = tempGameState.beenVisited;

                                Game1.movesCount++;
                            }
                       // }
                    }
                }

                if (Game1.movesCount >= 500 || board.EvaluateVictory(this) != 0)
                {
                    Game1.playsCount++;
                    Game1.movesCount = 0;
                }
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
}
