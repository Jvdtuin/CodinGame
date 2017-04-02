using CodersStrikeBack.AI;
using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Optimizer
{
    class Optimizer<TBrain> where TBrain:IPodBrain
    {
        class Creature
        {
            public double[] genome { get; set; }
            public int Score { get; set; } 
        }

        private List<Creature> _population = new List<Creature>();

        private static Random _random = new Random();

        private double[] _factors;

        private RaceInfo[] _races;
        private int _numRaces;

        private Type _type;

        private int _genomeSize;

        public Optimizer(int numRaces)
        {
            _numRaces = numRaces;
            _races = new RaceInfo[numRaces];

            for (int i = 0; i < numRaces; i++)
            {
                _races[i] = new RaceInfo();
            }

            for (int i= 0; i < 100; i++ )
            {
                _population.Add(new Creature()
                {
                    genome = new[] { _random.NextDouble() * 5.0, _random.NextDouble() * 600.0, _random.NextDouble() * 0.2 }
                });
            }
            _genomeSize = 3;
        }

        public int CalcultatePopulationScores()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();


             Parallel.ForEach(_population, (creature) =>
             {
                 int s = CalculateScore(creature.genome);
                 creature.Score = s;

             }
                ); 
            _population.Sort((c1, c2) =>
            {
                if (c1.Score < c2.Score) { return -1; }
                if (c1.Score > c2.Score) { return 1; }
                return 0;
            });

            sw.Stop();
            return _population[0].Score;
        }

        public void NextGeneration()
        {
            List<Creature> parants = SelectParents();
            _population = NextGeneration(parants);
        }

        private List<Creature> SelectParents()
        {
            var result = _population.Where(c => c.Score <= _population[10].Score).ToList();
            if (result.Count > 10)
            {
                for (int i =1; i< result.Count; i++)
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

        private Creature[] Cross(Creature c1, Creature c2 )
        {
            Creature[] result = new Creature[2];

            int pos = _random.Next(_genomeSize - 1);

            double[] genome1 = new double[_genomeSize];
            double[] genome2 = new double[_genomeSize];
            for (int i =0; i< _genomeSize; i++)
            {
                genome1[i] = i > pos ? c2.genome[i] : c1.genome[i];
                genome2[i] = i > pos ? c1.genome[i] : c2.genome[i];
            }
            result[0] = new Creature()
            {
                genome = genome1,
            };
            result[1] = new Creature()
            {
                genome = genome2,
            };
            return result;
        }

        private void Mutate(Creature creature, double chance, double ammount)
        {
            for (int i = 0; i < _genomeSize; i++)
            {
                if (_random.NextDouble() < chance)
                {
                    double factor = 1.0 + (_random.NextDouble() - 0.5) * ammount;
                    creature.genome[i] *= factor;
                }
            }
            
        }



        public int RunRace(RaceInfo raceInfo, double[] factors)
        {
            int i = 0;
            IPodBrain[] brains = new IPodBrain[1];
            brains[0] = new TransformedPodBrain();
            Pod pod = new Pod();
            brains[0].SetConditions(pod, raceInfo, factors);
            Race race = new Race(raceInfo, brains);
            do
            {
                i++;
            } while ((i < 2000) && (!race.Move().HasValue));

            return i;
        }

        public int CalculateScore(double[] factors)
        {
            int score = 0;
            for (int i = 0; i < _numRaces; i++)
            {
                int s = RunRace(_races[i], factors);
                if (s == 2000) { s = 5000; }
                score += s;
            }
            return score;
        }


    }
}
