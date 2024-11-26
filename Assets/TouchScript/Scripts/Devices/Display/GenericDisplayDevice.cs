/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System.Text.RegularExpressions;
using UnityEngine;

namespace TouchScript.Devices.Display
{
    /// <summary>
    /// Display device which tries to guess current DPI if it's not set by platform.
    /// </summary>
    public class GenericDisplayDevice : DisplayDevice
    {
        internal static bool INTERNAL_IsLaptop
        {
            get
            {
                if (isLaptop == null)
                {
                    var gpuName = SystemInfo.graphicsDeviceName.ToLower();
                    var regex = new Regex(@"^(.*mobile.*|intel hd graphics.*|.*m\s*(series)?\s*(opengl engine)?)$", RegexOptions.IgnoreCase);
                    isLaptop = regex.IsMatch(gpuName);
                }
                return isLaptop == true;
            }
        }

        private static bool? isLaptop = null;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            Name = Application.platform.ToString();
            if (INTERNAL_IsLaptop) Name += " (Laptop)";

            dpi = Screen.dpi;
            if (dpi < float.Epsilon)
            {
                // Calculations based on http://en.wikipedia.org/wiki/List_of_displays_by_pixel_density and probability
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.LinuxPlayer:
                        {
                            var width = Mathf.Max(Screen.currentResolution.width, Screen.currentResolution.height);
                            var height = Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height);

                            if (width >= 3840)
                            {
                                dpi = (height <= 2160) ? 150 : 200;
                            }
                            else if (width >= 2880 && height == 1800)
                            {
                                dpi = 220; // 15" retina
                            }
                            else if (width >= 2560)
                            {
                                dpi = (height >= 1600)
                                    ? (INTERNAL_IsLaptop ? 226 : 101) // 13.3" retina or 30" display
                                    : 109; // 27" iMac
                            }
                            else if (width >= 2048)
                            {
                                dpi = (height <= 1152) ? 100 : 171; // 23-27" or 15" laptop
                            }
                            else if (width >= 1920)
                            {
                                dpi = (height >= 1440) ? 110 :
                                      (height >= 1200) ? 90 :
                                      (height >= 1080) ? (INTERNAL_IsLaptop ? 130 : 92) : dpi;
                            }
                            else if (width >= 1680)
                            {
                                dpi = 129; // 15" laptop
                            }
                            else if (width >= 1600)
                            {
                                dpi = 140; // 13" laptop
                            }
                            else if (width >= 1440)
                            {
                                dpi = (height >= 1050) ? 125 : 110; // 14" laptop or 13" air/macbook pro
                            }
                            else if (width >= 1366)
                            {
                                dpi = 125; // 10"-14" laptops
                            }
                            else if (width >= 1280)
                            {
                                dpi = 110;
                            }
                            else
                            {
                                dpi = 96;
                            }
                            break;
                        }
                    case RuntimePlatform.Android:
                        {
                            var width = Mathf.Max(Screen.currentResolution.width, Screen.currentResolution.height);
                            var height = Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height);
                            if (width >= 1280)
                            {
                                dpi = (height >= 800) ? 285 : 312; // Galaxy Note or Galaxy S3
                            }
                            else if (width >= 1024)
                            {
                                dpi = 171; // Galaxy Tab
                            }
                            else if (width >= 960)
                            {
                                dpi = 256; // Sensation
                            }
                            else if (width >= 800)
                            {
                                dpi = 240; // Galaxy S2
                            }
                            else
                            {
                                dpi = 160;
                            }
                            break;
                        }
                    case RuntimePlatform.IPhonePlayer:
                        {
                            var width = Mathf.Max(Screen.currentResolution.width, Screen.currentResolution.height);
                            if (width >= 2048)
                            {
                                dpi = 290; // iPad4 or iPad Mini 2
                            }
                            else if (width >= 1136)
                            {
                                dpi = 326; // iPhone 5+
                            }
                            else if (width >= 1024)
                            {
                                dpi = 160; // iPad Mini 1
                            }
                            else if (width >= 960)
                            {
                                dpi = 326; // iPhone 4+
                            }
                            else
                            {
                                dpi = 160;
                            }
                            break;
                        }
                    case RuntimePlatform.WSAPlayerARM:
                    case RuntimePlatform.WSAPlayerX64:
                    case RuntimePlatform.WSAPlayerX86:
                        dpi = 160;
                        break;
                    case RuntimePlatform.WP8Player:
                        dpi = 160;
                        break;
                }
            }
        }
    }
}
