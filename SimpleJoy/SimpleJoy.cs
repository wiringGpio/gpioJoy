using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;
using PlatformHelper;


namespace SimpleJoy
{
    //  Joystick Types
    public enum JoystickType
    {
        Unknown,
        XBox,
        Wii,
    }

    //  Joystick Events
    //
    public class XBoxJoystickEventArgs : EventArgs
    {
        public XBoxJoystickEventArgs(IXBoxJoystickReading data)
        {
            Data = data;

        }

        public IXBoxJoystickReading Data { get; protected set; }
    }
    public delegate void XBoxJoystickEvent(object sender, XBoxJoystickEventArgs e);




    public delegate void JoystickDisconnectEvent(object sender, EventArgs e);


    /// <summary>
    /// SimpleJoystick
    /// implementation for receiving joystick inputs
    /// </summary>
    public class SimpleJoystick
    {
        //  Joystick Events
        //  each type of device will generate a different event
        //
        public event XBoxJoystickEvent XboxJoystickEventHandler;
       
        public event JoystickDisconnectEvent JoystickDisconnectHandler;

        /// <summary>
        /// Is Connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
                {
                    //  Linux, we are connected when file stream is open
                    return fs != null;
                }
                else
                {
                    //  Windows, we are connected when wiimote object is not null
                    return false;
                }
            }
        }


        /// <summary>
        /// Get Joystick Paths
        /// function used to get the file objects in /dev/input on linux machines, one of the 'jsN' objects will be your xbox joystick
        /// </summary>
        /// <returns>List of strings 'js0' to 'jsN'</returns>
        static public List<string> GetJoystickPaths()
        {
            List<string> joysticks = new List<string>();

            if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
            {
                //  get the joysticks in the /dev/input path
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "ls";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.Arguments = "/dev/input";
                proc.Start();
                string data = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                string[] paths = data.Split('\n');
                foreach (var nextPath in paths)
                {
                    if (nextPath.Contains("js"))
                        joysticks.Add(nextPath);
                }
                //  parse this string data	"by-id\nby-path\nevent0\nevent1\nevent2\nevent3\nevent4\njs0\nmice\nmouse0\n"	string
            }

            return joysticks;
        }

        /// <summary>
        /// Connect function
        /// </summary>
        /// <param name="type">type of connection</param>
        /// <param name="path">path to joystick (Linux, for windows leave blank)</param>
        /// <returns>true if connection succeeded</returns>
        public bool ConnectToJoystick(JoystickType type, string path)
        {
            Type = JoystickType.Unknown;

            switch (type)
            {
                case JoystickType.Wii:
                    return ConnectToWiimote(path);

                case JoystickType.XBox:
                    return ConnectToXBoxController(path);

                default:
                    return false;
            }
        }


        /// <summary>
        /// Disconnect function
        /// </summary>
        public void Disconnect()
        {
            if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
            {
                //  Linux has a thread open
                if (PollingThread != null && PollingThread.IsAlive)
                {
                    RunPollingThread = false;
                    //  TODO - fix the sync read in this thread so interrupt will work.
                    PollingThread.Interrupt();
                    PollingThread.Join();
                }
            }
            else
            {
                
            }

        }


        //  Class Properties
        private JoystickType Type;
      
        private string Path;


        /// <summary>
        /// Connect to Wiimote
        /// </summary>
        /// <param name="path">path to joystick for Linux, windows can leave blank)</param>
        /// <returns>true if connected</returns>
        private bool ConnectToWiimote(string path)
        {
            Path = path;
            if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
            {
                //  Linux, make sure path exists
                if (!File.Exists(path))
                {
                    return false;
                }

                //  start up the polling thread, the file stream will be opened before the thread loop starts
                RunPollingThread = true;
                PollingThread = new Thread(new ThreadStart(PollingThreadRun));
                PollingThread.Start();
            }
            else
            {
                
            }
            Type = JoystickType.Wii;
            return true;
        }


       



        /// <summary>
        /// Connect to Xbox controller
        /// </summary>
        /// <param name="path">path to joystick</param>
        /// <returns>true if connect success</returns>
        private bool ConnectToXBoxController(string path)
        {
            // Check if it exist.
            if (!File.Exists(path))
            {
                return false;
            }

            //  Remember this path
            Path = path;

            //  start up the thread
            RunPollingThread = true;
            //  path will be opened in the thread
            PollingThread = new Thread(new ThreadStart(PollingThreadRun));
            PollingThread.Start();

            Type = JoystickType.XBox;

            return true;
        }



        /// <summary>
        /// Joystick Changed - Linux file path handling
        /// generate the event for client handlers
        /// </summary>
        /// <param name="data"></param>
        protected void SignalJoystickChange(Joystick data)
        {
            switch (Type)
            {
               

                case JoystickType.XBox:
                    if (XboxJoystickEventHandler != null)
                        XboxJoystickEventHandler(this, new XBoxJoystickEventArgs(new XBoxJoystickReading(data)));
                    break;
            }
        }


        //  Polling Thread for Linux Joystick
        //
        FileStream fs = null;
        public bool RunPollingThread { get; protected set; }
        private Thread PollingThread;

        /// <summary>
        /// Polling Thread
        /// TODO - this has a flaw in it, fs.Read is sync operation, and won't break until a read, 
        /// therefore this thread will lock until the xbox controller sends one last signal to read
        /// </summary>
        public void PollingThreadRun()
        {
            try
            {
                // Read loop.
                using (fs = new FileStream(Path, FileMode.Open))
                {
                    byte[] buff = new byte[8];
                    Joystick j = new Joystick();
                    //fs.ReadTimeout = 1000;

                    while (RunPollingThread)
                    {
                        try
                        {
                            // Read 8 bytes from file and analyze.
                            if (fs.Read(buff, 0, 8) != 0)
                            {
                                //  parse the change 
                                j.DetectChange(buff);

                                //  signal the event handler
                                SignalJoystickChange(j);
                            }
                        }
                        catch (Exception e)
                        {
                            //  Catch thread run interrupt exception and exit
                            if (!RunPollingThread)
                                return;
                            Console.WriteLine($"Exception in PollingThreadRun {e}");
                        }
                    }
                    fs.Close();
                    fs = null;
                }
            }
            catch (Exception e)
            {
                RunPollingThread = false;
            }
        }
    }
}
