﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class Player
    {
        public Vector2 position;
        public static int nMoves;
        public static bool hasLost;
        Board board;
        public Player(Board board, Vector2 position)
        {
            this.board = board;
            this.position = position;
            nMoves = 0;
        }

        public bool Move(Direction direction)
        {                        
            Node currentNode = board.Node(position);
            Node targetNode = board.Move(currentNode, direction);

            // FIXME: return false if they are the same?

            foreach (Obstacle obstacle in board.obstacles)
            {
                // Obstacle ahead!
                if (obstacle.position == targetNode.position)
                {
                    // if the object is a box 
                    if (!targetNode.isEmpty && obstacle.tag == "box")
                    {
                        if (!obstacle.Move(direction))
                            return false;
                    }
                }

                // find the next node's enemy
                foreach (EnemyObject enemyObj in board.enemyObjects)
                {
                    //find the obstacle that's in the neighbor
                    if (enemyObj.position == targetNode.position)
                    {
                        // kill the player
                        enemyObj.Action();
                    }
                }
            }
       
            Player.nMoves++;
            position = targetNode.position;
            return true;
        }
    }
}
