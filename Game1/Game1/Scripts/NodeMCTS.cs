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

            foreach (KeyValuePair<Direction, Node> neighbor in root.gameState.board.nodes[x, y].neighbors)
            {
                //the direction hasn't yet been visited
                if (children.ContainsKey(neighbor.Key))
                {
                    break;
                }

                //make the movement
                if (root.gameState.player.Move(neighbor.Key))
                {
                    //create the child node
                    NodeMCTS child = new NodeMCTS(root.gameState);
                    root.children.Add(neighbor.Key, child);
                }
            }

            foreach (KeyValuePair<Direction, NodeMCTS> child in root.children)
            {

            }
        }
    }
}
