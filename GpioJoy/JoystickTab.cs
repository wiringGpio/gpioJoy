using PlatformHelper;
using SimpleJoy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GpioJoy
{
    public partial class JoystickTab : UserControl
    {
        public JoystickTab()
        {
            InitializeComponent();
        }

        JoystickManager _jsManager;

        public bool JoyStickConnected { get { return _jsManager.Joystick.IsConnected; } }
        
        /// <summary>
        /// Initialize the available controllers combo box
        /// </summary>
        public void InitializeJoystickTab(JoystickManager jsManager)
        {
            _jsManager = jsManager;

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

            //  setup the joystick disconnect message
            _jsManager.Joystick.JoystickDisconnectHandler += Joystick_JoystickDisconnectHandler;
        }

        static string NoneFound = "none found";


        /// <summary>
        /// Refresh controller paths
        /// </summary>
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
        /// Connect to controller button handler
        /// </summary>
        private void buttonConnectJoystick_Click(object sender, EventArgs e)
        {
            ConnectToController();
        }


        /// <summary>
        /// Connect to controller
        /// </summary>
        private void ConnectToController()
        {
            string selectedPath = "";
            if (!_jsManager.Joystick.IsConnected && PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
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

        
        /// <summary>
        /// Connect to controller background worker
        /// </summary>
        private void backgroundWorkerConnectJoystick_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_jsManager.Joystick.IsConnected)
            {
                e.Result = true;
                _jsManager.Joystick.Disconnect();
            }
            else
            {
                if (PlatformHelper.PlatformHelper.RunningPlatform() == Platform.Linux)
                    e.Result = _jsManager.Joystick.ConnectToJoystick(JoystickType.XBox, "/dev/input/" + e.Argument);     

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

            if (_jsManager.Joystick.IsConnected)
            {
                buttonConnectJoystick.Text = "Disconnect";
                comboBoxJoystickPaths.Enabled = false;
                buttonRefresh.Visible = false;
            }
            else
            {
                comboBoxJoystickPaths.Enabled = true;
                buttonRefresh.Visible = true;
                buttonConnectJoystick.Text = "Connect";
            }
        }


        private void Joystick_JoystickDisconnectHandler(object sender, EventArgs e)
        {
            //  TODO
        }



    }
}
