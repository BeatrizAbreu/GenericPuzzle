using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class NodeMCTS
    {
        const int MCRuns = 500;
        const float C = 1;
        public Dictionary<Direction, NodeMCTS> children;
        public GameState gameState;
        public int winCount;
        public int lossCount;
        public int playsCount;
     //   string hashGameState;

        public NodeMCTS(GameState gameState)
        {
            this.children = new Dictionary<Direction, NodeMCTS>();
            this.gameState = gameState;

            GameState copy = gameState; //.Copy(); or we will be copying twice, one in Iterate/Run, another here
            //make random run
            int result = copy.PlayTest(MCRuns);
            playsCount++;
            if (result > 0) winCount++;
            if (result < 0) lossCount++;
        }

        public NodeMCTS() { children = new Dictionary<Direction, NodeMCTS>(); }

        public void Run()
        {
            // get all possible moves
            List<Direction> possibleMoves = gameState.board.Node(gameState.player.position).neighbors.Keys.ToList();
            foreach (Direction move in possibleMoves)
            {
                if (!children.ContainsKey(move))
                {
                    bool isCopy = false;
                    GameState childState = gameState.Copy();
                    if (childState.player.Move(move)) {
                        //if the gamestate exists, don't make a new child
                        foreach (KeyValuePair<Direction, NodeMCTS> c in children)
                        {
                            if(c.Value.gameState == childState)
                            {
                                GameState copy = gameState; 
                                //make random run
                                int result = copy.PlayTest(MCRuns);
                                playsCount++;
                                if (result > 0) winCount++;
                                if (result < 0) lossCount++;
                                isCopy = true;
                                break;
                            }
                        }
                        //create a new child
                        if(!isCopy)
                        {
                            NodeMCTS child = new NodeMCTS(childState);
                            winCount += child.winCount;
                            lossCount += child.lossCount;
                            children[move] = child;
                        }
                        return; // Our work is done here, one node was expanded!
                    } 
                }
            }

            // If we got here, all moves were tested.
            // Decide which move should be explored.
            int N = children.Values.Select( node => node.playsCount ).Sum(); 

            Direction? bestDirection = null;
            float uBest = 0f;
            foreach (Direction direction in children.Keys) {
                NodeMCTS node = children[direction]; 
                float u = (node.winCount / node.playsCount) + C * (float)Math.Sqrt( 2 * Math.Log(N) / node.playsCount);
                if (u > uBest) {
                    uBest = u; bestDirection = direction;
                }
            }

            if (bestDirection.HasValue) {
                // Save child counts
                int childLosses = children[bestDirection.Value].lossCount;
                int childWins = children[bestDirection.Value].winCount;
                int childPlays = children[bestDirection.Value].playsCount;
                children[bestDirection.Value].Run();
                // Recompute counts
                playsCount = playsCount - childPlays + children[bestDirection.Value].playsCount;
                winCount = winCount - childWins + children[bestDirection.Value].winCount;
                lossCount = lossCount - childLosses + children[bestDirection.Value].lossCount;

                return; // Our work is done, we iterated down once
            }
            
            // If we got here, we were unable to expand, and there were no childs
            // check:
            // - if this is a win position, sum one to wins count
            // - if this is a lose position,   sum one to lost count
            // either way, increment  
            // FIXME!!!
        }


        //public void Iterate(NodeMCTS root, NodeMCTS firstRoot)
        //{
        //    int x = (int)root.gameState.player.position.X, y = (int)root.gameState.player.position.Y;

        //    //if the node has a neighbor list, iterate through each neighbor
        //    if (root.gameState.board.nodes[x, y].neighbors != null)
        //    {
        //        //shuffle the neighbors keys so the order is random (not really a necessary step)
        //        var neighborKeys = root.gameState.board.Node(new Microsoft.Xna.Framework.Vector2(x, y)).neighbors.Keys.Shuffle().ToList();

        //        //saves the best path given the formula
        //        NodeMCTS bestPath = new NodeMCTS();

        //        NodeMCTS rootCopy = root.Copy();

        //        bool isExpanding = false;

        //        foreach (var key in neighborKeys)
        //        {
        //            //if the node doesn't have children yet or its direction hasn't yet been visited
        //            if (!root.children.ContainsKey(key))
        //            {
        //                //if the player can move towards that direction, it moves
        //                if (rootCopy.gameState.player.Move(key))
        //                {
        //                    //create the child node and executes a random run
        //                    NodeMCTS child = new NodeMCTS(rootCopy.gameState);

        //                    //add the child to the children list
        //                    root.children.Add(key, child);

        //                    //update the parent's win/loss/plays values
        //                    root.playsCount += child.playsCount;
        //                    root.lossCount += child.lossCount;
        //                    root.winCount += child.winCount;
        //                }
        //            }

        //            //this node has already been visited
        //            else
        //            {
        //                isExpanding = true;
        //                break;
        //            }
        //        }

        //        //if the tree is expanding through evaluation
        //        if (isExpanding)
        //        {
        //            //find the best path using the formula
        //            bestPath = GetBestPath(root);

        //            //iterate through the bestPath's children and expand the tree
        //            Iterate(bestPath, firstRoot);
        //        }
                
        //        //go through the root again 
        //        Iterate(firstRoot, firstRoot);
        //    }
        //}

        public bool Iterate(NodeMCTS root, NodeMCTS firstRoot, int iNow, int iTotal)
        {
            int x = (int)root.gameState.player.position.X, y = (int)root.gameState.player.position.Y;

            //if the node has a neighbor list, iterate through each neighbor
            if (root.gameState.board.nodes[x, y].neighbors != null)
            {
                //shuffle the neighbors keys so the order is random (not really a necessary step)
                var neighborKeys = root.gameState.board.Node(new Microsoft.Xna.Framework.Vector2(x, y)).neighbors.Keys.Shuffle().ToList();

                //saves the best path given the formula
                NodeMCTS bestPath = new NodeMCTS();

                NodeMCTS rootCopy = root.Copy(); //new NodeMCTS();  
               // rootCopy.children = root.children.Copy();
               // rootCopy.gameState = root.gameState.Copy();
               // rootCopy.lossCount = root.lossCount.Copy();
               // rootCopy.winCount = root.winCount.Copy();

                bool isExpanding = false;

                foreach (var key in neighborKeys)
                {
                    if (iNow == iTotal)
                        return true;

                    //if the node doesn't have children yet or its direction hasn't yet been visited
                    if (!root.children.ContainsKey(key))
                    {
                        //if the player can move towards that direction, it moves
                        if (rootCopy.gameState.player.Move(key))
                        {
                            //create the child node and executes a random run
                            NodeMCTS child = new NodeMCTS(rootCopy.gameState);

                            //add the child to the children list
                            root.children.Add(key, child);

                            //update the parent's win/loss/plays values
                            root.playsCount += child.playsCount;
                            root.lossCount += child.lossCount;
                            root.winCount += child.winCount;
                            iNow++;
                        }
                    }

                    //this node has already been visited
                    else
                    {
                        isExpanding = true;
                        break;
                    }
                }

                if (iNow == iTotal)
                    return true;

                //if the tree is expanding through evaluation
                if (isExpanding)
                {
                    //find the best path using the formula
                    bestPath = GetBestPath(root);

                    bool res = Iterate(bestPath, firstRoot, iNow, iTotal);
                    //   FindParent(ref bestPath, ref firstRoot);

                    root.playsCount += bestPath.playsCount;
                    root.lossCount += bestPath.lossCount;
                    root.winCount += bestPath.winCount;

                    //iterate through the bestPath's children and expand the tree
                    if (res)
                    {
                        UpdateParentValues(ref firstRoot);
                        return true;
                    }                       
                }

                bool result = Iterate(firstRoot, firstRoot, iNow, iTotal);

                //root.playsCount += bestPath.playsCount;
                //root.lossCount += bestPath.lossCount;
                //root.winCount += bestPath.winCount;

                //go through the root again 
                if (result)
                {
                    UpdateParentValues(ref firstRoot);
                    return true;
                }
            }
            return false;
        }

        private NodeMCTS FindParent(ref NodeMCTS best, ref NodeMCTS root)
        {
            foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
            {
                //encontrei o best
                if(child.Value == best)
                {
                    //encontrei o pai e atualizo-o
                    UpdateParentValues(ref root);
                    return root;
                }
            }

            foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
            {
                NodeMCTS childVal = child.Value;
                FindParent(ref childVal, ref root);
            }

            return new NodeMCTS();
        }

        private void UpdateParentValues(ref NodeMCTS root)
        {
            foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
            {
                root.winCount += child.Value.winCount;
                root.lossCount += child.Value.lossCount;
                root.playsCount += child.Value.playsCount;
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
                result = (float)child.Value.winCount / child.Value.playsCount
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
