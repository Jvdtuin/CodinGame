using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Help the Christmas elves fetch presents in a magical labyrinth!
 **/
namespace XmasRush
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputs;

            // game loop
            while (true)
            {
                int turnType = int.Parse(Console.ReadLine());

                Map map = new Map();

                Player me = new Player();
                Player oponent = new Player();

                int numItems = int.Parse(Console.ReadLine()); // the total number of items available on board and on player tiles
                List<Item> items = new List<Item>();
                for (int i = 0; i < numItems; i++)
                {
                    items.Add(new Item());
                }

                int numQuests = int.Parse(Console.ReadLine()); // the total number of revealed quests for both players
                List<Quest> quests = new List<Quest>();
                for (int i = 0; i < numQuests; i++)
                {
                    quests.Add(new Quest());
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");
                if (turnType == 0)
                {
                    PlayPushTurn(map, me, oponent, items, quests);
                }
                else
                {
                    PlayMoveTurn(map, me, oponent, items, quests);
                }
            }
        }

        private static void PlayPushTurn(Map map, Player me, Player Oponent, List<Item> items, List<Quest> quests)
        {
            Console.WriteLine("PUSH 3 RIGHT"); // PUSH <id> <direction> | MOVE <direction> | PASS
        }

        private static void PlayMoveTurn(Map map, Player me, Player Oponent, List<Item> items, List<Quest> quests)
        {
            Quest myQuest = quests.First(q => q.PlayerId == 0);



            Console.WriteLine("MOVE RIGHT DOWN");
        }
    }

    public class Map
    {
        public Tile[,] Tiles { get; }

        public Map(Tile[,] tiles)
        {
            Tiles = tiles;
        }

        public Map()
        {
            Tiles = new Tile[7, 7];
            for (int i = 0; i < 7; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');

                for (int j = 0; j < 7; j++)
                {
                    string tile = inputs[j];
                    Tiles[j, i] = new Tile(tile);
                }
            }
        }

        private bool CanMove(Cord p, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (p.Y == 0) { return false; }
                    return Tiles[p.X, p.Y].Up && Tiles[p.X, p.Y - 1].Down;
                case Direction.RIGHT:
                    if (p.X == 6) { return false; }
                    return Tiles[p.X, p.Y].Right && Tiles[p.X + 1, p.Y].Left;
                case Direction.DOWN:
                    if (p.Y == 6) { return false; }
                    return Tiles[p.X, p.Y].Down && Tiles[p.X, p.Y + 1].Up;
                case Direction.LEFT:
                    if (p.X == 0) { return false; }
                    return Tiles[p.X, p.Y].Left && Tiles[p.X - 1, p.Y].Right;
                default: return false;
            }
        }

        struct Cord
        {
            public Cord(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; private set; }
            public int Y { get; private set; }

            public static Cord operator +(Cord c, Direction d)
            {
                switch (d)
                {
                    case Direction.UP: return new Cord(c.X, c.Y - 1);
                    case Direction.RIGHT: return new Cord(c.X + 1, c.Y);
                    case Direction.DOWN: return new Cord(c.X, c.Y + 1);
                    case Direction.LEFT: return new Cord(c.X - 1, c.Y);
                    default: return c;
                }
            }
        }

        public void CalcMoveMap(int orgx, int orgy)
        {
            Direction[,] moves = new Direction[7, 7];
            int[,] dist = new int[7, 7];

            int depth = 1;
            List<Cord> cords = new List<Cord>() { new Cord(orgx, orgy) };
            while (cords.Count > 0)
            {
                List<Cord> nextcords = new List<Cord>();

                foreach (Cord cord in cords)
                {
                    for (int i =0; i<4; i++)
                    {
                        Direction d = (Direction)i;
                        if (CanMove(cord, d))
                        {
                            Cord nc = cord + d;
                            if (dist[nc.X, nc.Y] != 0)
                            {
                                dist[nc.X, nc.Y] = depth;
                                moves[nc.X, nc.Y] = d;
                            }
                            nextcords.Add(nc);
                        }
                    }
                    
                }

                cords = nextcords;
                depth++;
            }

        }
    }

    public enum Direction
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3,
    }

    public struct Tile
    {
        public Tile(string value)
        {
            _value = value;
        }

        private string _value;

        public bool Up { get { return _value[0] == '1'; } }
        public bool Right { get { return _value[1] == '1'; } }
        public bool Down { get { return _value[2] == '1'; } }
        public bool Left { get { return _value[3] == '1'; } }
    }

    public class Player
    {
        public Player()
        {
            string[] inputs = Console.ReadLine().Split(' ');
            NumPlayerCards = int.Parse(inputs[0]); // the total number of quests for a player (hidden and revealed)
            X = int.Parse(inputs[1]);
            Y = int.Parse(inputs[2]);
            Tile = new Tile(inputs[3]);
        }

        public int NumPlayerCards { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public Tile Tile { get; private set; }
    }

    public class Item
    {
        public Item()
        {
            string[] inputs = Console.ReadLine().Split(' ');
            Name = inputs[0];
            X = int.Parse(inputs[1]);
            Y = int.Parse(inputs[2]);
            PlayerId = int.Parse(inputs[3]);
        }
        public string Name { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int PlayerId { get; private set; }
    }

    public class Quest
    {
        public Quest()
        {
            string[] inputs = Console.ReadLine().Split(' ');
            string questItemName = inputs[0];
            int questPlayerId = int.Parse(inputs[1]);
        }

        public string ItemName { get; private set; }

        public int PlayerId { get; private set; }
    }
}