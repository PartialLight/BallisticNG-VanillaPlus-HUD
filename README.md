# BallisticNG-VanillaPlus-HUD

A Custom HUD Code Mod for BallisticNG with a similar layout to the internal HUD

### Acknowledgements

Special thanks to Dekaid for sharing his rear view mirror implementation and elaborating on GT3HUD's design and functionality, Vonsnake for general help with modding/code examples/updating the options API/documentation and related questions, and Dinir N. for friendly correspondence and potential future collaboration.  

Check out Dekaid's GT3HUD and Dinir's Streamliner HUD (they're both much more professionally put-together than my own mod), and buy BallisticNG and its DLCs if you haven't already.

### Supported Modes

- [Single\] Race
- Tournament (Which is just a series of [Single\] Races\)

### Supported Versions
- 1.4.0.1
- 1.4.1-d15 as of 9/28/2025

### Features

1. Vanilla HUD Alignment and Positioning: HUD elements are positioned as closely as possible to their internal counterparts where applicable; Consequently, the internal Music Display and Pitlane Indicator can be used instead of the custom ones. Thrust Bar and Throttle Bar positions have been swapped.
2. Vanilla Weapon Icon Symbology with unique colours and alternative designs: Weapon icons use coloured versions of the vanilla weapon sprites. Missile has 1 alternative icon, Rockets have 5. Because weapon sprites are loaded and registered on game start, changes to icon preference require game restart to take effect.
3. Rear View Mirror: Switches to forward camera when holding the look behind input. Can be enabled/disabled per physics mode (2159, 2280, Floorhugger\).
4. Alternative Readouts: Ship speed can be displayed in terms of Kilometres/Miles Per Hour or in terms of Engine Force units. Ship Energy can be displayed to 5 digits of decimal precision. Can be configured to show fewer/zero decimal places.
5. Damage Flasher: A small red dot that flashes whenever you are slowed down by or take damage from any source (except afterburner usage\).
6. Relative Time Readout: Your time behind first place, or your time ahead of second place if you're in first place. Updates continuously as you pass over individual track sections. Changes colour to blue when out of tremor range of second place.
7. Hyperthrust Bar: Bar that fills up and decays based on afterburner usage, with accompanying Hyperthrust Force readout. Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout.
8. Speed Pad Counter: Keeps track of how many speed pads have been hit, with accompanying speed pad acceleration boost readout (+4/+8/+12 in terms of Engine Force units for 2159\). While functional in 2280, information conveyed is less inherently useful due to the changes 2280 makes to speed pad function. Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout.
9. Speed Pad Timer: Bar that fills up and decays based on remaining speed bad boost timer, with accompanying readout. While functional in 2280, information conveyed is less inherently useful due to the changes 2280 makes to speed pad function. Displayed next to rear view mirror by default, can be lowered to be displayed below the Relative Time Readout.
10. Speed Pad Colour Pulse: The Throttle Bar (Yellow\) will pulse from dark blue to light blue when you pass over a speed pad.
11. Energy Bar Colour Breakpoints: Energy Bar will cycle through colours based on damage breakpoints informed by worst-case and best-case damage received from weapons (Calculated from standard-roster ship shield and firepower stats, prototypes like the NX2000 excluded.\)
12. Canopy Camera Adjust for 2280: Users can choose to raise the position of the internal camera to be more similar to 2159 in 2280.
13. Cockpit Camera Adjust for 2280: Users can choose to raise the position of the cockpit camera to be more similar to 2159 in 2280.
14. Cockpit Mesh Adjust: Users can choose whether to have the cockpit interior or the ship's nose/forward hull visible when using the cockpit camera in any physics mode.
15. (EXPERIMENTAL\) 2280 Tilt Lock: Users can choose to have the camera's tilt lock to world up (similar to 2159 camera behavior\) in 2280 except when on maglock or no-tilt-lock surfaces.
