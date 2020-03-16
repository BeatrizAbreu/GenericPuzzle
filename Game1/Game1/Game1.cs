using Game1.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        Dictionary<Keys, Direction> Moves;
        
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch { get; private set; }

        //Textures
        private Texture2D playerTex;
        private Texture2D boxTex;
        private Texture2D pressurePlateTex;
        private Texture2D spikeTex;

        //Board making information
        static int nHoles = 2;
        static int nBoxes = 5;
        static int nEnemies = 2;
        static int width = 9;
        static int height = 5;
        static int nDirections = 6;
        private static Board board;

        //Player Info
        private static Player player;

        //Objects information
        static List<Obstacle> obstacles;
        static List<WinObject> winObjects;
        static List<EnemyObject> enemyObjects;
        static Vector2[] baseObstaclePos;

        //Keyboard
        KeyboardState previousState, state;

        //Time
        public static readonly TimeSpan timer = TimeSpan.FromMilliseconds(300);
        public static readonly TimeSpan spawnTimer = TimeSpan.FromMilliseconds(1500);
        public static TimeSpan timerStartTime;

        //MCTS
        GameState currentGameState;
        NodeMCTS treeRootMTCS;
        public static int lossCount, winCount;
        public static int playsCount;
        public static int movesCount = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            playsCount = lossCount = winCount = 0;
            new RNG(2);
            lossCount = winCount = 0;
            //setting first keyboard states
            state = Keyboard.GetState();
            previousState = state;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //create a board with 0 to maxHoles random holes 
            board = new HexaBoard(this, width, height, nHoles, nBoxes, nEnemies);

            board.boardInfo.nDirections = nDirections;

            Moves = board.GetKeysDirection(board.boardInfo.nDirections);
            

            //player is placed on the first tile if it isn't a hole
            if (board[0, 0] != null)
            {
                //create a player
                player = new Player(board, board[0, 0].position);
            }
            else
            {
                player = new Player(board, board[1, 0].position);
            }

            obstacles = board.obstacles;
            winObjects = board.winObjects;
            enemyObjects = board.enemyObjects;

            baseObstaclePos = new Vector2[obstacles.Count];

            for (int i = 0; i < obstacles.Count; i++)
            {
                baseObstaclePos[i] = obstacles[i].position;
            }

            currentGameState = new GameState(board.nodes, obstacles, enemyObjects, winObjects, player);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameTime gameTime = new GameTime();

            //Loading sprites
            playerTex = Content.Load<Texture2D>("assets/player");
            boxTex = Content.Load<Texture2D>("assets/box");
            pressurePlateTex = Content.Load<Texture2D>("assets/pressurePlate");
            spikeTex = Content.Load<Texture2D>("assets/spike");

            //MonteCarlo auto-player
           // currentGameState.PlayTest(gameTime, board);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //Exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ////Respawn the player when he loses/wins and give the info to the father node
            if (board.EvaluateVictory(currentGameState) != 0)
            {
                if (board.EvaluateVictory(currentGameState) == -1)
                {
                    lossCount++;
                }
                else
                {
                    winCount++;
                }
                //drawCount = root.children.Count - root.lossCount - root.winCount;

                Respawn(gameTime);
            }

            else
            {
                state = Keyboard.GetState();
                //MonteCarloTreeSearch(gameTime, treeRootMTCS);
                
                ////plays 500 times or until it loses/wins
                //for (int i = 0; i < 500; i++)
                //{
                //    //Autoplay
                //   // if (timerStartTime + timer < gameTime.TotalGameTime)
                //    {
                //        //if the player moved
                //        if(player.AutoPlay(obstacles, enemyObjects, winObjects, ref currentGameState))
                //        {
                //            //  timerStartTime = gameTime.TotalGameTime;
                //            if (Math.Abs(board.EvaluateVictory(currentGameState)) == 1)
                //            {
                //                break;
                //            }
                //            movesCount++;
                //        }
                //    }
                //}

                //if (movesCount == 500 || Math.Abs(board.EvaluateVictory(currentGameState)) == 1)
                //{
                //    playsCount++;
                //    movesCount = 0;
                //}

                //if(playsCount == 100)
                //{
                //    Console.WriteLine(playsCount + "  :" + winCount + " vs " + lossCount);
                //}

                //Restart the game upon clicking R
                if (state.IsKeyDown(Keys.R) && !previousState.IsKeyDown(Keys.R))
                {
                    RestartGame();
                }

                foreach (Keys k in Moves.Keys)
                {
                    if (state.IsKeyDown(k) && !previousState.IsKeyDown(k))
                        player.Move(Moves[k]);
                }

                previousState = state;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            board.Draw(gameTime);

            //draw the pressure plates' sprites
            foreach (WinObject winObject in winObjects)
                spriteBatch.Draw(pressurePlateTex, board.DrawPosition(winObject.position), Color.White);

            //draw the spikes' sprites
            foreach (EnemyObject enemyObject in enemyObjects)
                spriteBatch.Draw(spikeTex, board.DrawPosition(enemyObject.position), Color.White);

            //draw the boxes' sprites
            foreach (Obstacle obstacle in obstacles)
            {
                bool occupied = false;
                foreach (WinObject winObject in winObjects)
                {
                    if (winObject.position == obstacle.position)
                    {
                        spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position), Color.Green);
                        occupied = true;
                        break;
                    }
                    else
                    {
                        spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position), Color.White);
                    }
                }
                if (!occupied)
                {
                    foreach (EnemyObject enemyObject in enemyObjects)
                    {
                        if (enemyObject.position == obstacle.position)
                        {
                            spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position), Color.Red);
                            break;
                        }
                        else
                        {
                            spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position), Color.White);
                        }
                    }
                }
            }

            //draw the player sprite
            spriteBatch.Draw(playerTex, board.DrawPosition(player.position), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void RestartGame()
        {
            //reset player position & the currentNode
            if (board[0, 0] != null)
            {
                player.position = board[0, 0].position;
            }
            else
            {
                player.position = board[1, 0].position;
            }

            //reseting objects' position
            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].position = baseObstaclePos[i];
            }

            board = RestartBoard();
        }

        //Set the node states to the starter states
        private static Board RestartBoard()
        {
            //go through each node on the board
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //go through each obstacle's position
                    for (int i = 0; i < baseObstaclePos.Length; i++)
                    {
                        //if the node is not a hole
                        if (board[x, y] != null)
                        {
                            //if the node's position matches any of the first objects' position
                            if (board[x, y].position == baseObstaclePos[i])
                            {
                                //update the node's state to occupied
                                board[x, y].isEmpty = false;
                                break;
                            }
                            else
                            {
                                board[x, y].isEmpty = true;
                            }
                        }
                    }
                }
            }
            return board;
        }

        public static void Respawn(GameTime gameTime)
        {
            if (timerStartTime + spawnTimer < gameTime.TotalGameTime)
            {
                RestartGame();
                Player.hasLost = false;
                timerStartTime = gameTime.TotalGameTime;
            }
        }
    }
}
