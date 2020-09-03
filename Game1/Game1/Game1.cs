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
        static int nBoxes = 2;
        static int nCollectibles = 0;
        static int nPortals = 0;
        static int nLasers = 0;
        static int nEnemies = 0;
        static int width = 4;
        static int height = 4;
        static int nDirections = 4;

        Vector2[] baseObstaclePos;
        public static bool isOctaboard = false;

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
        bool realPlayer = false;
        bool MCTSPlayer = true;
        bool randomPlayer = false;

        //Board from file
        private bool fileON = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            if (nDirections == 8)
                isOctaboard = true;

            playsCount = lossCount = winCount = 0;
            Board board;
            Player player;
            new RNG(4);
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
                if (nDirections == 6) //Hexaboard
                    board = new HexaBoard(width, height, nHoles, nBoxes, nCollectibles, nEnemies, nPortals, nLasers, nDirections, this);               
                else if(nDirections == 4) //Quadboard
                    board = new QuadBoard(width, height, nHoles, nBoxes, nCollectibles, nEnemies, nPortals, nLasers, nDirections, this);
                else //Octaboard
                    board = new OctaBoard(width, height, nHoles, nBoxes, nCollectibles, nEnemies, nPortals, nLasers, nDirections, this);

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

            Moves = board.GetKeysDirection(BoardInfo.nDirections);

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
                treeRootMTCS.Iterate(treeRootMTCS, treeRootMTCS, 0, 30);
                System.Console.WriteLine($"{treeRootMTCS.winCount} vs {treeRootMTCS.lossCount} ({treeRootMTCS.playsCount})");
            //    currentGameState = GetMCTSWinBoard(treeRootMTCS).gameState;
            }
        }

        //private NodeMCTS GetMCTSWinBoard(NodeMCTS root)
        //{
        //    NodeMCTS bestChild = new NodeMCTS();
        //    bestChild.winCount = -1;
        //    int count = 0;

        //    if (root.children != null)
        //    {
        //        foreach (var child in root.children)
        //        {
        //            if (bestChild.winCount == -1)
        //                bestChild = child.Value;
        //            else if (bestChild.winCount < child.Value.winCount)
        //                bestChild = child.Value;
        //            count++;

        //            if (count == root.children.Count)
        //            {
        //                bestChild = GetMCTSWinBoard(bestChild);
        //            }
        //        }
        //    }

        //    return bestChild;
        //}

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

            ////Respawn the player when he loses/ wins and give the info to the father node
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
            //GraphicsDevice.Clear(new Color(38, 38,38));
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            Board board = currentGameState.board;

            int height = board.nodeTexture.Height;

            //draw the board's nodes
            foreach (Node node in currentGameState.board.nodes)
            {
                if (node != null)
                {
                    if (!isOctaboard)
                        spriteBatch.Draw(board.nodeTexture, board.DrawPosition(node.position) * height, Color.White);
                    else
                    {
                        //octaboard & octa node
                        if ((node.position.X + node.position.Y) % 2 == 0)
                            spriteBatch.Draw(board.nodeTexture, board.DrawPosition(node.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                        else
                            spriteBatch.Draw(OctaBoard.quadTexture, board.DrawPosition(node.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                    }
                }
            }

            //draw the portals' sprites
            if(currentGameState.board.portals != null)
            foreach (var portal in currentGameState.board.portals)
            {
                if(!isOctaboard)
                {
                    spriteBatch.Draw(portal.texture1, board.DrawPosition(portal.pos1) * height, portal.color);
                    spriteBatch.Draw(portal.texture2, board.DrawPosition(portal.pos2) * height, portal.color);
                }              
                else
                {
                    spriteBatch.Draw(AssignOctaTetxure(portal.texture1, "Portal", portal.pos1), board.DrawPosition(portal.pos1) * (height / 2 + OctaBoard.quadTexture.Height / 2), portal.color);
                    spriteBatch.Draw(AssignOctaTetxure(portal.texture1, "Portal", portal.pos2), board.DrawPosition(portal.pos2) * (height / 2 + OctaBoard.quadTexture.Height / 2), portal.color);                   
                }
            }


            //draw the lasers' sprites
            if (currentGameState.board.lasers != null)
                foreach (var laser in currentGameState.board.lasers)
            {
                if (!isOctaboard)
                {
                    //toggle
                    spriteBatch.Draw(laser.Key.texture, board.DrawPosition(laser.Key.position) * height, laser.Value.color);
                    //laser
                    spriteBatch.Draw(laser.Value.texture, board.DrawPosition(laser.Value.position) * height, laser.Value.color);
                }

                else
                {
                    //toggle on & laser off
                    if (laser.Key.isTriggered)
                    {
                        spriteBatch.Draw(AssignOctaTetxure(laser.Key.texture, "LaserToggleOn", laser.Key.position),
                            board.DrawPosition(laser.Key.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), laser.Value.color);

                        spriteBatch.Draw(AssignOctaTetxure(laser.Value.texture, "LaserOff", laser.Value.position),
                            board.DrawPosition(laser.Value.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), laser.Value.color);
                    }

                    //toggle off & laser on
                    else
                    {
                        spriteBatch.Draw(AssignOctaTetxure(laser.Key.texture, "LaserToggleOff", laser.Key.position),
                            board.DrawPosition(laser.Key.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), laser.Value.color);

                        spriteBatch.Draw(AssignOctaTetxure(laser.Value.texture, "LaserOn", laser.Value.position),
                            board.DrawPosition(laser.Value.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), laser.Value.color);
                    }
                }
            }

            //draw the winobjects' sprites
            foreach (WinObject winObject in currentGameState.winObjects)
            {
                if (!winObject.isTriggered)
                {
                    if (!isOctaboard)
                        spriteBatch.Draw(winObject.texture, board.DrawPosition(winObject.position) * height, Color.White);
                    else
                        spriteBatch.Draw(AssignOctaTetxure(winObject.texture, winObject.tag, winObject.position), board.DrawPosition(winObject.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                }
            }

            //draw the spikes' sprites
            foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
            {
                if (enemyObject.tag == "Spike")
                {
                    if (!isOctaboard)
                        spriteBatch.Draw(enemyObject.texture, board.DrawPosition(enemyObject.position) * height, Color.White);
                    else
                        spriteBatch.Draw(AssignOctaTetxure(enemyObject.texture, "Spike", enemyObject.position), board.DrawPosition(enemyObject.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                }

                else
                {
                    if (!isOctaboard)
                        spriteBatch.Draw(playerTex, board.DrawPosition(enemyObject.position) * height, Color.Red);
                    else
                        spriteBatch.Draw(AssignOctaTetxure(playerTex, "Player", enemyObject.position), board.DrawPosition(enemyObject.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.Red);
                }
            }

            //draw the boxes' sprites
            foreach (Obstacle obstacle in currentGameState.obstacles)
            {
                bool occupied = false;
                foreach (WinObject winObject in currentGameState.winObjects)
                {
                    if (winObject.tag == "Toggle" && winObject.position == obstacle.position)
                    {
                        if (!isOctaboard)
                            spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position) * height, Color.Green);
                        else
                            spriteBatch.Draw(AssignOctaTetxure(obstacle.texture, "Box", obstacle.position), board.DrawPosition(obstacle.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.Green);

                        occupied = true;
                        break;
                    }
                    else
                    {
                        if (!isOctaboard)
                            spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position) * height, Color.White);
                        else
                            spriteBatch.Draw(AssignOctaTetxure(obstacle.texture, "Box", obstacle.position), board.DrawPosition(obstacle.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                    }
                }
                if (!occupied)
                {
                    foreach (EnemyObject enemyObject in currentGameState.enemyObjects)
                    {
                        if (enemyObject.position == obstacle.position)
                        {
                            if (!isOctaboard)
                                spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position) * height, Color.Red);
                            else
                                spriteBatch.Draw(AssignOctaTetxure(obstacle.texture, "Box", obstacle.position), board.DrawPosition(obstacle.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.Red);
                            break;
                        }
                        else
                        {
                            if (!isOctaboard)
                                spriteBatch.Draw(obstacle.texture, board.DrawPosition(obstacle.position) * height, Color.White);
                            else
                                spriteBatch.Draw(AssignOctaTetxure(obstacle.texture, "Box", obstacle.position), board.DrawPosition(obstacle.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);
                        }
                    }
                }
            }

            //draw the player sprite
            if (!isOctaboard)
                spriteBatch.Draw(playerTex, board.DrawPosition(currentGameState.player.position) * height, Color.White);
            else
                spriteBatch.Draw(AssignOctaTetxure(playerTex, "Player", currentGameState.player.position), board.DrawPosition(currentGameState.player.position) * (height / 2 + OctaBoard.quadTexture.Height / 2), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Texture2D AssignOctaTetxure(Texture2D texture, string type, Vector2 pos)
        {
            if (type == "Player" && (pos.X + pos.Y) % 2 == 0)
                return Content.Load<Texture2D>("assets/octaboard/playerOcta");

            if ((pos.X + pos.Y) % 2 == 0)
                return texture;

            else if (type == "Player")
            {
                return Content.Load<Texture2D>("assets/octaboard/playerOctaQuad");
            }

            else if (type == "Toggle")
            {
                return Content.Load<Texture2D>("assets/octaboard/toggleOctaQuad");
            }

            else if (type == "Collectible")
            {
                return Content.Load<Texture2D>("assets/octaboard/collectibleOctaQuad");
            }

            else if (type == "Spike")
            {
                return Content.Load<Texture2D>("assets/octaboard/spikeOctaQuad");
            }

            else if (type == "Portal")
            {
                return Content.Load<Texture2D>("assets/octaboard/portalOctaQuad");
            }

            else if (type == "LaserOn")
            {
                return Content.Load<Texture2D>("assets/octaboard/laserOnOctaQuad");
            }

            else if (type == "LaserOff")
            {
                return Content.Load<Texture2D>("assets/octaboard/laserOffOctaQuad");
            }

            else if (type == "LaserToggleOn")
            {
                return Content.Load<Texture2D>("assets/octaboard/lasertoggleOnOctaQuad");
            }

            else if (type == "LaserToggleOff")
            {
                return Content.Load<Texture2D>("assets/octaboard/lasertoggleOffOctaQuad");
            }

            else //if (type == "Box")
            {
                return Content.Load<Texture2D>("assets/octaboard/boxOctaQuad");
            }
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

            //reseting lasers' state
            foreach (var laser in currentGameState.board.lasers)
            {
                laser.Key.isTriggered = false;
                laser.Key.texture = laser.Key.textures[0];
                laser.Value.texture = laser.Value.textures[0];
                currentGameState.board.Node(laser.Value.position).isEmpty = false;
            }
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
            if (randomPlayer)
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

            Board tempBoard = new HexaBoard(width, height, holesCount, boxCount, enemyCount, nPortals, nLasers, nCollectibles, nDirections, this);
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
