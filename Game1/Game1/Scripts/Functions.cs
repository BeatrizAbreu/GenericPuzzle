using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    static class Functions
    {
        //Sets each node's neighbor with the right direction name (UP/DOWN/LEFT/RIGHT/etc.) 
        public static Direction GetDirection(Vector2 direction)
        {
            Direction dir;

            //RIGHT
            if (direction.X > 0)
            {
                //UP
                if (direction.Y < 0)
                    dir = Direction.UpRight;
                //DOWN
                else if (direction.Y > 0)
                    dir = Direction.DownRight;
                //NEUTRAL
                else
                    dir = Direction.Right;
            }
            //LEFT
            else if (direction.X < 0)
            {
                //UP
                if (direction.Y < 0)
                    dir = Direction.UpLeft;
                //DOWN
                else if (direction.Y > 0)
                    dir = Direction.DownLeft;
                //NEUTRAL
                else
                    dir = Direction.Left;
            }
            //NEUTRAL
            else
            {
                if (direction.Y < 0)
                    dir = Direction.Up;
                else
                    dir = Direction.Down;
            }

            return dir;
        }

        public static List<DirectionValidation> EvaluateActions(Node currentNode, List<Obstacle> obstacles)
        {
            List<DirectionValidation> dirValidation = new List<DirectionValidation>();

            //finding the next possible node through the current node's neighbors
            foreach (KeyValuePair<Node, Direction> neighbor in currentNode.neighbors)
            {
                //fill in the pair's direction value
                DirectionValidation newDirVal = new DirectionValidation();
                newDirVal.direction = neighbor.Value;
                int count = 0;
                bool foundObstacle = false;

                //go through the obstacles' list
                foreach (var obstacle in obstacles)
                {
                    count++;
                    //find if there's an obstacle in that cell
                    if (obstacle.position == neighbor.Key.position)
                    {
                        //one obstacle has been found
                        foundObstacle = true;

                        ////if the object is a box
                        //if (!neighbor.Key.isEmpty)
                        //{
                        //    //find the box's next position once it's pushed
                        //    foreach (KeyValuePair<Node, Direction> futureNeighbor in neighbor.Key.neighbors)
                        //    {
                        //        //if that position is found and the node is empty
                        //        if (futureNeighbor.Value == neighbor.Value
                        //            && futureNeighbor.Key.isEmpty)
                        //        {
                        //            //the box can be pushed (VALID ACTION)
                        //            newDirVal.isValid = true;
                        //        }
                        //    }
                        //}
                        //else
                        {
                            //if the object is not a box it can be pushed (VALID ACTION)
                            newDirVal.isValid = true;
                        }
                    }
                    ////if no obstacle was found -> we're on the last obstacle in the list and no object was found
                    //else if(count == obstacles.Count
                    //    && !foundObstacle)
                    //{
                    //    //if there was no obstacle, it's valid (VALID ACTION)
                    //    newDirVal.isValid = true;
                    //}
                }
                dirValidation.Add(newDirVal);
            }
            return dirValidation;
        }
    }
}
