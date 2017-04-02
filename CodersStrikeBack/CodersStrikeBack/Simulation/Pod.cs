using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
    public class Pod : GameUnit
    {
        private int _lap = 1;
        private int _checkpointNr = 0;



        public Pod() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="angleDeg"></param>
        /// <param name="nextCheckPointId"></param>
        /// <param name="lap"></param>
        protected Pod(Vector position, Vector velocity, double angleDeg, int nextCheckPointId, int lap)
        {
            Position = position;
            Velocity = velocity;
            AngleDeg = angleDeg;
            _nextCheckPointId = nextCheckPointId;
            _lap = lap;
        }

        /// <summary>
        /// radius used for calculation collisions
        /// </summary>
        public override double Radius { get { return 400; } }

        /// <summary>
        /// te rotation angle of the pod
        /// </summary>
        public double AngleDeg { get; set; }


        public double AngleRad
        {
            get { return AngleDeg * Math.PI / 180.0; }
        }



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
            set
            {
                _nextCheckPointId = value;
                if (value == 1)
                {
                    _lap++;
                }
            }
        }

        public bool FirstTurn { get { return _turns == 0; } }

        public bool Shield
        {
            get;
            set;
        }

        public Pod Clone()
        {
            return new Pod(Position, Velocity, AngleDeg, _nextCheckPointId, _lap);
        }

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




        #region simulation methods
        // simulation methods

        private Vector _targetPoint;
        private int _thrust;
        private int _shieldCountDown;
        private bool _boosedUsed = false;
        private int _turns = 0;

        public void SetAction(Vector targetPoint, int thrust, bool useShield, bool useBoost)
        {
            _targetPoint = targetPoint;
            Shield = useShield;
            if (useShield)
            {
                _shieldCountDown = 3;
            }
            if (_shieldCountDown > 0)
            {
                _thrust = 0;
            }
            else
            {
                if (useBoost && !_boosedUsed)
                {
                    _boosedUsed = true;
                    _thrust = 650;
                }
                else
                {
                    _thrust = thrust;
                }
            }          
        }
        
        private double TargetAngleDeg(Vector targetPoint)
        {
            return (targetPoint - this.Position).AngleDeg;
        }

        private double AngleDiferance(Vector targetPoint)
        {
            double a = TargetAngleDeg(targetPoint);
            double r = AngleDeg <= a ? a - AngleDeg : a - AngleDeg + 360.0;
            double l = AngleDeg >= a ? AngleDeg - a : AngleDeg - a + 360.0;
            //   Console.Error.WriteLine("face {0} target {1} < {2} >{3}", AngleDeg, a, l, r  );
            return (r < l) ? r : -l;
        }

        public void Rotate()
        {
            double a = AngleDiferance(_targetPoint);
            if (_turns > 0)
            {
                if (a > 18.0) { a = 18.0; }
                else if (a < -18.0) { a = -18.0; }
            }
            AngleDeg += a;
            if (AngleDeg >= 360.0) { AngleDeg -= 360.0; }
            else if (AngleDeg < 0.0) { AngleDeg += 360.0; }
        }

        public void Thrust()
        {
            Velocity += Vector.CreateVectorAngleSizeRad(AngleRad, _thrust);
        }

        public void Move()
        {
            Position += Velocity;
            Position.Round();
            Velocity *= 0.85;
            Velocity.Truncate();
            AngleDeg = Math.Round(AngleDeg);
            _turns++;
           
        }

        #endregion

        public override string ToString()
        {
            return string.Format("pod p{0} v{1} a{2} c {3} lap {4}, ", Position, Velocity, AngleDeg, NextCheckPointId, Lap);
        }
    }
}