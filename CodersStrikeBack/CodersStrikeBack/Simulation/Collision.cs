using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
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

}
