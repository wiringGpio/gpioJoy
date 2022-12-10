using GpioManagerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GpioJoy
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
        LeftStickBtn,
        //  Right Stick
        RightStickUp,
        RightStickDown,
        RightStickRight,
        RightStickLeft,
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
        /// <summary>
        /// Constants
        /// </summary>
        protected const double JoystickDeadZone = 0.15;
        protected const int JoystickDeadZoneInt = 15;
        protected double JoystickScale = (1 / (1 - JoystickDeadZone));


        /// <summary>
        /// Constructor
        /// </summary>
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

        //  Process vector (stick/trigger) input
        public virtual void ProcessInput(double input, bool updateUi) { return; }

        //  Process button input
        public virtual void ProcessInput(bool input, bool updateUi) { return; }

        //  Mouse Up
        public virtual void UiControl_MouseUp(object sender, MouseEventArgs e) { return;  }

        //  Mouse Down
        public virtual void UiControl_MouseDown(object sender, MouseEventArgs e) { return; }

        //  Enabled / Active Flags
        public bool Enabled { get; set; }
        public bool Active { get; set; }

        // Joystick element we are assigned to
        public JoystickControl Assignment { get; protected set; }

        //  UI Controls
        public Control UiLabel { get; protected set; }
        public Control UiControl { get; protected set; }

        //  Name
        protected string _name;
        public virtual string Name { get { return _name != null ? _name : ""; } }

        /// <summary>
        /// Setup controls
        /// </summary>
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
            _trackBarControl = control;
            _currentValue = 0;
        }

        protected int _currentValue;
        protected int _direction;
        protected double _pwmScale;
        protected bool _reverse;

        protected TrackBar _trackBarControl;

        public override string Name => _name;


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
        protected void ProcessInputUi(double input, int newValue)
        {
            int value = (int)(input * 100);

            if (_trackBarControl != null)
            {
                _trackBarControl.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        _trackBarControl.Value = ReverseValue ? value * -1 : value;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ProcessInputUi exception: {e}");
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
            _pin = pin;
            _pwmScale = scale;
            _reverse = reverse;
            _name = displayName;
        }

        GpioPinWrapperJs _pin;

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (_currentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    if (_reverse)
                        scaleInput = 1.0 - scaleInput;
                    _pin.PwmSetValue(scaleInput * _pwmScale);
                    _currentValue = newValue;
                }
                else if ( _currentValue != 0 )
                {
                    _pin.PwmSetValue(_reverse ? 1.0 : 0.0);
                    _currentValue = 0;
                }

                if (updateUi )
                    ProcessInputUi(input, newValue);
               
            }
        }


        /// <summary>
        /// Mouse up
        /// </summary>
        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (_trackBarControl.Value > 0)
                    ProcessInput((double)_trackBarControl.Value / (double)_trackBarControl.Maximum, false);
                else
                    ProcessInput((double)_trackBarControl.Value / (double)(_trackBarControl.Minimum*-1), false);
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
            _direction = direction;
            _stepper = stepper;
        }

        StepperWrapper _stepper;

        public override string Name { get { return _stepper != null ? _stepper.Name : ""; } }

        /// <summary>
        /// Enable pins
        /// </summary>
        bool StepperPinsJoystickEnabled
        {
            get
            {
                foreach (var nextPin in _stepper.Pins)
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

            if (_currentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  figure out the input scaled past the dead zone
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    scaleInput *= (input > 0.0 ? 1.0 : -1.0);
                    //  is this reverse stick
                    if (StickPositive && _direction == -1)
                        scaleInput *= -1;
                    //  set stepper speed
                    _stepper.SetSpeed( (float)scaleInput);
                    // update current value 
                    _currentValue = newValue;
                }
                else if (_currentValue != 0)
                {
                    //  stop the stepper
                    _stepper.Stop();
                    _currentValue = 0;
                }

                //  update the UI
                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }


        /// <summary>
        /// Mouse up
        /// </summary>
        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (_trackBarControl.Value > 0)
                    ProcessInput((double)_trackBarControl.Value / (double)_trackBarControl.Maximum, false);
                else
                    ProcessInput((double)_trackBarControl.Value / (double)(_trackBarControl.Minimum*-1), false);
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
            _direction = direction;

            _hBridge = hbridge;
            _pwmScale = scale;
        }

        HBridgeWrapper _hBridge;

        public override string Name { get { return _hBridge != null ? _hBridge.Name : ""; } }


        /// <summary>
        /// Pins joystick enabled
        /// </summary>
        bool HBridgePinsJoystickEnabled
        {
            get
            {
                foreach (var nextPin in _hBridge.Pins)
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

            if (_currentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  scale the input for the dead zone
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    //  is this reverse stick
                    int direction = input > 0 ? 1 : -1;
                    if (StickPositive && _direction == -1)
                        direction *= -1;
                    //  set HBridge value
                    _hBridge.SetHBridgeValue((input > 0.0) ? 1 : -1, scaleInput * _pwmScale);
                    _currentValue = newValue;
                }
                else if (_currentValue != 0)
                {
                    //  turn it off
                    _hBridge.SetHBridgeValue(0, newValue);
                    _currentValue = 0;
                }

                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }


        /// <summary>
        /// Mouse up
        /// </summary>
        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (_trackBarControl.Value > 0)
                    ProcessInput((double)_trackBarControl.Value / (double)_trackBarControl.Maximum, false);
                else
                    ProcessInput((double)_trackBarControl.Value / (double)(_trackBarControl.Minimum*-1), false);
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
            _direction = direction;
            _servo = servo;
        }

        ServoWrapper _servo;

        public override string Name { get { return _servo != null ? _servo.Pin.Name : ""; } }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (_currentValue != newValue)
            {
                if (input > JoystickDeadZone || input < (-1 * JoystickDeadZone))
                {
                    //  calculate the tick for this value
                    double scaleInput = ((double)newValue / 100.0 - JoystickDeadZone) * JoystickScale;
                    scaleInput *= (input > 0 ? 1.0 : -1.0);
                    //  handle inverted stick
                    if (StickPositive && _direction == -1)
                        scaleInput *= -1;
                    int tickOffset = (int)(scaleInput * (_servo.MaxTick - _servo.CenterTick));
                    _servo.Pin.PwmSetValue(_servo.CenterTick + tickOffset);
                    _currentValue = newValue;

                }
                else if (_currentValue != 0)
                {
                    //  return to center
                    _servo.Pin.PwmSetValue(_servo.CenterTick);
                    _currentValue = 0;
                }

                if ( updateUi )
                    ProcessInputUi(input, NewValue(input));
            }
        }


        /// <summary>
        /// Mouse up
        /// </summary>
        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (_trackBarControl.Value > 0)
                    ProcessInput((double)_trackBarControl.Value / (double)_trackBarControl.Maximum, false);
                else
                    ProcessInput((double)_trackBarControl.Value / (double)(_trackBarControl.Minimum*-1), false);
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
        public JoystickSevenSegDisplay(SevenSegDisplayWrapper display, int direction, Control label, TrackBar control, JoystickControl assignment) : base(label, control, assignment)
        {
            _direction = direction;
            _display = display;
        }

        // Index of display in wiringPiExtension
        SevenSegDisplayWrapper _display;

        public override string Name { get { return _display != null ? _display.Name : ""; } }

        /// <summary>
        /// Process Input
        /// </summary>
        public override void ProcessInput(double input, bool updateUi)
        {
            if (!Enabled)
                return;

            int newValue = NewValue(input);

            if (_currentValue != newValue)
            {
                if (_currentValue == 0 && newValue < JoystickDeadZoneInt)
                    return;

                if (newValue < JoystickDeadZoneInt)
                    newValue = 0;

                _currentValue = newValue;

                if (_currentValue == 0)
                {
                    _display.Off();
                }
                else
                {   
                    int displayValue = (int)((_currentValue - JoystickDeadZoneInt )*JoystickScale + 0.5);
                    if (_direction < 0)
                        displayValue *= -1;
                    _display.Set(displayValue.ToString());
                }
            }
        }
        
        public override void UiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                if (_trackBarControl.Value > 0)
                    ProcessInput((double)_trackBarControl.Value / (double)_trackBarControl.Maximum, false);
                else
                    ProcessInput((double)_trackBarControl.Value / (double)(_trackBarControl.Minimum * -1), false);
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
            _buttonControl = control;
            _pin = pin;
            CurrentButtonValue = false;
            _name = displayName;
        }


        GpioPinWrapperJs _pin;
        Button _buttonControl;
        
        private bool CurrentButtonValue { get; set; }

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
                _pin.Write(input ? 1 : 0);
                CurrentButtonValue = input;

                if (updateUi && _buttonControl != null)
                {
                    _buttonControl.Invoke((MethodInvoker)delegate
                    {
                        _buttonControl.BackColor = input ? System.Drawing.Color.Black : System.Drawing.Color.White;
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
