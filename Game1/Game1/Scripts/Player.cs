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

        public Node Walk(Vector2 direction, int spriteWidth, int spriteHeight, List<Obstacle> obstacles, List<WinObject> winObjects, Node currentNode)
        {
            //finding the next node through the current node's neighbors
            foreach (KeyValuePair<Node, Direction> neighbor in currentNode.neighbors)
            {
                Direction dir = Functions.GetDirection(direction);
                if (neighbor.Value == dir)
                {
                    ////find the next node's obstacle
                    //foreach (var obstacle in obstacles)
                    //{
                    //    if (obstacle.position == neighbor.Key.position)
                    //    {
                    //        //cast the obstacle's action
                    //        obstacle.Action(direction);
                    //    }
                    //}

                    ////find the next node's win object
                    //foreach (var winObject in winObjects)
                    //{
                    //    if (winObject.position == neighbor.Key.position)
                    //    {
                    //        //cast the object's action
                    //        winObject.Action();
                    //    }
                    //}

                    //the current node is updated
                    return neighbor.Key;
                }
            }

            return currentNode;
        }
    }
}
