using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BallisticModding;
using BallisticUnityTools.Placeholders;
using BallisticUnityTools;
using BallisticNG;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NgContent;
using NgData;
using NgEvents;
using NgGame;
using NgLib;
using NgModes;
using NgModding.Huds;
using NgModding;
using NgMp;
using NgMusic;
using NgPickups;
using NgSettings;
using NgShips;
using NgSp;
using NgUi.RaceUi;

namespace ClassLibrary1HUD
{
    public class HudRegister : CodeMod
    {
        string id = "VanillaPlus";
        string HudPath = "vanillaplushud.hud";

        public NgNetworkBase New_Network;
        public int StartGameCounter;
        public AudioClip End_Beep;

        public static AssetBundle VanillaPlusHUD;
        public static SceneHudManager CurrentHudManager;

        public override void OnRegistered(string modPath)
        {
            DebugConsole.Log($"<color=#ABF3CD>[VanillaPlusHUD Version 1.1.2]</color> MD5 hash: {CodeModLoader.ActiveMods.Find(x => x.Name == id).DllMd5.ToUpper()}");

            NgNetworkBase.OnNetworkStart += ConnectToLobby;
            End_Beep = LoadWavFile((modPath + "/Audio/mpcountdownend.wav"));

            string TestPathString = Path.Combine(modPath, "config.ini");
            INIParser ini = new INIParser();
            ini.Open(TestPathString);

            VanillaPlusHUD = AssetBundle.LoadFromFile(Path.Combine(modPath, HudPath));
            CustomHudRegistry.RegisterMod(id);
            CustomHudRegistry.RegisterSceneManager("Race", id, new RaceHudManager());
            CustomHudRegistry.RegisterSceneManager("Speed Lap", id, new SpeedlapHudManager());
            CustomHudRegistry.RegisterSceneManager("Survival", id, new SurvivalHudManager());
            CustomHudRegistry.RegisterSceneManager("Upsurge", id, new UpsurgeHudManager());
            CustomHudRegistry.RegisterSceneManager("Stunt", id, new StuntHudManager());
            CustomHudRegistry.RegisterSceneManager("Precision", id, new PrecisionHudManager());
            CustomHudRegistry.RegisterSceneManager("Practice", id, new PracticeHudManager());
            CustomHudRegistry.RegisterSceneManager("Track Creator", id, new TrackCreatorHudManager());
            CustomHudRegistry.RegisterSceneManager("Rush Hour", id, new RushHourHudManager());
            CustomHudRegistry.RegisterSceneManager("Eliminator", id, new EliminatorHudManager());
            CustomHudRegistry.RegisterSceneManager("Knockout", id, new KnockoutHudManager());
            CustomHudRegistry.RegisterSceneManager("Time Trial", id, new TimeTrialHudManager());
            CustomHudRegistry.RegisterSceneManager("Online Team Race", id, new OnlineTeamRaceHudManager());
            CustomHudRegistry.RegisterSceneManager("Team Race", id, new TeamRaceHudManager());

            CustomHudRegistry.RegisterWeaponSprite("autopilot", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/AutoPilot.png")));
            CustomHudRegistry.RegisterWeaponSprite("cannon", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Cannon.png")));
            CustomHudRegistry.RegisterWeaponSprite("energywall", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/EnergyWall.png")));
            CustomHudRegistry.RegisterWeaponSprite("emergencypack", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/EPack.png")));
            CustomHudRegistry.RegisterWeaponSprite("hellstorm", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Hellstorm.png")));
            CustomHudRegistry.RegisterWeaponSprite("hunter", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Hunter.png")));
            //switch (VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle)
            switch (ini.ReadValue("Weapon Icons", "MissileIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle))
            {
                case 0:
                    CustomHudRegistry.RegisterWeaponSprite("missile", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Missile.png")));
                    break;
                case 1:
                    CustomHudRegistry.RegisterWeaponSprite("missile", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/MissileInscribedTriangle.png")));
                    break;
            }
            CustomHudRegistry.RegisterWeaponSprite("mines", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Mines.png")));
            CustomHudRegistry.RegisterWeaponSprite("plasma", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Plasma.png")));
            CustomHudRegistry.RegisterWeaponSprite("tremor", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Quake.png")));
            //switch (VanillaPlusHUDOptions.ModMenuOptions.RocketsIconStyle)
            switch (ini.ReadValue("Weapon Icons", "RocketsIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.RocketsIconStyle))
            {
                case 0:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Rockets.png")));
                    break;
                case 1:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/RocketsTrefoil.png")));
                    break;
                case 2:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/RocketsTrefoilUp.png")));
                    break;
                case 3:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/RocketsHorizontal.png")));
                    break;
                case 4:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/RocketsVertical.png")));
                    break;
                case 5:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/RocketsTetrahedronNet.png")));
                    break;
            }
            
            CustomHudRegistry.RegisterWeaponSprite("shield", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Shield.png")));
            CustomHudRegistry.RegisterWeaponSprite("turbo", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Turbo.png")));

            ini.Close();
        }

        public AudioClip LoadWavFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            WWW www = new WWW("file:///" + filePath);
            while (!www.isDone) 
            { 
                //Do Nothing
            }
            return www.GetAudioClip(false, false, AudioType.WAV);
        }

        public void ConnectToLobby(bool iServer)
        {
            New_Network = NgNetworkBase.CurrentNetwork;
            New_Network.OnCountdownStarted += MultiplayerCountdownStarted;
            New_Network.OnCountdownCanceled += MultiplayerCountdownCanceled;
            StartGameCounter = 0;
        }

        public void MultiplayerCountdownStarted(NgMp.Packets.NgCountdownHeaders type)
        {
            if (type == NgMp.Packets.NgCountdownHeaders.StartGameSound)
            {
                StartGameCounter += 1;
            }

            //DebugConsole.Log(type.ToString() + " " + StartGameCounter.ToString());

            if (StartGameCounter == 3)
            {
                if (VanillaPlusHUDOptions.ModMenuOptions.MultiplayerCountdownEndSound)
                {
                    NgAudio.NgSound.PlayVoice(End_Beep);
                }
                StartGameCounter = 0;
            }
        }

        public void MultiplayerCountdownCanceled(NgMp.Packets.NgCountdownHeaders type)
        {
            StartGameCounter = 0;
        }
    }

    public class RaceHudManager : SceneHudManager //Single Race and Tournament
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }            

            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab");
            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");

            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }            

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter Field.prefab"); //made the 'I' in 'FIeld' lowercase

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterHud<Best_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Best Time Field.prefab"); //made the 'I' in 'FIeld' lowercase

            if (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorStyle == 0)
            {
                RegisterHud<Pitlane_Indicator>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pitlane Indicator.prefab");
            }
            else
            {
                RegisterInternalHud("PitlaneIndicator"); //Internal Pitlane Indicator
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }
            
            RegisterHud<Lap_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Time Field.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            //RegisterHud<>(HudRegister.VanillaPlusHUD, "");
            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }
            
            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            if (NgNetworkBase.CurrentNetwork != null || NgSettings.Hud.PositionBoardInSingleplayer == true)
            {
                RegisterInternalHud("NetworkPeerList"); //Positionboard on the right side of the screen
            }
            
            RegisterInternalHud("KnockoutShipTracker"); //The position/lap progress tracker on the left side of the screen
            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator
            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }
            RegisterInternalHud("NetworkRaceFinish"); //Race finishes in 30 seconds

            HudRegister.CurrentHudManager = this;
        }
    }

    public class SpeedlapHudManager : SceneHudManager //Speedlap
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Speedlap_And_Precision>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedlap_Precision.prefab");            
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab"); //For Turbo pickup display

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }
            
            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class SurvivalHudManager : SceneHudManager //Survival
    {
        public override void OnCreateHuds()
        {
            RegisterHud<Survival>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Survival.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab");
            }

            if (NgCampaign.Enabled == true)
            {
                RegisterInternalHud("ZoneAwards");
            }
                
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            
            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class UpsurgeHudManager : SceneHudManager //Upsurge
    {
        public override void OnCreateHuds()
        {
            RegisterHud<Upsurge>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Upsurge.prefab");
            RegisterInternalHud("EliminatorScoreList"); //Upsurge scoreboard

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab");
            }

            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }

            HudRegister.CurrentHudManager = this;
        }        
    }

    public class StuntHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {            
            RegisterHud<Stunt>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Stunt.prefab");
            RegisterInternalHud("ChainScore");
            RegisterInternalHud("TitleDisplay"); //OVERTIME warning

            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter Field.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class PrecisionHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterInternalHud("PrecisionHud");

            RegisterHud<Speedlap_And_Precision>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedlap_Precision.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class PracticeHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");            
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");      
            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }            

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab");
            }            

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("KnockoutShipTracker");
            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class TrackCreatorHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");

            RegisterHud<Speedlap_And_Precision>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedlap_Precision.prefab");

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class RushHourHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");            
            RegisterInternalHud("ShipEnergyScoreboard");

            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class EliminatorHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");            
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterInternalHud("Eliminator"); //Shield bars
            RegisterInternalHud("EliminatorScoreList");
            RegisterInternalHud("KnockoutShipTracker");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab"); //TEST IN ELIMINATOR AND ON SW1R TRACKS ON REGULAR RACE
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }
                        
            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class KnockoutHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterInternalHud("KnockoutShipTracker");
            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");

            if (NgNetworkBase.CurrentNetwork != null || NgSettings.Hud.PositionBoardInSingleplayer == true)
            {
                RegisterInternalHud("NetworkPeerList"); //Positionboard on the right side of the screen
            }

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorStyle == 0)
            {
                RegisterHud<Pitlane_Indicator>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pitlane Indicator.prefab");
            }
            else
            {
                RegisterInternalHud("PitlaneIndicator"); //Internal Pitlane Indicator
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab"); //TEST IN ELIMINATOR AND ON SW1R TRACKS ON REGULAR RACE
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class TimeTrialHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab"); //For Turbo pickup display
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter Field.prefab");
            RegisterHud<Best_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Best Time Field.prefab");
            RegisterHud<Lap_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Time Field.prefab");

            if (NgCampaign.Enabled == true)
            {
                RegisterInternalHud("TimeTrialAwards");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorStyle == 0)
            {
                RegisterHud<Pitlane_Indicator>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pitlane Indicator.prefab");
            }
            else
            {
                RegisterInternalHud("PitlaneIndicator"); //Internal Pitlane Indicator
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab"); //TEST IN ELIMINATOR AND ON SW1R TRACKS ON REGULAR RACE
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class OnlineTeamRaceHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab"); //For Turbo pickup display
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");
            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter Field.prefab");
            RegisterHud<Best_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Best Time Field.prefab");
            RegisterHud<Lap_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Time Field.prefab");
            RegisterInternalHud("KnockoutShipTracker");
            RegisterInternalHud("OnlineTeamMateDisplay");
            RegisterInternalHud("OnlineTeamScoreList");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorStyle == 0)
            {
                RegisterHud<Pitlane_Indicator>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pitlane Indicator.prefab");
            }
            else
            {
                RegisterInternalHud("PitlaneIndicator"); //Internal Pitlane Indicator
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab"); //TEST IN ELIMINATOR AND ON SW1R TRACKS ON REGULAR RACE
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            RegisterInternalHud("NetworkRaceFinish"); //Race finishes in 30 seconds

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public class TeamRaceHudManager : SceneHudManager
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.ExtraWeaponInfoToggle)
            {
                RegisterHud<Extra_Weapon_Information>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Weapon Info.prefab");
            }

            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab"); //For Turbo pickup display
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Position_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Position Counter Field.prefab");
            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter Field.prefab");
            RegisterHud<Best_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Best Time Field.prefab");
            RegisterHud<Lap_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Time Field.prefab");
            RegisterInternalHud("KnockoutShipTracker");
            RegisterInternalHud("TeamScoreList");

            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.LastAttackerToggle)
            {
                RegisterHud<Last_Attacker>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Last Attacker.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorStyle == 0)
            {
                RegisterHud<Pitlane_Indicator>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pitlane Indicator.prefab");
            }
            else
            {
                RegisterInternalHud("PitlaneIndicator"); //Internal Pitlane Indicator
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumToggle)
            {
                RegisterHud<Recharge_Sum>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Recharge Sum.prefab"); //TEST IN ELIMINATOR AND ON SW1R TRACKS ON REGULAR RACE
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarToggle && Race.AfterburnerEnabled)
            {
                RegisterHud<Hyperthrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Hyperthrust Bar.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterToggle == 2)
            {
                RegisterHud<Speedpad_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Counter Field.prefab");
            }

            if (((VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 0) && (Cheats.IntFromPhysicsMod() != 1)) || VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerToggle == 2)
            {
                RegisterHud<Speedpad_Timer>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Speedpad Timer Bar.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.DamageFlasherToggle)
            {
                RegisterHud<Damage_Flasher>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Damage Flasher.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.RelativeTimeDisplayToggle)
            {
                RegisterHud<Relative_Time_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Relative Time Display.prefab");
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceNameTags)
            {
                RegisterInternalHud("NetworkNameTags"); //Nametags
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }

            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab"); //Should be in every HudManager
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab"); //Needed for skip song backwards

            if (VanillaPlusHUDOptions.ModMenuOptions.MusicDisplayStyle == 0)
            {
                RegisterHud<Music_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Music Display Field.prefab");
            }
            else
            {
                RegisterInternalHud("NowPlaying"); //Internal Music Display
            }

            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator

            if (VanillaPlusHUDOptions.ModMenuOptions.RespawnDarkenerToggle)
            {
                RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            }

            HudRegister.CurrentHudManager = this;
        }
    }

    public static class IL_Accessors
    {
        public static readonly Func<PickupMissiles, LockonSystem> GetLockonSystem;
        public static readonly Func<PickupHellstorm, int> GetHellstormLockonCount;

        static IL_Accessors()
        {
            FieldInfo fieldMissiles = typeof(PickupMissiles).GetField("_lockSystem", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldMissiles != null)
            {
                DynamicMethod dynamicGetterMissiles = new DynamicMethod("GetPrivateLockSystem", typeof(LockonSystem), new[] { typeof(PickupMissiles) }, typeof(PickupMissiles).Module, true);

                ILGenerator ilMissiles = dynamicGetterMissiles.GetILGenerator();
                ilMissiles.Emit(OpCodes.Ldarg_0);
                ilMissiles.Emit(OpCodes.Ldfld, fieldMissiles);
                ilMissiles.Emit(OpCodes.Ret);

                GetLockonSystem = (Func<PickupMissiles, LockonSystem>)dynamicGetterMissiles.CreateDelegate(typeof(Func<PickupMissiles, LockonSystem>));
            }

            PropertyInfo propHellstorm = typeof(PickupHellstorm).GetProperty("LockonCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            MethodInfo getMethodHellstorm = propHellstorm?.GetMethod;
            if (getMethodHellstorm != null)
            {
                DynamicMethod dynamicGetterHellstorm = new DynamicMethod("GetPrivateIntProperty", typeof(int), new[] { typeof(PickupHellstorm) }, typeof(PickupHellstorm).Module, true);

                ILGenerator ilHellstorm = dynamicGetterHellstorm.GetILGenerator();
                ilHellstorm.Emit(OpCodes.Ldarg_0);
                ilHellstorm.Emit(OpCodes.Call, getMethodHellstorm);
                ilHellstorm.Emit(OpCodes.Ret);

                GetHellstormLockonCount = (Func<PickupHellstorm, int>)dynamicGetterHellstorm.CreateDelegate(typeof(Func<PickupHellstorm, int>));
            }
        }
    }

    public class Extra_Weapon_Information : ScriptableHud
    {
        public Text Energy_Wall_Left_Readout;
        public Text Energy_Wall_Right_Readout;
        public Text Extra_Weapon_Information_Readout;
        public Text Lethal_Missile_Damage_Indicator;

        public static readonly string[] Hellstorm_Lock_Strings = { "×0", "×1", "×2", "×3", "×4" };

        public float Shield_Time_Remaining;
        public float Missile_Lockon_Strength;
        public float Missile_Lockon_Distance;
        public int Hellstorm_Lock_Count;
        public bool Mirror_Mode_Is_Enabled;

        public float Autopilot_Timer;

        public LockonSystem LockonSystem_Instance;

        public PickupMissiles Missile_Instance;
        public PickupHellstorm Hellstorm_Instance;
        public PickupEnergyWall EnergyWall_Instance;
        public PickupShield Shield_Instance;
        public PickupAutopilot Autopilot_Instance;

        public Outline Extra_Weapon_Information_Outline;

        public static readonly Vector4 Autopilot_Cyan = new Vector4((11f / 255f), 1f, 1f, 1f);
        public static readonly Vector4 Hellstorm_Indigo = new Vector4((56f / 255f), (56f / 255f), (161f / 255f), 1f);
        public static readonly Vector4 Missile_Fuchsia = new Vector4(1f, (25f / 255f), 1f, 1f);

        public override void Start()
        {
            base.Start();

            Energy_Wall_Left_Readout = CustomComponents.GetById<Text>("EnergyWallLeftReadout");
            Energy_Wall_Right_Readout = CustomComponents.GetById<Text>("EnergyWallRightReadout");
            Extra_Weapon_Information_Readout = CustomComponents.GetById<Text>("ExtraInfoReadout");
            Lethal_Missile_Damage_Indicator = CustomComponents.GetById<Text>("LethalMissileDamageIndicator");

            if (VanillaPlusHUDOptions.ModMenuOptions.WeaponMirrorPositionSwap)
            {
                Energy_Wall_Left_Readout.rectTransform.anchoredPosition = Energy_Wall_Left_Readout.rectTransform.anchoredPosition + Weapon_Display.Weapon_Display_Adjust_Vector;
                Energy_Wall_Right_Readout.rectTransform.anchoredPosition = Energy_Wall_Right_Readout.rectTransform.anchoredPosition + Weapon_Display.Weapon_Display_Adjust_Vector;
                Extra_Weapon_Information_Readout.rectTransform.anchoredPosition = Extra_Weapon_Information_Readout.rectTransform.anchoredPosition + Weapon_Display.Weapon_Display_Adjust_Vector;
                Lethal_Missile_Damage_Indicator.rectTransform.anchoredPosition = Lethal_Missile_Damage_Indicator.rectTransform.anchoredPosition + Weapon_Display.Weapon_Display_Adjust_Vector;
            }

            Extra_Weapon_Information_Outline = Extra_Weapon_Information_Readout.GetComponent<Outline>();

            Mirror_Mode_Is_Enabled = NgSettings.Gameplay.MirrorEnabled;
            Energy_Wall_Left_Readout.enabled = false;
            Energy_Wall_Right_Readout.enabled = false;
            Extra_Weapon_Information_Readout.enabled = false;
        }

        public override void Update()
        {
            base.Update();

            switch (TargetShip?.CurrentPickupRegister?.Name)
            {
                default:
                    Energy_Wall_Left_Readout.enabled = false;
                    Energy_Wall_Right_Readout.enabled = false;
                    Extra_Weapon_Information_Outline.effectColor = Color.black;
                    Extra_Weapon_Information_Readout.enabled = false;
                    Lethal_Missile_Damage_Indicator.enabled = false;
                    break;

                case "hellstorm":
                    Energy_Wall_Left_Readout.enabled = false;
                    Energy_Wall_Right_Readout.enabled = false;

                    Hellstorm_Instance = TargetShip.CurrentPickup as PickupHellstorm;
                    Hellstorm_Lock_Count = IL_Accessors.GetHellstormLockonCount(Hellstorm_Instance);

                    if (Hellstorm_Lock_Count > 0)
                    {
                        Extra_Weapon_Information_Readout.text = Hellstorm_Lock_Strings[Hellstorm_Lock_Count];
                        Extra_Weapon_Information_Readout.color = Hellstorm_Indigo;
                        //Extra_Weapon_Information_Outline.effectColor = Color.white;
                        Extra_Weapon_Information_Readout.enabled = true;
                    }
                    else
                    {
                        Extra_Weapon_Information_Outline.effectColor = Color.black;
                        Extra_Weapon_Information_Readout.enabled = false;
                    }
                    break;

                case "missile":
                    Energy_Wall_Left_Readout.enabled = false;
                    Energy_Wall_Right_Readout.enabled = false;

                    Missile_Instance = TargetShip.CurrentPickup as PickupMissiles;
                    LockonSystem_Instance = IL_Accessors.GetLockonSystem(Missile_Instance);

                    if (LockonSystem_Instance.TargetLockon != null)
                    {
                        Missile_Lockon_Distance = TargetShip.InverseTransformPoint(LockonSystem_Instance.TargetLockon.PhysicsPosition).magnitude;

                        Missile_Lockon_Strength = 100 * Mathf.Min(
                            (LockonSystem_Instance.MaxLockDistance - Missile_Lockon_Distance),
                            (Missile_Lockon_Distance - LockonSystem_Instance.MinLockDistance)
                            ) / (0.85f * ((LockonSystem_Instance.MaxLockDistance + LockonSystem_Instance.MinLockDistance) / 2f));
                        Extra_Weapon_Information_Readout.text = Missile_Lockon_Strength.ToString("0'%'");
                        Extra_Weapon_Information_Readout.color = Missile_Fuchsia;
                        Extra_Weapon_Information_Readout.enabled = true;

                        if (Missile_Instance.Target.ShieldIntegrity <= (TargetShip.Settings.DAMAGE_PWR * 15f * Missile_Instance.Target.Settings.DAMAGE_MULT) && !Missile_Instance.Target.IsShieldActivated && Missile_Instance.HasLock)
                        {
                            Lethal_Missile_Damage_Indicator.enabled = true;
                        }
                        else
                        {
                            Lethal_Missile_Damage_Indicator.enabled = false;
                        }
                    }
                    else
                    {
                        Extra_Weapon_Information_Readout.enabled = false;
                        Lethal_Missile_Damage_Indicator.enabled = false;
                    }                    
                    break;

                case "autopilot":                    
                    Energy_Wall_Left_Readout.enabled = false;
                    Energy_Wall_Right_Readout.enabled = false;

                    if (TargetShip.AutopilotActivated == true)
                    {
                        Autopilot_Timer -= Time.deltaTime;
                        
                        if (Autopilot_Timer < 0f)
                        {
                            Extra_Weapon_Information_Readout.text = (3f + Autopilot_Timer).ToString("0.00");
                        }
                        else
                        {
                            Extra_Weapon_Information_Readout.text = Autopilot_Timer.ToString("0.00");
                        }
                        
                        Extra_Weapon_Information_Readout.color = Autopilot_Cyan;
                        Extra_Weapon_Information_Readout.enabled = true;
                    }                    
                    else
                    {
                        Autopilot_Timer = 5f;
                        Extra_Weapon_Information_Readout.enabled = false;
                    }
                    break;

                case "energywall":
                    Extra_Weapon_Information_Readout.enabled = false;

                    if (TargetShip.CurrentSection.InverseTransformPoint(TargetShip.PhysicsPosition).x < 0f) //Ship is on left side of section
                    {
                        if (Mirror_Mode_Is_Enabled)
                        {
                            Energy_Wall_Left_Readout.enabled = true;
                            Energy_Wall_Right_Readout.enabled = false;
                        }
                        else
                        {
                            Energy_Wall_Right_Readout.enabled = true;
                            Energy_Wall_Left_Readout.enabled = false;
                        }
                        
                    }
                    else //Ship is on right side of section
                    {
                        if (Mirror_Mode_Is_Enabled)
                        {
                            Energy_Wall_Right_Readout.enabled = true;
                            Energy_Wall_Left_Readout.enabled = false;
                        }
                        else
                        {
                            Energy_Wall_Left_Readout.enabled = true;
                            Energy_Wall_Right_Readout.enabled = false;
                        }                        
                    }
                    break;
            }
        }
    }

    public class Last_Attacker : ScriptableHud
    {
        public Text Last_Attacker_Name;
        public ShipController Cached_Last_Attacker;
        public string Cached_Ship_Name;

        public override void Start()
        {
            base.Start();

            Last_Attacker_Name = CustomComponents.GetById<Text>("LastAttackerName");
            Last_Attacker_Name.text = "";
            //Last_Attacker_Name.enabled = false;
            Last_Attacker_Name.enabled = true;
        }

        public override void Update()
        {
            base.Update();

            //Last_Attacker_Name.enabled = true;

            Cached_Last_Attacker = TargetShip.LastAttacker;
            if (Cached_Last_Attacker != null)
            {
                Cached_Ship_Name = Cached_Last_Attacker.ShipName;
            }
            else
            {
                Cached_Ship_Name = "";
            }

            if (Last_Attacker_Name.text != Cached_Ship_Name)
            {
                Last_Attacker_Name.text = Cached_Ship_Name;
            }
        }
    }

    public class Recharge_Sum : ScriptableHud
    {
        public static Recharge_Sum Instance { get; private set; }

        public Text Recharge_Sum_Readout;
        public float Time_Spent_Recharging;
        public float Potential_Energy_Recharge;
        public float Energy_Before_Recharge;
        public float Energy_Difference;

        public float Non_Pitlane_Energy_Restore_Value;
        public float Restores_Before_Recharge;
        public float Absorbed_While_In_Pitlane_Restore_Value = 0f;
        public float Previous_Shield_Integrity;
        public float Current_Shield_Integrity;

        public Vector2 Recharge_Sum_Readout_Default_Position;
        public static readonly Vector2 Recharge_Sum_Lower_Middle_Adjust_Vector = new Vector2(0f, -232f);
        public static readonly Vector2 Recharge_Sum_Bottom_Adjust_Vector = new Vector2(0f, -528);

        public string Track_Display_Name;
        public int Current_Section_Index;
        public int Previous_Section_Index;

        public bool Has_Track_Restore;
        public bool Is_Escalation_Project = false;
        public bool Is_In_Escalation_Project_Damage_Zone = false;
        public NgTrackData.Zones.DamageZone Escalation_Project_Damage_Zone;
        public int Track_Restore_Section;
        public float Track_Restore_Amount;
        public Coroutine Recharge_Sum_Routine_Handle;
        public static readonly WaitForSeconds CachedWaitForSeconds = new WaitForSeconds(1.5f);

        public bool Recharge_Sum_Coroutine_Running;
        public bool Recharge_Sum_Waiting;

        public bool Is_Ship_In_Damage_Zone()
        {
            var ships = Escalation_Project_Damage_Zone.TrackedShips;
            int damagezoneShips = ships.Count;

            for (int s = 0; s < damagezoneShips; s++)
            {
                if (ships[s].Target == TargetShip)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Start()
        {
            base.Start();            

            Recharge_Sum_Readout = CustomComponents.GetById<Text>("RechargeSumReadout");
            Recharge_Sum_Readout.text = "";
            //Recharge_Sum_Readout.enabled = false;

            Recharge_Sum_Readout_Default_Position = Recharge_Sum_Readout.rectTransform.anchoredPosition;

            Track_Display_Name = ContentManager.GetCurrentTrack().DisplayName;

            if (Track_Display_Name == "Escalation Project")
            {
                Is_Escalation_Project = true;
                Escalation_Project_Damage_Zone = FindObjectOfType<NgTrackData.Zones.DamageZone>();                
            }

            switch (Track_Display_Name)
            {
                default:
                    Has_Track_Restore = false;
                    break;

                case "(NFS1) Alpine":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 1877;
                    Track_Restore_Amount = 50f;                    
                    break;

                case "(NFS1) Autumn Valley":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 1;
                    Track_Restore_Amount = 20f;                    
                    break;

                case "(NFS1) Transtropolis":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 0;
                    Track_Restore_Amount = 40f;                    
                    break;

                case "(SW1R) Beedo's Wild Ride":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 4;
                    Track_Restore_Amount = 40f;                    
                    break;

                case "(SW1R) Boonta Eve Training Course":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 0;
                    Track_Restore_Amount = 20f;                    
                    break;

                case "(SW1R) Bumpy's Breakers":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 1;
                    Track_Restore_Amount = 60f;                    
                    break;

                case "(SW1R) Fire Mountain Rally":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 0;
                    Track_Restore_Amount = 80f;                    
                    break;

                case "(SW1R) Malastare 100":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 185;
                    Track_Restore_Amount = 20f;                    
                    break;

                case "(SW1R) Scrapper's Run":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 1;
                    Track_Restore_Amount = 25f;                    
                    break;

                case "(SW1R) The Boonta Eve Classic":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 0;
                    Track_Restore_Amount = 80f;                    
                    break;

                case "[GPL] Torino '55":
                    Has_Track_Restore = true;
                    Track_Restore_Section = 0;
                    Track_Restore_Amount = 50f;                    
                    break;
            }

            NgRaceEvents.OnShipAbsorb += AbsorbRechargeSum;
            NgRaceEvents.OnShipUse += EPackRechargeSum;
            if (HudRegister.CurrentHudManager is EliminatorHudManager)
            {
                NgRaceEvents.OnStartLineTriggered += EliminatorRechargeSum;
            }

            switch (VanillaPlusHUDOptions.ModMenuOptions.RechargeSumPosition)
            {
                case 1:
                    Recharge_Sum_Readout.rectTransform.anchoredPosition = Recharge_Sum_Readout_Default_Position + Recharge_Sum_Lower_Middle_Adjust_Vector;
                    break;
                case 2:
                    Recharge_Sum_Readout.rectTransform.anchoredPosition = Recharge_Sum_Readout_Default_Position + Recharge_Sum_Bottom_Adjust_Vector;
                    break;                
            }

            Instance = this;
        }

        public override void OnDestroy()
        {
            NgRaceEvents.OnShipAbsorb -= AbsorbRechargeSum;
            NgRaceEvents.OnShipUse -= EPackRechargeSum;
            NgRaceEvents.OnStartLineTriggered -= EliminatorRechargeSum;

            if (Instance == this)
            {
                Instance = null;
            }

            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();            

            if (Is_Escalation_Project)
            {
                Is_In_Escalation_Project_Damage_Zone = Is_Ship_In_Damage_Zone();
            }

            if (TargetShip.CurrentSection == null)
            {
                return;
            }

            Current_Section_Index = TargetShip.CurrentSection.index;

            Current_Shield_Integrity = TargetShip.ShieldIntegrity;

            if ((Current_Shield_Integrity > Previous_Shield_Integrity) && !Is_In_Escalation_Project_Damage_Zone && (RaceManager.CurrentGamemode is GmUpsurge) && !TargetShip.ShipRestrained)
            {
                Register_Restore(20f);
                Start_Recharge_Sum();
            }

            if ((Current_Shield_Integrity > Previous_Shield_Integrity) && !Is_In_Escalation_Project_Damage_Zone && NgModes.GmSurvivalBackend.Instance != null && !TargetShip.ShipRestrained)
            {
                Register_Restore(GmSurvivalBackend.Instance.ShieldRecovery);
                Start_Recharge_Sum();
            }

            if (Has_Track_Restore == true && Current_Section_Index == Track_Restore_Section && Current_Section_Index != Previous_Section_Index)
            {
                Register_Restore(Track_Restore_Amount);
                Start_Recharge_Sum();
            }

            if (TargetShip.IsRecharging)
            {
                Time_Spent_Recharging += Time.deltaTime;
                Potential_Energy_Recharge = Time_Spent_Recharging * 20f;

                if (!Recharge_Sum_Coroutine_Running || Recharge_Sum_Waiting)
                {
                    Start_Recharge_Sum();
                }
            }
            else if (!Recharge_Sum_Coroutine_Running)
            {
                Energy_Before_Recharge = TargetShip.ShieldIntegrity;
            }

            if (Is_In_Escalation_Project_Damage_Zone)
            {
                if (TargetShip.IsShieldActivated && TargetShip.CurrentPickup is PickupAutopilot)
                {
                    Time_Spent_Recharging += Time.deltaTime * 0.5f;
                }
                else
                {
                    Time_Spent_Recharging += Time.deltaTime;
                }
                Potential_Energy_Recharge = Time_Spent_Recharging * 5f * TargetShip.Settings.DAMAGE_MULT; // -0.5 "damage" applied every 0.1 seconds = 5 energy restored per second

                if (!Recharge_Sum_Coroutine_Running || Recharge_Sum_Waiting)
                {
                    Start_Recharge_Sum();
                }
            }

            Previous_Shield_Integrity = Current_Shield_Integrity;

            Previous_Section_Index = Current_Section_Index;
        }

        public void AbsorbRechargeSum(ShipController ship, float shieldRestored)
        {
            if (ship == TargetShip && (shieldRestored > 0f))
            {
                if (TargetShip.IsRecharging)
                {
                    Absorbed_While_In_Pitlane_Restore_Value += shieldRestored;
                }

                Register_Restore(shieldRestored);
                Start_Recharge_Sum();
            }
        }

        public void EPackRechargeSum(ShipController ship, PickupBase pickup)
        {
            if (ship == TargetShip && pickup is PickupEPack)
            {
                if (TargetShip.IsRecharging)
                {
                    Absorbed_While_In_Pitlane_Restore_Value += 25f;
                }

                Register_Restore(25f);
                Start_Recharge_Sum();
            }
        }

        public void EliminatorRechargeSum(ShipController ship)
        {
            if (ship == TargetShip)
            {                
                Register_Restore(8f);
                Start_Recharge_Sum();
            }
        }

        void Register_Restore(float amount)
        {
            float gain = Mathf.Min(amount, (Mathf.Min((TargetShip.ShieldIntegrity + amount), 100f) - Previous_Shield_Integrity));

            if (!TargetShip.IsRecharging)
            {
                Restores_Before_Recharge += gain;
            }

            if (Recharge_Sum_Coroutine_Running == true)
            {
                Non_Pitlane_Energy_Restore_Value += gain;
            }
            else
            {
                Non_Pitlane_Energy_Restore_Value = gain;
            }
        }

        void Start_Recharge_Sum()
        {
            if (Recharge_Sum_Routine_Handle != null) 
            {
                StopCoroutine(Recharge_Sum_Routine_Handle);
            }
            
            Recharge_Sum_Routine_Handle = StartCoroutine(Recharge_Sum_Duration());
        }

        IEnumerator Recharge_Sum_Duration()
        {
            Recharge_Sum_Coroutine_Running = true;
            Recharge_Sum_Waiting = false;

            if (Non_Pitlane_Energy_Restore_Value >= 0.05f)
            {
                Recharge_Sum_Readout.text = Non_Pitlane_Energy_Restore_Value.ToString("+0.0");
            }
            else
            {
                Recharge_Sum_Readout.text = "";
            }

            while (TargetShip.IsRecharging || Is_In_Escalation_Project_Damage_Zone)
            {
                if (Is_In_Escalation_Project_Damage_Zone)
                {
                    Energy_Difference = Potential_Energy_Recharge;
                }
                else if (Current_Shield_Integrity < 100)
                {
                    Energy_Difference = Current_Shield_Integrity - Energy_Before_Recharge;
                }

                if (Current_Shield_Integrity >= 100 || (TargetShip.IsShieldActivated && !(TargetShip.CurrentPickup is PickupAutopilot) && TargetShip.IsRecharging == false))
                {
                    Recharge_Sum_Readout.text = Mathf.Max(Energy_Difference - Absorbed_While_In_Pitlane_Restore_Value - Restores_Before_Recharge, Potential_Energy_Recharge).ToString("∆0.0");
                }
                else
                {
                    Recharge_Sum_Readout.text = Energy_Difference.ToString("+0.0;-0.0");
                }

                yield return null;
            }

            Recharge_Sum_Waiting = true;

            yield return CachedWaitForSeconds;

            Recharge_Sum_Readout.text = "";
            Time_Spent_Recharging = 0f;
            Energy_Difference = 0f;
            Non_Pitlane_Energy_Restore_Value = 0f;
            Restores_Before_Recharge = 0f;
            Absorbed_While_In_Pitlane_Restore_Value = 0f;
            Energy_Before_Recharge = TargetShip.ShieldIntegrity;
            //Recharge_Sum_Readout.enabled = false;

            Recharge_Sum_Waiting = false;
            Recharge_Sum_Coroutine_Running = false;
        }
    }

    public class Pitlane_Indicator : ScriptableHud
    {
        public Text Pitlane_Indicator_Field_Name;
        public Image Left_Pitlane_Indicator_Arrow_1;
        public Image Left_Pitlane_Indicator_Arrow_2;
        public Image Left_Pitlane_Indicator_Arrow_3;
        public Image Right_Pitlane_Indicator_Arrow_1;
        public Image Right_Pitlane_Indicator_Arrow_2;
        public Image Right_Pitlane_Indicator_Arrow_3;
        
        public int Pitlane_Side;

        public Image[] Left_Pitlane_Arrows;
        public Image[] Right_Pitlane_Arrows;
        public Image[] Pitlane_Arrows;

        public Vector2 Arrow_1_Start_Position;
        public Vector2 Arrow_2_Start_Position;
        public Vector2 Arrow_3_Start_Position;        
        public Vector2 Arrow_Shift_Vector;

        public float Elapsed_Time;
        public const float Pitlane_Indicator_Animation_Duration = 4.35f;
        public const float Fill_Scale = 1.034f / 0.45f;
        public Image Arrow_1;
        public Image Arrow_2;
        public Image Arrow_3;
        public Vector2[] Left_Start_Positions;
        public Vector2[] Right_Start_Positions;
        public Vector2[] Pitlane_Start_Positions;

        public float Pitlane_Indicator_Animation_Start_Time;        

        public Vector2 Left_Pitlane_Indicator_Arrow_1_Default_Position;
        public Vector2 Left_Pitlane_Indicator_Arrow_2_Default_Position;
        public Vector2 Left_Pitlane_Indicator_Arrow_3_Default_Position;
        public Vector2 Right_Pitlane_Indicator_Arrow_1_Default_Position;
        public Vector2 Right_Pitlane_Indicator_Arrow_2_Default_Position;
        public Vector2 Right_Pitlane_Indicator_Arrow_3_Default_Position;
        public Vector2 Pitlane_Indicator_Field_Name_Default_Position;

        public static readonly Vector2 Pitlane_Indicator_Top_Adjust_Vector = new Vector2(0f, 471f);
        public static readonly Vector2 Pitlane_Indicator_Lower_Middle_Adjust_Vector = new Vector2(0f, -293f);
        public static readonly Vector2 Pitlane_Indicator_Bottom_Adjust_Vector = new Vector2(0f, -528f);


        IEnumerator Pitlane_Indicator_Animation()
        {
            Elapsed_Time = 0f;
            Pitlane_Indicator_Animation_Start_Time = Time.time;

            Arrow_1 = Pitlane_Arrows[0];
            Arrow_2 = Pitlane_Arrows[1];
            Arrow_3 = Pitlane_Arrows[2];

            Arrow_1_Start_Position = Pitlane_Start_Positions[0];
            Arrow_2_Start_Position = Pitlane_Start_Positions[1];
            Arrow_3_Start_Position = Pitlane_Start_Positions[2];

            Arrow_Shift_Vector = new Vector2(21f * Pitlane_Side, 0);            

            while (Elapsed_Time <= Pitlane_Indicator_Animation_Duration)
            {
                //FIRST ARROW
                if (0f < Elapsed_Time && Elapsed_Time <= 0.15f) //Blink first arrow + pit text on
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_1.enabled = true;
                    Arrow_1.fillAmount = Elapsed_Time * Fill_Scale;
                }
                if (0.15f < Elapsed_Time && Elapsed_Time <= 0.3f) //Blink first arrow + pit text off
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_1.enabled = false;
                    Arrow_1.fillAmount = Elapsed_Time * Fill_Scale;
                }
                if (0.3f < Elapsed_Time && Elapsed_Time <= 0.45f) //Blink first arrow + pit text back on, first arrow should be finished filling after this completes
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_1.enabled = true;
                    Arrow_1.fillAmount = Elapsed_Time * Fill_Scale;
                }

                //SECOND ARROW
                if (0.45f < Elapsed_Time && Elapsed_Time <= 0.6f) //Blink second arrow on
                {
                    Arrow_2.enabled = true;
                    Arrow_2.fillAmount = (Elapsed_Time - 0.45f) * Fill_Scale;
                }
                if (0.6f < Elapsed_Time && Elapsed_Time <= 0.75f) //Blink second arrow off
                {
                    Arrow_2.enabled = false;
                    Arrow_2.fillAmount = (Elapsed_Time - 0.45f) * Fill_Scale;
                }
                if (0.75f < Elapsed_Time && Elapsed_Time <= 0.9f) //Blink second arrow back on, second arrow should be finished filling after this completes
                {
                    Arrow_2.enabled = true;
                    Arrow_2.fillAmount = (Elapsed_Time - 0.45f) * Fill_Scale;
                }

                //THIRD ARROW
                if (0.9f < Elapsed_Time && Elapsed_Time <= 1.05f) //Blink third arrow on
                {
                    Arrow_3.enabled = true;
                    Arrow_3.fillAmount = (Elapsed_Time - 0.9f) * Fill_Scale;
                }
                if (1.05f < Elapsed_Time && Elapsed_Time <= 1.2f) //Blink third arrow off
                {
                    Arrow_3.enabled = false;
                    Arrow_3.fillAmount = (Elapsed_Time - 0.9f) * Fill_Scale;
                }
                if (1.2f < Elapsed_Time && Elapsed_Time <= 1.35f) //Blink third arrow back on, third arrow should be finished filling after this completes
                {
                    Arrow_3.enabled = true;
                    Arrow_3.fillAmount = (Elapsed_Time - 0.9f) * Fill_Scale;
                }

                //INTERMISSION
                if (2.85f < Elapsed_Time && Elapsed_Time <= 3f)
                {
                    Arrow_1.enabled = false;
                }

                //End of the animation, FIRST ARROW
                if (3f < Elapsed_Time && Elapsed_Time <= 3.15f)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_1.enabled = true;
                    Arrow_1.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3f) / 0.45f));
                    Arrow_1.fillAmount = 1f - ((Elapsed_Time - 3f) / 0.45f);
                }
                if (3.15f < Elapsed_Time && Elapsed_Time <= 3.3f)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_1.enabled = false;
                    Arrow_1.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3f) / 0.45f));
                }
                if (3.3f < Elapsed_Time && Elapsed_Time <= 3.45f)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_1.enabled = true;
                    Arrow_2.enabled = false;
                    Arrow_1.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3f) / 0.45f));
                    Arrow_1.fillAmount = 1f - ((Elapsed_Time - 3f) / 0.45f);
                }
                //End of the animation, SECOND ARROW
                if (3.45f < Elapsed_Time && Elapsed_Time <= 3.6f)
                {
                    Arrow_1.fillAmount = 0f;
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_2.enabled = true;
                    Arrow_2.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.45f) / 0.45f));
                    Arrow_2.fillAmount = 1f - ((Elapsed_Time - 3.45f) / 0.45f);
                }
                if (3.6f < Elapsed_Time && Elapsed_Time <= 3.75f)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_2.enabled = false;
                    Arrow_2.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.45f) / 0.45f));
                }
                if (3.75f < Elapsed_Time && Elapsed_Time <= 3.9f)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_2.enabled = true;
                    Arrow_3.enabled = false;
                    Arrow_2.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.45f) / 0.45f));
                    Arrow_2.fillAmount = 1f - ((Elapsed_Time - 3.45f) / 0.45f);
                }
                //End of the animation, THIRD ARROW
                if (3.9f < Elapsed_Time && Elapsed_Time <= 4.05f)
                {
                    Arrow_2.fillAmount = 0f;
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_3.enabled = true;
                    Arrow_3.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.9f) / 0.45f));
                    Arrow_3.fillAmount = 1f - ((Elapsed_Time - 3.9f) / 0.45f);
                }
                if (4.05f < Elapsed_Time && Elapsed_Time <= 4.2f)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Arrow_3.enabled = false;
                    Arrow_3.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.9f) / 0.45f));
                }
                if (4.2f < Elapsed_Time && Elapsed_Time <= 4.35f)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Arrow_3.enabled = true;
                    Arrow_3.rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Elapsed_Time - 3.9f) / 0.45f));
                    Arrow_3.fillAmount = 1f - ((Elapsed_Time - 3.9f) / 0.45f);
                }

                yield return null;
                Elapsed_Time = Time.time - Pitlane_Indicator_Animation_Start_Time;
            }
            Arrow_1.enabled = false;
            Arrow_2.enabled = false;
            Arrow_3.enabled = false;
            Pitlane_Indicator_Field_Name.enabled = false;

            Arrow_1.rectTransform.anchoredPosition = Arrow_1_Start_Position;
            Arrow_2.rectTransform.anchoredPosition = Arrow_2_Start_Position;
            Arrow_3.rectTransform.anchoredPosition = Arrow_3_Start_Position;
        }

        public override void Start()
        {
            base.Start();

            NgTrackData.Triggers.PitlaneIndicator.OnPitlaneIndicatorTriggered += Pitlane_Indicator_Method;

            Pitlane_Indicator_Field_Name = CustomComponents.GetById<Text>("Pitlane Indicator Field Name");
            Pitlane_Indicator_Field_Name.enabled = false;

            Left_Pitlane_Indicator_Arrow_1 = CustomComponents.GetById<Image>("LeftPitlaneIndicatorArrow1");
            Left_Pitlane_Indicator_Arrow_2 = CustomComponents.GetById<Image>("LeftPitlaneIndicatorArrow2");
            Left_Pitlane_Indicator_Arrow_3 = CustomComponents.GetById<Image>("LeftPitlaneIndicatorArrow3");
            Left_Pitlane_Indicator_Arrow_1.enabled = false;
            Left_Pitlane_Indicator_Arrow_2.enabled = false;
            Left_Pitlane_Indicator_Arrow_3.enabled = false;
            Left_Pitlane_Indicator_Arrow_1.fillAmount = 0f;
            Left_Pitlane_Indicator_Arrow_2.fillAmount = 0f;
            Left_Pitlane_Indicator_Arrow_3.fillAmount = 0f;

            Right_Pitlane_Indicator_Arrow_1 = CustomComponents.GetById<Image>("RightPitlaneIndicatorArrow1");
            Right_Pitlane_Indicator_Arrow_2 = CustomComponents.GetById<Image>("RightPitlaneIndicatorArrow2");
            Right_Pitlane_Indicator_Arrow_3 = CustomComponents.GetById<Image>("RightPitlaneIndicatorArrow3");
            Right_Pitlane_Indicator_Arrow_1.enabled = false;
            Right_Pitlane_Indicator_Arrow_2.enabled = false;
            Right_Pitlane_Indicator_Arrow_3.enabled = false;
            Right_Pitlane_Indicator_Arrow_1.fillAmount = 0f;
            Right_Pitlane_Indicator_Arrow_2.fillAmount = 0f;
            Right_Pitlane_Indicator_Arrow_3.fillAmount = 0f;

            Left_Pitlane_Indicator_Arrow_1_Default_Position = Left_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition;
            Left_Pitlane_Indicator_Arrow_2_Default_Position = Left_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition;
            Left_Pitlane_Indicator_Arrow_3_Default_Position = Left_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition;
            Right_Pitlane_Indicator_Arrow_1_Default_Position = Right_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition;
            Right_Pitlane_Indicator_Arrow_2_Default_Position = Right_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition;
            Right_Pitlane_Indicator_Arrow_3_Default_Position = Right_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition;
            Pitlane_Indicator_Field_Name_Default_Position = Pitlane_Indicator_Field_Name.rectTransform.anchoredPosition;

            switch (VanillaPlusHUDOptions.ModMenuOptions.PitlaneIndicatorPosition)
            {
                case 1:
                    Pitlane_Indicator_Field_Name.rectTransform.anchoredPosition = Pitlane_Indicator_Field_Name_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Top_Adjust_Vector;
                    break;
                case 2:
                    Pitlane_Indicator_Field_Name.rectTransform.anchoredPosition = Pitlane_Indicator_Field_Name_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Lower_Middle_Adjust_Vector;
                    break;
                case 3:
                    Pitlane_Indicator_Field_Name.rectTransform.anchoredPosition = Pitlane_Indicator_Field_Name_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Left_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Left_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_1_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_2_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    Right_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition = Right_Pitlane_Indicator_Arrow_3_Default_Position + Pitlane_Indicator_Bottom_Adjust_Vector;
                    break;
            }

            Left_Pitlane_Arrows = new Image[] { Left_Pitlane_Indicator_Arrow_1, Left_Pitlane_Indicator_Arrow_2, Left_Pitlane_Indicator_Arrow_3 };
            Right_Pitlane_Arrows = new Image[] { Right_Pitlane_Indicator_Arrow_1, Right_Pitlane_Indicator_Arrow_2, Right_Pitlane_Indicator_Arrow_3 };
            Left_Start_Positions = new Vector2[] { Left_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition, Left_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition, Left_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition };
            Right_Start_Positions = new Vector2[] { Right_Pitlane_Indicator_Arrow_1.rectTransform.anchoredPosition, Right_Pitlane_Indicator_Arrow_2.rectTransform.anchoredPosition, Right_Pitlane_Indicator_Arrow_3.rectTransform.anchoredPosition };
        }

        public void Pitlane_Indicator_Method(ShipController ship, int side)
        {
            if (ship != TargetShip)
            {
                return;
            }

            if (NgSettings.Gameplay.MirrorEnabled)
            {
                Pitlane_Side = -side;
            }
            else
            {
                Pitlane_Side = side;
            }

            if (Pitlane_Side == 1)
            {
                Pitlane_Arrows = Right_Pitlane_Arrows;
                Pitlane_Start_Positions = Right_Start_Positions;
            }
            else
            {
                Pitlane_Arrows = Left_Pitlane_Arrows;
                Pitlane_Start_Positions = Left_Start_Positions;
            }

            StartCoroutine(Pitlane_Indicator_Animation());
        }

        public override void OnDestroy()
        {
            NgTrackData.Triggers.PitlaneIndicator.OnPitlaneIndicatorTriggered -= Pitlane_Indicator_Method;

            base.OnDestroy();
        }
    }

    public class Relative_Time_Display : ScriptableHud
    {
        public Text Relative_Time_Readout;
        public static readonly Vector4 Race_Leader_Green = new Vector4(181f / 255f, 1f, 29f / 255f, 1f);
        public static readonly Vector4 Contender_Red = new Vector4(1f, 28f / 255f, 36f / 255f, 1f);
        public static readonly Vector4 Tremor_Safety_Blue = new Vector4(0f, (163f / 255f), (233f / 255f), 1f);
        public int Player_Section_Index;
        public int Second_Place_Index;
        public int Track_Section_Max;
        public int Player_Lap_Adjust;
        public int Second_Place_Lap_Adjust;
        public int Distance_In_Sections;
        public string Relative_Time_Readout_Units;
        public int Player_Current_Section_Index;
        public int Player_Previous_Section_Index;
        public int Second_Place_Current_Section_Index;
        public ShipController Second_Place_Ship;        

        public override void Start()
        {
            base.Start();            

            Relative_Time_Readout = CustomComponents.GetById<Text>("RelativeTimeReadout");
            Relative_Time_Readout.text = "–:––.––";
            Track_Section_Max = NgTrackData.TrackManager.Instance.data.sections.Count; //How many sections the current track has
        }

        public override void Update()
        {
            base.Update();

            if (TargetShip.CurrentSection == null)
            {
                return;
            }

            Player_Current_Section_Index = TargetShip.CurrentSection.index;

            if (Player_Current_Section_Index == Player_Previous_Section_Index)
            {
                return;
            }

            Second_Place_Ship = Ships.FindShipInPlace(2);
            
            if (Second_Place_Ship == null)
            {
                return;
            }

            Second_Place_Current_Section_Index = Second_Place_Ship.CurrentSection.index;

            if (Player_Current_Section_Index >= 2)
            {
                Player_Section_Index = Player_Current_Section_Index; //Section the player ship is currently on
            }
            else
            {
                Player_Section_Index = Player_Current_Section_Index + Track_Section_Max; //Sections 0 and 1 on most tracks are entered before or as the next lap is being set
            }

            if (Second_Place_Current_Section_Index >= 2)
            {
                Second_Place_Index = Second_Place_Current_Section_Index; //Section the second place ship is currently on
            }
            else
            {
                Second_Place_Index = Second_Place_Current_Section_Index + Track_Section_Max; //Sections 0 and 1 on most tracks are entered before or as the next lap is being set
            }

            //Player_Section_Index = TargetShip.CurrentSection.index; //Section the player ship is currently on
            //Second_Place_Index = Ships.FindShipInPlace(2).CurrentSection.index; //Section the second place ship is currently on

            Player_Lap_Adjust = (TargetShip.CurrentLap - 1); //Multiply Track_Section_Max by this value
            Second_Place_Lap_Adjust = (Second_Place_Ship.CurrentLap - 1); //Multiply Track_Section_Max by this value

            Player_Section_Index += Player_Lap_Adjust * Track_Section_Max;
            Second_Place_Index += Second_Place_Lap_Adjust * Track_Section_Max;

            Distance_In_Sections = Player_Section_Index - Second_Place_Index;

            TargetShip.CalculateRelativeTime();
            Relative_Time_Readout_Units = FloatToTime.Convert(Mathf.Abs(TargetShip.lastRelativeTime), "0:00.00"); //Time readout

            if (TargetShip.lastRelativeTime < 0)
            {
                Relative_Time_Readout.color = Contender_Red;
                Relative_Time_Readout.text = "+" + Relative_Time_Readout_Units;
            }
            //else if (Ships.SectionOffsetBetweenSigned(TargetShip, Ships.FindShipInPlace(2)) > 120)//(TargetShip.CurrentSection.index - Ships.FindShipInPlace(2).CurrentSection.index > 120) //Ahead by 120 sections
            else if (Distance_In_Sections > 120)
            {
                Relative_Time_Readout.color = Tremor_Safety_Blue;
                Relative_Time_Readout.text = "-" + Relative_Time_Readout_Units;
            }
            //else if (TargetShip.lastRelativeTime > 0 && (Ships.SectionOffsetBetweenSigned(TargetShip, Ships.FindShipInPlace(2)) < 120))
            else if (TargetShip.lastRelativeTime > 0 && Distance_In_Sections <= 120)
            {
                Relative_Time_Readout.color = Race_Leader_Green;
                Relative_Time_Readout.text = "-" + Relative_Time_Readout_Units;
            }
            else
            {
                Relative_Time_Readout.color = Color.white;
                Relative_Time_Readout.text = "0:00.00";
            }
            //Relative_Time_Readout_Units = FloatToTime.Convert(Mathf.Abs(TargetShip.lastRelativeTime), "0:00.00");
            //Relative_Time_Readout.text = FloatToTime.Convert(Mathf.Abs(TargetShip.lastRelativeTime), "0:00.00");
            //Relative_Time_Readout.text = TargetShip.lastRelativeTime.ToString();

            Player_Previous_Section_Index = Player_Current_Section_Index;
        }
    }

    public class Music_Display : ScriptableHud
    {
        public Text Music_Display_Readout;

        public Vector2 Music_Display_Readout_Default_Position;
        public static readonly Vector2 Music_Display_Readout_Adjust_Vector = new Vector2(0f, 37f);
        
        public Coroutine MusicFlasherCoroutine;

        public float Start_Time;
        public float Elapsed_Time;
        public int Current_Step;
        public int Previous_Step;

        IEnumerator Music_Flasher()
        {
            Start_Time = Time.time;
            Elapsed_Time = 0f;
            Previous_Step = -1;

            while (Elapsed_Time < 2.5f)
            {
                Current_Step = (int)(Elapsed_Time * 8f);                

                if (Current_Step != Previous_Step)
                {
                    Previous_Step = Current_Step;

                    //START
                    if (Current_Step < 6)
                    {
                        Music_Display_Readout.enabled = (Current_Step % 2 == 0);
                    }
                    //Stable for 1 second
                    else if (Current_Step < 14)
                    {
                        Music_Display_Readout.enabled = true;
                    }
                    //END
                    else
                    {
                        Music_Display_Readout.enabled = (Current_Step % 2 == 1);
                    }
                }
                
                yield return null;
                Elapsed_Time = Time.time - Start_Time;
            }
            Music_Display_Readout.enabled = false;

            MusicFlasherCoroutine = null;
        }

        public override void Start()
        {
            base.Start();
            NgUiEvents.OnNewSongPlaying += Music_Display_Method;
            Music_Display_Readout = CustomComponents.GetById<Text>("MusicReadout");
            Music_Display_Readout_Default_Position = Music_Display_Readout.rectTransform.anchoredPosition;
            Music_Display_Readout.text = "";
        }

        public void Music_Display_Method(string name)
        {
            Music_Display_Readout.text = name;
            Music_Display_Readout.cachedTextGenerator.Populate(Music_Display_Readout.text, Music_Display_Readout.GetGenerationSettings(Music_Display_Readout.rectTransform.rect.size));
            Music_Display_Readout.rectTransform.anchoredPosition = Music_Display_Readout_Default_Position + ((Music_Display_Readout.cachedTextGenerator.lineCount - 1) * Music_Display_Readout_Adjust_Vector);

            if (MusicFlasherCoroutine != null)
            {
                StopCoroutine(MusicFlasherCoroutine);
                MusicFlasherCoroutine = null;
            }

            MusicFlasherCoroutine = StartCoroutine(Music_Flasher());
        }

        public override void OnDestroy()
        {
            NgUiEvents.OnNewSongPlaying -= Music_Display_Method;

            base.OnDestroy();
        }
    }

    public class Damage_Flasher : ScriptableHud
    {
        public float Current_Shield_Integrity;
        public float Previous_Shield_Integrity;
        //public bool Hit_By_Weapon;
        //public bool Damaged_In_Autopilot;
        public Image Damage_Flasher_Image;
        public Image Damage_Flasher_Background;
        public Text Damage_Flasher_Text;
        public const float Autopilot_Flash_Duration = 0.5f;
        public float Autopilot_Flash_End_Time;

        public float Current_Time;
        public ShipSim Cached_PysSim;

        public override void Start()
        {
            base.Start();

            Damage_Flasher_Image = CustomComponents.GetById<Image>("DamageFlasher");
            Damage_Flasher_Background = CustomComponents.GetById<Image>("DamageFlasherBackground");
            Damage_Flasher_Text = CustomComponents.GetById<Text>("DamageFlasherText");
            Damage_Flasher_Image.enabled = false;
            Damage_Flasher_Background.enabled = false;
            Damage_Flasher_Text.enabled = false;

            Cached_PysSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();

            Current_Shield_Integrity = TargetShip.ShieldIntegrity;            
            Current_Time = Time.time;

            if ((Current_Shield_Integrity < Previous_Shield_Integrity) && TargetShip.IsShieldActivated && TargetShip.LastAttacker != null)
            {
                //Damage_Flasher_Image.enabled = true;
                //StartCoroutine(Damage_Flash_Autopilot_Duration());
                Autopilot_Flash_End_Time = Current_Time + Autopilot_Flash_Duration;
            }
            //else if (((TargetShip.PysSim.enginePower < 1f) || (TargetShip.PysSim.engineAccel < 1f)) && TargetShip.CurrentLap != 0)
            //{
            //    //Damage_Flasher_Image.enabled = true;
            //    StartCoroutine(Damage_Flash_Duration());
            //}
            //else
            //{
                //Damage_Flasher_Image.enabled = false;
            //}

            if (Current_Time < Autopilot_Flash_End_Time || (((Cached_PysSim.enginePower < 1f) || (Cached_PysSim.engineAccel < 1f)) && TargetShip.CurrentLap != 0))
            {
                //Damage_Flasher_Image.enabled = true;
                //Damage_Flasher_Background.enabled = true;
                Damage_Flasher_Text.enabled = true;
            }
            else
            {
                //Damage_Flasher_Image.enabled = false;
                //Damage_Flasher_Background.enabled = false;
                Damage_Flasher_Text.enabled = false;
            }

            //if (Damage_Flasher_Image.enabled != true)
            //{
            //    Damage_Flasher_Background.enabled = false;
            //}
            //else
            //{
            //    Damage_Flasher_Image.enabled = true;
            //}

            Previous_Shield_Integrity = Current_Shield_Integrity;
        }
    }

    public class Hyperthrust_Bar : ScriptableHud
    {
        public Text Hyperthrust_Bar_Numeric_Readout;
        public Image Hyperthrust_Bar_Background;
        public Image Hyperthrust_Bar_Whiteout;
        public Image Hyperthrust_Bar_Image;
        public string Hyperthrust_Units_String;

        public static readonly Vector2 Hyperthrust_Bar_Lowered_Adjust_Vector = new Vector2(-302f, -500f);
        public static readonly Vector2 Hyperthrust_Bar_Centered_Adjust_Vector = new Vector2(-302f, -367f);
        public static readonly Vector2 Hyperthrust_Bar_Lowered_Wide_Adjust_Vector = new Vector2(0f, -500f);
        public static readonly Vector2 Hyperthrust_Bar_Centered_Wide_Adjust_Vector = new Vector2(0f, -367f);

        public Vector2 HyperThrust_Bar_Numeric_Readout_Default_Position;
        public Vector2 Hyperthrust_Bar_Background_Default_Position;
        public Vector2 Hyperthrust_Bar_Whiteout_Default_Position;
        public Vector2 Hyperthrust_Bar_Image_Default_Position;

        public Vector2 Shared_Offset;

        public float Current_Hyperthrust_Amount;
        public float Previous_Hyperthrust_Amount;

        public ShipSim Cached_ShipSim;

        public override void Start()
        {
            base.Start();

            Hyperthrust_Bar_Numeric_Readout = CustomComponents.GetById<Text>("Hyperthrust Bar Numeric Readout");
            Hyperthrust_Bar_Background = CustomComponents.GetById<Image>("HyperthrustBarBackground");
            Hyperthrust_Bar_Whiteout = CustomComponents.GetById<Image>("HyperthrustBarWhiteout");
            Hyperthrust_Bar_Image = CustomComponents.GetById<Image>("HyperthrustBar");

            HyperThrust_Bar_Numeric_Readout_Default_Position = Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition;
            Hyperthrust_Bar_Background_Default_Position = Hyperthrust_Bar_Background.rectTransform.anchoredPosition;
            Hyperthrust_Bar_Whiteout_Default_Position = Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition;
            Hyperthrust_Bar_Image_Default_Position = Hyperthrust_Bar_Image.rectTransform.anchoredPosition;

            switch (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarPosition)
            {
                case 1:
                    Shared_Offset = Hyperthrust_Bar_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Shared_Offset = Hyperthrust_Bar_Centered_Adjust_Vector;
                    break;
                case 3:
                    Shared_Offset = Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                    break;
                case 4:
                    Shared_Offset = Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                    break;
            }

            Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Shared_Offset;
            Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Shared_Offset;
            Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Shared_Offset;
            Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Shared_Offset;

            Hyperthrust_Bar_Image.fillAmount = 0f;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();

            Current_Hyperthrust_Amount = Cached_ShipSim.engineHyper;

            if (Current_Hyperthrust_Amount != Previous_Hyperthrust_Amount)
            {
                Hyperthrust_Bar_Image.fillAmount = Current_Hyperthrust_Amount / 1.85f;

                if (Current_Hyperthrust_Amount < 0.01f)
                {
                    Hyperthrust_Units_String = "";
                }
                else
                {
                    Hyperthrust_Units_String = Current_Hyperthrust_Amount.ToString("0.00");
                }                
            }

            if (Current_Hyperthrust_Amount < 0.01f && VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarVisibility == 1)
            {
                Hyperthrust_Bar_Numeric_Readout.enabled = false; //Formerly, Hyperthrust_Bar_Numeric_Readout.text = "";
                Hyperthrust_Bar_Background.enabled = false;
                Hyperthrust_Bar_Whiteout.enabled = false;
                Hyperthrust_Bar_Image.enabled = false;
            }
            else
            {
                Hyperthrust_Bar_Background.enabled = true;
                Hyperthrust_Bar_Whiteout.enabled = true;
                Hyperthrust_Bar_Image.enabled = true;
                Hyperthrust_Bar_Numeric_Readout.enabled = VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarTextToggle;

                Hyperthrust_Bar_Numeric_Readout.text = Hyperthrust_Units_String;
            }

            Previous_Hyperthrust_Amount = Current_Hyperthrust_Amount;
        }
    }

    public class Speedpad_Counter : ScriptableHud
    {
        public Text Speedpad_Count_Numeric_Readout;
        public Image Speedpad_Counter_Background;
        public Image Speedpad_Counter_Whiteout;
        public Image Speedpad_Counter_Image;
                
        public float Speedpad_Counter_Current_Speedpad_Time_2280;
        public float Speedpad_Counter_Previous_Speedpad_Time_2280;
        public float Speedpad_Counter_Current_Boost_Acceleration;
        public float Speedpad_Counter_Previous_Boost_Acceleration;
        public float Speedpad_Counter_2280;
        public string Speedpad_Counter_Units_String;
        public bool Zero_Out_Speedpad_Counter;
        public bool Is_2280_Physics;

        public static readonly Vector2 Speedpad_Counter_Lowered_Adjust_Vector = new Vector2(287, -502);
        public static readonly Vector2 Speedpad_Counter_Centered_Adjust_Vector = new Vector2(287, -371);
        public static readonly Vector2 Speedpad_Counter_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
        public static readonly Vector2 Speedpad_Counter_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

        public Vector2 Speedpad_Count_Numeric_Readout_Default_Position;
        public Vector2 Speedpad_Counter_Background_Default_Position;
        public Vector2 Speedpad_Counter_Whiteout_Default_Position;
        public Vector2 Speedpad_Counter_Image_Default_Position;

        public Vector2 Shared_Offset;

        public ShipSim Cached_ShipSim;

        public override void Start()
        {
            base.Start();

            Speedpad_Count_Numeric_Readout = CustomComponents.GetById<Text>("Speedpad Count Numeric Readout");
            Speedpad_Counter_Background = CustomComponents.GetById<Image>("SpeedpadCounterBackground");
            Speedpad_Counter_Whiteout = CustomComponents.GetById<Image>("SpeedpadCounterWhiteout");
            Speedpad_Counter_Image = CustomComponents.GetById<Image>("SpeedpadCounter");

            Speedpad_Count_Numeric_Readout_Default_Position = Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition;
            Speedpad_Counter_Background_Default_Position = Speedpad_Counter_Background.rectTransform.anchoredPosition;
            Speedpad_Counter_Whiteout_Default_Position = Speedpad_Counter_Whiteout.rectTransform.anchoredPosition;
            Speedpad_Counter_Image_Default_Position = Speedpad_Counter_Image.rectTransform.anchoredPosition;

            if (Cheats.IntFromPhysicsMod() == 1) //Formerly if (Cheats.ModernPhysics)
            {
                Is_2280_Physics = true;
            }

            switch (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition)
            {
                case 1:
                    Shared_Offset = Speedpad_Counter_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Shared_Offset = Speedpad_Counter_Centered_Adjust_Vector;                    
                    break;
                case 3:
                    Shared_Offset = Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                    break;
                case 4:
                    Shared_Offset = Speedpad_Counter_Centered_Wide_Adjust_Vector;                    
                    break;
            }

            Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Shared_Offset;
            Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Shared_Offset;
            Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Shared_Offset;
            Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Shared_Offset;

            Speedpad_Counter_Image.fillAmount = 0f;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();

            if (Is_2280_Physics == true) //Formerly if (Cheats.ModernPhysics)
            {
                Speedpad_Counter_Current_Speedpad_Time_2280 = Cached_ShipSim.modernPadPushTimer;

                if (Speedpad_Counter_Current_Speedpad_Time_2280 > Speedpad_Counter_Previous_Speedpad_Time_2280) // && (Speedpad_Counter_2280 < 3))
                {
                    Zero_Out_Speedpad_Counter = false;
                    Speedpad_Counter_2280 += 1f;
                    Speedpad_Counter_Units_String = Speedpad_Counter_2280.ToString("+0");
                    Speedpad_Counter_Image.fillAmount = Speedpad_Counter_2280 / 3f;
                }

                if (Speedpad_Counter_Current_Speedpad_Time_2280 <= 0f)
                {
                    Zero_Out_Speedpad_Counter = true;
                    Speedpad_Counter_2280 = 0f;                                        
                    Speedpad_Counter_Units_String = "";
                    Speedpad_Counter_Image.fillAmount = 0f;
                }                

                Speedpad_Counter_Previous_Speedpad_Time_2280 = Speedpad_Counter_Current_Speedpad_Time_2280;
            }
            else
            {
                Speedpad_Counter_Current_Boost_Acceleration = TargetShip.BoostAcceleration;

                if (Speedpad_Counter_Current_Boost_Acceleration > Speedpad_Counter_Previous_Boost_Acceleration)
                {
                    Zero_Out_Speedpad_Counter = false;
                    Speedpad_Counter_Units_String = Speedpad_Counter_Current_Boost_Acceleration.ToString("+0");
                }

                if (Speedpad_Counter_Current_Boost_Acceleration == 0f)
                {
                    Zero_Out_Speedpad_Counter = true;
                    Speedpad_Counter_Units_String = "";
                }

                if (Speedpad_Counter_Current_Boost_Acceleration != Speedpad_Counter_Previous_Boost_Acceleration)
                {
                    Speedpad_Counter_Image.fillAmount = Speedpad_Counter_Current_Boost_Acceleration / 12f;
                }

                Speedpad_Counter_Previous_Boost_Acceleration = Speedpad_Counter_Current_Boost_Acceleration;
            }

            if (Zero_Out_Speedpad_Counter == true && VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterVisibility == 1)
            {
                Speedpad_Count_Numeric_Readout.enabled = false; //Formerly, Speedpad_Count_Numeric_Readout.text = "";
                Speedpad_Counter_Background.enabled = false;
                Speedpad_Counter_Whiteout.enabled = false;
                Speedpad_Counter_Image.enabled = false;
            }
            else
            {
                Speedpad_Counter_Background.enabled = true;
                Speedpad_Counter_Whiteout.enabled = true;
                Speedpad_Counter_Image.enabled = true;
                Speedpad_Count_Numeric_Readout.enabled = VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterTextToggle;
            }

            Speedpad_Count_Numeric_Readout.text = Speedpad_Counter_Units_String;

        }

    }

    public class Speedpad_Timer : ScriptableHud
    {
        public Text Speedpad_Timer_Numeric_Readout;
        public Image Speedpad_Timer_Background;
        public Image Speedpad_Timer_Whiteout;
        public Image Speedpad_Timer_Image;
        public string Speedpad_Timer_Units_String;

        public float Speedpad_Timer_Current_Speedpad_Time;
        public float Speedpad_Timer_Previous_Speedpad_Time;
        public float Speedpad_Timer_Max;
        public bool Zero_Out_Speedpad_Timer;
        public bool Is_2280_Physics;

        public Vector2 Speedpad_Timer_Numeric_Readout_Default_Position;
        public Vector2 Speedpad_Timer_Background_Default_Position;
        public Vector2 Speedpad_Timer_Whiteout_Default_Position;
        public Vector2 Speedpad_Timer_Image_Default_Position;

        public static readonly Vector2 Speedpad_Timer_Lowered_Adjust_Vector = new Vector2(287, -502);
        public static readonly Vector2 Speedpad_Timer_Centered_Adjust_Vector = new Vector2(287, -371);
        public static readonly Vector2 Speedpad_Timer_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
        public static readonly Vector2 Speedpad_Timer_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

        public Vector2 Shared_Offset;

        public ShipSim Cached_ShipSim;

        public override void Start()
        {
            base.Start();

            Speedpad_Timer_Numeric_Readout = CustomComponents.GetById<Text>("Speedpad Timer Numeric Readout");
            Speedpad_Timer_Background = CustomComponents.GetById<Image>("SpeedpadTimerBackground");
            Speedpad_Timer_Whiteout = CustomComponents.GetById<Image>("SpeedpadTimerWhiteout");
            Speedpad_Timer_Image = CustomComponents.GetById<Image>("SpeedpadTimer");

            Speedpad_Timer_Numeric_Readout_Default_Position = Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition;
            Speedpad_Timer_Background_Default_Position = Speedpad_Timer_Background.rectTransform.anchoredPosition;
            Speedpad_Timer_Whiteout_Default_Position = Speedpad_Timer_Whiteout.rectTransform.anchoredPosition;
            Speedpad_Timer_Image_Default_Position = Speedpad_Timer_Image.rectTransform.anchoredPosition;

            if (Cheats.IntFromPhysicsMod() == 1) //Formerly if (Cheats.ModernPhysics)
            {
                Is_2280_Physics = true;
                Speedpad_Timer_Max = 0.24f;
            }
            else
            {
                Is_2280_Physics = false;
                Speedpad_Timer_Max = 4.5f; //First 3 speedpads hit grant 1.5 seconds each, 4th and beyond grant 0.5 seconds each. Formerly this value was 5f
            }

            switch (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition)
            {
                case 1:
                    Shared_Offset = Speedpad_Timer_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Shared_Offset = Speedpad_Timer_Centered_Adjust_Vector;
                    break;
                case 3:
                    Shared_Offset = Speedpad_Timer_Lowered_Wide_Adjust_Vector;                    
                    break;
                case 4:
                    Shared_Offset = Speedpad_Timer_Centered_Wide_Adjust_Vector;                    
                    break;
            }

            Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Shared_Offset;
            Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Shared_Offset;
            Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Shared_Offset;
            Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Shared_Offset;

            Speedpad_Timer_Image.fillAmount = 0f;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();

            if (Is_2280_Physics)
            {
                Speedpad_Timer_Current_Speedpad_Time = Cached_ShipSim.modernPadPushTimer;
            }
            else
            {
                Speedpad_Timer_Current_Speedpad_Time = TargetShip.BoostTimer;
            }

            if (Speedpad_Timer_Current_Speedpad_Time != Speedpad_Timer_Previous_Speedpad_Time)
            {
                Speedpad_Timer_Image.fillAmount = Speedpad_Timer_Current_Speedpad_Time / Speedpad_Timer_Max;
            }

            if (Speedpad_Timer_Current_Speedpad_Time < 0.01f)
            {
                Zero_Out_Speedpad_Timer = true;
                Speedpad_Timer_Units_String = "";
            }
            else
            {
                Zero_Out_Speedpad_Timer = false;
                if (Speedpad_Timer_Current_Speedpad_Time != Speedpad_Timer_Previous_Speedpad_Time)
                {                    
                    Speedpad_Timer_Units_String = Speedpad_Timer_Current_Speedpad_Time.ToString("0.00");
                }
            }

            if (Zero_Out_Speedpad_Timer == true && VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerVisibility == 1)
            {
                Speedpad_Timer_Numeric_Readout.enabled = false; //Formerly, Speedpad_Timer_Units_String = "";
                Speedpad_Timer_Background.enabled = false;
                Speedpad_Timer_Whiteout.enabled = false;
                Speedpad_Timer_Image.enabled = false;
            }
            else
            {
                Speedpad_Timer_Background.enabled = true;
                Speedpad_Timer_Whiteout.enabled = true;
                Speedpad_Timer_Image.enabled = true;
                Speedpad_Timer_Numeric_Readout.enabled = VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerTextToggle;

                Speedpad_Timer_Numeric_Readout.text = Speedpad_Timer_Units_String;
            }

            Speedpad_Timer_Previous_Speedpad_Time = Speedpad_Timer_Current_Speedpad_Time;
        }
    }

    public class Lap_Counter : ScriptableHud
    {
        public Text Lap_Counter_Field_Name;
        public Text Current_Lap;
        public Text Max_Lap;
        public Image Lap_Counter_Background;
        public Image Lap_Counter_Image;        

        public override void Start()
        {
            base.Start();

            Lap_Counter_Field_Name = CustomComponents.GetById<Text>("Lap Counter Field Name");
            Current_Lap = CustomComponents.GetById<Text>("Current Lap");
            Max_Lap = CustomComponents.GetById<Text>("Max Lap");
            Lap_Counter_Image = CustomComponents.GetById<Image>("LapCounterField");
            Lap_Counter_Background = CustomComponents.GetById<Image>("LapCounterFieldBackground");

            Current_Lap.text = TargetShip.CurrentLap.ToString("00");
            Max_Lap.text = Race.MaxLaps.ToString("00");

            NgRaceEvents.OnShipLapUpdate += SetLap;
        }

        public void SetLap(ShipController ship)
        {
            if (ship == TargetShip)
            {
                Current_Lap.text = TargetShip.CurrentLap.ToString("00");
            }
        }

        public override void OnDestroy()
        {
            NgRaceEvents.OnShipLapUpdate -= SetLap;            
            base.OnDestroy();
        }

    }

    public class Position_Counter : ScriptableHud
    {
        public Text Position_Counter_Field_Name;
        public Text Current_Position;
        public Text Max_Position;
        public Image Position_Counter_Field_Image;
        public Image Position_Counter_Field_Background;        

        public int Current_Place;
        public int Previous_Place = -1;
        public int Current_Active_Count;
        public int Previous_Active_Count = -1;
        public int Current_Lap;
        public int Previous_Lap = -1;

        public static readonly Vector4 Platinum_Color = new Vector4((63f/255f), 1f, 1f, 1f);
        public static readonly Vector4 Gold_Color = new Vector4(1f, (205f/255f), (102f/255f), 1f);
        public static readonly Vector4 Silver_Color = new Vector4((128f / 255f), (128f / 255f), (128f / 255f), 1f);
        public static readonly Vector4 Bronze_Color = new Vector4(1f, (102f/255f), 0f, 1f);

        public override void Start()
        {
            base.Start();

            Position_Counter_Field_Name = CustomComponents.GetById<Text>("Position Field Name");
            Current_Position = CustomComponents.GetById<Text>("Current Position");
            Max_Position = CustomComponents.GetById<Text>("Max Position");
            Position_Counter_Field_Image = CustomComponents.GetById<Image>("PositionCounterField");
            Position_Counter_Field_Background = CustomComponents.GetById<Image>("PositionCounterFieldBackground");
        }

        public override void Update()
        {
            base.Update();

            Current_Place = TargetShip.CurrentPlace;
            Current_Active_Count = Ships.Active.Count;
            Current_Lap = TargetShip.CurrentLap;

            if (Current_Place == Previous_Place && Current_Active_Count == Previous_Active_Count && Current_Lap == Previous_Lap)
            {
                return;
            }

            Previous_Place = Current_Place;
            Previous_Active_Count = Current_Active_Count;
            Previous_Lap = Current_Lap;

            Current_Position.text = Current_Place.ToString("00");
            Max_Position.text = Current_Active_Count.ToString("00");

            if (Current_Place == Current_Active_Count && Current_Active_Count != 1)
            {
                Current_Position.color = Color.red;
            }
            else if (Current_Place == 1 && Current_Lap == Race.MaxLaps)
            {
                Current_Position.color = Platinum_Color;
            }
            else if (Current_Place == 1)
            {
                Current_Position.color = Gold_Color;
            }
            else if (Current_Place == 2)
            {
                Current_Position.color = Silver_Color;
            }
            else if (Current_Place == 3)
            {
                Current_Position.color = Bronze_Color;
            }
            else
            {
                Current_Position.color = Color.white;
            }            
        }
    }

    public class Best_Time_Field : ScriptableHud
    {
        public Image Best_Time_Field_Background;
        public Image Best_Time_Field_Image;
        public Text Best_Time_Field_Name;
        public Text Best_Time_Readout;
        public string Best_Time_Units;
        public float Time_Milliseconds;
        public float Previous_Time_Milliseconds = -1f;

        public override void Start()
        {
            base.Start();

            Best_Time_Field_Background = CustomComponents.GetById<Image>("BestTimeFieldBackground");
            Best_Time_Field_Image = CustomComponents.GetById<Image>("BestTimeField");
            Best_Time_Field_Name = CustomComponents.GetById<Text>("Best Time Field Name");
            Best_Time_Readout = CustomComponents.GetById<Text>("Best Time");            
        }

        public override void Update()
        {
            base.Update();

            if (TargetShip.TargetTime != 0f)
            {
                Time_Milliseconds = TargetShip.TargetTime;
            }
            else
            {
                Time_Milliseconds = TargetShip.BestLapTime;
            }

            if (Time_Milliseconds == Previous_Time_Milliseconds)
            {
                return;
            }

            Previous_Time_Milliseconds = Time_Milliseconds;

            if (Time_Milliseconds != 0f)
            {
                Best_Time_Units = FloatToTime.Convert(Time_Milliseconds, "0:00.00");
            }
            else
            {
                Best_Time_Units = "–:––.––";
            }

            Best_Time_Readout.text = Best_Time_Units;
        }
    }

    public class Lap_Time_Field : ScriptableHud
    {
        public Image Lap_Time_Field_Image;
        public Image Lap_Time_Arrow_1;
        public Image Lap_Time_Arrow_2;
        public Image Lap_Time_Arrow_3;
        public Image Lap_Time_Arrow_4;
        public Image Lap_Time_Arrow_5;

        public Text Total_Time;
        public Text Lap_Time_1;
        public Text Lap_Time_2;
        public Text Lap_Time_3;
        public Text Lap_Time_4;
        public Text Lap_Time_5;
        public Text[] Lap_Texts;
        public Image[] Lap_Images;
        public static readonly Vector4 Lap_Diamond_Green = new Vector4(181f/255f, 1f, 29f/255f, 1f);
        public static readonly Vector4 Lap_Diamond_Red = new Vector4(1f, 28f/255f, 36f/255f, 1f);

        public int Current_Lap;
        public int Previous_Lap = -1;
        public bool Current_Lap_Is_Perfect;
        public bool Previous_Lap_Is_Perfect;        
        public int Current_Lap_Centiseconds;
        public int Previous_Lap_Centiseconds = -1;
        public float Total_Time_Value;
        public int Current_Total_Centiseconds;
        public int Previous_Total_Centiseconds = -1;
        public int Lap_Indexer;
        public bool New_Lap;

        public override void Start()
        {
            base.Start();
            //CustomComponents.GetById<Text>("");

            Lap_Time_Field_Image = CustomComponents.GetById<Image>("LapTimeField");
            Lap_Time_Arrow_1 = CustomComponents.GetById<Image>("LapTimeArrow1");
            Lap_Time_Arrow_2 = CustomComponents.GetById<Image>("LapTimeArrow2");
            Lap_Time_Arrow_3 = CustomComponents.GetById<Image>("LapTimeArrow3");
            Lap_Time_Arrow_4 = CustomComponents.GetById<Image>("LapTimeArrow4");
            Lap_Time_Arrow_5 = CustomComponents.GetById<Image>("LapTimeArrow5");

            Total_Time = CustomComponents.GetById<Text>("Total Time");
            Lap_Time_1 = CustomComponents.GetById<Text>("LapTime1");
            Lap_Time_2 = CustomComponents.GetById<Text>("LapTime2");
            Lap_Time_3 = CustomComponents.GetById<Text>("LapTime3");
            Lap_Time_4 = CustomComponents.GetById<Text>("LapTime4");
            Lap_Time_5 = CustomComponents.GetById<Text>("LapTime5");

            Total_Time.text = "–:––.––";
            Lap_Time_1.text = "–:––.––";
            Lap_Time_2.text = "–:––.––";
            Lap_Time_3.text = "–:––.––";
            Lap_Time_4.text = "–:––.––";
            Lap_Time_5.text = "–:––.––";

            Lap_Texts = new Text[] { Lap_Time_1, Lap_Time_2, Lap_Time_3, Lap_Time_4, Lap_Time_5 };
            Lap_Images = new Image[] { Lap_Time_Arrow_1, Lap_Time_Arrow_2, Lap_Time_Arrow_3, Lap_Time_Arrow_4, Lap_Time_Arrow_5 };

            for (int i = 1; i <= 4; i++)
            {
                Lap_Images[i].enabled = true;
                Lap_Texts[i].enabled = true;
            }
            
        }                

        public override void Update()
        {
            base.Update();

            Current_Lap = TargetShip.CurrentLap;

            New_Lap = Current_Lap != Previous_Lap;

            if (New_Lap)
            {
                Previous_Lap = Current_Lap;
                Previous_Lap_Centiseconds = -1;

                if (Current_Lap > 5 && Current_Lap % 5 == 1)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        Lap_Images[i].color = Color.white;
                        Lap_Texts[i].text = "–:––.––";
                    }
                }

                if ((Current_Lap % 5 == 1 || Current_Lap == 0) && (Race.MaxLaps - Current_Lap) <= 4)
                {
                    for (int i = (Current_Lap % 5) + (Race.MaxLaps - Current_Lap); i <= 4; i++)
                    {
                        Lap_Images[i].enabled = false;
                        Lap_Texts[i].enabled = false;
                    }
                }
            }

            if (Current_Lap == 0)
            {
                return;
            }

            Lap_Indexer = (Current_Lap - 1) % 5;

            Current_Lap_Centiseconds = (int)(TargetShip.CurrentLapTime * 100f);
            if (Current_Lap_Centiseconds != Previous_Lap_Centiseconds)
            {
                Previous_Lap_Centiseconds = Current_Lap_Centiseconds;
                Lap_Texts[Lap_Indexer].text = FloatToTime.Convert(TargetShip.CurrentLapTime, "0:00.00");
            }

            Current_Lap_Is_Perfect = TargetShip.IsPerfectLap;
            if (New_Lap || Current_Lap_Is_Perfect != Previous_Lap_Is_Perfect)
            {
                Previous_Lap_Is_Perfect = Current_Lap_Is_Perfect;

                if (Current_Lap_Is_Perfect)
                {
                    Lap_Images[Lap_Indexer].color = Lap_Diamond_Green;
                }
                else
                {
                    Lap_Images[Lap_Indexer].color = Lap_Diamond_Red;
                }
            }

            Total_Time_Value = TargetShip.TotalRaceTime;
            Current_Total_Centiseconds = (int)(Total_Time_Value * 100f);

            if (Current_Total_Centiseconds != Previous_Total_Centiseconds)
            {
                Previous_Total_Centiseconds = Current_Total_Centiseconds;
                Total_Time.text = FloatToTime.Convert(Total_Time_Value, "0:00.00");
            }
        }
    }

    public class Thrust_Bar : ScriptableHud //Speed Bar
    {
        public Image Thrust_Bar_Image;
        public Image Thrust_Bar_Background_Image;
        public Text Engine_Force_Text;        
        public float Hud_Speed;
        public int Speedometer_Display_Style;
        public float Speedometer_Value;
        public float Speedometer_Scale;
        public int Current_Readout_Key;
        public int Previous_Readout_Key = int.MinValue;
        public int Previous_Readout_Style = -1;
        public ShipSim Cached_ShipSim;

        public override void Start()
        {
            base.Start();

            Thrust_Bar_Image = CustomComponents.GetById<Image>("ThrustBar");
            Thrust_Bar_Background_Image = CustomComponents.GetById<Image>("ThrustBarBackground");
            Engine_Force_Text = CustomComponents.GetById<Text>("Engine Force Numeric Readout");

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();

            Hud_Speed = TargetShip.HudSpeed;
            Speedometer_Display_Style = VanillaPlusHUDOptions.ModMenuOptions.SpeedometerReadoutStyle;

            Thrust_Bar_Image.fillAmount = Hud_Speed * (1f / 1400f);
            //Engine_Force_Text.text = TargetShip.HudSpeed.ToString();            
            switch (Speedometer_Display_Style)
            {
                case 0:
                    Speedometer_Value = Cached_ShipSim.engineThrust;
                    Speedometer_Scale = 10f;
                    break;
                case 1:
                    Speedometer_Value = Hud_Speed;
                    Speedometer_Scale = 1f;
                    break;
                case 2:
                    Speedometer_Value = TargetShip.RBody.velocity.magnitude;
                    Speedometer_Scale = 10f;
                    break;
            }

            Current_Readout_Key = Mathf.RoundToInt(Speedometer_Value * Speedometer_Scale);

            if (Current_Readout_Key == Previous_Readout_Key && Speedometer_Display_Style == Previous_Readout_Style)
            {
                return;
            }

            Previous_Readout_Key = Current_Readout_Key;
            Previous_Readout_Style = Speedometer_Display_Style;

            if (Speedometer_Display_Style == 1)
            {
                Engine_Force_Text.text = Speedometer_Value.ToString("0");
            }
            else
            {
                Engine_Force_Text.text = Speedometer_Value.ToString("0.0");
            }

            //Engine_Force_Text.text = TargetShip.NetworkedVelocity.magnitude.ToString();            
        }
    }

    public class Weapon_Display : ScriptableHud //Weapon Icons
    {
        public string weapon_id;
        public Image Weapon_Icon;
        public Image Weapon_Background;
        public Text Absorb_Text;
        public Text Weapon_Text;

        public static readonly Vector2 Weapon_Display_Adjust_Vector = new Vector2(0, -178);

        public const float Weapon_Effectiveness_Compensation_Value = 0.032f; //Formerly, (14f / 375f)        
        public int Default_Cannon_Firerate;
        public float Default_Weapon_Effectiveness;
        public float Weapon_Effectiveness_Override;

        public bool Cannon_Is_Blacklisted;
        public int Current_Cannon_Firerate_Override;
        public int Previous_Cannon_Firerate_Override = -1;
        public int Cannon_Type_Index;

        public bool Ships_Can_Absorb_Pickups;
        public float Current_Absorb_Amount;
        public float Previous_Absorb_Amount;

        public PickupBase Current_Pickup;        

        public void Apply_Cannon_Override(int option)
        {            
            if (option == 3)
            {
                Cannon_Type_Index = Default_Cannon_Firerate;
            }
            else
            {
                Cannon_Type_Index = option;
            }

            if (option == 3 || option == Default_Cannon_Firerate)
            {
                TargetShip.Settings.DAMAGE_PWR = Default_Weapon_Effectiveness;
                TargetShip.Settings.CANNON_TYPE = (BallisticUnityTools.Ships.ECannonStat)Cannon_Type_Index;
            }
            else if (Default_Weapon_Effectiveness >= 0f)
            {
                if (Cannon_Is_Blacklisted)
                {
                    Weapon_Effectiveness_Override = Default_Weapon_Effectiveness;
                }
                else
                {
                    Weapon_Effectiveness_Override = Default_Weapon_Effectiveness - (Weapon_Effectiveness_Compensation_Value * (Default_Cannon_Firerate - option));
                }

                TargetShip.Settings.DAMAGE_PWR = Weapon_Effectiveness_Override;
                TargetShip.Settings.CANNON_TYPE = (BallisticUnityTools.Ships.ECannonStat)Cannon_Type_Index;
            }
        }

        public override void Update()
        {
            base.Update();

            Weapon_Text.text = TargetShip.PickupDisplayText;            

            Current_Cannon_Firerate_Override = VanillaPlusHUDOptions.ModMenuOptions.CannonFirerateOverride;
            if (Current_Cannon_Firerate_Override != Previous_Cannon_Firerate_Override)
            {
                Previous_Cannon_Firerate_Override = Current_Cannon_Firerate_Override;
                Apply_Cannon_Override(Current_Cannon_Firerate_Override);
            }            

            Current_Pickup = TargetShip.CurrentPickup;
            if (Current_Pickup == null)
            {
                return;
            }

            Current_Absorb_Amount = 0f;
            if (Ships_Can_Absorb_Pickups)
            {
                Current_Absorb_Amount = Current_Pickup.GetCurrentAbsorbAmount();
            }

            if (Current_Absorb_Amount == Previous_Absorb_Amount)
            {
                return;
            }

            Previous_Absorb_Amount = Current_Absorb_Amount;

            if (Current_Absorb_Amount != 0f)
            {
                Absorb_Text.text = Current_Absorb_Amount.ToString("+0.##");
            }
            else
            {
                Absorb_Text.text = "";
            }
        }

        public override void Start()
        {
            base.Start();

            Default_Cannon_Firerate = (int)TargetShip.Settings.CANNON_TYPE;

            if (RaceManager.CurrentGmConfig.AllShipsHaveWeapons == false || TargetShip.Settings.DAMAGE_PWR > 0f)
            {
                Default_Weapon_Effectiveness = TargetShip.Settings.DAMAGE_PWR;
            }
            else
            {
                Default_Weapon_Effectiveness = 1f;
            }
            

            Weapon_Icon = CustomComponents.GetById<Image>("PickupDisplay");
            Weapon_Text = CustomComponents.GetById<Text>("Weapon Message");

            Weapon_Background = CustomComponents.GetById<Image>("PickupDisplayBackground");
            Absorb_Text = CustomComponents.GetById<Text>("AbsorbMessage");

            PickupBase.OnPickupInit += SetPickup;
            PickupBase.OnPickupDeinit += DropPickup;

            Weapon_Icon.enabled = false;
            Weapon_Background.enabled = false;
            Weapon_Text.text = "";
            Absorb_Text.text = "";

            if (VanillaPlusHUDOptions.ModMenuOptions.WeaponMirrorPositionSwap)
            {
                Weapon_Icon.rectTransform.anchoredPosition = Weapon_Icon.rectTransform.anchoredPosition + Weapon_Display_Adjust_Vector;
                Weapon_Text.rectTransform.anchoredPosition = Weapon_Text.rectTransform.anchoredPosition + Weapon_Display_Adjust_Vector;
                Weapon_Background.rectTransform.anchoredPosition = Weapon_Background.rectTransform.anchoredPosition + Weapon_Display_Adjust_Vector;
                Absorb_Text.rectTransform.anchoredPosition = Absorb_Text.rectTransform.anchoredPosition + Weapon_Display_Adjust_Vector;
            }

            Cannon_Is_Blacklisted = NgData.Race.WeaponBlackList.GetValue(RaceManager.Instance.RaceContext).WeaponNames.Contains("cannon");
            Ships_Can_Absorb_Pickups = RaceManager.CurrentGamemode.Configuration.ShipsCanAbsorbPickups;
        }

        public void SetPickup(PickupBase pickup, ShipController ship)

        {
            if (ship == TargetShip)
            {
                Weapon_Icon.enabled = true;
                Weapon_Background.enabled = true;
                weapon_id = TargetShip.CurrentPickupRegister.Name;
                Weapon_Icon.sprite = CustomHudRegistry.GetWeaponSprite(weapon_id, true);
            }
        }

        public void DropPickup(PickupBase pickup, ShipController ship)
        {
            if (ship == TargetShip)
            {
                Weapon_Icon.enabled = false;
                Weapon_Background.enabled = false;
                Previous_Absorb_Amount = 0f;
                Absorb_Text.text = "";
            }
        }
        public override void OnDestroy()
        {
            PickupBase.OnPickupInit -= SetPickup;
            PickupBase.OnPickupDeinit -= DropPickup;
            base.OnDestroy();
        }

    }

    public class Energy_Bar : ScriptableHud
    {
        public float Current_Shield_Integrity;
        public float Previous_Shield_Integrity = -1f;
        public float Shield_Display_Value;
        public float Previous_Shield_Display_Value;
        public int Current_Readout_Precision;
        public int Previous_Readout_Precision = -1;
        public float Shield_Effectiveness;
        public float Shield_Stat;        

        public Text Shield_Integrity_Numeric_Readout;
        public Image Energy_Bar_Image;
        public Image Energy_Bar_Background_Image;

        public bool Energy_Low_Coroutine_Running;
        public bool Energy_Critical_Coroutine_Running;        
        
        public float Lowest_Enemy_Weapon_Effectiveness = float.MaxValue;
        public float Highest_Enemy_Weapon_Effectiveness = 0f;

        public float Plasma_WCDR = 71.15920f;
        public float Plasma_BCDR = 60.27840f;
        public float Rockets_WCDR = 36.59616f;
        public float Missile_WCDR = 15.42840f;
        public float Tremor_WCDR = 8.13248f;

        public static readonly string[] Readout_Formats = { null, "0.####", "0.###", "0.##", "0.#", "0" };

        //public Vector4 Color_Breakpoint_0 = new Vector4(255, 28, 36, 255); //Red
        //public Vector4 Color_Breakpoint_1 = new Vector4(255, 127, 39, 255); //Orange
        //public Vector4 Color_Breakpoint_2 = new Vector4(255, 242, 0, 255); //Yellow
        //public Vector4 Color_Breakpoint_3 = new Vector4(87, 255, 23, 255); //Green
        //public Vector4 Color_Breakpoint_4 = new Vector4(0, 162, 232, 255); //Blue
        //public Vector4 Color_Breakpoint_5 = new Vector4(255, 255, 255, 255); //White
        public static readonly Vector4 Color_Breakpoint_0 = new Vector4(1f, (28f / 255f), (36f / 255f), 1f); //Red
        public static readonly Vector4 Color_Breakpoint_1 = new Vector4(1f, (127f / 255f), (39f / 255f), 1f); //Orange
        public static readonly Vector4 Color_Breakpoint_2 = new Vector4(1f, (242f / 255f), 0f, 1f); //Yellow
        public static readonly Vector4 Color_Breakpoint_3 = new Vector4((87f / 255f), 1f, (23f / 255f), 1f); //Green
        public static readonly Vector4 Color_Breakpoint_4 = new Vector4(0f, (162f / 255f), (232f / 255f), 1f); //Blue
        public static readonly Vector4 Color_Breakpoint_5 = new Vector4(1f, 1f, 1f, 1f); //White
        public Vector4 Energy_Bar_Background_Original_Color;

        public static readonly Vector4 Clear_Red = new Vector4(1f, 0f, 0f, 0f); //Don't have sprites you want to change the color of via script pre-colored and pre-alpha'd outside of Unity because modifying them in script modifies the color channels relative to the colors in the file provided as a sprite rather than absolute colors
        //public static readonly Vector4 Dark_Red = new Vector4(1f, 0f, 0f, 0.8f);

        IEnumerator Energy_Color_Pulse(float duration, bool critical)
        {
            if (critical) 
            { 
                Energy_Critical_Coroutine_Running = true; 
            }
            else 
            { 
                Energy_Low_Coroutine_Running = true; 
            }

            float half = duration * 0.5f;
            float elapsed = 0f;

            while (true)
            {
                yield return null;
                elapsed += Time.deltaTime;

                if (elapsed >= duration)
                {
                    break;
                }

                if (elapsed <= half)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Clear_Red, Color.red, elapsed / half);
                }
                else
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Color.red, Clear_Red, elapsed / duration);
                }
            }

            if (critical) 
            { 
                Energy_Critical_Coroutine_Running = false; 
            }
            else 
            { 
                Energy_Low_Coroutine_Running = false; 
            }
        }

        public void CalculateDamageBreakpoints()
        {
            foreach (ShipController shipcon in Ships.Loaded)
            {
                if (shipcon != TargetShip && shipcon.Settings.DAMAGE_PWR > 0f)
                {
                    float power = shipcon.Settings.DAMAGE_PWR;

                    if (power < Lowest_Enemy_Weapon_Effectiveness) { Lowest_Enemy_Weapon_Effectiveness = power; }
                    if (power > Highest_Enemy_Weapon_Effectiveness) { Highest_Enemy_Weapon_Effectiveness = power; }
                }
            }

            if (Highest_Enemy_Weapon_Effectiveness >= Lowest_Enemy_Weapon_Effectiveness)
            {
                Plasma_WCDR = Highest_Enemy_Weapon_Effectiveness * TargetShip.Settings.DAMAGE_MULT * 70f;
                Plasma_BCDR = Lowest_Enemy_Weapon_Effectiveness * TargetShip.Settings.DAMAGE_MULT * 70f;
                Rockets_WCDR = Highest_Enemy_Weapon_Effectiveness * TargetShip.Settings.DAMAGE_MULT * 36f;
                Missile_WCDR = Highest_Enemy_Weapon_Effectiveness * TargetShip.Settings.DAMAGE_MULT * 15f;
                Tremor_WCDR = Highest_Enemy_Weapon_Effectiveness * TargetShip.Settings.DAMAGE_MULT * 8f;
            }

            Previous_Shield_Integrity = -1f;
        }

        public override void OnDestroy()
        {
            NgRaceEvents.OnCountdownStart -= CalculateDamageBreakpoints;

            base.OnDestroy();
        }

        public override void Start()
        {
            base.Start();

            Shield_Effectiveness = TargetShip.Settings.DAMAGE_MULT;
            Shield_Stat = Shield_Effectiveness + 0.07f;            

            if (Shield_Stat > 1.0f)
            {
                Shield_Stat *= 1.1f;
            }
            else if (Shield_Stat < 1.0f)
            {
                Shield_Stat *= 0.9f;
            }

            Energy_Bar_Image = CustomComponents.GetById<Image>("EnergyBar");
            Energy_Bar_Background_Image = CustomComponents.GetById<Image>("EnergyBarBackground");
            Shield_Integrity_Numeric_Readout = CustomComponents.GetById<Text>("Shield Integrity Numeric Readout");

            Energy_Bar_Background_Original_Color = Energy_Bar_Background_Image.color;

            NgRaceEvents.OnCountdownStart += CalculateDamageBreakpoints;
        }

        public override void Update()
        {
            base.Update();

            Current_Shield_Integrity = TargetShip.ShieldIntegrity;            

            if (Current_Shield_Integrity != Previous_Shield_Integrity)
            {
                Previous_Shield_Integrity = Current_Shield_Integrity;
                Energy_Bar_Image.fillAmount = Current_Shield_Integrity * 0.01f;

                if (Current_Shield_Integrity > Plasma_WCDR)
                    Energy_Bar_Image.color = Color_Breakpoint_5;
                else if (Current_Shield_Integrity > Plasma_BCDR)
                    Energy_Bar_Image.color = Color_Breakpoint_4;
                else if (Current_Shield_Integrity > Rockets_WCDR)
                    Energy_Bar_Image.color = Color_Breakpoint_3;
                else if (Current_Shield_Integrity > Missile_WCDR)
                    Energy_Bar_Image.color = Color_Breakpoint_2;
                else if (Current_Shield_Integrity > Tremor_WCDR)
                    Energy_Bar_Image.color = Color_Breakpoint_1;
                else if (Current_Shield_Integrity >= 0.00001f)
                    Energy_Bar_Image.color = Color_Breakpoint_0;
            }

            if (Hud.ShieldDisplayMode == EShieldDisplayMode.Percentage)
            {
                Shield_Display_Value = Current_Shield_Integrity;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.AbsoluteShieldValueStyle == 1)
            {
                Shield_Display_Value = Current_Shield_Integrity / Shield_Effectiveness;
            }
            else
            {
                Shield_Display_Value = Current_Shield_Integrity * (1f / Shield_Stat);
            }

            Current_Readout_Precision = VanillaPlusHUDOptions.ModMenuOptions.EnergyBarReadoutDecimalPrecision;
            if (Shield_Display_Value != Previous_Shield_Display_Value || Current_Readout_Precision != Previous_Readout_Precision)
            {
                Previous_Shield_Display_Value = Shield_Display_Value;
                Previous_Readout_Precision = Current_Readout_Precision;
                Shield_Integrity_Numeric_Readout.text = Shield_Display_Value.ToString(Readout_Formats[Current_Readout_Precision]);
            }

            if (Current_Shield_Integrity > 10f && Current_Shield_Integrity <= 25f && !Energy_Low_Coroutine_Running)
            {
                StartCoroutine(Energy_Color_Pulse(0.4f, false));
            }
            else if (Current_Shield_Integrity <= 10f && !Energy_Critical_Coroutine_Running)
            {
                StartCoroutine(Energy_Color_Pulse(0.2f, true));
            }
            else if (Current_Shield_Integrity > 25f)
            {
                Energy_Bar_Background_Image.color = Energy_Bar_Background_Original_Color;
            }
        }
    }

    public class Throttle_Bar : ScriptableHud
    {
        public Image Throttle_Bar_Image;
        public Image Throttle_Bar_Background_Image;
        public Vector4 Throttle_Bar_Image_Original_Color;
        public static readonly Vector4 Dark_Blue = new Vector4(0f, (96f / 255f), 1f, 1f);
        public static readonly Vector4 Light_Blue = new Vector4((154f / 255f), (217f / 255f), 1f, 1f);
        public static readonly WaitForEndOfFrame Cached_WaitForEndOfFrame = new WaitForEndOfFrame();
        
        public const float Speed_Pad_Pulse_Duration = 0.5f;
        public ShipSim Cached_ShipSim;

        IEnumerator SpeedPadColorPulse_Coroutine()
        {
            float elapsed = 0f;            

            while (true)
            {
                yield return Cached_WaitForEndOfFrame;
                elapsed += Time.deltaTime;

                if (elapsed >= Speed_Pad_Pulse_Duration)
                {
                    break;
                }

                Throttle_Bar_Image.color = Color.Lerp(Dark_Blue, Light_Blue, elapsed / Speed_Pad_Pulse_Duration);
            }

            Throttle_Bar_Image.color = Throttle_Bar_Image_Original_Color;
        }

        public override void Start()
        {
            base.Start();

            NgRaceEvents.OnShipHitSpeedPad += SpeedPadColorPulse;
            NgRaceEvents.OnShipHitSpeedTile += SpeedPadColorPulse_Tile;

            Throttle_Bar_Background_Image = CustomComponents.GetById<Image>("ThrottleBarBackground");
            Throttle_Bar_Image = CustomComponents.GetById<Image>("ThrottleBar");

            Throttle_Bar_Image_Original_Color = Throttle_Bar_Image.color;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();            

            Throttle_Bar_Image.fillAmount = Mathf.Min(Cached_ShipSim.enginePower, Cached_ShipSim.engineAccel);
        }

        public void SpeedPadColorPulse(ShipController ship, BallisticUnityTools.TrackTools.TrackPad pad) //Works with 3D ("Modern") pads
        {
            if (ship == TargetShip)
            {
                StartCoroutine(SpeedPadColorPulse_Coroutine());
            }
        }

        public void SpeedPadColorPulse_Tile(ShipController ship, NgTrackData.Tile tile) //Works with "Classic" speedpads
        {
            if (ship == TargetShip)
            {
                StartCoroutine(SpeedPadColorPulse_Coroutine());
            }
        }

        public override void OnDestroy()
        {
            NgRaceEvents.OnShipHitSpeedPad -= SpeedPadColorPulse;
            NgRaceEvents.OnShipHitSpeedTile -= SpeedPadColorPulse_Tile;

            base.OnDestroy();
        }
    }

    public class Rear_View_Mirror : ScriptableHud //Special thanks to Dekaid for sharing his rear view mirror implementation, I would never have figured out how to set this up on my own
    {
        public RawImage RearViewMirror;
        public Camera Rear_View_Mirror_Camera;
        public RectTransform Empty_Game_Object;
        public RenderTexture Rear_View_Mirror_Feed;
        public static readonly Vector3 Point_Camera_Backwards_Transform_Scale = new Vector3(-1f, 1f);
        public static readonly Vector3 Point_Camera_Forwards_Transform_Scale = new Vector3(1f, 1f);
        public static readonly Vector2 Rear_View_Mirror_Adjust_Vector = new Vector2(0, 223);

        public float lastWidth, lastHeight;
        public int Current_Mirror_State;
        public int Previous_Mirror_State = -1;
        public int Hull_Layer_Mask;
        public int Current_Culling_Mask;
        public int Previous_Culling_Mask;

        public ShipCamera Cached_ShipCamera;

        public override void Start()
        {
            base.Start();

            RearViewMirror = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            RearViewMirror.color = Color.clear;
            Empty_Game_Object = CustomComponents.GetById("EmptyGameObject"); //initialize the RectTransform through the Empty Game Object component in the Unity prefab
            Rear_View_Mirror_Camera = Empty_Game_Object.gameObject.AddComponent<Camera>(); //Adding a camera dynamically by attaching it to the Empty Game Object
            Apply_Screen_Size(); //Sizes the mirror and creates the RenderTexture for the current resolution            

            Rear_View_Mirror_Camera.transform.SetParent(TargetShip.transform);
            Rear_View_Mirror_Camera.transform.localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / 2;

            Hull_Layer_Mask = ~(1 << TargetShip.Settings.REF_MESH.gameObject.layer);
            Previous_Culling_Mask = TargetShip.ShipCamera.cullingMask;
            Rear_View_Mirror_Camera.cullingMask = Previous_Culling_Mask & Hull_Layer_Mask;

            RearViewMirror.gameObject.SetActive(true);

            if (VanillaPlusHUDOptions.ModMenuOptions.WeaponMirrorPositionSwap)
            {
                RearViewMirror.rectTransform.anchoredPosition = RearViewMirror.rectTransform.anchoredPosition + Rear_View_Mirror_Adjust_Vector;
            }

            Cached_ShipCamera = TargetShip.CamSim;
        }        

        public void Apply_Screen_Size()
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            RearViewMirror.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 640f * ((lastWidth / lastHeight) / (16f / 9f)));
            CreateRenderTexture(640f, 160f); //Size of the RenderTexture on user screen
        }

        public void CreateRenderTexture(float rWidth, float rHeight)
        {
            if (Rear_View_Mirror_Feed != null)
            {
                Rear_View_Mirror_Camera.targetTexture = null;
                Rear_View_Mirror_Feed.Release();
                Destroy(Rear_View_Mirror_Feed);                
            }

            Rear_View_Mirror_Feed = new RenderTexture((int)(lastWidth / (1920f / rWidth)), (int)(lastHeight / (1080f / rHeight)), 32); //32 is the color format
            Rear_View_Mirror_Camera.targetTexture = Rear_View_Mirror_Feed;
            RearViewMirror.texture = Rear_View_Mirror_Feed;
        }

        public override void Update()
        {
            base.Update();

            if (Cached_ShipCamera.LookingBehind)
            {
                Current_Mirror_State = 1;
            }
            else
            {
                Current_Mirror_State = 0;
            }            

            if (Current_Mirror_State != Previous_Mirror_State)
            {
                Previous_Mirror_State = Current_Mirror_State;

                if (Current_Mirror_State == 0)
                {
                    RearViewMirror.transform.localScale = Point_Camera_Backwards_Transform_Scale;
                    Rear_View_Mirror_Camera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    RearViewMirror.transform.localScale = Point_Camera_Forwards_Transform_Scale;
                    Rear_View_Mirror_Camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }

                RearViewMirror.color = Color.white; //Set alpha back to full so the rear view mirror is visible and opaque
            }

            Current_Culling_Mask = TargetShip.ShipCamera.cullingMask;
            if (Current_Culling_Mask != Previous_Culling_Mask)
            {
                Previous_Culling_Mask = Current_Culling_Mask;
                Rear_View_Mirror_Camera.cullingMask = Current_Culling_Mask & Hull_Layer_Mask;
            }

            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                Apply_Screen_Size();
            }

            if (TargetShip.FinishedEvent && Rear_View_Mirror_Camera.enabled)
            {
                Rear_View_Mirror_Camera.enabled = false;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (Rear_View_Mirror_Camera != null) 
            { 
                Rear_View_Mirror_Camera.targetTexture = null;
                Destroy(Rear_View_Mirror_Camera.gameObject);
            }
            if (Rear_View_Mirror_Feed != null) 
            { 
                Rear_View_Mirror_Feed.Release(); 
                Destroy(Rear_View_Mirror_Feed); 
            }            
        }
    }

    public class Camera_Rotation_Overrides_2280 : ScriptableHud
    {
        public string Track_Display_Name;

        public bool Has_Entered_Jump_Boolean;
        //public bool Has_Entered_Jump_And_Is_Airborne_Boolean;
        //public bool Has_Left_Jump_And_Is_Grounded_Boolean;

        //public bool Has_Been_Out_Of_Bounds_Boolean;
        //public bool Has_Been_Jump_Airborne_Boolean;
        //public bool Jump_Airborne_Rotation_Complete_Boolean;

        public Vector3 Internal_Camera_Rotation;

        public Quaternion Pseudohugger_Rotation_Quaternion = Quaternion.identity;
        //public Quaternion Intermediate_Rotation_Quaternion;
        //public Quaternion Rigidbody_Rotation_Quaternion;

        public Quaternion Previous_Pseudohugger_Rotation_Quaternion = Quaternion.identity;
        //public Quaternion Previous_Intermediate_Rotation_Quaternion;
        //public Quaternion Previous_Rigidbody_Rotation_Quaternion;        

        public Quaternion Stored_Pseudohugger_Rotation_Quaternion = Quaternion.identity;
        //public Quaternion Stored_Intermediate_Rotation_Quaternion;
        //public Quaternion Stored_Rigidbody_Rotation_Quaternion;

        public Quaternion Target_Pseudohugger_Rotation_Quaternion = Quaternion.identity;



        public Quaternion Tilt_Lock_Rotation_Quaternion = Quaternion.identity; //used for Tiltlock2280 Camera Mode
        public Quaternion Target_Tilt_Lock_Rotation_Quaternion = Quaternion.identity; //used for Tiltlock2280 Camera Mode


        public float Rotation_Time;
        public float Tiltlock_Maglocked_Interpolation_Ratio;
        public float Tiltlock_Non_Maglocked_Interpolation_Ratio;
        //public float Pseudohugger_Aerial_Interpolation_Ratio;
        public float Pseudohugger_Grounded_Interpolation_Ratio;

        //Misalignment can be introduced by bad TRM traces, negatively indexed sections, bad pitlane geometry, bad junction geometry generally, junctions crossing over into the normal route and vice versa, gaps/jumps, and really sharp changes in track pitch, track width, or section-to-section distance
        public static readonly int[] BNGL_Arrivon_Falls_8 = { 398, 399, 461, 462 };
        public static readonly int[] BNGL_Oceana_8 = { 121, 122, 176, 177 };
        public static readonly int[] Helheim = { 43, 44, 113, 114, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243 };
        public static readonly int[] Alphard_Reverse = { 357, 358 };
        public static readonly int[] Samrong_Crossing_Reverse = { 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124 };
        public static readonly int[] Blackward_Decks = { 14, 15, 103, 105, 146, 279, 280, 459, 460, 508, 509, 574, 581, 588, 595 };
        public static readonly int[] Great_Manitou_Trail = { 156, 321, 354, 355, 380, 381, 402, 403, 443, 505, 506, 572, 648, 650, 715, 828, 829, 1122, 1144, 1145 };
        public static readonly int[] Vestfjorden = { 35, 36 };
        public static readonly int[] Abyssus = { 149, 150, 209, 210, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 259, 260, 261, 262, 380, 381, 489, 490, 500, 501 };
        public static readonly int[] Aeolus_II = { 0, 1, 2, 116, 117, 118, 170, 171, 172, 236, 237, 238, 245, 246, 247, 269, 270, 271, 296, 297, 298 };
        public static readonly int[] Altima_XIV = { 203, 204, 205 };
        public static readonly int[] Antelao = { 168, 169, 187, 188, 308, 309 };
        public static readonly int[] Antelao_Reverse = { 9, 10, 11, 130, 131, 132, 149, 150, 151 };
        public static readonly int[] Beltane = { 318, 319 };
        public static readonly int[] Cairodrome = { 22, 23, 24, 35, 57, 58, 59, 190, 191, 192, 281, 282, 283 };
        public static readonly int[] Canyonlands = { 48, 49, 50, 51, 281, 282, 295, 296, 310, 311, 329, 330, 493, 494, 495, 496, 497 };
        public static readonly int[] Canyonlands_Reverse = { 86, 87, 88, 105, 106, 107, 120, 121, 122, 134, 135, 136, 401, 402, 403 };
        public static readonly int[] Dione_IV = { -417, -418, -419, -420, -421, 390, 391, 392 };
        public static readonly int[] Elivagar_Reverse = { 327, 328 };
        public static readonly int[] Eschaton = { 3, 4, 14, 15, 37, 38, 159, 160, 165, 166, 177, 178, 308, 309 };
        public static readonly int[] Gehennom = { 1, 2, 107, 108, 220, 221, 243, 244, 255, 256, 276, 277, 291, 292, 301, 302, 321, 322, 462, 523, 529, 530, 568, 569, 613, 614, 634, 635, 654, 655, 672, 673 };
        public static readonly int[] Herne_Park = { 243, 244 };
        public static readonly int[] Kamanneq = { 2, 3, 7, 42, 102, 103, 147, 148 };
        public static readonly int[] Lumenar_V = { 7, 8, 31, 32 };
        public static readonly int[] Millenium_Wharf = { 78, 79, 251, 252, 355, 356, 428, 429 };
        public static readonly int[] Project_Pandora = { 180, 181, 500, 501, 510, 511, 521, 522 };
        public static readonly int[] Shanghai_Financial = { 13, 14, 179, 180 };
        public static readonly int[] South_Ridge = { 213, 214 };
        public static readonly int[] Sovereign = { 7, 8, 120, 121, 271, 272, 273, 274, 275, 276 };
        public static readonly int[] Sovereign_Reverse = { 65, 66, 67, 68, 69, 70, 220, 221, 333, 334 };
        public static readonly int[] The_STJ = { 201, 202 };
        public static readonly int[] Dagon_Prime = { -73, -74, -75, -76, -77, -78, -79, -80, -81, -82, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 119, 120, 175, 176, 177, 178, 179, 302, 303, 351, 352, 368, 369, 471, 472, 511, 512, 535, 536, 557, 558 };
        public static readonly int[] Prototype_19_WLD_Botanica = { 182, 183 };
        public static readonly int[] Prototype_77_ODR_Nachtmahr = { 6, 7 };
        public static readonly int[] Prototype_81_JMT_Lambda2Phi = { 2, 3, 26, 27, 155, 156, 188, 189 };
        public static readonly int[] Prototype_99_KEM_Sunthrone = { 20, 21, 44, 45, 46, 47, 48, 49, 50, 51, 61, 62, 66, 67, 71, 72, 76, 77, 78, 79, 80, 97, 98, 125, 126, 160, 161, 162, 163, 187, 188, 189, 205, 206, 207, 208, 209, 210, 234, 235, 271, 272, 273, 274, 275, 276, 277, 287, 288 };
        public static readonly int[] ES_0x016_Germania = { 191, 192 };
        public static readonly int[] ES_0x033_Dione_II = { 99, 100, 135, 136, 427, 428, 460, 461 };
        public static readonly int[] Annapurna = { 139, 201, 202, 203, 230, 231, 232, 233, 234, 322, 447, 466, 491, 498, 547, 582 };
        public static readonly int[] Desolata = { 679, 680 };
        public static readonly int[] Infinity_Spear = { 119, 120, 163, 164, 165, 221, 222, 231, 232, 241, 242, 250, 251, 263, 264 };
        public static readonly int[] bngl_metro_11 = { 157, 158, 391, 392, 393, 394, 395, 396, 397, 431, 432 };
        public static readonly int[] bngl_metro_reverse_11 = { 34, 35, 36, 222, 223, 224, 242, 243, 244, 377, 378, 379, 399, 400, 401, 505, 506, 507, 553, 554, 555 };
        public static readonly int[] bngl_nova_split_11 = { 13, 14, 33, 34, 121, 122, 151, 152, 173, 174, 211, 212, 245, 246, 309, 310, 311, 324, 367 };
        public static readonly int[] bngl_nova_split_reverse_11 = { 11, 12, 13, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, /*185, 186, 187, 188, 189, 190, 191, 192, 193, 194,*/ 195, 196, 197, 198, 199, 200, 201, /*202, 203, 204, 205,*/ 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 300, 301, 302, 303, 304, 305, 306, 307, 308, 330, 361, 362, 363, 364, 373 };
        public static readonly int[] bngl_wipeoutzone = { -517, -533, 388, 389, 390, 403, 404, 405, 418, 523, 524, 660, 661, 666, 667, 777, 778, 944, 945, 972, 973, 974, 975, 976, 977, 1000, 1001, 1029, 1030, 1031, 1061, 1125, 1126, 1192, 1194, 2252, 2253 };
        public static readonly int[] DandelionCircuit = { -327, -351, -352, -381, -382, -394, -395, -396, -397, -431 };
        public static readonly int[] Astra_Magnesium = { -421, 176, 177, 201, 202 };
        public static readonly int[] Serenewoods = { 208, 209, 210, 211, 212, 213, 214, 231, 232, 233, 234, 235, -589 };
        public static readonly int[] Cobbledark = { -543 };
        public static readonly int[] SW1R_Bumpys_Breakers = { 106, 107, 108, 109, 306, 307, 308, 309, 310, 482, 483 };
        public static readonly int[] SW1R_Malastare_100 = { 87, 88, 89, 90, 91, 92, 93, 94, 95, 101, 102, 103, 107, 108, 109, 110, };
        public static readonly int[] SW1R_Scrappers_Run = { 83, 84, 148, 149 };
        public static readonly int[] SW1R_The_Boonta_Eve_Classic = { 308, 309, 310, 579, 580 };
        public static readonly int[] SW1R_Boonta_Eve_Training_Course = { 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54 };
        public static readonly int[] SW1R_Aquilaris_Classic = { 139, 140, 141, 142, 143, 144, 145, 146, 147, 196, 197, 198, 199, 200, 201, 236, 238, 239, 240, 241, 242, 243, 244, 249, 251, 252 };
        public static readonly int[] NFS1_Alpine = { 430, 431, 432, 433 };
        public static readonly int[] NFS1_Transtropolis = { 357, 358, 359, 360 };
        public static readonly int[] Solaris = { 14, 15, 78, 79, 151, 152, 290, 291, 362, 363 };
        public static readonly int[] Solaris_Reverse = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 195, 196, 197, 198, 199, 200, 201, 202, 203, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422 };
        public static readonly int[] Aerodive = { 93, 94, 191, 192 };
        public static readonly int[] Escalation_Project = { 39, 40, 41, 42, 43, 44, 176, 177, 178, 179, 255, 256, 270, 288, 302, 403, 404, 423, 424, 425, 426, 480, 481 }; //note that 179 is included so it can be UNMARKED as JUMP
        public static readonly int[] Kaiten = { 145, 146, 158, 159, 206, 207 };
        public static readonly int[] Loch_Aberton = { 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 152, 153, -442, -444, -445, -446, -447, -452, -453, -468, -469, -478, -479, -491, -492, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395 };
        public static readonly int[] Parashant = { 233, 234, 235, 315, 316, 530, 569 };
        public static readonly int[] Stone_Run = { 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 263, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367 };
        public static readonly int[] Trinity_Coast = { 231, 451, 479, 480, 481, 482, 483, 0, 1 };
        public static readonly int[] wtrp_wo1_korodera = { 108, 109, 110, 349, 350 };
        public static readonly int[] wtrp_wo2097_spilskinanke = { 82, 102, 136, 137, 148, 149, 159, 160, 204, 205, 206, 207, 214, 215, 216, 217, 224, 225, 226, 227, 306, 307, 334, 335 };
        public static readonly int[] wtrp_wo2097_vostok_island = { 10, 11, 34, 35, 43, 44, 75, 76, 134, 135, 136, 137, 149, 150, 151, 152, 153, 195, 196, 215, 216 };
        public static readonly int[] wtrp_wo3_manor_top = { 78, 79, 91, 92, 132, 133, 142, 143, 152, 153, 162, 163 };
        public static readonly int[] moltocaldo_desert = { 172, 173, 188, 189, 205, 206, 218, 219, 374, 404, 405, -427, -460, -461 };
        public static readonly int[] Sunken_City = { 120, 121, 285, 286, 418, -508, -547 };
        //Pitlane-related (but also other sources of misalignment) below
        public static readonly int[] Atacama = { 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, /*160, 161,*/ 162, 163, 285, 286, 287, 288, 289, 304, 305, 306, 307, 308, 309, 310 }; //section 161 should be omitted in terms of correctness but the jump at sections 162 and 163 renders its exclusion mostly meaningless, so it has been included
        public static readonly int[] Atacama_Reverse = { 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 279, 280, 281, 282, 283, 284, 285, 300, 301, 302, 303, 304, 308, 309, 310 };
        public static readonly int[] Cairodrome_II = { 530, 531, 561, 562 };
        public static readonly int[] Cairodrome_II_Reverse = { 533, 534, 564, 565 };
        public static readonly int[] Cassandra = { 250, 284 };
        public static readonly int[] Cassandra_Reverse = { 12, 46 };
        public static readonly int[] Kanlaon_Peak = { 237, 261 };
        public static readonly int[] Skyline_Sprint = { -397, -425 };
        public static readonly int[] Aciknovae = { 621 };
        public static readonly int[] Aciknovae_Reverse = { 245, 246, 247, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 594 };
        public static readonly int[] Nova_Split_Reverse = { 108, 109, 110, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 171, 172, 173, 196, 197, 198, 304, 324, 368 };
        public static readonly int[] Ishtar_Citadel = { 316, 329, 331 };
        public static readonly int[] Ishtar_Citadel_Reverse = { 312, 313, 328 };
        public static readonly int[] Arrivon_XI = { 366 };
        public static readonly int[] Arrivon_XI_Reverse = { 343 };
        public static readonly int[] Utah_Project = { 288, 306 };
        public static readonly int[] Utah_Project_Reverse = { 287, 305 };
        public static readonly int[] Marina_Rush = { 299, 300, 338, 339 };
        public static readonly int[] Marina_Rush_Reverse = { 302, 303, 341, 342};
        public static readonly int[] Atlantica = { 187, 189, 349, 350, 351, 352, 353, 354, 355, 356, 357, 432 };
        public static readonly int[] Atlantica_Reverse = { 47, 48, 49, 50, 51, 52, 53, 54, 79, 80, 81, 96, 408 };
        public static readonly int[] Tokyo_Bowl = { 194, 237 };
        public static readonly int[] Tokyo_Bowl_Reverse = { 193, 236 };
        public static readonly int[] Lujiazui_Park_Reverse = { 392 };
        public static readonly int[] Pocono_Circuit = { 202, 203, 238 };
        public static readonly int[] Pocono_Circuit_Reverse = { 205, 240, 241 };
        public static readonly int[] Arrivon_Falls = { 318, 319, 351 };
        public static readonly int[] Arrivon_Falls_Reverse = { 349, 351 };
        public static readonly int[] Meads_Pass = { 221, 222, 252, 253 };
        public static readonly int[] Meads_Pass_Reverse = { 221, 251, 252 };
        public static readonly int[] Pacheon = { 309, 338 };
        public static readonly int[] Pacheon_Reverse = { 308, 336 };
        public static readonly int[] Hydrome_Bed = { 33, 34, 35, 50, 51, 52, 82, 85, 86, 87, 105, 106, 107, 112, 113, 114, 146, 147, 148, 191, 192, 241, 242, 243, 322, 351 };
        public static readonly int[] Arrivon_Peak = { 128, 129, 130, 131, 296, 318 };
        public static readonly int[] Brattelva_Circuit = { 396, 397, 431, 433 };
        public static readonly int[] Brattelva_Circuit_Reverse = { 394, 395, 431, 429 };
        public static readonly int[] Alice_Mountain = { 263, 264, 291, 293 };
        public static readonly int[] Alice_Mountain_Reverse = { 264, 291, 292 };
        public static readonly int[] Forgotten_Coast = { 333, 334, 366, 368 };
        public static readonly int[] Forgotten_Coast_Reverse = { 334, 335, 365, 367 };
        public static readonly int[] Maria_Tide = { 32, 33, 34, 35, 36, 68, 69, 70, 71, 292, 293 };
        public static readonly int[] Maria_Tide_Reverse = { 216, 217, 218, 219, 220, 221, 253, 254, 255, 256, 257, 258, 323, 325 };
        public static readonly int[] Europa = { 359, 386, 387 };
        public static readonly int[] Europa_Reverse = { 80, 81, 212, 213, 418, 444, 445 };
        public static readonly int[] Utah_Dam = { 520, 550 };
        public static readonly int[] Utah_Dam_Reverse = { 549 };
        public static readonly int[] Nova_Park = { 342, 385 };
        public static readonly int[] Nova_Park_Reverse = { 343, 386 };
        public static readonly int[] Vespera_Heights = { 449, 477 };
        public static readonly int[] Vespera_Heights_Reverse = { 149, 150, 151, 170, 176, 177, 178, 472, 500 };
        public static readonly int[] Pines_Complex_Course_1 = { 328, 329, 367 };
        public static readonly int[] Pines_Complex_Course_1_Reverse = { 327, 364, 366 };
        public static readonly int[] Pines_Complex_Course_2 = { 139, 140, 405, 570, 571, 588, 589, 627 };
        public static readonly int[] Pines_Complex_Course_2_Reverse = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 451, 452, 555, 556, 635, 637 };
        public static readonly int[] Pines_Complex_Course_3 = { 35, 36, 37, 623, 624, 625, 686, 687, 725 };
        public static readonly int[] Pines_Complex_Course_3_Reverse = { 54, 55, 56, 255, 256, 257, 567, 568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 591, 592, 642, 643, 644, 722, 724 };
        public static readonly int[] Basis_Divide = { 386, 387, 430 };
        public static readonly int[] Basis_Divide_Reverse = { 384, 427 };
        public static readonly int[] Gullfoss_Run = { 304, 334 };
        public static readonly int[] Gullfoss_Run_Reverse = { 66, 67, 79, 80, 331 };
        public static readonly int[] Oceana = { 189, 190, 191, 192, 193, 194, 195, 196, 197, 262, 431, 432, 456, 458 };
        public static readonly int[] Oceana_Reverse = { 189, 238, 433, 460 };
        public static readonly int[] Omega_Loop = { 241, 290, 291, 292, 310, 384, 414, 645, 676, 678 };
        public static readonly int[] Omega_Loop_Reverse = { 229, 259, 333, 382, 383, 384, 645, 678 };
        public static readonly int[] Maceno_Bay = { 11, 274, 275, 310, 311 };
        public static readonly int[] Maceno_Bay_Reverse = { 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 308, 309 };
        public static readonly int[] Therma_Fumos = { 331, 369, 370 };
        public static readonly int[] Therma_Fumos_Reverse = { 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 331, 332, 369, 370 };
        public static readonly int[] Maceno_Interchange = { 230, 231, 232, 233, 234, 235, 236, 237, 368, 369, 370, 371, 372, 373, 374, 434, 435, 467, 468 };
        public static readonly int[] Maceno_Interchange_Reverse = { 44, 45, 47, 53, 54, 55, 56, 176, 181, 182, 183, 184, 185, 186, 436, 437, 469, 470 };
        public static readonly int[] Freyr_Falls = { 14, 15, 16, 75, 76, 77, 82, 83, 84, 95, 96, 97, 178, 261, 262, 263, 264, 296, 297, 298, 303, 304, 305, 306, 521, 541, 542, 551, 552, 553, 579, 580, 581, 587, 588, 589, 593, 594, 595, 602, 603, 668, 670 };
        public static readonly int[] Freyr_Falls_Reverse = { 9, 10, 16, 17, 18, 23, 24, 31, 32, 59, 60, 70, 71, 261, 305, 306, 307, 313, 314, 315, 504, 514, 515, 528, 529, 534, 535, 536, 615, 616, 666, 668 };
        public static readonly int[] Alto_Overseer = { 586, 616, 617, 618, 619 };
        public static readonly int[] Alto_Overseer_Reverse = { 586, 587, 588, 589, 619 };
        public static readonly int[] Maceno_Peak = { 503, 504, 528, 529, 530, 531 };
        public static readonly int[] Maceno_Peak_Reverse = { 505, 506, 507, 508, 532, 534 };
        public static readonly int[] Vega_Square = { 211, 212, 213, 243, 244 };
        public static readonly int[] Vega_Square_Reverse = { 217, 218, 248, 249 };
        public static readonly int[] Route_05 = { 446, 476, 477, 478 };
        public static readonly int[] Route_05_Reverse = { 443, 444, 475, 476 };
        public static readonly int[] Thunderhead = { 168, 309, 348, 349 };
        public static readonly int[] Thunderhead_Reverse = { 350, 386, 389 };
        public static readonly int[] Basin = { 496, 497, 527, 528 };
        public static readonly int[] Basin_Reverse = { 86, 87, 88, 446, 447, 448, 461, 463, 493, 494 };
        public static readonly int[] Metro = { 391, 392, 393, 394, 395, 396, 397, 398, 559, 560, 590, 591 };
        public static readonly int[] Metro_Reverse = { 34, 35, 36, 222, 223, 224, 242, 243, 244, 376, 377, 378, 399, 400, 401, 458, 459, 460, 514, 515, 545, 546 };
        public static readonly int[] Caldera = { 361, 362, 394, 395 };
        public static readonly int[] Caldera_Reverse = { 362, 394, 395 };
        public static readonly int[] Project_9 = { 554, 555, 584, 586 };
        public static readonly int[] Project_9_Reverse = { 584, 585 };
        public static readonly int[] Haze = { 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 301, 302, 303, 305, 306, 330, 331, 332, 333, 495, 496, 533, 534 };
        public static readonly int[] Haze_Reverse = { 87, 88, 159, 160, 186, 187, 188, 189, 190, 231, 232, 240, 241, 242, 243, 244, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 533, 534 };
        public static readonly int[] Helios_Descent = { 294, 295, 296, 297, 303, 304, 305, 659, 660, 699, 701 };
        public static readonly int[] Helios_Ascent = { 346, 347, 348, 349, 354, 355, 356, 654, 655, 694, 696 };
        public static readonly int[] Port_Ares = { 109, 138, 139, 173, 176, 225, 226, 285, 286, 287, 288, 289, 290, 291, 292, 358, 360, 479, 514, 515 };
        public static readonly int[] Port_Ares_Reverse = { 116, 117, 118, 187, 188, 189, 190, 248, 249, 478, 479, 513, 514 };
        public static readonly int[] GT_Special_Stage_City = { 80, 81, 82, 241, 242, 243, 267, 268, 269, 361, 362, 363, 1012 };
        public static readonly int[] GT_Special_Stage_City_Reverse = { 460, 1107, 1108, 1109, 1201, 1202, 1203, 1226, 1227, 1228, 1388, 1389, 1390 };
        public static readonly int[] GT_Autumn_Ring = { 166 };
        public static readonly int[] GT_Autumn_Ring_Reverse = { 298, 439 };
        public static readonly int[] GT_Autumn_Ring_Mini = { 92, 93, 94, 160, 161, 162, 163, 164, 165, 229, 230, -721 };
        public static readonly int[] GT_Autumn_Ring_Mini_Reverse = { 30, 95, 96, 97, 98, 99, 100 };
        public static readonly int[] GT_Clubman_Stage_Route_5 = { 435, 438 };
        public static readonly int[] GT_Clubman_Stage_Route_5_Reverse = { 17 };
        public static readonly int[] GT_High_Speed_Ring = { -464, -487, 59, 60, 61, 88, 89, 90, 120, 121, 122, 193, 194, 195, 291, 292, 293, 320, 321, 322, 365, 366, 367, 375, 376, 377, 388, 389, 390, 409, 410, 411 };
        public static readonly int[] GT_High_Speed_Ring_Reverse = { 17, 35, 36, 37, 57, 58, 59, 70, 71, 72, 80, 81, 82, 125, 126, 127, 154, 155, 156, 252, 253, 254, 325, 326, 327, 357, 358, 359, 385, 386, 387 };
        public static readonly int[] GT_Special_Stage_Route_11_GT1 = { 80, 81, 82, 241, 242, 243, 267, 268, 269, 361, 362, 363, 368 };
        public static readonly int[] GT_Special_Stage_Route_11_GT1_Reverse = { 409, 410, 411, 503, 504, 505, 529, 530, 531, 535, 690, 691, 692, };
        public static readonly int[] GT_Special_Stage_Route_11_GT3 = { 80, 81, 82 };
        public static readonly int[] GT_Special_Stage_Route_11_GT3_Reverse = { 629, 630, 631 };
        public static readonly int[] GT_Special_Stage_Route_5 = { 496, 497, 498, 508, 509, 510, 684, 687 };
        public static readonly int[] GT_Special_Stage_Route_5_reverse = { 17, 194, 195, 196, 206, 207, 208 }; //lowercase r in reverse intentional
        public static readonly int[] Harvest_Bastion = { 319, 327, 338, 353, 371, 381, 400, 404, 405, 417, 418, 442, 449, 450, 468, 469, 520, 521, 610, 644 };
        public static readonly int[] Trinity_Coast_Reverse = { 215, 216, 217, 218, 480 };
        public static readonly int[] Ceres_Waypoint = { 335, 486, 487, 488, 489, 490, 491, 492, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, };
        public static readonly int[] Ceres_Waypoint_Reverse = { 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, /*544, 545,*/ 4, 5, 6, 7, 8, 9, 10, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160 };
        public static readonly int[] Gemini_Glacier = { 60, 154, 290, 291, 420, 421, 422, 423, 424, 460, 461, 462, 463 };
        public static readonly int[] Gemini_Glacier_Reverse = { 123, 124, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 419, 420, 421, 422, 423, 458, 459, 460, 461, 462 };
        public static readonly int[] GT_Deep_Forest = { 115, 116, 117, 257, 258, 259, 441, 442 };
        public static readonly int[] GT_Deep_Forest_reverse = { 47, 230, 231, 232, 372, 373, 374, }; //lowercase r in reverse intentional
        public static readonly int[] GT_Grand_Valley_East_Section = { 150, 203, 296, 332, 333, 334, 404, 405, 406, 428, 430 };
        public static readonly int[] GT_Grand_Valley_East_Section_Reverse = { 65, 66, 67, 100, 101, 102, 196, 197, 198, 402, 403, 404, 425, 426, 427 };
        public static readonly int[] GT_Grand_Valley_Speedway = { 311, 312, 313, 403, 404, 405, 626, 696, 697, 698, 720, 722 };
        public static readonly int[] GT_Grand_Valley_Speedway_Reverse = { 286, 378, 379, 380, 539, 694, 695, 716, 717, 718 };
        public static readonly int[] GPL_Montjuic_69 = { 587, 588, 617, 618, 619, 620, 621, 622, 623 };
        public static readonly int[] Pier_97 = { 64, 65, 66, 67, -68, -69, -70, -71, -72, -73, 84, 85, 86, 87, -100, -101, -102, -103, -104, -105, -106, -107, 212, 213, 214, 277, 278, 279, -460, -461, -462, -463, -464, -465, -466, -467, -468, -388, 359, 360, 361, 362, 363, 387, 388, 389, 390, 391, 392, -522, -523, -524, -525, -526, -452, -453, -454, -455, -456, -457, -458, -459, -557, -558, -559, -560, -561, -562, -563, -564, -565, -566, -947, -948, -949, -950, -951, -952, -953, -954, -955, -956, -957, -958, -959, -960, -961, -962, -963, -964 };
        public static readonly int[] bngl_harpstone_5 = { 93, 94, 166, 206 };
        public static readonly int[] bngl_harpstone_expert_5 = { 171, 172, 273, 274, 275, 276, 277, 278, 317 };
        public static readonly int[] BNGL_Utah_Project_6 = { 20, 21, 491 };
        public static readonly int[] bngl_xl_prototype_2 = { 0, 192, 193, 302, 303, 403 };
        public static readonly int[] bngl_xl_prototype_2_zone_edition = { 0, 192, 193, 302, 303, 403 };
        public static readonly int[] bngl_0x002_2 = { 0, 244, 245, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 391 };
        public static readonly int[] bngl_0x001_1 = { 146, 147, 148, 149, 150 };
        public static readonly int[] bngl_0x001_2 = { 150, 151, 152, 153, 158, 159, 160, 226, 227, 335 };
        public static readonly int[] bngl_0x001_5 = { 457, 482 };
        public static readonly int[] bngl_0x002_5 = { 378, 379, 411, 412 };
        public static readonly int[] BNGL_0x002_6 = { 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 244, 245, 257, 258, 288, 289, 303, 304, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 355, 356, 362, 363 };
        public static readonly int[] bngl_0x003_2 = { 0, 1, 21, 22, 37, 38, 54, 55, 72, 73, 121, 122, -150, -149, -506, -507, 130, 131, 146, 147, 157, 158, 190, 191, 264, 265, 266, 267 };
        public static readonly int[] BNGL_0x003_6 = { 209, 210, 218, 219, 228, 229, 369 };
        public static readonly int[] bngl_aciknovae_1 = { 40, 41, 42, 43, 44, 45, 132, 133, 134, 135, 156, 157, 158, 159, 183, 184, 185, 186, 187, 209, 210, 211, 212, 215, 216, 217, 218, 231 };
        public static readonly int[] bngl_aciknovae_5 = { 53, 54, 55, 56, 192, 193, 194, 195, 225, 226, 227, 228, 229, 280, 281, 343, 371 };
        public static readonly int[] BNGL_Aciknovae_6 = { 4, 5, 110, 111, 228, 229, 286, 287, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 394, 395, 396, 477, 478, 479, 632 };
        public static readonly int[] bngl_aciknovae_8 = { 4, 5, 110, 111, 228, 229, 286, 287, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 394, 395, 396, 477, 478, 479, 632 };
        public static readonly int[] bngl_aciknovae_OG = { 108, 109, 390, 391, 457, 458, 566, 567, 686, 687 };
        public static readonly int[] bngl_arrivon_example = { 281, 304 };
        public static readonly int[] bngl_arrivon_peak_94 = { 8, 9, 10, 21, 22, 23, 91, 92, 93, 211, 212, 230, 231, 232, 233, 234, 302, 303, 304, 305 };
        public static readonly int[] bngl_arrivonxi_6 = { 359, 360, 361, 453 };
        public static readonly int[] bngl_atlantica_5 = { 70, 71, 219, 250, 286 };
        public static readonly int[] bngl_atlanticaOG = { 34, 35, 144, 145, 167, 168, 169 };
        public static readonly int[] bngl_cassandra_5 = { 162, 181 };
        public static readonly int[] BNGL_Dover_beta = { 425, 426, 489 };
        public static readonly int[] bngl_dtt_open = { 15, 16, 17, -17, 26, 27, 28, 46, 47, 48, 192, 193, -201, 230, 239, 240, 241, 343, 344, 345, 348, 368, 369, 372, 373, 374, -403, -760, -1055 };
        public static readonly int[] bngl_dtt_advanced = { 15, 16, 17, 192, 193, 348, 372, 373, 374 };
        public static readonly int[] bngl_dtt_oval = { 141 };
        public static readonly int[] bngl_dtt_expert = { 15, 16, 17, 46, 47, 48, 343, 344, 345, 368, 369 };
        public static readonly int[] bngl_dtt_intermediate = { 15, 16, 17, 26, 27, 28, 230, 239, 240, 241 };
        public static readonly int[] bngl_gare_d_europa_remake_1 = { 363, 339, 340 };
        public static readonly int[] bngl_handling_tutorial_7 = { 171, 172 };
        public static readonly int[] bngl_harpstone_3 = { -197, -217 };
        public static readonly int[] bngl_harpstone_6 = { 202, 203 };
        public static readonly int[] bngl_harpstone_9 = { 202, 203 };
        public static readonly int[] bngl_harpstone_open_5 = { 93, 94, 165, 166, 185, 206, -208, -307, -308, -409, -410, -411, -412 };
        public static readonly int[] bngl_ishtar_citadel_1 = { 15, 16, 301 };
        public static readonly int[] bngl_ishtar_citadel_2 = { 0, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 398 };
        public static readonly int[] bngl_ishtar_citadel_4 = { 29, 30, 31, 160, 161 };
        public static readonly int[] BNGL_Ishtar_Citadel_5 = { 363, 364, 454, 455 };
        public static readonly int[] bngl_ishtar_citadel_7 = { 316, 331 };
        public static readonly int[] bngl_ishtar_citadel_9 = { 316, 331 };
        public static readonly int[] bngl_kahawai_bay_1 = { 31, 32, 77, 112, 151, 152, 255, 256 };
        public static readonly int[] bngl_kahawai_bay_3 = { 46, 47, 114, 115, 116, 117, 118, 149, 150, 151, 152, 169, 170, 171, 172 };
        public static readonly int[] bngl_korodera_remake_2 = { 72, 73, 74, 103, 104, 105, 112, 122, 123, 124, 352, 353, 515, 516, 517, 518 };
        public static readonly int[] bngl_lujiazui_park_9 = { 2, 3 };
        public static readonly int[] bngl_luna_1 = { 0, 78, 79, 124, 125 };
        public static readonly int[] bngl_luna_2 = { 0, 78, 79, 124, 125 };
        public static readonly int[] bngl_luna_7 = { 192, 193, 350, 351 };
        public static readonly int[] bngl_luna_prototype_6 = { 248, 250, 269, 270, 453, 454, 569 };
        public static readonly int[] bngl_maceno_climb = { 620, 621, 622 };
        public static readonly int[] bngl_maceno_reef_5 = { 281 };
        public static readonly int[] bngl_marina_rush_4 = { 230, 231 };
        public static readonly int[] bngl_marina_rush_4_VR = { 230, 231 };
        public static readonly int[] bngl_mission = { -487, -511, -512 };
        public static readonly int[] BNGL_Nova_Split_7 = { 41, 42, 56, 57, 77, 78, 91, 92, 108, 109, 336, 337, 405, 406, 429 };
        public static readonly int[] bngl_nova_split_9_unused = { 78, 79, 94, 95, 112, 113, 124, 125, 142, 143, 221, 222, 223, 224, 231, 232, 233, 234, 332, 355, 356, 357, 358 };
        public static readonly int[] bngl_omega_harbor_6 = { 120, 121, 267, 268, 278, 279, 717, 760 };
        public static readonly int[] bngl_omega_harbor_7 = { 505, 506, 512, 513, 519, 520, 529, 530 };
        public static readonly int[] bngl_pr0x001_93 = { 103, 104, 166, 167, 177, 178, 277, 278, 358, 359, 454, 455 };
        public static readonly int[] bngl_pr0x001_94 = { 361, 362, 427, 428 };
        public static readonly int[] bngl_pr0x002_94 = { 31, 32, 50, 51, 190, 191 };
        public static readonly int[] bngl_pr0x003_94 = { 323, 324, 445, 446 };
        public static readonly int[] bngl_pr0x004_94 = { 275, 276, 277, 278, 279, 280, 328, 329, 366, 367 };
        public static readonly int[] bngl_speed_stadium_7 = { 99, 100 };
        public static readonly int[] bngl_spilskinanke_remake_2 = { 82, 136, 137, 148, 149, 159, 160, 203, 204, 205, 206, 207, 208, 214, 215, 216, 217, 218, 219, 224, 225, 226, 227, 228, 306, 307, 334, 335 };
        public static readonly int[] bngl_stanza_inter_remake_2 = { 76, 99, 100, 101, 102, 103, 113, 114, 115, 116, 117, 118, 128, 129, 130, 131, 132, 156 };
        public static readonly int[] bngl_track09_6 = { 263, 264, 377, 378, 445, 446, 525, 552 };
        public static readonly int[] bngl_utah_project_1_1 = { 85, 86, 87, 149, 150 };
        public static readonly int[] bngl_utah_project_2 = { 315 };
        public static readonly int[] bngl_utah_project_4 = { 316 };
        public static readonly int[] bngl_utah_project_7 = { 155, 156, 288, 305 };
        public static readonly int[] bngl_utah_project_9 = { 155, 156, 288, 305 };
        public static readonly int[] bngl_utah_project_wip1 = { 290 };
        public static readonly int[] bngl_utah_example = { 137, 165 };
        public static readonly int[] bngl_zephyr_ridge_3 = { 0, 273 };
        public static readonly int[] bngl_zephyr_ridge_5 = { 0, 273 };
        public static readonly int[] bngl_zephyr_ridge_6 = { 205, 206, 352, 384 };
        public static readonly int[] bngl_zephyr_ridge_8 = { 205, 206, 352, 384 };
        public static readonly int[] bngl_zephyr_ridge_8_unused = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 52, 53, 54, 74, 75, 76, 90, 91, 92, 200, 201, 299, 300, 301, 323, 324, 325, 400, 401, 402, 403, 404, 405, 406 };
        public static readonly int[] POD_Burrow = { 25, 26, 27, 31, 32, 33, 34, 35, 36, 37, 44, 52, 53, 54, 60, 74, 75, 76, 77, 78, 83, 84, 85, 86, 87, 113, 117, 118, 119, 120 };
        public static readonly int[] POD_City = { 24, 35, 49, 53, 74, 75, 76, 77 };
        public static readonly int[] POD_DownTown = { 48, 67, 68, 69, 70, 71, 72, 76, 77, 84, 94 };
        public static readonly int[] POD_Factory = { 0, 12, 19, 22, 26, 49, 59, 101, 102, 122, 123, 128, 129, 130, 151, 152, 161, 196, 242 };
        public static readonly int[] POD_Galleria = { 73, 77, 85, 91, 98, 104, 112, 117, 118, 134, 135, 136, 144, 145, 146, 147, 148, 162, 205, 214, 230 };
        public static readonly int[] POD_Loon = { 2, 3, 4, 5, 15, 16, 17, 18, 19, 20, 21, 22, 23, 26, 27, 28, 32, 33, 34, 58, 59, 60, 68, 69, 70, 75, 76, 77, 78, 79, 80, 101, 113, 126, 127, 153, 154, 160, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 189, 209, 210, 211, 215, 216 };
        public static readonly int[] POD_Magnetic = { 64, 72 };
        public static readonly int[] POD_Nuke = { 110 };
        public static readonly int[] POD_Plant_21 = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 149, 150 };
        public static readonly int[] POD_Pompeii = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 56, 57, 58, 59, 60, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239 };
        public static readonly int[] POD_ZED = { 0, 1, 2, 42, 64, 118, 119, 120, 121, 122, 123, 124, 125, 126, 134, 141, 214 };
        public static readonly int[] POD_VeitLand = { 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 49, 60, 88, 89, 90, 131, 148 };
        public static readonly int[] POD_Road70 = { 18, 61, 62, 65, 66, 72, 73, 74, 81, 86, 91, 96, 102, 103, 107, 113, 118, 132, 137, 143, 148, 155, 160, 167, 172, 178, 208, 209, 221, 222, 223, 224, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 277, 340, 341, 342, 343, 344, -481, 363, 376, 377, 378, 379 };
        public static readonly int[] bngl_harpstone_11 = { 202, 203, 410, 439 };
        public static readonly int[] bngl_harpstone_reverse_11 = { 414, 443 };
        public static readonly int[] bngl_aciknovae_11 = { 594, 632 };
        public static readonly int[] bngl_aciknovae_reverse_11 = { 355, 356, 357, 358, 472, 473, 474, 592, 630 };
        public static readonly int[] bngl_marina_rush_9 = { 269, 297 };
        public static readonly int[] bngl_marina_rush_reverse_9 = { -2, -32, 269, 270, 271, 295, 296, 297, 298 };
        public static readonly int[] bngl_maceno_bay_5 = { 35, 36, 37, 38, 367, 400 };
        public static readonly int[] Zephyr_Climb = { 62, 63, 64, 65, 66, 147, 148, 149, 150, 151, 172, 173, 174, 175, 208, 209, 210 };
        public static readonly int[] Zephyr_Climb_Reverse = { 133, 134, 135, 136, 137, 281, 282, 283, 304, 305, 306, 353, 354, 355, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397 };
        public static readonly int[] Hel_II = { 49, 50, 109, 110, 111, 227, 243, 379, 380 };
        public static readonly int[] Kena_Divide = { 140, 141, 211, 212, 213, 319, 358 };
        public static readonly int[] Shalbatana = { 226, 227, 238, 239, 246, 247, 253, 254, 270, 297 };
        public static readonly int[] Track1 = { 18, 19, 20, 342, 364 }; //This track was originally going to be called "Hyades Cluster"
        public static readonly int[] ES_0x022_Sangreal = { 18, 19, 31, 32, 142, 143, 198, 199 };
        public static readonly int[] ES_0x029_Veira_Kaan = { 268, 269, 314, 315 };
        public static readonly int[] ES_0x043_Cerridan_III = { 67, 68, 313 };
        public static readonly int[] ES_0x048_Hengsha_Financial = { 14, 15, 180, 181 };
        public static readonly int[] ES_0x086_Arcadion = { 16, 17, 25, 26, 248, 249, 273, 274, 424 };
        public static readonly int[] map15 = { 100, 176, 187, 188, 189, 203, 244, 255, 283, 299, 350 };
        public static readonly int[] Regn_Alta = { 0, 62, 63, 64, 222, 223, 475 };
        public static readonly int[] Stenza_Line = { 70, 71, 72, 165, 166, 167, 239, 240, 241, 251, 252, 253, 267, 268, 269 };
        public static readonly int[] Track07 = { 33, 34, 35, 101, 102, 103, 166, -225, 213, 214, 215, 216, 344, 345, 359, 360, 392 };
        public static readonly int[] Distant_Shore = { 8, 9, 10, 11, 391, 392, 393, 394, 395, 396, 572, 595, /*82, 83, 84,*/ 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167 };
        public static readonly int[] Azure_Heights_1F = { 449, 2, 3 }; //DUMMY DATA
        public static readonly int[] Azure_Heights_1R = { 441, 5, 7 }; //DUMMY DATA
        public static readonly int[] Azure_Heights_2F = { 662, 11, 13 }; //DUMMY DATA
        public static readonly int[] Azure_Heights_2R = { 661, 17, 19 }; //DUMMY DATA
        public static readonly int[] Azure_Heights_3F = { 872, 23, 29 }; //DUMMY DATA
        public static readonly int[] Azure_Heights_3R = { 871, 31, 37 }; //DUMMY DATA
        public static readonly int[] Accelerator_Ring = { 261, 41, 43 }; //DUMMY DATA
        public static readonly int[] Druidia = { 502, 47, 53 }; //DUMMY DATA
        public static readonly int[] Cassini_Dome = { 386, 59, 61 }; //DUMMY DATA
        public static readonly int[] Pretoria_Stadium = { 458, 67, 71 }; //DUMMY DATA
        public static readonly int[] Pretoria_Stadium_Reverse = { 455, 73, 79 }; //DUMMY DATA
        public static readonly int[] Vostrukha_I = { 452, 83, 89 }; //DUMMY DATA
        public static readonly int[] ES_0x077_Nightmareland = { 724, 97, 101 }; //DUMMY DATA
        public static readonly int[] Helixx = { 456, 103, 107 }; //DUMMY DATA
        public static readonly int[] Highway35 = { 608, 109, 113 }; //DUMMY DATA
        public static readonly int[] Interlagos_Futuristic = { 1122, 127, 131 }; //DUMMY DATA
        public static readonly int[] Project_2 = { 218, 137, 139 }; //DUMMY DATA
        public static readonly int[] Rally = { 113, 114, 115, 116, 117, 191, 192, 193, 194, 195, 196, 197, 217, 218, 219, 220, 221, 222, 223, 224, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 281, 282, 283, 284, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323 };

        public bool Needs_Jump_Flags_Set_To_Normal_Boolean;
        public bool Use_Track_Creator_Defined_Jump_Flags;
        public bool ZeroGravityTrack; //Currently specific to Ceres Waypoint + Reverse, Kuiper Overturn doesn't seem to need manual definition.
        public bool FloorhuggerTrack;
        public HashSet<int> Cached_Jump_Flag_Definitions;
        public static int[] Cached_Section_Definitions = Array.Empty<int>();

        public int Physics_Mode;
        public bool Is_2159;
        public bool Forcing_NoTiltLock;
        public int Camera_Behavior_Mode_2280 = VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280;
        public Transform Camera_Transform;
        public NgTrackData.Section Current_Section;
        public ShipSim Cached_ShipSim;

        public bool Jump_Flag_Set_For_This_Section;

        //public bool In_Override_Normal;
        public bool In_Override_Jump;

        //public bool In_Creator_Defined_Normal;
        public bool In_Creator_Defined_Jump;

        public int[] Manually_Set_Section_Jump_Flags(string TrackName)
        {
            switch (TrackName)
            {
                default:
                    Needs_Jump_Flags_Set_To_Normal_Boolean = false;
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Array.Empty<int>();

                case "Cassandra":
                    return Cassandra;

                case "Cassandra Reverse":
                    return Cassandra_Reverse;

                case "BNGL-Arrivon Falls 8":
                    return BNGL_Arrivon_Falls_8;
                    
                case "BNGL-Oceana 8":
                    return BNGL_Oceana_8;
                    
                case "Helheim":
                    return Helheim;

                case "Alphard Reverse":
                    return Alphard_Reverse;

                case "Samrong Crossing Reverse":
                    return Samrong_Crossing_Reverse;

                case "Blackward Decks":
                    return Blackward_Decks;

                case "Great Manitou Trail":
                    return Great_Manitou_Trail;

                case "Vestfjorden":
                    return Vestfjorden;

                case "Abyssus":
                    return Abyssus;

                case "Aeolus II":
                    return Aeolus_II;

                case "Altima XIV":
                    return Altima_XIV;

                case "Antelao":
                    Needs_Jump_Flags_Set_To_Normal_Boolean = true;
                    return Antelao;

                case "Antelao Reverse":
                    return Antelao_Reverse;

                case "Beltane":
                    return Beltane;

                case "Cairodrome":
                    return Cairodrome;

                case "Cairodrome II":
                    return Cairodrome_II;

                case "Cairodrome II Reverse":
                    return Cairodrome_II_Reverse;

                case "Canyonlands":
                    return Canyonlands;

                case "Canyonlands Reverse":
                    return Canyonlands_Reverse;

                case "Dione IV":
                    return Dione_IV;

                case "Elivagar Reverse":
                    return Elivagar_Reverse;

                case "Eschaton":
                    return Eschaton;

                case "Gehennom":
                    return Gehennom;

                case "Herne Park":
                    return Herne_Park;

                case "Kamanneq":
                    return Kamanneq;

                case "Kanlaon Peak":
                    return Kanlaon_Peak;

                case "Lumenar V":
                    return Lumenar_V;

                case "Millennium Wharf": //note the doubled 'n' in 'Millennium'
                    return Millenium_Wharf;

                case "Project Pandora":
                    return Project_Pandora;

                case "Shanghai Financial":
                    return Shanghai_Financial;

                case "South Ridge":
                    return South_Ridge;

                case "Sovereign":
                    return Sovereign;

                case "Sovereign Reverse":
                    return Sovereign_Reverse;

                case "The S.T.J.":
                    return The_STJ;

                case "Dagon Prime":
                    return Dagon_Prime;

                case "Prototype 19-WLD Botanica":
                    return Prototype_19_WLD_Botanica;

                case "Prototype 77-ODR Nachtmahr":
                    return Prototype_77_ODR_Nachtmahr;

                case "Prototype 81-JMT Lambda2Phi":
                    return Prototype_81_JMT_Lambda2Phi;

                case "Prototype 99-KEM Sunthrone":
                    return Prototype_99_KEM_Sunthrone;

                case "ES-0x016 Germania":
                    return ES_0x016_Germania;

                case "ES-0x033 Dione II":
                    return ES_0x033_Dione_II;

                case "Annapurna":
                    return Annapurna;

                case "Desolata":
                    return Desolata;

                case "Infinity Spear":
                    return Infinity_Spear;

                case "bngl-metro 11":
                    return bngl_metro_11;

                case "bngl-metro reverse 11":
                    return bngl_metro_reverse_11;

                case "bngl-nova split 11":
                    return bngl_nova_split_11;

                case "bngl-nova split reverse 11":
                    return bngl_nova_split_reverse_11;

                case "bngl-wipeoutzone":
                    return bngl_wipeoutzone;

                case "DandelionCircuit":
                    return DandelionCircuit;

                case "Astra Magnesium":
                    return Astra_Magnesium;

                case "Serenewoods":
                    return Serenewoods;

                case "Cobbledark":
                    return Cobbledark;

                case "(SW1R) Bumpy's Breakers":
                    return SW1R_Bumpys_Breakers;

                case "(SW1R) Malastare 100":
                    return SW1R_Malastare_100;

                case "(SW1R) Scrapper's Run":
                    return SW1R_Scrappers_Run;

                case "(SW1R) The Boonta Eve Classic":
                    return SW1R_The_Boonta_Eve_Classic;

                case "(SW1R) Boonta Eve Training Course":
                    return SW1R_Boonta_Eve_Training_Course;

                case "(SW1R) Aquilaris Classic":
                    return SW1R_Aquilaris_Classic;

                case "(NFS1) Alpine":
                    return NFS1_Alpine;

                case "(NFS1) Transtropolis":
                    return NFS1_Transtropolis;

                case "Solaris":
                    return Solaris;

                case "Solaris Reverse":
                    return Solaris_Reverse;

                case "Aerodive":
                    return Aerodive;

                case "Escalation Project":
                    Needs_Jump_Flags_Set_To_Normal_Boolean = true;
                    return Escalation_Project;

                case "Kaiten":
                    return Kaiten;

                case "Loch Aberton":
                    return Loch_Aberton;

                case "Parashant":
                    return Parashant;

                case "Stone Run":
                    return Stone_Run;

                case "Trinity Coast":
                    return Trinity_Coast;

                case "wtrp-wo1-korodera":
                    return wtrp_wo1_korodera;

                case "wtrp-wo2097-spilskinanke":
                    return wtrp_wo2097_spilskinanke;

                case "wtrp-wo2097-vostok island":
                    return wtrp_wo2097_vostok_island;

                case "wtrp-wo3-manor top":
                    return wtrp_wo3_manor_top;

                case "moltocaldo desert":
                    return moltocaldo_desert;

                case "Sunken City":
                    return Sunken_City;

                case "Skyline Sprint":
                    return Skyline_Sprint;

                case "Atacama":
                    return Atacama;

                case "Atacama Reverse":
                    return Atacama_Reverse;

                case "Pier 97":
                    return Pier_97;

                case "Aciknovae":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Aciknovae;

                case "Aciknovae Reverse":                    
                    return Aciknovae_Reverse;

                case "Nova Split Reverse":
                    return Nova_Split_Reverse;

                case "Ishtar Citadel":
                    return Ishtar_Citadel;

                case "Ishtar Citadel Reverse":
                    return Ishtar_Citadel_Reverse;

                case "Arrivon XI":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Arrivon_XI;

                case "Arrivon XI Reverse":
                    return Arrivon_XI_Reverse;

                case "Utah Project":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Utah_Project;

                case "Utah Project Reverse":                    
                    return Utah_Project_Reverse;

                case "Marina Rush":
                    return Marina_Rush;

                case "Marina Rush Reverse":
                    return Marina_Rush_Reverse;

                case "Atlantica":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Atlantica;

                case "Atlantica Reverse":                    
                    return Atlantica_Reverse;

                case "Tokyo Bowl":
                    return Tokyo_Bowl;

                case "Tokyo Bowl Reverse":
                    return Tokyo_Bowl_Reverse;

                case "Lujiazui Park Reverse":
                    return Lujiazui_Park_Reverse;

                case "Pocono Circuit":
                    return Pocono_Circuit;

                case "Pocono Circuit Reverse":
                    return Pocono_Circuit_Reverse;

                case "Arrivon Falls":
                    return Arrivon_Falls;

                case "Arrivon Falls Reverse":
                    return Arrivon_Falls_Reverse;

                case "Meads Pass":
                    return Meads_Pass;

                case "Meads Pass Reverse":
                    return Meads_Pass_Reverse;

                case "Pacheon":
                    return Pacheon;

                case "Pacheon Reverse":
                    return Pacheon_Reverse;

                case "Hydrome Bed":                    
                    return Hydrome_Bed;

                case "Arrivon Peak":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Arrivon_Peak;

                case "Brattelva Circuit":
                    return Brattelva_Circuit;

                case "Brattelva Circuit Reverse":
                    return Brattelva_Circuit_Reverse;

                case "Alice Mountain":
                    return Alice_Mountain;

                case "Alice Mountain Reverse":
                    return Alice_Mountain_Reverse;

                case "Forgotten Coast":
                    return Forgotten_Coast;

                case "Forgotten Coast Reverse":
                    return Forgotten_Coast_Reverse;

                case "Maria Tide":
                    return Maria_Tide;

                case "Maria Tide Reverse":
                    return Maria_Tide_Reverse;

                case "Europa":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Europa;

                case "Europa Reverse":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Europa_Reverse;

                case "Utah Dam":
                    return Utah_Dam;

                case "Utah Dam Reverse":
                    return Utah_Dam_Reverse;

                case "Nova Park":
                    return Nova_Park;

                case "Nova Park Reverse":
                    return Nova_Park_Reverse;

                case "Vespera Heights":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Vespera_Heights;

                case "Vespera Heights Reverse":                    
                    return Vespera_Heights_Reverse;

                case "Pines Complex Course 1":
                    return Pines_Complex_Course_1;                

                case "Pines Complex Course 1 Reverse":
                    return Pines_Complex_Course_1_Reverse;

                case "Pines Complex Course 2":                    
                    return Pines_Complex_Course_2;

                case "Pines Complex Course 2 Reverse":
                    return Pines_Complex_Course_2_Reverse;

                case "Pines Complex Course 3":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Pines_Complex_Course_3;

                case "Pines Complex Course 3 Reverse":                    
                    return Pines_Complex_Course_3_Reverse;

                case "Basis Divide":
                    return Basis_Divide;

                case "Basis Divide Reverse":
                    return Basis_Divide_Reverse;

                case "Gullfoss Run":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Gullfoss_Run;

                case "Gullfoss Run Reverse":
                    return Gullfoss_Run_Reverse;

                case "Oceana":
                    return Oceana;

                case "Oceana Reverse":
                    return Oceana_Reverse;

                case "Omega Loop":
                    return Omega_Loop;

                case "Omega Loop Reverse":
                    return Omega_Loop_Reverse;

                case "Maceno Bay":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Maceno_Bay;

                case "Maceno Bay Reverse":
                    return Maceno_Bay_Reverse;

                case "Therma Fumos":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Therma_Fumos;

                case "Therma Fumos Reverse":                    
                    return Therma_Fumos_Reverse;

                case "Maceno Interchange":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Maceno_Interchange;

                case "Maceno Interchange Reverse":                    
                    return Maceno_Interchange_Reverse;

                case "Freyr Falls":                    
                    return Freyr_Falls;

                case "Freyr Falls Reverse":                    
                    return Freyr_Falls_Reverse;

                case "Alto Overseer":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Alto_Overseer;

                case "Alto Overseer Reverse":                    
                    return Alto_Overseer_Reverse;

                case "Maceno Peak":
                    return Maceno_Peak;

                case "Maceno Peak Reverse":
                    return Maceno_Peak_Reverse;

                case "Vega Square":
                    return Vega_Square;

                case "Vega Square Reverse":
                    return Vega_Square_Reverse;

                case "Route 05":
                    return Route_05;

                case "Route 05 Reverse":
                    return Route_05_Reverse;

                case "Thunderhead":
                    return Thunderhead;

                case "Thunderhead Reverse":
                    return Thunderhead_Reverse;

                case "Basin":
                    return Basin;

                case "Basin Reverse":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Basin_Reverse;

                case "Metro":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Metro;

                case "Metro Reverse":
                    return Metro_Reverse;

                case "Caldera":
                    return Caldera;

                case "Caldera Reverse":
                    return Caldera_Reverse;

                case "Project 9":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Project_9;

                case "Project 9 Reverse":
                    return Project_9_Reverse;

                case "Haze":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Haze;

                case "Haze Reverse":
                    return Haze_Reverse;

                case "Helios Descent":
                    return Helios_Descent;

                case "Helios Ascent":
                    return Helios_Ascent;

                case "Port Ares":
                    return Port_Ares;

                case "Port Ares Reverse":
                    return Port_Ares_Reverse;

                case "GT-Special Stage City":
                    return GT_Special_Stage_City;

                case "GT-Special Stage City Reverse":
                    return GT_Special_Stage_City_Reverse;

                case "GT-Autumn Ring":
                    return GT_Autumn_Ring;

                case "GT-Autumn Ring Reverse":
                    return GT_Autumn_Ring_Reverse;

                case "GT-Autumn Ring Mini":
                    return GT_Autumn_Ring_Mini;

                case "GT-Autumn Ring Mini Reverse":
                    return GT_Autumn_Ring_Mini_Reverse;

                case "GT-Clubman Stage Route 5":
                    return GT_Clubman_Stage_Route_5;

                case "GT-Clubman Stage Route 5 Reverse":
                    return GT_Clubman_Stage_Route_5_Reverse;

                case "GT-High Speed Ring":
                    return GT_High_Speed_Ring;

                case "GT-High Speed Ring Reverse":
                    return GT_High_Speed_Ring_Reverse;

                case "GT-Special Stage Route 11 GT1":
                    return GT_Special_Stage_Route_11_GT1;

                case "GT-Special Stage Route 11 GT1 Reverse":
                    return GT_Special_Stage_Route_11_GT1_Reverse;

                case "GT-Special Stage Route 11 GT3":
                    return GT_Special_Stage_Route_11_GT3;

                case "GT-Special Stage Route 11 GT3 Reverse":
                    return GT_Special_Stage_Route_11_GT3_Reverse;

                case "GT-Special Stage Route 5":
                    return GT_Special_Stage_Route_5;

                case "GT-Special Stage Route 5 reverse":
                    return GT_Special_Stage_Route_5_reverse;

                case "Harvest Bastion":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Harvest_Bastion;

                case "Trinity Coast Reverse":
                    return Trinity_Coast_Reverse;

                case "Ceres Waypoint":
                    return Ceres_Waypoint;

                case "Ceres Waypoint Reverse":
                    return Ceres_Waypoint_Reverse;

                case "Gemini Glacier":
                    return Gemini_Glacier;

                case "Gemini Glacier Reverse":
                    return Gemini_Glacier_Reverse;

                case "GT-Deep Forest":
                    return GT_Deep_Forest;

                case "GT-Deep Forest reverse":
                    return GT_Deep_Forest_reverse;

                case "GT-Grand Valley East Section":
                    return GT_Grand_Valley_East_Section;

                case "GT-Grand Valley East Section Reverse":
                    return GT_Grand_Valley_East_Section_Reverse;

                case "GT-Grand Valley Speedway":
                    return GT_Grand_Valley_Speedway;

                case "GT-Grand Valley Speedway Reverse":
                    return GT_Grand_Valley_Speedway_Reverse;

                case "[GPL] Montjuic '69":
                    return GPL_Montjuic_69;

                case "bngl-harpstone 5":
                    return bngl_harpstone_5;

                case "bngl-harpstone expert 5":
                    return bngl_harpstone_expert_5;

                case "BNGL-Utah Project 6":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return BNGL_Utah_Project_6;

                case "bngl-xl prototype 2":
                    return bngl_xl_prototype_2;

                case "bngl-xl prototype 2 zone edition":
                    return bngl_xl_prototype_2_zone_edition;

                case "bngl-0x002 2":
                    return bngl_0x002_2;

                case "bngl-0x001 1":
                    return bngl_0x001_1;

                case "bngl-0x001 2":
                    return bngl_0x001_2;

                case "bngl-0x001 5":
                    return bngl_0x001_5;

                case "bngl-0x002 5":
                    return bngl_0x002_5;

                case "BNGL-0x002 6":
                    return BNGL_0x002_6;

                case "bngl-0x003 2":
                    return bngl_0x003_2;

                case "BNGL-0x003 6":
                    return BNGL_0x003_6;

                case "bngl-aciknovae 1":
                    return bngl_aciknovae_1;

                case "bngl-aciknovae 5":
                    return bngl_aciknovae_5;

                case "BNGL-Aciknovae 6":
                    return BNGL_Aciknovae_6;

                case "bngl-aciknovae 8":
                    return bngl_aciknovae_8;

                case "bngl-aciknovae OG":
                    return bngl_aciknovae_OG;

                case "bngl-arrivon example":
                    return bngl_arrivon_example;

                case "bngl-arrivon peak 94":
                    return bngl_arrivon_peak_94;

                case "bngl-arrivonxi 6":
                    return bngl_arrivonxi_6;

                case "bngl-atlantica 5":
                    return bngl_atlantica_5;

                case "bngl-atlanticaOG":
                    return bngl_atlanticaOG;

                case "bngl-cassandra 5":
                    return bngl_cassandra_5;

                case "BNGL-Dover beta":
                    return BNGL_Dover_beta;

                case "bngl-dtt open":
                    return bngl_dtt_open;

                case "bngl-dtt advanced":
                    return bngl_dtt_advanced;

                case "bngl-dtt oval":
                    return bngl_dtt_oval;

                case "bngl-dtt expert":
                    return bngl_dtt_expert;

                case "bngl-dtt intermediate":
                    return bngl_dtt_intermediate;

                case "bngl-gare d europa remake 1":
                    return bngl_gare_d_europa_remake_1;

                case "bngl-handling tutorial 7":
                    return bngl_handling_tutorial_7;

                case "bngl-harpstone 3":
                    return bngl_harpstone_3;

                case "bngl-harpstone 6":
                    return bngl_harpstone_6;

                case "bngl-harpstone 9":
                    return bngl_harpstone_9;

                case "bngl-harpstone open 5":
                    return bngl_harpstone_open_5;

                case "bngl-ishtar citadel 1":
                    return bngl_ishtar_citadel_1;

                case "bngl-ishtar citadel 2":
                    return bngl_ishtar_citadel_2;

                case "bngl-ishtar citadel 4":
                    return bngl_ishtar_citadel_4;

                case "BNGL-Ishtar Citadel 5":
                    return BNGL_Ishtar_Citadel_5;

                case "bngl-ishtar citadel 7":
                    return bngl_ishtar_citadel_7;

                case "bngl-ishtar citadel 9":
                    return bngl_ishtar_citadel_9;

                case "bngl-kahawai bay 1":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return bngl_kahawai_bay_1;

                case "bngl-kahawai bay 3":
                    return bngl_kahawai_bay_3;

                case "bngl-korodera remake 2":
                    return bngl_korodera_remake_2;

                case "bngl-lujiazui park 9":
                    return bngl_lujiazui_park_9;

                case "bngl-luna 1":
                    return bngl_luna_1;

                case "bngl-luna 2":
                    return bngl_luna_2;

                case "bngl-luna 7":
                    return bngl_luna_7;

                case "bngl-luna prototype 6":
                    return bngl_luna_prototype_6;

                case "bngl-maceno climb":
                    return bngl_maceno_climb;

                case "bngl-maceno reef 5":
                    return bngl_maceno_reef_5;

                case "bngl-marina rush 4":
                    return bngl_marina_rush_4;

                case "bngl-marina rush 4 VR":
                    return bngl_marina_rush_4_VR;

                case "bngl-mission":
                    return bngl_mission;

                case "BNGL-Nova Split 7":
                    return BNGL_Nova_Split_7;

                case "bngl-nova split 9 unused":
                    return bngl_nova_split_9_unused;

                case "bngl-omega harbor 6":
                    return bngl_omega_harbor_6;

                case "bngl-omega harbor 7":
                    return bngl_omega_harbor_7;

                case "bngl-pr0x001 93":
                    return bngl_pr0x001_93;

                case "bngl-pr0x001 94":
                    return bngl_pr0x001_94;

                case "bngl-pr0x002 94":
                    return bngl_pr0x002_94;

                case "bngl-pr0x003 94":
                    return bngl_pr0x003_94;

                case "bngl-pr0x004 94":
                    return bngl_pr0x004_94;

                case "bngl-speed stadium 7":
                    return bngl_speed_stadium_7;

                case "bngl-spilskinanke remake 2":
                    return bngl_spilskinanke_remake_2;

                case "bngl-stanza inter remake 2":
                    return bngl_stanza_inter_remake_2;

                case "bngl-track09 6":
                    return bngl_track09_6;

                case "bngl-utah project 1-1":
                    return bngl_utah_project_1_1;

                case "bngl-utah project 2":
                    return bngl_utah_project_2;

                case "bngl-utah project 4":
                    return bngl_utah_project_4;

                case "bngl-utah project 7":
                    return bngl_utah_project_7;

                case "bngl-utah project 9":
                    return bngl_utah_project_9;

                case "bngl-utah project wip1":
                    return bngl_utah_project_wip1;

                case "bngl-utah example":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return bngl_utah_example;

                case "bngl-zephyr ridge 3":
                    return bngl_zephyr_ridge_3;

                case "bngl-zephyr ridge 5":
                    return bngl_zephyr_ridge_5;

                case "bngl-zephyr ridge 6":
                    return bngl_zephyr_ridge_6;

                case "bngl-zephyr ridge 8":
                    return bngl_zephyr_ridge_8;

                case "bngl-zephyr ridge 8-unused":
                    return bngl_zephyr_ridge_8_unused;

                case "(POD) Burrow":
                    return POD_Burrow;

                case "(POD) City":
                    return POD_City;

                case "(POD) DownTown":
                    return POD_DownTown;

                case "(POD) Factory":
                    return POD_Factory;

                case "(POD) Galleria":
                    return POD_Galleria;

                case "(POD) Loon":
                    return POD_Loon;

                case "(POD) Magnetic":
                    return POD_Magnetic;

                case "(POD) Nuke":
                    return POD_Nuke;

                case "(POD) Plant 21":
                    return POD_Plant_21;

                case "(POD) Pompeii":
                    return POD_Pompeii;

                case "(POD) ZED":
                    return POD_ZED;

                case "(POD) VeitLand":
                    return POD_VeitLand;

                case "(POD) Road70":
                    return POD_Road70;

                case "bngl-harpstone 11":
                    return bngl_harpstone_11;

                case "bngl-harpstone reverse 11":
                    return bngl_harpstone_reverse_11;

                case "bngl-aciknovae 11":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return bngl_aciknovae_11;

                case "bngl-aciknovae reverse 11":
                    return bngl_aciknovae_reverse_11;

                case "bngl-marina rush 9":
                    return bngl_marina_rush_9;

                case "bngl-marina rush reverse 9":
                    return bngl_marina_rush_reverse_9;

                case "bngl-maceno bay 5":
                    return bngl_maceno_bay_5;

                case "Zephyr Climb":
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Zephyr_Climb;

                case "Zephyr Climb Reverse":                    
                    return Zephyr_Climb_Reverse;

                case "Hel II":
                    return Hel_II;

                case "Kena Divide":
                    return Kena_Divide;

                case "Shalbatana":
                    return Shalbatana;

                case "Track1":
                    return Track1; //This track was originally going to be called "Hyades Cluster"

                case "ES-0x022 Sangreal":
                    return ES_0x022_Sangreal;

                case "ES-0x029 Veira Kaan":
                    return ES_0x029_Veira_Kaan;

                case "ES-0x043 Cerridan III":
                    return ES_0x043_Cerridan_III;

                case "ES-0x048 Hengsha Financial":
                    return ES_0x048_Hengsha_Financial;

                case "ES-0x086 Arcadion":
                    return ES_0x086_Arcadion;

                case "map15":
                    return map15;

                case "Regn Alta":
                    return Regn_Alta;

                case "Stenza Line":
                    return Stenza_Line;

                case "Track07":
                    return Track07;

                case "Distant Shore":
                    return Distant_Shore;

                case "Azure Heights 1F":
                    return Azure_Heights_1F;

                case "Azure Heights 1R":
                    return Azure_Heights_1R;

                case "Azure Heights 2F":
                    return Azure_Heights_2F;

                case "Azure Heights 2R":
                    return Azure_Heights_2R;

                case "Azure Heights 3F":
                    return Azure_Heights_3F;

                case "Azure Heights 3R":
                    return Azure_Heights_3R;

                case "Accelerator Ring":
                    return Accelerator_Ring;

                case "Druidia":
                    return Druidia;

                case "Cassini Dome":
                    return Cassini_Dome;

                case "Pretoria Stadium":
                    return Pretoria_Stadium;

                case "Pretoria Stadium Reverse":
                    return Pretoria_Stadium_Reverse;

                case "Vostrukha I":
                    return Vostrukha_I;

                case "ES-0x077 Nightmareland":
                    return ES_0x077_Nightmareland;

                case "Helixx":
                    return Helixx;                

                case "Highway35":
                    return Highway35;

                case "Interlagos Futuristic":
                    return Interlagos_Futuristic;

                case "Project 2":
                    return Project_2;

                case "Rally":
                    return Rally;
            }
        }

        public override void Start()
        {
            base.Start();            

            //Has_Left_Jump_And_Is_Grounded_Boolean = true;
            Physics_Mode = Cheats.IntFromPhysicsMod(); //0 = 2159, 1 = 2280, 2 = Floorhugger            

            Is_2159 = (Physics_Mode == 0);
            Forcing_NoTiltLock = VanillaPlusHUDOptions.ModMenuOptions.ForceNoTiltLock2159;

            Track_Display_Name = ContentManager.GetCurrentTrack().DisplayName;            

            Cached_Section_Definitions = Manually_Set_Section_Jump_Flags(Track_Display_Name);

            if (
                Cached_Section_Definitions == Europa ||
                Cached_Section_Definitions == Europa_Reverse ||
                Cached_Section_Definitions == Pines_Complex_Course_1 ||
                Cached_Section_Definitions == Pines_Complex_Course_1_Reverse ||
                Cached_Section_Definitions == Pines_Complex_Course_2 ||
                Cached_Section_Definitions == Pines_Complex_Course_2_Reverse ||
                Cached_Section_Definitions == Pines_Complex_Course_3 ||
                Cached_Section_Definitions == Pines_Complex_Course_3_Reverse ||
                Cached_Section_Definitions == Port_Ares ||
                Cached_Section_Definitions == Port_Ares_Reverse ||
                Cached_Section_Definitions == Helios_Descent ||
                Cached_Section_Definitions == Helios_Ascent ||
                Cached_Section_Definitions == map15 ||
                Cached_Section_Definitions == Azure_Heights_1F ||
                Cached_Section_Definitions == Azure_Heights_1R ||
                Cached_Section_Definitions == Azure_Heights_2F ||
                Cached_Section_Definitions == Azure_Heights_2R ||
                Cached_Section_Definitions == Azure_Heights_3F ||
                Cached_Section_Definitions == Azure_Heights_3R ||
                Cached_Section_Definitions == Accelerator_Ring ||
                Cached_Section_Definitions == Druidia ||
                Cached_Section_Definitions == Cassini_Dome ||
                Cached_Section_Definitions == Pretoria_Stadium ||
                Cached_Section_Definitions == Pretoria_Stadium_Reverse ||
                Cached_Section_Definitions == Vostrukha_I ||
                Cached_Section_Definitions == ES_0x077_Nightmareland ||
                Cached_Section_Definitions == Helixx ||
                Cached_Section_Definitions == Highway35 ||
                Cached_Section_Definitions == Interlagos_Futuristic ||
                Cached_Section_Definitions == Project_2
                )
            {
                FloorhuggerTrack = true;
            }
            else
            {
                FloorhuggerTrack = false;
            }

            //DebugConsole.Log("|" + Track_Display_Name + "|");
            //DebugConsole.Log($"{NgTrackData.TrackManager.Instance.data.MaxIndex}");

            if (Cached_Section_Definitions == Hel_II)
            {
                MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();

                foreach (MeshRenderer renderer in renderers)
                {
                    string rendererName = renderer.name;
                    if (rendererName == "AnimatedRotator" || rendererName == "BillboardAnimatedRotator")
                    {
                        renderer.enabled = false;
                    }
                }
            }

            Cached_Jump_Flag_Definitions = new HashSet<int>(Cached_Section_Definitions);

            if (Cached_Section_Definitions == Ceres_Waypoint|| Cached_Section_Definitions == Ceres_Waypoint_Reverse)
            {
                ZeroGravityTrack = true;                
            }

            foreach (NgTrackData.Section section in NgTrackData.TrackManager.Instance.data.sections)
            {
                if (Cached_Jump_Flag_Definitions.Contains(section.index) && (Needs_Jump_Flags_Set_To_Normal_Boolean == true) && (((section.index == 179) && (Cached_Section_Definitions == Escalation_Project)) || (Cached_Section_Definitions == Antelao)))
                {
                    section.type = NgTrackData.E_SECTIONTYPE.NORMAL;
                }

                if (Is_2159 && Forcing_NoTiltLock)
                {
                    section.NoTiltLock = true;
                }
            }

            RawImage HideTexture1 = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            Destroy(HideTexture1);

            Tiltlock_Maglocked_Interpolation_Ratio = 0f;
            Tiltlock_Non_Maglocked_Interpolation_Ratio = 1f;
            Pseudohugger_Grounded_Interpolation_Ratio = 1f;

            Rotation_Time = 2f;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void Update()
        {
            base.Update();            

            if (TargetShip.FinishedEvent || Physics_Mode != 1)
            {
                return;
            }

            if (FloorhuggerTrack)
            {
                return;
            }
            
            Camera_Behavior_Mode_2280 = VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280;

            if (Camera_Behavior_Mode_2280 == 1)
            {
                TiltLock2280(); //2280 TILT LOCK BEHAVIOR
            }
            else if (Camera_Behavior_Mode_2280 == 2)
            {
                Pseudohugger2280(); //2280 PSEUDOHUGGER BEHAVIOR
            }

            //if (Physics_Mode == 1)
            //{
            //    if (Camera_Behavior_Mode_2280 == 1)
            //    {
            //        TiltLock2280(); //2280 TILT LOCK BEHAVIOR
            //    }
            //    else if (Camera_Behavior_Mode_2280 == 2)
            //    {
            //        Pseudohugger2280(); //2280 PSEUDOHUGGER BEHAVIOR
            //    }
            //}
            //else if (Physics_Mode == 0 && VanillaPlusHUDOptions.ModMenuOptions.ForcePseudohugger2159)
            //{
            //    Pseudohugger2159(); //2159 PSEUDOHUGGER BEHAVIOR
            //}

            //Camera_Transform = TargetShip.ShipCameraTransform;
            //Current_Section = TargetShip.CurrentSection;
            //Internal_Camera_Rotation = Camera_Transform.rotation.eulerAngles;            
            //Tilt_Lock_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, 0f);

            //Jump_Flag_Set_For_This_Section = Cached_Jump_Flag_Definitions.Contains(Current_Section.index); //Formerly, Jump_Flag_Set_For_This_Section = (Array.IndexOf(Manually_Set_Section_Jump_Flags(Track_Display_Name), TargetShip.CurrentSection.index) != -1);
            //In_Override_Jump = Jump_Flag_Set_For_This_Section;

            //In_Override_Normal = !Jump_Flag_Set_For_This_Section;

            //if (Manually_Set_Section_Jump_Flags(Track_Display_Name).Contains(TargetShip.CurrentSection.index))
            //{
            //    In_Override_Jump = true;
            //}
            //else
            //{
            //    In_Override_Jump = false;
            //}

            //if ((Manually_Set_Section_Jump_Flags(Track_Display_Name).Contains(TargetShip.CurrentSection.index)) == false)
            //{
            //    In_Override_Normal = true;
            //}
            //else
            //{
            //    In_Override_Normal = false;
            //}

            //if ((Use_Track_Creator_Defined_Jump_Flags == true) && (Current_Section.type == NgTrackData.E_SECTIONTYPE.JUMP))
            //{
            //    In_Creator_Defined_Jump = true;
            //}
            //else 
            //{
            //    In_Creator_Defined_Jump = false;
            //}            

            //if ((Use_Track_Creator_Defined_Jump_Flags == true) && (Current_Section.type == NgTrackData.E_SECTIONTYPE.NORMAL))
            //{
            //    In_Creator_Defined_Normal = true;
            //}
            //else
            //{
            //    In_Creator_Defined_Normal = false;
            //}



            /*if (ZeroGravityTrack == true && !Cached_Jump_Flag_Definitions.Contains(TargetShip.CurrentSection.index))
            {
                Has_Entered_Jump_Boolean = false; //Formerly, TargetShip.PysSim.isShipGrounded = true;
            }*/



            //if ((Physics_Mode == 1) && !TargetShip.FinishedEvent && (Camera_Behavior_Mode_2280 == 1))
            //{
            //    TiltLock2280(); //2280 TILT LOCK BEHAVIOR
            //}
            //else if (((Physics_Mode == 1) && !TargetShip.FinishedEvent && (Camera_Behavior_Mode_2280 == 2))/* || ((Cheats.IntFromPhysicsMod() == 0) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.ForcePseudohugger2159 == true))*/)
            //{
            //    Pseudohugger2280(); //2280 PSEUDOHUGGER BEHAVIOR
            //}
            //else if (((Physics_Mode == 0) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.ForcePseudohugger2159 == true)))
            //{
            //    Pseudohugger2159(); //2159 PSEUDOHUGGER BEHAVIOR
            //}

            Previous_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            //Previous_Intermediate_Rotation_Quaternion = Intermediate_Rotation_Quaternion;
            //Previous_Rigidbody_Rotation_Quaternion = Rigidbody_Rotation_Quaternion;
        }

        public void TiltLock2280()
        {            
            Camera_Transform = TargetShip.ShipCameraTransform;

            if (TargetShip.CurrentSection == null)
            {
                return;
            }

            Current_Section = TargetShip.CurrentSection;            
            Internal_Camera_Rotation = Camera_Transform.rotation.eulerAngles;
            Tilt_Lock_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, 0f);

            //if (Cheats.ModernPhysics && (!TargetShip.OnMaglock || !TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent)
            if (!TargetShip.OnMaglock && !Current_Section.NoTiltLock) //2280 TILT LOCK BEHAVIOR
            {
                Tiltlock_Maglocked_Interpolation_Ratio = 0f;

                Tiltlock_Non_Maglocked_Interpolation_Ratio += Time.deltaTime;

                Target_Tilt_Lock_Rotation_Quaternion = Quaternion.Slerp(Camera_Transform.rotation, Tilt_Lock_Rotation_Quaternion, Tiltlock_Non_Maglocked_Interpolation_Ratio);

                Camera_Transform.rotation = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Target_Tilt_Lock_Rotation_Quaternion.eulerAngles.z);
            }

            //else if (Cheats.ModernPhysics && (TargetShip.OnMaglock || TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent) //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            else //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            {
                Tiltlock_Non_Maglocked_Interpolation_Ratio = 0f;

                Tiltlock_Maglocked_Interpolation_Ratio += Time.deltaTime * Rotation_Time;

                Target_Tilt_Lock_Rotation_Quaternion = Quaternion.Slerp(Tilt_Lock_Rotation_Quaternion, Camera_Transform.rotation, Tiltlock_Maglocked_Interpolation_Ratio); //Formerly, Target_Tilt_Lock_Rotation_Quaternion = Quaternion.Slerp(Tilt_Lock_Rotation_Quaternion, Quaternion.Euler(transform.InverseTransformDirection(TargetShip.ShipCamera.transform.rotation.eulerAngles)), Tiltlock_Maglocked_Interpolation_Ratio);

                Camera_Transform.rotation = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Target_Tilt_Lock_Rotation_Quaternion.eulerAngles.z);
            }

            Pseudohugger_Grounded_Interpolation_Ratio = 1f;
            Pseudohugger_Rotation_Quaternion = Camera_Transform.rotation;
        }

        public void Pseudohugger2280()
        {
            Camera_Transform = TargetShip.ShipCameraTransform;

            if (TargetShip.CurrentSection == null)
            {
                return;
            }

            Current_Section = TargetShip.CurrentSection;            
            Internal_Camera_Rotation = Camera_Transform.rotation.eulerAngles;

            Jump_Flag_Set_For_This_Section = Cached_Jump_Flag_Definitions.Contains(Current_Section.index); //Formerly, Jump_Flag_Set_For_This_Section = (Array.IndexOf(Manually_Set_Section_Jump_Flags(Track_Display_Name), TargetShip.CurrentSection.index) != -1);
            In_Override_Jump = Jump_Flag_Set_For_This_Section;

            if ((Use_Track_Creator_Defined_Jump_Flags == true) && (Current_Section.type == NgTrackData.E_SECTIONTYPE.JUMP))
            {
                In_Creator_Defined_Jump = true;
            }
            else
            {
                In_Creator_Defined_Jump = false;
            }

            if (In_Override_Jump || In_Creator_Defined_Jump)
            {
                Has_Entered_Jump_Boolean = true;
                Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
            }
            else if ((Cached_ShipSim.isShipGrounded == true || ZeroGravityTrack == true) /*&& (In_Override_Normal || In_Creator_Defined_Normal)*/)
            {
                Has_Entered_Jump_Boolean = false;
            }

            if (Has_Entered_Jump_Boolean == false) //2280 PSEUDOHUGGER BEHAVIOR
            {                
                if (Cached_ShipSim.outOfBounds == true /*&& Pseudohugger_Grounded_Interpolation_Ratio >= 1f*/)
                {
                    Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
                    Pseudohugger_Grounded_Interpolation_Ratio = 0f;
                    Pseudohugger_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z);
                }
                else
                {
                    Pseudohugger_Rotation_Quaternion = Quaternion.Slerp(Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Pseudohugger_Rotation_Quaternion.eulerAngles.z), Quaternion.LookRotation(Camera_Transform.forward, TargetShip.InterpolatedSection.Up), Pseudohugger_Grounded_Interpolation_Ratio);
                    Pseudohugger_Grounded_Interpolation_Ratio += Time.deltaTime;
                }
                Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            }
            else
            {
                Pseudohugger_Grounded_Interpolation_Ratio = 0f;
                Pseudohugger_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z);
                Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            }

            Camera_Transform.rotation = Target_Pseudohugger_Rotation_Quaternion;

            Tiltlock_Non_Maglocked_Interpolation_Ratio = 1f;
            Tiltlock_Maglocked_Interpolation_Ratio = 1f;
        }

        //public void Pseudohugger2159()
        //{        
        //    Camera_Transform = TargetShip.ShipCameraTransform;

        //    if (TargetShip.CurrentSection == null)
        //    {
        //        return;
        //    }

        //    Current_Section = TargetShip.CurrentSection;        
        //    Internal_Camera_Rotation = Camera_Transform.rotation.eulerAngles;
        //    Tilt_Lock_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, 0f);

        //    Jump_Flag_Set_For_This_Section = Cached_Jump_Flag_Definitions.Contains(Current_Section.index); //Formerly, Jump_Flag_Set_For_This_Section = (Array.IndexOf(Manually_Set_Section_Jump_Flags(Track_Display_Name), TargetShip.CurrentSection.index) != -1);
        //    In_Override_Jump = Jump_Flag_Set_For_This_Section;

        //    if ((Use_Track_Creator_Defined_Jump_Flags == true) && (Current_Section.type == NgTrackData.E_SECTIONTYPE.JUMP))
        //    {
        //        In_Creator_Defined_Jump = true;
        //    }
        //    else
        //    {
        //        In_Creator_Defined_Jump = false;
        //    }

        //    if ((In_Override_Jump || In_Creator_Defined_Jump) || (Cached_ShipSim.isShipGrounded == false))
        //    {
        //        Has_Entered_Jump_Boolean = true;
        //        Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
        //    }
        //    else /*if ((In_Override_Normal || In_Creator_Defined_Normal) && (Cached_ShipSim.isShipGrounded == true))*/
        //    {
        //        Has_Entered_Jump_Boolean = false;
        //    }

        //    if (Has_Entered_Jump_Boolean == false)
        //    {
        //        Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
        //        Pseudohugger_Rotation_Quaternion = Quaternion.Slerp(Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z), Quaternion.LookRotation(Camera_Transform.forward, TargetShip.InterpolatedSection.Up), TargetShip.PysSim.GroundedTime/* * Rotation_Time*/);
        //        Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
        //    }
        //    else
        //    {
        //        Pseudohugger_Rotation_Quaternion = Quaternion.Slerp(Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z), Tilt_Lock_Rotation_Quaternion, TargetShip.PysSim.AirTime);
        //        Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
        //    }

        //    Camera_Transform.rotation = Target_Pseudohugger_Rotation_Quaternion;
        //}
    }

    public class Camera_Height_Adjustments : ScriptableHud
    {
        public static readonly Vector3 Internal_Camera_Offset_2159 = new Vector3(0f, 0.2f, 0f);
        public static readonly Vector3 Camera_Correction_Offset_2280 = new Vector3(0f, 0f, 0.3f); //Formerly public static readonly Vector3 Camera_Correction_Offset_2280 = new Vector3(0f, -0.1f, 0.3f);        
        public static readonly Vector3 Cockpit_Camera_Offset_2280 = new Vector3(0f, 0.2f, 0f); // public static readonly Vector3 Cockpit_Camera_Offset_2280 = new Vector3(0f, 0.1f, 0f);
        //public static readonly Vector3 Cockpit_Camera_Offset_2159 = new Vector3(0f, 0.1f, -0.1f);
        public MeshRenderer MainHullMesh;
        public MeshRenderer ShieldMesh;
        public MeshRenderer CockpitMesh;
        public MeshRenderer VR_CockpitMesh;
        public MeshRenderer VR_CockpitMesh_NC;

        public Transform Hull_Transform;
        public Transform Shield_Transform;
        public Transform Cockpit_Transform;
        public Transform VR_Cockpit_Transform;
        public Transform VR_Cockpit_NC_Transform;
        public bool Is_2280_Physics;

        public ShipCamera Cached_ShipCamera;
        public int Camera_Mode;
        public bool Event_Finished;
        public bool Permanently_Eliminated;
        public bool Is_Networked_Race_Event;
        public bool Is_Valid_Gamemode;
        public bool Race_Gamemode_Is_Applicable;
        public bool Continue_Applying_Camera_Adjustments;
        public Gamemode Current_Gamemode;        

        public bool Bonnet_Mesh;
        public bool Nosecam_Mesh;

        public Vector3 MainHullMesh_Initial_Local_Position;
        //public Vector3 ShieldMesh_Initial_Local_Position;
        //public Vector3 CockpitMesh_Initial_Local_Position;
        //public Vector3 VR_CockpitMesh_Initial_Local_Position;
        //public Vector3 VR_CockpitMesh_NC_Initial_Local_Position;

        //public Vector3 MainHullMesh_Adjusted_Local_Position;
        //public Vector3 ShieldMesh_Adjusted_Local_Position;
        //public Vector3 CockpitMesh_Adjusted_Local_Position;
        //public Vector3 VR_CockpitMesh_Adjusted_Local_Position;
        //public Vector3 VR_CockpitMesh_NC_Adjusted_Local_Position;

        //public Vector3 MainHullMesh_Adjusted_Local_Position_Canopy_Nosecam;
        //public Vector3 ShieldMesh_Adjusted_Local_Position_Canopy_Nosecam;

        //public Vector3 MainHullMesh_Negative_Local_Position;
        //public Vector3 ShieldMesh_Negative_Local_Position;


        public override void Start()
        {
            base.Start();

            RawImage HideTexture2 = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            Destroy(HideTexture2);

            MainHullMesh = TargetShip.Settings.REF_MESH.GetComponent<MeshRenderer>();
            ShieldMesh = TargetShip.Settings.REF_SHIELD.GetComponent<MeshRenderer>();

            if (TargetShip.Settings.REF_COCKPITMESH != null)
            {
                CockpitMesh = TargetShip.Settings.REF_COCKPITMESH.GetComponent<MeshRenderer>();
            }
            if (TargetShip.Settings.REF_VRCOCKPITMESH != null)
            {
                VR_CockpitMesh = TargetShip.Settings.REF_VRCOCKPITMESH.GetComponent<MeshRenderer>();
            }
            if (TargetShip.Settings.REF_VRCOCKPITMESHNOCANOPY != null)
            {
                VR_CockpitMesh_NC = TargetShip.Settings.REF_VRCOCKPITMESHNOCANOPY.GetComponent<MeshRenderer>();
            }

            Is_2280_Physics = Cheats.IntFromPhysicsMod() == 1;

            Hull_Transform = MainHullMesh.transform;
            Shield_Transform = ShieldMesh.transform;

            if (CockpitMesh != null)
            {
                Cockpit_Transform = CockpitMesh.transform;
            }
            if (VR_CockpitMesh != null)
            {
                VR_Cockpit_Transform = VR_CockpitMesh.transform;
            }
            if (VR_CockpitMesh_NC != null)
            {
                VR_Cockpit_NC_Transform = VR_CockpitMesh_NC.transform;
            }

            MainHullMesh_Initial_Local_Position = Hull_Transform.localPosition;
            //ShieldMesh_Initial_Local_Position = ShieldMesh.transform.localPosition;
            //CockpitMesh_Initial_Local_Position = CockpitMesh.transform.localPosition;
            //VR_CockpitMesh_Initial_Local_Position = VR_CockpitMesh.transform.localPosition;
            //VR_CockpitMesh_NC_Initial_Local_Position = VR_CockpitMesh_NC.transform.localPosition;

            //MainHullMesh_Adjusted_Local_Position = MainHullMesh_Initial_Local_Position + Internal_Camera_Offset_2159;
            //ShieldMesh_Adjusted_Local_Position = ShieldMesh_Initial_Local_Position + Internal_Camera_Offset_2159;
            //CockpitMesh_Adjusted_Local_Position = CockpitMesh_Initial_Local_Position + Internal_Camera_Offset_2159;
            //VR_CockpitMesh_Adjusted_Local_Position = VR_CockpitMesh_Initial_Local_Position + Internal_Camera_Offset_2159;
            //VR_CockpitMesh_NC_Adjusted_Local_Position = VR_CockpitMesh_NC_Initial_Local_Position + Internal_Camera_Offset_2159;

            //MainHullMesh_Adjusted_Local_Position_Canopy_Nosecam = MainHullMesh_Initial_Local_Position + Camera_Correction_Offset_2280;
            //ShieldMesh_Adjusted_Local_Position_Canopy_Nosecam = ShieldMesh_Initial_Local_Position + Camera_Correction_Offset_2280;

            //MainHullMesh_Negative_Local_Position = MainHullMesh_Initial_Local_Position - Internal_Camera_Offset_2159;
            //ShieldMesh_Negative_Local_Position = ShieldMesh_Initial_Local_Position - Internal_Camera_Offset_2159;

            Cached_ShipCamera = TargetShip.CamSim;
            Current_Gamemode = RaceManager.CurrentGamemode;
            Is_Networked_Race_Event = (RaceManager.Instance.RaceContext == ERaceContext.Multiplayer);
            Race_Gamemode_Is_Applicable = (Is_Networked_Race_Event == true && Current_Gamemode is GmRace);
            Is_Valid_Gamemode = (!(Current_Gamemode is GmRace) && !(Current_Gamemode is GmTimeTrial) && !(Current_Gamemode is GmSpeedLap) && !(Current_Gamemode is GmSurvival) && !(Current_Gamemode is GmStunt));
        }

        public void Set_Cockpit_Meshes_Enabled(bool state)
        {
            if (CockpitMesh != null)
            {
                CockpitMesh.enabled = state;
            }
            if (VR_CockpitMesh != null)
            {
                VR_CockpitMesh.enabled = state;
            }
            if (VR_CockpitMesh_NC != null)
            {
                VR_CockpitMesh_NC.enabled = state;
            }
        }

        public void Set_Cockpit_Mesh_Positions(Vector3 position)
        {
            if (Cockpit_Transform != null)
            {
                Cockpit_Transform.localPosition = position;
            }
            if (VR_Cockpit_Transform != null)
            {
                VR_Cockpit_Transform.localPosition = position;
            }
            if (VR_Cockpit_NC_Transform != null)
            {
                VR_Cockpit_NC_Transform.localPosition = position;
            }
        }

        public override void Update()
        {
            base.Update();            

            if (VanillaPlusHUDOptions.ModMenuOptions.AdjustmentAlignmentFixToggle == true)
            {
                //Forces cache refresh which helps to produce the correct alignment, without this the interior cockpit mesh and camera position will misalign in 2159 when using the internal camera height. Other misalignment issues may also be occurring without this fix.
                //However, this 'fix' misaligns everything else, so it's an option toggle.
                var DummyVariableForReadingInto0 = TargetShip.ShipCameraTransform.localPosition; //CAMERA
                //var DummyVariableForReadingInto1 = MainHullMesh.transform.localPosition; //MAIN HULL
                //var DummyVariableForReadingInto2 = ShieldMesh.transform.localPosition; //SHIELD
                //var DummyVariableForReadingInto3 = CockpitMesh.transform.localPosition; //COCKPIT
                //var DummyVariableForReadingInto4 = VR_CockpitMesh.transform.localPosition; //VR COCKPIT
                //var DummyVariableForReadingInto5 = VR_CockpitMesh_NC.transform.localPosition; //VR COCKPIT, NO CANOPY
            }
            
            Camera_Mode = Cached_ShipCamera.CameraMode;
            Event_Finished = TargetShip.FinishedEvent;
            Permanently_Eliminated = TargetShip.IsEliminatedPermanently;
            Continue_Applying_Camera_Adjustments = (Race_Gamemode_Is_Applicable == true || Is_Valid_Gamemode == true);

            if (Permanently_Eliminated == false && ((Camera_Mode == 2 && !Event_Finished) || (Camera_Mode == 2 && Event_Finished && Continue_Applying_Camera_Adjustments == true)))
            {
                Bonnet_Mesh = VanillaPlusHUDOptions.ModMenuOptions.CanopyMeshAdjustment == 0 && !Cached_ShipCamera.LookingBehind;                

                if (Is_2280_Physics)
                {
                    //RAISED CANOPY CAMERA
                    if (VanillaPlusHUDOptions.ModMenuOptions.CanopyCameraAdjustment2280 == 0)
                    {
                        TargetShip.ShipCameraTransform.localPosition = Cockpit_Camera_Offset_2280;
                        MainHullMesh.enabled = Bonnet_Mesh; //BONNETCAM MESH AND HIDDEN MESH
                        Hull_Transform.localPosition = Vector3.zero;
                        Shield_Transform.localPosition = Vector3.zero;
                    }
                    //INTERNAL CANOPY CAMERA
                    else
                    {
                        MainHullMesh.enabled = Bonnet_Mesh;

                        //BONNETCAM MESH
                        if (Bonnet_Mesh)
                        {
                            Hull_Transform.localPosition = Camera_Correction_Offset_2280;
                        }
                        //HIDDEN MESH
                        else
                        {
                            Hull_Transform.localPosition = Vector3.zero;
                        }

                        Shield_Transform.localPosition = Camera_Correction_Offset_2280;
                    }
                }
                else
                {
                    MainHullMesh.enabled = Bonnet_Mesh; //BONNETCAM MESH AND HIDDEN MESH
                }
            }
            else if (Permanently_Eliminated == false && ((Camera_Mode == 3 && !Event_Finished) || (Camera_Mode == 3 && Event_Finished && Continue_Applying_Camera_Adjustments == true)))
            {
                Nosecam_Mesh = VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0 && !Cached_ShipCamera.LookingBehind;                

                if (Is_2280_Physics)
                {
                    //RAISED COCKPIT CAMERA
                    if (VanillaPlusHUDOptions.ModMenuOptions.CockpitCameraAdjustment2280 == 0)
                    {
                        TargetShip.ShipCameraTransform.localPosition = Cockpit_Camera_Offset_2280;

                        //NOSECAM MESH
                        if (Nosecam_Mesh)
                        {
                            Set_Cockpit_Mesh_Positions(Vector3.zero);
                            Shield_Transform.localPosition = Vector3.zero;
                            Set_Cockpit_Meshes_Enabled(false);
                            MainHullMesh.enabled = true;
                            Hull_Transform.localPosition = Vector3.zero;
                        }
                        //INTERIOR COCKPIT MESH
                        else
                        {
                            Set_Cockpit_Mesh_Positions(Cockpit_Camera_Offset_2280);
                            Shield_Transform.localPosition = Cockpit_Camera_Offset_2280;
                            Set_Cockpit_Meshes_Enabled(true);
                            MainHullMesh.enabled = false;
                        }
                    }
                    //INTERNAL COCKPIT CAMERA
                    else
                    {
                        //NOSECAM MESH
                        if (Nosecam_Mesh)
                        {
                            Set_Cockpit_Meshes_Enabled(false);
                            MainHullMesh.enabled = true;
                            Hull_Transform.localPosition = -Cockpit_Camera_Offset_2280;
                            Shield_Transform.localPosition = -Cockpit_Camera_Offset_2280;
                        }
                        //INTERIOR COCKPIT MESH
                        else
                        {
                            Set_Cockpit_Meshes_Enabled(true);
                            Set_Cockpit_Mesh_Positions(Vector3.zero);
                            MainHullMesh.enabled = false;
                            Hull_Transform.localPosition = Vector3.zero;
                            Shield_Transform.localPosition = Vector3.zero;
                        }
                    }
                }
                else
                {
                    //RAISED COCKPIT CAMERA
                    if (VanillaPlusHUDOptions.ModMenuOptions.CockpitCameraAdjustment2159 == 0)
                    {
                        TargetShip.ShipCameraTransform.localPosition = Internal_Camera_Offset_2159;

                        //NOSECAM MESH
                        if (Nosecam_Mesh)
                        {
                            Set_Cockpit_Meshes_Enabled(false);
                            MainHullMesh.enabled = true;
                            Hull_Transform.localPosition = MainHullMesh_Initial_Local_Position;
                            Shield_Transform.localPosition = Vector3.zero;
                            Set_Cockpit_Mesh_Positions(Vector3.zero);
                        }
                        //INTERIOR COCKPIT MESH
                        else
                        {
                            Set_Cockpit_Meshes_Enabled(true);
                            MainHullMesh.enabled = false;
                            Shield_Transform.localPosition = Internal_Camera_Offset_2159;
                            Set_Cockpit_Mesh_Positions(Internal_Camera_Offset_2159);
                        }
                    }
                    //INTERNAL COCKPIT CAMERA
                    else
                    {
                        //NOSECAM MESH
                        if (Nosecam_Mesh)
                        {
                            Set_Cockpit_Meshes_Enabled(false);
                            MainHullMesh.enabled = true;
                            Hull_Transform.localPosition = -Internal_Camera_Offset_2159;
                            Shield_Transform.localPosition = -Internal_Camera_Offset_2159;
                        }
                        //INTERIOR COCKPIT MESH
                        else
                        {
                            Set_Cockpit_Meshes_Enabled(true);
                            MainHullMesh.enabled = false;
                            Hull_Transform.localPosition = Vector3.zero;
                            Shield_Transform.localPosition = Vector3.zero;
                            Set_Cockpit_Mesh_Positions(Vector3.zero);
                        }
                    }
                }
            }
            //Reset to default position
            else
            {                
                MainHullMesh.enabled = true;
                Set_Cockpit_Meshes_Enabled(false);
                Shield_Transform.localPosition = Vector3.zero;
                Hull_Transform.localPosition = MainHullMesh_Initial_Local_Position;
                Set_Cockpit_Mesh_Positions(Vector3.zero);
            }
        }
    }

    public class Extra_Warnings : ScriptableHud
    {
        public Text Final_Lap_Warning_Text;
        public Text Tremor_Warning_Text;
        public Text Hunter_Warning_Text;
        public Text Shield_Timer_Text;

        public float Hunter_Projectile_Distance_From_Target;
        public int Current_Hunter_Distance_Key;
        public int Previous_Hunter_Distance_Key = -1;
        public int Current_Shield_Timer_Key;
        public int Previous_Shield_Timer_Key = -1;

        public float Shield_Timer;

        public float Self_Destruct_Timer = 0f;

        public NgUi.RaceUi.HUD.EliminatorHud Eliminator_Hud;
        public NgUi.RaceUi.HUD.NetworkNameTags Network_Name_Tags;
        public bool Visibility_Toggle_State = true;

        public int Smartshift_Direction;

        public NgPickups.Physical.Projectiles.HunterProjectile Hunter_Projectile;
        public Rigidbody Hunter_Body;
        public ShipController Hunter_Target;
        public bool Hunter_Warning_Visible;

        public ShipSim Cached_ShipSim;

        public override void Start()
        {
            base.Start();

            Eliminator_Hud = HudRegister.CurrentHudManager.Huds.OfType<NgUi.RaceUi.HUD.EliminatorHud>().FirstOrDefault();
            Network_Name_Tags = HudRegister.CurrentHudManager.Huds.OfType<NgUi.RaceUi.HUD.NetworkNameTags>().FirstOrDefault();

            Final_Lap_Warning_Text = CustomComponents.GetById<Text>("FinalLapWarning");
            Tremor_Warning_Text = CustomComponents.GetById<Text>("TremorWarning");
            Hunter_Warning_Text = CustomComponents.GetById<Text>("HunterWarning");
            Shield_Timer_Text = CustomComponents.GetById<Text>("ShieldTimer");

            Final_Lap_Warning_Text.enabled = false;
            Tremor_Warning_Text.enabled = false;
            Hunter_Warning_Text.enabled = false;
            Shield_Timer_Text.enabled = false;

            NgUiEvents.OnTriggerMessage += FinalLapWarning;

            Cached_ShipSim = TargetShip.PysSim;
        }

        public override void OnDestroy()
        {
            NgUiEvents.OnTriggerMessage -= FinalLapWarning;

            base.OnDestroy();
        }

        public void FinalLapWarning(string message, ShipController ship, Color color)
        {
            if (message == "FINAL LAP" && VanillaPlusHUDOptions.ModMenuOptions.FinalLapWarningToggle)
            {
                Final_Lap_Warning_Text.enabled = true;
            }            
        }

        public override void Update()
        {
            base.Update();

            if (TargetShip.AutopilotActivated)
            {
                Smartshift_Direction = 0;
            }

            if (TargetShip.Input.AxisSteer > 0)
            {
                Smartshift_Direction = 1;
            }
            else if (TargetShip.Input.AxisSteer < 0)
            {
                Smartshift_Direction = -1;
            }            

            if (Race.QuakeExists && NgPickups.Physical.Tremor.Instance.Owner != TargetShip && VanillaPlusHUDOptions.ModMenuOptions.TremorWarningToggle)
            {
                Tremor_Warning_Text.enabled = true;
            }
            else
            {
                Tremor_Warning_Text.enabled = false;
            }

            Hunter_Warning_Visible = false;
            Hunter_Projectile = NgPickups.Physical.Projectiles.HunterProjectile.CurrentProjectile;

            if (Hunter_Projectile != null && VanillaPlusHUDOptions.ModMenuOptions.HunterWarningToggle)
            {
                Hunter_Body = Hunter_Projectile.Body;
                Hunter_Target = Hunter_Projectile.TargetShip;

                if (Hunter_Body != null && Hunter_Target != null)
                {
                    Hunter_Projectile_Distance_From_Target = Hunter_Target.InverseTransformPoint(Hunter_Body.position).magnitude;

                    if (Hunter_Projectile_Distance_From_Target <= 30f)
                    {
                        Current_Hunter_Distance_Key = Mathf.RoundToInt(Hunter_Projectile_Distance_From_Target * 10f);
                        if (Current_Hunter_Distance_Key != Previous_Hunter_Distance_Key)
                        {
                            Previous_Hunter_Distance_Key = Current_Hunter_Distance_Key;
                            Hunter_Warning_Text.text = Hunter_Projectile_Distance_From_Target.ToString("'HUNTER: '0.0'm'");
                        }
                        Hunter_Warning_Visible = true;
                    }
                }
            }

            Hunter_Warning_Text.enabled = Hunter_Warning_Visible;

            Shield_Timer = TargetShip.ShieldTimer;
            if (Shield_Timer > 0f && VanillaPlusHUDOptions.ModMenuOptions.ShieldTimerToggle == true)
            {
                Current_Shield_Timer_Key = (int)(Shield_Timer * 100f);
                if (Current_Shield_Timer_Key != Previous_Shield_Timer_Key)
                {
                    Previous_Shield_Timer_Key = Current_Shield_Timer_Key;
                    Shield_Timer_Text.text = Shield_Timer.ToString("0.00");
                }
                Shield_Timer_Text.enabled = true;
            }
            else
            {
                Shield_Timer_Text.enabled = false;
            }

            if (Input.GetKeyDown(VanillaPlusHUDOptions.ModMenuOptions.PreviousSongKeyCode))
            {
                NgMusic.MusicPlayer.Instance.PlayPreviousSong(false); //the boolean passed here is whether or not to allow random song selection
            }

            if (Input.GetKey(VanillaPlusHUDOptions.ModMenuOptions.BarrelRollKeyCode))
            {
                TargetShip.PysSim.PerformBarrelRoll(TargetShip.PysSim.prevTurnInput);
            }

            if (Input.GetKeyDown(VanillaPlusHUDOptions.ModMenuOptions.SideshiftLeftKeyCode))
            {
                Cached_ShipSim.StartSideshift(-1);
            }

            if (Input.GetKeyDown(VanillaPlusHUDOptions.ModMenuOptions.SideshiftRightKeyCode))
            {
                Cached_ShipSim.StartSideshift(1);
            }

            if (Input.GetKeyDown(VanillaPlusHUDOptions.ModMenuOptions.SmartshiftKeyCode))
            {
                Cached_ShipSim.StartSideshift(Smartshift_Direction);
            }

            if (Input.GetKeyDown(VanillaPlusHUDOptions.ModMenuOptions.NameTag_And_ShieldBars_Visibility_Toggle_KeyCode))
            {
                Visibility_Toggle_State = !Visibility_Toggle_State;

                if (Visibility_Toggle_State == false)
                {
                    if (Eliminator_Hud != null)
                    {
                        Eliminator_Hud.Close();
                    }
                    if (Network_Name_Tags != null)
                    {
                        Network_Name_Tags.Close();
                    }
                    if (VanillaPlusHUDOptions.ModMenuOptions.VisibilityToggleAffectsRechargeSum == true && Recharge_Sum.Instance != null)
                    {
                        Recharge_Sum.Instance.Close();
                    }
                }
                else
                {
                    if (Eliminator_Hud != null)
                    {
                        Eliminator_Hud.Open();
                    }
                    if (Network_Name_Tags != null)
                    {
                        Network_Name_Tags.Open();
                    }
                    if (VanillaPlusHUDOptions.ModMenuOptions.VisibilityToggleAffectsRechargeSum == true && Recharge_Sum.Instance != null)
                    {
                        Recharge_Sum.Instance.Open();
                    }
                }
            }

            if (Input.GetKey(VanillaPlusHUDOptions.ModMenuOptions.SelfDestructKeyCode))
            {
                Self_Destruct_Timer += Time.deltaTime;
                if (Self_Destruct_Timer >= VanillaPlusHUDOptions.ModMenuOptions.SelfDestructTimer)
                {
                    TargetShip.ShieldIntegrity = -1f;
                }
            }
            else
            {
                Self_Destruct_Timer = 0f;
            }
        }
    }

    public class Overtake_Radar : ScriptableHud
    {
        public Image Overtake_Warning_Outline_Image;
        public Image Overtake_Warning_Background_Image;
        public Image Overtake_Warning_Pipper_Image;

        public ShipController Overtaking_Ship;

        public Vector3 Raw_Offset;

        public float Longitudinal_Offset_Float;
        public float Lateral_Offset_Float;

        public static readonly Vector2 Pipper_Left_Position = new Vector2(-115f, 139.5f);
        public static readonly Vector2 Pipper_Right_Position = new Vector2(115f, 139.5f);

        public int Current_Visibility;
        public int Previous_Visibility = -1;
        public bool Show_Radar;
        public float Pipper_Fill;
        public RectTransform Pipper_Rect;
        public const float Pipper_Empty_Fill_Threshold = 0.27f / 27f;

        public Transform Camera_Transform;

        public override void Start()
        {
            base.Start();            

            Overtake_Warning_Outline_Image = CustomComponents.GetById<Image>("OvertakeWarningOutline");
            Overtake_Warning_Background_Image = CustomComponents.GetById<Image>("OvertakeWarningBackground");
            Overtake_Warning_Pipper_Image = CustomComponents.GetById<Image>("OvertakeWarningPipper");
            Pipper_Rect = Overtake_Warning_Pipper_Image.rectTransform;            
        }

        public override void Update()
        {
            base.Update();     

            Overtaking_Ship = null;

            if (TargetShip.CurrentPlace < Ships.Active.Count) //Not in last place
            {
                Overtaking_Ship = Ships.FindShipInPlace(TargetShip.CurrentPlace + 1);
            }
            else if (TargetShip.CurrentPlace == Ships.Active.Count && Ships.Active.Count != 1) //In last place
            {
                Overtaking_Ship = Ships.FindShipInPlace(TargetShip.CurrentPlace - 1);
            }

            if (Overtaking_Ship != null)
            {
                Longitudinal_Offset_Float = TargetShip.InverseTransformPoint(Overtaking_Ship.PhysicsPosition).magnitude;
            }
            else //only 1 ship on the track
            {
                Overtaking_Ship = TargetShip;
                Longitudinal_Offset_Float = 50f;
            }

            if (Camera_Transform == null)
            {
                Camera_Transform = TargetShip.ShipCameraTransform;
                return;
            }

            Raw_Offset = Camera_Transform.InverseTransformPoint(Overtaking_Ship.PhysicsPosition);
            Lateral_Offset_Float = Raw_Offset.x;

            Pipper_Rect.anchoredPosition = Vector2.Lerp(Pipper_Left_Position, Pipper_Right_Position, ((Mathf.Clamp(Lateral_Offset_Float, -4f, 4f) + 4f) / 8f));

            Pipper_Fill = 1f - (Mathf.Clamp(Longitudinal_Offset_Float, 0f, 50f) * 0.02f);
            Overtake_Warning_Pipper_Image.fillAmount = Pipper_Fill;

            Current_Visibility = (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarVisibility == 1 && Pipper_Fill < Pipper_Empty_Fill_Threshold) ? 0 : 1;

            if (Current_Visibility != Previous_Visibility)
            {
                Previous_Visibility = Current_Visibility;

                Show_Radar = Current_Visibility == 1;
                Overtake_Warning_Background_Image.enabled = Show_Radar;
                Overtake_Warning_Outline_Image.enabled = Show_Radar;
                Overtake_Warning_Pipper_Image.enabled = Show_Radar;
            }
        }
    }

    public class Speedlap_And_Precision : ScriptableHud
    {
        public Image Speedlap_Best_Field;
        public Image Speedlap_Best_Accent;
        public Image Speedlap_Time_Field;
        public Image Speedlap_Time_Accent;
        public Text Speedlap_Best_Name;
        public Text Speedlap_Best_Readout;
        public Text Speedlap_Time_Name;
        public Text Speedlap_Time_Readout;

        public string Track_Scene_Name;
        public string Track_Scene_Save_File_Name;
        public string Precision_Save_Path;

        public GmPrecision Precision_Gamemode_Instance;

        public float Speedlap_Best_Time_Seconds;
        public string Speedlap_Best_Time_Units;        

        public float Current_Lap_Time;
        public int Current_Lap_Centiseconds;
        public int Previous_Lap_Centiseconds = -1;

        public void Refresh_Best_Readout()
        {
            if (Speedlap_Best_Time_Seconds > 0f)
            {
                Speedlap_Best_Time_Units = FloatToTime.Convert(Speedlap_Best_Time_Seconds, "0:00.00");
            }
            else
            {
                Speedlap_Best_Time_Units = "–:––.––";
            }

            Speedlap_Best_Readout.text = Speedlap_Best_Time_Units;
        }

        public void Retrieve_Precision_Best_Time_From_Disk()
        {
            Precision_Save_Path = Directory.GetCurrentDirectory();
            Precision_Save_Path += "/User/Save/Precision/";

            Track_Scene_Name = ContentManager.GetCurrentTrack().SceneName;
            Track_Scene_Save_File_Name = Track_Scene_Name + ".sav";

            Precision_Save_Path += Track_Scene_Save_File_Name;

            Precision_Save_Path = Path.GetFullPath(Precision_Save_Path);

            if (File.Exists(Precision_Save_Path))
            {
                byte[] Byte_Array = File.ReadAllBytes(Precision_Save_Path);
                
                Speedlap_Best_Time_Seconds = BitConverter.ToSingle(Byte_Array, 1);
            }
            else
            {
                Speedlap_Best_Time_Seconds = -1f;
            }
        }

        public void Retrieve_Speed_Lap_Best_Time_From_Disk()
        {
            string Speed_Lap_Save_Path = Directory.GetCurrentDirectory();
            Speed_Lap_Save_Path += "/User/Save/Times/";

            string Track_Display_Name = ContentManager.GetCurrentTrack().DisplayName;
            string Track_Display_Save_File_Name =  "Speed Lap" + "_" + Track_Display_Name + "_" + NgStr.GetEventSpeedClassName() + "_" + NgStr.ShipToSaveFileString();
            //Alternatively,                       "Speed Lap" + "_" + Track_Scene_Name + "_" + RaceManager.Instance.SpeedClass.ToString() + "_" + TargetShip.TargetShipData.CategorySortClass + "_" + NgStr.PhysicsModToSaveFileString() + ".bin";

            if (Cheats.IntFromPhysicsMod() == 0)
            {
                Track_Display_Save_File_Name += ".bin";
            }
            else
            {
                Track_Display_Save_File_Name += "_" + NgStr.PhysicsModToSaveFileString() + ".bin";
            }

            Speed_Lap_Save_Path += Track_Display_Save_File_Name;

            Speed_Lap_Save_Path = Path.GetFullPath(Speed_Lap_Save_Path);

            if (File.Exists(Speed_Lap_Save_Path))
            {
                byte[] Byte_Array = File.ReadAllBytes(Speed_Lap_Save_Path);

                Speedlap_Best_Time_Seconds = BitConverter.ToSingle(Byte_Array, 0);
            }
            else
            {
                Speedlap_Best_Time_Seconds = -1f;
            }
        }

        public void UpdateLapTime(float time)
        {
            Speedlap_Best_Time_Seconds = time;
            Refresh_Best_Readout();
        }

        public override void Start()
        {
            base.Start();

            Speedlap_Best_Field = CustomComponents.GetById<Image>("SpeedlapBestField");
            Speedlap_Best_Accent = CustomComponents.GetById<Image>("SpeedlapBestAccent");
            Speedlap_Time_Field = CustomComponents.GetById<Image>("SpeedlapTimeField");
            Speedlap_Time_Accent = CustomComponents.GetById<Image>("SpeedlapTimeAccent");
            Speedlap_Best_Name = CustomComponents.GetById<Text>("SpeedlapBestName");
            Speedlap_Best_Readout = CustomComponents.GetById<Text>("SpeedlapBestReadout");
            Speedlap_Time_Name = CustomComponents.GetById<Text>("SpeedlapTimeName");
            Speedlap_Time_Readout = CustomComponents.GetById<Text>("SpeedlapTimeReadout");
            
            Speedlap_Time_Readout.text = "–:––.––";

            if (RaceManager.CurrentGamemode is GmPrecision)
            {
                Retrieve_Precision_Best_Time_From_Disk();
            }            

            if (RaceManager.CurrentGamemode is GmSpeedLap)
            {
                Retrieve_Speed_Lap_Best_Time_From_Disk();
            }

            Refresh_Best_Readout();            

            NgUiEvents.OnGamemodeUpdateBestTime += UpdateLapTime;

            Precision_Gamemode_Instance = RaceManager.CurrentGamemode as GmPrecision;
        }

        public override void OnDestroy()
        {
            NgUiEvents.OnGamemodeUpdateBestTime -= UpdateLapTime;

            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();            

            Current_Lap_Time = TargetShip.CurrentLapTime;
            Current_Lap_Centiseconds = (int)(Current_Lap_Time * 100f);

            if (Current_Lap_Centiseconds != Previous_Lap_Centiseconds)
            {
                Previous_Lap_Centiseconds = Current_Lap_Centiseconds;
                Speedlap_Time_Readout.text = FloatToTime.Convert(Current_Lap_Time, "0:00.00");
            }            

            if (Precision_Gamemode_Instance == null || Precision_Gamemode_Instance.AuthorTimes == null)
            {
                return;
            }                        

            if (Current_Lap_Time < Precision_Gamemode_Instance.AuthorTimes.PlatinumTarget)
            {
                Speedlap_Time_Readout.color = Position_Counter.Platinum_Color;
            }
            else if (Current_Lap_Time < Precision_Gamemode_Instance.AuthorTimes.GoldTarget)
            {
                Speedlap_Time_Readout.color = Position_Counter.Gold_Color;
            }
            else if (Current_Lap_Time < Precision_Gamemode_Instance.AuthorTimes.SilverTarget)
            {
                Speedlap_Time_Readout.color = Position_Counter.Silver_Color;
            }
            else if (Current_Lap_Time < Precision_Gamemode_Instance.AuthorTimes.BronzeTarget)
            {
                Speedlap_Time_Readout.color = Position_Counter.Bronze_Color;
            }
            else
            {
                Speedlap_Time_Readout.color = Color.white;
            }
        }
    }

    public class Survival : ScriptableHud
    {
        public Text Zone_Number;
        public Text Zone_Name;
        public Text Zone_Score;
        public Image Zone_Progress_Diamonds_Background;
        public Image Diamond_1;
        public Image Diamond_2;
        public Image Diamond_3;
        public Image Diamond_4;
        public Image Diamond_5;
        public Image Diamond_6;
        public Image Diamond_7;
        public Image Diamond_8;
        public Image Diamond_9;
        public Image Diamond_10;
        public Image Diamond_11;
        public Image Diamond_12;
        public Image Diamond_13;
        public Image Diamond_14;
        public Image Diamond_15;
        public Text Perfect_Zone;

        public Vector4 Default_Zone_Number_Color;
        public Vector4 Default_Zone_Name_Color;
        public Vector4 Default_Zone_Score_Color;

        public Image[] Zone_Diamonds;

        public float Zone_Progress;

        public int Zone_Diamond_Index;

        public Vector4 Green_Diamond_Color;
        public Vector4 Red_Diamond_Color;
        public Vector4 Blue_Diamond_Color;
        public Vector4 Diamond_Target_Color;
        
        public int Previous_Zone_Diamond_Index = -1;
        public int Lit_Diamond_Index;
        public int Current_Zone;
        public int Previous_Zone = -1;
        public int Current_Perfect_Zone_State;
        public int Previous_Perfect_Zone_State = -1;
        public int Current_Zone_Score_Key;
        public int Previous_Zone_Score_Key = -1;
        public GmSurvivalBackend Current_Survival_Backend;
        public NgVirtual.ZonePalleteSettings.ZoneColors Current_ZonePallete_Colors;

        public override void Start()
        {
            base.Start();

            Zone_Number = CustomComponents.GetById<Text>("ZoneNumber");
            Zone_Name = CustomComponents.GetById<Text>("ZoneName");
            Zone_Score = CustomComponents.GetById<Text>("ZoneScore");
            Diamond_1 = CustomComponents.GetById<Image>("Diamond 1");
            Diamond_2 = CustomComponents.GetById<Image>("Diamond 2");
            Diamond_3 = CustomComponents.GetById<Image>("Diamond 3");
            Diamond_4 = CustomComponents.GetById<Image>("Diamond 4");
            Diamond_5 = CustomComponents.GetById<Image>("Diamond 5");
            Diamond_6 = CustomComponents.GetById<Image>("Diamond 6");
            Diamond_7 = CustomComponents.GetById<Image>("Diamond 7");
            Diamond_8 = CustomComponents.GetById<Image>("Diamond 8");
            Diamond_9 = CustomComponents.GetById<Image>("Diamond 9");
            Diamond_10 = CustomComponents.GetById<Image>("Diamond 10");
            Diamond_11 = CustomComponents.GetById<Image>("Diamond 11");
            Diamond_12 = CustomComponents.GetById<Image>("Diamond 12");
            Diamond_13 = CustomComponents.GetById<Image>("Diamond 13");
            Diamond_14 = CustomComponents.GetById<Image>("Diamond 14");
            Diamond_15 = CustomComponents.GetById<Image>("Diamond 15");
            Perfect_Zone = CustomComponents.GetById<Text>("PerfectZone");

            Default_Zone_Number_Color = Zone_Number.color;
            Default_Zone_Name_Color = Zone_Name.color;
            Default_Zone_Score_Color = Zone_Score.color;

            Green_Diamond_Color = Diamond_1.color;
            Red_Diamond_Color = Diamond_6.color;
            Blue_Diamond_Color = Diamond_11.color;

            Perfect_Zone.enabled = true;

            NgUiEvents.OnZoneTitleUpdate += ZoneTitle;
            NgUiEvents.OnZoneProgressUpdate += ZoneProgress;

            Zone_Number.text = "0";
            Zone_Name.text = "TOXIC";
            Zone_Score.text = "0";

            Zone_Diamonds = new Image[] { Diamond_1, Diamond_2, Diamond_3, Diamond_4, Diamond_5, Diamond_6, Diamond_7, Diamond_8, Diamond_9, Diamond_10, Diamond_11, Diamond_12, Diamond_13, Diamond_14, Diamond_15 };            
        }

        public override void OnDestroy()
        {
            NgUiEvents.OnZoneTitleUpdate -= ZoneTitle;
            NgUiEvents.OnZoneProgressUpdate -= ZoneProgress;

            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();

            if (Current_Survival_Backend == null)
            {
                Current_Survival_Backend = GmSurvivalBackend.Instance;
                return;
            }            

            Current_Zone_Score_Key = (int)(Current_Survival_Backend.zoneScore + 0.5f);
            if (Current_Zone_Score_Key != Previous_Zone_Score_Key)
            {
                Previous_Zone_Score_Key = Current_Zone_Score_Key;
                Zone_Score.text = Current_Zone_Score_Key.ToString("N0");
            }

            Zone_Diamond_Index = (int)(Zone_Progress * 15f);

            if (Zone_Progress >= (59f / 60f))
            {
                Diamond_15.color = Blue_Diamond_Color;
            }

            if (Zone_Diamond_Index != Previous_Zone_Diamond_Index)
            {
                Previous_Zone_Diamond_Index = Zone_Diamond_Index;

                if (Zone_Diamond_Index < 1)
                {
                    for (int i = 0; i < Zone_Diamonds.Length; i++)
                    {
                        Zone_Diamonds[i].color = Color.white;
                    }
                }
                else
                {
                    Lit_Diamond_Index = Zone_Diamond_Index - 1;

                    if (Lit_Diamond_Index < 5)
                    {
                        Zone_Diamonds[Lit_Diamond_Index].color = Green_Diamond_Color;
                    }
                    else if (Lit_Diamond_Index < 10)
                    {
                        Zone_Diamonds[Lit_Diamond_Index].color = Red_Diamond_Color;
                    }
                    else
                    {
                        Zone_Diamonds[Lit_Diamond_Index].color = Blue_Diamond_Color;
                    }
                }
            }

            Current_Zone = Current_Survival_Backend.zone;
            if (Current_Zone != Previous_Zone)
            {
                Previous_Zone = Current_Zone;
                Zone_Number.text = Current_Zone.ToString();
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.UseZoneColorsToggle == true)
            {
                Current_ZonePallete_Colors = NgVirtual.ZonePalleteSettings.CurrentColors;

                Zone_Number.color = Current_ZonePallete_Colors.TrackIllumColor;

                if (Current_ZonePallete_Colors.EnvironmentColor.maxColorComponent > Current_ZonePallete_Colors.TrackFogColor.maxColorComponent)
                {
                    Zone_Name.color = Current_ZonePallete_Colors.EnvironmentColor;
                }
                else
                {
                    Zone_Name.color = Current_ZonePallete_Colors.TrackFogColor;
                }

                Zone_Score.color = Current_ZonePallete_Colors.EnvironmentDetailsColor;
            }
            else
            {
                //set back to initial default colors
                Zone_Number.color = Default_Zone_Number_Color;
                Zone_Name.color = Default_Zone_Name_Color;
                Zone_Score.color = Default_Zone_Score_Color;
            }

            if ((VanillaPlusHUDOptions.ModMenuOptions.PerfectZoneWarningToggle == true && Current_Survival_Backend.perfectZone))
            {
                Current_Perfect_Zone_State = 1;
            }
            else
            {
                Current_Perfect_Zone_State = 0;
            }

            if (Current_Perfect_Zone_State != Previous_Perfect_Zone_State)
            {
                Previous_Perfect_Zone_State = Current_Perfect_Zone_State;
                Perfect_Zone.enabled = Current_Perfect_Zone_State == 1;
            }
        }

        public void ZoneProgress(float progress)
        {
            Zone_Progress = progress;
        }

        public void ZoneTitle(string value)
        {
            Zone_Name.text = value;
        }
    }

    public class Upsurge : ScriptableHud
    {
        public Image Upsurge_Zone_Buildup_Bar_Background;
        public Image Stored_Shield_Charge_Bar_Background;
        public Image Stored_Zones_Bar_Background;

        public Image Upsurge_Zone_Buildup_Bar;
        public Image Stored_Shield_Charge_Bar;
        public Image Stored_Zones_Bar;

        public Text Zones_Full_Warning;
        public Text Target_Attainable_Warning;

        public Image Barrier_Warn_Left;
        public Image Barrier_Warn_Right;
        public Image Barrier_Warn_Center;        
        public Image[] Barrier_Warning_Images;

        public Image Zone_Count_Background;
        public Image Zone_Label_Background;

        public Image Shield_Charge_Background;
        public Image Shield_Label_Background;

        public Text Zone_Count;
        public Text Zone_Label;

        public Text Shield_Charge;
        public Text Shield_Label;

        public Text Barrier_Zone_Count;

        public GmUpsurge Upsurge_Gamemode_Instance;
        public UpsurgeShip Upsurge_Ship_Instance;

        public float Buildup_Bar_Target_Fill_Amount;
        public float Stored_Shield_Bar_Target_Fill_Amount;
        public float Stored_Zones_Bar_Target_Fill_Amount;

        public float Buildup_Bar_Previous_Fill_Amount;
        public float Stored_Shield_Bar_Previous_Fill_Amount;
        public float Stored_Zones_Bar_Previous_Fill_Amount;

        public float Buildup_Bar_Interpolation_Ratio;
        public float Stored_Shield_Bar_Interpolation_Ratio;
        public float Stored_Zones_Bar_Interpolation_Ratio;        

        public Vector4 Default_Buildup_Bar_Background_Color;
        public Vector4 Default_Buildup_Bar_Color;
        public Vector4 Default_Stored_Shield_Bar_Background_Color;
        public Vector4 Default_Stored_Shield_Bar_Color;
        public Vector4 Default_Stored_Zones_Bar_Background_Color;
        public Vector4 Default_Stored_Zones_Bar_Color;
        public Vector4 Default_Zone_Count_Background_Color;
        public Vector4 Default_Zone_Count_Color;
        public Vector4 Default_Shield_Charge_Background_Color;
        public Vector4 Default_Shield_Charge_Color;
        public Vector4 Default_Zone_Label_Background_Color;
        public Vector4 Default_Zone_Label_Color;
        public Vector4 Default_Shield_Label_Background_Color;
        public Vector4 Default_Shield_Label_Color;

        public static readonly Vector2 Raised_Barrier_Warning_Adjust_Vector = new Vector2(0, 816);
        public Vector2 Lowered_Barrier_Left_Warning_Starting_Position_Vector;
        public Vector2 Lowered_Barrier_Center_Warning_Starting_Position_Vector;
        public Vector2 Lowered_Barrier_Right_Warning_Starting_Position_Vector;
        public Vector2 Lowered_Barrier_Zone_Count_Starting_Position_Vector;

        public int Current_Built_Zones;
        public int Previous_Built_Zones = -1;
        public int Current_Shield_Charge;
        public int Current_Zone_Number;
        public int Previous_Zone_Number = -1;
        public float Current_Zone_Time;
        public int Current_Zones_Full_State;
        public int Previous_Zones_Full_State = -1;
        public int Current_Target_Attainable_State;
        public int Previous_Target_Attainable_State = -1;
        public int Current_Use_Upsurge_Colors;
        public int Previous_Use_Upsurge_Colors = -1;

        public NgVirtual.ZonePalleteSettings.ZoneColors Current_UpsurgePallete_Colors;
        public Coroutine[] Barrier_Warning_Coroutines = new Coroutine[3];

        public float Cached_Delta_Time;

        public override void Start()
        {
            base.Start();         

            Upsurge_Zone_Buildup_Bar_Background = CustomComponents.GetById<Image>("UpsurgeZoneBuildupBarBackground");
            Stored_Shield_Charge_Bar_Background = CustomComponents.GetById<Image>("StoredShieldChargeBarBackground");
            Stored_Zones_Bar_Background = CustomComponents.GetById<Image>("StoredZonesBarBackground");
            Default_Buildup_Bar_Background_Color = Upsurge_Zone_Buildup_Bar_Background.color;
            Default_Stored_Shield_Bar_Background_Color = Stored_Shield_Charge_Bar_Background.color;
            Default_Stored_Zones_Bar_Background_Color = Stored_Zones_Bar_Background.color;            

            Upsurge_Zone_Buildup_Bar = CustomComponents.GetById<Image>("UpsurgeZoneBuildupBar");
            Stored_Shield_Charge_Bar = CustomComponents.GetById<Image>("StoredShieldChargeBar");
            Stored_Zones_Bar = CustomComponents.GetById<Image>("StoredZonesBar");
            Default_Buildup_Bar_Color = Upsurge_Zone_Buildup_Bar.color;
            Default_Stored_Shield_Bar_Color = Stored_Shield_Charge_Bar.color;
            Default_Stored_Zones_Bar_Color = Stored_Zones_Bar.color;

            Zones_Full_Warning = CustomComponents.GetById<Text>("ZonesFullWarning");
            Target_Attainable_Warning = CustomComponents.GetById<Text>("TargetAttainableWarning");

            Zones_Full_Warning.enabled = false;
            Target_Attainable_Warning.enabled = false;

            Barrier_Warn_Left = CustomComponents.GetById<Image>("BarrierWarnLeft");
            Barrier_Warn_Right = CustomComponents.GetById<Image>("BarrierWarnRight");
            Barrier_Warn_Center = CustomComponents.GetById<Image>("BarrierWarnCenter");

            Lowered_Barrier_Left_Warning_Starting_Position_Vector = Barrier_Warn_Left.rectTransform.anchoredPosition;
            Lowered_Barrier_Center_Warning_Starting_Position_Vector = Barrier_Warn_Center.rectTransform.anchoredPosition;
            Lowered_Barrier_Right_Warning_Starting_Position_Vector = Barrier_Warn_Right.rectTransform.anchoredPosition;

            Zone_Count_Background = CustomComponents.GetById<Image>("ZoneCountBackground");
            Zone_Label_Background = CustomComponents.GetById<Image>("ZoneLabelBackground");
            Default_Zone_Count_Background_Color = Zone_Count_Background.color;
            Default_Zone_Label_Background_Color = Zone_Label_Background.color;

            Shield_Charge_Background = CustomComponents.GetById<Image>("ShieldChargeBackground");
            Shield_Label_Background = CustomComponents.GetById<Image>("ShieldLabelBackground");
            Default_Shield_Charge_Background_Color = Shield_Charge_Background.color;
            Default_Shield_Label_Background_Color = Shield_Label_Background.color;

            Zone_Count = CustomComponents.GetById<Text>("ZoneCount");
            Zone_Label = CustomComponents.GetById<Text>("ZoneLabel");
            Default_Zone_Count_Color = Zone_Count.color;
            Default_Zone_Label_Color = Zone_Label.color;

            Shield_Charge = CustomComponents.GetById<Text>("ShieldCharge");
            Shield_Label = CustomComponents.GetById<Text>("ShieldLabel");
            Default_Shield_Charge_Color = Shield_Charge.color;
            Default_Shield_Label_Color = Shield_Label.color;

            Barrier_Zone_Count = CustomComponents.GetById<Text>("BarrierZoneCount");
            Lowered_Barrier_Zone_Count_Starting_Position_Vector = Barrier_Zone_Count.rectTransform.anchoredPosition;

            Barrier_Warning_Images = new Image[] { Barrier_Warn_Left, Barrier_Warn_Center, Barrier_Warn_Right };

            NgPickups.Physical.Barrier.OnPlayerBarrierWarned += BarrierWarning;
            //NgPickups.Physical.Barrier.OnAiBarrierWarned += BarrierWarning;

            Upsurge_Zone_Buildup_Bar.fillAmount = 0f;
            Stored_Shield_Charge_Bar.fillAmount = 0f;
            Stored_Zones_Bar.fillAmount = 0f;

            if (VanillaPlusHUDOptions.ModMenuOptions.BarrierWarningPosition == 0) //BarrierWarningPosition = top
            {
                Barrier_Zone_Count.rectTransform.anchoredPosition = Lowered_Barrier_Zone_Count_Starting_Position_Vector + Raised_Barrier_Warning_Adjust_Vector;
                Barrier_Warning_Images[0].rectTransform.anchoredPosition = Lowered_Barrier_Left_Warning_Starting_Position_Vector + Raised_Barrier_Warning_Adjust_Vector;
                Barrier_Warning_Images[1].rectTransform.anchoredPosition = Lowered_Barrier_Center_Warning_Starting_Position_Vector + Raised_Barrier_Warning_Adjust_Vector;
                Barrier_Warning_Images[2].rectTransform.anchoredPosition = Lowered_Barrier_Right_Warning_Starting_Position_Vector + Raised_Barrier_Warning_Adjust_Vector;
            }

            Upsurge_Gamemode_Instance = RaceManager.CurrentGamemode as GmUpsurge;
        }

        public override void OnDestroy()
        {
            NgPickups.Physical.Barrier.OnPlayerBarrierWarned -= BarrierWarning;

            base.OnDestroy();
        }

        public override void Update()
        {
            base.Update();

            Cached_Delta_Time = Time.deltaTime;

            if (Upsurge_Ship_Instance == null)
            {
                int shipCount = Upsurge_Gamemode_Instance.Ships.Count;

                for (int i = 0; i < shipCount; i++)
                {
                    UpsurgeShip upsurgeShip = Upsurge_Gamemode_Instance.Ships[i];

                    if (upsurgeShip.TargetShip == TargetShip)
                    {
                        Upsurge_Ship_Instance = upsurgeShip;
                        break;
                    }
                }

                if (Upsurge_Ship_Instance == null)
                {
                    return;
                }
            }

            Current_Built_Zones = Upsurge_Ship_Instance.BuiltZones;
            Current_Zone_Number = Upsurge_Ship_Instance.CurrentZone;
            Current_Zone_Time = Upsurge_Ship_Instance.ZoneTime;

            if (Current_Built_Zones != Previous_Built_Zones)
            {
                Previous_Built_Zones = Current_Built_Zones;

                Current_Shield_Charge = Mathf.Clamp(Current_Built_Zones * 20, 0, 100);

                Zone_Count.text = Current_Built_Zones.ToString("+0");
                Shield_Charge.text = Current_Shield_Charge.ToString("+0");

                Stored_Zones_Bar_Target_Fill_Amount = Current_Built_Zones / 10f;
                Stored_Shield_Bar_Target_Fill_Amount = Current_Shield_Charge / 100f;
            }

            if (Current_Zone_Number != Previous_Zone_Number)
            {
                Previous_Zone_Number = Current_Zone_Number;
                Barrier_Zone_Count.text = Current_Zone_Number.ToString();
            }

            Buildup_Bar_Target_Fill_Amount = Current_Zone_Time / 5f;

            if (Stored_Zones_Bar_Target_Fill_Amount > Stored_Zones_Bar_Previous_Fill_Amount)
            {
                Stored_Zones_Bar_Interpolation_Ratio += Cached_Delta_Time;
                Stored_Zones_Bar.fillAmount = Mathf.Lerp(Stored_Zones_Bar_Previous_Fill_Amount, Stored_Zones_Bar_Target_Fill_Amount, Stored_Zones_Bar_Interpolation_Ratio);
            }
            else
            {
                Stored_Zones_Bar_Interpolation_Ratio = 0f;
                if (Stored_Zones_Bar_Target_Fill_Amount < Stored_Zones_Bar.fillAmount)
                {
                    Stored_Zones_Bar.fillAmount = Stored_Zones_Bar_Target_Fill_Amount;
                }
            }

            if (Stored_Shield_Bar_Target_Fill_Amount > Stored_Shield_Bar_Previous_Fill_Amount)
            {
                Stored_Shield_Bar_Interpolation_Ratio += Cached_Delta_Time;
                Stored_Shield_Charge_Bar.fillAmount = Mathf.Lerp(Stored_Shield_Bar_Previous_Fill_Amount, Stored_Shield_Bar_Target_Fill_Amount, Stored_Shield_Bar_Interpolation_Ratio);
            }
            else
            {
                Stored_Shield_Bar_Interpolation_Ratio = 0f;
                if (Stored_Shield_Bar_Target_Fill_Amount < Stored_Shield_Charge_Bar.fillAmount)
                {
                    Stored_Shield_Charge_Bar.fillAmount = Stored_Shield_Bar_Target_Fill_Amount;
                }
            }

            if (Buildup_Bar_Target_Fill_Amount > Buildup_Bar_Previous_Fill_Amount)
            {
                Buildup_Bar_Interpolation_Ratio += Cached_Delta_Time;
                Upsurge_Zone_Buildup_Bar.fillAmount = Mathf.Lerp(Buildup_Bar_Previous_Fill_Amount, Buildup_Bar_Target_Fill_Amount, Buildup_Bar_Interpolation_Ratio);
            }
            else
            {
                if (Current_Zone_Time == 0f)
                {
                    Upsurge_Zone_Buildup_Bar.fillAmount = 0f;
                }
                Buildup_Bar_Interpolation_Ratio = 0f;
            }

            Stored_Zones_Bar_Previous_Fill_Amount = Stored_Zones_Bar.fillAmount;
            Stored_Shield_Bar_Previous_Fill_Amount = Stored_Shield_Charge_Bar.fillAmount;
            Buildup_Bar_Previous_Fill_Amount = Upsurge_Zone_Buildup_Bar.fillAmount;

            Current_Zones_Full_State = (Current_Built_Zones == 10 && VanillaPlusHUDOptions.ModMenuOptions.ZonesFullWarningToggle == true) ? 1 : 0;
            if (Current_Zones_Full_State != Previous_Zones_Full_State)
            {
                Previous_Zones_Full_State = Current_Zones_Full_State;
                Zones_Full_Warning.enabled = Current_Zones_Full_State == 1;
            }

            Current_Target_Attainable_State = ((Current_Zone_Number + Current_Built_Zones) >= Upsurge_Ship_Instance.Target && VanillaPlusHUDOptions.ModMenuOptions.TargetAttainableWarningToggle == true) ? 1 : 0;
            if (Current_Target_Attainable_State != Previous_Target_Attainable_State)
            {
                Previous_Target_Attainable_State = Current_Target_Attainable_State;
                Target_Attainable_Warning.enabled = Current_Target_Attainable_State == 1;
            }
            
            if (VanillaPlusHUDOptions.ModMenuOptions.UseUpsurgeColorsToggle == true)
            {
                Current_Use_Upsurge_Colors = 1;
            }
            else
            {
                Current_Use_Upsurge_Colors = 0;
            }

            if (Current_Use_Upsurge_Colors == 1)
            {
                Current_UpsurgePallete_Colors = NgVirtual.ZonePalleteSettings.CurrentColors;

                Zone_Label_Background.color = Current_UpsurgePallete_Colors.TrackIllumColor;
                Zone_Count_Background.color = Current_UpsurgePallete_Colors.TrackIllumColor;
                Zone_Label.color = Current_UpsurgePallete_Colors.TrackColor;
                Zone_Count.color = Current_UpsurgePallete_Colors.TrackColor;

                Shield_Charge_Background.color = Current_UpsurgePallete_Colors.EqBackgroundColor;
                Shield_Label_Background.color = Current_UpsurgePallete_Colors.EqBackgroundColor;
                Shield_Charge.color = Current_UpsurgePallete_Colors.EnvironmentDetailsColor;
                Shield_Label.color = Current_UpsurgePallete_Colors.EnvironmentDetailsColor;

                Upsurge_Zone_Buildup_Bar_Background.color = Current_UpsurgePallete_Colors.TrackIllumColor; //Bottom bar, where shield/energy normally are
                Upsurge_Zone_Buildup_Bar.color = Current_UpsurgePallete_Colors.EnvironmentColor;

                Stored_Zones_Bar_Background.color = Current_UpsurgePallete_Colors.TrackColor; //Top bar
                Stored_Zones_Bar.color = Current_UpsurgePallete_Colors.TrackIllumColor;

                Stored_Shield_Charge_Bar_Background.color = Current_UpsurgePallete_Colors.EnvironmentDetailsColor; //Middle bar
                Stored_Shield_Charge_Bar.color = Current_UpsurgePallete_Colors.EqBackgroundColor;
            }
            else if (Previous_Use_Upsurge_Colors != 0)
            {
                //set back to initial default colors
                Zone_Label_Background.color = Default_Zone_Label_Background_Color;
                Zone_Count_Background.color = Default_Zone_Count_Background_Color;
                Zone_Label.color = Default_Zone_Label_Color;
                Zone_Count.color = Default_Zone_Count_Color;

                Shield_Charge_Background.color = Default_Shield_Charge_Background_Color;
                Shield_Label_Background.color = Default_Shield_Label_Background_Color;
                Shield_Charge.color = Default_Shield_Charge_Color;
                Shield_Label.color = Default_Shield_Label_Color;

                Upsurge_Zone_Buildup_Bar_Background.color = Default_Buildup_Bar_Background_Color; //Bottom bar, where shield/energy normally are
                Upsurge_Zone_Buildup_Bar.color = Default_Buildup_Bar_Color;

                Stored_Zones_Bar_Background.color = Default_Stored_Zones_Bar_Background_Color; //Top bar
                Stored_Zones_Bar.color = Default_Stored_Zones_Bar_Color;

                Stored_Shield_Charge_Bar_Background.color = Default_Stored_Shield_Bar_Background_Color;//Middle bar
                Stored_Shield_Charge_Bar.color = Default_Stored_Shield_Bar_Color;
            }

            Previous_Use_Upsurge_Colors = Current_Use_Upsurge_Colors;
        }

        public void BarrierWarning(ShipController ship, NgPickups.Physical.Barrier barrier, int trackSide)
        {
            if (NgSettings.Gameplay.MirrorEnabled)
            {
                trackSide *= -1;
            }

            int Barrier_Side = trackSide + 1;

            if (Barrier_Warning_Coroutines[Barrier_Side] != null)
            {
                StopCoroutine(Barrier_Warning_Coroutines[Barrier_Side]);
            }

            Barrier_Warning_Coroutines[Barrier_Side] = StartCoroutine(Barrier_Warning_Flasher(Barrier_Side));
        }

        IEnumerator Barrier_Warning_Flasher(int Barrier_Side)
        {
            const float Flash_Interval = 0.125f;
            const float Warning_Duration = 2.375f;

            Image Warning_Image = Barrier_Warning_Images[Barrier_Side];
            float Elapsed_Time = 0f;

            while (Elapsed_Time < Warning_Duration)
            {
                if (((int)(Elapsed_Time / Flash_Interval) % 2) == 0)
                {
                    Warning_Image.color = Color.red;
                }
                else
                {
                    Warning_Image.color = Color.white;
                }

                yield return null;
                Elapsed_Time += Time.deltaTime;
            }

            Warning_Image.color = Color.white;
        }
    }

    public class Stunt : ScriptableHud
    {
        public static readonly Vector4 Ballistic_NG_Orange = new Vector4(1f, (173f / 255f), 0f, 1f);

        public Image Stunt_Score_Field;
        public Image Stunt_Score_Accent;
        public Text Stunt_Score_Name;
        public Text Stunt_Score_Readout;

        public Image Stunt_Score_Best_Field;
        public Image Stunt_Score_Best_Accent;
        public Text Stunt_Score_Best_Name;
        public Text Stunt_Score_Best_Readout;

        public Text Stunt_Height_Readout;
        public Text Stunt_Chain_Readout;
        public Image Stunt_Height_Diamond;
        public Image Stunt_Chain_Bar_Background;
        public Image Stunt_Chain_Bar;
        public Image Roll_Input_Spam_Bar;

        public Text Uplift_Name;
        public Image Uplift_Bar_Background;
        public Image Uplift_Bar;
        public Image Uplift_Pip_1;
        public Image Uplift_Pip_2;
        public Image Uplift_Pip_3;

        public GmStunt Stunt_Gamemode_Instance;
        public StuntPlayer Stunt_Player_Instance;

        public int Current_Best_Score_Key;
        public int Previous_Best_Score_Key = -1;
        public int Current_Chain_Amount;
        public int Previous_Chain_Amount = -1;
        public float Current_Height;
        public int Current_Height_Key;
        public int Previous_Height_Key = -1;
        public float Current_Shield;
        public int Current_Uplift_Pips;
        public int Previous_Uplift_Pips = -1;
        public int Current_Hud_Show_Input;
        public int Previous_Hud_Show_Input = -1;

        public override void Start()
        {
            base.Start();

            Stunt_Score_Field = CustomComponents.GetById<Image>("StuntScoreField");
            Stunt_Score_Accent = CustomComponents.GetById<Image>("StuntScoreAccent");
            Stunt_Score_Name = CustomComponents.GetById<Text>("StuntScoreName");
            Stunt_Score_Readout = CustomComponents.GetById<Text>("StuntScoreReadout");

            Stunt_Score_Best_Field = CustomComponents.GetById<Image>("StuntScoreBestField");
            Stunt_Score_Best_Accent = CustomComponents.GetById<Image>("StuntScoreBestAccent");
            Stunt_Score_Best_Name = CustomComponents.GetById<Text>("StuntScoreBestName");
            Stunt_Score_Best_Readout = CustomComponents.GetById<Text>("StuntScoreBestReadout");

            Stunt_Height_Readout = CustomComponents.GetById<Text>("StuntHeightReadout");
            Stunt_Chain_Readout = CustomComponents.GetById<Text>("StuntChainReadout");
            Stunt_Height_Diamond = CustomComponents.GetById<Image>("StuntHeightDiamond");
            Stunt_Chain_Bar_Background = CustomComponents.GetById<Image>("StuntChainBarBackground");
            Stunt_Chain_Bar = CustomComponents.GetById<Image>("StuntChainBar");
            Roll_Input_Spam_Bar = CustomComponents.GetById<Image>("Roll_InputSpamBar");

            Uplift_Name = CustomComponents.GetById<Text>("UpliftName");
            Uplift_Bar_Background = CustomComponents.GetById<Image>("UpliftBarBackground");
            Uplift_Bar = CustomComponents.GetById<Image>("UpliftBar");
            Uplift_Pip_1 = CustomComponents.GetById<Image>("UpliftPip1");
            Uplift_Pip_2 = CustomComponents.GetById<Image>("UpliftPip2");
            Uplift_Pip_3 = CustomComponents.GetById<Image>("UpliftPip3");

            Stunt_Score_Readout.text = "0";
            Stunt_Score_Best_Readout.text = "0";

            NgRaceEvents.OnShipScoreChanged += StuntScore;
        }

        public override void OnDestroy()
        {
            NgRaceEvents.OnShipScoreChanged -= StuntScore;

            base.OnDestroy();
        }

        public void StuntScore(ShipController ship, float oldScore, float newScore)
        {
            if (ship == TargetShip)
            {
                Stunt_Score_Readout.text = newScore.ToString("N0");
            }
        }

        public override void Update()
        {
            base.Update();

            if (Stunt_Player_Instance == null)
            {
                Stunt_Gamemode_Instance = RaceManager.CurrentGamemode as GmStunt;

                if (Stunt_Gamemode_Instance == null || Stunt_Gamemode_Instance.Players == null)
                {
                    return;
                }

                int playerCount = Stunt_Gamemode_Instance.Players.Count;

                for (int i = 0; i < playerCount; i++)
                {
                    StuntPlayer stuntPlayer = Stunt_Gamemode_Instance.Players[i];

                    if (stuntPlayer != null && stuntPlayer.R == TargetShip)
                    {
                        Stunt_Player_Instance = stuntPlayer;
                        break;
                    }
                }

                if (Stunt_Player_Instance == null)
                {
                    return;
                }
            }

            Current_Best_Score_Key = (int)(Stunt_Gamemode_Instance.BestScore + 0.5f);
            if (Current_Best_Score_Key != Previous_Best_Score_Key)
            {
                Previous_Best_Score_Key = Current_Best_Score_Key;
                Stunt_Score_Best_Readout.text = Current_Best_Score_Key.ToString("N0");
            }

            Current_Chain_Amount = Stunt_Player_Instance.ChainAmount;
            if (Current_Chain_Amount != Previous_Chain_Amount)
            {
                Previous_Chain_Amount = Current_Chain_Amount;
                Stunt_Chain_Readout.text = "×" + Current_Chain_Amount.ToString();
                Stunt_Chain_Readout.enabled = Current_Chain_Amount >= 1;
            }

            Current_Height = (TargetShip.InterpolatedSection.InverseTransformPoint(TargetShip.PhysicsPosition).y * 18f) - 6f;
            if (Current_Height <= 0f)
            {
                Current_Height_Key = 0;
            }
            else
            {
                Current_Height_Key = (int)(Current_Height + 0.5f);
            }

            if (Current_Height_Key != Previous_Height_Key)
            {
                Previous_Height_Key = Current_Height_Key;
                Stunt_Height_Readout.text = Current_Height_Key.ToString("0'm'");
            }

            Stunt_Chain_Bar.fillAmount = (Stunt_Player_Instance.ChainTimer / 3f);
            Roll_Input_Spam_Bar.fillAmount = Stunt_Player_Instance.RollInputCooldown;

            Current_Shield = TargetShip.ShieldIntegrity;
            Uplift_Bar.fillAmount = (Current_Shield / 100f);

            if (Current_Shield == 100f)
            {
                Current_Uplift_Pips = 3;
            }
            else if (Current_Shield >= (200f / 3f))
            {
                Current_Uplift_Pips = 2;
            }
            else if (Current_Shield >= (100f / 3f))
            {
                Current_Uplift_Pips = 1;
            }
            else
            {
                Current_Uplift_Pips = 0;
            }

            if (Current_Uplift_Pips != Previous_Uplift_Pips)
            {
                Previous_Uplift_Pips = Current_Uplift_Pips;

                if (Current_Uplift_Pips >= 1) { Uplift_Pip_1.color = Lap_Time_Field.Lap_Diamond_Green; } else { Uplift_Pip_1.color = Lap_Time_Field.Lap_Diamond_Red; }
                if (Current_Uplift_Pips >= 2) { Uplift_Pip_2.color = Lap_Time_Field.Lap_Diamond_Green; } else { Uplift_Pip_2.color = Lap_Time_Field.Lap_Diamond_Red; }
                if (Current_Uplift_Pips >= 3) { Uplift_Pip_3.color = Lap_Time_Field.Lap_Diamond_Green; } else { Uplift_Pip_3.color = Lap_Time_Field.Lap_Diamond_Red; }
            }

            Current_Hud_Show_Input = Stunt_Player_Instance.HudShowInput ? 1 : 0;
            if (Current_Hud_Show_Input != Previous_Hud_Show_Input)
            {
                Previous_Hud_Show_Input = Current_Hud_Show_Input;

                if (Current_Hud_Show_Input == 1)
                {
                    Stunt_Height_Diamond.color = Ballistic_NG_Orange;
                    Stunt_Height_Readout.color = Ballistic_NG_Orange;
                    Stunt_Chain_Readout.color = Ballistic_NG_Orange;
                    Stunt_Chain_Bar.color = Ballistic_NG_Orange;
                }
                else
                {
                    Stunt_Height_Diamond.color = Color.white;
                    Stunt_Height_Readout.color = Color.white;
                    Stunt_Chain_Readout.color = Color.white;
                    Stunt_Chain_Bar.color = Color.white;
                }
            }
        }
    }
}