using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    abstract class TriggerObject
    {
        //ex: Portal (teleport from pos1 to pos2) / Laser + Toggle (pos1 triggers pos2) / Key + Door (pos1 deletes pos2 object)? / Slippery tiles from pos1 to pos2
        Vector2 pos1;
        Vector2 pos2;

        Texture2D texture1;
        Texture2D texture2;

        TriggerObject(Vector2 pos1, Vector2 pos2)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
        }

        //Trigger function
        public void Trigger()
        {

        }
    }
}
