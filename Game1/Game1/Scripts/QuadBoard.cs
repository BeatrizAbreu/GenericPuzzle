using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class QuadBoard : Board
    {
        public QuadBoard(int width, int height, int nHoles, int nBoxes, int nEnemies, int nCollectibles, int nDirections, Game1 game) : base(width, height, nHoles, nBoxes, nEnemies, nCollectibles, nDirections, game)
        {
            nodeTexture = game.Content.Load<Texture2D>("assets/quadnode");
        }

        public QuadBoard(int width, int height, Vector2[] holesPosition, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, int nDirections, Game1 game)
            : base(width, height, holesPosition, obstacles, enemyObjects, winObjects, nDirections, game)
        {
            nodeTexture = game.Content.Load<Texture2D>("assets/quadnode");
        }

        internal override void CreateNeighbors()
        {
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    Node node = this[x, y];
                    if (node == null) continue;

                    // Not the top line - can go Top
                    if (y > 0 && this[x, y - 1] != null)
                        node.neighbors[Direction.Up] = this[x, y - 1];

                    // Not the bottom line - can go Down
                    if (y < boardInfo.height - 1 && this[x, y + 1] != null)
                        node.neighbors[Direction.Down] = this[x, y + 1];

                    // Not last column - can go Right
                    if (x < boardInfo.width - 1)
                    {
                        if (this[x + 1, y] != null)
                            node.neighbors[Direction.Right] = this[x + 1, y];
                    }

                    // Not first column - can go Left
                    if (x > 0)
                    {
                        if (this[x - 1, y] != null)
                            node.neighbors[Direction.Left] = this[x - 1, y];
                    }
                }
            }
        }

        public override Vector2 DrawPosition(Vector2 cellPos)
        {
            return new Vector2(cellPos.X, cellPos.Y);
        }

        public override Node Move(Node currentNode, Direction direction)
        {
            return currentNode.neighbors.ContainsKey(direction) ? currentNode.neighbors[direction] : currentNode;
        }
    }
}
