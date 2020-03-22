using System;
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
            //make random run
            int result = gameState.PlayTest(500);
            playsCount++;
            if (result > 0) winCount++;
            if (result < 0) lossCount++;
        }

        public void Iterate(NodeMCTS root)
        {
            int x = (int)root.gameState.player.position.X, y = (int)root.gameState.player.position.Y;
            var neighborKeys = root.gameState.board.Node(new Microsoft.Xna.Framework.Vector2(x, y)).neighbors.Keys.Shuffle().ToList();

            //if the node has a neighbor list, iterate through each neighbor
            if (root.gameState.board.nodes[x, y].neighbors != null)
            {
                foreach (KeyValuePair<Direction, Node> neighbor in root.gameState.board.nodes[x, y].neighbors)
                {
                    //if the direction hasn't yet been visited
                    if (!children.ContainsKey(neighbor.Key))
                    {
                        if (root.gameState.player.Move(neighbor.Key))
                        {
                            //create the child node and random run
                            NodeMCTS child = new NodeMCTS(root.gameState);
                            //add the child to the children list
                            root.children.Add(neighbor.Key, child);
                            //update the parent's win/loss/plays values
                            root.playsCount += child.playsCount;
                            root.lossCount += child.lossCount;
                            root.winCount += child.winCount;
                        }
                    }
                }

                foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
                {
                    //iterate through each child's children as well
                    Iterate(child.Value);
                }
            }
        }
    }
}
