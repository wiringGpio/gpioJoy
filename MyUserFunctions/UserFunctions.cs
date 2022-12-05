using GpioJoy;
using GpioManagerObjects;
using System;
using System.Threading.Tasks;

namespace MyUserFunctions
{
    public static class UserFunctions
    {
        //  To use pins in your user functions, add a reference to GpioJoy, GpioManagerObjects, and wiringGpioExtensions
        static GpioManager PinManager => GpioJoyProgram.PinManager;

        public static void Init()
        {
            Console.WriteLine("User functions, init called");
        }

        public static void ButtonDown(bool input )
        {
            Console.WriteLine($"User function button down {input}");
        }

        public static async void ButtonDownPinFourty(bool input)
        {
            //  check for button mashing, only accept this button once every six seconds
            if ( input && Environment.TickCount - _buttonDownPinFourtyTick > 6000)
            {
                await FlashButtonFourty();
            }
        }
        static int _buttonDownPinFourtyTick = 0;


        /// <summary>
        /// Flash pin 7 five times
        /// </summary>
        static async Task FlashButtonFourty()
        {
            for (int i = 0; i < 5; i++)
            {
                PinManager.GetPin(40).Write(1);
                await Task.Delay(500);
                PinManager.GetPin(40).Write(0);
                await Task.Delay(500);
            }
        }
    }
}
