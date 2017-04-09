using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Simulation
{
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

        public static Collision GetCollisionData(GameUnit A, GameUnit B)
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
        } 

        public static double GetClossestTime (GameUnit a, GameUnit b, out double distance)
        {
            Vector DeltaP = a.Position - b.Position;
            Vector DeltaV = a.Velocity - b.Velocity;

            double t = -(DeltaP * DeltaV) / DeltaV.Size2;

            Vector d = DeltaP + DeltaV * t;
            distance = d.Size;

            return t;
        }


        public override string ToString()
        {
            return string.Format("{0}  {1}", Position, Velocity);
        }
    }
}
