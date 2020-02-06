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

        public Node Walk(Vector2 direction, List<Obstacle> obstacles, List<WinObject> winObjects, Node currentNode)
        {
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
                                //find the box's next position once it's pushed
                                foreach (KeyValuePair<Node, Direction> futureNeighbor in neighbor.Key.neighbors)
                                {
                                    //if that position is found
                                    if (futureNeighbor.Value == dir
                                        && futureNeighbor.Key.isEmpty)
                                    {
                                        //cast the box's action
                                        obstacle.Action(direction);
                                        //update the neighbor node's state to empty as the box is pushed
                                        neighbor.Key.isEmpty = true;
                                        //update the future neighbor's state to not empty
                                        futureNeighbor.Key.isEmpty = false;

                                        foreach(WinObject winObject in winObjects)
                                        {
                                            //the box is placed on a pressure plate
                                            if(winObject.position == futureNeighbor.Key.position)
                                            {
                                                winObject.isTriggered = true;
                                            }
                                        }

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

                    //the current node is updated
                    return neighbor.Key;
                }
            }

            return currentNode;
        }
    }
}
