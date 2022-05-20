using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    class Building_RennPond : Building_RennBase
    {

        public Building_RennPond()
        {
            baseMoodEffectOfBuilding = 3;
            minThreatLoss = 250;
        }

        protected override void updateThreatPoints(ref RennPondSettings settings)
        {
            var refuelable = GetComp<CompRefuelable>();
            var power = GetComp<CompPowerTrader>();
            int moodEffect = baseMoodEffectOfBuilding;
            if ((power == null || (power != null && power.PowerOn)) && refuelable.HasFuel)
            {
                switch (lastFed)
                {
                    case FeedOptions.RennFiber:
                        moodEffect += 3;
                        break;
                    case FeedOptions.RennFiberDomestic:
                        moodEffect += 6;
                        break;
                    case FeedOptions.RennPowder:
                        moodEffect += 9;
                        break;
                    case FeedOptions.RennConcentrate:
                        moodEffect += 12;
                        break;
                    case FeedOptions.RennCapsule:
                        moodEffect += 15;
                        break;
                }
            }
            
            settings.moodEffect = moodEffect;
            settings.threatMultiplier = 1.0f - 0.04f * moodEffect;
            settings.threatCap = int.MaxValue;
            settings.minThreatReduction = minThreatLoss;
        }
    }

}
