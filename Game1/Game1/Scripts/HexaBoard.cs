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
        SpriteBatch spriteBatch;
        Texture2D boardNodeTex;
    
        public HexaBoard(Game1 game, int width, int height, int nHoles, int nBoxes, int nEnemies) : base(game, width, height, nHoles, nBoxes, nEnemies)
        {
            /* nothing specific to hexaboard for now */
            boardNodeTex = game.Content.Load<Texture2D>("assets/node");
            spriteBatch = game.spriteBatch;
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

        
        public override void Draw(GameTime gameTime) {
            //draw the board sprites
            for (int y = 0; y < boardInfo.height; y++)
            {
                for (int x = 0; x < boardInfo.width; x++)
                {
                    float yDelta = x % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
                    Color colorDelta = y % 2 == 0 ? Color.White : Color.LightGray;

                    //if the node isn't a hole
                    if (this[x, y] != null)
                        spriteBatch.Draw(boardNodeTex, DrawPosition(this[x,y].position), colorDelta);
                    else
                        spriteBatch.Draw(boardNodeTex, DrawPosition(new Vector2(x,y)), Color.Black);
                }
            }
        }

        public override Vector2 DrawPosition(Vector2 cellPos)
        {
            float yDelta = cellPos.X % 2 == 0 ? 0 : boardNodeTex.Height / 2f;
            return new Vector2(cellPos.X * boardNodeTex.Height, cellPos.Y * boardNodeTex.Height + yDelta);    
        } 

        public override Node Move(Node currentNode, Direction direction)
        {
            return currentNode.neighbors.ContainsKey(direction) ? currentNode.neighbors[direction] : currentNode;
        }
    }
}