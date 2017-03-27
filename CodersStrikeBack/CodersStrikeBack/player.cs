using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace tmp
{
    class Player
    {
#if !DEBUG
    static void Main(string[] args)
#else
        static void Run()
#endif
        {
            DateTime nu = DateTime.Now;
            raceInfo = new RaceInfo();

            Pod[] podsInfo = new Pod[4];
            podsInfo[0] = new Pod();
            podsInfo[1] = new Pod();
            podsInfo[2] = new Pod();
            podsInfo[3] = new Pod();

            UpdatePods(podsInfo);

            List<Move> enemyMoves = PredictEnemyMoves(podsInfo, 6);
            List<Solution> Solutions = new List<Solution>();
            for (int i = 0; i < 100; i++)
            {
                Solutions.Add(new Solution(enemyMoves, true));
            }
            while ((DateTime.Now - nu).Milliseconds < 100)
            {
                Solutions = MutateAndCrossSolutions(Solutions, 10);

                RunSimulations(Solutions, podsInfo);

                Solutions = SelectSolutions(Solutions, 5);
            }

            while (true)
            {
                nu = DateTime.Now;
                Vector t0 = raceInfo.Checkpoints[podsInfo[0].NextCheckPointId].Position - podsInfo[0].Velocity;
                Vector t1 = raceInfo.Checkpoints[podsInfo[1].NextCheckPointId].Position - podsInfo[1].Velocity;

                Console.Error.WriteLine(podsInfo[0]);
                Console.Error.WriteLine(podsInfo[1]);
                Console.Error.WriteLine(podsInfo[2]);
                Console.Error.WriteLine(podsInfo[3]);

                int teller = 0;
                while ((DateTime.Now - nu).Milliseconds < 90)
                {
                    Solutions = MutateAndCrossSolutions(Solutions, 10);

                    RunSimulations(Solutions, podsInfo);

                    Solutions = SelectSolutions(Solutions, 5);
                    teller++;
                }
                Console.Error.WriteLine(teller);

                foreach (Solution s in Solutions)
                {
                    Console.Error.WriteLine(s.Score);
                }

                Solution usedSolution = Solutions.First();


                //   Vector t1 = podsInfo[0].Position + usedSolution.Moves[0].Target(podsInfo[0]);
                //    Console.WriteLine("{0} {1} {2}", (int)t1.X, (int)t1.Y, usedSolution.Moves[0].Thrust);

                //   Vector t2 = podsInfo[1].Position + usedSolution.Moves[1].Target(podsInfo[1]);
                //      Console.WriteLine("{0} {1} {2}", (int)t2.X, (int)t2.Y, usedSolution.Moves[1].Thrust);

                Console.WriteLine("{0} {1} {2}", t0.X, t0.Y, 100);
                Console.WriteLine("{0} {1} {2}", t1.X, t1.Y, 100);

                // simulate oponent


                // simulate me


                //} while ((DateTime.Now -nu).Milliseconds < 90);

                // choice best option

                //      Console.WriteLine((podsInfo[0] as OwnPod).GetAction());
                //     Console.WriteLine((podsInfo[1] as OwnPod).GetAction());

                UpdatePods(podsInfo);

                foreach (Solution solution in Solutions)
                {
                    solution.NextTurn();
                }

            }
        }

#if !DEBUG
    static void UpdatePods(Pod[] podsInfo)
    {
        for (int i = 0; i < 4; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            podsInfo[i].UpdateValues(int.Parse(inputs[0]), int.Parse(inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]),
              int.Parse(inputs[4]), int.Parse(inputs[5]));
        }
    }
#else
        static void UpdatePods(Pod[] podsInfo)
        {

        }
#endif
        public static RaceInfo raceInfo;

        private static List<Move> PredictEnemyMoves(Pod[] pods, int count)
        {
            Pod runner = null;
            Pod blocker = null;
            Solution.WhoRunnerBlocker(pods[2], pods[3], out runner, out blocker);
            runner = runner.Clone();
            blocker = blocker.Clone();
            List<Move> enemyMoves = new List<Move>();
            for (int i = 0; i < count; i++)
            {
                Move action1 = runner.GetRunnerAction();
                Move action2 = blocker.GetBlockerAction();
                if (runner == pods[2])
                {
                    enemyMoves.Add(action1);
                    enemyMoves.Add(action2);
                }
                else
                {
                    enemyMoves.Add(action2);
                    enemyMoves.Add(action1);
                }
                runner.Rotate(action1.Target(runner));
                runner.Thrust(action1.Thrust);
                blocker.Rotate(action2.Target(blocker));
                blocker.Thrust(action2.Thrust);
                Collision data = GameUnit.CollisionData(runner, blocker);
                if (data != null)
                {
                    data.Bounce();
                }
                runner.Move();
                blocker.Move();
            }
            return enemyMoves;
        }

        private static void RunSimulations(List<Solution> solutions, Pod[] pods)
        {
            foreach (Solution solution in solutions)
            {
                solution.Simulate(pods);
            }
        }

        private static List<Solution> SelectSolutions(List<Solution> solutions, int count)
        {
            solutions.Sort(delegate (Solution s1, Solution s2)
            {
                if (s2.Score > s1.Score) { return 1; }
                if (s1.Score > s2.Score) { return -1; }
                return 0;
            });
            List<Solution> nextSolutions = new List<Solution>();
            for (int i = 0; i < count; i++)
            {
                nextSolutions.Add(solutions[i]);
            }
            return nextSolutions;
        }

        private static List<Solution> MutateAndCrossSolutions(List<Solution> solutions, int newCount)
        {
            List<Solution> newSolutions = new List<Solution>();
            newSolutions.Add(solutions.First());


            foreach (Solution solution in solutions)
            {
                newSolutions.Add(solution.Mutate());
                newSolutions.Add(solution.Mutate());
            }

            return newSolutions;
        }
    }

    public class Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector CreateVectorAngleSizeRad(double angleRad, double size)
        {
            return new Vector(Math.Cos(angleRad) * size, Math.Sin(angleRad) * size);
        }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double AngleRad
        {
            get
            {
                double a = 0.0;
                double sx = X > 0 ? 1.0 : -1.0;
                double sy = Y > 0 ? 1.0 : -1.0;
                if (sx * X > sy * Y)
                {
                    if (X > 0)
                    {
                        a = Math.Atan(Y / X);
                    }
                    else if (X < 0)
                    {
                        a = Math.PI + Math.Atan(Y / X); ;
                    }
                }
                else
                {
                    if (Y > 0)
                    {
                        //B
                        a = Math.PI * 0.5 - Math.Atan(X / Y);
                    }
                    else if (Y < 0)
                    {
                        //D
                        a = Math.PI * 1.5 - Math.Atan(X / Y);
                    }
                }
                if (a > Math.PI)
                {
                    a -= 2 * Math.PI;
                }
                return a;
            }
        }

        public double AngleDeg
        {
            get
            {
                return AngleRad / Math.PI * 180.0;
            }
        }

        public double Size2
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public double Size
        {
            get { return Math.Sqrt(Size2); }
        }

        public void Round()
        {
            X = Math.Round(X);
            Y = Math.Round(Y);
        }

        public void Truncate()
        {
            X = Math.Truncate(X);
            Y = Math.Truncate(Y);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator *(Vector v1, double x)
        {
            return new Vector(v1.X * x, v1.Y * x);
        }

        public static Vector operator /(Vector v1, double x)
        {
            return new Vector(v1.X / x, v1.Y / x);
        }

        public static double operator *(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public void Normalize(double newSize)
        {
            double factor = newSize / Size;
            X *= factor;
            Y *= factor;
        }

        public override string ToString()
        {
            return string.Format("({0:0.00} {1:0.00})", X, Y);
        }
    }

    public abstract class GameUnit
    {
        public abstract double Radius { get; }

        public Vector Position { get; set; }

        public Vector Velocity { get; set; }

        public double Distance(GameUnit other)
        {
            return (Position - other.Position).Size;
        }

        public double Distance2(GameUnit other)
        {
            return (Position - other.Position).Size2;
        }

        public static Collision CollisionData(GameUnit A, GameUnit B)
        {
            Vector DeltaP = A.Position - B.Position;
            Vector DeltaV = A.Velocity - B.Velocity;

            double srad2 = (A.Radius + B.Radius) * (A.Radius + B.Radius);

            double c = DeltaP.Size2 - srad2;
            if (c < 0)
            {
                //  elementen botsen al
                return new Collision(A, B, 0.0);
            }
            double b = (DeltaP * DeltaV) * 2.0;
            double a = DeltaV.Size2;
            double det = b * b - 4 * a * c;
            if (det < 0.0)
            {
                // elementen botsen nooit;   
                return null;
            }
            double t = (-b - Math.Sqrt(det)) / (2 * a);
            if (t < 0.0)
            {
                // botsing vind plaats in het verleden
                return null;
            }
            if (t > 1.0)
            {
                // botst niet deze beurt
                return null;
            }
            return new Collision(A, B, t);
        } //*/


        public override string ToString()
        {
            return string.Format("{0}  {1}", Position, Velocity);
        }
    }

    public class Checkpoint : GameUnit
    {
        public Checkpoint(int id, int x, int y)
        {
            CheckpointId = id;
            Position = new Vector(x, y);
            Velocity = new Vector(0.0, 0.0);
        }

        public int CheckpointId { get; set; }

        public override double Radius { get { return 200; } }
    }

    public class Pod : GameUnit
    {
        public Pod Clone()
        {
            return new Pod(Position, Velocity, AngleDeg, _nextCheckPointId, _lap);
        }

        public Pod() : base()
        {
        }

        protected Pod(Vector position, Vector velocity, double angleDeg, int nextCheckPointId, int lap)
        {
            Position = position;
            Velocity = velocity;
            AngleDeg = angleDeg;
            _nextCheckPointId = nextCheckPointId;
            _lap = lap;
        }

        public override double Radius { get { return 400; } }

        public double AngleDeg { get; set; }

        public double AngleRad
        {
            get { return AngleDeg * Math.PI / 180.0; }
        }

        private int _lap = 1;
        private int _checkpointNr = 0;

        public int CheckPointNr
        {
            get { return _checkpointNr; }
        }

        public int Lap
        {
            get { return _lap; }
        }

        private int _nextCheckPointId = 1;

        public int NextCheckPointId
        {
            get { return _nextCheckPointId; }
        }

        public void NextCheckPoint()
        {
            _nextCheckPointId = (_nextCheckPointId + 1) % Player.raceInfo.CheckpointCount;
            if (_nextCheckPointId == 1)
            {
                _lap++;
            }
            _checkpointNr++;
        }

        public bool Shield { get; set; }

        public void UpdateValues(int x, int y, int vx, int vy, int angleDeg, int nextCheckPointId)
        {
            Position = new Vector(x, y);
            Velocity = new Vector(vx, vy);
            AngleDeg = angleDeg;

            if (_nextCheckPointId != nextCheckPointId)
            {
                if (nextCheckPointId == 1)
                {
                    _lap++;
                }
                _checkpointNr++;
            }
            _nextCheckPointId = nextCheckPointId;
        }

        // simulation methods
        public double TargetAngleDeg(Vector targetPoint)
        {
            return (targetPoint - this.Position).AngleDeg;
        }

        public double AngleDiferance(Vector targetPoint)
        {
            double a = TargetAngleDeg(targetPoint);
            double r = AngleDeg <= a ? a - AngleDeg : a - AngleDeg + 360.0;
            double l = AngleDeg >= a ? AngleDeg - a : AngleDeg - a + 360.0;
            //   Console.Error.WriteLine("face {0} target {1} < {2} >{3}", AngleDeg, a, l, r  );
            return (r < l) ? r : -l;
        }

        public void Rotate(Vector targetPoint)
        {
            double a = AngleDiferance(targetPoint);
            if (a > 18.0) { a = 18.0; }
            else if (a < -18.0) { a = -18.0; }
            AngleDeg += a;
            if (AngleDeg >= 360.0) { AngleDeg -= 360.0; }
            else if (AngleDeg < 0.0) { AngleDeg += 360.0; }
        }

        public void Thrust(int thrust)
        {
            if (Shield)
            {
                return;
            }
            Velocity += Vector.CreateVectorAngleSizeRad(AngleRad, thrust);
        }

        public void Move()
        {
            Position += Velocity;
            Position.Round();
            Velocity *= 0.85;
            Velocity.Truncate();
            AngleDeg = Math.Round(AngleDeg);
        }

        public Move GetBlockerAction()
        {
            return GetRunnerAction();
        }

        public Move GetRunnerAction()
        {
            Vector target = Player.raceInfo.Checkpoints[NextCheckPointId].Position - Position - Velocity;

            double rotation = target.AngleRad - AngleRad;
            Move move = new Move();
            move.Rotation = rotation;
            move.Thrust = 100;
            return move;
        }

        public override string ToString()
        {
            return string.Format("pod p{0} v{1} a{2} c {3} lap {4}, ", Position, Velocity, AngleDeg, NextCheckPointId, Lap);
        }
    }

    public class Collision
    {
        public Collision(GameUnit a, GameUnit b, double time)
        {
            A = a;
            B = b;
            T = time;
        }

        public GameUnit A { get; set; }
        public GameUnit B { get; set; }
        public double T { get; set; }

        public override string ToString()
        {
            return string.Format("time: {0}", T);
        }

        public void Bounce()
        {
            if ((A is Pod) && (B is Pod))
            {
                // move to colliction point
                A.Position += A.Velocity * T;
                B.Position += B.Velocity * T;

                // bepaal massa
                double mass1 = (A as Pod).Shield ? 10 : 1;
                double mass2 = (B as Pod).Shield ? 10 : 1;

                double massCoefficient = (mass1 + mass2) / (mass1 * mass2);

                Vector DeltaP = (A.Position - B.Position);
                double DeltaPSize2 = DeltaP.Size2; // ~= 640 000

                Vector DeltaV = A.Velocity - B.Velocity;
                double CrosProduct = DeltaP * DeltaV;
                double forceFactor = CrosProduct / (DeltaPSize2 * massCoefficient);
                Vector Force = DeltaP * forceFactor;

                // pas impuls overdacht toe zodat bijde voorwerpen dezelfde velocity hebben
                A.Velocity -= Force / mass1;
                B.Velocity += Force / mass2;
                // minimale impuls voor van elkaar afstoten is 120
                if (Force.Size < 120.0)
                {
                    Force.Normalize(120.0);
                }
                // pas impuls overdacht toe zodat de voorwerpen uit elkaar bewegen
                A.Velocity -= Force / mass1;
                B.Velocity += Force / mass2;

                // reken terug waar de objecten nu vandaag gekomen zouden zijn
                A.Position -= A.Velocity * T;
                B.Position -= B.Velocity * T;
            }
        }

    }

    public class Solution
    {
        public Solution(List<Move> expectedEnemyMoves, bool random)
        {
            _expectedEnemyMoves = expectedEnemyMoves;
            if (random)
            {
                Randomize();
            }
        }

        public Solution Mutate()
        {
            Solution newSolution = new Solution(_expectedEnemyMoves, false);
            newSolution.Moves = new List<Move>();
            for (int i = 0; i < 6; i++)
            {
                newSolution.Moves.Add(Moves[i * 2].Mutate(0.1 * i));
                newSolution.Moves.Add(Moves[i * 2 + 1].Mutate(0.1 * i));
            }
            return newSolution;
        }

        private List<Move> _expectedEnemyMoves;

        public List<Move> Moves { get; set; }

        public void Randomize()
        {
            Moves = new List<Move>();
            for (int i = 0; i < 12; i++)
            {
                Moves.Add(new Move());
            }
        }

        public void Simulate(Pod[] pods)
        {
            Pod[] simPods = new Pod[4];
            for (int i = 0; i < 4; i++)
            {
                simPods[i] = pods[i].Clone();
            }
            for (int m = 0; m < 12; m += 2)
            {
                simPods[0].Rotate(Moves[m].Target(simPods[0]));

                simPods[0].Thrust(Moves[m].Thrust);
                simPods[1].Rotate(Moves[m + 1].Target(simPods[1]));
                simPods[1].Thrust(Moves[m + 1].Thrust);

                simPods[2].Rotate(_expectedEnemyMoves[m].Target(simPods[2]));
                simPods[2].Thrust(_expectedEnemyMoves[m].Thrust);
                simPods[3].Rotate(_expectedEnemyMoves[m + 1].Target(simPods[3]));
                simPods[3].Thrust(_expectedEnemyMoves[m + 1].Thrust);

                for (int i = 0; i < 3; i++)
                {
                    for (int j = i + 1; j < 4; j++)
                    {
                        Collision data = GameUnit.CollisionData(simPods[i], simPods[j]);
                        if (data != null)
                        {
                            data.Bounce();
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    // check waypoints
                    Collision data = GameUnit.CollisionData(simPods[i], Player.raceInfo.Checkpoints[simPods[i].NextCheckPointId]);
                    if (data != null)
                    {
                        simPods[i].NextCheckPoint();
                    }
                }

                simPods[0].Move();
                simPods[1].Move();
                simPods[2].Move();
                simPods[3].Move();
            }
            Score = CalculateScore(simPods);
        }

        public double Score { get; private set; }

        public double CalculateScore(Pod[] pods)
        {
            double myScore = PlayerScore(pods[0], pods[1]);
            return myScore;
            //       double opponentScore = PlayerScore(pods[2], pods[3]);
            //      return myScore - opponentScore;
        }

        private double PlayerScore(Pod pod1, Pod pod2)
        {
            Pod runner = null;
            Pod blocker = null;
            WhoRunnerBlocker(pod1, pod2, out runner, out blocker);
            return RunnerScore(runner) + BlokkerScore(blocker);
        }

        public static void WhoRunnerBlocker(Pod pod1, Pod pod2, out Pod runner, out Pod blocker)
        {
            if (pod1.CheckPointNr > pod2.CheckPointNr)
            {
                runner = pod1; blocker = pod2;
            }
            else if (pod1.CheckPointNr < pod2.CheckPointNr)
            {
                runner = pod2; blocker = pod1;
            }
            else
            {
                Checkpoint cp = Player.raceInfo.Checkpoints[pod1.NextCheckPointId];
                double d1 = (cp.Position - pod1.Position).Size2;
                double d2 = (cp.Position - pod2.Position).Size2;
                if (d1 > d2)
                {
                    runner = pod2; blocker = pod1;
                }
                else
                {
                    runner = pod1; blocker = pod2;
                }
            }
        }
        public static double RunnerScore(Pod runnnerpod)
        {
            return runnnerpod.CheckPointNr * 16000 - (Player.raceInfo.Checkpoints[runnnerpod.NextCheckPointId].Position - runnnerpod.Position).Size;
        }

        public static double BlokkerScore(Pod blokkerPod)
        {
            return 0.0;
        }


        public void NextTurn()
        {
            Moves.RemoveAt(0);
            Moves.RemoveAt(0);
            _expectedEnemyMoves.RemoveAt(0);
            _expectedEnemyMoves.RemoveAt(0);
            Moves.Add(new Move());
            Moves.Add(new Move());
            _expectedEnemyMoves.Add(new Move());
            _expectedEnemyMoves.Add(new Move());
        }
    }

    public class Move
    {
        private double[] _moveData = new double[3]; // element 0 => rotation element 1 => trust, 2=> shield usage

        private static Random random = new Random();

        public Move(double[] moveData)
        {
            _moveData = moveData;
        }

        public Move()
        {
            _moveData[0] = random.NextDouble();
            _moveData[1] = random.NextDouble();
            _moveData[2] = random.NextDouble();
        }

        public Move Mutate(double amplitude)
        {
            double f = (random.NextDouble() - 0.5) * amplitude;
            double[] moveData = new double[3];
            for (int i = 0; i < 3; i++)
            {
                moveData[i] = _moveData[i] + f;
                if (moveData[i] < 0.0) { moveData[i] = 0.0; }
                if (moveData[i] > 1.0) { moveData[i] = 1.0; }
            }
            return new Move(moveData);
        }

        // rotation in radials <0 is left, <0 is right
        public double Rotation
        {
            get
            {
                return (_moveData[0] - 0.5); // 1=> +/- 28 graden             
            }
            set
            {
                _moveData[0] = value + 0.5;
            }
        }

        public int Thrust
        {
            get
            {
                int t = (int)(_moveData[1] * 125.0);
                if (t > 100) { t = 100; }
                return t;
            }
            set
            {
                _moveData[1] = value;
                _moveData[1] /= 125.0;
            }
        }

        public bool Shield
        {
            get
            {
                return _moveData[2] > 0.95;
            }
        }

        public Vector Target(Pod pod)
        {
            return Vector.CreateVectorAngleSizeRad(Rotation + pod.AngleRad, 1000);
        }
    }

    public class Action
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Trust { get; set; }
        public bool Boost { get; set; }
        public bool Shield { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y,
             (Boost ? "BOOST" : (Shield ? "SHIELD" : Trust.ToString())));
        }
    }


    public class RaceInfo
    {

#if !DEBUG
    public RaceInfo()
    {
        string input = Console.ReadLine();
        _lapCount = int.Parse(input);
        input = Console.ReadLine();
        _checkpointCount = int.Parse(input);
        _checkpoints = new Checkpoint[_checkpointCount];
        for (int i = 0; i < _checkpointCount; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            _checkpoints[i] = new Checkpoint(i, int.Parse(inputs[0]), int.Parse(inputs[1]));
        }
    }
#else
        public RaceInfo()
        {
            _lapCount = 3;
            _checkpointCount = 6;
            _checkpoints = new[]{  new Checkpoint(0, 9507 ,4404 ),
                                new Checkpoint(1,14539,7796),
                                new Checkpoint(2, 6293,4297),
                                new Checkpoint(3, 7810,881),
                                new Checkpoint(4, 7689,5944),
                                new Checkpoint(5,3129,7530),};
        }
#endif

        private int _lapCount;
        private int _checkpointCount;
        private Checkpoint[] _checkpoints;

        public int LapCount { get { return _lapCount; } }
        public int CheckpointCount { get { return _checkpointCount; } }
        public Checkpoint[] Checkpoints { get { return _checkpoints; } }
    }

    /*


    class Model
    {
        private bool firstFrame = true;

        private Point _position;
        private Point _prevPosition;
        private Point _target;
        private Point _prevOpponent;
        private Point _opponent;
        private int _nextCheckpointDist;
        private int _nextCheckpointAngle;

        private List<Point> WayPoints = new List<Point>();


        public Point Position { get { return _position; } }
        public Point Target { get { return _target; } }
        public Point Opponent { get { return _opponent; } }
        public int NextCheckpointAngle { get { return _nextCheckpointAngle; } }
        public int NextCheckpointDist { get { return _nextCheckpointDist; } }

        public void LoadGameInfo()
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            _prevPosition = _position;
            _position = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            _target = new Point(int.Parse(inputs[2]), int.Parse(inputs[3]));// x position of the next check point // y position of the next check point
            _nextCheckpointDist = int.Parse(inputs[4]); // distance to the next checkpoint
            _nextCheckpointAngle = int.Parse(inputs[5]); // angle between your pod orientation and the direction of the next checkpoint
            inputs = Console.ReadLine().Split(' ');
            _prevOpponent = _opponent;
            _opponent = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            if (firstFrame)
            {
                _prevOpponent = _opponent;
                _prevPosition = _position;
                firstFrame = false;
            }
        }

        public Vector Velocity { get { return _position - _prevPosition; } }

        public Vector TargetVector { get { return _target - _position; } }

        public void PrintDebug()
        {
            Console.Error.WriteLine("pos: {0} speed: {1} ", _position, Velocity);
            Vector wayPoint = _target - _position;
            Console.Error.WriteLine("nCA: {0} nCD: {1} vector: {2}", _nextCheckpointAngle, _nextCheckpointDist, wayPoint);

            Console.Error.WriteLine(Vector.FromAngeleSize(45, 1000));
        }
    }

    abstract class PodModel
    {
        private Point _position = default(Point);
        private Vector _speed = default(Vector);
        private int _angle = default(int);
        private int _nexCheckPointId = default(int);

        public Point Position { get { return _position; } }
        public Vector Speed { get { return _speed; } }
        public int Angle { get { return _angle; } }
        public int nextCheckPointId { get { return _nexCheckPointId; } }

        protected Vector TargetVector
        {
            get
            {
                return Player.raceInfo.Checkpoints[_nexCheckPointId] - _position;
            }
        }

        public void ReadValues()
        {
            string[] inputs = Console.ReadLine().Split(' ');
            _position = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));
            _speed = new Vector(int.Parse(inputs[2]), int.Parse(inputs[3]));
            _angle = int.Parse(inputs[4]);
            _nexCheckPointId = int.Parse(inputs[5]);
        }
    }

    class OwnPod : PodModel
    {
        private bool boostUsed = false;



        public string GetAction()
        {
            int thrust;
            int relangle = (540 + TargetVector.AngleDeg - Angle) % 360 - 180;
              Console.Error.WriteLine(relangle);    
            if ((relangle > 90) || (relangle < -90))
            {
                thrust = 10;
            }
            else
            {
                thrust = TargetVector.Size / 12;
                if (thrust > 100)
                {
                    thrust = 100;
                }
            }
            string thrustStr = thrust.ToString();
            if ((thrust == 100) != boostUsed)
            {
                if ((Math.Abs(relangle) < 5) && (TargetVector.Size > 6000))
                {
                    thrustStr = "BOOST";
                    boostUsed = true;
                }
            }
        //    model.PrintDebug();
        //    Console.Error.WriteLine((model.NextCheckpointAngle + model.TargetVector.AngleDeg) - model.Velocity.AngleDeg);

            Vector TrustV = TargetVector - Speed;
            Point p = Position + TrustV;
            return String.Format("{0} {1} {2}", p.X, p.Y, thrustStr);
        }
    }

    class OpponentPod : PodModel
    {

    }


    */
}
