using System.Collections.Generic;
using Verse;

namespace DanielRenner.RennOrganisms
{
    public class FeedOptionDetails
    {
        public FeedOptions Feed;
        public int MoodOffset;
        public float ThreatMultiplier;
        public int MinThreatReduction;
    }

    public class Building_RennBaseDef : ThingDef {
        public List<FeedOptionDetails> Feeds;
        public int MaxThreatCap = int.MaxValue;
        // base effects without power / feeding
        public bool RequiresPowerForBaseRennEffect;
        public float ThreatMultiplierBase;
        public int MinThreatReductionBase;
        public int MoodOffsetBase;
    }
}
