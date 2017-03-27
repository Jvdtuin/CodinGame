using CodersStrikeBack.AI;
using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.Optimizer
{
    class Optimizer<TBrain> where TBrain:IPodBrain
    {

        private double[] _factors;

        private RaceInfo[] _races = new RaceInfo[10];

        private Type _type;

        public Optimizer()
        {
       

            for (int i = 0; i < 10; i++)
            {
                _races[i] = new RaceInfo();
            }

            
        }

        
        public int RunRace(RaceInfo raceInfo)
        {
            int i = 0;

            IPodBrain[] brains = new  IPodBrain[1];
            brains[0] = new SimplePodBrain();

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
                IPodBrain brain = (IPodBrain)Activator.CreateInstance(_type);
                Pod pod = new Pod();


                pod.UpdateValues((int)_races[i].Checkpoints[0].Position.X, (int)_races[i].Checkpoints[0].Position.Y, 0, 0, 0, 1);


                brain.SetConditions(pod, _races[i], factors);

             //   score += RunRace(brain, _races[i]);

            }
            return score;
        }


    }
}
