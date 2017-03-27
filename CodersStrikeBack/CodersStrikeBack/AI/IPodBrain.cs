using CodersStrikeBack.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodersStrikeBack.AI
{
    public interface IPodBrain
    {
        string GetAction();

        void SetConditions(Pod pod, RaceInfo raceInfo, double[] factors);

        Pod GetPod();

        int GetFactorCount();

    }
}
