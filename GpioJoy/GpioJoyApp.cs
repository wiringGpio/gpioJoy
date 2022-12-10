using GpioManagerObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GpioJoy
{
    public static class GpioJoyProgram
    {
        //  The GPIO Pin Manager
        static GpioManager _pinManager;
        public static GpioManager PinManager => _pinManager;

        //  The Joystick
        static JoystickManager _jsManager;
        public static JoystickManager JsManager => _jsManager;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
            Application.EnableVisualStyles();
            }
            catch ( Exception e )
            {
                Console.WriteLine(e.ToString());
            }

            _pinManager = new GpioManager();
            _pinManager.Setup();
            _jsManager = new JoystickManager(_pinManager);

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(_jsManager, _pinManager)); ;

            _jsManager.ShutDown();
            _pinManager.ShutDown();
        }
    }
}
