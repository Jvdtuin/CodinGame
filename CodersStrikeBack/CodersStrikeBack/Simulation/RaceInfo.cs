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
    }
#else
        public RaceInfo()
        {
            Random r = new Random();
            _lapCount = 3;
            _checkpointCount = r.Next(3, 6);

            _checkpoints = new Checkpoint[_checkpointCount];

            for (int i=0; i<_checkpointCount; i++)
            {
                _checkpoints[i] = new Checkpoint(i, r.Next(1000, 15000), r.Next(1000, 8000));
            }
        }
#endif

        private int _lapCount;
        private int _checkpointCount;
        private Checkpoint[] _checkpoints;

        public int LapCount { get { return _lapCount; } }
        public int CheckpointCount { get { return _checkpointCount; } }
        public Checkpoint[] Checkpoints { get { return _checkpoints; } }
    }
}
