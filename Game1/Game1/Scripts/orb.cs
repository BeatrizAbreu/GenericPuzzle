using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class Orb : WinObject
    {
        public static int activeObjectCount;
        public static int objectCount;

        public Orb()
        {
            tag = "orb";
            objectCount++;
        }

        public override void Action()
        {
            isTriggered = true;
            activeObjectCount++;
        }
    }
}
