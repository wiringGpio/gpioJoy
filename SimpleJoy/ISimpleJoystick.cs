using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;


/// SimpleJoy
/// super simple interface that handles joystick data for ev3Cpanel solution
/// works with XBox on Linux, using code from this post http://mpolaczyk.pl/raspberry-pi-mono-c-joystick-handler/
/// and Wiimote on Windows using the WiimoteLib
/// 

namespace SimpleJoy
{
    /// <summary>
    /// XBox joystick reading interface
    /// </summary>
    public interface IXBoxJoystickReading
    {
        Vector2D LeftStick { get; }
        Vector2D RightStick { get; }
        double LeftTrigger { get; }
        double RightTrigger { get; }
        bool ABtn { get; }
        bool BBtn { get; }
        bool XBtn { get; }
        bool YBtn { get; }
        bool LeftBumper { get; }
        bool RightBumper { get; }
        bool BackBtn { get; }
        bool StartBtn { get; }
        bool HomeBtn { get; }
        bool LeftStickBtn { get; }
        bool RightStickBtn { get; }

        bool DpadUp { get; }
        bool DpadDown { get; }
        bool DpadLeft { get; }
        bool DpadRight { get; }
        bool Dpad { get; }
    }

    /// <summary>
    /// XBox joystick handling class
    /// </summary>
    public class XBoxJoystickReading : IXBoxJoystickReading
    {
        public static double XBOXMAXJOY = (32768.0);
        public static double XBOX2MAXJOY = (32768.0*2.0);

        public XBoxJoystickReading()
        {

        }

        public XBoxJoystickReading(Joystick data)
        {
            //  simple check that we have at least as much data as we will parse below
            //  data from wired xbox controller has dbad as axis 6 and 7
            if (data.Axis.Count == 8 && data.Button.Count == 11)
            {
                LeftStick = new Vector2D((double)data.Axis[0] / XBOXMAXJOY, -1.0 * (double)data.Axis[1] / XBOXMAXJOY);
                double trigger = (double)data.Axis[2] + XBOXMAXJOY;
                LeftTrigger = trigger / XBOX2MAXJOY;
                RightStick = new Vector2D((double)data.Axis[3] / XBOXMAXJOY, -1.0 * (double)data.Axis[4] / XBOXMAXJOY);
                trigger = (double)data.Axis[5] + XBOXMAXJOY;
                RightTrigger = trigger / XBOX2MAXJOY;

                //  DPad comes in like an axis, we convert to to bool
                DpadUp = (data.Axis[7] < - 10);
                DpadDown = (data.Axis[7] > 10);
                DpadLeft = (data.Axis[6] < -10);
                DpadRight = (data.Axis[6] > 10);

                ABtn = data.Button[0];
                BBtn = data.Button[1];
                XBtn = data.Button[2];
                YBtn = data.Button[3];
                LeftBumper = data.Button[4];
                RightBumper = data.Button[5];
                BackBtn = data.Button[6];
                StartBtn = data.Button[7];
                HomeBtn = data.Button[8];
                LeftStickBtn = data.Button[9];
                RightStickBtn = data.Button[10];
            }

            //  data from HDE wireless adapter has dpad as buttons 11 - 14
            if (data.Axis.Count == 6 && data.Button.Count == 15)
            {
                LeftStick = new Vector2D((double)data.Axis[0] / XBOXMAXJOY, -1.0 * (double)data.Axis[1] / XBOXMAXJOY);
                double trigger = (double)data.Axis[2] + XBOXMAXJOY;
                LeftTrigger = trigger / XBOX2MAXJOY;
                RightStick = new Vector2D((double)data.Axis[3] / XBOXMAXJOY, -1.0 * (double)data.Axis[4] / XBOXMAXJOY);
                trigger = (double)data.Axis[5] + XBOXMAXJOY;
                RightTrigger = trigger / XBOX2MAXJOY;

                //  DPad comes in like an axis, we convert to to bool
                DpadUp = data.Button[13];
                DpadDown = data.Button[14];
                DpadLeft = data.Button[11];
                DpadRight = data.Button[12];

                ABtn = data.Button[0];
                BBtn = data.Button[1];
                XBtn = data.Button[2];
                YBtn = data.Button[3];
                LeftBumper = data.Button[4];
                RightBumper = data.Button[5];
                BackBtn = data.Button[6];
                StartBtn = data.Button[7];
                HomeBtn = data.Button[8];
                LeftStickBtn = data.Button[9];
                RightStickBtn = data.Button[10];
            }

        }

        //  properties holding the xbox gamepad values
        public Vector2D LeftStick { get; protected set; }
        public Vector2D RightStick { get; protected set; }
        public double LeftTrigger { get; protected set; }
        public double RightTrigger { get; protected set; }
        public bool ABtn { get; protected set; }
        public bool BBtn { get; protected set; }
        public bool XBtn { get; protected set; }
        public bool YBtn { get; protected set; }
        public bool LeftBumper { get; protected set; }
        public bool RightBumper { get; protected set; }
        public bool BackBtn { get; protected set; }
        public bool StartBtn { get; protected set; }
        public bool HomeBtn { get; protected set; }
        public bool LeftStickBtn { get; protected set; }
        public bool RightStickBtn { get; protected set; }

        public bool Dpad { get { return (DpadUp || DpadDown || DpadLeft || DpadRight); } }
        public bool DpadUp { get; protected set; }
        public bool DpadDown { get; protected set; }
        public bool DpadLeft { get; protected set; }
        public bool DpadRight { get; protected set; }
    }




}
