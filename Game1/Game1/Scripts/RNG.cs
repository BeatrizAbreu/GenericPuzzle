using System;

namespace Game1
{
    public class RNG {
        static RNG rngen;
        Random rng;
        public RNG() {
            Random tmp = new Random();
            Int32 seed = tmp.Next();
            Console.WriteLine($"RANDOM SEED: {seed}");
            rng = new Random(seed);
            rngen = this;
        }
        public RNG(Int32 seed) {
            rng = new Random(seed);
            rngen = this;
        }

        public static int Next(int limit) { return RNG.rngen.rng.Next(limit); }
    }
}