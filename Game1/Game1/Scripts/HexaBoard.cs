using Game1.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    //Creates and manages the game's board
    public class HexaBoard : Board
    {    
        public HexaBoard(int width, int height, int nHoles, int nBoxes, int nEnemies, int nCollectibles, int nDirections) : base(width, height, nHoles, nBoxes, nEnemies, nCollectibles, nDirections)
        {
        }

        public HexaBoard(int width, int height, Vector2[] holesPosition, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects) 
            : base(width, height, holesPosition, obstacles, enemyObjects, winObjects)
        {
            /* nothing specific to hexaboard for now */
        }

        internal override void CreateNeighbors()
        {
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    Node node = this[x,y];
                    if (node == null) continue;

                    // Not top
                    if (y > 0 && this[x, y-1] != null)
                        node.neighbors[Direction.Up] = this[x,y-1];
                    // Not bottom
                    if (y < boardInfo.height - 1 && this[x, y+1] != null)
                        node.neighbors[Direction.Down] = this[x,y+1];

                    // Not last column
                    if (x < boardInfo.width - 1)
                    {          
                        if (x % 2 == 0)
                        {
                            if (this[x+1,y]!= null)
                                node.neighbors[Direction.DownRight] = this[x+1,y];
                            if (y > 0 && this[x+1,y-1] != null)
                                node.neighbors[Direction.UpRight] = this[x+1,y-1];
                        }
                        else
                        {
                            if (this[x+1,y]!=null)
                                node.neighbors[Direction.UpRight] = this[x+1,y];
                            if (y < boardInfo.height-1 && this[x+1,y+1] != null)
                                node.neighbors[Direction.DownRight] = this[x+1,y+1];
                        }
                    }
                     
                    // Not first column
                    if (x > 0)
                    {
                        if (x%2 == 0) 
                        {
                            if (this[x-1,y] != null)
                                node.neighbors[Direction.DownLeft] = this[x-1,y];
                            if (y > 0 && this[x-1,y-1] != null)
                                node.neighbors[Direction.UpLeft] = this[x-1,y-1];
                        }
                        else
                        {
                            if (this[x-1,y] != null)
                                node.neighbors[Direction.UpLeft] = this[x-1,y];
                            if (y < boardInfo.height - 1 && this[x-1,y+1]!=null)
                                node.neighbors[Direction.DownLeft] = this[x-1,y+1];
                        }
                    }
                }
            }
        }

        public override Vector2 DrawPosition(Vector2 cellPos)
        {
            float yDelta = cellPos.X % 2 == 0 ? 0 : 0.5f;
            return new Vector2(cellPos.X, cellPos.Y + yDelta);    
        } 

        public override Node Move(Node currentNode, Direction direction)
        {
            return currentNode.neighbors.ContainsKey(direction) ? currentNode.neighbors[direction] : currentNode;
        }
    }
}