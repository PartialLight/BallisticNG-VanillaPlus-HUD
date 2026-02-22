using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using BallisticModding;
using BallisticUnityTools.Placeholders;
using BallisticUnityTools;
using BallisticNG;
using UnityEngine;
using UnityEngine.UI;
using NgUi.RaceUi;
using NgUi.MenuUi;
using NgContent;
using ModOptions = NgUi.Options.ModOptions;
using NgEvents;
using NgData;
using NgGame;
using NgLib;
using NgMusic;
using NgMp;
using NgShips;
using NgModding.Huds;
using NgModding;
using NgPickups;

namespace VanillaPlusHUDOptions
{

    public class ModMenuOptions : CodeMod
    {
        //private string _settingsIni;
        private string _configPath;

        public static bool WeaponMirrorPositionSwap;
        public static bool ForceShieldBars;
        public static bool ForceNameTags;
        public static bool MultiplayerCountdownEndSound;

        //public static bool MusicDisplayStyle;
        //public static bool PitlaneIndicatorStyle;
        public static int MusicDisplayStyle;
        public static int PitlaneIndicatorStyle;

        public static int MissileIconStyle;
        public static int RocketsIconStyle;

        public static bool RearViewMirror2159;
        public static bool RearViewMirror2280;
        public static bool RearViewMirrorFloorhugger;

        //public static bool SpeedometerReadoutStyle;
        public static int SpeedometerReadoutStyle;
        public static int EnergyBarReadoutDecimalPrecision;

        public static bool DamageFlasherToggle;
        public static bool RelativeTimeDisplayToggle;
        public static bool RechargeSumToggle;
        public static bool LastAttackerToggle;

        public static bool HyperThrustBarToggle;
        //public static bool HyperThrustBarPosition;
        public static int HyperThrustBarPosition;
        public static bool HyperThrustBarTextToggle;

        public static int SpeedPadCounterToggle;
        public static bool SpeedPadCounterTextToggle;
        public static int SpeedPadTimerToggle;
        public static bool SpeedPadTimerTextToggle;

        //public static bool SpeedPadElementsPosition;
        public static int SpeedPadElementsPosition;

        public static int CanopyCameraAdjustment2280;
        public static int CockpitCameraAdjustment2280;
        public static int CockpitMeshAdjustment;
        public static int CameraBehavior2280;

        public override void OnRegistered(string ModLocation)
        {
            _configPath = Path.Combine(ModLocation, "config.ini");

            RegisterSettings();

            NgSystemEvents.OnConfigRead += OnConfigRead;
            NgSystemEvents.OnConfigWrite += OnConfigWrite;

            //_settingsIni = Path.Combine(ModLocation, "settings.ini");

            //ModOptions.OnLoadSettings += OnLoadSettings;
            //ModOptions.OnSaveSettings += OnSaveSettings;

            //ModOptions.RegisterMod("Vanilla Plus Options Menu", GenerateModUi, ModUiToCode);
        }

        private void RegisterSettings()
        {
            string ModID = "Vanilla Plus";

            string SelectorCategory0 = "Shared Settings";
            string SelectorCategory1 = "Music Display | Pitlane Indicator";
            string SelectorCategory2 = "Weapon Icons";
            string SelectorCategory3 = "Rear View Mirror";
            string SelectorCategory4 = "Numeric Readouts";
            string SelectorCategory5 = "Race Awareness Elements";
            string SelectorCategory6 = "Hyperthrust Bar";
            string SelectorCategory7 = "Speed Pad Elements";
            string SelectorCategory8 = "Experimental Camera Adjustments";

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "WeaponMirrorPositionSwap_ID",
                selector =>
                {
                    selector.Configure("Weapon-Mirror Position Swap", "Whether to swap the positions of the rear view mirror and weapon pickup display. If enabled, this option will lower the position of the pickup display even if the rear view mirror is disabled.",
                        WeaponMirrorPositionSwap, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    WeaponMirrorPositionSwap = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "ForceShieldBars_ID",
                selector =>
                {
                    selector.Configure("Force Shield Bars", "Whether to force shield bars.",
                        ForceShieldBars, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ForceShieldBars = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "ForceNameTags_ID",
                selector =>
                {
                    selector.Configure("Force Name Tags", "Whether to force name tags.",
                        ForceNameTags, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ForceNameTags = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "MultiplayerCountdownEndSound_ID",
                selector =>
                {
                    selector.Configure("Multiplayer Countdown End Sound", "Whether to enable playback of a custom fourth sound at the end of the multiplayer lobby countdown sequence.",
                        MultiplayerCountdownEndSound, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    MultiplayerCountdownEndSound = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory1, "MusicDisplayStyle_ID",
                selector =>
                {
                    selector.Configure("Music Display Style", "Whether to use modded or internal music display.",
                        MusicDisplayStyle, null, "Modded", "Internal");
                },
                selector =>
                {
                    MusicDisplayStyle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory1, "PitlaneIndicatorStyle_ID",
                selector =>
                {
                    selector.Configure("Pitlane Indicator Style", "Whether to use modded or internal pitlane indicator.",
                        PitlaneIndicatorStyle, null, "Modded", "Internal");
                },
                selector =>
                {
                    PitlaneIndicatorStyle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory2, "MissileIconStyle_ID",
                selector =>
                {
                    selector.Configure("Missile Icon Style", "Internal\n    The internal missile icon.\n\nInscribed Triangle\n    Replaces the internal missile icon with a solid triangle\n    bounded by a larger triangle.",
                        MissileIconStyle, null, "Internal", "Inscribed Triangle");
                },
                selector =>
                {
                    MissileIconStyle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory2, "RocketsIconStyle_ID",
                selector =>
                {
                    selector.Configure("Rockets Icon Style", "Internal\n    The internal rockets icon.\n\nTrefoil\n    Replaces the internal rockets icon with 3 inward-pointed\n    triangles offset by 120 degrees.\n\nUpward Trefoil\n    Same as Trefoil but top triangle points up.\n\nHorizontal\n    Three triangles in a row.\n\nVertical\n    Three triangles in a column.\n\nTetrahedron Net\v    Triforce.",
                        RocketsIconStyle, null, "Internal", "Trefoil", "Upward Trefoil", "Horizontal", "Vertical", "Tetrahedron Net");
                },
                selector =>
                {
                    RocketsIconStyle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory3, "RearViewMirror2159_ID",
                selector =>
                {
                    selector.Configure("2159 Rear View Mirror", "Whether to enable the rear view mirror in 2159.",
                        RearViewMirror2159, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RearViewMirror2159 = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory3, "RearViewMirror2280_ID",
                selector =>
                {
                    selector.Configure("2280 Rear View Mirror", "Whether to enable the rear view mirror in 2280.",
                        RearViewMirror2280, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RearViewMirror2280 = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory3, "RearViewMirrorFloorhugger_ID",
                selector =>
                {
                    selector.Configure("Floorhugger Rear View Mirror", "Whether to enable the rear view mirror in Floorhugger.",
                        RearViewMirrorFloorhugger, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RearViewMirrorFloorhugger = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory4, "SpeedometerReadoutStyle_ID",
                selector =>
                {
                    selector.Configure("Speedometer Readout Style", "Whether to display ship speed in Engine Force, KPH/MPH, or Unity world units per second.",
                        SpeedometerReadoutStyle, null, "Engine Force", "Metric/Imperial", "Unity World Units");
                },
                selector =>
                {
                    SpeedometerReadoutStyle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory4, "EnergyBarReadoutDecimalPrecision_ID",
                selector =>
                {
                    selector.Configure("Energy Bar Readout Decimal Precision", "How many digits after the decimal should be displayed for the ship energy readout.",
                        EnergyBarReadoutDecimalPrecision, null, "Five", "Four", "Three", "Two", "One", "Zero");
                },
                selector =>
                {
                    EnergyBarReadoutDecimalPrecision = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory4, "RechargeSumToggle_ID",
                selector =>
                {
                    selector.Configure("Energy Recharge Sum", "Whether to display the amount of shield energy recharged based on time spent in the pitlane.",
                        RechargeSumToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RechargeSumToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "DamageFlasherToggle_ID",
                selector =>
                {
                    selector.Configure("Damage Flasher", "Whether to enable the damage flasher.",
                        DamageFlasherToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    DamageFlasherToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "RelativeTimeDisplayToggle_ID",
                selector =>
                {
                    selector.Configure("Relative Time Display", "Whether to enable the relative time display.",
                        RelativeTimeDisplayToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RelativeTimeDisplayToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "LastAttackerToggle_ID",
                selector =>
                {
                    selector.Configure("Last Attacker Display", "Whether to enable the last attacker display.",
                        LastAttackerToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    LastAttackerToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory6, "HyperThrustBarToggle_ID",
                selector =>
                {
                    selector.Configure("Hyperthrust Bar", "Whether to enable the hyperthrust (afterburner) bar.",
                        HyperThrustBarToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    HyperThrustBarToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory6, "HyperThrustBarTextToggle_ID",
                selector =>
                {
                    selector.Configure("Hyperthrust Bar Text", "Whether to display the hyperthrust bar's text readout.",
                        HyperThrustBarTextToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    HyperThrustBarTextToggle = selector.ToBool();
                });


            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory6, "HyperThrustBarPosition_ID",
                selector =>
                {
                    selector.Configure("Hyperthrust Bar Position", "Whether to display the hyperthrust (afterburner) bar next to the rear view mirror, towards the bottom of the screen, or in the middle of the screen. Options labelled '-Wide' preserve default horizontal spacing.",
                        HyperThrustBarPosition, null, "Default", "Lowered", "Centered", "Lowered-Wide", "Centered-Wide");
                },
                selector =>
                {
                    HyperThrustBarPosition = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadCounterToggle_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Counter", "When to display the speed pad counter. Exclude 2280 recommended.",
                        SpeedPadCounterToggle, null, "Exclude 2280", "Never", "Always");
                },
                selector =>
                {
                    SpeedPadCounterToggle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadCounterTextToggle_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Counter Text", "Whether to display the speed pad counter's text readout.",
                        SpeedPadCounterTextToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    SpeedPadCounterTextToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadTimerToggle_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Timer", "When to display the speed pad timer. Exclude 2280 recommended.",
                        SpeedPadTimerToggle, null, "Exclude 2280", "Never", "Always");
                },
                selector =>
                {
                    SpeedPadTimerToggle = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadTimerTextToggle_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Timer Text", "Whether to display the speed pad timer's text readout.",
                        SpeedPadTimerTextToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    SpeedPadTimerTextToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadElementsPosition_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Elements Position", "Whether to display the speed pad counter and timer next to the rear view mirror, towards the bottom of the screen, or in the middle of the screen. Options labelled '-Wide' preserve default horizontal spacing.",
                        SpeedPadElementsPosition, null, "Default", "Lowered", "Centered", "Lowered-Wide", "Centered-Wide");
                },
                selector =>
                {
                    SpeedPadElementsPosition = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CanopyCameraAdjustment2280_ID",
                selector =>
                {
                    selector.Configure("2280 Canopy Camera Adjustment", "Whether to use the internal canopy camera height or raise it to be more similar to 2159.",
                        CanopyCameraAdjustment2280, null, "Raised", "Internal");
                },
                selector =>
                {
                    CanopyCameraAdjustment2280 = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitCameraAdjustment2280_ID",
                selector =>
                {
                    selector.Configure("2280 Cockpit Camera Adjustment", "Whether to use the internal cockpit camera height or raise it to be more similar to 2159.",
                        CockpitCameraAdjustment2280, null, "Raised", "Internal");
                },
                selector =>
                {
                    CockpitCameraAdjustment2280 = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitMeshAdjustment_ID",
                selector =>
                {
                    selector.Configure("Cockpit Mesh Adjustment", "Whether to display the cockpit interior or the nose/forward hull of the ship when using the cockpit camera.",
                        CockpitMeshAdjustment, null, "Nosecam", "Interior");
                },
                selector =>
                {
                    CockpitMeshAdjustment = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CameraBehavior2280_ID",
                selector =>
                {
                    selector.Configure("2280 Camera Behavior", "Internal\n    2280 cameras will behave as normal.\n\n2280 Tilt Lock\n    2280 cameras will have their tilt locked to Z=0 degrees, \n    similar to 2159.\n\nPseudohugger\n    2280 cameras will align to the tilt of the track surface.\n    WARNING: On a small number of tracks, Pseudohugger\n    behavior will be jarring at breaks in the track surface.",
                        CameraBehavior2280, null, "Internal", "Tilt Lock", "Pseudohugger");
                },
                selector =>
                {
                    CameraBehavior2280 = selector.Value;
                });
        }

        //private void GenerateModUi(ModOptionsUiContext ctx)
        //{
        //    ctx.GenerateHeader("Music Display | Pitlane Indicator");
        //    ctx.GenerateSelector("MusicDisplayStyle_ID", "Music Display Style", "Whether to use internal or modded music display.", MusicDisplayStyle ? 1 : 0, "Internal", "Modded");
        //    ctx.GenerateSelector("PitlaneIndicatorStyle_ID", "Pitlane Indicator Style", "Whether to use internal or modded pitlane indicator.", PitlaneIndicatorStyle ? 1 : 0, "Internal", "Modded");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Weapon Icons");
        //    ctx.GenerateSelector("MissileIconStyle_ID", "Missile Icon Style", "Internal\vThe internal missile icon.\nInscribed Triangle\vReplaces the internal missile icon with a solid triangle bounded by a larger triangle.", MissileIconStyle, "Internal", "Inscribed Triangle");
        //    ctx.GenerateSelector("RocketsIconStyle_ID", "Rockets Icon Style", "Internal\vThe internal rockets icon.\nTrefoil\vReplaces the internal rockets icon with 3 inward-pointed triangles offset by 120 degrees.\nUpward Trefoil\vSame as Trefoil but top triangle points up.\nHorizontal\vThree triangles in a row.\nVertical\vThree triangles in a column.\nTetrahedron Net\vTriforce.", RocketsIconStyle, "Internal", "Trefoil", "Upward Trefoil", "Horizontal", "Vertical", "Tetrahedron Net");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Rear View Mirror");
        //    ctx.GenerateSelector("RearViewMirror2159_ID", "2159 Rear View Mirror", "Whether to enable the rear view mirror in 2159.", RearViewMirror2159 ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSelector("RearViewMirror2280_ID", "2280 Rear View Mirror", "Whether to enable the rear view mirror in 2280.", RearViewMirror2280 ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSelector("RearViewMirrorFloorhugger_ID", "Floorhugger Rear View Mirror", "Whether to enable the rear view mirror in Floorhugger.", RearViewMirrorFloorhugger ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Numeric Readouts");
        //    ctx.GenerateSelector("SpeedometerReadoutStyle_ID", "Speedometer Readout Style", "Whether to display ship speed in KPH/MPH or Engine Force.", SpeedometerReadoutStyle ? 1 : 0, "Metric/Imperial", "Engine Force");
        //    ctx.GenerateSelector("EnergyBarReadoutDecimalPrecision_ID", "Energy Bar Readout Decimal Precision", "How many digits after the decimal should be displayed for the ship energy readout.", EnergyBarReadoutDecimalPrecision, "Five", "Four", "Three", "Two", "One", "Zero");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Race Awareness");
        //    ctx.GenerateSelector("DamageFlasherToggle_ID", "Damage Flasher", "Whether to enable the damage flasher.", DamageFlasherToggle ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSelector("RelativeTimeDisplayToggle_ID", "Relative Time Display", "Whether to enable the relative time display.", RelativeTimeDisplayToggle ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Hyperthrust Bar");
        //    ctx.GenerateSelector("HyperThrustBarToggle_ID", "Hyperthrust Bar", "Whether to enable the hyperthrust (afterburner) bar", HyperThrustBarToggle ? 1 : 0, "Enabled", "Disabled");
        //    ctx.GenerateSelector("HyperThrustBarPosition_ID", "Hyperthrust Bar Position", "Whether to display the hyperthrust bar next to the rear view mirror or towards the bottom of the screen.", HyperThrustBarPosition ? 1 : 0, "Lowered", "Default");
        //    ctx.GenerateSpace();

        //    ctx.GenerateHeader("Speed Pad Elements");
        //    ctx.GenerateSelector("SpeedPadCounterToggle_ID", "Speed Pad Counter", "When to display the speed pad counter. Exclude 2280 recommended.", SpeedPadCounterToggle, "Exclude 2280", "Never", "Always");
        //    ctx.GenerateSelector("SpeedPadTimerToggle_ID", "Speed Pad Timer", "When to display the speed pad timer. Exclude 2280 recommended.", SpeedPadTimerToggle, "Exclude 2280", "Never", "Always");
        //    ctx.GenerateSelector("SpeedPadElementsPosition_ID", "Speed Pad Elements Position", "Whether to display the speed pad counter and timer next to the rear view mirror or towards the bottom of the screen.", SpeedPadElementsPosition ? 1 : 0, "Lowered", "Default");
        //    ctx.GenerateSpace();
        //}

        //private void ModUiToCode(ModOptionsUiContext ctx)
        //{
        //    MusicDisplayStyle = ctx.GetSelectorValue("MusicDisplayStyle_ID") == 1;
        //    PitlaneIndicatorStyle = ctx.GetSelectorValue("PitlaneIndicatorStyle_ID") == 1;

        //    MissileIconStyle = ctx.GetSelectorValue("MissileIconStyle_ID");
        //    RocketsIconStyle = ctx.GetSelectorValue("RocketsIconStyle_ID");

        //    RearViewMirror2159 = ctx.GetSelectorValue("RearViewMirror2159_ID") == 1;
        //    RearViewMirror2280 = ctx.GetSelectorValue("RearViewMirror2280_ID") == 1;
        //    RearViewMirrorFloorhugger = ctx.GetSelectorValue("RearViewMirrorFloorhugger_ID") == 1;

        //    SpeedometerReadoutStyle = ctx.GetSelectorValue("SpeedometerReadoutStyle_ID") == 1;
        //    EnergyBarReadoutDecimalPrecision = ctx.GetSelectorValue("EnergyBarReadoutDecimalPrecision_ID");

        //    DamageFlasherToggle = ctx.GetSelectorValue("DamageFlasherToggle_ID") == 1;
        //    RelativeTimeDisplayToggle = ctx.GetSelectorValue("RelativeTimeDisplayToggle_ID") == 1;

        //    HyperThrustBarToggle = ctx.GetSelectorValue("HyperThrustBarToggle_ID") == 1;
        //    HyperThrustBarPosition = ctx.GetSelectorValue("HyperThrustBarPosition_ID") == 1;

        //    SpeedPadCounterToggle = ctx.GetSelectorValue("SpeedPadCounterToggle_ID");
        //    SpeedPadTimerToggle = ctx.GetSelectorValue("SpeedPadTimerToggle_ID");
        //    SpeedPadElementsPosition = ctx.GetSelectorValue("SpeedPadElementsPosition_ID") == 1;
        //}

        //private void OnLoadSettings()
        private void OnConfigRead()
        {
            INIParser ini = new INIParser();
            //ini.Open(_settingsIni);
            ini.Open(_configPath);

            WeaponMirrorPositionSwap = ini.ReadValue("Settings", "WeaponMirrorPositionSwap_ID", WeaponMirrorPositionSwap);
            ForceShieldBars = ini.ReadValue("Settings", "ForceShieldBars_ID", ForceShieldBars);
            ForceNameTags = ini.ReadValue("Settings", "ForceNameTags_ID", ForceNameTags);
            MultiplayerCountdownEndSound = ini.ReadValue("Settings", "MultiplayerCountdownEndSound_ID", MultiplayerCountdownEndSound);

            MusicDisplayStyle = ini.ReadValue("Settings", "MusicDisplayStyle_ID", MusicDisplayStyle);
            PitlaneIndicatorStyle = ini.ReadValue("Settings", "PitlaneIndicatorStyle_ID", PitlaneIndicatorStyle);

            MissileIconStyle = ini.ReadValue("Settings", "MissileIconStyle_ID", MissileIconStyle);
            RocketsIconStyle = ini.ReadValue("Settings", "RocketsIconStyle_ID", RocketsIconStyle);

            RearViewMirror2159 = ini.ReadValue("Settings", "RearViewMirror2159_ID", RearViewMirror2159);
            RearViewMirror2280 = ini.ReadValue("Settings", "RearViewMirror2280", RearViewMirror2280);
            RearViewMirrorFloorhugger = ini.ReadValue("Settings", "RearViewMirrorFloorhugger_ID", RearViewMirrorFloorhugger);

            SpeedometerReadoutStyle = ini.ReadValue("Settings", "SpeedometerReadoutStyle_ID", SpeedometerReadoutStyle);
            EnergyBarReadoutDecimalPrecision = ini.ReadValue("Settings", "EnergyBarReadoutDecimalPrecision", EnergyBarReadoutDecimalPrecision);

            DamageFlasherToggle = ini.ReadValue("Settings", "DamageFlasherToggle_ID", DamageFlasherToggle);
            RelativeTimeDisplayToggle = ini.ReadValue("Settings", "RelativeTimeDisplayToggle_ID", RelativeTimeDisplayToggle);
            RechargeSumToggle = ini.ReadValue("Settings", "RechargeSumToggle_ID", RechargeSumToggle);
            LastAttackerToggle = ini.ReadValue("Settings", "LastAttackerToggle_ID", LastAttackerToggle);

            HyperThrustBarToggle = ini.ReadValue("Settings", "HyperThrustBarToggle_ID", HyperThrustBarToggle);
            HyperThrustBarTextToggle = ini.ReadValue("Settings", "HyperThrustBarTextToggle_ID", HyperThrustBarTextToggle);
            HyperThrustBarPosition = ini.ReadValue("Settings", "HyperThrustBarPosition_ID", HyperThrustBarPosition);

            SpeedPadCounterToggle = ini.ReadValue("Settings", "SpeedPadCounterToggle_ID", SpeedPadCounterToggle);
            SpeedPadCounterTextToggle = ini.ReadValue("Settings", "SpeedPadCounterTextToggle_ID", SpeedPadCounterTextToggle);
            SpeedPadTimerToggle = ini.ReadValue("Settings", "SpeedPadTimerToggle_ID", SpeedPadTimerToggle);
            SpeedPadTimerTextToggle = ini.ReadValue("Settings", "SpeedPadTimerTextToggle_ID", SpeedPadTimerTextToggle);

            SpeedPadElementsPosition = ini.ReadValue("Settings", "SpeedPadElementsPosition_ID", SpeedPadElementsPosition);

            CanopyCameraAdjustment2280 = ini.ReadValue("Settings", "CanopyCameraAdjustment2280_ID", CanopyCameraAdjustment2280);
            CockpitCameraAdjustment2280 = ini.ReadValue("Settings", "CockpitCameraAdjustment2280_ID", CockpitCameraAdjustment2280);
            CockpitMeshAdjustment = ini.ReadValue("Settings", "CockpitMeshAdjustment_ID", CockpitMeshAdjustment);
            CameraBehavior2280 = ini.ReadValue("Settings", "CameraBehavior2280_ID", CameraBehavior2280);

            ini.Close();
        }

        //private void OnSaveSettings()
        private void OnConfigWrite()
        {
            INIParser ini = new INIParser();
            //ini.Open(_settingsIni);
            ini.Open(_configPath);

            ini.WriteValue("Settings", "WeaponMirrorPositionSwap_ID", WeaponMirrorPositionSwap);
            ini.WriteValue("Settings", "ForceShieldBars_ID", ForceShieldBars);
            ini.WriteValue("Settings", "ForceNameTags_ID", ForceNameTags);
            ini.WriteValue("Settings", "MultiplayerCountdownEndSound_ID", MultiplayerCountdownEndSound);

            ini.WriteValue("Settings", "MusicDisplayStyle_ID", MusicDisplayStyle);
            ini.WriteValue("Settings", "PitlaneIndicatorStyle_ID", PitlaneIndicatorStyle);

            ini.WriteValue("Settings", "MissileIconStyle_ID", MissileIconStyle);
            ini.WriteValue("Settings", "RocketsIconStyle_ID", RocketsIconStyle);

            ini.WriteValue("Settings", "RearViewMirror2159_ID", RearViewMirror2159);
            ini.WriteValue("Settings", "RearViewMirror2280", RearViewMirror2280);
            ini.WriteValue("Settings", "RearViewMirrorFloorhugger_ID", RearViewMirrorFloorhugger);

            ini.WriteValue("Settings", "SpeedometerReadoutStyle_ID", SpeedometerReadoutStyle);
            ini.WriteValue("Settings", "EnergyBarReadoutDecimalPrecision", EnergyBarReadoutDecimalPrecision);

            ini.WriteValue("Settings", "DamageFlasherToggle_ID", DamageFlasherToggle);
            ini.WriteValue("Settings", "RelativeTimeDisplayToggle_ID", RelativeTimeDisplayToggle);
            ini.WriteValue("Settings", "RechargeSumToggle_ID", RechargeSumToggle);
            ini.WriteValue("Settings", "LastAttackerToggle_ID", LastAttackerToggle);

            ini.WriteValue("Settings", "HyperThrustBarToggle_ID", HyperThrustBarToggle);
            ini.WriteValue("Settings", "HyperThrustBarTextToggle_ID", HyperThrustBarTextToggle);
            ini.WriteValue("Settings", "HyperThrustBarPosition_ID", HyperThrustBarPosition);

            ini.WriteValue("Settings", "SpeedPadCounterToggle_ID", SpeedPadCounterToggle);
            ini.WriteValue("Settings", "SpeedPadCounterTextToggle_ID", SpeedPadCounterTextToggle);
            ini.WriteValue("Settings", "SpeedPadTimerToggle_ID", SpeedPadTimerToggle);
            ini.WriteValue("Settings", "SpeedPadTimerTextToggle_ID", SpeedPadTimerTextToggle);
            ini.WriteValue("Settings", "SpeedPadElementsPosition_ID", SpeedPadElementsPosition);

            ini.WriteValue("Settings", "CanopyCameraAdjustment2280_ID", CanopyCameraAdjustment2280);
            ini.WriteValue("Settings", "CockpitCameraAdjustment2280_ID", CockpitCameraAdjustment2280);
            ini.WriteValue("Settings", "CockpitMeshAdjustment_ID", CockpitMeshAdjustment);
            ini.WriteValue("Settings", "CameraBehavior2280_ID", CameraBehavior2280);

            ini.Close();
        }
    }
}