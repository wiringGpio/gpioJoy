using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpioManagerObjects
{
    /// <summary>
    /// Servo Motor Wrapper
    /// </summary>
    public class ServoWrapper
    {
        public ServoWrapper(GpioPinWrapper pin, int range, double frequency)
        {
            Pin = pin;
            Range = range;
            Frequency = frequency;

            //  calculate the center and max/min ticks, function of frequency and range
            double cycleMs = 1000.0 / frequency;

            //  center tick at 1.5 miliseconds
            double pulseMs = 1.5;
            //  convert to int in the range
            CenterTick = (int)(range * pulseMs / cycleMs + 0.5f);

            //  max tick is 2.0 ms pulse
            pulseMs = 2.0;
            MaxTick = (int)(range * pulseMs / cycleMs + 0.5f);

            //  max tick is 1.0 ms pulse
            pulseMs = 1.0;
            MinTick = (int)(range * pulseMs / cycleMs + 0.5f);
        }


        public GpioPinWrapper Pin { get; protected set; }
        public int Range { get; protected set; }
        public int CenterTick { get; protected set; }
        public int MaxTick { get; protected set; }
        public int MinTick { get; protected set; }
        public double Frequency { get; protected set; }

        public double Center()
        {
            //  calculate the center tick, servo wants 1.5 milisecond pulse for stationary position
            
            double cycleMs = 1000.0 / Frequency;
            double pulseMs = 1.5;
            //  initial tick for this pwm range
            int initialTick = (int)(Pin.PwmRange * pulseMs / cycleMs + 0.5f);
            //  turn this back into a unit vector with the range
            double initialValue = (double)initialTick / Pin.PwmRange;

            //  set the servo at the center tick
            Pin.PwmSetValue(initialValue);

            return initialValue;
        }

        public double SetPulse(double milliseconds)
        {
            double cycleMs = 1000.0 / Frequency;
           
            //  initial tick for this pwm range
            int initialTick = (int)(Pin.PwmRange * milliseconds / cycleMs + 0.5f);
            //  turn this back into a unit vector with the range
            double initialValue = (double)initialTick / Pin.PwmRange;

            //  set the servo at the center tick
            Pin.PwmSetValue(initialValue);

            return initialValue;
        }
    }
}
