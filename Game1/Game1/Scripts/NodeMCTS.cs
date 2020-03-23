﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class NodeMCTS
    {
        public Dictionary<Direction, NodeMCTS> children;
        public GameState gameState;
        public int winCount;
        public int lossCount;
        public int playsCount;
        string hashGameState;

        public NodeMCTS(GameState gameState)
        {
            this.children = new Dictionary<Direction, NodeMCTS>();
            this.gameState = gameState;

            //make random run
            int result = gameState.PlayTest(500);
            playsCount++;
            if (result > 0) winCount++;
            if (result < 0) lossCount++;
        }

        public NodeMCTS()
        {
            this.children = new Dictionary<Direction, NodeMCTS>();
        }

        public void Iterate(NodeMCTS root, NodeMCTS firstRoot)
        {
            int x = (int)root.gameState.player.position.X, y = (int)root.gameState.player.position.Y;

            //if the node has a neighbor list, iterate through each neighbor
            if (root.gameState.board.nodes[x, y].neighbors != null)
            {
                //shuffle the neighbors keys so the order is random (not really a necessary step)
                var neighborKeys = root.gameState.board.Node(new Microsoft.Xna.Framework.Vector2(x, y)).neighbors.Keys.Shuffle().ToList();

                //saves the best path given the formula
                NodeMCTS bestPath = new NodeMCTS();

                bool isExpanding = false;

                foreach (var key in neighborKeys)
                {
                    //if the node doesn't have children yet or its direction hasn't yet been visited
                    if (!children.ContainsKey(key))
                    {
                        //if the player can move towards that direction, it moves
                        if (root.gameState.player.Move(key))
                        {
                            //create the child node and executes a random run
                            NodeMCTS child = new NodeMCTS(root.gameState);

                            //add the child to the children list
                            firstRoot.children.Add(key, child);

                            //update the parent's win/loss/plays values
                            firstRoot.playsCount += child.playsCount;
                            firstRoot.lossCount += child.lossCount;
                            firstRoot.winCount += child.winCount;
                        }
                    }

                    //this node has already been visited
                    else
                    {
                        isExpanding = true;
                        break;
                    }
                }

                //if the tree is expanding through evaluation
                if (isExpanding)
                {
                    /*FIX ME: BESTPATH IS NULL AT SOME POINT. 
                     1ST IT ITERATES THROUGH THE ROOT AND CREATES CHILDREN
                     2ND IT ITERATES THE ROOT'S CHILDREN AND CHOOSES THE BEST CHILD. THEN EXPANDS THAT CHILD
                     3RD - FAILS!! IT SHOULD RETURN TO THE ORIGINAL ROOT AND FIND THE BEST CHILD AGAIN */
                    bestPath = GetBestPath(root);

                    //iterate through the bestPath's children and expand the tree
                    Iterate(bestPath, firstRoot);
                }
                
                //go through the root again 
                Iterate(firstRoot, firstRoot);
            }
        }

        private NodeMCTS GetBestPath(NodeMCTS root)
        {
            //formula variables
            int C = 1;
            int N = 0;
            float result;
            float bestResult = 0f;
            NodeMCTS bestResNode = new NodeMCTS();

            int i = 0;

            //calculate the formula value for each child
            foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
            {
                //setting the N value
                N = GetNValue(child.Value);

                //calculate the node's formula
                result = child.Value.winCount / child.Value.playsCount
                    + C * (float)Math.Sqrt(2 * Math.Log(Math.E, N) / child.Value.playsCount);

                if (i == 0)
                {
                    bestResult = result;
                    bestResNode = child.Value;
                }

                //update the best current result given the result
                else if (result > bestResult)
                {
                    bestResult = result;
                    bestResNode = child.Value;
                }

                i++;
            }

            //return the child with the best result
            return bestResNode;
        }

        private int GetNValue(NodeMCTS node)
        {
            int N = 0;
            foreach (KeyValuePair<Direction, NodeMCTS> child in node.children)
            {
                N += child.Value.playsCount;
            }
            return N;
        }
    }
}
