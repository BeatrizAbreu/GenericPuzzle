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

        internal override void CreateNeighbors(Node currentNode)
        {
            //create a neighbor list
            currentNode.neighbors = new Dictionary<Node, Direction>();

            float x = currentNode.position.X;
            float y = currentNode.position.Y;
            Node node = new Node();

            //identify neighbors and fill the neighbor list
            for (int y1 = 0; y1 < boardInfo.height; y1++)
            {
                for (int x1 = 0; x1 < boardInfo.width; x1++)
                {
                    node = this[x1, y1];
                    //if it's a neighbor and not a hole
                    if (node != null &&
                      (node.position == new Vector2(x - 1, y - 1)
                    || node.position == new Vector2(x, y - 1)
                    || node.position == new Vector2(x + 1, y - 1)
                    || node.position == new Vector2(x - 1, y + 1)
                    || node.position == new Vector2(x, y + 1)
                    || node.position == new Vector2(x + 1, y + 1)
                    || node.position == new Vector2(x - 1, y)
                    || node.position == new Vector2(x + 1, y)))
                    {
                        //calculate the direction from the current node to the neighbor
                        Vector2 direction = node.position - currentNode.position;
                        //direction is defined for each neighbor
                        currentNode.neighbors.Add(node, Functions.GetDirection(direction));
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
    }
}