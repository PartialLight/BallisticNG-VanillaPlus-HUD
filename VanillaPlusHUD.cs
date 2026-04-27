using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using NgModding.Huds;
using NgModding;
using NgMp;
using NgMusic;
using NgPickups;
using NgShips;
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

        public override void OnRegistered(string modPath)
        {
            NgNetworkBase.OnNetworkStart += ConnectToLobby;
            End_Beep = LoadWavFile((modPath + "\\Audio\\mpcountdownend.wav"));

            string TestPathString = Path.Combine(modPath, "config.ini");
            INIParser ini = new INIParser();
            ini.Open(TestPathString);

            VanillaPlusHUD = AssetBundle.LoadFromFile(Path.Combine(modPath, HudPath));
            CustomHudRegistry.RegisterMod(id);
            CustomHudRegistry.RegisterSceneManager("Race", id, new HudManager());

            CustomHudRegistry.RegisterWeaponSprite("autopilot", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/AutoPilot.png")));
            CustomHudRegistry.RegisterWeaponSprite("cannon", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Cannon.png")));
            CustomHudRegistry.RegisterWeaponSprite("energywall", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/EnergyWall.png")));
            CustomHudRegistry.RegisterWeaponSprite("emergencypack", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/EPack.png")));
            CustomHudRegistry.RegisterWeaponSprite("hellstorm", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Hellstorm.png")));
            CustomHudRegistry.RegisterWeaponSprite("hunter", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(modPath, "Weapons/Hunter.png")));
            //switch (VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle)
            switch (ini.ReadValue("Settings", "MissileIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle))
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
            switch (ini.ReadValue("Settings", "RocketsIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.RocketsIconStyle))
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
            if (type.ToString() == "StartGameSound")
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

    public class HudManager : SceneHudManager //Mothership
    {
        public override void OnCreateHuds()
        {
            if (VanillaPlusHUDOptions.ModMenuOptions.OvertakeRadarToggle)
            {
                RegisterHud<Overtake_Radar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Overtake Radar.prefab");
            }
            
            RegisterHud<Extra_Warnings>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Extra Warnings.prefab");
            RegisterHud<Camera_Rotation_Overrides_2280>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");
            RegisterHud<Camera_Height_Adjustments>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");

            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");

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

            RegisterHud<Lap_Counter>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Lap Counter FIeld.prefab");

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

            RegisterHud<Best_Time_Field>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Best Time FIeld.prefab");

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
            
            RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn

            if (VanillaPlusHUDOptions.ModMenuOptions.ForceShieldBars)
            {
                RegisterInternalHud("Eliminator"); //Shield bars
            }
            
            RegisterInternalHud("NetworkPeerList"); //Positionboard on the right side of the screen
            RegisterInternalHud("KnockoutShipTracker"); //The position/lap progress tracker on the left side of the screen
            RegisterInternalHud("NotificationBuffer"); //Have to make my own
            RegisterInternalHud("WrongWayDisplay"); //Wrong Way indicator
            if (NgNetworkBase.CurrentNetwork != null)
            {
                RegisterInternalHud("NetworkWaitingList"); //Waiting for PLAYER at start of race while people are loading
            }
            RegisterInternalHud("NetworkRaceFinish"); //Race finishes in 30 seconds
        }
    }

    public class Last_Attacker : ScriptableHud
    {
        public Text Last_Attacker_Name;

        public override void Start()
        {
            base.Start();

            Last_Attacker_Name = CustomComponents.GetById<Text>("LastAttackerName");
            Last_Attacker_Name.text = "";
            Last_Attacker_Name.enabled = false;
        }

        public override void Update()
        {
            base.Update();

            Last_Attacker_Name.enabled = true;

            if (TargetShip.LastAttacker.IsAi)
            {
                Last_Attacker_Name.text = TargetShip.LastAttacker.ShipName;
            }
            else
            {
                Last_Attacker_Name.text = TargetShip.LastAttacker.ShipName;
            }
            
        }
    }

    public class Recharge_Sum : ScriptableHud
    {
        public Text Recharge_Sum_Readout;
        public float Time_Spent_Recharging;
        public float Potential_Energy_Recharge;
        public float Energy_Before_Recharge;
        public float Energy_Difference;

        public override void Start()
        {
            base.Start();

            Recharge_Sum_Readout = CustomComponents.GetById<Text>("RechargeSumReadout");
            Recharge_Sum_Readout.text = "";
            Recharge_Sum_Readout.enabled = false;
            
        }

        public override void Update()
        {
            base.Update();

            if (TargetShip.IsRecharging)
            {
                Recharge_Sum_Readout.enabled = true;

                Time_Spent_Recharging += Time.deltaTime;
                Potential_Energy_Recharge = Time_Spent_Recharging * 20f;

                StartCoroutine(Recharge_Sum_Duration());
            }
            else
            {
                Energy_Before_Recharge = TargetShip.ShieldIntegrity;
            }
        }

        IEnumerator Recharge_Sum_Duration()
        {

            while (TargetShip.IsRecharging)
            {
                if (TargetShip.ShieldIntegrity < 100)
                {
                    Energy_Difference = TargetShip.ShieldIntegrity - Energy_Before_Recharge;
                    Recharge_Sum_Readout.text = "+" + string.Format("{0:N1}", Energy_Difference);
                }
                else if (TargetShip.ShieldIntegrity >= 100)
                {
                    Recharge_Sum_Readout.text = "∆" + string.Format("{0:N1}", Mathf.Max(Energy_Difference, Potential_Energy_Recharge));
                }

                yield return null;
            }

            float Recharge_Linger_Start_Time = Time.time;
            float Recharge_Linger_End_Time = Time.time + 1.5f;

            while (Recharge_Linger_Start_Time <= Recharge_Linger_End_Time)
            {
                Recharge_Linger_Start_Time += Time.deltaTime;

                yield return null;
            }
            Recharge_Sum_Readout.text = "";
            Time_Spent_Recharging = 0f;
            Energy_Difference = 0f;
            Recharge_Sum_Readout.enabled = false;
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

        public int Pitlane_Indicator_Section_Index;
        public int Pitlane_Side;
        public int Pitlane_Current_Section;
        public int Pitlane_Previous_Section;

        public Image[] Left_Pitlane_Arrows;
        public Image[] Right_Pitlane_Arrows;
        public Image[] Pitlane_Arrows;

        public Vector2 Arrow_1_Start_Position;
        public Vector2 Arrow_2_Start_Position;
        public Vector2 Arrow_3_Start_Position;
        public float Arrow_Shift_Distance;
        public Vector2 Arrow_Shift_Vector;

        public float Pitlane_Indicator_Animation_Duration;
        public float Pitlane_Indicator_Animation_Start_Time;
        public float Pitlane_Indicator_Animation_End_Time;

        public float First_Arrow_Blink_On_1_Time;
        public float First_Arrow_Blink_Off_Time;
        public float First_Arrow_Blink_On_2_Time;

        public float Second_Arrow_Blink_On_1_Time;
        public float Second_Arrow_Blink_Off_Time;
        public float Second_Arrow_Blink_On_2_Time;

        public float Third_Arrow_Blink_On_1_Time;
        public float Third_Arrow_Blink_Off_Time;
        public float Third_Arrow_Blink_On_2_Time;

        public float First_Arrow_Pre_Blink_Time;

        public float First_Arrow_End_Animation_Time_1;
        public float First_Arrow_End_Animation_Time_2;
        public float First_Arrow_End_Animation_Time_3;

        public float Second_Arrow_End_Animation_Time_1;
        public float Second_Arrow_End_Animation_Time_2;
        public float Second_Arrow_End_Animation_Time_3;

        public float Third_Arrow_End_Animation_Time_1;
        public float Third_Arrow_End_Animation_Time_2;
        public float Third_Arrow_End_Animation_Time_3;


        IEnumerator Pitlane_Indicator_Animation()
        {
            Pitlane_Indicator_Animation_Duration = 4.35f;
            Pitlane_Indicator_Animation_Start_Time = Time.time;
            Pitlane_Indicator_Animation_End_Time = Pitlane_Indicator_Animation_Start_Time + Pitlane_Indicator_Animation_Duration;

            First_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 0.15f;
            First_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time + 0.3f;
            First_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 0.45f;

            Second_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 0.6f;
            Second_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time +0.75f;
            Second_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 0.9f;

            Third_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 1.05f;
            Third_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time + 1.2f;
            Third_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 1.35f;

            //FIRST ARROW PRE-BLINK
            First_Arrow_Pre_Blink_Time = Pitlane_Indicator_Animation_Start_Time + 2.85f;

            //MoveToward and blink off, maybe reduce fill over time
            First_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3f;
            First_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 3.15f;
            First_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 3.3f;

            Second_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3.45f;
            Second_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 3.6f;
            Second_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 3.75f;

            Third_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3.9f;
            Third_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 4.05f;
            Third_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 4.2f;

            Arrow_1_Start_Position = Pitlane_Arrows[0].rectTransform.anchoredPosition;
            Arrow_2_Start_Position = Pitlane_Arrows[1].rectTransform.anchoredPosition;
            Arrow_3_Start_Position = Pitlane_Arrows[2].rectTransform.anchoredPosition;
            Arrow_Shift_Distance = 21f * Pitlane_Side;
            Arrow_Shift_Vector = new Vector2(Arrow_Shift_Distance, 0);


            while (Time.time <= Pitlane_Indicator_Animation_End_Time)
            {
                

                //FIRST ARROW
                if (Pitlane_Indicator_Animation_Start_Time < Time.time && Time.time <= First_Arrow_Blink_On_1_Time) //Blink first arrow + pit text on
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 1.034f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }
                if (First_Arrow_Blink_On_1_Time < Time.time && Time.time <= First_Arrow_Blink_Off_Time) //Blink first arrow + pit text off
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = false;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 1.034f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }
                if (First_Arrow_Blink_Off_Time < Time.time && Time.time <= First_Arrow_Blink_On_2_Time) //Blink first arrow + pit text back on, first arrow should be finished filling after this completes
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 1.034f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }

                //SECOND ARROW
                if (First_Arrow_Blink_On_2_Time < Time.time && Time.time <= Second_Arrow_Blink_On_1_Time) //Blink second arrow on
                {
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 1.034f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }
                if (Second_Arrow_Blink_On_1_Time < Time.time && Time.time <= Second_Arrow_Blink_Off_Time) //Blink second arrow off
                {
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 1.034f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }
                if (Second_Arrow_Blink_Off_Time < Time.time && Time.time <= Second_Arrow_Blink_On_2_Time) //Blink second arrow back on, second arrow should be finished filling after this completes
                {
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 1.034f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }

                //THIRD ARROW
                if (Second_Arrow_Blink_On_2_Time < Time.time && Time.time <= Third_Arrow_Blink_On_1_Time) //Blink third arrow on
                {
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 1.034f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }
                if (Third_Arrow_Blink_On_1_Time < Time.time && Time.time <= Third_Arrow_Blink_Off_Time) //Blink third arrow off
                {
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 1.034f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }
                if (Third_Arrow_Blink_Off_Time < Time.time && Time.time <= Third_Arrow_Blink_On_2_Time) //Blink third arrow back on, third arrow should be finished filling after this completes
                {
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 1.034f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }

                //INTERMISSION
                if (First_Arrow_Pre_Blink_Time < Time.time && Time.time <= First_Arrow_End_Animation_Time_1)
                {
                    Pitlane_Arrows[0].enabled = false;
                }

                //End of the animation, FIRST ARROW
                if (First_Arrow_End_Animation_Time_1 < Time.time && Time.time <= First_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[0].fillAmount = 1f - ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1));
                }
                if (First_Arrow_End_Animation_Time_2 < Time.time && Time.time <= First_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = false;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                }
                if (First_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_1)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[0].fillAmount = 1f - ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1));
                }
                //End of the animation, SECOND ARROW
                if (Second_Arrow_End_Animation_Time_1 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Arrows[0].fillAmount = 0f;
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[1].fillAmount = 1f - ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1));
                }
                if (Second_Arrow_End_Animation_Time_2 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                }
                if (Second_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_1)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[1].fillAmount = 1f - ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1));
                }
                //End of the animation, THIRD ARROW
                if (Third_Arrow_End_Animation_Time_1 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Arrows[1].fillAmount = 0f;
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[2].fillAmount = 1f - ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1));
                }
                if (Third_Arrow_End_Animation_Time_2 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                }
                if (Third_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Pitlane_Indicator_Animation_End_Time)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[2].fillAmount = 1f - ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1));
                }

                yield return null;
            }
            Pitlane_Arrows[0].enabled = false;
            Pitlane_Arrows[1].enabled = false;
            Pitlane_Arrows[2].enabled = false;
            Pitlane_Indicator_Field_Name.enabled = false;

            Pitlane_Arrows[0].rectTransform.anchoredPosition = Arrow_1_Start_Position;
            Pitlane_Arrows[1].rectTransform.anchoredPosition = Arrow_2_Start_Position;
            Pitlane_Arrows[2].rectTransform.anchoredPosition = Arrow_3_Start_Position;
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
            
            Left_Pitlane_Arrows = new Image[] { Left_Pitlane_Indicator_Arrow_1, Left_Pitlane_Indicator_Arrow_2, Left_Pitlane_Indicator_Arrow_3 };
            Right_Pitlane_Arrows = new Image[] { Right_Pitlane_Indicator_Arrow_1, Right_Pitlane_Indicator_Arrow_2, Right_Pitlane_Indicator_Arrow_3 };
        }

        public override void Update()
        {
            base.Update();

            Pitlane_Current_Section = TargetShip.CurrentSection.index;

            
            
            Pitlane_Previous_Section = Pitlane_Current_Section;

        }

        public void Pitlane_Indicator_Method(ShipController ship, int side)
        {
            if (ship.IsPlayer)
            {
                if (side == 1)
                {
                    if (NgSettings.Gameplay.MirrorEnabled)
                    {
                        Pitlane_Side = -1;
                        Pitlane_Arrows = Left_Pitlane_Arrows;
                    }
                    else
                    {
                        Pitlane_Side = 1;
                        Pitlane_Arrows = Right_Pitlane_Arrows;
                    }                    
                }

                else if (side == -1)
                {
                    if (NgSettings.Gameplay.MirrorEnabled)
                    {
                        Pitlane_Side = 1;
                        Pitlane_Arrows = Right_Pitlane_Arrows;
                    }
                    else
                    {
                        Pitlane_Side = -1;
                        Pitlane_Arrows = Left_Pitlane_Arrows;
                    }
                }

                StartCoroutine(Pitlane_Indicator_Animation());
            }
        }

        public override void OnDestroy()
        {
            NgTrackData.Triggers.PitlaneIndicator.OnPitlaneIndicatorTriggered -= Pitlane_Indicator_Method;
        }
    }

    public class Relative_Time_Display : ScriptableHud
    {
        public Text Relative_Time_Readout;
        public static readonly Vector4 Race_Leader_Green = new Vector4(181f / 255f, 1f, 29f / 255f, 1f);
        public static readonly Vector4 Contender_Red = new Vector4(1f, 28f / 255f, 36f / 255f, 1f);
        public static readonly Vector4 Tremor_Safety_Blue = new Vector4(0f, (163f / 256f), (233f / 256f), 1f);
        public int Player_Section_Index;
        public int Second_Place_Index;
        public int Track_Section_Max;
        public int Player_Lap_Adjust;
        public int Second_Place_Lap_Adjust;
        public int Distance_In_Sections;
        public string Relative_Time_Readout_Units;
        public int Player_Current_Section_Index;
        public int Player_Previous_Section_Index;

        public override void Start()
        {
            base.Start();

            Relative_Time_Readout = CustomComponents.GetById<Text>("RelativeTimeReadout");
            Track_Section_Max = NgTrackData.TrackManager.Instance.data.sections.Count; //How many sections the current track has
        }

        public override void Update()
        {
            base.Update();

            Player_Current_Section_Index = TargetShip.CurrentSection.index;

            if (TargetShip.CurrentSection.index >= 2)
            {
                Player_Section_Index = TargetShip.CurrentSection.index; //Section the player ship is currently on
            }
            else
            {
                Player_Section_Index = TargetShip.CurrentSection.index + Track_Section_Max; //Sections 0 and 1 on most tracks are entered before or as the next lap is being set
            }

            if (Ships.FindShipInPlace(2).CurrentSection.index >= 2)
            {
                Second_Place_Index = Ships.FindShipInPlace(2).CurrentSection.index; //Section the second place ship is currently on
            }
            else
            {
                Second_Place_Index = Ships.FindShipInPlace(2).CurrentSection.index + Track_Section_Max; //Sections 0 and 1 on most tracks are entered before or as the next lap is being set
            }

            //Player_Section_Index = TargetShip.CurrentSection.index; //Section the player ship is currently on
            //Second_Place_Index = Ships.FindShipInPlace(2).CurrentSection.index; //Section the second place ship is currently on

            Player_Lap_Adjust = (TargetShip.CurrentLap - 1); //Multiply Track_Section_Max by this value
            Second_Place_Lap_Adjust = (Ships.FindShipInPlace(2).CurrentLap - 1); //Multiply Track_Section_Max by this value

            Player_Section_Index += Player_Lap_Adjust * Track_Section_Max;
            Second_Place_Index += Second_Place_Lap_Adjust * Track_Section_Max;

            Distance_In_Sections = Player_Section_Index - Second_Place_Index;

            if (Player_Current_Section_Index > Player_Previous_Section_Index)
            {
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
                else if (TargetShip.lastRelativeTime > 0 && Distance_In_Sections < 120)
                {
                    Relative_Time_Readout.color = Race_Leader_Green;
                    Relative_Time_Readout.text = "-" + Relative_Time_Readout_Units;
                }
                else
                {
                    Relative_Time_Readout.text = "0:00.00";
                }
                //Relative_Time_Readout_Units = FloatToTime.Convert(Mathf.Abs(TargetShip.lastRelativeTime), "0:00.00");
                //Relative_Time_Readout.text = FloatToTime.Convert(Mathf.Abs(TargetShip.lastRelativeTime), "0:00.00");
                //Relative_Time_Readout.text = TargetShip.lastRelativeTime.ToString();
            }

            Player_Previous_Section_Index = TargetShip.CurrentSection.index;
        }
    }

    public class Music_Display : ScriptableHud
    {
        public Text Music_Display_Readout;

        IEnumerator Music_Flasher()
        {
            float New_Song_Start_Time = Time.time;
            float New_Song_End_Time = New_Song_Start_Time + 2.5f;

            float Start_Flash_On_1 = New_Song_Start_Time + 0.125f;
            float Start_Flash_Off_1 = New_Song_Start_Time + 0.25f;
            float Start_Flash_On_2 = New_Song_Start_Time + 0.375f;
            float Start_Flash_Off_2 = New_Song_Start_Time + 0.5f;
            float Start_Flash_On_3 = New_Song_Start_Time + 0.625f;
            float Start_Flash_Off_3 = New_Song_Start_Time + 0.75f;

            float End_Flash_On_0 = New_Song_Start_Time + 1.75f;
            float End_Flash_On_1 = New_Song_Start_Time + 1.875f;
            float End_Flash_Off_1 = New_Song_Start_Time + 2f;
            float End_Flash_On_2 = New_Song_Start_Time + 2.125f;
            float End_Flash_Off_2 = New_Song_Start_Time + 2.25f;
            float End_Flash_On_3 = New_Song_Start_Time + 2.375f;
            float End_Flash_Off_3 = New_Song_End_Time;

            while (Time.time < New_Song_End_Time)
            {
                //START
                if (New_Song_Start_Time < Time.time && Time.time <= Start_Flash_On_1)
                {
                    Music_Display_Readout.enabled = true;
                }
                if (Start_Flash_On_1 < Time.time && Time.time <= Start_Flash_Off_1)
                {
                    Music_Display_Readout.enabled = false;
                }
                if (Start_Flash_Off_1 < Time.time && Time.time <= Start_Flash_On_2)
                {
                    Music_Display_Readout.enabled = true;
                }
                if (Start_Flash_On_2 < Time.time && Time.time <= Start_Flash_Off_2)
                {
                    Music_Display_Readout.enabled = false;
                }
                if (Start_Flash_Off_2 < Time.time && Time.time <= Start_Flash_On_3)
                {
                    Music_Display_Readout.enabled = true;
                }
                if (Start_Flash_On_3 < Time.time && Time.time <= Start_Flash_Off_3)
                {
                    Music_Display_Readout.enabled = false;
                }

                if (Start_Flash_Off_3 < Time.time && Time.time <= End_Flash_On_0) //Stable for 1 second
                {
                    Music_Display_Readout.enabled = true;
                }

                //END
                if (End_Flash_On_0 < Time.time && Time.time <= End_Flash_On_1)
                {
                    Music_Display_Readout.enabled = false;
                }
                if (End_Flash_On_1 < Time.time && Time.time <= End_Flash_Off_1)
                {
                    Music_Display_Readout.enabled = true;
                }
                if (End_Flash_Off_1 < Time.time && Time.time <= End_Flash_On_2)
                {
                    Music_Display_Readout.enabled = false;
                }
                if (End_Flash_On_2 < Time.time && Time.time <= End_Flash_Off_2)
                {
                    Music_Display_Readout.enabled = true;
                }
                if (End_Flash_Off_2 < Time.time && Time.time <= End_Flash_On_3)
                {
                    Music_Display_Readout.enabled = false;
                }
                if (End_Flash_On_3 < Time.time && Time.time <= End_Flash_Off_3)
                {
                    Music_Display_Readout.enabled = true;
                }
                yield return null;
            }
            Music_Display_Readout.enabled = false;
        }

        public override void Start()
        {
            base.Start();
            NgUiEvents.OnNewSongPlaying += Music_Display_Method;
            Music_Display_Readout = CustomComponents.GetById<Text>("MusicReadout");
            Music_Display_Readout.text = "";
        }

        public void Music_Display_Method(string name)
        {
            Music_Display_Readout.text = name;

            StartCoroutine(Music_Flasher());
        }

        public override void OnDestroy()
        {
            NgUiEvents.OnNewSongPlaying -= Music_Display_Method;
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

        IEnumerator Damage_Flash_Autopilot_Duration()
        {
            float Duration_Time = 0.5f;
            float Start_Time = Time.time;
            float End_Time = Start_Time + Duration_Time;
            while (Time.time < End_Time)
            {
                Damage_Flasher_Image.enabled = true;
                yield return null;
            }
            Damage_Flasher_Image.enabled = false;
            //yield return new WaitForEndOfFrame();
        }

        IEnumerator Damage_Flash_Duration()
        {
            while ((TargetShip.PysSim.enginePower < 1f) || (TargetShip.PysSim.engineAccel < 1f))
            {
                Damage_Flasher_Image.enabled = true;
                yield return null;
            }
            Damage_Flasher_Image.enabled = false;
        }

        public override void Start()
        {
            base.Start();

            Damage_Flasher_Image = CustomComponents.GetById<Image>("DamageFlasher");
            Damage_Flasher_Background = CustomComponents.GetById<Image>("DamageFlasherBackground");
            Damage_Flasher_Image.enabled = false;
        }

        public override void Update()
        {
            base.Update();

            Current_Shield_Integrity = TargetShip.ShieldIntegrity;

            if ((Current_Shield_Integrity < Previous_Shield_Integrity) && TargetShip.IsShieldActivated && TargetShip.LastAttacker != null)
            {
                //Damage_Flasher_Image.enabled = true;
                StartCoroutine(Damage_Flash_Autopilot_Duration());
            }
            else if (((TargetShip.PysSim.enginePower < 1f) || (TargetShip.PysSim.engineAccel < 1f)) && TargetShip.CurrentLap != 0)
            {
                //Damage_Flasher_Image.enabled = true;
                StartCoroutine(Damage_Flash_Duration());
            }
            //else
            //{
                //Damage_Flasher_Image.enabled = false;
            //}
            Previous_Shield_Integrity = TargetShip.ShieldIntegrity;
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
                    Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                    Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                    Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                    Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                    Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                    Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                    Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                    break;
                case 3:
                    Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                    break;
                case 4:
                    Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                    Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                    break;
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarTextToggle == false)
            {
                Hyperthrust_Bar_Numeric_Readout.enabled = false;
            }

        }

        public override void Update()
        {
            base.Update();

            Hyperthrust_Bar_Image.fillAmount = TargetShip.PysSim.engineHyper / 1.85f;
            Hyperthrust_Units_String = string.Format("{0:N2}", TargetShip.PysSim.engineHyper);
            Hyperthrust_Bar_Numeric_Readout.text = Hyperthrust_Units_String;
            if (TargetShip.PysSim.engineHyper < 0.01f)
            {
                Hyperthrust_Bar_Numeric_Readout.text = "";
            }
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
        public float Speedpad_Counter_2280;

        public static readonly Vector2 Speedpad_Counter_Lowered_Adjust_Vector = new Vector2(287, -502);
        public static readonly Vector2 Speedpad_Counter_Centered_Adjust_Vector = new Vector2(287, -371);
        public static readonly Vector2 Speedpad_Counter_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
        public static readonly Vector2 Speedpad_Counter_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

        public Vector2 Speedpad_Count_Numeric_Readout_Default_Position;
        public Vector2 Speedpad_Counter_Background_Default_Position;
        public Vector2 Speedpad_Counter_Whiteout_Default_Position;
        public Vector2 Speedpad_Counter_Image_Default_Position;

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

            switch (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition)
            {
                case 1:
                    Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                    Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                    Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                    Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                    Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                    Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                    Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                    break;
                case 3:
                    Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                    Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                    Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                    Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                    break;
                case 4:
                    Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                    Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                    Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                    Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                    break;
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadCounterTextToggle == false)
            {
                Speedpad_Count_Numeric_Readout.enabled = false;
            }

        }

        public override void Update()
        {
            base.Update();

            if (Cheats.IntFromPhysicsMod() == 1) //Formerly if (Cheats.ModernPhysics)
            {
                Speedpad_Counter_Current_Speedpad_Time_2280 = TargetShip.PysSim.modernPadPushTimer;

                if ((Speedpad_Counter_Current_Speedpad_Time_2280 > Speedpad_Counter_Previous_Speedpad_Time_2280)) // && (Speedpad_Counter_2280 < 3))
                {
                    Speedpad_Counter_2280 += 1f;
                }
                if (Speedpad_Counter_Current_Speedpad_Time_2280 <= 0f)
                {
                    Speedpad_Counter_2280 = 0f;
                }

                Speedpad_Counter_Image.fillAmount = Speedpad_Counter_2280 / 3f;
                Speedpad_Count_Numeric_Readout.text = "+" + Speedpad_Counter_2280.ToString();

                if (Speedpad_Counter_2280 == 0f)
                {
                    Speedpad_Count_Numeric_Readout.text = "";
                }

                Speedpad_Counter_Previous_Speedpad_Time_2280 = TargetShip.PysSim.modernPadPushTimer;
            }
            else
            {
                Speedpad_Counter_Image.fillAmount = TargetShip.BoostAcceleration / 12f;
                Speedpad_Count_Numeric_Readout.text = "+" + TargetShip.BoostAcceleration.ToString();

                if (TargetShip.BoostAcceleration == 0f)
                {
                    Speedpad_Count_Numeric_Readout.text = "";
                }

            }

        }

    }

    public class Speedpad_Timer : ScriptableHud
    {
        public Text Speedpad_Timer_Numeric_Readout;
        public Image Speedpad_Timer_Background;
        public Image Speedpad_Timer_Whiteout;
        public Image Speedpad_Timer_Image;
        public string Speedpad_Timer_Units_String;

        public Vector2 Speedpad_Timer_Numeric_Readout_Default_Position;
        public Vector2 Speedpad_Timer_Background_Default_Position;
        public Vector2 Speedpad_Timer_Whiteout_Default_Position;
        public Vector2 Speedpad_Timer_Image_Default_Position;

        public static readonly Vector2 Speedpad_Timer_Lowered_Adjust_Vector = new Vector2(287, -502);
        public static readonly Vector2 Speedpad_Timer_Centered_Adjust_Vector = new Vector2(287, -371);
        public static readonly Vector2 Speedpad_Timer_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
        public static readonly Vector2 Speedpad_Timer_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

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

            switch (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition)
            {
                case 1:
                    Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                    Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                    Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                    Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                    break;
                case 2:
                    Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                    Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                    Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                    Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                    break;
                case 3:
                    Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                    Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                    Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                    Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                    break;
                case 4:
                    Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                    Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                    Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                    Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                    break;
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadTimerTextToggle == false)
            {
                Speedpad_Timer_Numeric_Readout.enabled = false;
            }

        }

        public override void Update()
        {
            base.Update();
            if (Cheats.IntFromPhysicsMod() == 1) //Formerly if (Cheats.ModernPhysics)
            {
                Speedpad_Timer_Image.fillAmount = TargetShip.PysSim.modernPadPushTimer / 0.24f;
                Speedpad_Timer_Units_String = string.Format("{0:N2}", TargetShip.PysSim.modernPadPushTimer);

                Speedpad_Timer_Numeric_Readout.text = Speedpad_Timer_Units_String;
                if (TargetShip.PysSim.modernPadPushTimer < 0.01f)
                {
                    Speedpad_Timer_Units_String = "";
                }
                Speedpad_Timer_Numeric_Readout.text = Speedpad_Timer_Units_String;
            }
            else
            {
                Speedpad_Timer_Image.fillAmount = TargetShip.BoostTimer / 4.5f; //First 3 speedpads hit grant 1.5 seconds each, 4th and beyond grant 0.5 seconds each. Formerly this value was 5f
                Speedpad_Timer_Units_String = string.Format("{0:N2}", TargetShip.BoostTimer);

                if (TargetShip.BoostTimer < 0.01f)
                {
                    Speedpad_Timer_Units_String = "";
                }
                Speedpad_Timer_Numeric_Readout.text = Speedpad_Timer_Units_String;
            }            
        }
    }

    public class Lap_Counter : ScriptableHud
    {
        public Text Lap_Counter_Field_Name;
        public Text Current_Lap;
        public Text Max_Lap;
        public Image Lap_Counter_Background;
        public Image Lap_Counter_Image;
        public string Current_Lap_String;
        public string Max_Lap_String;

        public override void Start()
        {
            base.Start();

            Lap_Counter_Field_Name = CustomComponents.GetById<Text>("Lap Counter Field Name");
            Current_Lap = CustomComponents.GetById<Text>("Current Lap");
            Max_Lap = CustomComponents.GetById<Text>("Max Lap");
            Lap_Counter_Image = CustomComponents.GetById<Image>("LapCounterField");
            Lap_Counter_Background = CustomComponents.GetById<Image>("LapCounterFieldBackground");
        }

        public override void Update()
        {
            base.Update();

            Current_Lap_String = TargetShip.CurrentLap.ToString();
            Max_Lap_String = Race.MaxLaps.ToString();

            if (TargetShip.CurrentLap < 10)
            {
                Current_Lap_String = "0" + Current_Lap_String;
            }
            if (Race.MaxLaps < 10)
            {
                Max_Lap_String = "0" + Max_Lap_String;
            }

            Current_Lap.text = Current_Lap_String;
            Max_Lap.text = Max_Lap_String;
        }

    }

    public class Position_Counter : ScriptableHud
    {
        public Text Position_Counter_Field_Name;
        public Text Current_Position;
        public Text Max_Position;
        public Image Position_Counter_Field_Image;
        public Image Position_Counter_Field_Background;
        public string Current_Position_String;
        public string Max_Position_String;
        
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

            Current_Position_String = TargetShip.CurrentPlace.ToString();
            Max_Position_String = Ships.Active.Count.ToString();
            if (TargetShip.CurrentPlace < 10)
            {
                Current_Position_String = "0" + Current_Position_String;
            }
            if (Ships.Active.Count < 10)
            {
                Max_Position_String = "0" + Max_Position_String;
            }
            Current_Position.text = Current_Position_String;
            Max_Position.text = Max_Position_String;
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
                Best_Time_Units = FloatToTime.Convert(Time_Milliseconds, "0:00.00");
            }
            else if (TargetShip.TargetTime == 0f && TargetShip.BestLapTime != 0f)
            {
                Time_Milliseconds = TargetShip.BestLapTime;
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

            if (TargetShip.CurrentLap > 5 && (TargetShip.CurrentLap % 5) == 1)
            {
                Lap_Images[1].color = Color.white;
                Lap_Images[2].color = Color.white;
                Lap_Images[3].color = Color.white;
                Lap_Images[4].color = Color.white;
                Lap_Texts[1].text = "–:––.––";
                Lap_Texts[2].text = "–:––.––";
                Lap_Texts[3].text = "–:––.––";
                Lap_Texts[4].text = "–:––.––";
            }

            if (((TargetShip.CurrentLap % 5 == 1) || (TargetShip.CurrentLap == 0)) && ((Race.MaxLaps - TargetShip.CurrentLap) <= 4))
            {
                for (int i = ((TargetShip.CurrentLap % 5) + (Race.MaxLaps - TargetShip.CurrentLap)); i <= 4; i++)
                {
                    Lap_Images[i].enabled = false;
                    Lap_Texts[i].enabled = false;
                }
            }

            if (TargetShip.CurrentLap % 5 == 0 && TargetShip.CurrentLap != 0)
            {
                Lap_Texts[4].text = FloatToTime.Convert(TargetShip.CurrentLapTime, "0:00.00").ToString();
            }
            else
            {
                Lap_Texts[((TargetShip.CurrentLap % 5) - 1)].text = FloatToTime.Convert(TargetShip.CurrentLapTime, "0:00.00").ToString();
            }

            Total_Time.text = FloatToTime.Convert(TargetShip.TotalRaceTime, "0:00.00").ToString();

            if (TargetShip.IsPerfectLap)
            {
                if (TargetShip.CurrentLap % 5 == 0)
                {
                    Lap_Images[4].color = Lap_Diamond_Green;
                }
                else
                {
                    Lap_Images[((TargetShip.CurrentLap % 5) - 1)].color = Lap_Diamond_Green;
                }
                
            }
            else
            {
                if (TargetShip.CurrentLap % 5 == 0 && TargetShip.CurrentLap != 0)
                {
                    Lap_Images[4].color = Lap_Diamond_Red;
                }
                else
                {
                    Lap_Images[((TargetShip.CurrentLap % 5) - 1)].color = Lap_Diamond_Red;
                }
            }
        }
    }

    public class Thrust_Bar : ScriptableHud //Speed Bar
    {
        public Image Thrust_Bar_Image;
        public Image Thrust_Bar_Background_Image;
        public Text Engine_Force_Text;
        public float Engine_Force_Units;
        public string Engine_Force_Units_String;


        public override void Start()
        {
            base.Start();

            Thrust_Bar_Image = CustomComponents.GetById<Image>("ThrustBar");
            Thrust_Bar_Background_Image = CustomComponents.GetById<Image>("ThrustBarBackground");
            Engine_Force_Text = CustomComponents.GetById<Text>("Engine Force Numeric Readout");
        }

        public override void Update()
        {
            base.Update();
            Thrust_Bar_Image.fillAmount = TargetShip.HudSpeed * (1f / 1400f);
            //Engine_Force_Text.text = TargetShip.HudSpeed.ToString();
            Engine_Force_Units = TargetShip.PysSim.engineThrust;
            switch (VanillaPlusHUDOptions.ModMenuOptions.SpeedometerReadoutStyle)
            {
                case 0:
                    Engine_Force_Units_String = string.Format("{0:N1}", Engine_Force_Units);
                    break;
                case 1:
                    Engine_Force_Units_String = string.Format("{0:N0}", TargetShip.HudSpeed);
                    break;
                case 2:
                    Engine_Force_Units_String = string.Format("{0:N1}", TargetShip.RBody.velocity.magnitude);
                    break;
            }

            //Engine_Force_Text.text = TargetShip.NetworkedVelocity.magnitude.ToString();
            Engine_Force_Text.text = Engine_Force_Units_String;
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

        public override void Update()
        {
            base.Update();

            Weapon_Text.text = TargetShip.PickupDisplayText;

            if (RaceManager.CurrentGamemode.Configuration.ShipsCanAbsorbPickups == true && TargetShip.CurrentPickup.GetCurrentAbsorbAmount() != 0)
                Absorb_Text.text = "+" + TargetShip.CurrentPickup.GetCurrentAbsorbAmount().ToString();
            else
                Absorb_Text.text = "";
        }

        public override void Start()
        {
            base.Start();

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

        }

        public void SetPickup(PickupBase pickup, ShipController ship)

        {
            if (ship.IsPlayer)
            {
                Weapon_Icon.enabled = true;
                Weapon_Background.enabled = true;
                weapon_id = TargetShip.CurrentPickupRegister.Name;
                Weapon_Icon.sprite = CustomHudRegistry.GetWeaponSprite(weapon_id, true);
            }
        }

        public void DropPickup(PickupBase pickup, ShipController ship)
        {
            if (ship.IsPlayer)
            {
                Weapon_Icon.enabled = false;
                Weapon_Background.enabled = false;
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
        public Text Shield_Integrity_Numeric_Readout;
        public Image Energy_Bar_Image;
        public Image Energy_Bar_Background_Image;

        public bool Energy_Low_Coroutine_Running;
        public bool Energy_Critical_Coroutine_Running;

        //public Vector4 Color_Breakpoint_0 = new Vector4(255, 28, 36, 255); //Red
        //public Vector4 Color_Breakpoint_1 = new Vector4(255, 127, 39, 255); //Orange
        //public Vector4 Color_Breakpoint_2 = new Vector4(255, 242, 0, 255); //Yellow
        //public Vector4 Color_Breakpoint_3 = new Vector4(87, 255, 23, 255); //Green
        //public Vector4 Color_Breakpoint_4 = new Vector4(0, 162, 232, 255); //Blue
        //public Vector4 Color_Breakpoint_5 = new Vector4(255, 255, 255, 255); //White
        public static readonly Vector4 Color_Breakpoint_0 = new Vector4(1f, (29f / 256f), (37f / 256f), 1f); //Red
        public static readonly Vector4 Color_Breakpoint_1 = new Vector4(1f, (128f / 256f), (40f / 256f), 1f); //Orange
        public static readonly Vector4 Color_Breakpoint_2 = new Vector4(1f, (243f / 256f), 0f, 1f); //Yellow
        public static readonly Vector4 Color_Breakpoint_3 = new Vector4((88f / 256f), 1f, (24f / 256f), 1f); //Green
        public static readonly Vector4 Color_Breakpoint_4 = new Vector4(0f, (163f / 256f), (233f / 256f), 1f); //Blue
        public static readonly Vector4 Color_Breakpoint_5 = new Vector4(1f, 1f, 1f, 1f); //White
        public Vector4 Energy_Bar_Background_Original_Color;

        public static readonly Vector4 Clear_Red = new Vector4(1f, 0f, 0f, 0f); //Don't have sprites you want to change the color of via script pre-colored and pre-alpha'd outside of Unity because modifying them in script modifies the color channels relative to the colors in the file provided as a sprite rather than absolute colors
        //public static readonly Vector4 Dark_Red = new Vector4(1f, 0f, 0f, 0.8f);

        IEnumerator EnergyLowColorPulse() //Might have to set the color to the lerp target after Time.time == LerpInTime and after Time.time == EndTime
        {
            Energy_Low_Coroutine_Running = true;
            float Energy_Low_Color_Pulse_Start_Time = Time.time;
            float Energy_Low_Color_Pulse_End_Time = Energy_Low_Color_Pulse_Start_Time + 0.4f;

            float Energy_Low_Color_Pulse_Lerp_In_Time = Energy_Low_Color_Pulse_Start_Time + 0.2f;

            while (Time.time < Energy_Low_Color_Pulse_End_Time)
            {
                if (Energy_Low_Color_Pulse_Start_Time < Time.time && Time.time <= Energy_Low_Color_Pulse_Lerp_In_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Clear_Red, Color.red, (Time.time - Energy_Low_Color_Pulse_Start_Time) / (Energy_Low_Color_Pulse_Lerp_In_Time - Energy_Low_Color_Pulse_Start_Time));
                }
                if (Energy_Low_Color_Pulse_Lerp_In_Time < Time.time && Time.time <= Energy_Low_Color_Pulse_End_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Color.red, Clear_Red, (Time.time - Energy_Low_Color_Pulse_Start_Time) / (Energy_Low_Color_Pulse_End_Time - Energy_Low_Color_Pulse_Start_Time));
                }
                yield return null;
            }
            Energy_Low_Coroutine_Running = false;
        }

        IEnumerator EnergyCriticalColorPulse() //Might have to set the color to the lerp target after Time.time == LerpInTime and after Time.time == EndTime
        {
            Energy_Critical_Coroutine_Running = true;
            float Energy_Critical_Color_Pulse_Start_Time = Time.time;
            float Energy_Critical_Color_Pulse_End_Time = Energy_Critical_Color_Pulse_Start_Time + 0.2f;

            float Energy_Critical_Color_Pulse_Lerp_In_Time = Energy_Critical_Color_Pulse_Start_Time + 0.1f;

            while (Time.time < Energy_Critical_Color_Pulse_End_Time)
            {
                if (Energy_Critical_Color_Pulse_Start_Time < Time.time && Time.time <= Energy_Critical_Color_Pulse_Lerp_In_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Clear_Red, Color.red, (Time.time - Energy_Critical_Color_Pulse_Start_Time) / (Energy_Critical_Color_Pulse_Lerp_In_Time - Energy_Critical_Color_Pulse_Start_Time));
                }
                if (Energy_Critical_Color_Pulse_Lerp_In_Time < Time.time && Time.time <= Energy_Critical_Color_Pulse_End_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Color.red, Clear_Red, (Time.time - Energy_Critical_Color_Pulse_Start_Time) / (Energy_Critical_Color_Pulse_End_Time - Energy_Critical_Color_Pulse_Start_Time));
                }
                yield return null;
            }
            Energy_Critical_Coroutine_Running = false;
        }

        public override void Start()
        {
            base.Start();

            Energy_Bar_Image = CustomComponents.GetById<Image>("EnergyBar");
            Energy_Bar_Background_Image = CustomComponents.GetById<Image>("EnergyBarBackground");
            Shield_Integrity_Numeric_Readout = CustomComponents.GetById<Text>("Shield Integrity Numeric Readout");

            Energy_Bar_Background_Original_Color = Energy_Bar_Background_Image.color;
        }

        public override void Update()
        {
            base.Update();

            Energy_Bar_Image.fillAmount = TargetShip.ShieldIntegrity * 0.01f;
            switch (VanillaPlusHUDOptions.ModMenuOptions.EnergyBarReadoutDecimalPrecision)
            {
                case 0:
                    Shield_Integrity_Numeric_Readout.text = TargetShip.ShieldIntegrity.ToString();
                    break;
                case 1:
                    Shield_Integrity_Numeric_Readout.text = string.Format("{0:N4}", TargetShip.ShieldIntegrity);
                    break;
                case 2:
                    Shield_Integrity_Numeric_Readout.text = string.Format("{0:N3}", TargetShip.ShieldIntegrity);
                    break;
                case 3:
                    Shield_Integrity_Numeric_Readout.text = string.Format("{0:N2}", TargetShip.ShieldIntegrity);
                    break;
                case 4:
                    Shield_Integrity_Numeric_Readout.text = string.Format("{0:N1}", TargetShip.ShieldIntegrity);
                    break;
                case 5:
                    Shield_Integrity_Numeric_Readout.text = string.Format("{0:N0}", TargetShip.ShieldIntegrity);
                    break;
            }

            if (TargetShip.ShieldIntegrity > 71.15920f)
                Energy_Bar_Image.color = Color_Breakpoint_5; //White
            if (TargetShip.ShieldIntegrity > 60.27840f && TargetShip.ShieldIntegrity <= 71.15920f)
                Energy_Bar_Image.color = Color_Breakpoint_4; //Blue
            if (TargetShip.ShieldIntegrity > 36.59616f && TargetShip.ShieldIntegrity <= 60.27840f)
                Energy_Bar_Image.color = Color_Breakpoint_3; //Green
            if (TargetShip.ShieldIntegrity > 15.42840f && TargetShip.ShieldIntegrity <= 36.59616f)
                Energy_Bar_Image.color = Color_Breakpoint_2; //Yellow
            if (TargetShip.ShieldIntegrity > 8.13248f && TargetShip.ShieldIntegrity <= 15.42840f)
                Energy_Bar_Image.color = Color_Breakpoint_1; //Orange
            if (TargetShip.ShieldIntegrity >= 0.00001f && TargetShip.ShieldIntegrity <= 8.13248f)
                Energy_Bar_Image.color = Color_Breakpoint_0; //Red

            if (10f < TargetShip.ShieldIntegrity && TargetShip.ShieldIntegrity <= 25f && !Energy_Low_Coroutine_Running)
            {
                StartCoroutine(EnergyLowColorPulse());
            }
            else if (TargetShip.ShieldIntegrity <= 10f && !Energy_Critical_Coroutine_Running)
            {
                StartCoroutine(EnergyCriticalColorPulse());
            }
            else if (TargetShip.ShieldIntegrity > 25f)
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

        IEnumerator SpeedPadColorPulse_Coroutine()
        {            
            float Speed_Pad_Color_Pulse_Start_Time = Time.time;
            float Speed_Pad_Color_Pulse_End_Time = Speed_Pad_Color_Pulse_Start_Time + 0.5f;

            //float Speed_Pad_Color_Pulse_Lerp_In_Time = Speed_Pad_Color_Pulse_Start_Time + 0.25f;

            while (Time.time < Speed_Pad_Color_Pulse_End_Time)
            {
                if (Speed_Pad_Color_Pulse_Start_Time < Time.time && Time.time <= Speed_Pad_Color_Pulse_End_Time)
                {
                    Throttle_Bar_Image.color = Color.Lerp(Dark_Blue, Light_Blue, (Time.time - Speed_Pad_Color_Pulse_Start_Time) / (Speed_Pad_Color_Pulse_End_Time - Speed_Pad_Color_Pulse_Start_Time));
                }
                yield return Cached_WaitForEndOfFrame;
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
        }

        public override void Update()
        {
            base.Update();

            Throttle_Bar_Image.fillAmount = Mathf.Min(TargetShip.PysSim.enginePower, TargetShip.PysSim.engineAccel);
        }

        public void SpeedPadColorPulse(ShipController ship, BallisticUnityTools.TrackTools.TrackPad pad) //Works with 3D ("Modern") pads
        {
            if (ship.IsPlayer)
            {
                StartCoroutine(SpeedPadColorPulse_Coroutine());
            }
        }

        public void SpeedPadColorPulse_Tile(ShipController ship, NgTrackData.Tile tile) //Works with "Classic" speedpads
        {
            if (ship.IsPlayer)
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

        

        public float lastWidth, lastHeight;



        public static readonly Vector2 Rear_View_Mirror_Adjust_Vector = new Vector2(0, 223);

        public override void Start()
        {
            base.Start();

            lastWidth = 640;
            lastHeight = 480;

            RearViewMirror = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            RearViewMirror.color = Color.clear;


            
            if ((VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2159 && Cheats.IntFromPhysicsMod() == 0) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirror2280 && Cheats.IntFromPhysicsMod() == 1) || (VanillaPlusHUDOptions.ModMenuOptions.RearViewMirrorFloorhugger && Cheats.IntFromPhysicsMod() == 2))
            {
                StartCoroutine(WaitAFrame());
                //Other logic can go in between these two
                StartCoroutine(RenderFrame());
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.WeaponMirrorPositionSwap)
            {
                RearViewMirror.rectTransform.anchoredPosition = RearViewMirror.rectTransform.anchoredPosition + Rear_View_Mirror_Adjust_Vector;
            }

        }

        IEnumerator WaitAFrame()
        {
            for (int i = 0; i < 60; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            Empty_Game_Object = CustomComponents.GetById("EmptyGameObject"); //initialize the RectTransform through the Empty Game Object component in the Unity prefab
            Rear_View_Mirror_Camera = Empty_Game_Object.gameObject.AddComponent<Camera>(); //Adding a camera dynamically by attaching it to the Empty Game Object
            //Rear_View_Mirror_Camera.enabled = false; //Disabled so it's not rendering until instructed to do so
            CreateRenderTexture(640f, 160f); //Size of the RenderTexture on user screen

            

        }

        IEnumerator RenderFrame()
        {
            while (!TargetShip.FinishedEvent)
            {
                Rear_View_Mirror_Camera.Render();
                //yield return new WaitForSeconds(1f / 60); //60 here represents the refresh rate of the rear view mirror, WaitForEndOfFrame() is for unlimited FPS and looks best
                yield return new WaitForEndOfFrame();
            }
        }

        public void CreateRenderTexture(float rWidth, float rHeight)
        {
            Rear_View_Mirror_Feed = new RenderTexture((int)(lastWidth / (1920f / rWidth)), (int)(lastHeight / (1080f / rHeight)), 32); //32 is the color format
            Rear_View_Mirror_Camera.targetTexture = Rear_View_Mirror_Feed;
        }

        public override void Update()
        {
            base.Update();
            //Rear_View_Mirror_Camera.enabled = true;

            



            

            if (!TargetShip.CamSim.LookingBehind)
            {
                RearViewMirror.transform.localScale = Point_Camera_Backwards_Transform_Scale;
                Rear_View_Mirror_Camera.cullingMask = TargetShip.ShipCamera.cullingMask;
                Rear_View_Mirror_Camera.transform.SetParent(TargetShip.transform);
                Rear_View_Mirror_Camera.transform.localPosition = -Vector3.forward * TargetShip.ShipToShipCollider.size.z / 2;
                Rear_View_Mirror_Camera.transform.localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / 2;
                Rear_View_Mirror_Camera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                //if (!Cheats.ModernPhysics) //if (!Cheats.ModernPhysics || Cheats.ModernPhysics)
                //{
                    //RearViewMirror.color = Color.white; //Set alpha back to full so the rear view mirror is visible and opaque
                //}
                RearViewMirror.color = Color.white; //Set alpha back to full so the rear view mirror is visible and opaque
                
            }
            else
            {
                RearViewMirror.transform.localScale = Point_Camera_Forwards_Transform_Scale;
                Rear_View_Mirror_Camera.cullingMask = TargetShip.ShipCamera.cullingMask;
                Rear_View_Mirror_Camera.transform.SetParent(TargetShip.transform);
                Rear_View_Mirror_Camera.transform.localPosition = Vector3.forward * TargetShip.ShipToShipCollider.size.z / 2;
                Rear_View_Mirror_Camera.transform.localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / 2;
                Rear_View_Mirror_Camera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                //if (!Cheats.ModernPhysics)
                //{
                    //RearViewMirror.color = Color.white; //Set alpha back to full so the rear view mirror is visible and opaque
                //}
                RearViewMirror.color = Color.white; //Set alpha back to full so the rear view mirror is visible and opaque
            }

            if (Rear_View_Mirror_Camera != null && Rear_View_Mirror_Feed != null)
            {
                float width = (float)Screen.width;
                float height = (float)Screen.height;

                if (lastWidth != width || lastHeight != height)
                {
                    lastWidth = width;
                    lastHeight = height;

                    RearViewMirror.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 640f * ((width / height) / (16f / 9f)));
                    CreateRenderTexture(640f, 160f);
                }

                RearViewMirror.gameObject.SetActive(true);
                RearViewMirror.texture = Rear_View_Mirror_Feed;
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

        public Quaternion Pseudohugger_Rotation_Quaternion;
        //public Quaternion Intermediate_Rotation_Quaternion;
        //public Quaternion Rigidbody_Rotation_Quaternion;

        public Quaternion Previous_Pseudohugger_Rotation_Quaternion;
        //public Quaternion Previous_Intermediate_Rotation_Quaternion;
        //public Quaternion Previous_Rigidbody_Rotation_Quaternion;        

        public Quaternion Stored_Pseudohugger_Rotation_Quaternion;
        //public Quaternion Stored_Intermediate_Rotation_Quaternion;
        //public Quaternion Stored_Rigidbody_Rotation_Quaternion;

        public Quaternion Target_Pseudohugger_Rotation_Quaternion;



        public Quaternion Tilt_Lock_Rotation_Quaternion; //used for Tiltlock2280 Camera Mode
        public Quaternion Target_Tilt_Lock_Rotation_Quaternion; //used for Tiltlock2280 Camera Mode


        public float Rotation_Time;
        public float Tiltlock_Maglocked_Interpolation_Ratio;
        public float Tiltlock_Non_Maglocked_Interpolation_Ratio;
        //public float Pseudohugger_Aerial_Interpolation_Ratio;
        public float Pseudohugger_Grounded_Interpolation_Ratio;

        public static readonly int[] BNGL_Arrivon_Falls_8 = { 398, 399, 461, 462 };
        public static readonly int[] BNGL_Oceana_8 = { 121, 122, 176, 177 };
        public static readonly int[] Helheim = { 43, 44, 113, 114, 201, 202, 242, 243 };
        public static readonly int[] Alphard_Reverse = { 357, 358 };
        public static readonly int[] Samrong_Crossing_Reverse = { 67, 68, 94, 95, 123, 124 };
        public static readonly int[] Blackward_Decks = { 14, 15, 103, 105, 279, 280, 459, 460 };
        public static readonly int[] Great_Manitou_Trail = { 354, 355, 380, 381, 402, 403, 648, 650 };
        public static readonly int[] Vestfjorden = { 35, 36 };
        public static readonly int[] Abyssus = { 149, 150, 209, 210, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 259, 260, 261, 262 };
        public static readonly int[] Aeolus_II = { 0, 1, 116, 117, 170, 171, 236, 237, 245, 246, 269, 270, 296, 297 };
        public static readonly int[] Altima_XIV = { 203, 204 };
        public static readonly int[] Antelao = { 168, 187, 308 };
        public static readonly int[] Antelao_Reverse = { 130, 131, 149, 150 };
        public static readonly int[] Beltane = { 318, 319};
        public static readonly int[] Cairodrome = { 57, 58 };
        public static readonly int[] Canyonlands = { 48, 49, 50, 51, 281, 282, 295, 296, 310, 311, 329, 330, 493, 494, 495, 496, 497 };
        public static readonly int[] Canyonlands_Reverse = { 86, 87, 88, 105, 106, 107, 120, 121, 122, 134, 135, 136, 401, 402, 403 };
        public static readonly int[] Dione_IV = { -417 };
        public static readonly int[] Elivagar_Reverse = { 327, 328 };
        public static readonly int[] Eschaton = { 3, 4, 14, 15, 37, 38, 159, 160, 165, 166, 177, 178, 308, 309 };
        public static readonly int[] Gehennom = { 1, 2, 107, 108, 220, 221, 243, 244, 255, 256, 276, 277, 291, 292, 301, 302, 321, 322, 462, 523, 529, 530, 568, 569, 613, 614, 634, 635, 654, 655, 672, 673 };
        public static readonly int[] Herne_Park = { 243, 244 };
        public static readonly int[] Kamanneq = { 2, 3, 102, 103, 147, 148 };
        public static readonly int[] Lumenar_V = { 7, 8, 31, 32};
        public static readonly int[] Millenium_Wharf = { 78, 79, 251, 252, 355, 356, 428, 429 };
        public static readonly int[] Project_Pandora = { 180, 181, 500, 501, 510, 511, 521, 522 };
        public static readonly int[] Shanghai_Financial = { 13, 14, 179, 180 };
        public static readonly int[] South_Ridge = { 213, 214 };
        public static readonly int[] Sovereign = { 271, 272, 273 };
        public static readonly int[] Sovereign_Reverse = { 65, 66, 67 };
        public static readonly int[] The_STJ = { 201, 202 };
        public static readonly int[] Dagon_Prime = { 16, 17, 119, 120, 175, 176, 177, 178, 179, 302, 303, 351, 352, 368, 369, 471, 472, 511, 512, 535, 536, 557, 558 };
        public static readonly int[] Prototype_19_WLD_Botanica = { 182, 183 };
        public static readonly int[] Prototype_77_ODR_Nachtmahr = { 6, 7 };
        public static readonly int[] Prototype_81_JMT_Lambda2Phi = { 2, 3, 26, 27, 155, 156, 188, 189 };
        public static readonly int[] Prototype_99_KEM_Sunthrone = { 20, 21, 44, 45, 46, 47, 48, 49, 50, 51, 61, 62, 66, 67, 71, 72, 76, 77, 78, 79, 80, 97, 98, 125, 126, 160, 161, 162, 163, 187, 188, 189, 205, 206, 207, 208, 209, 210, 234, 235, 271, 272, 273, 274, 275, 276, 277, 287, 288 };
        public static readonly int[] ES_0x016_Germania = { 191, 192};
        public static readonly int[] ES_0x033_Dione_II = { 99, 100, 135, 136, 427, 428, 460, 461 };
        public static readonly int[] Annapurna = { 139, 201, 202, 203 };
        public static readonly int[] Desolata = { 679, 680 };
        public static readonly int[] Infinity_Spear = { 119, 120, 161, 162, 163, 164, 221, 222, 231, 232, 241, 242, 250, 251, 263, 264 };
        public static readonly int[] bngl_metro_11 = { 157, 158, 391, 392, 393, 394, 395, 396, 397, 431, 432 };
        public static readonly int[] bngl_nova_split_11 = { 13, 14, 33, 34, 121, 122, 151, 152, 173, 174, 211, 212, 245, 246 };
        public static readonly int[] bngl_wipeoutzone = { 1125, 1126 };
        public static readonly int[] DandelionCircuit = { -394, -395 };
        public static readonly int[] Astra_Magnesium = { 176, 177, 201, 202 };
        public static readonly int[] Serenewoods = { 208, 209, 210, 211, 212, 213, 214, 231, 232, 233, 234, 235 };
        public static readonly int[] Cobbledark = { -543 };
        public static readonly int[] SW1R_Bumpys_Breakers = { 306, 307, 308, 309, 310, 482, 483 };
        public static readonly int[] SW1R_Malastare_100 = { 87, 88, 89, 90, 91, 92, 93, 94, 95, 101, 102, 103, 107, 108, 109, 110, };
        public static readonly int[] SW1R_Scrappers_Run = { 83, 84, 148, 149 };
        public static readonly int[] SW1R_The_Boonta_Eve_Classic = { 308, 309, 310, 579, 580 };
        public static readonly int[] SW1R_Aquilaris_Classic = { 196, 197, 198, 199, 200, 201, 236, 238, 239, 240, 241, 242, 243, 244, 249, 251, 252 };
        public static readonly int[] NFS1_Alpine = { 430, 431, 432, 433 };
        public static readonly int[] NFS1_Transtropolis = { 357, 358, 359, 360 };
        public static readonly int[] Solaris = { 14, 15, 78, 79, 151, 152, 290, 291, 362, 363 };
        public static readonly int[] Solaris_Reverse = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 195, 196, 197, 198, 199, 200, 201, 202, 203, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422 };
        public static readonly int[] Aerodive = { 93, 94, 191, 192 };
        public static readonly int[] Escalation_Project = { 39, 40, 41, 42, 43, 44, 176, 177, 178, 179, 255, 256, 270, 288, 302, 403, 404, 423, 424, 425, 426, 480, 481 }; //note that 179 is included so it can be UNMARKED as JUMP
        public static readonly int[] Kaiten = { 145, 146, 158, 159, 206, 207 };
        public static readonly int[] Loch_Aberton = { 152, 153, -452, -453, -468, -469, -478, -479, -491, -492 };
        public static readonly int[] Parashant = { 233, 234, 235, 315, 316 };
        public static readonly int[] Stone_Run = { 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 263, 361 };
        public static readonly int[] Trinity_Coast = { 232 };
        public static readonly int[] wtrp_wo1_korodera = { 108, 109, 110, 349, 350 };
        public static readonly int[] wtrp_wo2097_spilskinanke = { 136, 137, 148, 149, 159, 160, 204, 205, 206, 207, 214, 215, 216, 217, 224, 225, 226, 227, 306, 307, 334, 335 };
        public static readonly int[] wtrp_wo2097_vostok_island = { 10, 11, 34, 35, 43, 44, 75, 76, 134, 135, 136, 137, 149, 150, 151, 152, 153, 195, 196, 215, 216 };
        public static readonly int[] wtrp_wo3_manor_top = { 78, 79, 91, 92, 132, 133, 142, 143, 152, 153, 162, 163 };
        public static readonly int[] moltocaldo_desert = { 172, 173, 188, 189, 205, 206, 218, 219, 404, 405 };
        public static readonly int[] Sunken_City = { 120, 121, 285, 286 };

        public bool Needs_Jump_Flags_Set_To_Normal_Boolean;
        public bool Use_Track_Creator_Defined_Jump_Flags;

        public bool In_Override_Normal;
        public bool In_Override_Jump;

        public bool In_Creator_Defined_Normal;
        public bool In_Creator_Defined_Jump;

        public int[] Manually_Set_Section_Jump_Flags(string TrackName)
        {
            switch (TrackName)
            {
                default:
                    Needs_Jump_Flags_Set_To_Normal_Boolean = false;
                    Use_Track_Creator_Defined_Jump_Flags = true;
                    return Array.Empty<int>();

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

                case "bngl-nova split 11":
                    return bngl_nova_split_11;

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

                case "(SW1R Aquilaris Classic)":
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
            }
        }

        public override void Start()
        {
            base.Start();

            //Has_Left_Jump_And_Is_Grounded_Boolean = true;

            Scene activeScene = SceneManager.GetActiveScene();
            Track_Display_Name = ContentManager.Instance.GetTrackBySceneName(activeScene.name).DisplayName;

            foreach (NgTrackData.Section section in NgTrackData.TrackManager.Instance.data.sections)
            {
                if (Manually_Set_Section_Jump_Flags(Track_Display_Name).Contains(section.index) && (Needs_Jump_Flags_Set_To_Normal_Boolean == true) && ((section.index == 179) || (Manually_Set_Section_Jump_Flags(Track_Display_Name) == Antelao)))
                {
                    section.type = NgTrackData.E_SECTIONTYPE.NORMAL;
                }
            }

            RawImage HideTexture1 = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            Destroy(HideTexture1);

            Tiltlock_Maglocked_Interpolation_Ratio = 0f;
            Tiltlock_Non_Maglocked_Interpolation_Ratio = 1f;
            Pseudohugger_Grounded_Interpolation_Ratio = 1f;

            Rotation_Time = 2f;
        }

        public override void Update()
        {
            base.Update();

            Internal_Camera_Rotation = TargetShip.ShipCamera.transform.rotation.eulerAngles;
            
            Tilt_Lock_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, 0f);



            if (Manually_Set_Section_Jump_Flags(Track_Display_Name).Contains(TargetShip.CurrentSection.index))
            {
                In_Override_Jump = true;
            }
            else
            {
                In_Override_Jump = false;
            }

            if ((Use_Track_Creator_Defined_Jump_Flags == true) && (TargetShip.CurrentSection.type == NgTrackData.E_SECTIONTYPE.JUMP))
            {
                In_Creator_Defined_Jump = true;
            }
            else 
            {
                In_Creator_Defined_Jump = false;
            }



            if ((Manually_Set_Section_Jump_Flags(Track_Display_Name).Contains(TargetShip.CurrentSection.index)) == false)
            {
                In_Override_Normal = true;
            }
            else
            {
                In_Override_Normal = false;
            }

            if ((Use_Track_Creator_Defined_Jump_Flags == true) && (TargetShip.CurrentSection.type == NgTrackData.E_SECTIONTYPE.NORMAL))
            {
                In_Creator_Defined_Normal = true;
            }
            else
            {
                In_Creator_Defined_Normal = false;
            }


            if ((Cheats.IntFromPhysicsMod() == 1) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280 == 1))
            {
                TiltLock2280(); //2280 TILT LOCK BEHAVIOR
            }
            else if ((Cheats.IntFromPhysicsMod() == 1) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280 == 2))
            {
                Pseudohugger(); //2280 PSEUDOHUGGER BEHAVIOR
            }
                        
            Previous_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            //Previous_Intermediate_Rotation_Quaternion = Intermediate_Rotation_Quaternion;
            //Previous_Rigidbody_Rotation_Quaternion = Rigidbody_Rotation_Quaternion;
        }

        public void TiltLock2280()
        {
            //if (Cheats.ModernPhysics && (!TargetShip.OnMaglock || !TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent)
            if (!TargetShip.OnMaglock && !TargetShip.CurrentSection.NoTiltLock) //2280 TILT LOCK BEHAVIOR
            {

                if (!TargetShip.CamSim.LookingBehind)
                {
                    Tiltlock_Maglocked_Interpolation_Ratio = 0f;

                    Tiltlock_Non_Maglocked_Interpolation_Ratio += Time.deltaTime;

                    Target_Tilt_Lock_Rotation_Quaternion = Quaternion.Slerp(TargetShip.ShipCamera.transform.rotation, Tilt_Lock_Rotation_Quaternion, Tiltlock_Non_Maglocked_Interpolation_Ratio);
                }
            }

            //else if (Cheats.ModernPhysics && (TargetShip.OnMaglock || TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent) //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            else if (TargetShip.OnMaglock || TargetShip.CurrentSection.NoTiltLock) //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            {

                if (!TargetShip.CamSim.LookingBehind)
                {
                    Tiltlock_Non_Maglocked_Interpolation_Ratio = 0f;

                    Tiltlock_Maglocked_Interpolation_Ratio += Time.deltaTime * Rotation_Time;

                    Target_Tilt_Lock_Rotation_Quaternion = Quaternion.Slerp(Tilt_Lock_Rotation_Quaternion, Quaternion.Euler(transform.InverseTransformDirection(TargetShip.ShipCamera.transform.rotation.eulerAngles)), Tiltlock_Maglocked_Interpolation_Ratio);
                }
            }

            TargetShip.ShipCamera.transform.rotation = Target_Tilt_Lock_Rotation_Quaternion;
        }

        public void Pseudohugger()
        {
            if (In_Override_Jump || In_Creator_Defined_Jump)
            {
                Has_Entered_Jump_Boolean = true;
                Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
            }
            else if ((In_Override_Normal || In_Creator_Defined_Normal) && (TargetShip.PysSim.isShipGrounded == true))
            {
                Has_Entered_Jump_Boolean = false;
            }

            if (Has_Entered_Jump_Boolean == false) //2280 PSEUDOHUGGER BEHAVIOR
            {
                Pseudohugger_Grounded_Interpolation_Ratio += Time.deltaTime;

                if (TargetShip.PysSim.outOfBounds == true /*&& Pseudohugger_Grounded_Interpolation_Ratio >= 1f*/)
                {
                    Stored_Pseudohugger_Rotation_Quaternion = Previous_Pseudohugger_Rotation_Quaternion;
                    Pseudohugger_Grounded_Interpolation_Ratio = 0f;
                    Pseudohugger_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z);
                }
                else
                {
                    Pseudohugger_Rotation_Quaternion = Quaternion.Slerp(Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Pseudohugger_Rotation_Quaternion.eulerAngles.z), Quaternion.LookRotation(TargetShip.ShipCamera.transform.forward, TargetShip.InterpolatedSection.Up), Pseudohugger_Grounded_Interpolation_Ratio);
                }
                Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            }
            else
            {
                Pseudohugger_Grounded_Interpolation_Ratio = 0f;
                Pseudohugger_Rotation_Quaternion = Quaternion.Euler(Internal_Camera_Rotation.x, Internal_Camera_Rotation.y, Stored_Pseudohugger_Rotation_Quaternion.eulerAngles.z);
                Target_Pseudohugger_Rotation_Quaternion = Pseudohugger_Rotation_Quaternion;
            }

            TargetShip.ShipCamera.transform.rotation = Target_Pseudohugger_Rotation_Quaternion;
        }
    }

    public class Camera_Height_Adjustments : ScriptableHud
    {
        public override void Start()
        {
            base.Start();

            RawImage HideTexture2 = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            Destroy(HideTexture2);

        }

        public override void Update()
        {
            base.Update();

            if (TargetShip.CamSim.CameraMode == 2 && !TargetShip.FinishedEvent)
            {
                if ((Cheats.IntFromPhysicsMod() == 1) && VanillaPlusHUDOptions.ModMenuOptions.CanopyCameraAdjustment2280 == 0)
                {
                    TargetShip.ShipCamera.transform.localPosition = Vector3.up * ((TargetShip.ShipToShipCollider.size.y / 2f) - 0.085f); //Raise the 2280 Internal Camera to the same camera height as 2159 Internal Camera (most recently (TargetShip.ShipToShipCollider.size.y / 1.25f))
                }
            }

            if (TargetShip.CamSim.CameraMode == 3 && !TargetShip.FinishedEvent)
            {
                if ((Cheats.IntFromPhysicsMod() == 1))
                {
                    //RAISED COCKPIT CAMERA
                    if (VanillaPlusHUDOptions.ModMenuOptions.CockpitCameraAdjustment2280 == 0)
                    {
                        TargetShip.ShipCamera.transform.localPosition = Vector3.up * ((TargetShip.ShipToShipCollider.size.y / 2f) - 0.085f); //(most recently (TargetShip.ShipToShipCollider.size.y / (341f / 180f))), before that it was (26f / 15f); //Raise the 2280 Cockpit Camera to the same camera height as 2159 Internal Camera (formerly)

                        //NOSECAM MESH
                        if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0))
                        {
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * ((TargetShip.ShipToShipCollider.size.y / 2f) - (250f / 1024f)); //(most recently (TargetShip.ShipToShipCollider.size.y / (341f / 180f))),Raise the 2280 Cockpit Mesh 174f/60f (-132f/60f for default camera height) seems to be the magic number, maybe higher, also recall 68/15f
                        }

                        //INTERIOR COCKPIT MESH
                        else
                        {
                            //Lock cockpit transform to camera transform
                            TargetShip.CockpitParent.GetChild(0).localPosition = TargetShip.ShipCamera.transform.localPosition;
                        }
                    }

                    //INTERNAL COCKPIT CAMERA
                    else
                    {
                        //NOSECAM MESH
                        if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0))
                        {
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * (-(TargetShip.ShipToShipCollider.size.y / 2f) + (13f / 1024f));
                        }

                        //INTERIOR COCKPIT MESH
                        else
                        {
                            //Reset to default position
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.zero;
                        }
                    }
                }

                else if (((Cheats.IntFromPhysicsMod() == 0) || ((Cheats.IntFromPhysicsMod() == 2))))
                {
                    //NOSECAM MESH
                    if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0))
                    {
                        TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * (-(TargetShip.ShipToShipCollider.size.y / 2f) + (13f / 1024f)); //(most recently TargetShip.ShipToShipCollider.size.y / (-132f / 60f))
                    }

                    //INTERIOR COCKPIT MESH
                    else
                    {
                        //Reset to default position
                        TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.zero;
                    }
                }
            }
        }
    }

    public class Extra_Warnings : ScriptableHud
    {
        public Text Final_Lap_Warning_Text;
        public Text Tremor_Warning_Text;
        public Text Hunter_Warning_Text;

        public override void Start()
        {
            base.Start();

            Final_Lap_Warning_Text = CustomComponents.GetById<Text>("FinalLapWarning");
            Tremor_Warning_Text = CustomComponents.GetById<Text>("TremorWarning");
            Hunter_Warning_Text = CustomComponents.GetById<Text>("HunterWarning");

            Final_Lap_Warning_Text.enabled = false;
            Tremor_Warning_Text.enabled = false;
            Hunter_Warning_Text.enabled = false;
        }

        public override void Update()
        {
            if (TargetShip.CurrentLap == Race.MaxLaps && VanillaPlusHUDOptions.ModMenuOptions.FinalLapWarningToggle)
            {
                Final_Lap_Warning_Text.enabled = true;
            }
            else
            {
                Final_Lap_Warning_Text.enabled = false;
            }

            if (Race.QuakeExists && NgPickups.Physical.Tremor.Instance.Owner != TargetShip && VanillaPlusHUDOptions.ModMenuOptions.TremorWarningToggle)
            {
                Tremor_Warning_Text.enabled = true;
            }
            else
            {
                Tremor_Warning_Text.enabled = false;
            }

            if (NgPickups.Physical.Projectiles.HunterProjectile.CurrentProjectile != null && VanillaPlusHUDOptions.ModMenuOptions.HunterWarningToggle)
            {
                Hunter_Warning_Text.enabled = true;
            }
            else
            {
                Hunter_Warning_Text.enabled = false;
            }

            if (VanillaPlusHUDOptions.ModMenuOptions.SkipSongBackwardToggle == true)
            {

            }

            if (NgIo.NgIn.GetButtonDown("Previous Song", 0) && VanillaPlusHUDOptions.ModMenuOptions.SkipSongBackwardToggle == true && NgGameState.IsPaused == false)
            {
                NgMusic.MusicPlayer.Instance.PlayPreviousSong(false); //the boolean passed here is whether or not to allow random song selection
            }

            if (NgGameState.IsPaused && Input.GetKeyDown(KeyCode.A))
            {
                NgMusic.MusicPlayer.Instance.PlayPreviousSong(false);
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

        public override void Start()
        {
            base.Start();

            Overtake_Warning_Outline_Image = CustomComponents.GetById<Image>("OvertakeWarningOutline");
            Overtake_Warning_Background_Image = CustomComponents.GetById<Image>("OvertakeWarningBackground");
            Overtake_Warning_Pipper_Image = CustomComponents.GetById<Image>("OvertakeWarningPipper");
        }

        public override void Update()
        {
            if (TargetShip.CurrentPlace < Ships.Active.Count)
            {
                Overtaking_Ship = Ships.FindShipInPlace(TargetShip.CurrentPlace + 1);
                Longitudinal_Offset_Float = Ships.SectionOffsetBetween(TargetShip, Overtaking_Ship);
            }
            else if (TargetShip.CurrentPlace == Ships.Active.Count && Ships.Active.Count != 1)
            {
                Overtaking_Ship = Ships.FindShipInPlace(TargetShip.CurrentPlace - 1);
                Longitudinal_Offset_Float = Ships.SectionOffsetBetween(Overtaking_Ship, TargetShip);
            }
            else
            {
                Overtaking_Ship = TargetShip;
                Longitudinal_Offset_Float = 25f;
            }

            Raw_Offset = TargetShip.ShipCameraTransform.InverseTransformPoint(Overtaking_Ship.PhysicsPosition);

            Lateral_Offset_Float = Raw_Offset.x;

            Overtake_Warning_Pipper_Image.rectTransform.anchoredPosition = Vector2.Lerp(Pipper_Left_Position, Pipper_Right_Position, ((Mathf.Clamp(Lateral_Offset_Float, -4f, 4f) + 4f) / 8f));
            Overtake_Warning_Pipper_Image.fillAmount = ((1f - (Mathf.Clamp(Longitudinal_Offset_Float, 0f, 25f) * 0.04f)));
        }
    }
}