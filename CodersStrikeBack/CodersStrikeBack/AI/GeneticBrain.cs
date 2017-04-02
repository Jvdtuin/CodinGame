using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodersStrikeBack.Simulation;

namespace CodersStrikeBack.AI
{
    public class GeneticBrain : IPodBrain
    {
        private const int GenomeSize = 18;

        private static Random _random = new Random();

        private Pod _pod;
        private RaceInfo _raceInfo;
        private double[] _factors;
        private List<Creature> _population = new List<Creature>();

        public class Creature
        {
            public Creature()
            {
                Genome = new double[GenomeSize];
                for (int i =0; i< GenomeSize; i++)
                {
                    Genome[i] = _random.NextDouble();
                }

            }

            public double[] Genome { get; set; }

            public double Score { get; set; }
        }

        public GeneticBrain()
        {
            for (int i = 0; i < 100; i++)
            {
                _population.Add(new Creature());
            }
        }


        public string GetAction()
        {
            if (_pod.FirstTurn)
            {
                _pod.AngleDeg = (_raceInfo.Checkpoints[1].Position - _pod.Position).AngleDeg;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i > 0)
                {
                    NextGeneration();
                }
                foreach (Creature c in _population)
                {
                    CaluculateScore(c);
                }
                _population.Sort((c1, c2) =>
                {
                    if (c1.Score < c2.Score) { return -1; }
                    if (c1.Score > c2.Score) { return 1; }
                    return 0;
                });
            }


            double turnGen = _population[0].Genome[0];
            double trustGen = _population[0].Genome[1];
            //           double shieldGen = creature.Genome[i * 3 + 2];
            Vector v = Vector.CreateVectorAngleSizeRad(_pod.AngleRad + (turnGen - 0.5) * Math.PI / 5, 1000);
            v += _pod.Position;

            int thrust = (int)(trustGen * 120);
            thrust = (thrust > 100) ? 100 : thrust;
            _pod.SetAction(v, thrust, false, false);
            for (int i = 0; i < 10; i++)
            {
                MoveCreature(_population[i]);
            }

            for (int i =10; i< 100; i++)
            {
                _population[i] = new Creature();
            }

            return $"{v.X} {v.Y} {thrust}";
        }

        public int GetFactorCount()
        {
            return 0;
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

        public void CaluculateScore(Creature creature)
        {
            Pod creaturepod = _pod.Clone();
            int finnisch = (_pod.NextCheckPointId + 2)%_raceInfo.CheckpointCount;
            for (int i =0; i< GenomeSize / 3; i++) // move the pod x steps
            {
                double turnGen = creature.Genome[i*3];
                double trustGen = creature.Genome[i * 3 + 1];
                double shieldGen = creature.Genome[i * 3 + 2];
                Vector v = Vector.CreateVectorAngleSizeRad(creaturepod.AngleRad + (turnGen-0.5)  * Math.PI / 5.0, 1000);
                v += creaturepod.Position;

                int thrust = (int)(trustGen * 120);
                thrust = (thrust > 100) ? 100 : thrust;

                creaturepod.SetAction(v, thrust, false, false);
                creaturepod.Rotate();
                creaturepod.Thrust();
               // bounces here
                creaturepod.Move();

                if ((_raceInfo.Checkpoints[creaturepod.NextCheckPointId].Position - creaturepod.Position).Size < 600)
                {
                    creaturepod.NextCheckPointId = (creaturepod.NextCheckPointId + 1) % _raceInfo.CheckpointCount;
                }
            }

            // calculate the score of that position
            double score = 0;
            while (creaturepod.NextCheckPointId != finnisch && score < 100)
            {
                Vector v = _raceInfo.Checkpoints[creaturepod.NextCheckPointId].Position - creaturepod.Velocity * 2.5;
                creaturepod.SetAction(v, 100, false, false);
                creaturepod.Rotate();
                creaturepod.Thrust();

                creaturepod.Move();

                if ((_raceInfo.Checkpoints[creaturepod.NextCheckPointId].Position - creaturepod.Position).Size < 600)
                {
                    creaturepod.NextCheckPointId = (creaturepod.NextCheckPointId + 1) % _raceInfo.CheckpointCount;
                }
                score++;
            }
            

            creature.Score = score;
        }

        public void NextGeneration()
        {
            List<Creature> parants = SelectParents();
            _population = NextGeneration(parants);
        }

        private List<Creature> SelectParents()
        {
            List<Creature> result = _population.Where(c => c.Score <= _population[10].Score).ToList();
            if (result.Count > 10)
            {
                for (int i = 1; i < result.Count; i++)
                {
                    Mutate(result[i], 0.5, 0.005);
                }
            }

            return result;
        }

        private List<Creature> NextGeneration(List<Creature> paranents)
        {
            List<Creature> result = new List<Creature>();
            result.AddRange(paranents);
            while (result.Count < 100)
            {
                int parent1Index = _random.Next(paranents.Count);
                int parent2Index = _random.Next(paranents.Count - 1);
                if (parent1Index == parent2Index) { parent2Index++; }

                Creature[] young = Cross(paranents[parent1Index], paranents[parent2Index]);
                Mutate(young[0], 0.1, 0.01);
                Mutate(young[1], 0.1, 0.01);

                result.Add(young[0]);
                result.Add(young[1]);
            }
            return result;
        }

        private Creature[] Cross(Creature c1, Creature c2)
        {
            Creature[] result = new Creature[2];

            int pos = _random.Next(GenomeSize - 1);

            double[] genome1 = new double[GenomeSize];
            double[] genome2 = new double[GenomeSize];
            for (int i = 0; i < GenomeSize; i++)
            {
                genome1[i] = i > pos ? c2.Genome[i] : c1.Genome[i];
                genome2[i] = i > pos ? c1.Genome[i] : c2.Genome[i];
            }
            result[0] = new Creature()
            {
                Genome = genome1,
            };
            result[1] = new Creature()
            {
                Genome = genome2,
            };
            return result;
        }

        private void Mutate(Creature creature, double chance, double ammount)
        {
            for (int i = 0; i < GenomeSize; i++)
            {
                if (_random.NextDouble() < chance)
                {
                    double factor = 1.0 + (_random.NextDouble() - 0.5) * ammount;
                    creature.Genome[i] *= factor;
                }
            }

        }

        private void MoveCreature(Creature creature)
        {
            double[] nextGenome = new double[GenomeSize];
            for (int i = 3; i < GenomeSize; i++)
            {
                nextGenome[i - 3] = creature.Genome[i];
            }
            for (int i = GenomeSize - 3; i < GenomeSize; i++)
            {
                nextGenome[i] = _random.NextDouble();
            }
            creature.Genome = nextGenome;
        }

    }
}
