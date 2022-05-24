using System.Collections.Generic;
using Verse;

namespace DanielRenner.RennOrganisms
{
    public class Building_RennBaseDef : ThingDef {
        public List<FeedOptions> availableFeedOptions;
        public int baseMoodEffectOfBuildingUnpoweredAndUnfueled;
        public int moodOffsetOfBuildingPowered;
        public float moodToThreatMultiplier = 0.04f;
        public int maxThreatCap = int.MaxValue;
        public int minThreatReduction = 0;

        public int moodOffsetFedRennFiber = 3;
        public int moodOffsetFedRennFiberDomestic = 6;
        public int moodOffsetFedRennPowder = 9;
        public int moodOffsetFedRennConcentrate = 12;
        public int moodOffsetFedRennCapsule = 15;
        //public FeedOptions defaultFeedOptionSelected; we simply use the 
    }
}
