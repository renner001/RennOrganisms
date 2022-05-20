using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DanielRenner.RennOrganisms
{
    public class RennPondSettings
    {
        public int moodEffect = 0;
        public int tickLastReported = Find.TickManager.TicksGame;
        public int threatCap = int.MaxValue;
        public int minThreatReduction = 0;
        public float threatMultiplier = 1.0f;
    }

    public class GameComponent_RennPondManager : GameComponent
    {
        // saved settings
        Dictionary<Thing, RennPondSettings> knownThreatSettings = new Dictionary<Thing, RennPondSettings>();

        public GameComponent_RennPondManager(Game game)
        {
            Log.Debug("GameComponent_RennPondManager created");
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (Find.TickManager.TicksGame % GenTicks.TickRareInterval == 0)
            {
                foreach (var map in Find.Maps)
                {
                    if (map.IsPlayerHome)
                    {
                        applyMoodEffect(map, GetCurrentSettings(map).moodEffect);
                    }
                }
            }
        }

        private void applyMoodEffect(Map map, int moodEffect)
        {
            if (map == null)
            {
                return;
            }
            var mapHumanlikes = map.mapPawns.AllPawns.Where(pawn => { return pawn.RaceProps != null && pawn.RaceProps.Humanlike && pawn.needs != null && pawn.needs.mood != null; });
            // todo: increase performance by only doing this once and caching the results
            var rightStage = DefOfs_RennOrganisms.RennOrganisms.stages.FindIndex(stage => { return stage.baseMoodEffect == -1 * moodEffect; });
            if (rightStage < 0)
            {
                Log.ErrorOnce("missing stage for mood effect " + moodEffect, 849462741);
                rightStage = 0;
            }
            Log.Debug("total of " + mapHumanlikes.Count() + " pawns found on map " + map + " for a " + moodEffect + " negative mood effect through moodlet stage=" + rightStage + ": " + String.Join(", ", mapHumanlikes.Select(pawn => pawn.Name)));
            foreach (var pawn in mapHumanlikes)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefOfs_RennOrganisms.RennOrganisms, rightStage));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            try
            {
                List<Thing> tempKeyList = new List<Thing>();
                List<RennPondSettings> tempValueList = new List<RennPondSettings>();
                Scribe_Collections.Look(ref knownThreatSettings, "knownThreatSettings", LookMode.Reference, LookMode.Value, ref tempKeyList, ref tempValueList);
            } catch (Exception ex)
            {
                Log.Warning("Failed to load settings of GameComponent_RennPondManager. This is an error the game will recover from within the next seconds. Details: " + ex);
            }
            Log.Debug("GameComponent_RennPondManager.ExposeData(): " + String.Join(", ", knownThreatSettings.Select(setting => { return setting.Key.ToString() + ": threatCap=" + setting.Value.threatCap + "; threatMultiplier=" + setting.Value.threatMultiplier; })));
        }

        public RennPondSettings GetCurrentSettings()
        {
            Log.DebugOnce("GameComponent_RennPondManager.GetCurrentSettings() is getting called");
            shrinkOldSettings();
            if (knownThreatSettings.Count > 0)
            {
                var bestEntry = knownThreatSettings.MinBy(setting => { return setting.Value.threatMultiplier; });
                return bestEntry.Value;
            }
            return new RennPondSettings();
        }
        public RennPondSettings GetCurrentSettings(Map map)
        {
            Log.DebugOnce("GameComponent_RennPondManager.GetCurrentSettings() is getting called");
            shrinkOldSettings();
            if (knownThreatSettings.Count > 0 && knownThreatSettings.First(setting => { return setting.Key.Map == map; }).Value != null)
            {
                var bestEntry = knownThreatSettings.Where(setting => { return setting.Key.Map == map; }).MinBy(setting => { return setting.Value.threatMultiplier; });
                return bestEntry.Value;
            }
            return new RennPondSettings();
        }

        public void SetThreatPoints(Thing sender, RennPondSettings settings)
        {
            settings.tickLastReported = Find.TickManager.TicksGame;
            knownThreatSettings[sender] = settings;
        }

        public void RemoveTrackedThing(Thing thing)
        {
            Log.Debug("GameComponent_RennPondManager.RemoveTrackedThing() of " + thing);
            if (knownThreatSettings.ContainsKey(thing))
            {
                knownThreatSettings.Remove(thing);
            }
        }

        private void shrinkOldSettings()
        {
            var oldestValidTick = Find.TickManager.TicksGame - GenTicks.TickLongInterval;
            var outdatedEntries = knownThreatSettings.Where(setting => { return setting.Value.tickLastReported < oldestValidTick; });
            foreach (var entry in outdatedEntries)
            {
                Log.Warning("An object seems to have gotten lost: " + entry.Key);
                knownThreatSettings.Remove(entry.Key);
            }
        }
    }
}
