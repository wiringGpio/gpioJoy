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

        //  function to toggle the configuration to the next one, used by all configurations
        public void ConfigurationPageGoHome(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input )
            {
                HomeConfiguration();
                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public void ConfigurationPageGoBack(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input )
            {
                ToggleConfiguration(-1);
                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public void ConfigurationPageGoForward(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input )
            {
                ToggleConfiguration(1);
                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }



     



    }
}
