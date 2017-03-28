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

        private RaceInfo[] _races = new RaceInfo[10];

        private Type _type;


        public Optimizer()
        {
            for (int i = 0; i < 10; i++)
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

        }

        public void CalcultatePopulationScores()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start(); 
            foreach(Creature creature in _population)
            {
                int s = CalculateScore(creature.genome);
                creature.Score = s;
            }
            sw.Stop();

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
            for (int i = 0; i < 10; i++)
            {
                int s = RunRace(_races[i], factors);
                if (s == 2000) { s = 5000; }
                score += s;
            }
            return score;
        }


    }
}
