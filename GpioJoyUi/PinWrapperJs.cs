using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiringPiWrapper;
using GpioManagerObjects;

namespace GpioJoyUi
{
    /// <summary>
    /// Pin Wrapper for Joystick
    /// Derived class to implement joystick features on the pin
    /// </summary>
    public class GpioPinWrapperJs : GpioPinWrapper
    {
        /// <summary>
        /// Pin Wrapper for Joystick
        /// sub classed from basic pin wrapper
        /// </summary>
        /// <param name="pinNumber">physical pin number</param>
        /// <param name="hwPwm">flag if this is the RPi hardware PWM pin (12) or a PCA chip pin</param>
        public GpioPinWrapperJs(int pinNumber, string name, bool hwPwm = false) 
            : base(pinNumber, name, hwPwm)
        {
            JoystickAssignments = new List<JoystickInput>();
        }

        //  Public Properties
        //

        /// <summary>
        /// This pin has a joystick assignment ?
        /// </summary>
        public bool HasJoystickAssignment { get { return JoystickAssignments.Count > 0; } }

        /// <summary>
        /// Add a joystick assignment to this pin
        /// </summary>
        /// <param name="assignment"></param>
        public void AddJoystickAssignment(JoystickInput assignment)
        {
            JoystickAssignments.RemoveAll(x => x.Assignment == assignment.Assignment);
            JoystickAssignments.Add(assignment);
        }

        /// <summary>
        /// Does this pin have any enabled joystick assignments ?
        /// </summary>
        public bool JoystickAssignmentsEnabled
        {
            get
            {
                foreach (var nextAssignment in JoystickAssignments)
                {
                    if (!nextAssignment.Enabled)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Set joystick assignments enabled/disabled
        /// </summary>
        /// <param name="enable"></param>
        public void SetJoystickAssignmentEnabled(bool enable)
        {
            foreach (var nextAssignment in JoystickAssignments)
                nextAssignment.Enabled = enable;
        }


        /// <summary>
        /// List of joystick assignments for this pin
        /// </summary>
        protected List<JoystickInput> JoystickAssignments { get; set; }


    }


    /// <summary>
    /// Derived class for PCA pins
    /// PCA pins behave a bit different on read
    /// </summary>
    public class GpioPinWrapperPcaJs : GpioPinWrapperJs
    {
        public GpioPinWrapperPcaJs(int pinNumber, string name, bool hwPwm = false)
            : base(pinNumber, name, hwPwm)
        {
            PwmRange = 4096;
        }


        public override int Read()
        {
            int read = GPIO.DigitalRead(PinNumber);

            return ((read & 0x1000) == 4096) ? 0 : 1;
        }
    }
}
