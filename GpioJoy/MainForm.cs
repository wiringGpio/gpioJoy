using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using GpioManagerObjects;
using SimpleJoy;
using PlatformHelper;
using System.IO;

namespace GpioJoy
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm(JoystickManager jsManager, GpioManager pinManager)
        {
            //  Winform initialization function
            InitializeComponent();

            _pinManager = pinManager;
            _jsManager = jsManager;

            LoadConfigurations();

            gpioTab.InitializeGpioTab(_pinManager);

            _jsManager.OnStateChanged += JsManager_StateChanged;
            joystickTab1.InitializeJoystickTab(_jsManager);

            InitLabels();
            _jsManager.SetControlLabels();

            labelConfigFile.Text = _jsManager.ConfigName;
        }


        //  The GPIO Pin Manager
        GpioManager _pinManager;
        //  The Joystick
        JoystickManager _jsManager;


        /// <summary>
        /// Load configuration files
        /// </summary>
        private void LoadConfigurations()
        {
            var configFiles = Directory.GetFiles("./Config/", "*.xml", SearchOption.AllDirectories);
            foreach (var nextFile in configFiles)
            {
                LoadConfiguration.LoadConfigFile(Path.Combine(nextFile), this, _pinManager, _jsManager);
            }
        }


        /// <summary>
        /// Joystick state changed, update the control labels
        /// </summary>
        private void JsManager_StateChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate {
                InitLabels();
                _jsManager.SetControlLabels();
                labelConfigFile.Text = _jsManager.ConfigName; 
            });
        }


        /// <summary>
        /// Initialize labels
        /// </summary>
        public void InitLabels()
        {

            labelLSU.Text = "";
            labelLSD.Text = "";
            labelLSR.Text = "";
            labelLSL.Text = "";
            labelLT.Text = "";
            labelRSU.Text = "";
            labelRSD.Text = "";
            labelRSR.Text = "";
            labelRSL.Text = "";
            labelRT.Text = "";

            trackBarLSU.Enabled = false;
            trackBarLSL.Enabled = false;
            trackBarLSD.Enabled = false;
            trackBarLSR.Enabled = false;
            trackBarLT.Enabled = false;
            trackBarRSU.Enabled = false;
            trackBarRSL.Enabled = false;
            trackBarRSD.Enabled = false;
            trackBarRSR.Enabled = false;
            trackBarRT.Enabled = false;

            buttonA.Enabled = false;
            buttonA.Text = "";
            buttonB.Enabled = false;
            buttonB.Text = "";
            buttonX.Enabled = false;
            buttonX.Text = "";
            buttonY.Enabled = false;
            buttonY.Text = "";
            buttonLB.Enabled = false;
            buttonLB.Text = "";
            buttonRB.Enabled = false;
            buttonRB.Text = "";
            buttonBack.Enabled = false;
            buttonBack.Text = "";
            buttonStart.Enabled = false;
            buttonStart.Text = "";
            buttonHome.Enabled = false;
            buttonHome.Text = "";
            buttonLS.Enabled = false;
            buttonLS.Text = "";
            buttonRS.Enabled = false;
            buttonRS.Text = "";
            buttonDPU.Enabled = false;
            buttonDPU.Text = "";
            buttonDPD.Enabled = false;
            buttonDPD.Text = "";
            buttonDPL.Enabled = false;
            buttonDPL.Text = "";
            buttonDPR.Enabled = false;
            buttonDPR.Text = "";
        }


        /// <summary>
        /// Get the track bar control for the control joystick asssignment
        /// </summary>
        public TrackBar GetControlForStickAssignment(JoystickControl assignment)
        {
            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                    return trackBarLSU;
                case JoystickControl.LeftStickDown:
                    return trackBarLSD;
                case JoystickControl.LeftStickRight:
                    return trackBarLSR;
                case JoystickControl.LeftStickLeft:
                    return trackBarLSL;
                case JoystickControl.LeftTrigger:
                    return trackBarLT;
                case JoystickControl.RightStickUp:
                    return trackBarRSU;
                case JoystickControl.RightStickDown:
                    return trackBarRSD;
                case JoystickControl.RightStickRight:
                    return trackBarRSR;
                case JoystickControl.RightStickLeft:
                    return trackBarRSL;
                case JoystickControl.RightTrigger:
                    return trackBarRT;

                default:
                    return null;
            }
        }


        /// <summary>
        /// Get the button control for the control button assignment
        /// </summary>
        public Button GetControlForButtonAssignment(JoystickControl assignment)
        {
            switch (assignment)
            {
                //
                case JoystickControl.ABtn:
                    return buttonA;
                case JoystickControl.BBtn:
                    return buttonB;
                case JoystickControl.XBtn:
                    return buttonX;
                case JoystickControl.YBtn:
                    return buttonY;
                case JoystickControl.LeftBumper:
                    return buttonLB;
                case JoystickControl.RightBumper:
                    return buttonRB;
                case JoystickControl.BackBtn:
                    return buttonBack;
                case JoystickControl.StartBtn:
                    return buttonStart;
                case JoystickControl.HomeBtn:
                    return buttonHome;
                case JoystickControl.LeftStickBtn:
                    return buttonLS;
                case JoystickControl.RightStickBtn:
                    return buttonRS;
                case JoystickControl.DpadUp:
                    return buttonDPU;
                case JoystickControl.DpadDown:
                    return buttonDPD;
                case JoystickControl.DpadLeft:
                    return buttonDPL;
                case JoystickControl.DpadRight:
                    return buttonDPR;

                default:
                    return null;
            }
        }


        /// <summary>
        /// Get the label control for the control joystick assignment 
        /// </summary>
        public Control GetLabelForStickAssignment(JoystickControl assignment)
        {
            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                    return labelLSU;
                case JoystickControl.LeftStickDown:
                    return labelLSD;
                case JoystickControl.LeftStickRight:
                    return labelLSR;
                case JoystickControl.LeftStickLeft:
                    return labelLSL;
                case JoystickControl.LeftTrigger:
                    return labelLT;
                case JoystickControl.RightStickUp:
                    return labelRSU;
                case JoystickControl.RightStickDown:
                    return labelRSD;
                case JoystickControl.RightStickRight:
                    return labelRSR;
                case JoystickControl.RightStickLeft:
                    return labelRSL;
                case JoystickControl.RightTrigger:
                    return labelRT;
                case JoystickControl.ABtn:
                    return buttonA;
                case JoystickControl.BBtn:
                    return buttonB;
                case JoystickControl.XBtn:
                    return buttonX;
                case JoystickControl.YBtn:
                    return buttonY;
                case JoystickControl.LeftBumper:
                    return buttonLB;
                case JoystickControl.RightBumper:
                    return buttonRB;
                case JoystickControl.BackBtn:
                    return buttonBack;
                case JoystickControl.StartBtn:
                    return buttonStart;
                case JoystickControl.HomeBtn:
                    return buttonHome;
                case JoystickControl.LeftStickBtn:
                    return buttonLS;
                case JoystickControl.RightStickBtn:
                    return buttonRS;
                case JoystickControl.DpadUp:
                    return buttonDPU;
                case JoystickControl.DpadDown:
                    return buttonDPD;
                case JoystickControl.DpadLeft:
                    return buttonDPL;
                case JoystickControl.DpadRight:
                    return buttonDPR;
                default:
                    return null;
            }
        }
    }
}
