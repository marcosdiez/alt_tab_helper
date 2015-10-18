# alt_tab_helper
An Open Source App Switcher based on VistaSwitcher


This program, based on VistaSwitcher ( http://www.ntwind.com/software/vistaswitcher.html ), improves switching tasks on Windows.

Every time one pushes Alt + ` [the key above TAB and left to 1, usually  apostrophe], it switches to another window of the same app.

One can use it to switch command prompts, terminal emulators, folders in windows explorer, browser windows, etc...

If one pushes Alt + Shift + `, than the reverse order will be used.

Many thanks to http://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C and http://www.tcx.be/blog/2006/list-open-windows/ ,
from which most of the code was based.

This program was developed and tested on Visual Studio 2015 with Windows 10. Please email me marcos AT unitron DOT com DOT br if it does not work on your platform.

Due to techinicallities on how Microsoft Windows works, one can only switch focus of other windows while not being on focus if under running under a debugger.
Therefore is this app is not launched via a debugger, it will relaunch itself via a debugger.

More info can be found here: http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo 
