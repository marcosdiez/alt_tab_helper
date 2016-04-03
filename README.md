# Alt Tab Helper
An Open Source App Switcher based on VistaSwitcher


This program, based on VistaSwitcher ( http://www.ntwind.com/software/vistaswitcher.html ), improves switching tasks on Windows.

Every time one pushes Alt + ` [the key above TAB and left to 1, usually  apostrophe], it switches to another window of the same app.

One can use it to switch command prompts, terminal emulators, folders in windows explorer, browser windows, etc...

If one pushes Alt + Shift + `, the same will happen, but in the reverse order.

If you use the right tab and the focused window is Maximized,
it will bring to front all the non maximized windows in the same desktop.

Many thanks to http://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C and http://www.tcx.be/blog/2006/list-open-windows/ ,
from which most of the code was based.

This program was developed and tested on Visual Studio 2015 with Windows 10. Please email me marcos AT unitron DOT com DOT br if it does not work on your platform.

Due to techinicallities on how Microsoft Windows works, one can only switch focus of other windows while not being on focus if under running under a debugger.

Therefore, whenever the app is normally launched it will actually pretent to be a debugger relaunch itself in debug mode.
This is a limitation on Windows and I could not think of any other way to override it.

More info can be found here: http://www.codeproject.com/Tips/76427/How-to-bring-window-to-top-with-SetForegroundWindo
