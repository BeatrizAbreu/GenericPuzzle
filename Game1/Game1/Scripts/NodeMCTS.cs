using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class NodeMCTS
    {
        GameState gameState;
        List<NodeMCTS> children;
        int winCount;
        int playsCount;
        string hashGameState;
        bool beenVisited;
    }
}
