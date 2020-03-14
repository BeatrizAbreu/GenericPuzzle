﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Scripts
{
    public class GameState
    {
        Node[,] nodes;
        Player player;
        List<Obstacle> obstacles;
        List<EnemyObject> enemyObjects;
        public List<WinObject> winObjects;
        bool beenVisited;

        public GameState(Node[,] nodes, List<Obstacle> obstacles, List<EnemyObject> enemyObjects, List<WinObject> winObjects, Player player)
        {
            this.nodes = nodes;
            this.obstacles = obstacles;
            this.enemyObjects = enemyObjects;
            this.winObjects = winObjects;
            this.player = player;
            beenVisited = false;
        }
    }
}
