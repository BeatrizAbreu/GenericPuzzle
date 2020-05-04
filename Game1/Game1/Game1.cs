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
        private Texture2D playerTex;

        //Board making information
        static int nHoles = 0;
        static int nBoxes = 3;
        static int nCollectibles = 3;
        static int nEnemies = 2;
        static int width = 5;
        static int height = 5;
        static int nDirections = 6;
        Vector2[] baseObstaclePos;

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

        //Player type
        bool realPlayer = true;
        bool MCTSPlayer = false;
        bool randomPlayer = false;

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
            Board board;
            Player player;
            new RNG(2);
            lossCount = winCount = 0;
            //setting first keyboard states
            state = Keyboard.GetState();
            previousState = state;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (fileON)
            {
                board = LoadLevel(out player);
                foreach (Obstacle obs in board.obstacles)
                {
                    obs.board = board;
                }
            }
            else
            {
                //Create a game board
                board = new HexaBoard(width, height, nHoles, nBoxes, nEnemies, nCollectibles, nDirections, this);
                
                if(nDirections == 4)
                    board = new QuadBoard(width, height, nHoles, nBoxes, nEnemies, nCollectibles, nDirections, this);

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

            Moves = board.GetKeysDirection(board.boardInfo.nDirections);

            if(MCTSPlayer)
               treeRootMTCS = new NodeMCTS(currentGameState);

            baseObstaclePos = new Vector2[board.obstacles.Count];

            for (int i = 0; i < board.obstacles.Count; i++)
            {
                baseObstaclePos[i] = board.obstacles[i].position;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {        
            GameTime gameTime = new GameTime();           
            playerTex = Content.Load<Texture2D>("assets/player");

            GameState sourceState = currentGameState;

            if(MCTSPlayer)
            {
                treeRootMTCS.Iterate(treeRootMTCS, treeRootMTCS, 0, 10);
                System.Console.WriteLine($"{treeRootMTCS.winCount} vs {treeRootMTCS.lossCount} ({treeRootMTCS.playsCount})");
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //Exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Restart the game upon clicking R
            if (state.IsKeyDown(Keys.R) && !previousState.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            //Respawn the player when he loses/wins and give the info to the father node
            if (currentGameState.board.EvaluateVictory(currentGameState) != 0)
            {
                if (currentGameState.board.EvaluateVictory(currentGameState) == -1)
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

            if (realPlayer)
            {               
                foreach (Keys k in Moves.Keys)
                {
                    if (state.IsKeyDown(k) && !previousState.IsKeyDown(k))
                        currentGameState.player.Move(Moves[k]);
                }

                previousState = state;
                state = Keyboard.GetState();
            }        
       
            if(randomPlayer)
            {
                //plays 500 times or until it loses/wins
                for (int i = 0; i < 500; i++)
                {
                    //Autoplay
                    if (timerStartTime + timer < gameTime.TotalGameTime)
                    {
                        //if the player moved
                        if (currentGameState.player.AutoPlay(currentGameState.obstacles, currentGameState.enemyObjects, currentGameState.winObjects, currentGameState))
                        {
                            timerStartTime = gameTime.TotalGameTime;
                            movesCount++;
                            if (Math.Abs(currentGameState.board.EvaluateVictory(currentGameState)) != 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            Board board= currentGameState.board;

            int height = board.nodeTexture.Height;

            //draw the board's nodes
            foreach (Node node in currentGameState.board.nodes)
                if (node != null)
                    spriteBatch.Draw(board.nodeTexture, board.DrawPosition(node.position) * height, Color.White);

            //draw the winobjects' sprites
            foreach (WinObject winObject in currentGameState.winObjects)
            {
                if (!winObject.isTriggered)
                    spriteBatch.Draw(winObject.texture, board.DrawPosition(winObject.position) * height, Color.White);
            }

            //draw the spikes' sprites
            foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
                spriteBatch.Draw(enemyObject.texture, board.DrawPosition(enemyObject.position)* height, Color.White);

            //draw the boxes' sprites
            foreach (Obstacle obstacle in currentGameState.obstacles)
            {
                bool occupied = false;
                foreach (WinObject winObject in currentGameState.winObjects)
                {
                    if (winObject.tag == "Toggle" && winObject.position == obstacle.position)
                    {
                        spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position)* height, Color.Green);
                        occupied = true;
                        break;
                    }
                    else
                    {
                        spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position)* height, Color.White);
                    }
                }
                if (!occupied)
                {
                    foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
                    {
                        if (enemyObject.position == obstacle.position)
                        {
                            spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position)* height, Color.Red);
                            break;
                        }
                        else
                        {
                            spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position)* height, Color.White);
                        }
                    }
                }
            }

            //draw the player sprite
            spriteBatch.Draw(playerTex, board.DrawPosition(currentGameState.player.position)* board.nodeTexture.Height, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void RestartGame()
        {
            //reset player position & the currentNode
            if (currentGameState.board[0, 0] != null)
            {
                currentGameState.player.position = currentGameState.board[0, 0].position;
            }
            else
            {
                currentGameState.player.position = currentGameState.board[1, 0].position;
            }

            //reseting objects' position
            for (int i = 0; i < currentGameState.board.obstacles.Count; i++)
            {
                currentGameState.board.obstacles[i].position = baseObstaclePos[i];
            }

            currentGameState.board = RestartBoard(currentGameState.board);
        }

        //Set the node states to the starter states
        private Board RestartBoard(Board board)
        {
            //go through each node on the board
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //go through each obstacle's position
                    for (int i = 0; i < board.obstacles.Count; i++)
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

            foreach (WinObject winObj in board.winObjects)
            {
                winObj.isTriggered = false;
            }

            return board;
        }

        public void Respawn(GameTime gameTime)
        {
            if(randomPlayer)
            {
                RestartGame();
                Player.hasLost = false;
            }
            else if(timerStartTime + spawnTimer < gameTime.TotalGameTime)
            {
                RestartGame();
                Player.hasLost = false;
                timerStartTime = gameTime.TotalGameTime;
            }      
        }

        Board LoadLevel(out Player player)
        {         
            string[] file = File.ReadAllLines(Content.RootDirectory + "/level.txt");
            width = file[0].Length;
            height = file.Length;
            List<Obstacle> obstacles = new List<Obstacle>();
            List<WinObject> winObjects = new List<WinObject>();
            List<EnemyObject> enemyObjects = new List<EnemyObject>();
            Vector2 playerPos = new Vector2();
            int holesCount = 0, boxCount = 0, enemyCount = 0, collectibleCount = 0;      

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
                    else if (file[i][j] == 'E')
                    {
                        enemyCount++;
                    }
                    //Collectible
                    else if (file[i][j] == 'C')
                    {
                        collectibleCount++; 
                    }
                }
            }

            Board tempBoard = new HexaBoard(width, height, holesCount, boxCount, enemyCount, nCollectibles, nDirections, this);
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
                        Box box = new Box(tempBoard, this)
                        {
                            position = new Vector2(x, y)
                        };
                        obstacles.Add(box);
                    }
                    //Toggle
                    else if (file[y][x] == 'T')
                    {                       
                        Toggle toggle = new Toggle(this);
                        toggle.position = new Vector2(x, y);
                        winObjects.Add(toggle);
                    }
                    //Collectible
                    else if (file[y][x] == 'C')
                    {
                        Collectible collectible = new Collectible(this);
                        collectible.position = new Vector2(x, y);
                        winObjects.Add(collectible);
                    }
                    //Hole
                    else if (file[y][x] == '#')
                    {
                        holesPosition[holesCount] = new Vector2(x, y);
                        holesCount++;
                    }
                    //Enemy
                    else if(file[y][x] == 'E')
                    {
                        Spike spike = new Spike(this);
                        spike.position = new Vector2(x, y);
                        enemyObjects.Add(spike);
                    }
                }
            }

            Board board = new HexaBoard(width, height, holesPosition, obstacles, enemyObjects, winObjects, nDirections, this);

            if (nDirections == 4)
                board = new QuadBoard(width, height, holesPosition, obstacles, enemyObjects, winObjects, nDirections, this);

            player = new Player(board, playerPos);
            currentGameState = new GameState(board, player);
            return board;
        }
    }
}
