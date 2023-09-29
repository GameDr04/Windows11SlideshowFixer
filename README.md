# Windows 11 Slideshow Fixer
The Windows 11 Slideshow Wallpaper functionality is broken.

When switching between Virtual Desktops on Windows (or sometimes at-random and unprovoked), the slideshow wallpaper reverts back to the alphabetically first image in the slideshow.

This is my fix for the annoyance.

## Usage
You must first setup a Windows Slideshow.
* `Settings → Personalization → Background`
* Choose your images.
* This sets up the files Windows needs to run the slideshow. Without it, you'll ether end up with the stock Windows wallpaper or a black wallpaper.

You have two download options:
* download the latest release from the Releases section
* clone the project, open the solution in Visual Studio, and compile it

Run it however you'd like.

I prefer to run it as a Scheduled Task a few seconds after logon of my user. This guarantees that for as long as I'm logged in, the Slideshow won't break.

Running it manually after the Slideshow breaks is also an option.

## How it works
Windows stores Wallpaper information for each Virtual Desktop in the Registry (one key for each desktop) at
```
HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops
```
I haven't identified *why* this happens, but when the slideshow breaks, a String named `Wallpaper` is created in each Virtual Desktop Key, and it contains the full path to the alphabetically first image in the slideshow.

As long as those Strings are present, the Slideshow will not work.

My program starts by finding all Virtual Desktops and deleting their `Wallpaper` String values.

It then re-configures the Windows Slideshow according to my personal settings (10 minute intervals, `fit image` instead of `tile`, shuffle) by setting six values in the Registry:
```
• HKCU\Control Panel\Desktop\WallpaperStyle → 10
• HKCU\Control Panel\Desktop\TileWallpaper → 0

• HKCU\Control Panel\Personalization\Desktop Slideshow\LastTickHigh → 0
• HKCU\Control Panel\Personalization\Desktop Slideshow\LastTickLow → 0
• HKCU\Control Panel\Personalization\Desktop Slideshow\Interval → 600000
• HKCU\Control Panel\Personalization\Desktop Slideshow\Shuffle → 1
```

Then it calls `SystemParametersInfo` to "commit" the changes to the system and inform Explorer of what it should render.

You may notice that on line 23, I call `SystemParametersInfo` with an empty string as the wallpaper. For some reason, this is necessary to make things work. I wish to the Lords of Kobol that I knew why. Alas, I know not.