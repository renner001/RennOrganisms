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

    class Building_RennPortal : Building_RennBase
    {
        public Building_RennPortal()
        {
            baseMoodEffectOfBuilding = 9;
            minThreatLoss = 5000;
        }

    }
}
