﻿using System;
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
        public int winCount;
        public int lossCount;
        string hashGameState;

        public NodeMCTS(List<NodeMCTS> children)
        {
            this.children = children;
            winCount = 0;
            lossCount = 0;
            hashGameState = "";
        }
    }
}
