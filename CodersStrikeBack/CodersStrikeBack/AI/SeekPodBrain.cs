using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.AI
{
    public class SeekPodBrain : IPodBrain
    {

        private Pod _pod;
        private RaceInfo _raceInfo;
        private double _factor;

        public SeekPodBrain(Pod pod, RaceInfo raceInfo, double[] factors)
        {
            _pod = pod;
            _raceInfo = raceInfo;
            _factor = factors[0];
        }

        public SeekPodBrain()
        {

        }

        public string GetAction()
        {
            Vector target = _raceInfo.Checkpoints[_pod.NextCheckPointId].Position;

            double a1 = (target - _pod.Position).AngleRad;
            double a2 = _pod.Velocity.AngleRad;

            double a3 = (4 * Math.PI + a1 - a2) % (2*Math.PI);

            Vector correction;

            double size = Math.Sin(a3) *  _pod.Velocity.Size;

            correction = Vector.CreateVectorAngleSizeRad(a1 + 0.5 * Math.PI, size * _factor);          
          
            

            int thrust = 100;
            
            _pod.SetAction(target+ correction , thrust, false, false);
            return string.Format("{0} {1} {2}", target.X, target.Y, thrust);


        }

        public Pod GetPod()
        {
            return _pod;
        }

        public int GetFactorCount()
        {
            return 1;
        }

        public void SetConditions(Pod pod, RaceInfo raceInfo, double[] factors)
        {
            _pod = pod;
            _raceInfo = raceInfo;
            _factor = factors[0];
        }
    }
}
