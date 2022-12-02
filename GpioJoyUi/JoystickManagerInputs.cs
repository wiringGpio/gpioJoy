using GpioManagerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GpioJoyUi
{
    /// <summary>
    /// Joystick Control
    /// an individual item for each action on the joystick
    /// sticks are broken up into four different items
    /// triggers and buttons are one item
    /// </summary>
    public enum JoystickControl
    {
        //  Left Stick
        LeftStickUp,
        LeftStickDown,
        LeftStickRight,
        LeftStickLeft,
        //  button
        LeftStickBtn,
        //  Right Stick
        RightStickUp,
        RightStickDown,
        RightStickRight,
        RightStickLeft,
        //  button
        RightStickBtn,
        //  Triggers
        LeftTrigger,
        RightTrigger,
        // Buttons
        ABtn,
        BBtn,
        XBtn,
        YBtn,
        LeftBumper,
        RightBumper,
        BackBtn,
        StartBtn,
        HomeBtn,
        //  DPad
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight,
    };


    /// <summary>
    /// Joystick input base class 
    /// </summary>
    public class JoystickInput
    {
        protected const double JoystickDeadZone = 0.15;
        protected const int JoystickDeadZoneInt = 15;
        protected double JoystickScale = (1 / (1 - JoystickDeadZone));

        public JoystickInput(Control label, Control control, JoystickControl assignment)
        {
            Assignment = assignment;
            Enabled = true;
            UiLabel = label;
            UiControl = control;

            if (UiControl != null)
            {
                UiControl.MouseUp += UiControl_MouseUp;
                UiControl.MouseDown += UiControl_MouseDown;
            }
         

        }

       


        //  function to process vector (stick/trigger) input
        public virtual void ProcessInput(double input, bool updateUi) { return; }

        //  function to process button input
        public virtual void ProcessInput(bool input, bool updateUi) { return; }

        public virtual void UiControl_MouseUp(object sender, MouseEventArgs e) { return;  }

        public virtual void UiControl_MouseDown(object sender, MouseEventArgs e) { return; }

      

      

        //  flag if is enabled
        public bool Enabled { get; set; }
        public bool Active { get; set; }

        // specific control on the joystick we are assigned to
        public JoystickControl Assignment { get; protected set; }

        public Control UiLabel { get; protected set; }
        public Control UiControl { get; protected set; }

        public virtual string Name { get { return ""; } }

      
        public virtual void SetupControlUi()
        {
            if ( UiLabel != null )
                UiLabel.Text = Name;
            if(UiControl != null)
                UiControl.Enabled = true;
        }
        
        
    }


    /// <summary>
    /// Joystick stick class
    /// base class for handling stick (vector) inputs
    /// </summary>
    public class JoystickStickInput : JoystickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickStickInput(Control label, TrackBar control, JoystickControl assignment) : base(label, control, assignment)
        {
            Control = control;
            CurrentValue = 0;
        }


       
      

        //  Keep track of the last value processed as an integer (effective range of joystick is 0-100
        protected int CurrentValue { get; set; }
        protected TrackBar Control { get; set; }

    
        /// <summary>
        /// Stick Positive returns true if the vector for this stick direction is > 0
        /// </summary>
        protected bool StickPositive
        {
            get
            {
                switch (Assignment)
                {
                    case JoystickControl.LeftStickUp:
                    case JoystickControl.LeftStickRight:
                    case JoystickControl.LeftTrigger:
                    case JoystickControl.RightStickUp:
                    case JoystickControl.RightStickRight:
                    case JoystickControl.RightTrigger:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Reverse values for trigger assignments
        /// </summary>
        protected bool ReverseValue
        {
            get
            {
                switch (Assignment)
                {
                    case JoystickControl.LeftTrigger:
                    case JoystickControl.RightTrigger:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Get the value for this input
        /// handles the positive / negative stick direction
        /// returns 0 if this is negative stick and positive value, or if this is positive stick an negative value
        /// otherwise returns the value with the sign reversed if this is a stick negative direction
        /// </summary>
        /// <param name="input">raw joystick input for this control item</param>
        /// <returns>integer value between 0-100 for this input, sign is reversed if this is stick negative</returns>
        protected int NewValue(double input)
        {
            int newValue = (int)(input * 100);
            if (StickPositive && newValue < 0)
                newValue = 0;
            else if (!StickPositive && newValue > 0)
                newValue = 0;
            else if (!StickPositive)
                newValue *= -1;

            return newValue;
        }


        /// <summary>
        /// Update the UI control for this stick value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="newValue"></param>
        protected void ProcessInputUi(double input, int newValue)
        {
            int value = (int)(input * 100);

            if (Control != null)
            {
                Control.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        Control.Value = ReverseValue ? value * -1 : value;
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Exception in set ui " + e.ToString());
                    }
                });
            }
        }

       
    }



    /// <summary>
    /// Joystick input to control Pulse Width Modulation input
    /// control PWM to a single pin
    /// </summary>
    public class JoystickPwmStickInput : JoystickStickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickPwmStickInput(GpioPinWrapperJs pin, string displayName, Control label, TrackBar control, JoystickControl assignment, double scale, bool reverse) : base(label, control, assignment)
        {
            Pin = pin;
            PwmScale = scale;
            Reverse = reverse;
            DisplayName = displayName;
        }

        GpioPinWrapperJs Pin { get; set; }
        double PwmScale { get; set; }
        bool Reverse { get; set; }
        public string DisplayName { get; protected set; }

        public override string Name { get { return DisplayName; } }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (CurrentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    if (Reverse)
                        scaleInput = 1.0 - scaleInput;
                    Pin.PwmSetValue(scaleInput * PwmScale);
                    CurrentValue = newValue;
                }
                else if ( CurrentValue != 0 )
                {
                    Pin.PwmSetValue(Reverse ? 1.0 : 0.0);
                    CurrentValue = 0;
                }

                if (updateUi )
                    ProcessInputUi(input, newValue);
               
            }
        }

        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (Control.Value > 0)
                    ProcessInput((double)Control.Value / (double)Control.Maximum, false);
                else
                    ProcessInput((double)Control.Value / (double)(Control.Minimum*-1), false);
            }
        }

    }


    /// <summary>
    /// Joystick input to control stepper motor
    /// Stepper motor is referenced by index to wiringPiExtensions stepper motor item
    /// can be any number of physical pins
    /// </summary>
    public class JoystickStepperStickInput : JoystickStickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickStepperStickInput(StepperWrapper stepper, int direction, Control label, TrackBar control, JoystickControl assignment) : base(label, control, assignment)
        {
            Direction = direction;
            Stepper = stepper;
        }

        // Index of stepper motor in wiringPiExtension
        StepperWrapper Stepper { get; set; }

        public override string Name { get { return Stepper.Name; } }

        //  Direction Flag, this might be reversed stick
        int Direction { get; set; }

        bool StepperPinsJoystickEnabled
        {
            get
            {
                foreach (var nextPin in Stepper.Pins)
                {
                    var nextPinJs = (GpioPinWrapperJs)nextPin;
                    if (!nextPinJs.JoystickAssignmentsEnabled)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!StepperPinsJoystickEnabled)
                return;

            int newValue = NewValue(input);

            if (CurrentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  figure out the input scaled past the dead zone
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    scaleInput *= (input > 0.0 ? 1.0 : -1.0);
                    //  is this reverse stick
                    if (StickPositive && Direction == -1)
                        scaleInput *= -1;
                    //  set stepper speed
                    Stepper.SetSpeed( (float)scaleInput);
                    // update current value 
                    CurrentValue = newValue;
                }
                else if (CurrentValue != 0)
                {
                    //  stop the stepper
                    Stepper.Stop();
                    CurrentValue = 0;
                }

                //  update the UI
                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }

        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (Control.Value > 0)
                    ProcessInput((double)Control.Value / (double)Control.Maximum, false);
                else
                    ProcessInput((double)Control.Value / (double)(Control.Minimum*-1), false);
            }
        }
    }

    /// <summary>
    /// Joystick input to control stepper motor
    /// Stepper motor is referenced by index to wiringPiExtensions stepper motor item
    /// can be any number of physical pins
    /// </summary>
    public class JoystickHBridgeStickInput : JoystickStickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickHBridgeStickInput(HBridgeWrapper hbridge, int direction, Control label, TrackBar control, JoystickControl assignment, double scale) : base(label, control, assignment)
        {
            Direction = direction;

            HBridge = hbridge;
            PwmScale = scale;
        }

        // Index of stepper motor in wiringPiExtension
        HBridgeWrapper HBridge { get; set; }
        double PwmScale { get; set; }

        int Direction { get; set; }

        public override string Name { get { return HBridge.Name; } }


        bool HBridgePinsJoystickEnabled
        {
            get
            {
                foreach (var nextPin in HBridge.Pins)
                {
                    var nextPinJs = (GpioPinWrapperJs)nextPin;
                    if (!nextPinJs.JoystickAssignmentsEnabled)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!HBridgePinsJoystickEnabled)
                return;

            int newValue = NewValue(input);

            if (CurrentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  scale the input for the dead zone
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    //  is this reverse stick
                    int direction = input > 0 ? 1 : -1;
                    if (StickPositive && Direction == -1)
                        direction *= -1;
                    //  set HBridge value
                    HBridge.SetHBridgeValue((input > 0.0) ? 1 : -1, scaleInput * PwmScale);
                    CurrentValue = newValue;
                }
                else if (CurrentValue != 0)
                {
                    //  turn it off
                    HBridge.SetHBridgeValue(0, newValue);
                    CurrentValue = 0;
                }

                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }

        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (Control.Value > 0)
                    ProcessInput((double)Control.Value / (double)Control.Maximum, false);
                else
                    ProcessInput((double)Control.Value / (double)(Control.Minimum*-1), false);
            }
        }
    }


    /// <summary>
    /// Joystick input to control stepper motor
    /// Stepper motor is referenced by index to wiringPiExtensions stepper motor item
    /// can be any number of physical pins
    /// </summary>
    public class JoystickServoStickInput : JoystickStickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickServoStickInput(ServoWrapper servo, int direction, Control label, TrackBar control, JoystickControl assignment) : base(label, control, assignment)
        {
            Direction = direction;
            Servo = servo;
        }

        // Index of stepper motor in wiringPiExtension
        ServoWrapper Servo { get; set; }

        int Direction { get; set; }

        public override string Name { get { return Servo.Pin.Name; } }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (CurrentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  calculate the tick for this value
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    scaleInput *= (input > 0 ? 1.0 : -1.0);
                    //  handle inverted stick
                    if (StickPositive && Direction == -1)
                        scaleInput *= -1;
                    int tickOffset = (int)(scaleInput * (Servo.MaxTick - Servo.CenterTick));
                    Servo.Pin.PwmSetValue(Servo.CenterTick + tickOffset);
                    CurrentValue = newValue;

                }
                else if (CurrentValue != 0)
                {
                    //  return to center
                    Servo.Pin.PwmSetValue(Servo.CenterTick);
                    CurrentValue = 0;
                }

                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }

        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (Control.Value > 0)
                    ProcessInput((double)Control.Value / (double)Control.Maximum, false);
                else
                    ProcessInput((double)Control.Value / (double)(Control.Minimum*-1), false);
            }
        }
    }


    /// <summary>
    /// Joystick input to control stepper motor
    /// Stepper motor is referenced by index to wiringPiExtensions stepper motor item
    /// can be any number of physical pins
    /// </summary>
    public class JoystickSevenSegDisplay : JoystickStickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickSevenSegDisplay(SevenSegDisplayWrapper display, int direction, JoystickControl assignment) : base(null, null, assignment)
        {
            Direction = direction;
            Display = display;
        }

        // Index of display in wiringPiExtension
        SevenSegDisplayWrapper Display { get; set; }

        int Direction { get; set; }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (CurrentValue != newValue)
            {
                if (CurrentValue == 0 && newValue < JoystickDeadZoneInt)
                    return;

                if (newValue < JoystickDeadZoneInt)
                    newValue = 0;

                CurrentValue = newValue;

                if (CurrentValue == 0)
                {
                    System.Diagnostics.Debug.WriteLine("JOystick seven off: ");
                    Display.Off();
                }
                else
                {   
                    int displayValue = (int)((CurrentValue - JoystickDeadZoneInt )*JoystickScale + 0.5);
                    if (Direction < 0)
                        displayValue *= -1;
                    Display.Set(displayValue.ToString());
                }
            }
        }

        
    }



    /// <summary>
    /// Joystick Button
    /// </summary>
    public class JoystickButtonInput : JoystickInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickButtonInput(GpioPinWrapperJs pin, string displayName, Control label, Button control, JoystickControl assignment) : base(label, control, assignment)
        {
            
            Control = control;
            Pin = pin;
            CurrentButtonValue = false;
            DisplayName = displayName;
        }

      

        GpioPinWrapperJs Pin { get; set; }
        Button Control { get; set; }
        private bool CurrentButtonValue { get; set; }
        public string DisplayName { get; protected set; }

        public override string Name { get { return DisplayName; } }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(bool input, bool updateUi)
        {
            if (!Enabled)
                return;

           // Console.WriteLine("ProcessInput " + input.ToString() + " " + CurrentButtonValue.ToString() + " " + updateUi.ToString());

            if (CurrentButtonValue != input)
            {
                Pin.Write(input ? 1 : 0);
                CurrentButtonValue = input;

                if (updateUi && Control != null)
                {
                    Control.Invoke((MethodInvoker)delegate
                    {
                        Control.BackColor = input ? System.Drawing.Color.Black : System.Drawing.Color.White;
                    });
                }
            }
        }


        public override void UiControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                //Console.WriteLine("Mouse Down " + Control.Text + " " + CurrentButtonValue.ToString());
                if (CurrentButtonValue != true)
                    ProcessInput(true, false);
            }
        }


        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                //Console.WriteLine("Mouse Up " + Control.Text + " " + CurrentButtonValue.ToString());
                if (CurrentButtonValue != false)
                    ProcessInput(false, false);
            }
        }
    }
}
