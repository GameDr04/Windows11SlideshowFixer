using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Windows11SlideshowFixer
{
    internal class Program
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 1;
        private const int SPIF_SENDCHANGE = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            Program p = new Program();
            p.DeleteStupidKeys();
            p.ReinstateSlideshow();

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, @"", SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, @"%APPDATA%\Microsoft\Windows\Themes\TranscodedWallpaper", SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        [SupportedOSPlatform("windows")]
        private void DeleteStupidKeys()
        {
            String registryRootPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\";
            RegistryKey? root = Registry.CurrentUser.OpenSubKey(registryRootPath);

            if (root == null)
            {
                Console.WriteLine("nope"); 
                return;
            }

            foreach (String desktopID in root.GetSubKeyNames())
            {
                RegistryKey? desktopProperties = Registry.CurrentUser.OpenSubKey(registryRootPath + desktopID, true);
                if (desktopProperties == null)
                {
                    Console.WriteLine("nope");
                    return;
                }
                foreach (String value in desktopProperties.GetValueNames())
                {
                    if (value == "Wallpaper")
                        desktopProperties.DeleteValue(value);
                }
            }

            root.Close();
        }

        [SupportedOSPlatform("windows")]
        private void ReinstateSlideshow()
        {
            RegistryKey? root = Registry.CurrentUser.OpenSubKey(@"Control Panel\Personalization\Desktop Slideshow", true);
            if(root == null)
            {
                Console.WriteLine("nope");
                return;
            }

            root.SetValue(@"LastTickHigh", 0);
            root.SetValue(@"LastTickLow", 0);

            root.SetValue(@"Interval", 600000);
            root.SetValue(@"Shuffle", 1);


            root = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (root == null)
            {
                Console.WriteLine("nope");
                return;
            }
            root.SetValue(@"WallpaperStyle", "10");
            root.SetValue(@"TileWallpaper", "0");

            root.Close();

        }
    }
}
// Wallpaper  |  X:\Pictures\Wallpapers\shingeki-no-kyojin-shingeki-no-kyojin-attack-on-titan-35018472-3840-1080.jpg