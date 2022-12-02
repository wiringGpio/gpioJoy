using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace SimpleJoy
{
    //  created using code from this post http://mpolaczyk.pl/raspberry-pi-mono-c-joystick-handler/
    public class Joystick
    {
        public Joystick()
        {
            Button = new Dictionary<byte, bool>();
            Axis = new Dictionary<byte, short>();
        }

        enum STATE : byte { PRESSED = 0x01, RELEASED = 0x00 }
        enum TYPE : byte { AXIS = 0x02, BUTTON = 0x01 }
        enum MODE : byte { CONFIGURATION = 0x80, VALUE = 0x00 }

        /// <summary>
        /// Buttons collection, key: address, bool: value
        /// </summary>
        public Dictionary<byte, bool> Button;

        /// <summary>
        /// Axis collection, key: address, short: value
        /// </summary>
        public Dictionary<byte, short> Axis;

        /// <summary>
        /// Function recognizes flags in buffer and modifies value of button, axis or configuration.
        /// Every new buffer changes only one value of one button/axis. Joystick object have to remember all previous values.
        /// </summary>
        public void DetectChange(byte[] buff)
        {
            // If configuration
            if (checkBit(buff[6], (byte)MODE.CONFIGURATION))
            {
                if (checkBit(buff[6], (byte)TYPE.AXIS))
                {
                    // Axis configuration, read address and register axis
                    byte key = (byte)buff[7];
                    if (!Axis.ContainsKey(key))
                    {
                        Axis.Add(key, 0);
                        return;
                    }
                }
                else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                {
                    // Button configuration, read address and register button
                    byte key = (byte)buff[7];
                    if (!Button.ContainsKey(key))
                    {
                        Button.Add((byte)buff[7], false);
                        return;
                    }
                }
            }

            // If new button/axis value
            if (checkBit(buff[6], (byte)TYPE.AXIS))
            {
                // Axis value, decode U2 and save to Axis dictionary.
                short value = BitConverter.ToInt16(new byte[2] { buff[4], buff[5] }, 0);
                Axis[(byte)buff[7]] = value;
                return;
            }
            else if (checkBit(buff[6], (byte)TYPE.BUTTON))
            {
                // Bytton value, decode value and save to Button dictionary.
                Button[(byte)buff[7]] = buff[4] == (byte)STATE.PRESSED;
                return;
            }
        }

        /// <summary>
        /// Checks if bits that are set in flag are set in value.
        /// </summary>
        bool checkBit(byte value, byte flag)
        {
            byte c = (byte)(value & flag);
            return c == (byte)flag;
        }
    }
}
