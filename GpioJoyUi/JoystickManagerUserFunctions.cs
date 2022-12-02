using GpioManagerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WiringPiWrapper;

namespace GpioJoyUi
{
    public partial class JoystickManager
    {
        //  Initialization
        #region Initialization
        List<GpioPinWrapper> XBoxModelPinsLeftSide;
        List<GpioPinWrapper> XBoxModelPinsRightSide;
        List<GpioPinWrapper> XBoxModelPinsHomeRow;
        List<GpioPinWrapper> XBoxModelPins
        {
            get
            {
                var list = new List<GpioPinWrapper>();
                list.AddRange(XBoxModelPinsLeftSide);
                list.AddRange(XBoxModelPinsRightSide);
                list.AddRange(XBoxModelPinsHomeRow);
                return list;
            }
        }

        SevenSegDisplayWrapper BoardDisplayDriver;
        SevenSegDisplayWrapper RemoteDisplayDriver;
        ServoWrapper ServoOne;
        ServoWrapper ServoTwo;
        HBridgeWrapper Motor2;
        HBridgeWrapper MotorNxt;
        GpioPinWrapper Motor1;
        StepperWrapper Stepper1;
        StepperWrapper Stepper2;
        GpioPinWrapper RedPin;
        GpioPinWrapper GreenPin;
        GpioPinWrapper BluePin;

        public void InitXBoxModelPins()
        {
            BoardDisplayDriver = PinManager.GetSevenSegDisplayDriver("BoardSevenSeg");
            RemoteDisplayDriver = PinManager.GetSevenSegDisplayDriver("RemoteSevenSeg");

            ServoOne = PinManager.GetServoDriver(500);
            ServoTwo = PinManager.GetServoDriver(501);
            Motor1 = PinManager.GetPin(405);
            Motor2 = PinManager.GetHBridgeDriver("Motor Two");
            MotorNxt = PinManager.GetHBridgeDriver("Motor NXT");

            Stepper1 = PinManager.GetStepperDriver("Stepper One");
            Stepper2 = PinManager.GetStepperDriver("Stepper Two");

            RedPin = PinManager.GetPin(513);
            GreenPin = PinManager.GetPin(514);
            BluePin = PinManager.GetPin(515);

            XBoxModelPinsLeftSide = new List<GpioPinWrapper>();
            XBoxModelPinsRightSide = new List<GpioPinWrapper>();
            XBoxModelPinsHomeRow = new List<GpioPinWrapper>();

            //  right side, starting at trigger then clockwise through controls
            XBoxModelPinsRightSide.Add(PinManager.GetPin(300));    //  right trigger
            XBoxModelPinsRightSide.Add(PinManager.GetPin(301));    //  right bumper
            XBoxModelPinsRightSide.Add(PinManager.GetPin(305));    //  Y
            XBoxModelPinsRightSide.Add(PinManager.GetPin(304));    //  B
            XBoxModelPinsRightSide.Add(PinManager.GetPin(303));    //  A
            XBoxModelPinsRightSide.Add(PinManager.GetPin(302));    //  X
            XBoxModelPinsRightSide.Add(PinManager.GetPin(214));    //  RS up
            XBoxModelPinsRightSide.Add(PinManager.GetPin(215));    //  RS rt
            XBoxModelPinsRightSide.Add(PinManager.GetPin(307));    //  RS dn
            XBoxModelPinsRightSide.Add(PinManager.GetPin(306));    //  RS lf
            XBoxModelPinsRightSide.Add(PinManager.GetPin(407));    //  RS bt

            XBoxModelPinsLeftSide.Add(PinManager.GetPin(207));     //  left trigger
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(206));     //  left bumper
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(205));     //  LS up
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(204));     //  LS rt
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(203));     //  LS dn
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(202));     //  LS lf
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(201));     //  LS bt
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(200));     //  DP up
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(208));     //  DP rt
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(209));     //  DP dn
            XBoxModelPinsLeftSide.Add(PinManager.GetPin(210));     //  DP lf

            XBoxModelPinsHomeRow.Add(PinManager.GetPin(211));      //  back
            XBoxModelPinsHomeRow.Add(PinManager.GetPin(212));      //  home
            XBoxModelPinsHomeRow.Add(PinManager.GetPin(213));      //  start
        }

     
        //  function to toggle the configuration to the next one, used by all configurations
        public void HomeBtn(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - HomeBtnChangeTick > ButtonDelay)
            {
                HomeConfiguration();
                HomeBtnChangeTick = tickNow;

                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public void BackBtn(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BackBtnChangeTick > ButtonDelay)
            {
                ToggleConfiguration(-1);
                BackBtnChangeTick = tickNow;

                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public void StartBtn(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - StartBtnChangeTick > ButtonDelay)
            {
                ToggleConfiguration(1);
                StartBtnChangeTick = tickNow;

                OnStateChanged?.Invoke(this, new EventArgs());
            }
        }

        #endregion

        //  Servo Functions
        //
        #region Servos


        //  Servo speed up/down
        double ServoOneValue = 1.5;
        double ServoTwoValue = 1.5;

        public void DPadUpServoPlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                ServoOneValue += 0.01;

                if (ServoOneValue >= 2.0)
                    ServoOneValue = 2.0;

                ServoOne.SetPulse(ServoOneValue);

                BoardDisplayDriver.Set(string.Format("{0:0.00}", ServoOneValue));
                
                DpadUpChangeTick = tickNow;
            }
        }

        public void DPadDownServoMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                ServoOneValue -= 0.01;

                if (ServoOneValue <= 1.0)
                    ServoOneValue = 1.0;

               ServoOne.SetPulse(ServoOneValue);

               BoardDisplayDriver.Set(string.Format("{0:0.00}", ServoOneValue));

                DpadDownChangeTick = tickNow;
            }
        }

        public void DPadRightServoCenter(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                ServoOne.Center();
                ServoOneValue = 1.5;

               BoardDisplayDriver.Set(string.Format("{0:0.00}", ServoOneValue));
                
                DpadRightChangeTick = tickNow;
            }
        }

        
        public void YBtnServoTwoPlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - YBtnChangeTick > ButtonDelay)
            {
                ServoTwoValue += 0.01;

                if (ServoTwoValue >= 2.0)
                    ServoTwoValue = 2.0;

                ServoTwo.SetPulse(ServoTwoValue);

                RemoteDisplayDriver.Set(string.Format("{0:0.00}", ServoTwoValue));
                
                YBtnChangeTick = tickNow;
            }
        }

        public void ABtnServoTwoOneMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - ABtnChangeTick > ButtonDelay)
            {
                ServoTwoValue -= 0.01;

                if (ServoTwoValue <= 1.0)
                    ServoTwoValue = 1.0;

                ServoTwo.SetPulse(ServoTwoValue);

                RemoteDisplayDriver.Set(string.Format("{0:0.00}", ServoTwoValue));

                ABtnChangeTick = tickNow;
            }
        }

        public void BBtServoTwoCenter(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BBtnChangeTick > ButtonDelay)
            {
                ServoTwo.Center();
                ServoTwoValue = 1.5;

                RemoteDisplayDriver.Set(string.Format("{0:0.00}", ServoTwoValue));

                BBtnChangeTick = tickNow;
            }
        }

        #endregion


        //  Motors
        #region Motors

        double MotorValueOne = 0.0;
        double MotorValueTwo = 0.0;
        double MotorValueNxt = 0.0;

        public void SetMotorOneDisplay()
        {
            if (MotorValueOne < 0.0)
            {
                if (MotorValueOne <= -1.0)
                    BoardDisplayDriver.Set(string.Format("{0:.0}", MotorValueOne));
                else
                    BoardDisplayDriver.Set(string.Format("{0:.00}", MotorValueOne));
            }
            else
            {
                BoardDisplayDriver.Set(string.Format("{0:0.00}", MotorValueOne));
            }
        }

        public void SetMotorTwoDisplay()
        {
            if (MotorValueTwo < 0.0)
            {
                if (MotorValueTwo <= -1.0)
                    RemoteDisplayDriver.Set(string.Format("{0:.0}", MotorValueTwo));
                else
                    RemoteDisplayDriver.Set(string.Format("{0:.00}", MotorValueTwo));
            }
            else
            {
                RemoteDisplayDriver.Set(string.Format("{0:0.00}", MotorValueTwo));
            }
        }

        public void SetMotorNxtDisplay()
        {
            if (MotorValueNxt < 0.0)
            {
                if (MotorValueNxt <= -1.0)
                    RemoteDisplayDriver.Set(string.Format("{0:.0}", MotorValueNxt));
                else
                    RemoteDisplayDriver.Set(string.Format("{0:.00}", MotorValueNxt));
            }
            else
            {
                RemoteDisplayDriver.Set(string.Format("{0:0.00}", MotorValueNxt));
            }
        }

        public void YBtnMotorTwoPlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - YBtnChangeTick > ButtonDelay)
            {
                MotorValueTwo += 0.05;
                if (MotorValueTwo > 1.0)
                    MotorValueTwo = 1.0;

                Motor2.SetHBridgeValue(MotorValueTwo > 0.0 ? 1 : -1, MotorValueTwo);
                SetMotorTwoDisplay();
                YBtnChangeTick = tickNow;
            }
        }

        public void ABtnMotorTwoMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - ABtnChangeTick > ButtonDelay)
            {
                MotorValueTwo -= 0.05;
                if (MotorValueTwo < -1.0)
                    MotorValueTwo = -1.0;

                Motor2.SetHBridgeValue(MotorValueTwo > 0.0 ? 1 : -1, MotorValueTwo);
                SetMotorTwoDisplay();
                ABtnChangeTick = tickNow;
            }
        }

        public void BBtnMotorTwoStop(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BBtnChangeTick > ButtonDelay)
            {
                MotorValueTwo = 0.0;
               
                Motor2.SetHBridgeValue(0, MotorValueTwo);
                SetMotorTwoDisplay();
                BBtnChangeTick = tickNow;
            }
        }

      

        public void DPadUpMotorOnePlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                MotorValueOne += 0.05;
                if (MotorValueOne > 1.0)
                    MotorValueOne = 1.0;
               
                Motor1.PwmSetValue(MotorValueOne);
                SetMotorOneDisplay();
                DpadUpChangeTick = tickNow;
            }
        }

        public void DPadDownMotorOneMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                MotorValueOne -= 0.05;
                if (MotorValueOne < -1.0)
                    MotorValueOne = -1.0;

                Motor1.PwmSetValue(MotorValueOne);
                SetMotorOneDisplay();
                DpadDownChangeTick = tickNow;
            }
        }

        public void DPadRightMotorOneStop(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                MotorValueOne = 0.0;

                Motor1.PwmSetValue(MotorValueOne);
                SetMotorOneDisplay();
                DpadRightChangeTick = tickNow;
            }
        }


        public void RightBumperMotorNxtPlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - RightBumperChangeTick > ButtonDelay)
            {
                MotorValueNxt += 0.05;
                if (MotorValueNxt > 1.0)
                    MotorValueNxt = 1.0;

                MotorNxt.SetHBridgeValue(MotorValueNxt > 0.0 ? 1 : -1, MotorValueNxt);
                SetMotorNxtDisplay();
                RightBumperChangeTick = tickNow;
            }
        }

        public void LeftBumperMotorNxtMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - LeftBumperChangeTick > ButtonDelay)
            {
                MotorValueNxt -= 0.05;
                if (MotorValueNxt < -1.0)
                    MotorValueNxt = -1.0;

                MotorNxt.SetHBridgeValue(MotorValueNxt > 0.0 ? 1 : -1, MotorValueNxt);
                SetMotorNxtDisplay();
                LeftBumperChangeTick = tickNow;
            }
        }

        public void DPadLeftMotorNxtStop(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadLeftChangeTick > ButtonDelay)
            {
                MotorValueNxt = 0.0;

                MotorNxt.SetHBridgeValue(0, MotorValueNxt);
                SetMotorNxtDisplay();
                DpadLeftChangeTick = tickNow;
            }
        }

        #endregion

        //  Gpio Joy Board Demo
        //
        #region GpioDemo

        public void LeftBumperStartGpioJoyDemo(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - LeftBumperChangeTick > ButtonDelay)
            {
                if (GpioDemoRun)
                    return;

                GpioDemoThread = new Thread(new ThreadStart(GpioDemoThreadRunFunction));

                GpioDemoRun = true;
                GpioDemoThread.Start();

                LeftBumperChangeTick = tickNow;
            }
        }

        public void RightBumperStopGpioJoyDemo(bool input)
        {
            if (input)
            {
                PinManager.AllOff();

                if (GpioDemoThread == null)
                    return;

                GpioDemoRun = false;
                GpioDemoThread.Interrupt();
                GpioDemoThread.Join();
                GpioDemoThread = null;

                Motor2.SetHBridgeValue(0, 0);
                ServoOne.Center();
                Stepper1.Stop();
                Stepper2.Stop();
                Motor1.Write(0);

                //  init the xbox pins
                foreach (var nextPin in XBoxModelPins)
                {
                    nextPin.Write(0);
                    if (nextPin.Mode == GPIO.GPIOpinmode.PWMOutput)
                        nextPin.PwmResume();
                }
            }
        }


      


        bool GpioDemoRun = false;
        Thread GpioDemoThread;

        public void PinOnOff(GpioPinWrapper pin, int sleepMs)
        {

            //if (pin.Mode == GPIO.GPIOpinmode.PWMOutput)
            //    pin.PwmSetValue(1.0);
            //else
            pin.Write(1);
            Thread.Sleep(sleepMs);
            //if (pin.Mode == GPIO.GPIOpinmode.PWMOutput)
            //    pin.PwmSetValue(0.0);
            //else
            pin.Write(0);
        }

        public void PinOn(GpioPinWrapper pin)
        {
            //var thisPin = PinManager.GetPin(pin);
            //if (thisPin.Mode == GPIO.GPIOpinmode.PWMOutput)
            //    thisPin.PwmSetValue(1.0);
            //else
            pin.Write(1);
        }

        public void PinOff(GpioPinWrapper pin)
        {
            //var thisPin = PinManager.GetPin(pin);
            //if (thisPin.Mode == GPIO.GPIOpinmode.PWMOutput)
            //    thisPin.PwmSetValue(0.0);
            //else
            pin.Write(0);
        }

        public void BlinkHome(int delay)
        {
            try
            {
                Thread.Sleep(1000);
                int tickStart = Environment.TickCount;
                int blink = 0;
                //  blink
                while (Environment.TickCount - tickStart < 3000)
                {
                    foreach (var nextPin in XBoxModelPinsHomeRow)
                    {
                        PinOn(nextPin);
                    }

                    Thread.Sleep(delay);

                    foreach (var nextPin in XBoxModelPinsHomeRow)
                    {
                        PinOff(nextPin);
                    }

                    Thread.Sleep(delay);

                    blink++;
                }
            }
            catch
            {

            }
        }

        public void GpioDemoThreadRunFunction()
        {
            int counter = 9;
            int direction = -1;
            int multiple = 10;

            //  init the xbox pins
            foreach (var nextPin in XBoxModelPins)
            {
                if (nextPin.Mode == GPIO.GPIOpinmode.PWMOutput)
                    nextPin.PwmPause();
            }

            Stepper1.SetDelay(20.0f);
            //    Stepper2.SetDelay(500.0f);

            Stepper1.SetSpeed(1.0f);
            //      Stepper2.SetSpeed(1.0f);


            try
            {
                while (GpioDemoRun)
                {
                    int sleepLength = counter * multiple;
                    if (sleepLength > 9 * multiple)
                    {
                        counter = 9;
                        sleepLength = 9 * multiple;
                        direction = -1;
                        Stepper1.Stop();
                        //    Stepper2.SetSpeed(1.0f);
                        BlinkHome(500);

                    }
                    else if (sleepLength < multiple)
                    {
                        counter = 1;
                        sleepLength = multiple;
                        direction = 1;
                        Stepper1.Stop();
                        //    Stepper2.SetSpeed(-1.0f);
                        BlinkHome(100);
                    }

                    Stepper1.SetSpeed(direction * 1.0f);

                    BoardDisplayDriver.Set(string.Format("{0}{1}{2}", counter, counter, counter));


                    double inv = 10 - counter;
                    double servoValue = 1.5 + 0.007 * inv;

                    ServoOne.SetPulse(servoValue);

                    double pwmValue = .35 + inv * 0.0075;
                    Motor2.SetHBridgeValue(1, pwmValue);



                    //  set the motor speeds
                    foreach (var nextPin in XBoxModelPinsLeftSide)
                    {
                        PinOnOff(nextPin, sleepLength);
                    }

                    foreach (var nextPin in XBoxModelPinsHomeRow)
                    {
                        PinOnOff(nextPin, sleepLength);
                    }

                    foreach (var nextPin in XBoxModelPinsRightSide)
                    {
                        PinOnOff(nextPin, sleepLength);
                    }




                    if (direction > 0)
                        counter++;
                    else
                        counter--;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion


        //  Three DOF Conciguration Functions
        //
        #region ThreeDofConfig

        public void DpadUpThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("X", 10);
                PinManager.StepperStep("X", 10);
                DpadUpChangeTick = tickNow;
            }
        }

        public void DpadDownThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("X", 10);
                PinManager.StepperStep("X", -10);
                DpadDownChangeTick = tickNow;
            }
        }

        public void DpadLeftThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("Y", 10);
                PinManager.StepperStep("Y", -10);
                DpadLeftChangeTick = tickNow;
            }
        }

        public void DpadRightThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("Y", 10);
                PinManager.StepperStep("Y", 10);
                DpadRightChangeTick = tickNow;
            }
        }

        public void BBtnThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BBtnChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("Z", 10);
                PinManager.StepperStep("Z", 10);
                DpadRightChangeTick = tickNow;
            }
        }

        public void XBtnThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - XBtnChangeTick > ButtonDelay)
            {
                //  get the X stepper
                PinManager.StepperSetDelay("Z", 10);
                PinManager.StepperStep("Z", -10);
                DpadRightChangeTick = tickNow;
            }
        }

        public void LeftBumperThreeDof(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - LeftBumperChangeTick > ButtonDelay)
            {
                if (ThreeDofDemoRun)
                    return;

                PinManager.StepperSetDelay("X", 30);
                PinManager.StepperSetDelay("Y", 30);
                PinManager.StepperSetDelay("Z", 30);

                ThreeDofDemoThread = new Thread(new ThreadStart(ThreeDofThreadRunFunction));

                ThreeDofDemoRun = true;
                ThreeDofDemoThread.Start();

                LeftBumperChangeTick = tickNow;
            }
        }

        public void RightBumperThreeDof(bool input)
        {
            if (input)
            {
                PinManager.StepperStop("X");
                PinManager.StepperStop("Y");
                PinManager.StepperStop("Z");

                if (ThreeDofDemoThread == null)
                    return;

                ThreeDofDemoRun = false;
                ThreeDofDemoThread.Interrupt();
                ThreeDofDemoThread.Join();
                ThreeDofDemoThread = null;

                PinManager.StepperStop("X");
                PinManager.StepperStop("Y");
                PinManager.StepperStop("Z");
            }
        }

        bool ThreeDofDemoRun = false;
        Thread ThreeDofDemoThread;

        public void ThreeDofThreadRunFunction()
        {


            try
            {
                //  first half turn
                PinManager.StepperStep("Y", -70);   //  blue
                PinManager.StepperStep("X", -75);   //  green
                PinManager.StepperStep("Z", -150);  //  red
                Thread.Sleep(10000);

                while (ThreeDofDemoRun)
                {
                    PinManager.StepperStep("Y", 140);
                    PinManager.StepperStep("Y", -140);
                    PinManager.StepperStep("X", 150);
                    PinManager.StepperStep("X", -150);
                    PinManager.StepperStep("Z", 300);
                    PinManager.StepperStep("Z", -300);

                    Thread.Sleep(3000);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        //  RGB
        //
        #region RGB

        double RedValue = 1.0;
        double GreenValue = 1.0;
        double BlueValue = 1.0;
        
        public void SetRgb()
        {
            RedPin.PwmSetValue(RedValue);
            GreenPin.PwmSetValue(GreenValue);
            BluePin.PwmSetValue(BlueValue);

            double invRed = (1.0 - RedValue) * 10;
            int display = (int)(invRed * 100 + 0.5);
            double invGreen = (1.0 - GreenValue) * 10;
            display += (int)(invGreen * 10 + 0.5);
            double invBlue = (1.0 - BlueValue) * 10;
            display += (int)(invBlue + 0.5);
            BoardDisplayDriver.Set(display.ToString());
            Console.WriteLine("SetRGB " + display.ToString());

        }

        public void YBtnRGBYellow(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - YBtnChangeTick > ButtonDelay)
            {
                YBtnChangeTick = tickNow;

                RedValue = 0.9;
                GreenValue = 0.9;
                BlueValue = 1.0;
                SetRgb();
               
               
            }
        }

        public void ABtnRGBGreen(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - ABtnChangeTick > ButtonDelay)
            {
                ABtnChangeTick = tickNow;
                GreenValue -= .1;
                if (GreenValue <= 0.0)
                    GreenValue = 0.0;
                SetRgb();


            }
        }

        public void BBtnRGBRed(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BBtnChangeTick > ButtonDelay)
            {
                BBtnChangeTick = tickNow;
                RedValue -= .1;
                if (RedValue <= 0.0)
                    RedValue = 0.0;
                SetRgb();
            }
;
        }

        public void XBtnRGBBlue(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - XBtnChangeTick > ButtonDelay)
            {
                    XBtnChangeTick = tickNow;
                BlueValue -= .1;
                if (BlueValue <= 0.0)
                    BlueValue = 0.0;
                SetRgb();
                
            }
        }

        public void DpadRightRGBReset(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - DpadChangeTick > ButtonDelay)
            {
                    DpadRightChangeTick = tickNow;
                RedValue = 1.0;
                GreenValue = 1.0;
                BlueValue = 1.0;
                SetRgb();
                
            }
        }

        #endregion


        //  Steppers
        //
        #region Steppers

        float StepperOneDelay = 1.0f;
        int StepperOneDirection = 1;
        float StepperTwoDelay = 1.0f;
        int StepperTwoDirection = 1;

        public void YBtnStepperOnePlus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - YBtnChangeTick > ButtonDelay)
            {
                if (Math.Abs(StepperOneDelay) < 10)
                    StepperOneDelay += 1.0f;
                else if (Math.Abs(StepperOneDelay) < 100)
                    StepperOneDelay += 10f;
                else 
                    StepperOneDelay += 50;

                //  switching directions
                if (StepperOneDelay < 1.0 && StepperOneDelay > -1.0)
                {
                    StepperOneDelay = (StepperOneDirection > 0 ? -1.0f : 1.0f);
                }

                if (StepperOneDelay > 0.0f)
                    StepperOneDirection = 1;
                else
                    StepperOneDirection = -1;

                Stepper1.SetDelay(Math.Abs(StepperOneDelay));
                Stepper1.Spin(StepperOneDirection > 0 ? 1:-1);

                int delayMs = 0;
                if (StepperOneDelay > 0.0f)
                    delayMs = (int)(StepperOneDelay + 0.5);
                else
                    delayMs = (int)(StepperOneDelay - 0.5);
                BoardDisplayDriver.Set(string.Format("{0}", delayMs.ToString()));

                YBtnChangeTick = tickNow;
            }
        }

        public void ABtnStepperOneMinus(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - ABtnChangeTick > ButtonDelay)
            {
                if (Math.Abs(StepperOneDelay) < 10)
                    StepperOneDelay -= 1.0f;
                else if (Math.Abs(StepperOneDelay) < 100)
                    StepperOneDelay -= 10f;
                else
                    StepperOneDelay -= 50;

                //  switching directions
                if (StepperOneDelay < 1.0 && StepperOneDelay > -1.0)
                {
                    StepperOneDelay = (StepperOneDirection > 0 ? -1.0f : 1.0f);
                }

               

                if (StepperOneDelay > 0.0f)
                    StepperOneDirection = 1;
                else
                    StepperOneDirection = -1;

                Stepper1.SetDelay(Math.Abs(StepperOneDelay));
                Stepper1.Spin(StepperOneDirection > 0 ? 1 : -1);

                int delayMs = 0;
                if (StepperOneDelay > 0.0f)
                    delayMs = (int)(StepperOneDelay + 0.5);
                else
                    delayMs = (int)(StepperOneDelay - 0.5);

                
                BoardDisplayDriver.Set(string.Format("{0}", delayMs.ToString()));
                ABtnChangeTick = tickNow;
            }
        }

        public void BBtnStepperOneStop(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - BBtnChangeTick > ButtonDelay)
            {
                Stepper1.Stop();
                BoardDisplayDriver.Off();
                BBtnChangeTick = tickNow;
                StepperOneDelay = 1.0f;
                StepperOneDirection = 1;
            }
        }

        public void XBtnStepperOneResetDelay(bool input)
        {
            var tickNow = System.Environment.TickCount;
            if (input && System.Environment.TickCount - XBtnChangeTick > ButtonDelay)
            {
                Stepper1.Stop();
                StepperOneDelay = 1.0f;
                StepperOneDirection = 1;
                BoardDisplayDriver.Off();
                BBtnChangeTick = tickNow;
            }
        }



     
        #endregion

        //  Button Timer Handling
        //  used to prevent high frequency spamming of functions
        #region ButtonTimer

        int LeftStickBtnChangeTick = 0;
        int RightStickBtnChangeTick = 0;
        int ABtnChangeTick = 0;
        int BBtnChangeTick = 0;
        int XBtnChangeTick = 0;
        int YBtnChangeTick = 0;
        int LeftBumperChangeTick = 0;
        int RightBumperChangeTick = 0;

        int BackBtnChangeTick = 0;
        int StartBtnChangeTick = 0;
        int HomeBtnChangeTick = 0;
        int DpadUpChangeTick = 0;
        int DpadDownChangeTick = 0;
        int DpadLeftChangeTick = 0;
        int DpadRightChangeTick = 0;

        public int ButtonDelay = 333;

        int DpadChangeTick
        {

            get
            {
                return new[] { DpadUpChangeTick, DpadDownChangeTick, DpadLeftChangeTick, DpadRightChangeTick }.Max();
            }
        }
        #endregion


    }
}
