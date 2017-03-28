using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.AI
{
    public class Race
    {

        private Pod[] _pods;
        private IPodBrain[] _podBrains;
        private RaceInfo _raceInfo;


        public Race(RaceInfo raceInfo)
        {
            _raceInfo = raceInfo;
            int podCount = 2;
            _pods = new Pod[podCount];
            for (int i=0; i< podCount; i++)
            {
                _pods[i] = new Pod();
            }
            _podBrains = new IPodBrain[podCount];
            InitializePods(podCount);
            _podBrains[0] = new TransformedPodBrain(); _podBrains[0].SetConditions(_pods[0], _raceInfo, new[] { 2.0, -300.0, 0.5 });
            _podBrains[1] = new SimpleSeekPodBrain(_pods[1], _raceInfo, new[] { 2.5 });
            //   _podBrains[2] = new SimpleSeekPodBrain(_pods[2], _raceInfo, new[] { 3.0 });
            //   _podBrains[3] = new SimpleSeekPodBrain(_pods[3], _raceInfo, new[] { 4.0 });
        }

        private void InitializePods(int podCount)
        {
            bool[] positions = new bool[podCount];

            Random r = new Random();
            double startline = (_raceInfo.Checkpoints[0].Position - _raceInfo.Checkpoints[1].Position).AngleRad + 0.5 * Math.PI;
            for (int i = 0; i < podCount; i++)
            {

                int posnr = 0;
                do
                {
                    posnr = r.Next(podCount);
                } while (positions[posnr]);
                positions[posnr] = true;

                double dist = ((double)posnr - ((double)podCount - 1.0) / 2.0) * 1000;



                Vector v = _raceInfo.Checkpoints[0].Position + Vector.CreateVectorAngleSizeRad(startline, dist);

                int x = (int)v.X;
                int y = (int)v.Y;

                _pods[i].UpdateValues(x, y, 0, 0, 0, 1);
            }
        }

        public Race(RaceInfo raceInfo, IPodBrain[] podBrains)
        {
            _podBrains = podBrains;
            _raceInfo = raceInfo;
            _pods = new Pod[podBrains.Length];

            for (int i=0; i<podBrains.Length; i++)
            {
                _pods[i] = podBrains[i].GetPod();
            }
            InitializePods(_pods.Length);

        }


        public int? Move()
        {
            // iedere pod bepaalt zijn actie
            foreach (IPodBrain podbrain in _podBrains)
            {
                podbrain.GetAction();
            }

            // draai en stel throttle in
            foreach (Pod pod in _pods)
            {
                pod.Rotate();
                pod.Thrust();
            }
            // bereken de botsingen

            List<Collision> collisions = new List<Collision>();
            for (int i = 0; i < _pods.Length - 1; i++)
            {
                for (int j = i + 1; j < _pods.Length; j++)
                {
                    Collision collision = GameUnit.CollisionData(_pods[i], _pods[j]);
                    if (collision != null)
                    {
                        collisions.Add(collision);
                    }
                }
            }

            foreach (Collision c in collisions)
            {
                c.Bounce();
            }

            // plaats pods op nieuwe locaties
            for(int i = 0; i<_pods.Length; i++)
            {
                Pod pod = _pods[i];
                pod.Move();
                // bepaal of pod checkpoint heeft bereikt

                if ((_raceInfo.Checkpoints[pod.NextCheckPointId].Position - pod.Position).Size < 600)
                {
                    pod.NextCheckPointId = (pod.NextCheckPointId + 1) % _raceInfo.CheckpointCount;
                    if (pod.Lap > _raceInfo.LapCount)
                    {
                        return i ;
                    }
                }
            }
            return null;
        }

        public Pod[] Pods { get { return _pods; } }

    }
}
