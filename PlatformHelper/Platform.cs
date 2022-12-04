using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


///  Platform Helper
///  Code borrowed from MonoBrick solution to deal with platform specific mechanics
///  extracted to its own namespace to share with other assemblies in this solution
/// 


namespace PlatformHelper
{
    /// <summary>
    /// Platform enumeration
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Windows.
        /// </summary>
        Windows,

        /// <summary>
        /// Linux.
        /// </summary>
        Linux,

        /// <summary>
        /// Mac OS.
        /// </summary>
        Mac
    }

    /// <summary>
    /// PlatformHelper class
    /// </summary>
    public class PlatformHelper
    {
        /// <summary>
        /// Get the operating system
        /// </summary>
        /// <returns>The platform.</returns>
        public static Platform RunningPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return Platform.Mac;
                    else
                        return Platform.Linux;

                case PlatformID.MacOSX:
                    return Platform.Mac;

                default:
                    return Platform.Windows;
            }
        }
    }
}
