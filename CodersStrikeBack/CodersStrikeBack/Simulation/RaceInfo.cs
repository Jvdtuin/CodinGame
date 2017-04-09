using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
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
        FillDistances();
    }

#else
        private static Random r = new Random();

        public RaceInfo()
        {
            _lapCount = 3;
            _checkpointCount = r.Next(3, 6);

            _checkpoints = new Checkpoint[_checkpointCount];

            for (int i=0; i<_checkpointCount; i++)
            {
                _checkpoints[i] = new Checkpoint(i, r.Next(1000, 15000), r.Next(1000, 8000));
            }
            FillDistances();
        }


#endif
        private void FillDistances()
        {
            _distances = new List<double>();
            double distanceCum = 0.0;
            for (int l=_lapCount; l >0; l--)
            {
                for (int c=_checkpointCount- 1; c>=0;  c--)
                {
                    _distances.Insert(0, distanceCum);
                    Vector thisCheckPoint = _checkpoints[c].Position;
                    Vector nextCheckPoint = _checkpoints[(c + 1) % _checkpointCount].Position;

                    double distance = (thisCheckPoint - nextCheckPoint).Size;
                    distanceCum += distance + 2000 ;
                }
            }
        }

        private List<double> _distances;

        private int _lapCount;
        private int _checkpointCount;
        private Checkpoint[] _checkpoints;

        public int LapCount { get { return _lapCount; } }
        public int CheckpointCount { get { return _checkpointCount; } }
        public Checkpoint[] Checkpoints { get { return _checkpoints; } }

        public List<double> Distances
        {
            get { return _distances; }
        }
    }
}
