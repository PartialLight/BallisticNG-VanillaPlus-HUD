MD5: 1B5F20902DF8D7555BAD4F918DA69749
___________________________________________________________________________________________________________


INSTALLATION:

Extract the files from the provided zip.
After extracting the files from the zip, place the VanillaPlus folder into BallisticNG\User\Mods\Code Mods\

Your final folder structure should look like this:

BallisticNG\User\Mods\Code Mods\VanillaPlus\

Weapons
config.ini
VanillaPlus.dll
VanillaPlus.ini
VanillaPlusHUD
VanillaPlusHUD.cs
vanillaplushud.hud
VanillaPlusMenu.cs
___________________________________________________________________________________________________________


FAST SETUP:
Open VanillaPlus.ini in a text editor, like Notepad.

Make sure 'Always Recompile' is set = to 0.
Set 'Ignore Assembly' = to 0.
Save the file and close it.
Launch BallisticNG.
The mod will already be enabled, skipping the re-launch required with the in-game setup method.
___________________________________________________________________________________________________________


IN-GAME SETUP:
Launch BallisticNG
From the main menu, go to: 

modding>code mods>vanillaplus

Set 'enabled' to enabled, leave 'Always Recompile' disabled.
Re-launch BallisticNG.
The mod will now be enabled.
___________________________________________________________________________________________________________


ABOUT 'Always Recompile'
The provided zip includes the C# script files which the game compiled into the included VanillaPlus.dll file, which is what actually gets loaded as a code mod.
Consequently, enabling Always Recompile will work, but it may end up changing the MD5 hash of the .dll file (included at the top of this README) as a result of the recompilation.
The 'Allow Matching' lobby config option used for BallisticN/v/ lobbies checks clients' mod .dll file MD5 hashes and compares them against the MD5 hashes of the host's mod .dll file.
If these hashes don't match and 'Allow Matching' is being used, the game will not let you connect to the host.
The in-game debug console (opened with CTRL+BACKSPACE) will show you an error message and report your file's MD5 hash.

If this happens, simply delete the VanillaPlus folder from your codemods folder and repeat the installation process.

If you want to check your file hash easily (no command line required), my suggestion would be to install TC4Shell (I am not affiliated with or sponsored by TC4Shell in any way).
TC4Shell is a shareware model file archiver software, similar to WinRAR or 7-Zip.
It's designed to integrate seamlessly with Windows Explorer, which it does.
Aside from compressing and decompressing file archives, TC4Shell also provides a 'Hash' tab in the file properties dialog.

This means you can right click any(?) file, select 'Properties', and go to the 'Hash' tab to see a variety of hash values for that file, including the MD5 hash.

Other software like 7-Zip may have similar functionalities, but I think TC4Shell provides the most seamless, easy way to view file hashes.
It's what I personally use and recommend, as I think it's a very good piece of software.
___________________________________________________________________________________________________________


If you have any feature/setting requests/suggestions or bug reports, post them in the /vm/ thread and/or tag me in the Steam group chat, or submit an issue on the github page.
You can also add me on Steam if you want to message me directly.
___________________________________________________________________________________________________________