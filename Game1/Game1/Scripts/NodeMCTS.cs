using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class NodeMCTS
    {
        public Direction pathDirection;
        public GameState gameState;
        public List<NodeMCTS> children;
        int winCount;
        int playsCount;
        string hashGameState;
        bool beenVisited;

        public NodeMCTS(List<NodeMCTS> children)
        {
            this.children = children;
            beenVisited = false;
            winCount = 0;
            playsCount = 0;
            hashGameState = "";
        }
    }
}
