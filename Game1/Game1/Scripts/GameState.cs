using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class GameState
    {
        Node[,] nodes;
        List<Obstacle> obstacles;
        List<EnemyObject> enemyObjects;
        List<WinObject> winObjects;

        public GameState(Node[,] nodes, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects)
        {
            this.nodes = nodes;
            this.obstacles = obstacles;
            this.enemyObjects = enemyObjects;
            this.winObjects = winObjects;
        }
    }
}
