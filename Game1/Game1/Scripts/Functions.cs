using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    static class Functions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> l)
        {
            return l.OrderBy(a => Guid.NewGuid());
        }

        public static int GetPlacementChance(int x, int y, int width, int height, int nObjects)
        {
            float attenuationFactor = (float)x * y / nObjects > 20 ? 0.5f : 1;

            //if this is the last line on the board, the chances are bigger
            if (x == width - 1)
                return (int)((float)width * height / 25 > 1 ? (float)width * height / 25 * 30 * attenuationFactor : 0) + 30;

            return (int)((float)width * height / 25 > 1 ? (float)width * height / 25 * 15 * attenuationFactor : 0) + 30;
        }
    }
}
