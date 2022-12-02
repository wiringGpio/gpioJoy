﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using wiringGpioExtensions;



namespace GpioManagerObjects
{
    public enum Mcp230xxType
    {
        Mcp23017 = 0,
        Mcp23008 = 1,
    }


    /// <summary>
    /// GPIO Events
    /// Pin state changed event
    /// </summary>
    public class GpioEventArgs : EventArgs
    {
        public GpioEventArgs(int pinNumber)
        {
            PinNumber = pinNumber;
        }

        public int PinNumber;
    }
    public delegate void GpioEventHandler(object sender, GpioEventArgs e);


    public class PcaChip
    {
        public PcaChip(int fd, float freq)
        {
            Fd = fd;
            Frequency = freq;
        }

        public int Fd;
        public double Frequency;
    }

    /// <summary>
    /// GpioManager
    /// Handles GPIO pin state for the program
    /// also wraps up pin handling in ifdef so you can compile and run on windows with dummy pin objects
    /// </summary>
    public class GpioManager
    {
        const int I2CBus = 1;

        // Public Events
        //

        /// <summary>
        /// Pin updated event
        /// </summary>
        public event GpioEventHandler GpioEvents;


        //  Public Methods
        //


        /// <summary>
        /// Setup MCP expander chip
        /// </summary>
        /// <param name="type">23017 or 23018</param>
        /// <param name="baseNumber">base number for pin indices</param>
        /// <param name="address">address on i2C bus, use decimal number (not hex)</param>
        /// <returns></returns>
        public int SetupMcp(Mcp230xxType type, int baseNumber, int address)
        {
#if !RPI
            return 1;

#else
            switch (type)
            {
                case Mcp230xxType.Mcp23008:
                    {
                        int fd = DeviceI2CExtensions.Mcp23008Setup(I2CBus, baseNumber, address);
                        for (int i = baseNumber; i < baseNumber + 8; i++)
                            McpPins[i] = fd;
                        return fd;
                    }

                case Mcp230xxType.Mcp23017:
                    {
                        int fd = DeviceI2CExtensions.Mcp23017Setup(I2CBus, baseNumber, address);
                        for (int i = baseNumber; i < baseNumber + 16; i++)
                            McpPins[i] = fd;
                        return fd;
                    }

                default:
                    return 0;
            }
#endif
        }


        /// <summary>
        /// Setup PCA expander chip
        /// </summary>
        /// <param name="baseNumber">base number for pin indices</param>
        /// <param name="address">address on i2C buss, use decimal number</param>
        /// <param name="frequency">PWM frequency </param>
        /// <returns></returns>
        public int SetupPca(int baseNumber, int address, float frequency)
        {
#if !RPI
            return 1;
#else
            int fd = DeviceI2CExtensions.Pca9685Setup(1, baseNumber, address, frequency);
            for (int i = baseNumber; i < baseNumber + 16; i++)
            {
                PcaPins[i] = new PcaChip(fd, frequency);
            }
            DeviceI2CExtensions.Pca9685PWMReset(fd);
            return fd;
#endif
        }


        /// <summary>
        /// Query if a pin is PCA chip pin
        /// </summary>
        /// <param name="pinNumber">pin number</param>
        /// <returns>true if is PCA chip pin</returns>
        public bool IsPcaPin(int pinNumber)
        {
            return PcaPins.ContainsKey(pinNumber);
        }

        
        /// <summary>
        /// Get the PWM frequency for this pin
        /// </summary>
        /// <param name="pinNumber">pin number</param>
        /// <returns>pwm frequency for this pin</returns>
        public double PinPwmFrequency(int pinNumber)
        {
            if (!Pins.ContainsKey(pinNumber))
                return 0.0;

            if (PcaPins.ContainsKey(pinNumber))
            {
                return PcaPins[pinNumber].Frequency;
            }
            else if (pinNumber == 12)
            {
                //  program is hard coded to use 50 Hz on RPi PWM pin
                return 50.0;
            }
            else
            {
                //  software PWM
                return 1.0 / (Pins[pinNumber].PwmRange * 0.000100);
            }
        }


        /// <summary>
        /// Initialize Pin
        /// </summary>
        /// <param name="pinNumber">Physical pin number</param>
        /// <returns>false if this pin has already been created, otherwise true</returns>
        public void AddPinToManager(GpioPinWrapper pin)
        {
            try
            {
                //  return existing pin if it exists
                GpioPinWrapper existingPin;
                if (Pins.TryGetValue(pin.PinNumber, out existingPin))
                    throw new ArgumentException();
                else
                {
                    Pins.Add(pin.PinNumber, pin);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }



        /// <summary>
        /// Create stepper driver
        /// </summary>
        /// <param name="name">name to assign to this stepper driver</param>
        /// <param name="sequenceElement">sequence xml element node</param>
        /// <param name="stepperElement">stepper xml element node</param>
        /// <returns>stepper wrapper object created</returns>
        public StepperWrapper CreateStepperDriver(XmlNode sequenceElement, XmlNode stepperElement)
        {
            int index = 0;
#if !RPI
            FakeIndex++;
            index = FakeIndex;
#else
            index =  StepperMotor.CreateFromXml(sequenceElement.OuterXml, stepperElement.OuterXml);
#endif
            //  the library created a stepper driver
            //  we need to parse the stepper element so we can create pin wrappers 
            //  required to hook to the joystick and user interace for all the pins used
            if (index >= 0)
            {
                //  get the name info
                string name = "";
                XmlNode nameNode = stepperElement.SelectSingleNode("Name");
                if (nameNode != null)
                {
                    XmlElement nameElement = (XmlElement)nameNode;
                    name = nameElement.InnerText;
                }
                else
                {
                    name = string.Format("Step {0}", index.ToString());
                }

                //  parse pins info
                XmlNode pinNode = stepperElement.SelectSingleNode("Pins");
                if (pinNode == null)
                    return null;

                //  we made a motor, now lets make pins for it
                List<int> pinIndexList = new List<int>();
                string[] pinStrings = Regex.Split(pinNode.InnerText, ",");

                try
                {
                    foreach (string nextPinString in pinStrings)
                    {
                        pinIndexList.Add(Math.Abs(Int32.Parse(nextPinString)));
                    }
                }
                catch (Exception e)
                {
                    //  invalid pins for this motor
                    return null;
                }

                //  process the polarity pins
                var polarities = stepperElement.SelectNodes("Polarity");
                foreach (var nextPolarity in polarities)
                {
                    var polarityElement = (XmlElement)nextPolarity;
                    pinStrings = Regex.Split(polarityElement.InnerText, ",");

                    try
                    {
                        foreach (string nextPinString in pinStrings)
                        {
                            pinIndexList.Add(Math.Abs(Int32.Parse(nextPinString)));
                        }
                    }
                    catch (Exception e)
                    {
                        //  invalid pins for this motor
                        return null;
                    }
                }

                //  go through the list of pins for this motor, call Distinct on the pin index list because bipolar motor will have pin X and -X
                List<GpioPinWrapper> stepperPins = new List<GpioPinWrapper>();
                foreach (int nextIndex in pinIndexList.Distinct())
                {
                    //  find this pin
                    var nextPin = GetPin(nextIndex);

                    //  these pins should exist and be created already
                    if (nextPin == null)
                        return null;

                    stepperPins.Add(nextPin);
                }

                //  create a new stepper wrapper 
                StepperWrapper newWrapper = new StepperWrapper(index, stepperPins, name);
                StepperDriversByIndex[index] = newWrapper;
                StepperDriversByName[name] = index;

                //  done
                return newWrapper;
            }

            //  error
            return null;
        }


        public StepperWrapper GetStepperDriver(string name)
        {
            StepperWrapper driver = null;

            try
            {
                int index = StepperDriversByName[name];
                driver = StepperDriversByIndex[index];
            }
            catch (Exception)
            {
   
            }

            return driver;
        }


        /// <summary>
        /// Stepper set delay
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        /// <param name="delay">delay in milliseconds</param>
        public void StepperSetDelay(int index, float delay)
        {
#if !RPI
            Console.WriteLine("Stepper SetDelay");
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                driver.SetDelay(delay);
            }
#endif
        }


        /// <summary>
        /// Stepper move number of steps
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        /// <param name="steps">number of steps to move</param>
        public void StepperStep(int index, int steps)
        {
#if !RPI
            Console.WriteLine("Stepper StepperStep");
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                driver.Step(steps);
            }
#endif
        }

        /// <summary>
        /// Stepper set speed
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        /// <param name="value">unit vector (-1.0 to 1.0) for stepper speed, delay between steps will be calculated from stepper delay min and max</param>
        public void StepperSetSpeed(int index, float value)
        {
#if !RPI
            Console.WriteLine("Stepper set speed " + value.ToString());
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                driver.SetSpeed(value);
            }
#endif
        }


        /// <summary>
        /// Stepper stop
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        public void StepperStop(int index)
        {
#if !RPI
            Console.WriteLine("Stepper Stop");
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                driver.Stop();
            }
#endif
        }

        /// <summary>
        /// Stepper reset tacho count
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        public int StepperGetTachoCount(int index)
        {
#if !RPI
            Console.WriteLine("Stepper StepperGetTachoCount");
            return 0;
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                return driver.TachoCount;
            }
            return 0;
#endif
        }

        /// <summary>
        /// Stepper reset tacho count
        /// </summary>
        /// <param name="index">index of the stepper motor</param>
        public void StepperResetTachoCount(int index)
        {
#if !RPI
            Console.WriteLine("Stepper Stop");
#else
            StepperWrapper driver = null;
            if (StepperDriversByIndex.TryGetValue(index, out driver))
            {
                driver.ResetTachoCount();
            }
#endif
        }

        /// <summary>
        /// Stepper set delay
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        /// <param name="delay">delay in milliseconds</param>
        public void StepperSetDelay(string name, float delay)
        {
#if !RPI
            Console.WriteLine("Stepper SetDelay");
#else
            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                driver.SetDelay(delay);
            }
            catch
            {
                return;
            }
#endif
        }

        /// <summary>
        /// Step a specified number of steps, this will queue so you will get all your steps in order to the end
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        /// <param name="steps">number of steps to turn</param>
        public void StepperStep(string name, int steps)
        {
#if !RPI
            Console.WriteLine("Stepper step " + steps.ToString());
#else
            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                driver.Step(steps);
            }
            catch
            {
                return;
            }
#endif
        }


        /// <summary>
        /// Stepper set speed
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        /// <param name="value">unit vector between -1.0 and 1.0 to turn between min delay and max delay</param>
        public void StepperSetSpeed(string name, float value)
        {
#if !RPI
            Console.WriteLine("Stepper set speed " + value.ToString());
#else
            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                driver.SetSpeed(value);
            }
            catch
            {
                return;
            }
#endif
        }



        /// <summary>
        /// Stepper stop
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        public void StepperStop(string name)
        {
#if !RPI
            Console.WriteLine("Stepper Stop");
#else
            
            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                driver.Stop();
            }
            catch
            {
                return;
            }
#endif
        }

        /// <summary>
        /// Stepper stop
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        public int StepperGetTachoCount(string name)
        {
#if !RPI
            Console.WriteLine("Stepper Stop");
            return 0;
#else

            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                return driver.TachoCount;
            }
            catch
            {
                return 0;
            }
#endif
        }

        /// <summary>
        /// Stepper stop
        /// </summary>
        /// <param name="name">the name of the stepper motor</param>
        public void StepperResetTachoCount(string name)
        {
#if !RPI
            Console.WriteLine("Stepper Reset tacho count");
            return;
#else
            try
            {
                int index = StepperDriversByName[name];
                var driver = StepperDriversByIndex[index];
                driver.ResetTachoCount();
            }
            catch
            {
                return;
            }
#endif
        }



        //  HBridge
        int UnnamedHBCounter;

        /// <summary>
        /// Create HBridge driver
        /// </summary>
        /// <param name="stepperElement">stepper xml element node</param>
        /// <returns>stepper wrapper object created</returns>
        public HBridgeWrapper CreateHBridgeDriver(XmlNode hbridgeElement)
        {
            //  keep track of the pins this motor needs
            List<int> polaritiesList = new List<int>();

            //  process the polarity pins
            var polarityNode = hbridgeElement.SelectSingleNode("Polarity");
            if (polarityNode == null)
                return null;

            List<int> polarityList = new List<int>();

            string[] pinStrings = Regex.Split(polarityNode.InnerText, ",");

            try
            {
                foreach (string nextPinString in pinStrings)
                {
                    polaritiesList.Add(Math.Abs(Int32.Parse(nextPinString)));
                }
            }
            catch (Exception e)
            {
                //  invalid pins for this motor
                return null;
            }


            if (polaritiesList.Count != 3)
                return null;

            GpioPinWrapper pwmPin = GetPin(polaritiesList[0]);
            GpioPinWrapper polarityPositivePin = GetPin(polaritiesList[1]);
            GpioPinWrapper polarityNegativePin = GetPin(polaritiesList[2]);

            //  get the name info
            string name = "";
            XmlNode nameNode = hbridgeElement.SelectSingleNode("Name");
            if (nameNode != null)
            {
                XmlElement nameElement = (XmlElement)nameNode;
                name = nameElement.InnerText;
            }
            else
            {
                name = string.Format("HB{0}", UnnamedHBCounter.ToString());
                UnnamedHBCounter++;
            }

            HBridgeWrapper newWrapper = new HBridgeWrapper(pwmPin, polarityPositivePin, polarityNegativePin, name);
 
            HBridgeDriversByName[name] = newWrapper;
            return newWrapper;
        }


        public HBridgeWrapper GetHBridgeDriver(string name)
        {
            HBridgeWrapper driver = null;

            try
            {
                driver = HBridgeDriversByName[name];
            }
            catch (Exception)
            {
            }

            return driver;
        }



        public ServoWrapper CreateServoDriver(GpioPinWrapper pin)
        {
            ServoWrapper servoWrapper = new ServoWrapper(pin, pin.PwmRange, PinPwmFrequency(pin.PinNumber));
            ServoDriversByIndex[pin.PinNumber] = servoWrapper;
            return servoWrapper;
        }



        public ServoWrapper GetServoDriver(int pin)
        {
            ServoWrapper driver = null;
            try
            {
                driver = ServoDriversByIndex[pin];
            }
            catch (Exception)
            {

            }

            return driver;
        }
       
        /// <summary>
        /// Create HBridge driver
        /// </summary>
        /// <param name="stepperElement">stepper xml element node</param>
        /// <returns>stepper wrapper object created</returns>
        public SevenSegDisplayWrapper CreateSevenSegDisplayDriver(XmlNode sevenSegDisplayElement)
        {
            int index = 0;

#if !RPI
            FakeIndex++;
            index = FakeIndex;
#else
            index = SevenSegDisplay.CreateFromXml(sevenSegDisplayElement.OuterXml);
#endif

            //  the library created a stepper driver
            //  we need to parse the stepper element so we can create pin wrappers 
            //  required to hook to the joystick and user interace for all the pins used
            if (index >= 0)
            {
                //  get the name info
                string name = "";
                XmlNode nameNode = sevenSegDisplayElement.SelectSingleNode("Name");
                if (nameNode != null)
                {
                    XmlElement nameElement = (XmlElement)nameNode;
                    name = nameElement.InnerText;
                }
                else
                {
                    name = string.Format("ssd {0}", index.ToString());
                }

                //  parse pins info
                XmlNode pinNode = sevenSegDisplayElement.SelectSingleNode("SegPins");
                if (pinNode == null)
                    return null;

                //  we made a display, now lets make pins for it
                List<int> pinIndexList = new List<int>();
                string[] pinStrings = Regex.Split(pinNode.InnerText, ",");

                try
                {
                    foreach (string nextPinString in pinStrings)
                    {
                        pinIndexList.Add(Math.Abs(Int32.Parse(nextPinString)));
                    }
                }
                catch (Exception e)
                {
                    //  invalid pins for this display
                    return null;
                }

                //  process the digit pins
                var digits = sevenSegDisplayElement.SelectNodes("Digits");
                foreach (var nextDigit in digits)
                {
                    var digitElement = (XmlElement)nextDigit;
                    pinStrings = Regex.Split(digitElement.InnerText, ",");

                    try
                    {
                        foreach (string nextPinString in pinStrings)
                        {
                            pinIndexList.Add(Math.Abs(Int32.Parse(nextPinString)));
                        }
                    }
                    catch (Exception e)
                    {
                        //  invalid pins for this display
                        return null;
                    }
                }

                //  go through the list of pins for this display
                List<GpioPinWrapper> segPins = new List<GpioPinWrapper>();
                foreach (int nextIndex in pinIndexList.Distinct())
                {
                    //  find this pin
                    var nextPin = GetPin(nextIndex);

                    //  these pins should exist and be created already
                    if (nextPin == null)
                        return null;

                    segPins.Add(nextPin);
                }

                //  create a new seven seg display wrapper 
                SevenSegDisplayWrapper newWrapper = new SevenSegDisplayWrapper(index, segPins);
                SevenSegDisplayDriversByIndex[index] = newWrapper;
                SevenSegDisplayDriversByName[name] = index;

                //  done
                return newWrapper;
            }

            //  error
            return null;
        }

        public SevenSegDisplayWrapper GetSevenSegDisplayDriver(string name)
        {
            SevenSegDisplayWrapper driver = null;

            try
            {
                int index = SevenSegDisplayDriversByName[name];
                driver = SevenSegDisplayDriversByIndex[index];
            }
            catch (Exception)
            {

                
            }

            return driver;
        }


        public void SevenSegmentDisplaySetDisplay(int index, string display)
        {
#if !RPI
            Console.WriteLine("Seven SEg Display " + display);
#else
            SevenSegDisplayWrapper driver = null;
            if (SevenSegDisplayDriversByIndex.TryGetValue(index, out driver))
            {
                driver.Set(display);
            }
#endif   
        }



        public void SevenSegmentDisplayOff(int index)
        {
#if !RPI
            Console.WriteLine("Seven SEg Display " + index.ToString());
#else
            SevenSegDisplayWrapper driver = null;
            if (SevenSegDisplayDriversByIndex.TryGetValue(index, out driver))
            {
                driver.Off();
            }
            
#endif   
        }


        /// <summary>
        /// Shut Down - stops all software PWM threads, and sets all output pins to 0
        /// </summary>
        public void ShutDown()
        {
            foreach (var nextPin in Pins)
            {
                //  turn off anything that might be output
                if (nextPin.Value.Mode == PinMode.PWMOutput)
                    nextPin.Value.PwmStop();
                else if (nextPin.Value.Mode == PinMode.Output)
                    nextPin.Value.Write(0);
            }
        }


        /// <summary>
        /// Get a list of pins owned by the program instance
        /// </summary>
        /// <returns>list of pin objects, sorted by number</returns>
        public List<GpioPinWrapper> GetAvailablePins()
        {
            List<GpioPinWrapper> pinList = new List<GpioPinWrapper>();
            pinList.AddRange(Pins.Values);
            return pinList.OrderBy(x => x.PinNumber).ToList();

        }


        /// <summary>
        /// Get Pin 
        /// </summary>
        /// <param name="pinNumber">physical pin number</param>
        /// <returns>pin object for this pin number</returns>
        public GpioPinWrapper GetPin(int pinNumber)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                return findPin;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! Get Pin : " + e.ToString());
                throw (e);
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinNumber"></param>
        /// <returns></returns>
        public PinMode GetPinMode(int pinNumber)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                return findPin.Mode;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! GetPinMode : " + e.ToString());
                throw (e);
            }
        }

        /// <summary>
        /// Set Pin State
        /// </summary>
        /// <param name="pinNumber">pin number on RPI model 2</param>
        /// <param name="state">pin state: true for on false for off</param>
        public void WritePin(int pinNumber, int value)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                findPin.Write(value);
                if (GpioEvents != null)
                    GpioEvents(this, new GpioEventArgs(pinNumber));
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! WritePin : " + e.ToString());
                throw (e);
            }
        }

        //  Turn off some pins
        public void WritePin(List<int> pinNumbers, int value)
        {
            for (int i = 0; i < pinNumbers.Count; i++)
            {
                WritePin(pinNumbers.ElementAt(i), value);
            }
        }


        /// <summary>
        /// Get the state of the pin
        /// </summary>
        /// <param name="pinNumber">pin number on RPI model 2</param>
        /// <returns></returns>
        public int ReadPin(int pinNumber)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                return findPin.Read();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! SetPinState : " + e.ToString());
                throw (e);
            }
        }


        //Function to turn off all pins
        public void AllOff()
        {
            int setPin = 1;
            while (setPin <= 40)
            {
                try
                {
                    GpioPinWrapper findPin;
                    Pins.TryGetValue(setPin, out findPin);

                    if (findPin.Mode == PinMode.PWMOutput)
                        findPin.PwmSetValue(0.0);
                    else
                        findPin.Write(0);

                    GpioEvents?.Invoke(this, new GpioEventArgs(findPin.PinNumber));

                }
                catch (Exception e)
                {
                    // this pin does not exist
                }

                setPin = setPin + 1;
            }
        }



        /// <summary>
        /// Start PWM for pin
        /// </summary>
        /// <param name="pinNumber">pin number on RPI model 2</param>
        /// <returns></returns>
        public void StartPwm(int pinNumber)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                findPin.PwmStart(0, findPin.PwmRange);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! StartPwm : " + e.ToString());
                throw (e);
            }
        }

        /// <summary>
        /// Stop PWM for pin
        /// </summary>
        /// <param name="pinNumber">pin number on RPI model 2</param>
        /// <returns></returns>
        public void StopPwm(int pinNumber)
        {
            try
            {
                GpioPinWrapper findPin;
                Pins.TryGetValue(pinNumber, out findPin);
                findPin.PwmStop();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("! Stop PWM : " + e.ToString());
                throw (e);
            }
        }


        /// <summary>
        /// Setup function, initializes the wiringPi library and extension library
        /// </summary>
        public void Setup()
        {

#if RPI
            //  setup wiring pi and extension
            if (wiringGpioExtensions.Setup.WiringGpioSetupPhys() < 0)
            {   
                return;
            }

            //  Set PWM clock to mark space mode
            //GPIO.PwmSetMode(0);
#endif
        }

        //  Keep track of our extension objects
        //

        //  Stepper drivers
        protected Dictionary<int, StepperWrapper> StepperDriversByIndex;
        protected Dictionary<string, int> StepperDriversByName;
        //  HBridge drivers
        protected Dictionary<string, HBridgeWrapper> HBridgeDriversByName;
        //  Seven segment dispay drivers
        protected Dictionary<int, SevenSegDisplayWrapper> SevenSegDisplayDriversByIndex;
        protected Dictionary<string, int> SevenSegDisplayDriversByName;

        protected Dictionary<int, ServoWrapper> ServoDriversByIndex;

    

        /// <summary>
        /// Constructor
        /// </summary>
        public GpioManager()
        {
            //  Keep track of pins in a dictionary:  key is physical pin number, value is GpioPinWrapper
            Pins = new Dictionary<int, GpioPinWrapper>();

            Initialize();
        }


        public void Initialize()
        {
            FakeIndex = 0;

            StepperDriversByName = new Dictionary<string, int>();
            StepperDriversByIndex = new Dictionary<int, StepperWrapper>();

            HBridgeDriversByName = new Dictionary<string, HBridgeWrapper>();

            SevenSegDisplayDriversByIndex = new Dictionary<int, SevenSegDisplayWrapper>();
            SevenSegDisplayDriversByName = new Dictionary<string, int>();

            ServoDriversByIndex = new Dictionary<int, ServoWrapper>();
            UnnamedHBCounter = 0;

            
        }



        protected Dictionary<int, int> McpPins = new Dictionary<int, int>();
        protected Dictionary<int, PcaChip> PcaPins = new Dictionary<int, PcaChip>();

        //  Pins Dictionary
        Dictionary<int, GpioPinWrapper> Pins;

        int FakeIndex = -1;


    }
}
