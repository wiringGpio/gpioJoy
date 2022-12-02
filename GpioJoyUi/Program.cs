using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GpioJoyUi
{
    static class Program
    {
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

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
