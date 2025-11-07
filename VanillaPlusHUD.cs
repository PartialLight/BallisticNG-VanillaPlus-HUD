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

namespace ClassLibrary1HUD
{

    public class HudRegister : CodeMod //HudRegister should be renamed to RaceHudRegister because it's for the Race mode

    {
        string id = "VanillaPlus";
        string HudPath = "vanillaplushud.hud";

        public static AssetBundle VanillaPlusHUD;

        public override void OnRegistered(string ModLocation)
        {
            string TestPathString = Path.Combine(ModLocation, "config.ini");
            INIParser ini = new INIParser();
            ini.Open(TestPathString);

            VanillaPlusHUD = AssetBundle.LoadFromFile(Path.Combine(ModLocation, HudPath));
            CustomHudRegistry.RegisterMod(id);
            CustomHudRegistry.RegisterSceneManager("Race", id, new HudManager());

            CustomHudRegistry.RegisterWeaponSprite("autopilot", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/AutoPilot.png")));
            CustomHudRegistry.RegisterWeaponSprite("cannon", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Cannon.png")));
            CustomHudRegistry.RegisterWeaponSprite("energywall", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/EnergyWall.png")));
            CustomHudRegistry.RegisterWeaponSprite("emergencypack", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/EPack.png")));
            CustomHudRegistry.RegisterWeaponSprite("hellstorm", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Hellstorm.png")));
            CustomHudRegistry.RegisterWeaponSprite("hunter", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Hunter.png")));
            //switch (VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle)
            switch (ini.ReadValue("Settings", "MissileIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.MissileIconStyle))
            {
                case 0:
                    CustomHudRegistry.RegisterWeaponSprite("missile", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Missile.png")));
                    break;
                case 1:
                    CustomHudRegistry.RegisterWeaponSprite("missile", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/MissileInscribedTriangle.png")));
                    break;
            }
            CustomHudRegistry.RegisterWeaponSprite("mines", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Mines.png")));
            CustomHudRegistry.RegisterWeaponSprite("plasma", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Plasma.png")));
            CustomHudRegistry.RegisterWeaponSprite("tremor", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Quake.png")));
            //switch (VanillaPlusHUDOptions.ModMenuOptions.RocketsIconStyle)
            switch (ini.ReadValue("Settings", "RocketsIconStyle_ID", VanillaPlusHUDOptions.ModMenuOptions.RocketsIconStyle))
            {
                case 0:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Rockets.png")));
                    break;
                case 1:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/RocketsTrefoil.png")));
                    break;
                case 2:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/RocketsTrefoilUp.png")));
                    break;
                case 3:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/RocketsHorizontal.png")));
                    break;
                case 4:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/RocketsVertical.png")));
                    break;
                case 5:
                    CustomHudRegistry.RegisterWeaponSprite("rockets", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/RocketsTetrahedronNet.png")));
                    break;
            }
            
            CustomHudRegistry.RegisterWeaponSprite("shield", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Shield.png")));
            CustomHudRegistry.RegisterWeaponSprite("turbo", id, CustomHudRegistry.LoadSpriteFromDisk(Path.Combine(ModLocation, "Weapons/Turbo.png")));

            ini.Close();
        }
    }

    public class HudManager : SceneHudManager //Mothership
    {
        public override void OnCreateHuds()
        {
            RegisterHud<Thrust_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Thrust Bar.prefab");
            RegisterHud<Weapon_Display>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Pickup.prefab");
            RegisterHud<Energy_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Energy Bar.prefab");
            RegisterHud<Throttle_Bar>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Throttle Bar.prefab");
            RegisterHud<Rear_View_Mirror>(HudRegister.VanillaPlusHUD, "Assets/MYFOLDER/HUDs/Tutorial/Rear View Mirror.prefab");

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
            RegisterInternalHud("NetworkNameTags"); //Nametags
            RegisterInternalHud("RespawnDarkener"); //Fade to black on respawn
            RegisterInternalHud("Eliminator"); //Shield bars
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

    public class Pitlane_Indicator : ScriptableHud
    {
        //CustomComponents.GetById<Image>("");
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
        public List<Image> Pitlane_Arrows;
        public List<int> Pitlane_Indicator_Section_Indices;
        public List<int> Pitlane_Sides;
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
            Pitlane_Indicator_Animation_Duration = 4.25f;
            Pitlane_Indicator_Animation_Start_Time = Time.time;
            Pitlane_Indicator_Animation_End_Time = Pitlane_Indicator_Animation_Start_Time + Pitlane_Indicator_Animation_Duration;

            First_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 0.25f;
            First_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time + 0.5f;
            First_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 0.75f;

            Second_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 1f;
            Second_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time + 1.25f;
            Second_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 1.5f;

            Third_Arrow_Blink_On_1_Time = Pitlane_Indicator_Animation_Start_Time + 1.75f;
            Third_Arrow_Blink_Off_Time = Pitlane_Indicator_Animation_Start_Time + 2f;
            Third_Arrow_Blink_On_2_Time = Pitlane_Indicator_Animation_Start_Time + 2.25f;

            //MoveToward and blink off, maybe reduce fill over time
            First_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3f;
            First_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 3.125f;
            First_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 3.25f;

            Second_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3.375f;
            Second_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 3.5f;
            Second_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 3.625f;

            Third_Arrow_End_Animation_Time_1 = Pitlane_Indicator_Animation_Start_Time + 3.75f;
            Third_Arrow_End_Animation_Time_2 = Pitlane_Indicator_Animation_Start_Time + 3.875f;
            Third_Arrow_End_Animation_Time_3 = Pitlane_Indicator_Animation_Start_Time + 4f;

            Arrow_1_Start_Position = Pitlane_Arrows[0].rectTransform.anchoredPosition;
            Arrow_2_Start_Position = Pitlane_Arrows[1].rectTransform.anchoredPosition;
            Arrow_3_Start_Position = Pitlane_Arrows[2].rectTransform.anchoredPosition;
            Arrow_Shift_Distance = 21f * Pitlane_Side;
            Arrow_Shift_Vector = new Vector2(Arrow_Shift_Distance, 0);


            while (Time.time <= Pitlane_Indicator_Animation_End_Time)
            {
                //Pitlane_Indicator_Field_Name.enabled = true;
                //Pitlane_Arrows[0].enabled = true;
                

                //Pitlane_Arrows[1].enabled = true;
                

                //Pitlane_Arrows[2].enabled = true;

                //Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                //Pitlane_Arrows[1].fillAmount = (Time.time / Second_Arrow_Blink_On_2_Time);
                //Pitlane_Arrows[2].fillAmount = (Time.time / Third_Arrow_Blink_On_2_Time);

                //FIRST ARROW
                if (Pitlane_Indicator_Animation_Start_Time < Time.time && Time.time <= First_Arrow_Blink_On_1_Time) //Blink first arrow + pit text on
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 3f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }
                if (First_Arrow_Blink_On_1_Time < Time.time && Time.time <= First_Arrow_Blink_Off_Time) //Blink first arrow + pit text off
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = false;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 3f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }
                if (First_Arrow_Blink_Off_Time < Time.time && Time.time <= First_Arrow_Blink_On_2_Time) //Blink first arrow + pit text back on, first arrow should be finished filling after this completes
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].fillAmount = ((Time.time - Pitlane_Indicator_Animation_Start_Time) * 3f / (First_Arrow_Blink_On_2_Time - Pitlane_Indicator_Animation_Start_Time));
                }

                //SECOND ARROW
                if (First_Arrow_Blink_On_2_Time < Time.time && Time.time <= Second_Arrow_Blink_On_1_Time) //Blink second arrow on
                {
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 3f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }
                if (Second_Arrow_Blink_On_1_Time < Time.time && Time.time <= Second_Arrow_Blink_Off_Time) //Blink second arrow off
                {
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 3f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }
                if (Second_Arrow_Blink_Off_Time < Time.time && Time.time <= Second_Arrow_Blink_On_2_Time) //Blink second arrow back on, second arrow should be finished filling after this completes
                {
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].fillAmount = ((Time.time - First_Arrow_Blink_On_2_Time) * 3f / (Second_Arrow_Blink_On_2_Time - First_Arrow_Blink_On_2_Time));
                }

                //THIRD ARROW
                if (Second_Arrow_Blink_On_2_Time < Time.time && Time.time <= Third_Arrow_Blink_On_1_Time) //Blink third arrow on
                {
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 3f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }
                if (Third_Arrow_Blink_On_1_Time < Time.time && Time.time <= Third_Arrow_Blink_Off_Time) //Blink third arrow off
                {
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 3f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }
                if (Third_Arrow_Blink_Off_Time < Time.time && Time.time <= Third_Arrow_Blink_On_2_Time) //Blink third arrow back on, third arrow should be finished filling after this completes
                {
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].fillAmount = ((Time.time - Second_Arrow_Blink_On_2_Time) * 3f / (Third_Arrow_Blink_On_2_Time - Second_Arrow_Blink_On_2_Time));
                }

                //End of the animation, FIRST ARROW
                if (First_Arrow_End_Animation_Time_1 < Time.time && Time.time <= First_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = false;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                }
                if (First_Arrow_End_Animation_Time_2 < Time.time && Time.time <= First_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[0].enabled = true;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                }
                if (First_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_1)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[0].enabled = false;
                    Pitlane_Arrows[0].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_1_Start_Position, (Arrow_1_Start_Position + Arrow_Shift_Vector), ((Time.time - First_Arrow_End_Animation_Time_1) / (Second_Arrow_End_Animation_Time_1 - First_Arrow_End_Animation_Time_1)));
                }
                if (Second_Arrow_End_Animation_Time_1 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                }
                if (Second_Arrow_End_Animation_Time_2 < Time.time && Time.time <= Second_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[1].enabled = true;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                }
                if (Second_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_1)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[1].enabled = false;
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[1].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_2_Start_Position, (Arrow_2_Start_Position + Arrow_Shift_Vector), ((Time.time - Second_Arrow_End_Animation_Time_1) / (Third_Arrow_End_Animation_Time_1 - Second_Arrow_End_Animation_Time_1)));
                }
                if (Third_Arrow_End_Animation_Time_1 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_2)
                {
                    Pitlane_Indicator_Field_Name.enabled = false;
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                }
                if (Third_Arrow_End_Animation_Time_2 < Time.time && Time.time <= Third_Arrow_End_Animation_Time_3)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[2].enabled = false;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                }
                if (Third_Arrow_End_Animation_Time_3 < Time.time && Time.time <= Pitlane_Indicator_Animation_End_Time)
                {
                    Pitlane_Indicator_Field_Name.enabled = true;
                    Pitlane_Arrows[2].enabled = true;
                    Pitlane_Arrows[2].rectTransform.anchoredPosition = Vector2.Lerp(Arrow_3_Start_Position, (Arrow_3_Start_Position + Arrow_Shift_Vector), ((Time.time - Third_Arrow_End_Animation_Time_1) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_1)));
                    Pitlane_Arrows[2].fillAmount =  1f - ((Time.time - Third_Arrow_End_Animation_Time_3) / (Pitlane_Indicator_Animation_End_Time - Third_Arrow_End_Animation_Time_3));
                }

                yield return new WaitForEndOfFrame();
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

            Pitlane_Indicator_Section_Indices = new List<int> { };
            Pitlane_Sides = new List<int> { };

            //Pitlane_Indicator_Section_Index = NgTrackData.TrackManager.Instance.PitlaneIndicators[0].Trigger.TargetSection.index;
            //Pitlane_Side = NgTrackData.TrackManager.Instance.PitlaneIndicators[0].PitlaneSide;

            foreach (NgTrackData.Triggers.PitlaneIndicator p in NgTrackData.TrackManager.Instance.PitlaneIndicators)
            {
                Pitlane_Indicator_Section_Indices.Add(NgTrackData.TrackManager.Instance.PitlaneIndicators[NgTrackData.TrackManager.Instance.PitlaneIndicators.IndexOf(p)].Trigger.TargetSection.index);
                Pitlane_Sides.Add(NgTrackData.TrackManager.Instance.PitlaneIndicators[NgTrackData.TrackManager.Instance.PitlaneIndicators.IndexOf(p)].PitlaneSide);
            }            

            //if (Pitlane_Side == 1)
            //{
                //Pitlane_Arrows = new List<Image> { Right_Pitlane_Indicator_Arrow_1, Right_Pitlane_Indicator_Arrow_2, Right_Pitlane_Indicator_Arrow_3 };
            //}
            //else if (Pitlane_Side == -1)
            //{
                //Pitlane_Arrows = new List<Image> { Left_Pitlane_Indicator_Arrow_1, Left_Pitlane_Indicator_Arrow_2, Left_Pitlane_Indicator_Arrow_3 };
            //}
        }

        public override void Update()
        {
            base.Update();

            Pitlane_Current_Section = TargetShip.CurrentSection.index;

            if (Pitlane_Current_Section > Pitlane_Previous_Section)
            {
                if (Pitlane_Indicator_Section_Indices.Contains(TargetShip.CurrentSection.index))
                {
                    Pitlane_Side = Pitlane_Sides[Pitlane_Indicator_Section_Indices.IndexOf(TargetShip.CurrentSection.index)];
                    if (Pitlane_Side == 1)
                    {
                        Pitlane_Arrows = new List<Image> { Right_Pitlane_Indicator_Arrow_1, Right_Pitlane_Indicator_Arrow_2, Right_Pitlane_Indicator_Arrow_3 };
                    }
                    else if (Pitlane_Side == -1)
                    {
                        Pitlane_Arrows = new List<Image> { Left_Pitlane_Indicator_Arrow_1, Left_Pitlane_Indicator_Arrow_2, Left_Pitlane_Indicator_Arrow_3 };
                    }
                    StartCoroutine(Pitlane_Indicator_Animation());
                }
            }
            
            Pitlane_Previous_Section = Pitlane_Current_Section;

        }
    }

    public class Relative_Time_Display : ScriptableHud
    {
        public Text Relative_Time_Readout;
        public Vector4 Race_Leader_Green = new Vector4(181f / 255f, 1f, 29f / 255f, 1f);
        public Vector4 Contender_Red = new Vector4(1f, 28f / 255f, 36f / 255f, 1f);
        public Vector4 Tremor_Safety_Blue = new Vector4(0f, (163f / 256f), (233f / 256f), 1f);
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
            Second_Place_Lap_Adjust = Ships.FindShipInPlace(2).CurrentLap - 1; //Multiply Track_Section_Max by this value

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
        public string Current_Music;
        public string Previous_Music;

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
                yield return new WaitForEndOfFrame();
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

        public override void Update()
        {
            base.Update();

            //Current_Music = MusicPlayer.Instance.GetSongDisplayName();
            //Music_Display_Readout.text = MusicPlayer.Instance.GetSongDisplayName();

            if (Current_Music != Previous_Music)
            {
                StartCoroutine(Music_Flasher());
            }

            Previous_Music = Current_Music;
        }

        public void Music_Display_Method(string name)
        {
            Music_Display_Readout.text = name;
            Current_Music = name;
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
                yield return new WaitForEndOfFrame();
            }
            Damage_Flasher_Image.enabled = false;
            //yield return new WaitForEndOfFrame();
        }

        IEnumerator Damage_Flash_Duration()
        {
            while ((TargetShip.PysSim.enginePower < 1f) || (TargetShip.PysSim.engineAccel < 1f))
            {
                Damage_Flasher_Image.enabled = true;
                yield return new WaitForEndOfFrame();
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

        public Vector2 Hyperthrust_Bar_Lowered_Adjust_Vector;
        public Vector2 Hyperthrust_Bar_Centered_Adjust_Vector;
        public Vector2 Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
        public Vector2 Hyperthrust_Bar_Centered_Wide_Adjust_Vector;

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

            Hyperthrust_Bar_Lowered_Adjust_Vector = new Vector2(-302f, -500f);
            Hyperthrust_Bar_Centered_Adjust_Vector = new Vector2(-302f, -367f);
            Hyperthrust_Bar_Lowered_Wide_Adjust_Vector = new Vector2(0f, -500f);
            Hyperthrust_Bar_Centered_Wide_Adjust_Vector = new Vector2(0f, -367f);

            if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarPosition == 1)
            {
                Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
                Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Lowered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarPosition == 2)
            {
                Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
                Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Centered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarPosition == 3)
            {
                Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Lowered_Wide_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.HyperThrustBarPosition == 4)
            {
                Hyperthrust_Bar_Numeric_Readout.rectTransform.anchoredPosition = HyperThrust_Bar_Numeric_Readout_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Background.rectTransform.anchoredPosition = Hyperthrust_Bar_Background_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Whiteout.rectTransform.anchoredPosition = Hyperthrust_Bar_Whiteout_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
                Hyperthrust_Bar_Image.rectTransform.anchoredPosition = Hyperthrust_Bar_Image_Default_Position + Hyperthrust_Bar_Centered_Wide_Adjust_Vector;
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
        public string Speedpad_Counter_Units_String;

        public float Speedpad_Counter_Current_Speedpad_Time_2280;
        public float Speedpad_Counter_Previous_Speedpad_Time_2280;
        public float Speedpad_Counter_2280;

        public Vector2 Speedpad_Counter_Lowered_Adjust_Vector;
        public Vector2 Speedpad_Counter_Centered_Adjust_Vector;
        public Vector2 Speedpad_Counter_Lowered_Wide_Adjust_Vector;
        public Vector2 Speedpad_Counter_Centered_Wide_Adjust_Vector;

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

            Speedpad_Counter_Lowered_Adjust_Vector = new Vector2(287, -502);
            Speedpad_Counter_Centered_Adjust_Vector = new Vector2(287, -371);
            Speedpad_Counter_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
            Speedpad_Counter_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

            if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 1)
            {
                Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
                Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Lowered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 2)
            {
                Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
                Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Centered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 3)
            {
                Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
                Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Lowered_Wide_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 4)
            {
                Speedpad_Count_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Count_Numeric_Readout_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                Speedpad_Counter_Background.rectTransform.anchoredPosition = Speedpad_Counter_Background_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                Speedpad_Counter_Whiteout.rectTransform.anchoredPosition = Speedpad_Counter_Whiteout_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
                Speedpad_Counter_Image.rectTransform.anchoredPosition = Speedpad_Counter_Image_Default_Position + Speedpad_Counter_Centered_Wide_Adjust_Vector;
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
                Speedpad_Counter_Units_String = Speedpad_Counter_2280.ToString(); //these two lines
                Speedpad_Count_Numeric_Readout.text = "+" + Speedpad_Counter_Units_String; //can be combined into a single line
                if (Speedpad_Counter_2280 == 0f)
                {
                    Speedpad_Count_Numeric_Readout.text = "";
                }

                Speedpad_Counter_Previous_Speedpad_Time_2280 = TargetShip.PysSim.modernPadPushTimer;
            }
            else
            {
                Speedpad_Counter_Image.fillAmount = TargetShip.BoostAcceleration / 12f;
                Speedpad_Counter_Units_String = TargetShip.BoostAcceleration.ToString(); //these two lines
                Speedpad_Count_Numeric_Readout.text = "+" + Speedpad_Counter_Units_String; //can be combined into a single line
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

        public Vector2 Speedpad_Timer_Lowered_Adjust_Vector;
        public Vector2 Speedpad_Timer_Centered_Adjust_Vector;
        public Vector2 Speedpad_Timer_Lowered_Wide_Adjust_Vector;
        public Vector2 Speedpad_Timer_Centered_Wide_Adjust_Vector;

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

            Speedpad_Timer_Lowered_Adjust_Vector = new Vector2(287, -502);
            Speedpad_Timer_Centered_Adjust_Vector = new Vector2(287, -371);
            Speedpad_Timer_Lowered_Wide_Adjust_Vector = new Vector2(0, -502);
            Speedpad_Timer_Centered_Wide_Adjust_Vector = new Vector2(0, -371);

            if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 1)
            {
                Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
                Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Lowered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 2)
            {
                Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
                Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Centered_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 3)
            {
                Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
                Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Lowered_Wide_Adjust_Vector;
            }
            else if (VanillaPlusHUDOptions.ModMenuOptions.SpeedPadElementsPosition == 4)
            {
                Speedpad_Timer_Numeric_Readout.rectTransform.anchoredPosition = Speedpad_Timer_Numeric_Readout_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                Speedpad_Timer_Background.rectTransform.anchoredPosition = Speedpad_Timer_Background_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                Speedpad_Timer_Whiteout.rectTransform.anchoredPosition = Speedpad_Timer_Whiteout_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
                Speedpad_Timer_Image.rectTransform.anchoredPosition = Speedpad_Timer_Image_Default_Position + Speedpad_Timer_Centered_Wide_Adjust_Vector;
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
        public List<Text> Lap_Texts;
        public List<Image> Lap_Images;
        public Vector4 Lap_Diamond_Green = new Vector4(181f/255f, 1f, 29f/255f, 1f);
        public Vector4 Lap_Diamond_Red = new Vector4(1f, 28f/255f, 36f/255f, 1f);

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

            Lap_Texts = new List<Text> { Lap_Time_1, Lap_Time_2, Lap_Time_3, Lap_Time_4, Lap_Time_5 };
            Lap_Images = new List<Image> { Lap_Time_Arrow_1, Lap_Time_Arrow_2, Lap_Time_Arrow_3, Lap_Time_Arrow_4, Lap_Time_Arrow_5 };

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

        public Vector2 Weapon_Display_Adjust_Vector;

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

            Weapon_Display_Adjust_Vector = new Vector2(0, -178);

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

        //public readonly Vector4 Color_Breakpoint_0 = new Vector4(255, 28, 36, 255); //Red
        //public readonly Vector4 Color_Breakpoint_1 = new Vector4(255, 127, 39, 255); //Orange
        //public readonly Vector4 Color_Breakpoint_2 = new Vector4(255, 242, 0, 255); //Yellow
        //public readonly Vector4 Color_Breakpoint_3 = new Vector4(87, 255, 23, 255); //Green
        //public readonly Vector4 Color_Breakpoint_4 = new Vector4(0, 162, 232, 255); //Blue
        //public readonly Vector4 Color_Breakpoint_5 = new Vector4(255, 255, 255, 255); //White
        public readonly Vector4 Color_Breakpoint_0 = new Vector4(1f, (29f / 256f), (37f / 256f), 1f); //Red
        public readonly Vector4 Color_Breakpoint_1 = new Vector4(1f, (128f / 256f), (40f / 256f), 1f); //Orange
        public readonly Vector4 Color_Breakpoint_2 = new Vector4(1f, (243f / 256f), 0f, 1f); //Yellow
        public readonly Vector4 Color_Breakpoint_3 = new Vector4((88f / 256f), 1f, (24f / 256f), 1f); //Green
        public readonly Vector4 Color_Breakpoint_4 = new Vector4(0f, (163f / 256f), (233f / 256f), 1f); //Blue
        public readonly Vector4 Color_Breakpoint_5 = new Vector4(1f, 1f, 1f, 1f); //White
        public Vector4 Energy_Bar_Background_Original_Color;

        public Vector4 Salmon = new Vector4(1f, (112f / 255f), (112f / 255f), 0.8f); //Don't have sprites you want to change the color of via script pre-colored and pre-alpha'd outside of Unity because modifying them in script modifies the color channels relative to the colors in the file provided as a sprite rather than absolute colors
        public Vector4 Dark_Red = new Vector4(1f, 0f, 0f, 0.8f);

        IEnumerator EnergyLowColorPulse() //Might have to set the color to the lerp target after Time.time == LerpInTime and after Time.time == EndTime
        {
            Energy_Low_Coroutine_Running = true;
            float Energy_Low_Color_Pulse_Start_Time = Time.time;
            float Energy_Low_Color_Pulse_End_Time = Energy_Low_Color_Pulse_Start_Time + 0.5f;

            float Energy_Low_Color_Pulse_Lerp_In_Time = Energy_Low_Color_Pulse_Start_Time + 0.25f;

            while (Time.time < Energy_Low_Color_Pulse_End_Time)
            {
                if (Energy_Low_Color_Pulse_Start_Time < Time.time && Time.time <= Energy_Low_Color_Pulse_Lerp_In_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Salmon, Dark_Red, (Time.time - Energy_Low_Color_Pulse_Start_Time) / (Energy_Low_Color_Pulse_Lerp_In_Time - Energy_Low_Color_Pulse_Start_Time));
                }
                if (Energy_Low_Color_Pulse_Lerp_In_Time < Time.time && Time.time <= Energy_Low_Color_Pulse_End_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Dark_Red, Salmon, (Time.time - Energy_Low_Color_Pulse_Start_Time) / (Energy_Low_Color_Pulse_End_Time - Energy_Low_Color_Pulse_Start_Time));
                }
                yield return new WaitForEndOfFrame();
            }
            Energy_Low_Coroutine_Running = false;
        }

        IEnumerator EnergyCriticalColorPulse() //Might have to set the color to the lerp target after Time.time == LerpInTime and after Time.time == EndTime
        {
            Energy_Critical_Coroutine_Running = true;
            float Energy_Critical_Color_Pulse_Start_Time = Time.time;
            float Energy_Critical_Color_Pulse_End_Time = Energy_Critical_Color_Pulse_Start_Time + 0.25f;

            float Energy_Critical_Color_Pulse_Lerp_In_Time = Energy_Critical_Color_Pulse_Start_Time + 0.125f;

            while (Time.time < Energy_Critical_Color_Pulse_End_Time)
            {
                if (Energy_Critical_Color_Pulse_Start_Time < Time.time && Time.time <= Energy_Critical_Color_Pulse_Lerp_In_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Salmon, Color.red, (Time.time - Energy_Critical_Color_Pulse_Start_Time) / (Energy_Critical_Color_Pulse_Lerp_In_Time - Energy_Critical_Color_Pulse_Start_Time));
                }
                if (Energy_Critical_Color_Pulse_Lerp_In_Time < Time.time && Time.time <= Energy_Critical_Color_Pulse_End_Time)
                {
                    Energy_Bar_Background_Image.color = Color.Lerp(Color.red, Salmon, (Time.time - Energy_Critical_Color_Pulse_Start_Time) / (Energy_Critical_Color_Pulse_End_Time - Energy_Critical_Color_Pulse_Start_Time));
                }
                yield return new WaitForEndOfFrame();
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

        public float Current_Speedpad_Time;
        public float Previous_Speedpad_Time;

        public float Current_Speedpad_Time_2280;
        public float Previous_Speedpad_Time_2280;

        IEnumerator SpeedPadColorPulse_Coroutine()
        {
            Vector4 Dark_Blue = new Vector4(0f, (96f / 255f), 1f, 1f);
            Vector4 Light_Blue = new Vector4((154f / 255f), (217f / 255f), 1f, 1f);

            float Speed_Pad_Color_Pulse_Start_Time = Time.time;
            float Speed_Pad_Color_Pulse_End_Time = Speed_Pad_Color_Pulse_Start_Time + 0.625f;

            //float Speed_Pad_Color_Pulse_Lerp_In_Time = Speed_Pad_Color_Pulse_Start_Time + 0.25f;

            while (Time.time < Speed_Pad_Color_Pulse_End_Time)
            {
                //if (Speed_Pad_Color_Pulse_Start_Time < Time.time && Time.time <= Speed_Pad_Color_Pulse_Lerp_In_Time)
                //{
                //Throttle_Bar_Image.color = Color.Lerp(Salmon, Color.red, (Time.time - Speed_Pad_Color_Pulse_Start_Time) / (Speed_Pad_Color_Pulse_Lerp_In_Time - Speed_Pad_Color_Pulse_Start_Time));
                //}
                if (Speed_Pad_Color_Pulse_Start_Time < Time.time && Time.time <= Speed_Pad_Color_Pulse_End_Time)
                {
                    Throttle_Bar_Image.color = Color.Lerp(Dark_Blue, Light_Blue, (Time.time - Speed_Pad_Color_Pulse_Start_Time) / (Speed_Pad_Color_Pulse_End_Time - Speed_Pad_Color_Pulse_Start_Time));
                }
                yield return new WaitForEndOfFrame();
            }
            Throttle_Bar_Image.color = Throttle_Bar_Image_Original_Color;
        }

        public override void Start()
        {
            base.Start();

            Throttle_Bar_Background_Image = CustomComponents.GetById<Image>("ThrottleBarBackground");
            Throttle_Bar_Image = CustomComponents.GetById<Image>("ThrottleBar");

            Throttle_Bar_Image_Original_Color = Throttle_Bar_Image.color;

            //NgRaceEvents.OnShipHitSpeedPad += SpeedPadColorPulse;
        }

        public override void Update()
        {
            base.Update();

            Throttle_Bar_Image.fillAmount = Mathf.Min(TargetShip.PysSim.enginePower, TargetShip.PysSim.engineAccel);

            Current_Speedpad_Time = TargetShip.BoostTimer;
            Current_Speedpad_Time_2280 = TargetShip.PysSim.modernPadPushTimer;

            if ((Current_Speedpad_Time > Previous_Speedpad_Time) || (Current_Speedpad_Time_2280 > Previous_Speedpad_Time_2280))
            {
                StartCoroutine(SpeedPadColorPulse_Coroutine());
            }

            Previous_Speedpad_Time = TargetShip.BoostTimer;
            Previous_Speedpad_Time_2280 = TargetShip.PysSim.modernPadPushTimer;

        }            

        //public override void OnDestroy()
        //{
            //NgRaceEvents.OnShipHitSpeedPad -= SpeedPadColorPulse;

            //base.OnDestroy();
        //}

        //public void SpeedPadColorPulse(ShipController ship, BallisticUnityTools.TrackTools.TrackPad pad) //Doesn't do anything
        //{
            //if (ship.IsPlayer)
            //{
                //StartCoroutine(SpeedPadColorPulse_Coroutine());
           // }
        //}
    }

    public class Rear_View_Mirror : ScriptableHud //Special thanks to Dekaid for sharing his rear view mirror implementation, I would never have figured out how to set this up on my own
    {
        public RawImage RearViewMirror;
        public Camera Rear_View_Mirror_Camera;
        public RectTransform Empty_Game_Object;
        public RenderTexture Rear_View_Mirror_Feed;

        public Vector3 Correct_Rotation;
        public Vector3 Initial_Rotation;
        public Vector3 Target_Rotation;
        public Vector3 Section_Right_Vector;
        public Vector3 Forward_Vector;
        public Vector3 Up_Vector;
        public Vector3 Next_Forward_Vector;
        public Vector3 Next_Up_Vector;
        public Quaternion Next_Rotation_Quaternion;
        public float Correct_Rotation_X; //I didn't use it
        public float Correct_Rotation_Y; //I didn't use it
        public float Correct_Rotation_Z; //I didn't use it
        public Quaternion Correct_Rotation_Quaternion;
        public Quaternion Intermediate_Rotation_Quaternion;
        public Quaternion Maglock_Rotation_Quaternion;
        public float Rotation_Time;
        public float progress;
        public float rollAngle;
        public bool PreviousSectionOnMaglock;

        public float startTime;
        public float endTime;
        public float lastWidth, lastHeight;

        public float Interpolation_Ratio;
        public float Non_Maglock_Interpolation_Ratio;
        public float Aerial_Interpolation_Ratio;

        public Vector2 Rear_View_Mirror_Adjust_Vector;

        public override void Start()
        {
            base.Start();

            lastWidth = 640;
            lastHeight = 480;

            RearViewMirror = CustomComponents.GetById<RawImage>("RearViewMirrorTexture");
            RearViewMirror.color = Color.clear;

            Interpolation_Ratio = 0f;
            Non_Maglock_Interpolation_Ratio = 1f;
            Aerial_Interpolation_Ratio = 0f;

            Rotation_Time = 2f;

            Rear_View_Mirror_Adjust_Vector = new Vector2(0, 223);
            
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
            if (TargetShip.CamSim.CameraMode == 2 && !TargetShip.FinishedEvent)
            {
                if ((Cheats.IntFromPhysicsMod() == 1) && VanillaPlusHUDOptions.ModMenuOptions.CanopyCameraAdjustment2280 == 0)
                {
                    TargetShip.ShipCamera.transform.localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / 1.25f; //Raise the 2280 Internal Camera to the same camera height as 2159 Internal Camera
                }
            }

            if (TargetShip.CamSim.CameraMode == 3 && !TargetShip.FinishedEvent)
            {


                if ((Cheats.IntFromPhysicsMod() == 1))
                {
                    if (VanillaPlusHUDOptions.ModMenuOptions.CockpitCameraAdjustment2280 == 0) //RAISED COCKPIT CAMERA
                    {

                        TargetShip.ShipCamera.transform.localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / (341f / 180f); //(26f / 15f); //Raise the 2280 Cockpit Camera to the same camera height as 2159 Internal Camera

                        if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0)) //NOSECAM MESH
                        {
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / (820f / 60f); //Raise the 2280 Cockpit Mesh 174f/60f (-132f/60f for default camera height) seems to be the magic number, maybe higher, also recall 68/15f
                        }
                        else //INTERIOR COCKPIT MESH
                        {
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / (341f / 180f);
                        }
                    }
                    else //INTERNAL COCKPIT CAMERA
                    {
                        if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0)) //NOSECAM MESH
                        {
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / (-132f / 60f);
                        }
                        else //INTERIOR COCKPIT MESH
                        {
                            //Do Nothing
                            TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.zero;
                        }
                    }
                }
                else if (((Cheats.IntFromPhysicsMod() == 0) || ((Cheats.IntFromPhysicsMod() == 2))))
                {
                    if ((VanillaPlusHUDOptions.ModMenuOptions.CockpitMeshAdjustment == 0)) //NOSECAM MESH
                    {
                        TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.up * TargetShip.ShipToShipCollider.size.y / (-132f / 60f);
                    }
                    else //INTERIOR COCKPIT MESH
                    {
                        //Do Nothing
                        TargetShip.CockpitParent.GetChild(0).localPosition = Vector3.zero;
                    }
                }
            }



            Target_Rotation = TargetShip.ShipCamera.transform.rotation.eulerAngles;
            //Next_Rotation_Quaternion = Quaternion.LookRotation(TargetShip.ForwardOnSection(TargetShip.CurrentSection), TargetShip.InterpolatedSection.Up);
            Next_Rotation_Quaternion = Quaternion.LookRotation(TargetShip.ShipCamera.transform.forward, TargetShip.InterpolatedSection.Up);
            Correct_Rotation_Quaternion = Quaternion.Euler(Target_Rotation.x, Target_Rotation.y, 0f);
            Maglock_Rotation_Quaternion = Quaternion.Euler(Target_Rotation.x, Target_Rotation.y, Next_Rotation_Quaternion.eulerAngles.z);
            Intermediate_Rotation_Quaternion = TargetShip.RBody.transform.rotation;

            //TargetShip.ModernPhysicsGroundedForce = 4f;

            if ((Cheats.IntFromPhysicsMod() == 1) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280 == 2)) //2280 PSEUDOHUGGER BEHAVIOR
            {
                TargetShip.ShipCamera.transform.rotation = Next_Rotation_Quaternion;
            }

            //if (Cheats.ModernPhysics && (!TargetShip.OnMaglock || !TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent) //2280 TILT LOCK BEHAVIOR
            if ((Cheats.IntFromPhysicsMod() == 1) && (!TargetShip.OnMaglock && !TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280 == 1)) //2280 TILT LOCK BEHAVIOR
            {

                if (!TargetShip.CamSim.LookingBehind)
                {
                    Interpolation_Ratio = 0f;

                    Non_Maglock_Interpolation_Ratio += Time.deltaTime;

                    TargetShip.ShipCamera.transform.rotation = Quaternion.Slerp(Intermediate_Rotation_Quaternion, Correct_Rotation_Quaternion, Non_Maglock_Interpolation_Ratio);
                }
            }

            //else if (Cheats.ModernPhysics && (TargetShip.OnMaglock || TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent) //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            else if ((Cheats.IntFromPhysicsMod() == 1) && (TargetShip.OnMaglock || TargetShip.CurrentSection.NoTiltLock) && !TargetShip.FinishedEvent && (VanillaPlusHUDOptions.ModMenuOptions.CameraBehavior2280 == 1)) //2280 TILT LOCK BEHAVIOR ON SECTIONS WITH NOTILTLOCK OR ON MAGLOCK SECTIONS/TRACKS THAT FORCE FLOORHUGGER
            {

                if (!TargetShip.CamSim.LookingBehind)
                {
                    Non_Maglock_Interpolation_Ratio = 0f;

                    Interpolation_Ratio += Time.deltaTime * Rotation_Time;

                    TargetShip.ShipCamera.transform.rotation = Quaternion.Slerp(Correct_Rotation_Quaternion, Quaternion.Euler(transform.InverseTransformDirection(Intermediate_Rotation_Quaternion.eulerAngles)), Interpolation_Ratio);
                }
            }

            if (!TargetShip.CamSim.LookingBehind)
            {
                RearViewMirror.transform.localScale = new Vector3(-1f, 1f);
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
                RearViewMirror.transform.localScale = new Vector3(1f, 1f);
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
}