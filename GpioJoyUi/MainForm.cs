using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using GpioManagerObjects;
using SimpleJoy;
using PlatformHelper;
using System.IO;

namespace GpioJoyUi
{



    public partial class MainForm : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            //  Winform initialization function
            InitializeComponent();

            // Hook up Joystick events
               //  Init Joystick Control
            if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
            {
                List<string> paths = SimpleJoystick.GetJoystickPaths();
                foreach (var nextPath in paths)
                    comboBoxJoystickPaths.Items.Add(nextPath);
                if (paths.Count == 0)
                    comboBoxJoystickPaths.Items.Add(NoneFound);
                comboBoxJoystickPaths.SelectedIndex = 0;
            }
            else
            {
                comboBoxJoystickPaths.Visible = false;
                buttonRefresh.Visible = false;
            }

      
            //  instantiate the Pin Manager
            PinManager = new GpioManager();
            JsManager = new JoystickManager(PinManager);
            JsManager.OnStateChanged += JsManager_StateChanged;
            PinManager.Setup();

            string configFilePath = "";
            if (PlatformHelper.PlatformHelper.RunningPlatform() == PlatformHelper.Platform.Linux)
                configFilePath = "/home/pi/GpioJoy/Config/";
            else
                configFilePath = "./Config/";

         
            var configFiles = Directory.GetFiles(configFilePath, "*.xml", SearchOption.AllDirectories);

            foreach (var nextFile in configFiles)
            {
                InitLabels();
                JsManager.SetControlLabels();
               

                LoadConfiguration.LoadConfigFile(Path.Combine( nextFile), this, PinManager, JsManager);

                labelConfigFile.Text = JsManager.ConfigName;
            }

            JsManager.InitXBoxModelPins();

            //  setup the UI to use the pin manager
            gpioTab.InitializeGpioTab(PinManager);

            //  setup the joystick disconnect message
            JsManager.Joystick.JoystickDisconnectHandler += Joystick_JoystickDisconnectHandler;
        }



        private void JsManager_StateChanged(object sender, EventArgs e)
        {
            
            this.Invoke((MethodInvoker)delegate {
                InitLabels();
                JsManager.SetControlLabels();
                labelConfigFile.Text = JsManager.ConfigName; 
            });

          
        }

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

        /// <summary>
        /// Shut down Robot on main form closing
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            PinManager.ShutDown();
            JsManager.ShutDown();

            
        }


        //  The GPIO Pin Manager
        public GpioManager PinManager { get; protected set; }


        //  The Joystick
        JoystickManager JsManager;
          //
        public bool JoyStickConnected { get { return JsManager.Joystick.IsConnected; } }
        static string NoneFound = "none found";

        int PwmPower = 0;

        /// <summary>
        /// Keyboard Input Handling 
        /// pass keyboard key presses along to the robot
        /// </summary>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
          
            //switch ( (ConsoleKey) e.KeyValue )
            //{
                    
            //    case ConsoleKey.UpArrow:
            //        PwmPower += 10;
            //        if (PwmPower > 100)
            //            PwmPower = 100;
            //        testPin.PwmSetValue(PwmPower);
            //        break;

            //    case ConsoleKey.DownArrow:
            //        PwmPower -= 10;
            //        if (PwmPower < 0)
            //            PwmPower = 0;
            //        testPin.PwmSetValue(PwmPower);
            //        break;

            //    //case ConsoleKey.RightArrow:
            //    //    PwmPower2 += 3;
            //    //    if (PwmPower2 > 30)
            //    //        PwmPower2 = 30;
            //    //    test2.SetPwmPower(PwmPower2);
            //    //    break;

            //    //case ConsoleKey.LeftArrow:
            //    //    PwmPower2 -= 3;
            //    //    if (PwmPower2 < 0)
            //    //        PwmPower2 = 0;
            //    //    test2.SetPwmPower(PwmPower2);
            //    //    break;

            //    //case ConsoleKey.Q:
            //    //    PwmPower3 += 3;
            //    //    if (PwmPower3 > 30)
            //    //        PwmPower3 = 30;
            //    //    test3.SetPwmPower(PwmPower3);
            //    //    break;

            //    //case ConsoleKey.A:
            //    //    PwmPower3 -= 3;
            //    //    if (PwmPower3 < 0)
            //    //        PwmPower3 = 0;
            //    //    test3.SetPwmPower(PwmPower3);
            //    //    break;
                     
            //}
        }


        //  Joystick
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            comboBoxJoystickPaths.Items.Clear();

            List<string> paths = SimpleJoystick.GetJoystickPaths();
            foreach (var nextPath in paths)
                comboBoxJoystickPaths.Items.Add(nextPath);
            if (paths.Count == 0)
                comboBoxJoystickPaths.Items.Add(NoneFound);
        }


        /// <summary>
        /// Check Use Joystick handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConnectJoystick_Click(object sender, EventArgs e)
        {
            string selectedPath = "";
            if (!JsManager.Joystick.IsConnected && PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
            {
                selectedPath = (string)comboBoxJoystickPaths.SelectedItem;
                if (selectedPath == null || selectedPath.Length == 0 || selectedPath == NoneFound)
                {
                    MessageBox.Show("Please select a path for the joystick!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            buttonConnectJoystick.Enabled = false;
            backgroundWorkerConnectJoystick.RunWorkerAsync(selectedPath);
        }
        //
        //  Background worker for connect to joystick
        private void backgroundWorkerConnectJoystick_DoWork(object sender, DoWorkEventArgs e)
        {
            if (JsManager.Joystick.IsConnected)
            {
                e.Result = true;
                JsManager.Joystick.Disconnect();
            }
            else
            {
                if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
                    e.Result = JsManager.Joystick.ConnectToJoystick(JoystickType.XBox, "/dev/input/" + e.Argument);     //  TODO - from UI on this page
               
            }
        }
        //
        private void backgroundWorkerConnectJoystick_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result != true)
            {
                MessageBox.Show("Failed to connect to Joystick !", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            buttonConnectJoystick.Enabled = true;

            if (JsManager.Joystick.IsConnected)
            {
                //  control fo PC build using wiimote - you can't reconnect so hide the control
                if (JsManager.Joystick.IsConnected && PlatformHelper.PlatformHelper.RunningPlatform() == PlatformHelper.Platform.Windows)
                    buttonConnectJoystick.Visible = false;
                else
                    buttonConnectJoystick.Visible = true;

                buttonConnectJoystick.Text = "Disconnect";
                comboBoxJoystickPaths.Visible = false;
                buttonRefresh.Visible = false;
              
            }
            else
            {
                comboBoxJoystickPaths.Visible = true;
                buttonRefresh.Visible = true;
                buttonConnectJoystick.Text = "Connect";
               
            }
        }


        private void Joystick_JoystickDisconnectHandler(object sender, EventArgs e)
        {
            //  TODO
        }

        private void buttonDPL_Click(object sender, EventArgs e)
        {

        }

        private void buttonDPR_Click(object sender, EventArgs e)
        {

        }

        private void buttonBack_Click(object sender, EventArgs e)
        {

        }









        //// Joystick control of UI
        ////
        //long timeSinceTabSwitch = 0;
        //long timeSinceDpadSwitch = 0;
        //long timeSinceYBtn = 0;

        ///// <summary>
        ///// Joystick UI Control
        ///// </summary>
        //void JoystickUserInterfaceControls(SimpleJoy.XBoxJoystickEventArgs e)
        //{
        //    long tickCount = Environment.TickCount;

        //    //  Back and Start Button cycle tab pages
        //    if ((e.Data.StartBtn || e.Data.BackBtn) && (tickCount - timeSinceTabSwitch > 200))
        //    {
        //        timeSinceTabSwitch = tickCount;

        //        int newIndex = tabControlRobot.SelectedIndex + (e.Data.StartBtn ? 1 : -1);
        //        if (newIndex == tabControlRobot.TabPages.Count)
        //            newIndex = 0;
        //        else if (newIndex == -1)
        //            newIndex = tabControlRobot.TabPages.Count - 1;

        //        this.BeginInvoke(new MethodInvoker(delegate()
        //        {
        //            tabControlRobot.SelectedIndex = newIndex;
        //        }));
        //    }

        //    //  DPad selects from program tab (when program tab is visible)
        //    if (e.Data.Dpad && (tickCount - timeSinceDpadSwitch > 200))
        //    {
        //        timeSinceDpadSwitch = tickCount;


        //    }

        //    //  home button changes driving mode
        //    if (e.Data.YBtn && (tickCount - timeSinceYBtn > 1000))
        //    {
        //        timeSinceYBtn = tickCount;


        //    }
        //}







    }
}
