using UnityEngine;
using System.Collections;
using System;

namespace ActionGameServer
{

    public enum ObstacleType { block, toge };

    public class Obstacle
    {
        public int X;
        public int Y;
        public ObstacleType Type;

        public Obstacle()
        {

        }

        public Obstacle(int x, int y, ObstacleType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }

    public enum GameState { Start, Game, Pwin, Plose };

    public class Player
    {
        public GameState State;
        public double X;
        public double Y;
        public int HP;

        public Player()
        {
        }

        public Player(GameState state, double x, double y, int hp)
        {
            this.State = state;
            this.X = x;
            this.Y = y;
            this.HP = hp;
        }
    }
}
