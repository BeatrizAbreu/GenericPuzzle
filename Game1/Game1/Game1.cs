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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D boardNodeTex;
        private Texture2D playerTex;

        //Board making information
        Random random = new Random();
        int maxHoles = 1;
        int width = 4;
        int height = 4;
        private Board board;

        //Player Info
        Player player;
        Node currentNode;

        //Objects information
        List<Obstacle> obstacles;
        List<WinObject> winObjects;

        //Keyboard
        KeyboardState previousState, state;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //setting first keyboard states
            state = Keyboard.GetState();
            previousState = state;

            //create a board with 0 to maxHoles random holes 
            board = new Board(width, height, random.Next(maxHoles));
            //create a player
            player = new Player();

            //player is placed on the first tile if it isn't a hole
            if (board.nodes[0, 0] != null)
            {
                player.position = board.nodes[0, 0].position;
                currentNode = board.nodes[0, 0];
            }            
            else
            {
                player.position = board.nodes[1, 0].position;
                currentNode = board.nodes[1, 0];
            }


            obstacles = new List<Obstacle>();
            winObjects = new List<WinObject>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            boardNodeTex = Content.Load<Texture2D>("assets/node");
            playerTex = Content.Load<Texture2D>("assets/player");

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            state = Keyboard.GetState();

            //Exit the game
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            //Player movement control
            if (state.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.D)) //RIGHT
             player.Walk(new Vector2(1, 0), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.A)) //LEFT
                player.Walk(new Vector2(-1, 0), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.W)) //UP
                player.Walk(new Vector2(0, -1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.S)) //DOWN
                player.Walk(new Vector2(0, 1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.E) && !previousState.IsKeyDown(Keys.E)) //UPRIGHT
                player.Walk(new Vector2(1, -1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.Q) && !previousState.IsKeyDown(Keys.Q)) //UPLEFT
                player.Walk(new Vector2(-1, -1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.X) && !previousState.IsKeyDown(Keys.X)) //DOWNRIGHT
                player.Walk(new Vector2(1, 1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.Z) && !previousState.IsKeyDown(Keys.Z)) //DOWNLEFT
                player.Walk(new Vector2(-1, 1), boardNodeTex.Width, boardNodeTex.Height, obstacles, winObjects, currentNode);

            previousState = state;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            //draw the board sprites
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //if the node isn't a hole
                    if (board.nodes[x, y] != null)
                        spriteBatch.Draw(boardNodeTex, new Vector2(board.nodes[x, y].position.X * boardNodeTex.Width, board.nodes[x, y].position.Y * boardNodeTex.Height), Color.White);
                    else
                    {
                        spriteBatch.Draw(boardNodeTex, new Vector2(x * boardNodeTex.Width, y * boardNodeTex.Height), Color.Black);
                    }
                }
            }
            //draw the player sprite
            spriteBatch.Draw(playerTex, player.position, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
