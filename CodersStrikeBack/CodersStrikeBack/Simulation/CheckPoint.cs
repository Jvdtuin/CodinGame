using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
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
}
