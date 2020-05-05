using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class OctaBoard : Board
    {
        public static Texture2D quadTexture;

        public OctaBoard(int width, int height, int nHoles, int nBoxes, int nEnemies, int nCollectibles, int nDirections, Game1 game) : base(width, height, nHoles, nBoxes, nEnemies, nCollectibles, nDirections, game)
        {
            nodeTexture = game.Content.Load<Texture2D>("assets/octanode");
            quadTexture = game.Content.Load<Texture2D>("assets/octaquadnode2");
        }

        public OctaBoard(int width, int height, Vector2[] holesPosition, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, int nDirections, Game1 game)
            : base(width, height, holesPosition, obstacles, enemyObjects, winObjects, nDirections, game)
        {
            nodeTexture = game.Content.Load<Texture2D>("assets/octanode");
            quadTexture = game.Content.Load<Texture2D>("assets/octaquadnode2");
        }

        internal override void CreateNeighbors()
        {
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    Node node = this[x, y];
                    if (node == null) continue;

                    //QuadNode
                    //UP
                    if (y > 0 && this[x, y - 1] != null)
                        node.neighbors[Direction.Up] = this[x, y - 1];

                    //DOWN
                    if (y < boardInfo.height - 1 && this[x, y + 1] != null)
                        node.neighbors[Direction.Down] = this[x, y + 1];

                    //RIGHT
                    if (x < boardInfo.width - 1)
                    {
                        if (this[x + 1, y] != null)
                            node.neighbors[Direction.Right] = this[x + 1, y];
                    }

                    //LEFT
                    if (x > 0)
                    {
                        if (this[x - 1, y] != null)
                            node.neighbors[Direction.Left] = this[x - 1, y];
                    }

                    //OctaNode
                    if((x + y) % 2 == 0)
                    {
                        //DOWNRIGHT & UPRIGHT
                        if (x < boardInfo.width - 1)
                        {
                            if (x % 2 == 0)
                            {
                                if (this[x + 1, y] != null)
                                    node.neighbors[Direction.DownRight] = this[x + 1, y];
                                if (y > 0 && this[x + 1, y - 1] != null)
                                    node.neighbors[Direction.UpRight] = this[x + 1, y - 1];
                            }
                            else
                            {
                                if (this[x + 1, y] != null)
                                    node.neighbors[Direction.UpRight] = this[x + 1, y];
                                if (y < boardInfo.height - 1 && this[x + 1, y + 1] != null)
                                    node.neighbors[Direction.DownRight] = this[x + 1, y + 1];
                            }
                        }

                        //DOWNLEFT & UPLEFT
                        if (x > 0)
                        {
                            if (x % 2 == 0)
                            {
                                if (this[x - 1, y] != null)
                                    node.neighbors[Direction.DownLeft] = this[x - 1, y];
                                if (y > 0 && this[x - 1, y - 1] != null)
                                    node.neighbors[Direction.UpLeft] = this[x - 1, y - 1];
                            }
                            else
                            {
                                if (this[x - 1, y] != null)
                                    node.neighbors[Direction.UpLeft] = this[x - 1, y];
                                if (y < boardInfo.height - 1 && this[x - 1, y + 1] != null)
                                    node.neighbors[Direction.DownLeft] = this[x - 1, y + 1];
                            }
                        }
                    }            
                }
            }
        }

        public override Vector2 DrawPosition(Vector2 cellPos)
        {
            float yDelta = (cellPos.X + cellPos.Y) % 2 == 0 ? 0 : 118/2 * 0.00678f;
            float xDelta = (cellPos.X + cellPos.Y) % 2 == 0 ? 0 : 118/2 * 0.00678f;

            return new Vector2(cellPos.X + xDelta, cellPos.Y + yDelta);
        }

        public override Node Move(Node currentNode, Direction direction)
        {
            return currentNode.neighbors.ContainsKey(direction) ? currentNode.neighbors[direction] : currentNode;
        }
    }
}
