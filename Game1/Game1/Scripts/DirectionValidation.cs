using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    class DirectionValidation
    {
        public Direction direction;
        public bool isValid;

        public DirectionValidation()
        {
            direction = new Direction();
            isValid = false;
        }
    }
}
