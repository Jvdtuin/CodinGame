using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodersStrikeBack.Simulation;

namespace CodersStrikeBack.AI
{
    public class TransformedPodBrain : IPodBrain
    {
        private Pod _pod;
        private RaceInfo _raceInfo;

        private double[] _factors;

        protected double SpeedCorrection { get { return _factors[0]; } }

        protected double NextWaypontCorrectionA { get { return _factors[1]; } }

        protected double NextWaypontCorrectionB { get { return _factors[2]; } }

        public string GetAction()
        {

            Vector position = _pod.Position;
            Vector velocity = _pod.Velocity;

            Vector firstCheckpoint = _raceInfo.Checkpoints[_pod.NextCheckPointId].Position;
            Vector secondCheckpoint = _raceInfo.Checkpoints[(_pod.NextCheckPointId + 1) % _raceInfo.CheckpointCount].Position;


            if (_pod.FirstTurn)
            {
                _pod.SetAction(firstCheckpoint, 100, false, false);
                return string.Format("{0} {1} {2}", firstCheckpoint.X, firstCheckpoint.Y, 100);
            }

            // transform 
            firstCheckpoint -= position;
            secondCheckpoint -= position;
            velocity = velocity.Rotate(-_pod.AngleRad);
            firstCheckpoint = firstCheckpoint.Rotate(-_pod.AngleRad);
            secondCheckpoint = secondCheckpoint.Rotate(-_pod.AngleRad);
            double size = Math.Sin(secondCheckpoint.AngleRad);
            size *= size;
            firstCheckpoint -= secondCheckpoint.Normalize((NextWaypontCorrectionA + NextWaypontCorrectionB * firstCheckpoint.Size )* size);
            double stuurhoek = 0.0;
            double thrust = 100.0;

            double rturns = Math.Abs(secondCheckpoint.AngleDeg / 18.0);
            double d = 0.0;
            double s = velocity.Size;
            for (int i=0; i<rturns; i++)
            {
                d += s;
                s *= 0.85;
            }

            if (firstCheckpoint.Size > d  )
            {

                Vector v1 = firstCheckpoint - velocity * SpeedCorrection ;
                 stuurhoek = (v1.AngleDeg > 18.0) ? 18.0 : (v1.AngleDeg < -18.0) ? -18.0 : v1.AngleDeg;
                 thrust = 100;
                if (v1.AngleDeg != stuurhoek)
                {
                    double hh = Math.Abs(v1.AngleDeg - stuurhoek);
                    thrust -= (hh > 90) ? 100 : hh * hh / 81.0;
                }
            }
            else
            {
                Vector v1 = secondCheckpoint;
                stuurhoek = (v1.AngleDeg > 18.0) ? 18.0 : (v1.AngleDeg < -18.0) ? -18.0 : v1.AngleDeg;
                thrust = 100;
                if (v1.AngleDeg != stuurhoek)
                {
                    double hh = Math.Abs(v1.AngleDeg - stuurhoek);
                    thrust -= (hh > 90) ? 100 : hh * hh / 81.0;
                }
            }


            // transform terug

            Vector v2 = Vector.CreateVectorAngleSizeRad((_pod.AngleDeg + stuurhoek) * Math.PI / 180, 10000.0);
            Vector target = _pod.Position + v2;

            _pod.SetAction(target, (int)thrust, false, false);
            return string.Format("{0} {1} {2}", target.X, target.Y, thrust);
        }

        public int GetFactorCount()
        {
            return 2;
        }

        public Pod GetPod()
        {
            return _pod;
        }

        public void SetConditions(Pod pod, RaceInfo raceInfo, double[] factors)
        {
            _pod = pod;
            _raceInfo = raceInfo;
            _factors = factors;
        }
    }
}
