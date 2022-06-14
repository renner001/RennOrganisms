using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DanielRenner.RennOrganisms
{

    public abstract class Building_RennBase : Building
    {
        // saved variables
        public FeedOptions feedOptionSelected = FeedOptions.notDefined;
        public FeedOptions lastFed = FeedOptions.notDefined;

        public new Building_RennBaseDef def {
            get 
            {
                //Log.Debug("returning base.def=" + base.def + " as def=" + (base.def as Building_RennBaseDef));
                return base.def as Building_RennBaseDef;
            }
        }

        // temporary variables
        RennPondSettings mySettings;
        public List<FeedOptions> availableFeedOptions
        {
            get
            {
                return def.Feeds.Select(feed => feed.Feed).ToList();
            }
        }
        private Comp_SpecificRefuelable refuelable;
        protected Comp_SpecificRefuelable Refuelable
        {
            get 
            { 
                if (refuelable == null)
                {
                    refuelable = GetComp<Comp_SpecificRefuelable>();
                }
                return refuelable;
            }
        }
        private CompPowerTrader power;
        protected CompPowerTrader Power {
            get
            {
                if (power == null)
                {
                    power = GetComp<CompPowerTrader>();
                }
                return power;
            }
        }

        public override void PostMake()
        {
            Log.Debug("Building_RennBase.PostMake() called");
            base.PostMake();
            if (feedOptionSelected == FeedOptions.notDefined)
            {
                Log.Debug("Building_RennBase.PostMake() setting up inital feed setting to " + availableFeedOptions[0]);
                updateRefuelComp(availableFeedOptions[0]);
            }
            updateInspectString();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            Log.Debug("Building_RennBase.SpawnSetup() called");
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Log.Debug("Building_RennBase.Destroy() called");
            var rennPondManager = Current.Game?.GetComponent<GameComponent_RennPondManager>();
            rennPondManager.RemoveTrackedThing(this);
            base.Destroy(mode);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Log.Debug("Building_RennBase.DeSpawn() called");
            var rennPondManager = Current.Game?.GetComponent<GameComponent_RennPondManager>();
            rennPondManager.RemoveTrackedThing(this);
            base.DeSpawn(mode);
        }

        private string cachedInspectThreatpoints = "";
        public override string GetInspectString()
        {
            var baseString = base.GetInspectString();
            return baseString + cachedInspectThreatpoints;
        }
        private void updateInspectString()
        {
            var rennPondManager = Current.Game?.GetComponent<GameComponent_RennPondManager>();
            var map = Current.Game?.CurrentMap;
            var settings = rennPondManager.GetCurrentTotalSettings(map);

            var inspectString = "\nCurrent threat points: " + StorytellerUtility.DefaultThreatPointsNow(map) + "\n";
            inspectString += "based on a threat reduction of " + Math.Round((1f - settings.threatMultiplier) * 100) + "% or " + settings.minThreatReduction + " points on this map.";
            this.cachedInspectThreatpoints = inspectString;
        }


        protected override void ReceiveCompSignal(string signal)
        {
            Log.Debug("Building_RennPond.ReceiveCompSignal() called with signal '" + signal + "'");
            if (signal == "Refueled")
            {
                var lastRefueledWith = Refuelable.lastRefueledWith;
                if (lastRefueledWith == DefOfs_RennOrganisms.RennFiber && availableFeedOptions.Contains(FeedOptions.RennFiber))
                {
                    Log.Debug("refueled with fuel '" + lastRefueledWith + "'");
                    lastFed = FeedOptions.RennFiber;
                }
                else if (lastRefueledWith == DefOfs_RennOrganisms.RennFiberDomestic && availableFeedOptions.Contains(FeedOptions.RennFiberDomestic))
                {
                    Log.Debug("refueled with fuel '" + lastRefueledWith + "'");
                    lastFed = FeedOptions.RennFiberDomestic;
                }
                else if (lastRefueledWith == DefOfs_RennOrganisms.RennPowder && availableFeedOptions.Contains(FeedOptions.RennPowder))
                {
                    Log.Debug("refueled with fuel '" + lastRefueledWith + "'");
                    lastFed = FeedOptions.RennPowder;
                }
                else if (lastRefueledWith == DefOfs_RennOrganisms.RennConcentrate && availableFeedOptions.Contains(FeedOptions.RennConcentrate))
                {
                    Log.Debug("refueled with fuel '" + lastRefueledWith + "'");
                    lastFed = FeedOptions.RennConcentrate;
                }
                else if (lastRefueledWith == DefOfs_RennOrganisms.RennCapsule && availableFeedOptions.Contains(FeedOptions.RennCapsule))
                {
                    Log.Debug("refueled with fuel '" + lastRefueledWith + "'");
                    lastFed = FeedOptions.RennCapsule;
                }
                else if (lastRefueledWith == null) // in debug mode, the debug buttons refuel with null
                {
                    Log.Debug("debug tools used to refuel");
                    lastFed = feedOptionSelected;
                }
                else
                {
                    Log.Error("refueled with invalid fuel '" + lastRefueledWith + "'");
                }
                base.ReceiveCompSignal(signal);
            }
        }

        /// <summary>
        /// generates the threat points, overriden in each RennBase to generate different effects
        /// </summary>
        /// <returns></returns>
        protected virtual void updateThreatPoints(ref RennPondSettings settings)
        {
            int moodEffect = 0;
            float threatMultiplier = 1.0f;
            int minThreatReduction = 0;
            if (!def.RequiresPowerForBaseRennEffect || (def.RequiresPowerForBaseRennEffect && Power != null && Power.PowerOn))
            {
                moodEffect = def.MoodOffsetBase;
                threatMultiplier = def.ThreatMultiplierBase;
                minThreatReduction = def.MinThreatReductionBase;
            }
            if (Power == null || (Power != null && Power.PowerOn))
            {
                if (Refuelable != null && Refuelable.HasFuel) {
                    if (lastFed != FeedOptions.notDefined)
                    {
                        var feedSetting = def.Feeds.FirstOrDefault(feed => { return feed.Feed == lastFed; });
                        if (feedSetting != null)
                        {
                            moodEffect += feedSetting.MoodOffset;
                            minThreatReduction += feedSetting.MinThreatReduction;
                            threatMultiplier = -1 + threatMultiplier + feedSetting.ThreatMultiplier;
                        }
                        else
                        {
                            Log.ErrorOnce("feed settings are missing for " + lastFed, 42835412);
                        }
                    }
                }
            }
            settings.threatMultiplier = threatMultiplier;
            settings.threatCap = def.MaxThreatCap;
            settings.minThreatReduction = minThreatReduction;
            moodEffect = (int)Math.Round((double)moodEffect * ModSettings_RennOrganisms.moodMultipilerSettingsPercent / 100, 0);
            settings.moodEffect = moodEffect;
        }

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % GenTicks.TickRareInterval == 0)
            {
                if (mySettings == null)
                {
                    mySettings = new RennPondSettings();
                }
                updateThreatPoints(ref mySettings);
                updateInspectString();
                NotifyGameComponentAboutThreatpoints();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref feedOptionSelected, "feedOptionSelected", FeedOptions.notDefined);
            Scribe_Values.Look(ref lastFed, "lastFed", FeedOptions.notDefined);
            updateRefuelComp(feedOptionSelected);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            return base.GetGizmos().Concat(buildCodeGizmos());
        }

        private IEnumerable<Gizmo> buildCodeGizmos()
        {
            string labelName;
            Texture2D texture;
            switch (feedOptionSelected)
            {
                case FeedOptions.RennFiber:
                    labelName = DefOfs_RennOrganisms.RennFiber.label;
                    texture = Textures_RennOrganisms.RennFiber;
                    break;
                case FeedOptions.RennFiberDomestic:
                    labelName = DefOfs_RennOrganisms.RennFiberDomestic.label;
                    texture = Textures_RennOrganisms.RennFiberDomestic;
                    break;
                case FeedOptions.RennPowder:
                    labelName = DefOfs_RennOrganisms.RennPowder.label;
                    texture = Textures_RennOrganisms.RennPowder;
                    break;
                case FeedOptions.RennConcentrate:
                    labelName = DefOfs_RennOrganisms.RennConcentrate.label;
                    texture = Textures_RennOrganisms.RennConcentrate;
                    break;
                case FeedOptions.RennCapsule:
                    labelName = DefOfs_RennOrganisms.RennCapsule.label;
                    texture = Textures_RennOrganisms.RennCapsule;
                    break;
                default:
                    labelName = "";
                    texture = null;
                    break;
            }
            if (availableFeedOptions.Count > 1)
            {
                var refuelModeCommand = new Command_Action()
                {
                    icon = texture,
                    defaultLabel = "Feed " + labelName,
                    defaultDesc = "Feed the microbes " + labelName,
                    hotKey = KeyBindingDefOf.Misc1,
                    activateSound = SoundDefOf.Click,
                    action = delegate ()
                    {
                        Log.Debug("Building_RennBase.Feed was clicked");
                        Find.WindowStack.Add(MakeModeDropdown(this));
                    },
                };
                yield return refuelModeCommand;
            }
            
            // todo: rest?

            yield break;
        }

        public void updateRefuelComp(FeedOptions option)
        {
            feedOptionSelected = option;
            var refuelableComp = GetComp<CompRefuelable>();
            CompProperties_Refuelable props = refuelableComp.props as CompProperties_Refuelable;
            switch (option)
            {
                case FeedOptions.RennFiber:
                    Log.Debug("Building_RennPond: setting allowed fuels to RennFiber");
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiber, true);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiberDomestic, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennPowder, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennConcentrate, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennCapsule, false);
                    break;
                case FeedOptions.RennFiberDomestic:
                    Log.Debug("Building_RennPond: setting allowed fuels to RennFiberDomestic");
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiber, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiberDomestic, true);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennPowder, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennConcentrate, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennCapsule, false);
                    break;
                case FeedOptions.RennPowder:
                    Log.Debug("Building_RennPond: setting allowed fuels to RennPowder");
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiber, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiberDomestic, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennPowder, true);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennConcentrate, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennCapsule, false);
                    break;
                case FeedOptions.RennConcentrate:
                    Log.Debug("Building_RennPond: setting allowed fuels to RennConcentrate");
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiber, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiberDomestic, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennPowder, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennConcentrate, true);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennCapsule, false);
                    break;
                    break;
                case FeedOptions.RennCapsule:
                    Log.Debug("Building_RennPond: setting allowed fuels to RennCapsule");
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiber, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennFiberDomestic, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennPowder, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennConcentrate, false);
                    props.fuelFilter.SetAllow(DefOfs_RennOrganisms.RennCapsule, true);
                    break;
            }
        }

        private FloatMenu MakeModeDropdown(Building_RennBase sender)
        {
            List<FloatMenuOption> floatMenu = new List<FloatMenuOption>();
            if (availableFeedOptions.Contains(FeedOptions.RennFiber))
            {
                floatMenu.Add(new FloatMenuOption(DefOfs_RennOrganisms.RennFiber.label, delegate ()
                {
                    sender.updateRefuelComp(FeedOptions.RennFiber);
                }, DefOfs_RennOrganisms.RennFiber, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (availableFeedOptions.Contains(FeedOptions.RennFiberDomestic))
            {
                floatMenu.Add(new FloatMenuOption(DefOfs_RennOrganisms.RennFiberDomestic.label, delegate ()
                {
                    sender.updateRefuelComp(FeedOptions.RennFiberDomestic);
                }, DefOfs_RennOrganisms.RennFiberDomestic, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (availableFeedOptions.Contains(FeedOptions.RennPowder))
            {
                floatMenu.Add(new FloatMenuOption(DefOfs_RennOrganisms.RennPowder.label, delegate ()
                {
                    sender.updateRefuelComp(FeedOptions.RennPowder);
                }, DefOfs_RennOrganisms.RennPowder, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (availableFeedOptions.Contains(FeedOptions.RennConcentrate))
            {
                floatMenu.Add(new FloatMenuOption(DefOfs_RennOrganisms.RennConcentrate.label, delegate ()
                {
                    sender.updateRefuelComp(FeedOptions.RennConcentrate);
                }, DefOfs_RennOrganisms.RennConcentrate, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (availableFeedOptions.Contains(FeedOptions.RennCapsule))
            {
                floatMenu.Add(new FloatMenuOption(DefOfs_RennOrganisms.RennCapsule.label, delegate ()
                {
                    sender.updateRefuelComp(FeedOptions.RennCapsule);
                }, DefOfs_RennOrganisms.RennCapsule, MenuOptionPriority.Default, null, null, 0f, null, null));
            }

            return new FloatMenu(floatMenu);
        }

        private void NotifyGameComponentAboutThreatpoints()
        {
            Log.DebugOnce("Building_RennBase.SetThreatPoints() is getting called");
            var rennPondManager = Current.Game?.GetComponent<GameComponent_RennPondManager>();
            rennPondManager.SetThreatPoints(this, mySettings);
        }
    }
}
