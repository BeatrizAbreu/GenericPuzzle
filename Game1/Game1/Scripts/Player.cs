using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Player
    {
        public Vector2 position;
        private static int nMoves;

        public Player(Vector2 position)
        {
            this.position = position;
            nMoves = 0;
        }

        public Node Walk(Vector2 direction, List<Obstacle> obstacles, List<WinObject> winObjects, Node currentNode)
        {
            //if x is pair and the direction is DOWN or if x is not pair and the direction is UP and direction.X is NOT ZERO
            if (direction.X != 0
                && ((currentNode.position.X % 2 == 0 && direction.Y > 0)
                || (currentNode.position.X % 2 != 0 && direction.Y < 0)))
                direction = new Vector2(direction.X, 0);

            //finding the next node through the current node's neighbors
            foreach (KeyValuePair<Node, Direction> neighbor in currentNode.neighbors)
            {
                Direction dir = Functions.GetDirection(direction);
                if (neighbor.Value == dir)
                {
                    //find the next node's obstacle
                    foreach (var obstacle in obstacles)
                    {
                        //find the obstacle that's in the neighbor
                        if (obstacle.position == neighbor.Key.position)
                        {
                            //if the object is a box
                            if (!neighbor.Key.isEmpty)
                            {
                                //indicates the box's movement direction
                                Vector2 futureDirection = direction;
                                Direction futureDir = dir;

                                //find the box's next position once it's pushed
                                foreach (KeyValuePair<Node, Direction> futureNeighbor in neighbor.Key.neighbors)
                                {
                                    //if the node's x is pair and the direction is DOWN or if x is not pair and the direction is UP and direction.X is NOT ZERO
                                    if (direction.X != 0
                                        && ((neighbor.Key.position.X % 2 == 0 && direction.Y > 0)
                                        || (neighbor.Key.position.X % 2 != 0 && direction.Y < 0)))
                                    {
                                        futureDirection = new Vector2(direction.X, 0);
                                        futureDir = Functions.GetDirection(futureDirection);
                                    }

                                    //if that position is found and the node is empty
                                    if (futureNeighbor.Value == futureDir
                                        && futureNeighbor.Key.isEmpty)
                                    {
                                        //cast the box's action
                                        obstacle.Action(futureDirection);
                                        //update the neighbor node's state to empty as the box is pushed
                                        neighbor.Key.isEmpty = true;
                                        //update the future neighbor's state to not empty
                                        futureNeighbor.Key.isEmpty = false;

                                        foreach(WinObject winObject in winObjects)
                                        {
                                            //if a pressure plate is found in the same position, the box is placed on the pressure plate
                                            if (winObject.position == futureNeighbor.Key.position)                                              
                                            {
                                                winObject.Action();
                                                //inObject.isTriggered = true;
                                            }
                                            //if the box was in a pressure plate
                                            if (winObject.position == neighbor.Key.position)
                                            {
                                                winObject.Deactivate();
                                            }
                                        }

                                        nMoves++;
                                        //update the player's position
                                        return neighbor.Key;
                                    }
                                }
                                return currentNode;
                            }

                            ////if the object is a spike
                            //else
                            //{

                            //}
                        }
                    }

                    nMoves++;
                    //the current node is updated
                    return neighbor.Key;
                }
            }

            return currentNode;
        }
    }
}
