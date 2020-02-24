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

        /* FIXME: 
        Dictionary<Keys, Vector2> Moves;
        */;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D boardNodeTex,
                          playerTex,
                          boxTex,
                          pressurePlateTex;

        //Board making information
        Random random = new Random();
        int nHoles,
            nBoxes = 3,
            width = 4,
            height = 4;
        private Board board;

        //Player Info
        Player player;
        Node currentNode;

        //Objects information
        List<Obstacle> obstacles; 
        List<WinObject> winObjects;
        Vector2[] baseObstaclePos;

        //Keyboard
        KeyboardState previousState, state;

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

            //create a board with 0 to maxHoles random holes 
            board = new Board(width, height, nHoles, nBoxes);

/* FIXME:
 Moves = board.GetDirections();
*/

            //player is placed on the first tile if it isn't a hole
            if (board.nodes[0, 0] != null)
            {
                //create a player
                player = new Player(board.nodes[0, 0].position);
                currentNode = board.nodes[0, 0];
            }            
            else
            {
                player = new Player(board.nodes[1, 0].position);
                currentNode = board.nodes[1, 0];
            }

            obstacles = board.obstacles;
            winObjects = board.winObjects;

            baseObstaclePos = new Vector2[obstacles.Count];

            for (int i = 0; i < obstacles.Count; i++)
            {
                baseObstaclePos[i] = obstacles[i].position;
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            boardNodeTex = Content.Load<Texture2D>("assets/node");
            playerTex = Content.Load<Texture2D>("assets/player");
            boxTex = Content.Load<Texture2D>("assets/box");
            pressurePlateTex = Content.Load<Texture2D>("assets/pressurePlate");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            state = Keyboard.GetState();

            //Exit the game
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            //Restart the game
            if (state.IsKeyDown(Keys.R) && !previousState.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

           /* FIXME:
            foreach (Keys k in Moves.Keys) {
                if (state.IsKeyDown(k) && !previousState.IsKeyDown(k))
                    currentNode = board.Move(currentNode, Moves[k]);
            }
            */

            //Player movement control
            if (state.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.D)) //DOWNRIGHT
                currentNode = player.Walk(new Vector2(1, 1), obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.A)) //DOWNLEFT
                currentNode = player.Walk(new Vector2(-1, 1), obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.W)) //UP
                currentNode = player.Walk(new Vector2(0, -1), obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.S)) //DOWN
                currentNode = player.Walk(new Vector2(0, 1), obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.E) && !previousState.IsKeyDown(Keys.E)) //UPRIGHT
                currentNode = player.Walk(new Vector2(1, -1), obstacles, winObjects, currentNode);
            if (state.IsKeyDown(Keys.Q) && !previousState.IsKeyDown(Keys.Q)) //UPLEFT
                currentNode = player.Walk(new Vector2(-1, -1), obstacles, winObjects, currentNode);
            //if (state.IsKeyDown(Keys.X) && !previousState.IsKeyDown(Keys.X)) //DOWNRIGHT
            //    currentNode = player.Walk(new Vector2(1, 1), obstacles, winObjects, currentNode);
            //if (state.IsKeyDown(Keys.Z) && !previousState.IsKeyDown(Keys.Z)) //DOWNLEFT
            //    currentNode = player.Walk(new Vector2(-1, 1), obstacles, winObjects, currentNode);

            previousState = state;
            player.position = currentNode.position;

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
                    float yDelta = x % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
                    Color colorDelta = y % 2 == 0 ? Color.White : Color.LightGray;

                    //if the node isn't a hole
                    if (board.nodes[x, y] != null)
                    {
                        spriteBatch.Draw(boardNodeTex,
                                new Vector2(board.nodes[x, y].position.X * boardNodeTex.Height,
                                            board.nodes[x, y].position.Y * boardNodeTex.Height + yDelta), colorDelta);
                    }
                    else
                    {
                        spriteBatch.Draw(boardNodeTex,
                                new Vector2(x * boardNodeTex.Height, y * boardNodeTex.Height + yDelta), Color.Black);
                    }
                }
            }

            //draw the pressure plates' sprites
            foreach (WinObject winObject in winObjects)
            {
                float yDelta = winObject.position.X % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
                spriteBatch.Draw(pressurePlateTex, new Vector2(winObject.position.X * boardNodeTex.Height, winObject.position.Y * boardNodeTex.Height + yDelta), Color.White);
            }

            //draw the boxes' sprites
            foreach (Obstacle obstacle in obstacles)
            {
                float yDelta = obstacle.position.X % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
                spriteBatch.Draw(boxTex, new Vector2(obstacle.position.X * boardNodeTex.Height, obstacle.position.Y * boardNodeTex.Height + yDelta), Color.White);
            }

            //draw the player sprite
            {
                float yDelta = player.position.X % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
                spriteBatch.Draw(playerTex, new Vector2(player.position.X * boardNodeTex.Height, player.position.Y * boardNodeTex.Height + yDelta), Color.White);

            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void RestartGame()
        {
            //reset player position & the currentNode
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
                        if (board.nodes[x, y] != null)
                        {
                            //if the node's position matches any of the first objects' position
                            if (board.nodes[x, y].position == baseObstaclePos[i])
                            {
                                //update the node's state to occupied
                                board.nodes[x, y].isEmpty = false;
                                break;
                            }
                            else
                            {
                                board.nodes[x, y].isEmpty = true;
                            }
                        }
                    }
                }
            }
            return board;
        }
    }
}
