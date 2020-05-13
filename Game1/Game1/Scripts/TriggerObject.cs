using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public abstract class TriggerObject
    {
        //ex: Portal (teleport from pos1 to pos2) / Laser + Toggle (pos1 triggers pos2) / Key + Door (pos1 deletes pos2 object)? / Slippery tiles from pos1 to pos2
        public Vector2 pos1;
        public Vector2 pos2;

        public Texture2D texture1;
        public Texture2D texture2;

        //Basic trigger function
        public virtual void Trigger()
        {

        }

        //Player/Box-based trigger function through movement
        public virtual Vector2 Trigger(Vector2 position, Vector2 dir, Vector2 pos)
        {
            return Vector2.One;
        }
    }
}
