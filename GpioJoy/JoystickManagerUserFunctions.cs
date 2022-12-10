using GpioManagerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wiringGpioExtensions;

namespace GpioJoy
{
    public partial class JoystickManager
    {
        /// <summary>
        /// Go to the 'home' configuration page (first one loaded)
        /// </summary>
        public void ConfigurationPageGoHome(bool input)
        {
            if (input && Environment.TickCount - GoHomeChangeTick > 1000)
            {
                HomeConfiguration();
                OnStateChanged?.Invoke(this, new EventArgs());
                GoHomeChangeTick= Environment.TickCount;
            }
        }
        int GoHomeChangeTick = 0;


        /// <summary>
        /// Go forward one configuration page
        /// </summary>
        public void ConfigurationPageGoBack(bool input)
        {
            if (input && Environment.TickCount - GoBackChangeTick > 1000)
            {
                ToggleConfiguration(-1);
                OnStateChanged?.Invoke(this, new EventArgs());
                GoBackChangeTick= Environment.TickCount;
            }
        }
        int GoBackChangeTick = 0;



        /// <summary>
        /// Go back one configuration page
        /// </summary>
        public void ConfigurationPageGoForward(bool input)
        {
            if (input && Environment.TickCount - GoForwardChangeTick > 1000)
            {
                ToggleConfiguration(1);
                OnStateChanged?.Invoke(this, new EventArgs());
                GoForwardChangeTick= Environment.TickCount;
            }
        }
        int GoForwardChangeTick = 0;
    }
}
