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
        /* FIXME: */
        Dictionary<Keys, Vector2> Moves;
        
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch { get; private set; }

        private Texture2D playerTex;
        private Texture2D boxTex;
        private Texture2D pressurePlateTex;
        private Texture2D spikeTex;

        //Board making information
        Random random = new Random();
        int nHoles;
        int nBoxes = 5;
        int nEnemies = 2;
        int width = 9;
        int height = 5;
        int nDirections = 6;
        private Board board;

        //Player Info
        Player player;
        Node currentNode;

        //Objects information
        List<Obstacle> obstacles; 
        List<WinObject> winObjects;
        List<EnemyObject> enemyObjects;
        Vector2[] baseObstaclePos;

        //Keyboard
        KeyboardState previousState, state;

        //Time
        private static readonly TimeSpan timer = TimeSpan.FromMilliseconds(300);
        private static readonly TimeSpan fastTimer = TimeSpan.FromMilliseconds(100);
        private TimeSpan timerStartTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            nHoles = (int)width / 4;

            //setting first keyboard states
            state = Keyboard.GetState();
            previousState = state;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //create a board with 0 to maxHoles random holes 
            board = new HexaBoard(this, width, height, nHoles, nBoxes, nEnemies);

            board.boardInfo.nDirections = nDirections;

            /* FIXME: */
            Moves = board.GetKeysDirection(board.boardInfo.nDirections);
            

            //player is placed on the first tile if it isn't a hole
            if (board[0, 0] != null)
            {
                //create a player
                player = new Player(board[0, 0].position);
                currentNode = board[0, 0];
            }
            else
            {
                player = new Player(board[1, 0].position);
                currentNode = board[1, 0];
            }

            obstacles = board.obstacles;
            winObjects = board.winObjects;
            enemyObjects = board.enemyObjects;

            baseObstaclePos = new Vector2[obstacles.Count];

            for (int i = 0; i < obstacles.Count; i++)
            {
                baseObstaclePos[i] = obstacles[i].position;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            playerTex = Content.Load<Texture2D>("assets/player");
            boxTex = Content.Load<Texture2D>("assets/box");
            pressurePlateTex = Content.Load<Texture2D>("assets/pressurePlate");
            spikeTex = Content.Load<Texture2D>("assets/spike");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Player.hasLost)
            {
                if (timerStartTime + fastTimer < gameTime.TotalGameTime)
                {
                    RestartGame();
                    Player.hasLost = false;
                    timerStartTime = gameTime.TotalGameTime;
                }                
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            state = Keyboard.GetState();

            //Autoplay
            if (timerStartTime + timer < gameTime.TotalGameTime)
            {
                currentNode = board.AutoPlay(currentNode);
                timerStartTime = gameTime.TotalGameTime;
            }

            //Exit the game
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            //Restart the game
            if (state.IsKeyDown(Keys.R) && !previousState.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            /* FIXME: */
             foreach (Keys k in Moves.Keys) {
                 if (state.IsKeyDown(k) && !previousState.IsKeyDown(k))
                     currentNode = board.Move(currentNode, Moves[k]);
             }
             
            previousState = state;
            player.position = currentNode.position;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            board.Draw(gameTime);

            //draw the pressure plates' sprites
            foreach (WinObject winObject in winObjects)
                spriteBatch.Draw(pressurePlateTex, board.DrawPosition(winObject.position), Color.White);

            //draw the boxes' sprites
            foreach (Obstacle obstacle in obstacles)
                spriteBatch.Draw(boxTex, board.DrawPosition(obstacle.position), Color.White);

            //draw the spikes' sprites
            foreach (EnemyObject enemyObject in enemyObjects)
                spriteBatch.Draw(spikeTex, board.DrawPosition(enemyObject.position), Color.White);

            //draw the player sprite
            spriteBatch.Draw(playerTex, board.DrawPosition(player.position), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RestartGame()
        {
            //reset player position & the currentNode
            if (board[0, 0] != null)
            {
                player.position = board[0, 0].position;
                currentNode = board[0, 0];
            }
            else
            {
                player.position = board[1, 0].position;
                currentNode = board[1, 0];
            }

            //reseting objects' position
            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].position = baseObstaclePos[i];
            }

            board = RestartBoard();
        }

        //Set the node states to the starter states
        private Board RestartBoard()
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
    }
}
