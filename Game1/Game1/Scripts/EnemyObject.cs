﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public abstract class EnemyObject
    {
        public Vector2 position;
        public Texture2D texture;
        public string tag;

        public void Action()
        {
            Player.hasLost = true;
        }
        public virtual void Action(Board board)
        {

        }
    }
}
