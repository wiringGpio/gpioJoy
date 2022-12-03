using PlatformHelper;
using static PlatformHelper.PlatformHelper;
using System.Collections.Generic;
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
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.SetDelay(StepperIndex, delay);
            }
        }

        public void Step(int steps)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.Step(StepperIndex, steps);
            }
        }

        public void Spin(int direction)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.Spin(StepperIndex, direction);
            }
        }

        public void SetSpeed(float value)
        {
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.SetSpeed(StepperIndex, value);
            }
        }

        public void Stop()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.Stop(StepperIndex);
            }
        }

        public int TachoCount
        {
            get
            {
                if (RunningPlatform() == Platform.Linux)
                {
                    return StepperMotor.GetTachoCount(StepperIndex);
                }
                return 0;
            }
        }

        public void ResetTachoCount()
        {
            if (RunningPlatform() == Platform.Linux)
            {
                StepperMotor.ResetTachoCount(StepperIndex);
            }
        }

       

        public int StepperIndex { get; protected set; }
        public List<GpioPinWrapper> Pins { get; protected set; }
        public string Name { get; protected set; }
    }
}
