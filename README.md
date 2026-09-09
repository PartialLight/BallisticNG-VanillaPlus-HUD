# BallisticNG-VanillaPlus-HUD

A Custom HUD Code Mod for BallisticNG with a similar layout to the internal HUD

### Acknowledgements

Special thanks to Dekaid for sharing his rear view mirror implementation and elaborating on GT3HUD's design and functionality, Vonsnake for general help with modding/code examples/updating the options API/documentation and related questions, Dinir N. for friendly correspondence and potential future collaboration, and moebius for fixing alignment and other issues with the weapon sprites.  

Check out Dekaid's GT3HUD and Dinir's Streamliner HUD (they're both much more professionally put-together than my own mod), and buy BallisticNG and its DLCs if you haven't already.

### Supported Modes

- [Single\] Race
- Tournament (Which is just a series of [Single\] Races\)
- Team Race
- Online Team Race
- Time Trial
- Speed Lap
- Survival
- Knockout
- Eliminator
- Upsurge
- Rush Hour
- Stunt
- Track Creator
- Precision
- Practice

### Supported Version(s\)
- 1.4.1.6

### Features

1. Vanilla HUD Alignment and Positioning: HUD elements are positioned as closely as possible to their internal counterparts where applicable; Consequently, the internal Music Display and Pitlane Indicator can be used instead of the custom ones. Thrust Bar and Throttle Bar positions have been swapped.
2. Vanilla Weapon Icon Symbology with unique colours and alternative designs: Weapon icons use coloured versions of the vanilla weapon sprites (redrawn and realigned courtesy of moebius\). Missile has 1 alternative icon, Rockets have 5. Because weapon sprites are loaded and registered on game start, changes to icon preference require game restart to take effect.
3. Rear View Mirror: Switches to forward camera when holding the look behind input. Can be enabled/disabled per physics mode (2159, 2280, Floorhugger\). Users can choose to swap the position of the rear view mirror and weapon pickup display.
4. Toggleable Name Tags: Nametags can be toggled on/off and will otherwise use the internal interface settings.
5. Toggleable Shield Bars: Shield Bars for other ships can be toggled on/off and will otherwise use the internal interface settings.
6. Toggleable Respawn Darkener: The screen darkening effect that occurs on respawn can be toggled off.
7. Multiplayer Countdown End Sound: Toggleable custom sound that punctuates the triple sequence of beeps for the last 3 seconds of the multiplayer lobby countdown sequence. Instructions on how to replace with your own custom sound found in Audio folder.
8. Alternative Readouts: Ship speed can be displayed in terms of Kilometres/Miles Per Hour, in terms of Engine Force units, or in terms of Unity world units travelled per second. Ship Energy can be displayed to 5 digits of decimal precision. Can be configured to show fewer/zero decimal places. Absolute Shield Values can be displayed based on vanilla BallisticNG calculations, or modded calculations.
9. Damage Flasher: A small red dot that flashes whenever you are slowed down by or take damage from any source (except afterburner usage\).
10. Relative Time Readout: Your time behind first place, or your time ahead of second place if you're in first place. Updates continuously as you pass over individual track sections. Changes colour to blue when out of tremor range of second place.
11. Recharge Sum: The amount of energy recharged while in the pitlane (or from absorbing a pickup\), which changes into an estimation of the total energy that could potentially have been restored after the player ship's shield integrity reaches 100. Inspired by a similar feature in Streamliner.
12. Last Attacker Display: The name of the player or AI-controlled ship that last hit you with a weapon.
13. Hyperthrust Bar: Bar that fills up and decays based on afterburner usage, with accompanying Hyperthrust Force readout (readout text can be disabled\). Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout, or centered to be displayed above it.
14. Speed Pad Counter: Keeps track of how many speed pads have been hit, with accompanying speed pad acceleration boost readout (+4/+8/+12 in terms of Engine Force units for 2159, readout text can be disabled\). While functional in 2280, information conveyed is less inherently useful due to the changes 2280 makes to speed pad function. Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout, or centered to be displayed above it.
15. Speed Pad Timer: Bar that fills up and decays based on remaining speed bad boost timer, with accompanying readout (readout text can be disabled\). While functional in 2280, information conveyed is less inherently useful due to the changes 2280 makes to speed pad function. Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout, or centered to be displayed above it.
16. Speed Pad Colour Pulse: The Throttle Bar (Yellow\) will pulse from dark blue to light blue when you pass over a speed pad.
17. Energy Bar Colour Breakpoints: Energy Bar will cycle through colours based on damage breakpoints informed by worst-case and best-case damage received from weapons (Calculated from standard-roster ship shield and firepower stats, prototypes like the NX2000 excluded.\)
18. Canopy Camera Adjust for 2280: Users can choose to raise the position of the internal camera to be the same as 2159 in 2280.
19. Cockpit Camera Adjust for 2280: Users can choose to raise the position of the cockpit camera to be the same as 2159 in 2280.
20. Cockpit Camera Adjust for 2159: Users can choose to raise the position of the cockpit camera to be the same as the internal camera height in 2159.
21. Cockpit Mesh Adjust: Users can choose whether to have the cockpit interior or the ship's nose/forward hull visible when using the cockpit camera in any physics mode.
22. Canopy Mesh Adjust: Users can choose whether to have the ship's nose/forward hull visible or hidden when using the internal camera in any physics mode.
23. (EXPERIMENTAL\) Alternate Camera Modes for 2280: Users can choose to have the camera's tilt lock to world up (similar to 2159 camera behavior\) in 2280 except when on maglock or no-tilt-lock surfaces, or orientate to the track surface at all times (similar to Floorhugger\).
24. Extra Warnings: Users can choose to receive visual warnings when it's the final lap of a race, when an enemy Tremor is active, or when a Hunter missile is active, and the amount of time remaining on an active shield. For Survival and Upsurge, users have the choice of receiving visual alerts for when the current zone being progressed through is considered perfect (Survival-specific\), when the amount of zones possessed by an Upsurge ship is at the max (Upsurge-specific\), and when the Upsurge zone target is within reach (Upsurge-specific\).
25. Overtake Radar: Wipeout HD inspired proximity warning, shows you how far ahead you are of the ship behind you. If in last place, instead shows how close you are to the ship in the next position.
27. Position Counter Colors: The position counter will now change color based on your position in a race; Red for last place, bronze for third place, silver for second place, and gold for first place.
28. Zone colors for Survival/Upsurge HUD elements: Users can choose to have the colors of the current zone apply to parts of the HUD, adding some visual flair without significant decreases in HUD readability (only tested with default survival/virtual palettes\).
29. Extra Weapon Information: Users can choose to have additional information displayed next to the weapon icon for certain pickups. For Hellstorm, the total number of unique locked-on targets will be displayed. For Missile, the lock-on "signal integrity" will be displayed, indicating how close you are to losing the lock. For Autopilot, the amount of time before autopilot attempts to disengage will be displayed, followed by the amount of time before autopilot is forcibly disengaged. For Energy Wall, the side of the track on which the energy wall will be deployed will be displayed.
30. Cannon Firerate Override: Users can choose to reduce their ship's firepower (weapon effectiveness, i.e. how damaging the ship's weapons are\) in exchange for faster-firing cannons, or to lower their ship's cannon firerate in exchange for greater firepower.
31. Pitlane Indicator Position: Users can choose to display the pitlane indicator at the middle (Default), top, lower middle, or bottom of the screen.
32. Recharge Sum Position: Users can choose to display the recharge sum readout at the middle (Default), lower middle, or bottom of the screen.
33. Hyperthrust Bar Visibility: Users can choose to have the hyperthrust bar hide itself when inactive.
34. Speed Pad Counter Visibility: Users can choose to have the speed pad counter hide itself when inactive.
35. Speed Pad Timer Visibility: Users can choose to have the speed pad timer hide itself when inactive.
36. Overtake Radar Visibility: Users can choose to have the overtake radar timer hide itself when inactive.
37. Force No Tilt Lock in 2159: Users can choose to convert the entire track surface to no tilt lock for a different handling experience/self-imposed handicap.
38. Barrel Roll Keybind: Users can choose to bind a keyboard key or gamepad button to single-button barrel rolls using this mapping.
39. Sideshift Left Keybind: Users can choose to bind a keyboard key or gamepad button to single-button leftward sideshifts using this mapping.
40. Sideshift Right Keybind: Users can choose to bind a keyboard key or gamepad button to single-button rightward sideshifts using this mapping.
41. Smartshift Keybind: Users can choose to bind a keyboard key or gamepad button to single-button, context-sensitive sideshifts using this mapping. Sideshifts inputted this way will be executed in the direction in which the user last inputted a steer.
42. Previous Song Keybind: Users can choose to bind a keyboard key or gamepad button to skip to the previous song in the in-game music playlist using this mapping.
43. Name Tag And Shield Bar Visibility Toggle Keybind: Users can choose to bind a keyboard key or gamepad button to toggle the visibility of the name tag and shield bar ship overlays using this mapping.
44. Visibility Toggle Affects Recharge Sum: Users can choose to have the name tag and shield bar visibility toggle keybind affect the recharge sum readout as well.
45. Self-Destruct Keybind: Users can choose to bind a keyboard key or gamepad button to execute a self-destruct.
46. Self Destruct Timer: Users can choose to set the time for which the self-destruct button must be held before the self-destruct triggers. Leave this set to 0 for instant self-destruct on keypress or buttonpress.
