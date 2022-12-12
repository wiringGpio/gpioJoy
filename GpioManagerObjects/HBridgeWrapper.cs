using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wiringGpioExtensions;


namespace GpioManagerObjects
{
    /// <summary>
    /// HBridge Wrapper
    /// </summary>
    public class HBridgeWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HBridgeWrapper( GpioPinWrapper pwmPin, GpioPinWrapper polarityPositivePin, GpioPinWrapper polarityNegativePin, string name)
        {
            Direction = 0;

            Name = name;
            PwmPin = pwmPin;
            PolarityPositivePin = polarityPositivePin;
            PolarityNegativePin = polarityNegativePin;
        }
        
        public List<GpioPinWrapper> Pins { get { return new List<GpioPinWrapper>() { PwmPin, PolarityPositivePin, PolarityNegativePin }; } }

        protected int Direction { get; set; }

        public GpioPinWrapper PwmPin { get; protected set; }
        public GpioPinWrapper PolarityPositivePin { get; protected set; }
        public GpioPinWrapper PolarityNegativePin { get; protected set; }
        public string Name { get; protected set; }


        /// <summary>
        /// Set HBridge Value
        /// </summary>
        /// <param name="direction">1,0,-1 for the direction.  1 will enable positive polarity, -1 for negative polarity at the PWM pin, 0 turns off</param>
        /// <param name="value"></param>
        public void SetHBridgeValue(int direction, double value)
        {
            //  check to change direction
            if ( Direction != direction )
            {
                //  change polarity
                switch ( direction )
                {
                    case 1:
                        PolarityPositivePin.Write(1);
                        PolarityNegativePin.Write(0);
                        break;
                    case -1:
                        PolarityPositivePin.Write(0);
                        PolarityNegativePin.Write(1);
                        break;

                    case 0:
                        PolarityPositivePin.Write(0);
                        PolarityNegativePin.Write(0);
                        break;

                    default:
                        throw new ArgumentException();
                       
                }

                Direction = direction;
            }

            if (direction == 0)
                PwmPin.PwmSetValue(0);
            else
                PwmPin.PwmSetValue(Math.Abs(value));
        }
    }
}
