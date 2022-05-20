﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    class Building_RennReactor : Building_RennBase
    {
        public Building_RennReactor()
        {
            baseMoodEffectOfBuilding = 6;
            minThreatLoss = 2000;
        }
    }
}
