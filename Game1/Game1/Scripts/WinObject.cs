using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class WinObject
    {
        public Vector2 position;
        public bool isTriggered;
        public Texture2D texture;
        public string tag;

        public virtual void Action()
        {
            isTriggered = true;
        }

        public virtual void Deactivate()
        {
            isTriggered = false;
        }

    }
}
