using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        public static string SelectorCategory0 = "Shared Settings";
        public static string SelectorCategory1 = "Music Display | Pitlane Indicator";
        public static string SelectorCategory2 = "Weapon Icons";
        public static string SelectorCategory3 = "Rear View Mirror";
        public static string SelectorCategory4 = "Numeric Readouts";
        public static string SelectorCategory5 = "Race Awareness Elements";
        public static string SelectorCategory6 = "Hyperthrust Bar";
        public static string SelectorCategory7 = "Speed Pad Elements";
        public static string SelectorCategory8 = "Experimental Camera Adjustments";
        public static string SelectorCategory9 = "Extra Warnings";
        /*public static string SelectorCategoryTEN = "Overtake Radar";*/
        public static string SelectorCategory10 = "Zone Settings";
        public static string SelectorCategory11 = "Upsurge Settings";
        public static string SelectorCategory12 = "Custom Keybinds";

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
        public static int PitlaneIndicatorPosition;

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
        public static int RechargeSumPosition;
        public static bool LastAttackerToggle;

        public static bool HyperThrustBarToggle;
        //public static bool HyperThrustBarPosition;
        public static int HyperThrustBarPosition;
        public static bool HyperThrustBarTextToggle;
        public static int HyperThrustBarVisibility;

        public static int SpeedPadCounterToggle;
        public static bool SpeedPadCounterTextToggle;
        public static int SpeedPadCounterVisibility;
        public static int SpeedPadTimerToggle;
        public static bool SpeedPadTimerTextToggle;
        public static int SpeedPadTimerVisibility;

        //public static bool SpeedPadElementsPosition;
        public static int SpeedPadElementsPosition;

        public static bool AdjustmentAlignmentFixToggle;
        public static int CanopyCameraAdjustment2280;
        public static int CockpitCameraAdjustment2280;
        public static int CockpitCameraAdjustment2159;
        public static int CockpitMeshAdjustment;
        public static int CanopyMeshAdjustment;
        //public static int CockpitShieldAdjustment;
        //public static int CanopyShieldAdjustment;
        //public static int MeshRotationLockToggle;
        public static int CameraBehavior2280;
        //public static bool ForcePseudohugger2159;
        public static bool ForceNoTiltLock2159;

        public static bool FinalLapWarningToggle;
        public static bool TremorWarningToggle;
        public static bool HunterWarningToggle;
        public static bool ShieldTimerToggle;

        public static bool OvertakeRadarToggle;
        public static int OvertakeRadarVisibility;

        //public static int ControllerInputType;

        public static bool UseZoneColorsToggle;
        public static bool PerfectZoneWarningToggle;

        //public static bool LoweredBarrierWarningToggle;
        public static int BarrierWarningPosition;
        public static bool UseUpsurgeColorsToggle;
        public static bool ZonesFullWarningToggle;
        public static bool TargetAttainableWarningToggle;

        public static bool ExtraWeaponInfoToggle;
        public static bool RespawnDarkenerToggle;
        public static int AbsoluteShieldValueStyle;

        public static int CannonFirerateOverride;

        private static readonly HashSet<KeyCode> DeprecatedUnderPhysicalKeys = new HashSet<KeyCode>
        {
        KeyCode.Exclaim, KeyCode.DoubleQuote, KeyCode.Hash, KeyCode.Dollar,
        KeyCode.Percent, KeyCode.Ampersand, KeyCode.LeftParen, KeyCode.RightParen,
        KeyCode.Asterisk, KeyCode.Plus, KeyCode.Colon, KeyCode.Less,
        KeyCode.Greater, KeyCode.Question, KeyCode.At, KeyCode.Caret,
        KeyCode.Underscore, KeyCode.LeftCurlyBracket, KeyCode.Pipe,
        KeyCode.RightCurlyBracket, KeyCode.Tilde, KeyCode.LeftWindows,
        KeyCode.RightWindows, KeyCode.AltGr, KeyCode.Help, KeyCode.SysReq,
        KeyCode.Break
        };

        //private static readonly KeyCode[] AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        private static readonly KeyCode[] AllKeyCodes = BuildFilteredKeyCodes();
        private static readonly string[] AllKeyCodeNames = Array.ConvertAll(AllKeyCodes, kc => kc.ToString());

        private static KeyCode[] BuildFilteredKeyCodes()
        {
            var result = new List<KeyCode>();
            var seenValues = new HashSet<int>();

            foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
            {
                if (DeprecatedUnderPhysicalKeys.Contains(value))
                    continue;

                if (!seenValues.Add((int)value))
                    continue;

                result.Add(value);
            }

            return result.ToArray();
        }

        public static KeyCode BarrelRollKeyCode;
        public static KeyCode SideshiftLeftKeyCode;
        public static KeyCode SideshiftRightKeyCode;
        public static KeyCode SmartshiftKeyCode;
        public static KeyCode PreviousSongKeyCode;
        public static KeyCode NameTag_And_ShieldBars_Visibility_Toggle_KeyCode;
        public static bool VisibilityToggleAffectsRechargeSum;
        public static KeyCode SelfDestructKeyCode;

        public static float SelfDestructTimer;

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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "RespawnDarkenerToggle_ID",
                selector =>
                {
                    selector.Configure("Respawn Darkener", "Whether to enable the respawn darkener. When enabled, the screen will be shaded on respawn before fading back to normal brightness/color.",
                        RespawnDarkenerToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    RespawnDarkenerToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "CannonFirerateOverride_ID",
                selector =>
                {
                    selector.Configure("Cannon Firerate Override", "This setting will attempt to override your ship's cannon firerate. Faster firerates will lower your weapon effectiveness, slower firerates will increase your weapon effectiveness. This has no effect on weaponless ships when \"Force Weapons\" isn't enabled.\n\nDefault\n    Your ship's cannon firerate and weapon effectiveness\n    stats will be unchanged from their default values\n\nLight\n    Fastest firerate, lowest damage\n\nMedium\n    Balanced firerate and damage\n\nHeavy\n    Slowest firerate, highest damage",
                        CannonFirerateOverride, null, "Light", "Medium", "Heavy", "Default");
                },
                selector =>
                {
                    CannonFirerateOverride = selector.Value;
                });

            //ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory0, "ControllerInputType_ID",
            //    selector =>
            //    {
            //        selector.Configure("Controller Input Type", "Please select your controller's input API.\n\nXInput\n    Xbox controllers and most modern generic controllers.\n\nDirectInput\n    PlayStation controllers, Nintendo controllers, and some\n    older generic controllers.",
            //            ControllerInputType, null, "XInput", "DirectInput");
            //    },
            //    selector =>
            //    {
            //        ControllerInputType = selector.Value;
            //    });

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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory1, "PitlaneIndicatorPosition_ID",
                selector =>
                {
                    selector.Configure("Pitlane Indicator Position", "Whether to display the pitlane indicator at the middle (Default), top, lower middle, or bottom of the screen.",
                        PitlaneIndicatorPosition, null, "Default", "Top", "Lower Middle", "Bottom");
                },
                selector =>
                {
                    PitlaneIndicatorPosition = selector.Value;
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory4, "AbsoluteShieldValueStyle_ID",
                selector =>
                {
                    selector.Configure("Absolute Shield Value Style", "Whether to use the internal or modded calculation for absolute shield values. Internal will produce the same numbers as vanilla, modded provides numbers more representative of a ship's true survivability.",
                        AbsoluteShieldValueStyle, null, "Internal", "Modded");
                },
                selector =>
                {
                    AbsoluteShieldValueStyle = selector.Value;
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory4, "RechargeSumPosition_ID",
                selector =>
                {
                    selector.Configure("Recharge Sum Position", "Whether to display the recharge sum at the middle (Default), lower middle, or bottom of the screen.",
                        RechargeSumPosition, null, "Default", "Lower Middle", "Bottom");
                },
                selector =>
                {
                    RechargeSumPosition = selector.Value;
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "OvertakeRadarToggle_ID",
                selector =>
                {
                    selector.Configure("Overtake Radar", "Whether to enable the Overtake Radar. Functions as a proximity warning (similar to Wipeout HD) that shows you how far ahead you are of the ship behind you. If in last place, instead shows how close you are to the first ship ahead of you.",
                        OvertakeRadarToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    OvertakeRadarToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "OvertakeRadarVisibility_ID",
                selector =>
                {
                    selector.Configure("Overtake Radar Visibility", "Whether the Overtake Radar should always be visible or be hidden automatically when out of range.",
                        OvertakeRadarVisibility, null, "Always Visible", "Auto-Hide");
                },
                selector =>
                {
                    OvertakeRadarVisibility = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory5, "ExtraWeaponInfoToggle_ID",
                selector =>
                {
                    selector.Configure("Extra Weapon Information", "Whether to display additional weapon information. When enabled, additional info will be displayed for the following weapons:\n\nMissile\n    Lockon signal integrity (how close you are to losing a\n    lockon) + lethal damage indicator (locked target is\n    unshielded, will die to the missile impact, and you have\n    full lock)\n\nHellstorm\n    The total number of unique targets you've locked on to\n\nAutopilot\n    The amount of time remaining before autopilot attempts\n    to disengage, followed by the amount of time before\n    autopilot forcibly disengages\n\nEnergy Wall\n    Which side of the track your energy wall will deploy on",
                        ExtraWeaponInfoToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ExtraWeaponInfoToggle = selector.ToBool();
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory6, "HyperThrustBarVisibility_ID",
                selector =>
                {
                    selector.Configure("Hyper Thrust Bar Visibility", "Whether the Hyper Thrust Bar should always be visible or be hidden automatically when empty.",
                        HyperThrustBarVisibility, null, "Always Visible", "Auto-Hide");
                },
                selector =>
                {
                    HyperThrustBarVisibility = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory6, "HyperThrustBarPosition_ID",
                selector =>
                {
                    selector.Configure("Hyperthrust Bar Position", "Whether to display the hyperthrust (afterburner) bar next to the rear view mirror (Default), towards the bottom of the screen, or in the middle of the screen. Options labelled '-Wide' preserve default horizontal spacing.",
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadCounterVisibility_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Counter Visibility", "Whether the Speed Pad Counter should always be visible or be hidden automatically when empty.",
                        SpeedPadCounterVisibility, null, "Always Visible", "Auto-Hide");
                },
                selector =>
                {
                    SpeedPadCounterVisibility = selector.Value;
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

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadTimerVisibility_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Timer Visibility", "Whether the Speed Pad Timer should always be visible or be hidden automatically when empty.",
                        SpeedPadTimerVisibility, null, "Always Visible", "Auto-Hide");
                },
                selector =>
                {
                    SpeedPadTimerVisibility = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory7, "SpeedPadElementsPosition_ID",
                selector =>
                {
                    selector.Configure("Speed Pad Elements Position", "Whether to display the speed pad counter and timer next to the rear view mirror (Default), towards the bottom of the screen, or in the middle of the screen. Options labelled '-Wide' preserve default horizontal spacing.",
                        SpeedPadElementsPosition, null, "Default", "Lowered", "Centered", "Lowered-Wide", "Centered-Wide");
                },
                selector =>
                {
                    SpeedPadElementsPosition = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "AdjustmentAlignmentFixToggle_ID",
                selector =>
                {
                    selector.Configure("Adjustment Alignment Fix", "Enabling this setting will fix some alignment issues with camera and mesh adjustments (i.e. 2159 cockpit camera being lower than it should be) but create misalignments elsewhere. If you don't intend to use nosecam, bonnetcam, or any of the raised camera positions, and you want the most vanilla-accurate behavior, enable this setting. Otherwise, this can safely be left disabled.",
                        AdjustmentAlignmentFixToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    AdjustmentAlignmentFixToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CanopyCameraAdjustment2280_ID",
                selector =>
                {
                    selector.Configure("2280 Canopy Camera Adjustment", "Whether to use the internal canopy camera height for 2280 or raise it to the same height as the canopy camera height in 2159.",
                        CanopyCameraAdjustment2280, null, "Raised", "Internal");
                },
                selector =>
                {
                    CanopyCameraAdjustment2280 = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitCameraAdjustment2280_ID",
                selector =>
                {
                    selector.Configure("2280 Cockpit Camera Adjustment", "Whether to use the internal cockpit camera height for 2280 or raise it to the same height as the canopy camera height in 2159.",
                        CockpitCameraAdjustment2280, null, "Raised", "Internal");
                },
                selector =>
                {
                    CockpitCameraAdjustment2280 = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitCameraAdjustment2159_ID",
                selector =>
                {
                    selector.Configure("2159 Cockpit Camera Adjustment", "Whether to use the internal cockpit camera height or raise it to the same height as the canopy camera height. For best results with Nosecam, 'Raised' is suggested.",
                        CockpitCameraAdjustment2159, null, "Raised", "Internal");
                },
                selector =>
                {
                    CockpitCameraAdjustment2159 = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CanopyMeshAdjustment_ID",
                selector =>
                {
                    selector.Configure("Canopy Mesh Adjustment", "Whether to display the nose/forward hull ('Bonnet') of the ship when using the canopy camera or keep it hidden as normal.\n\n    WARNING: Some ships, such as 2159 Nexus, will not\n    display properly in Bonnetcam.",
                        CanopyMeshAdjustment, null, "Bonnetcam", "Hidden");
                },
                selector =>
                {
                    CanopyMeshAdjustment = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitMeshAdjustment_ID",
                selector =>
                {
                    selector.Configure("Cockpit Mesh Adjustment", "Whether to display the cockpit interior or the nose/forward hull of the ship when using the cockpit camera.\n\n    WARNING: Ships with a virtual cockpit ('glassless\n    canopy') will not be visible in Nosecam. This\n    includes but is not limited to Tenrai and all of the\n    Barracudas.",
                        CockpitMeshAdjustment, null, "Nosecam", "Interior");
                },
                selector =>
                {
                    CockpitMeshAdjustment = selector.Value;
                });            

            //ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CockpitShieldAdjustment_ID",
            //    selector =>
            //    {
            //        selector.Configure("Cockpit Shield Adjustment", "Whether to position the shield mesh at the top of the screen ('Umbrella', default for cockpit camera mode) or at the bottom of the screen when using the cockpit camera mode.",
            //            CockpitShieldAdjustment, null, "Umbrella", "Bonnet");
            //    },
            //    selector =>
            //    {
            //        CockpitShieldAdjustment = selector.Value;
            //    });

            //ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CanopyShieldAdjustment_ID",
            //    selector =>
            //    {
            //        selector.Configure("Canopy Shield Adjustment", "Whether to position the shield mesh at the top of the screen or at the bottom of the screen ('Bonnet', default for internal/canopy camera mode) when using the internal/canopy camera mode.",
            //            CanopyShieldAdjustment, null, "Umbrella", "Bonnet");
            //    },
            //    selector =>
            //    {
            //        CanopyShieldAdjustment = selector.Value;
            //    });

            //ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "MeshRotationLockToggle_ID",
            //    selector =>
            //    {
            //        selector.Configure("Mesh Rotation Lock", "Whether to keep the shield and ship meshes visually locked or allow them to rotate freely with the ship's rigidbody when using the cockpit camera mode or the canopy/internal camera mode.",
            //            MeshRotationLockToggle, null, "Locked", "Free-Rotating");
            //    },
            //    selector =>
            //    {
            //        MeshRotationLockToggle = selector.Value;
            //    });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "CameraBehavior2280_ID",
                selector =>
                {
                    selector.Configure("2280 Camera Behavior", "Internal\n    2280 cameras will behave as normal.\n\n2280 Tilt Lock\n    2280 cameras will have their tilt locked to Z=0 degrees, \n    similar to 2159.\n\nPseudohugger\n    2280 cameras will align to the tilt of the track surface.\n\n    WARNING: On a small number of tracks, Pseudohugger\n    behavior will be jarring at breaks in the track surface.",
                        CameraBehavior2280, null, "Internal", "Tilt Lock", "Pseudohugger");
                },
                selector =>
                {
                    CameraBehavior2280 = selector.Value;
                });

            //ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "ForcePseudohugger2159_ID",
            //    selector =>
            //    {
            //        selector.Configure("Force Pseudohugger in 2159", "Whether to enable the forcing of the Pseudohugger camera behavior mode in 2159 physics.\n\n    WARNING: On a small number of tracks, Pseudohugger\n    behavior will be jarring at breaks in the track surface.",
            //            ForcePseudohugger2159, EBooleanDisplayType.EnabledDisabled);
            //    },
            //    selector =>
            //    {
            //        ForcePseudohugger2159 = selector.ToBool();
            //    });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory8, "ForceNoTiltLock2159_ID",
                selector =>
                {
                    selector.Configure("Force No Tilt Lock in 2159", "Whether to enable the forcing of No Tilt Lock on all track sections in 2159. This will prevent your ship from locking its tilt and will generally make racing cleanly and skipping harder.",
                        ForceNoTiltLock2159, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ForceNoTiltLock2159 = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory9, "FinalLapWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Final Lap Warning", "Whether to enable the Final Lap warning.",
                        FinalLapWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    FinalLapWarningToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory9, "TremorWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Tremor Warning", "Whether to enable the Tremor warning.",
                        TremorWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    TremorWarningToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory9, "HunterWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Hunter Warning", "Whether to enable the Hunter warning.",
                        HunterWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    HunterWarningToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory9, "ShieldTimerToggle_ID",
                selector =>
                {
                    selector.Configure("Shield Timer", "Whether to enable the Shield timer.",
                        ShieldTimerToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ShieldTimerToggle = selector.ToBool();
                });            

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory10, "UseZoneColorsToggle_ID",
                selector =>
                {
                    selector.Configure("Use Zone Colors", "Whether to enable the use of zone colors for the zone score, zone count, and zone title HUD elements.",
                        UseZoneColorsToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    UseZoneColorsToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory10, "PerfectZoneWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Perfect Zone Warning", "Whether to enable the perfect zone warning. When enabled, perfect zone will be displayed at the bottom of the screen whenever the current zone being progressed through is considered to be a perfect zone.",
                        PerfectZoneWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    PerfectZoneWarningToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory11, "BarrierWarningPosition_ID",
                selector =>
                {
                    selector.Configure("Barrier Warning Position", "Whether the barrier warning should be positioned at the top or bottom of the screen.",
                        BarrierWarningPosition, null, "Top", "Bottom");
                },
                selector =>
                {
                    BarrierWarningPosition = selector.Value;
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory11, "UseUpsurgeColorsToggle_ID",
                selector =>
                {
                    selector.Configure("Use Upsurge Colors", "Whether to enable the use of upsurge colors for the upsurge display HUD elements.",
                        UseUpsurgeColorsToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    UseUpsurgeColorsToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory11, "ZonesFullWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Zones Full Warning", "Whether to enable the zones full warning. When enabled, text will be displayed whenever you have a full 10 zones stored.",
                        ZonesFullWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    ZonesFullWarningToggle = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory11, "TargetAttainableWarningToggle_ID",
                selector =>
                {
                    selector.Configure("Target Attainable Warning", "Whether to enable the target attainable warning. When enabled, text will be displayed in the center of the screen when the Upsurge zone target is attainable.",
                        TargetAttainableWarningToggle, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    TargetAttainableWarningToggle = selector.ToBool();
                });                                                

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "BarrelRollKeyCode_ID",
                selector =>
                {
                    selector.Configure("Barrel Roll Keybind", "Custom binding for single-button barrel rolls. Set to 'None' to leave this unbound/disabled.",
                        BarrelRollKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, BarrelRollKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    BarrelRollKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "SideshiftLeftKeyCode_ID",
                selector =>
                {
                    selector.Configure("Sideshift Left Keybind", "Custom binding for sideshifting left. Set to 'None' to leave this unbound/disabled.",
                        SideshiftLeftKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, SideshiftLeftKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    SideshiftLeftKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "SideshiftRightKeyCode_ID",
                selector =>
                {
                    selector.Configure("Sideshift Right Keybind", "Custom binding for sideshifting right. Set to 'None' to leave this unbound/disabled.",
                        SideshiftRightKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, SideshiftRightKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    SideshiftRightKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "SmartshiftKeyCode_ID",
                selector =>
                {
                    selector.Configure("Smartshift Keybind", "Custom binding for 'smartshifting', which will automatically execute a sideshift in the direction you are steering or the direction of the last steer you inputted. Set to 'None' to leave this unbound/disabled.",
                        SmartshiftKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, SmartshiftKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    SmartshiftKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "PreviousSongKeyCode_ID",
                selector =>
                {
                    selector.Configure("Previous Song Keybind", "Custom binding for playing the previous song in the in-game music playlist without having to open the pause menu. Set to 'None' to leave this unbound/disabled.",
                        PreviousSongKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, PreviousSongKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    PreviousSongKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "NameTag_And_ShieldBars_Visibility_Toggle_KeyCode_ID",
                selector =>
                {
                    selector.Configure("Name Tag And Shield Bars Visibility Toggle Keybind", "Custom binding for toggling the visibility of name tags and shield bars. Set to 'None' to leave this unbound/disabled.",
                        NameTag_And_ShieldBars_Visibility_Toggle_KeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, NameTag_And_ShieldBars_Visibility_Toggle_KeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    NameTag_And_ShieldBars_Visibility_Toggle_KeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "VisibilityToggleAffectsRechargeSum_ID",
                selector =>
                {
                    selector.Configure("Visibility Toggle Affects Recharge Sum", "Whether the name tag and shield bar visibility toggle keybind should also hide/show the recharge sum readout.",
                        VisibilityToggleAffectsRechargeSum, EBooleanDisplayType.EnabledDisabled);
                },
                selector =>
                {
                    VisibilityToggleAffectsRechargeSum = selector.ToBool();
                });

            ModOptions.RegisterOption<NgBoxSelector>(false, ModID, SelectorCategory12, "SelfDestructKeyCode_ID",
                selector =>
                {
                    selector.Configure("Self-Destruct Keybind", "Custom binding for self-destructing and forcing a respawn when you get stuck. Set to 'None' to leave this unbound/disabled.",
                        SelfDestructKeyCode);
                    selector.SetOptions(Array.IndexOf(AllKeyCodes, SelfDestructKeyCode), AllKeyCodeNames);
                }, selector =>
                {
                    SelfDestructKeyCode = AllKeyCodes[selector.Value];
                });

            ModOptions.RegisterOption<NgBoxSlider>(false, ModID, SelectorCategory12, "SelfDestructTimer_ID",
                slider =>
                {
                    slider.Configure("Self Destruct Timer", "How many consecutive seconds you have to hold the self-destruct button down for in order to trigger a self-destruct. Setting this to 0 will allow you to self-destruct immediately when you press the self-destruct binding.",
                        " Seconds", SelfDestructTimer, 0.00f, 3.00f, 0.01f);
                }, slider =>
                {
                    SelfDestructTimer = slider.Value;
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

            WeaponMirrorPositionSwap = ini.ReadValue(SelectorCategory0, "WeaponMirrorPositionSwap_ID", WeaponMirrorPositionSwap);
            ForceShieldBars = ini.ReadValue(SelectorCategory0, "ForceShieldBars_ID", ForceShieldBars);
            ForceNameTags = ini.ReadValue(SelectorCategory0, "ForceNameTags_ID", ForceNameTags);
            MultiplayerCountdownEndSound = ini.ReadValue(SelectorCategory0, "MultiplayerCountdownEndSound_ID", MultiplayerCountdownEndSound);
            RespawnDarkenerToggle = ini.ReadValue(SelectorCategory0, "RespawnDarkenerToggle_ID", RespawnDarkenerToggle);
            CannonFirerateOverride = ini.ReadValue(SelectorCategory0, "CannonFirerateOverride_ID", CannonFirerateOverride);

            MusicDisplayStyle = ini.ReadValue(SelectorCategory1, "MusicDisplayStyle_ID", MusicDisplayStyle);
            PitlaneIndicatorStyle = ini.ReadValue(SelectorCategory1, "PitlaneIndicatorStyle_ID", PitlaneIndicatorStyle);
            PitlaneIndicatorPosition = ini.ReadValue(SelectorCategory1, "PitlaneIndicatorPosition_ID", PitlaneIndicatorPosition);

            MissileIconStyle = ini.ReadValue(SelectorCategory2, "MissileIconStyle_ID", MissileIconStyle);
            RocketsIconStyle = ini.ReadValue(SelectorCategory2, "RocketsIconStyle_ID", RocketsIconStyle);

            RearViewMirror2159 = ini.ReadValue(SelectorCategory3, "RearViewMirror2159_ID", RearViewMirror2159);
            RearViewMirror2280 = ini.ReadValue(SelectorCategory3, "RearViewMirror2280_ID", RearViewMirror2280);
            RearViewMirrorFloorhugger = ini.ReadValue(SelectorCategory3, "RearViewMirrorFloorhugger_ID", RearViewMirrorFloorhugger);

            SpeedometerReadoutStyle = ini.ReadValue(SelectorCategory4, "SpeedometerReadoutStyle_ID", SpeedometerReadoutStyle);
            EnergyBarReadoutDecimalPrecision = ini.ReadValue(SelectorCategory4, "EnergyBarReadoutDecimalPrecision_ID", EnergyBarReadoutDecimalPrecision);
            RechargeSumToggle = ini.ReadValue(SelectorCategory4, "RechargeSumToggle_ID", RechargeSumToggle);
            RechargeSumPosition = ini.ReadValue(SelectorCategory4, "RechargeSumPosition_ID", RechargeSumPosition);
            AbsoluteShieldValueStyle = ini.ReadValue(SelectorCategory4, "AbsoluteShieldValueStyle_ID", AbsoluteShieldValueStyle);

            DamageFlasherToggle = ini.ReadValue(SelectorCategory5, "DamageFlasherToggle_ID", DamageFlasherToggle);
            RelativeTimeDisplayToggle = ini.ReadValue(SelectorCategory5, "RelativeTimeDisplayToggle_ID", RelativeTimeDisplayToggle);            
            LastAttackerToggle = ini.ReadValue(SelectorCategory5, "LastAttackerToggle_ID", LastAttackerToggle);
            OvertakeRadarToggle = ini.ReadValue(SelectorCategory5, "OvertakeRadarToggle_ID", OvertakeRadarToggle);
            OvertakeRadarVisibility = ini.ReadValue(SelectorCategory5, "OvertakeRadarVisibility_ID", OvertakeRadarVisibility);
            ExtraWeaponInfoToggle = ini.ReadValue(SelectorCategory5, "ExtraWeaponInfoToggle_ID", ExtraWeaponInfoToggle);

            HyperThrustBarToggle = ini.ReadValue(SelectorCategory6, "HyperThrustBarToggle_ID", HyperThrustBarToggle);
            HyperThrustBarTextToggle = ini.ReadValue(SelectorCategory6, "HyperThrustBarTextToggle_ID", HyperThrustBarTextToggle);
            HyperThrustBarVisibility = ini.ReadValue(SelectorCategory6, "HyperThrustBarVisibility_ID", HyperThrustBarVisibility);
            HyperThrustBarPosition = ini.ReadValue(SelectorCategory6, "HyperThrustBarPosition_ID", HyperThrustBarPosition);            

            SpeedPadCounterToggle = ini.ReadValue(SelectorCategory7, "SpeedPadCounterToggle_ID", SpeedPadCounterToggle);
            SpeedPadCounterTextToggle = ini.ReadValue(SelectorCategory7, "SpeedPadCounterTextToggle_ID", SpeedPadCounterTextToggle);
            SpeedPadCounterVisibility = ini.ReadValue(SelectorCategory7, "SpeedPadCounterVisibility_ID", SpeedPadCounterVisibility);
            SpeedPadTimerToggle = ini.ReadValue(SelectorCategory7, "SpeedPadTimerToggle_ID", SpeedPadTimerToggle);
            SpeedPadTimerTextToggle = ini.ReadValue(SelectorCategory7, "SpeedPadTimerTextToggle_ID", SpeedPadTimerTextToggle);
            SpeedPadTimerVisibility = ini.ReadValue(SelectorCategory7, "SpeedPadTimerVisibility_ID", SpeedPadTimerVisibility);
            SpeedPadElementsPosition = ini.ReadValue(SelectorCategory7, "SpeedPadElementsPosition_ID", SpeedPadElementsPosition);

            AdjustmentAlignmentFixToggle = ini.ReadValue(SelectorCategory8, "AdjustmentAlignmentFixToggle_ID", AdjustmentAlignmentFixToggle);
            CanopyCameraAdjustment2280 = ini.ReadValue(SelectorCategory8, "CanopyCameraAdjustment2280_ID", CanopyCameraAdjustment2280);
            CockpitCameraAdjustment2280 = ini.ReadValue(SelectorCategory8, "CockpitCameraAdjustment2280_ID", CockpitCameraAdjustment2280);
            CockpitCameraAdjustment2159 = ini.ReadValue(SelectorCategory8, "CockpitCameraAdjustment2159_ID", CockpitCameraAdjustment2159);
            CanopyMeshAdjustment = ini.ReadValue(SelectorCategory8, "CanopyMeshAdjustment_ID", CanopyMeshAdjustment);
            CockpitMeshAdjustment = ini.ReadValue(SelectorCategory8, "CockpitMeshAdjustment_ID", CockpitMeshAdjustment);
            //CockpitShieldAdjustment = ini.ReadValue(SelectorCategory8, "CockpitShieldAdjustment_ID", CockpitShieldAdjustment);
            //CanopyShieldAdjustment = ini.ReadValue(SelectorCategory8, "CanopyShieldAdjustment_ID", CanopyShieldAdjustment);
            //MeshRotationLockToggle = ini.ReadValue(SelectorCategory8, "MeshRotationLockToggle_ID", MeshRotationLockToggle);
            CameraBehavior2280 = ini.ReadValue(SelectorCategory8, "CameraBehavior2280_ID", CameraBehavior2280);
            //ForcePseudohugger2159 = ini.ReadValue(SelectorCategory8, "ForcePseudohugger2159_ID", ForcePseudohugger2159);
            ForceNoTiltLock2159 = ini.ReadValue(SelectorCategory8, "ForceNoTiltLock2159_ID", ForceNoTiltLock2159);

            FinalLapWarningToggle = ini.ReadValue(SelectorCategory9, "FinalLapWarningToggle_ID", FinalLapWarningToggle);
            TremorWarningToggle = ini.ReadValue(SelectorCategory9, "TremorWarningToggle_ID", TremorWarningToggle);
            HunterWarningToggle = ini.ReadValue(SelectorCategory9, "HunterWarningToggle_ID", HunterWarningToggle);
            ShieldTimerToggle = ini.ReadValue(SelectorCategory9, "ShieldTimerToggle_ID", ShieldTimerToggle);

            //ControllerInputType = ini.ReadValue(SelectorCategory0, "ControllerInputType_ID", ControllerInputType);

            UseZoneColorsToggle = ini.ReadValue(SelectorCategory10, "UseZoneColorsToggle_ID", UseZoneColorsToggle);
            PerfectZoneWarningToggle = ini.ReadValue(SelectorCategory10, "PerfectZoneWarningToggle_ID", PerfectZoneWarningToggle);

            BarrierWarningPosition = ini.ReadValue(SelectorCategory11, "BarrierWarningPosition_ID", BarrierWarningPosition);
            UseUpsurgeColorsToggle = ini.ReadValue(SelectorCategory11, "UseUpsurgeColorsToggle_ID", UseUpsurgeColorsToggle);
            ZonesFullWarningToggle = ini.ReadValue(SelectorCategory11, "ZonesFullWarningToggle_ID", ZonesFullWarningToggle);
            TargetAttainableWarningToggle = ini.ReadValue(SelectorCategory11, "TargetAttainableWarningToggle_ID", TargetAttainableWarningToggle);                                                

            BarrelRollKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "BarrelRollKeyCode_ID", (int)BarrelRollKeyCode);
            SideshiftLeftKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "SideshiftLeftKeyCode_ID", (int)SideshiftLeftKeyCode);
            SideshiftRightKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "SideshiftRightKeyCode_ID", (int)SideshiftRightKeyCode);
            SmartshiftKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "SmartshiftKeyCode_ID", (int)SmartshiftKeyCode);
            PreviousSongKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "PreviousSongKeyCode_ID", (int)PreviousSongKeyCode);
            NameTag_And_ShieldBars_Visibility_Toggle_KeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "NameTag_And_ShieldBars_Visibility_Toggle_KeyCode_ID", (int)NameTag_And_ShieldBars_Visibility_Toggle_KeyCode);
            VisibilityToggleAffectsRechargeSum = ini.ReadValue(SelectorCategory12, "VisibilityToggleAffectsRechargeSum_ID", VisibilityToggleAffectsRechargeSum);
            SelfDestructKeyCode = (KeyCode)ini.ReadValue(SelectorCategory12, "SelfDestructKeyCode_ID", (int)SelfDestructKeyCode);
            SelfDestructTimer = (float)ini.ReadValue(SelectorCategory12, "SelfDestructTimer_ID", SelfDestructTimer);

            ini.Close();
        }

        //private void OnSaveSettings()
        private void OnConfigWrite()
        {
            INIParser ini = new INIParser();
            //ini.Open(_settingsIni);
            ini.Open(_configPath);

            ini.WriteValue(SelectorCategory0, "WeaponMirrorPositionSwap_ID", WeaponMirrorPositionSwap);
            ini.WriteValue(SelectorCategory0, "ForceShieldBars_ID", ForceShieldBars);
            ini.WriteValue(SelectorCategory0, "ForceNameTags_ID", ForceNameTags);
            ini.WriteValue(SelectorCategory0, "MultiplayerCountdownEndSound_ID", MultiplayerCountdownEndSound);
            ini.WriteValue(SelectorCategory0, "RespawnDarkenerToggle_ID", RespawnDarkenerToggle);
            ini.WriteValue(SelectorCategory0, "CannonFirerateOverride_ID", CannonFirerateOverride);

            ini.WriteValue(SelectorCategory1, "MusicDisplayStyle_ID", MusicDisplayStyle);
            ini.WriteValue(SelectorCategory1, "PitlaneIndicatorStyle_ID", PitlaneIndicatorStyle);
            ini.WriteValue(SelectorCategory1, "PitlaneIndicatorPosition_ID", PitlaneIndicatorPosition);

            ini.WriteValue(SelectorCategory2, "MissileIconStyle_ID", MissileIconStyle);
            ini.WriteValue(SelectorCategory2, "RocketsIconStyle_ID", RocketsIconStyle);

            ini.WriteValue(SelectorCategory3, "RearViewMirror2159_ID", RearViewMirror2159);
            ini.WriteValue(SelectorCategory3, "RearViewMirror2280_ID", RearViewMirror2280);
            ini.WriteValue(SelectorCategory3, "RearViewMirrorFloorhugger_ID", RearViewMirrorFloorhugger);

            ini.WriteValue(SelectorCategory4, "SpeedometerReadoutStyle_ID", SpeedometerReadoutStyle);
            ini.WriteValue(SelectorCategory4, "EnergyBarReadoutDecimalPrecision_ID", EnergyBarReadoutDecimalPrecision);
            ini.WriteValue(SelectorCategory4, "RechargeSumToggle_ID", RechargeSumToggle);
            ini.WriteValue(SelectorCategory4, "RechargeSumPosition_ID", RechargeSumPosition);
            ini.WriteValue(SelectorCategory4, "AbsoluteShieldValueStyle_ID", AbsoluteShieldValueStyle);

            ini.WriteValue(SelectorCategory5, "DamageFlasherToggle_ID", DamageFlasherToggle);
            ini.WriteValue(SelectorCategory5, "RelativeTimeDisplayToggle_ID", RelativeTimeDisplayToggle);
            ini.WriteValue(SelectorCategory5, "LastAttackerToggle_ID", LastAttackerToggle);
            ini.WriteValue(SelectorCategory5, "OvertakeRadarToggle_ID", OvertakeRadarToggle);
            ini.WriteValue(SelectorCategory5, "OvertakeRadarVisibility_ID", OvertakeRadarVisibility);
            ini.WriteValue(SelectorCategory5, "ExtraWeaponInfoToggle_ID", ExtraWeaponInfoToggle);

            ini.WriteValue(SelectorCategory6, "HyperThrustBarToggle_ID", HyperThrustBarToggle);
            ini.WriteValue(SelectorCategory6, "HyperThrustBarTextToggle_ID", HyperThrustBarTextToggle);
            ini.WriteValue(SelectorCategory6, "HyperThrustBarVisibility_ID", HyperThrustBarVisibility);
            ini.WriteValue(SelectorCategory6, "HyperThrustBarPosition_ID", HyperThrustBarPosition);            

            ini.WriteValue(SelectorCategory7, "SpeedPadCounterToggle_ID", SpeedPadCounterToggle);
            ini.WriteValue(SelectorCategory7, "SpeedPadCounterTextToggle_ID", SpeedPadCounterTextToggle);
            ini.WriteValue(SelectorCategory7, "SpeedPadCounterVisibility_ID", SpeedPadCounterVisibility);
            ini.WriteValue(SelectorCategory7, "SpeedPadTimerToggle_ID", SpeedPadTimerToggle);
            ini.WriteValue(SelectorCategory7, "SpeedPadTimerTextToggle_ID", SpeedPadTimerTextToggle);
            ini.WriteValue(SelectorCategory7, "SpeedPadTimerVisibility_ID", SpeedPadTimerVisibility);
            ini.WriteValue(SelectorCategory7, "SpeedPadElementsPosition_ID", SpeedPadElementsPosition);

            ini.WriteValue(SelectorCategory8, "AdjustmentAlignmentFixToggle_ID", AdjustmentAlignmentFixToggle);
            ini.WriteValue(SelectorCategory8, "CanopyCameraAdjustment2280_ID", CanopyCameraAdjustment2280);
            ini.WriteValue(SelectorCategory8, "CockpitCameraAdjustment2280_ID", CockpitCameraAdjustment2280);
            ini.WriteValue(SelectorCategory8, "CockpitCameraAdjustment2159_ID", CockpitCameraAdjustment2159);
            ini.WriteValue(SelectorCategory8, "CanopyMeshAdjustment_ID", CanopyMeshAdjustment);
            ini.WriteValue(SelectorCategory8, "CockpitMeshAdjustment_ID", CockpitMeshAdjustment);
            //ini.WriteValue(SelectorCategory8, "CockpitShieldAdjustment_ID", CockpitShieldAdjustment);
            //ini.WriteValue(SelectorCategory8, "CanopyShieldAdjustment_ID", CanopyShieldAdjustment);
            //ini.WriteValue(SelectorCategory8, "MeshRotationLockToggle_ID", MeshRotationLockToggle);
            ini.WriteValue(SelectorCategory8, "CameraBehavior2280_ID", CameraBehavior2280);
            //ini.WriteValue(SelectorCategory8, "ForcePseudohugger2159_ID", ForcePseudohugger2159);
            ini.WriteValue(SelectorCategory8, "ForceNoTiltLock2159_ID", ForceNoTiltLock2159);

            ini.WriteValue(SelectorCategory9, "FinalLapWarningToggle_ID", FinalLapWarningToggle);
            ini.WriteValue(SelectorCategory9, "TremorWarningToggle_ID", TremorWarningToggle);
            ini.WriteValue(SelectorCategory9, "HunterWarningToggle_ID", HunterWarningToggle);
            ini.WriteValue(SelectorCategory9, "ShieldTimerToggle_ID", ShieldTimerToggle);

            //ini.WriteValue(SelectorCategory0, "ControllerInputType_ID", ControllerInputType);

            ini.WriteValue(SelectorCategory10, "UseZoneColorsToggle_ID", UseZoneColorsToggle);
            ini.WriteValue(SelectorCategory10, "PerfectZoneWarningToggle_ID", PerfectZoneWarningToggle);

            ini.WriteValue(SelectorCategory11, "BarrierWarningPosition_ID", BarrierWarningPosition);
            ini.WriteValue(SelectorCategory11, "UseUpsurgeColorsToggle_ID", UseUpsurgeColorsToggle);
            ini.WriteValue(SelectorCategory11, "ZonesFullWarningToggle_ID", ZonesFullWarningToggle);
            ini.WriteValue(SelectorCategory11, "TargetAttainableWarningToggle_ID", TargetAttainableWarningToggle);                                                

            ini.WriteValue(SelectorCategory12, "BarrelRollKeyCode_ID", (int)BarrelRollKeyCode);
            ini.WriteValue(SelectorCategory12, "SideshiftLeftKeyCode_ID", (int)SideshiftLeftKeyCode);
            ini.WriteValue(SelectorCategory12, "SideshiftRightKeyCode_ID", (int)SideshiftRightKeyCode);
            ini.WriteValue(SelectorCategory12, "SmartshiftKeyCode_ID", (int)SmartshiftKeyCode);
            ini.WriteValue(SelectorCategory12, "PreviousSongKeyCode_ID", (int)PreviousSongKeyCode);
            ini.WriteValue(SelectorCategory12, "NameTag_And_ShieldBars_Visibility_Toggle_KeyCode_ID", (int)NameTag_And_ShieldBars_Visibility_Toggle_KeyCode);
            ini.WriteValue(SelectorCategory12, "VisibilityToggleAffectsRechargeSum_ID", VisibilityToggleAffectsRechargeSum);
            ini.WriteValue(SelectorCategory12, "SelfDestructKeyCode_ID", (int)SelfDestructKeyCode);
            ini.WriteValue(SelectorCategory12, "SelfDestructTimer_ID", SelfDestructTimer);

            ini.Close();
        }
    }
}