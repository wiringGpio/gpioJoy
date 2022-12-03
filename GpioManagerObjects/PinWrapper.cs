using wiringGpioExtensions;
using PlatformHelper;
using static PlatformHelper.PlatformHelper;

namespace GpioManagerObjects
{
    /// <summary>
    /// GpioPinWrapper
    /// Class to wrap up a wiringPi GPIO Pin
    /// includes dummy implementation for windows compile - ( define _WINDOWS )
    /// </summary>
    public class GpioPinWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pinNumber">physical pin number</param>
        public GpioPinWrapper(int pinNumber, string name, bool hwPwm = false)
        {
            //  set pin number
            PinNumber = pinNumber;

            //  set name
            Name = name;

            //  init PWM parameters
            PwmRunning = false;
            if (pinNumber == 12)
                PwmRange = 1024;
            else
                PwmRange = 200;

            PwmValue = 0;
            HardwarePwm = hwPwm;

            if (RunningPlatform() == Platform.Windows)
            {   
                //  using the dummy object
                Mode = PinMode.Input;
                State = PinValue.Low;
            }
        }


        //  Properties
        //

        //  Pin Number
        public int PinNumber { get; protected set; }

        public string Name { get; protected set; }
        //  Pin Mode
        public PinMode _Mode { get; set; }

     
        //  PWM Parameters
        public bool HardwarePwm { get; protected set; }
        public bool PwmRunning { get; protected set; }
        public bool PwmStarted { get; protected set; }
        public int PwmRange { get; protected set; }
        public int PwmValue { get; protected set; }
        public double PwmScale { get; protected set; }


        /// <summary>
        /// Write to pin - digitalWrite
        /// </summary>
        /// <param name="value">1 or 0</param>
        public void Write(int value)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                GPIO.DigitalWrite(PinNumber, (PinValue)value);

                //  Raspberry Pi hardware PWM
                if (PinNumber == 12 && Mode == PinMode.PWMOutput)
                    GPIO.PwmWrite(PinNumber, 0);
            }
            else
            {
                FakeWrite(value);
            }
        }

        
        /// <summary>
        /// Read - digitalRead
        /// </summary>
        /// <returns>1 or 0 value read</returns>
        public virtual int Read()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                return GPIO.DigitalRead(PinNumber);
            }
            else
            {
                return FakeRead();
            }
        }


        /// <summary>
        /// Digital Read
        /// get raw digital read value, will be different than the regular 1/0 response when this is a PCA pin
        /// </summary>
        /// <returns></returns>
        public int DigitalRead()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                return GPIO.DigitalRead(PinNumber);
            }
            else
            {
                return FakeRead();
            }
        }


        /// <summary>
        /// Pin Mode
        /// Calling set will change pin mode in wiringPi, including start/stop PWM
        /// </summary>
        public PinMode Mode
        {
            get
            {
                return _Mode;
            }

            set
            {
                if (_Mode == value)
                    return;

                if (RunningPlatform() == Platform.Linux)
                {
                    if (_Mode == PinMode.PWMOutput && PwmRunning)
                    {
                        PwmStop();
                    }

                    GPIO.PinMode(PinNumber, (PinMode)value);
                }

                _Mode = value;
            }
        }

        

        /// <summary>
        /// Start PWM
        /// </summary>
        /// <param name="value">initial pwm value from 0 to Range</param>
        /// <param name="range">pwm frequency (hardware) or range (software)</param>
        /// <returns>return 0 for hardware PWM, or value from softPwmCreate</returns>
        public int PwmStart(int value, int range)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                int ret = 0;
                //  set pwm depending on pin type
                if (HardwarePwm)
                {
                    //  set pin to off
                    GPIO.DigitalWrite(PinNumber, 0);

                    if (PinNumber == 12 && Mode == PinMode.PWMOutput)
                    {
                        //  set the clock to give 50 Hz for their range - TODO this is hard coded for 50 Hz (if this calculation is correct).  Need to understand more about PWM
                        //  http://raspberrypi.stackexchange.com/questions/4906/control-hardware-pwm-frequency
                        int clock = (int)(Constants.RPiPwmClockSpeed / (range * Constants.RPiPwmFrequency));

                        //  Set PWM clock to mark space mode
                        GPIO.PwmSetMode(0);
                        GPIO.PwmSetClock(clock);
                        GPIO.PwmSetRange(range);

                        //  Start at off
                        GPIO.PwmWrite(PinNumber, 0);
                    }
                }
                else
                {
                    //  this is all soft pwm
                    if (PwmStarted)
                    {
                        PwmStop();
                    }

                    ret = SoftwarePwm.SoftPwmCreate(PinNumber, value, range);
                    if (ret == 0)
                    {
                        _Mode = PinMode.PWMOutput;
                    }
                }

                PwmRange = range;
                PwmValue = value;
                PwmRunning = true;
                PwmStarted = true;
                return ret;
            }
            else
            {
                return FakePwmStart(value, range);
            }
        }


        /// <summary>
        /// Stop PWM
        /// </summary>
        public void PwmStop()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                if (_Mode != PinMode.PWMOutput)
                    return;

                //  stop pwm depending on pin type
                if (HardwarePwm)
                {
                    GPIO.DigitalWrite(PinNumber, 0);
                    if (PinNumber == 12 && Mode == PinMode.PWMOutput)
                        GPIO.PwmWrite(PinNumber, 0);
                }
                else
                {
                    SoftwarePwm.SoftPwmStop(PinNumber);
                    GPIO.PinMode(PinNumber, (int)PinMode.Input);
                    _Mode = PinMode.Input;
                }

                PwmRunning = false;
                PwmStarted = false;
                PwmValue = 0;
            }
            else
            {
                FakePwmStop();
            }
        }


        /// <summary>
        /// Pause PWM
        /// pause pwm so you can resume at previous values
        /// </summary>
        public void PwmPause()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                if (HardwarePwm)
                {
                    GPIO.DigitalWrite(PinNumber, 0);
                    if (PinNumber == 12 && Mode == PinMode.PWMOutput)
                        GPIO.PwmWrite(PinNumber, 0);
                }
                else
                {
                    SoftwarePwm.SoftPwmStop(PinNumber);
                }

                PwmRunning = false;
            }
            else
            {
                FakePwmPause();
            }
        }


        /// <summary>
        /// Resume PWM
        /// resume pwm at previous values
        /// </summary>
        /// <returns></returns>
        public int PwmResume()
        {
            return PwmStart(PwmValue, PwmRange);
        }


        /// <summary>
        /// Set PWM value with unit vector
        /// </summary>
        /// <param name="value">unit vector between 0 and 1.0, used to multiply the range</param>
        public void PwmSetValue(double value)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                if (_Mode != PinMode.PWMOutput)
                    return;

                int pwmValue = (int)(value * PwmRange);

                PwmSetValue(pwmValue);
            }
            else
            {
                FakePwmSetValue(value);
            }
        }


        /// <summary>
        /// Set PWM value with integer in range
        /// </summary>
        /// <param name="value"> integer value between 0 and Range</param>
        public void PwmSetValue(int value)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                //  set pwm value depending on pin type
                if (HardwarePwm)
                {
                    GPIO.PwmWrite(PinNumber, value);
                }
                else
                {
                    SoftwarePwm.SoftPwmWrite(PinNumber, value);
                }

                PwmValue = value;
            }
            else
            {
                FakePwmSetValue(value);
            }
        }




        // Fake Pins for Windows BUild
        //

        //  Windows build, use dummy functions
        public void FakeWrite(int value)
        {
            if (value == 1)
                State = PinValue.High;
            else
                State = PinValue.Low;
        }

        //  Read
        public virtual int FakeRead()
        {
            return State == PinValue.High ? 1 : 0;
        }

        public PinValue State;
        
         public PinMode FakeMode
        {
            get
            {
                return _Mode;
            }

            set
            {
                _Mode = value;
            }
        }

        //  Start PWM
        public int FakePwmStart(int value, int range)
        {
            _Mode = PinMode.PWMOutput;
            PwmStarted = true;
            PwmRunning = true;
            PwmRange = range;
            PwmValue = value;
            return 0;   //  zero is success
        }

        //  Stop PWM
        public void FakePwmStop()
        {
            PwmValue = 0;
           
            PwmStarted = false;
            PwmRunning = false;
            _Mode = PinMode.Input;
        }

        public void FakePwmPause()
        {
            
            PwmRunning = false;
        }

        public int FakePwmResume()
        {
            return PwmStart(PwmValue, PwmRange);
        }

        //  Set PWM value
        public void FakePwmSetValue(double value)
        {
            if (_Mode != PinMode.PWMOutput)
                return;
            PwmValue = (int)value;
        }

      public void FakePwmSetValue(int value)
        {
            if (_Mode != PinMode.PWMOutput)
                return;
            PwmValue = (int)value;
        }


    }

    /// <summary>
    /// Pin Wrapper for PCA
    /// PCA pins behave differently on DigitalRead
    /// </summary>
    public class GpioPinWrapperPca : GpioPinWrapper
    {
        public GpioPinWrapperPca(int pinNumber, string name)
            : base(pinNumber, name, true)
        {
            PwmRange = 4096;
        }
         
        /// <summary>
        /// Read
        /// override read for PCA to return 1/0 for on/off state
        /// </summary>
        /// <returns>0 if the full off bit is set, otherwise 1</returns>
        public override int Read()
        {
            int read = GPIO.DigitalRead(PinNumber);

            return ( (read & 0x1000) == 4096) ? 0 : 1;
        }
    }

}
