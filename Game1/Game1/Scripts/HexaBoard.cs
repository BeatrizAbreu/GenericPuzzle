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
            currentNode.neighbors = new Dictionary<Direction, Node>();

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
                        currentNode.neighbors.Add(Functions.GetDirection(direction), node);
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

        public override Node Move(Node currentNode, Vector2 direction)
        {
            //indicates the box's movement direction
            Vector2 futureDirection = direction;

            //if x is pair and the direction is DOWN or if x is not pair and the direction is UP and direction.X is NOT ZERO
            if (direction.X != 0
                && ((currentNode.position.X % 2 == 0 && direction.Y > 0)
                || (currentNode.position.X % 2 != 0 && direction.Y < 0)))
            {
                futureDirection = direction;
                direction = new Vector2(direction.X, 0);
            }
            else if (direction.X != 0)
                futureDirection = new Vector2(direction.X, 0);

            //finding the next node through the current node's neighbors
            foreach (KeyValuePair<Direction, Node> neighbor in currentNode.neighbors)
            {
                Direction dir = Functions.GetDirection(direction);
                Direction futureDir = Functions.GetDirection(futureDirection);

                if (neighbor.Key == dir)
                {
                    //find the next node's obstacle
                    foreach (var obstacle in obstacles)
                    {
                        //find the obstacle that's in the neighbor
                        if (obstacle.position == neighbor.Value.position)
                        {
                            //if the object is a box 
                            if (!neighbor.Value.isEmpty && obstacle.tag == "box")
                            {
                                //find the box's next position once it's pushed
                                foreach (KeyValuePair<Direction, Node> futureNeighbor in neighbor.Value.neighbors)
                                {
                                    //if that position is found and the node is empty
                                    if (futureNeighbor.Key == futureDir
                                        && futureNeighbor.Value.isEmpty)
                                    {
                                        //cast the box's action
                                        obstacle.Action(futureDirection);
                                        //update the neighbor node's state to empty as the box is pushed
                                        neighbor.Value.isEmpty = true;
                                        //update the future neighbor's state to not empty
                                        futureNeighbor.Value.isEmpty = false;

                                        foreach (WinObject winObject in winObjects)
                                        {
                                            //if a pressure plate is found in the same position, the box is placed on the pressure plate
                                            if (winObject.position == futureNeighbor.Value.position)
                                            {
                                                winObject.Action();
                                                //inObject.isTriggered = true;
                                            }
                                            //if the box was in a pressure plate
                                            if (winObject.position == neighbor.Value.position)
                                            {
                                                winObject.Deactivate();
                                            }
                                        }

                                        Player.nMoves++;
                                        //update the player's position
                                        return neighbor.Value;
                                    }
                                }
                                return currentNode;
                            }
                        }
                    }

                    //find the next node's enemy
                    foreach (var enemyObj in enemyObjects)
                    {
                        //find the obstacle that's in the neighbor
                        if (enemyObj.position == neighbor.Value.position)
                        {
                            //kill the player
                            enemyObj.Action();
                        }
                    }

                    Player.nMoves++;
                    //the current node is updated
                    return neighbor.Value;
                }
            }

            return currentNode;
        }

    }
}