using Game1.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game1
{
    public class Game1 : Game
    {
        Dictionary<Keys, Direction> Moves;
        
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch { get; private set; }

        //Textures
        private Texture2D playerTex, boardNodeTex, boxTex, pressurePlateTex, spikeTex;

        //Board making information
        static int nHoles = 0;
        static int nBoxes = 1;
        static int nEnemies = 0;
        static int width = 5;
        static int height = 5;
        static int nDirections = 6;

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

        //Board from file
        private bool fileON = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            playsCount = lossCount = winCount = 0;
            new RNG(3);
            lossCount = winCount = 0;
            //setting first keyboard states
            state = Keyboard.GetState();
            previousState = state;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Player player;
            Board board;

            if (!fileON)
            {
                //Create a game board
                board = new HexaBoard(width, height, nHoles, nBoxes, nEnemies);

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
                currentGameState = new GameState(board, player);
            }
            else
            {
                board = LoadLevel(out player);
                foreach (Obstacle obs in board.obstacles)
                {
                    obs.board = board;
                }
            }

            board.boardInfo.nDirections = nDirections;
            Moves = board.GetKeysDirection(board.boardInfo.nDirections);
            treeRootMTCS = new NodeMCTS(currentGameState);

            base.Initialize();
        }

        protected override void LoadContent()
        {        
            GameTime gameTime = new GameTime();

            //Loading sprites
            boxTex           = Content.Load<Texture2D>("assets/box");
            spikeTex         = Content.Load<Texture2D>("assets/spike");
            playerTex        = Content.Load<Texture2D>("assets/player");
            boardNodeTex     = Content.Load<Texture2D>("assets/node");
            pressurePlateTex = Content.Load<Texture2D>("assets/pressurePlate");

            GameState sourceState = currentGameState;

            //MonteCarlo auto-player
            //for (int i = 0; i < 10; i++)
            //{
            //    currentGameState = sourceState.Copy();
            //    int result = currentGameState.PlayTest(500);

            //    if (result > 0) winCount++;
            //    if (result < 0) lossCount++;

            //    System.Console.WriteLine($"{winCount} vs {lossCount} ({currentGameState.player.nMoves})");
            //}
            treeRootMTCS.Iterate(treeRootMTCS, treeRootMTCS);
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
            // if (board.EvaluateVictory(currentGameState) != 0)
            // {
            //     if (board.EvaluateVictory(currentGameState) == -1)
            //     {
            //         lossCount++;
            //     }
            //     else
            //     {
            //         winCount++;
            //     }
            //     //drawCount = root.children.Count - root.lossCount - root.winCount;

            //     Respawn(gameTime);
            // }

            // else
            // {
            //     state = Keyboard.GetState();
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
            //     if (state.IsKeyDown(Keys.R) && !previousState.IsKeyDown(Keys.R))
            //     {
            //         RestartGame();
            //     }

            //     foreach (Keys k in Moves.Keys)
            //     {
            //         if (state.IsKeyDown(k) && !previousState.IsKeyDown(k))
            //             player.Move(Moves[k]);
            //     }

            //     previousState = state;
            // }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            Board board= currentGameState.board;

            foreach (Node node in currentGameState.board.nodes)
                if (node != null) spriteBatch.Draw(boardNodeTex, board.DrawPosition(node.position) * boardNodeTex.Height, Color.White);

            //draw the pressure plates' sprites
            foreach (WinObject winObject in currentGameState.winObjects)
                spriteBatch.Draw(pressurePlateTex, board.DrawPosition(winObject.position) * boardNodeTex.Height, Color.White);

            //draw the spikes' sprites
            foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
                spriteBatch.Draw(spikeTex, board.DrawPosition(enemyObject.position)* boardNodeTex.Height, Color.White);

            //draw the boxes' sprites
            foreach (Obstacle obstacle in currentGameState.obstacles)
            {
                bool occupied = false;
                foreach (WinObject winObject in currentGameState.winObjects)
                {
                    if (winObject.position == obstacle.position)
                    {
                        spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position)* boardNodeTex.Height, Color.Green);
                        occupied = true;
                        break;
                    }
                    else
                    {
                        spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position)* boardNodeTex.Height, Color.White);
                    }
                }
                if (!occupied)
                {
                    foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
                    {
                        if (enemyObject.position == obstacle.position)
                        {
                            spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position)* boardNodeTex.Height, Color.Red);
                            break;
                        }
                        else
                        {
                            spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position)* boardNodeTex.Height, Color.White);
                        }
                    }
                }
            }

            //draw the player sprite
            spriteBatch.Draw(playerTex, board.DrawPosition(currentGameState.player.position)* boardNodeTex.Height, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // public void RestartGame()
        // {
        //     Board board = currentGameState.board;
        //     //reset player position & the currentNode
        //     if (board[0, 0] != null)
        //     {
        //         currentGameState.player.position = board[0, 0].position;
        //     }
        //     else
        //     {
        //         currentGameState.player.position = board[1, 0].position;
        //     }

        //     board = RestartBoard(board);
        // }

        //Set the node states to the starter states
        // private static Board RestartBoard(Board board)
        // {
        //     //go through each node on the board
        //     for (int y = 0; y < height; y++)
        //     {
        //         for (int x = 0; x < width; x++)
        //         {
        //             //go through each obstacle's position
        //             for (int i = 0; i < obstacles.Count; i++)
        //             {
        //                 //if the node is not a hole
        //                 if (board[x, y] != null)
        //                 {
        //                     //if the node's position matches any of the first objects' position
        //                     if (board[x, y].position == obstacles[i].position)
        //                     {
        //                         //update the node's state to occupied
        //                         board[x, y].isEmpty = false;
        //                         break;
        //                     }
        //                     else
        //                     {
        //                         board[x, y].isEmpty = true;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     return board;
        // }

        // public void Respawn(GameTime gameTime)
        // {
        //     if (timerStartTime + spawnTimer < gameTime.TotalGameTime)
        //     {
        //         RestartGame();
        //         Player.hasLost = false;
        //         timerStartTime = gameTime.TotalGameTime;
        //     }
        // }

        Board LoadLevel(out Player player)
        {         
            string[] file = File.ReadAllLines(Content.RootDirectory + "/level.txt");
            width = file[0].Length;
            height = file.Length;
            List<Obstacle> obstacles = new List<Obstacle>();
            List<WinObject> winObjects = new List<WinObject>();
            List<EnemyObject> enemyObjects = new List<EnemyObject>();
            Vector2 playerPos = new Vector2();
            int holesCount = 0, boxCount = 0, enemyCount = 0;      

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Hole
                    if (file[i][j] == '#')
                    {
                        holesCount++;
                    }
                    //Box
                    else if (file[i][j] == 'B')
                    {
                        boxCount++;
                    }
                    //Enemy
                    else if (file[i][j] == 'X')
                    {
                        enemyCount++;
                    }
                }
            }

            Board tempBoard = new HexaBoard(width, height, holesCount, boxCount, enemyCount);
            Vector2[] holesPosition = new Vector2[holesCount];
            holesCount = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Player
                    if (file[y][x] == 'P')
                    {                      
                        playerPos = new Vector2(x, y);
                    }
                    //Box
                    else if (file[y][x] == 'B')
                    {                        
                        Box box = new Box(tempBoard)
                        {
                            position = new Vector2(x, y)
                        };
                        obstacles.Add(box);
                    }
                    //Pressure plate
                    else if (file[y][x] == '.')
                    {                       
                        PressurePlate pp = new PressurePlate();
                        pp.position = new Vector2(x, y);
                        winObjects.Add(pp);
                    }
                    //Hole
                    else if (file[y][x] == '#')
                    {
                        holesPosition[holesCount] = new Vector2(x, y);
                        holesCount++;
                    }
                    //Enemy
                    else if(file[y][x] == 'X')
                    {
                        Spike spike = new Spike();
                        spike.position = new Vector2(x, y);
                        enemyObjects.Add(spike);
                    }
                }
            }

            Board board = new HexaBoard(width, height, holesPosition, obstacles, enemyObjects, winObjects);
            player = new Player(board, playerPos);
            currentGameState = new GameState(board, player);
            return board;
        }
    }
}
