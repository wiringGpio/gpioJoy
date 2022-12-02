using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wiringGpioExtensions;

namespace GpioManagerObjects
{
    /// <summary>
    /// Stepper Motor Wrapper
    /// </summary>
    public class StepperWrapper
    {
        public StepperWrapper(int stepperIndex, List<GpioPinWrapper> pins, string name)
        {
            Name = name;
            Pins = pins;
            StepperIndex = stepperIndex;
        }

        public void SetDelay(float delay)
        {
            StepperMotor.SetDelay(StepperIndex, delay);
        }

        public void Step(int steps)
        {
            StepperMotor.Step(StepperIndex, steps);
        }

        public void Spin(int direction)
        {
            StepperMotor.Spin(StepperIndex, direction);
        }

        public void SetSpeed(float value)
        {
            StepperMotor.SetSpeed(StepperIndex, value);
        }

        public void Stop()
        {
            StepperMotor.Stop(StepperIndex);
        }

        public int TachoCount
        {
            get
            {
                return StepperMotor.GetTachoCount(StepperIndex);
            }
        }

        public void ResetTachoCount()
        {
            StepperMotor.ResetTachoCount(StepperIndex);
        }

       

        public int StepperIndex { get; protected set; }
        public List<GpioPinWrapper> Pins { get; protected set; }
        public string Name { get; protected set; }
    }
}
