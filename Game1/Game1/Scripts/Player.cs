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

        public Player(Vector2 position)
        {
            this.position = position;
            nMoves = 0;
        }
    }
}
