using PlatformHelper;
using static PlatformHelper.PlatformHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wiringGpioExtensions;

namespace GpioManagerObjects
{
    public class SevenSegDisplayWrapper
    {
        public SevenSegDisplayWrapper(int displayIndex, List<GpioPinWrapper> pins)
        {
            Pins = pins;
            DisplayIndex = displayIndex;
        }

        public void Set(string display)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                SevenSegDisplay.Set(DisplayIndex, display);
            }
        }

        public void Off()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                SevenSegDisplay.Off(DisplayIndex);
            }
        }

        public int DisplayIndex { get; protected set; }
        public List<GpioPinWrapper> Pins { get; protected set; }
    }
}
