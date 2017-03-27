using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.AI
{
    public class SimplePodBrain : IPodBrain
    {
        public SimplePodBrain()
        {

        }

        public void SetConditions(Pod pod, RaceInfo raceInfo, double[] factors )
        {
            _pod = pod;
            _raceInfo = raceInfo;
            
        }

        public SimplePodBrain(Pod pod, RaceInfo raceInfo)
        {
            _pod = pod;
            _raceInfo = raceInfo;
        }

        private Pod _pod;
        private RaceInfo _raceInfo;

        public string GetAction()
        {
            Vector target = _raceInfo.Checkpoints[_pod.NextCheckPointId].Position;
            int thrust = 100;
            _pod.SetAction(target, thrust, false, false);
            return string.Format("{0} {1} {2}", target.X, target.Y, thrust);
        }

        public Pod GetPod()
        {
            return _pod;
        }

        public int GetFactorCount()
        {
            return 0;
        }
    }

    public class SimpleSeekPodBrain : IPodBrain
    {

        public SimpleSeekPodBrain() { }



        public SimpleSeekPodBrain(Pod pod, RaceInfo raceInfo, double[] factors)
        {
            _pod = pod;
            _raceInfo = raceInfo;
            _factor = factors[0];
        }

        private Pod _pod;
        private RaceInfo _raceInfo;
        private double _factor;

        public string GetAction()
        {
            Vector target = _raceInfo.Checkpoints[_pod.NextCheckPointId].Position - _pod.Velocity *_factor;
            int thrust = 100;
            _pod.SetAction(target, thrust, false, false);
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
