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
        public int winCount;
        public int lossCount;
        public int playsCount;
        string hashGameState;

        public NodeMCTS(Dictionary<Direction, NodeMCTS> children)
        {
            this.children = children;
            winCount = 0;
            lossCount = 0;
            playsCount = 0;
            hashGameState = "";
        }
    }
}
