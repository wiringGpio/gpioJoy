using System;
using System.Collections.Generic;
using System.Windows.Forms;
using wiringGpioExtensions;
using GpioManagerObjects;

namespace GpioJoy
{

    public partial class GpioTab : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GpioTab()
        {
            InitializeComponent();
           
            _pinUi = new Dictionary<int, CheckBox>();
        }


        /// <summary>
        /// Initialize Function
        /// </summary>
        public void InitializeGpioTab(GpioManager pinManager)
        {
            _pinManager = pinManager;
            _pinManager.GpioEvents += GpioEvent;

            foreach (var nextPinNumber in _pinManager.GetAvailablePins())
            {
                comboBoxSelectPin.Items.Add(nextPinNumber.PinNumber);
            }

            SetupUiForPins();
            SetUiStateForAll();

            ShowPinOutputControls(false, null);
        }

        //  Reference to the pin manager
        GpioManager _pinManager;

        //  Map of check boxes to pin index
        Dictionary<int, CheckBox> _pinUi;

        //  Event Handler
        void GpioEvent(object sender, GpioEventArgs e)
        {
            SetUiStateForPin(e.PinNumber);
        }


        /// <summary>
        /// Map the UI controls to pin index number
        /// </summary>
        private void SetupUiForPins()
        {
            foreach (var nextPin in _pinManager.GetAvailablePins())
            {
                //  extension pins don't have combo box UI
                if (nextPin.PinNumber > 40)
                    continue;

                string checkBoxName = "checkBox" + nextPin.PinNumber;
                try
                {
                    var control = Controls.Find(checkBoxName, false);
                    if (control.Length == 1)
                    {
                        CheckBox cb = (CheckBox)control[0];
                        _pinUi.Add(nextPin.PinNumber, cb);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("! SetUiStateForPin : " + e.ToString());
                }
            }
        }


        /// <summary>
        /// Refresh UI for all pins
        /// </summary>
        private void SetUiStateForAll()
        {
            foreach (var nextPin in _pinManager.GetAvailablePins())
            {
                SetUiStateForPin(nextPin, true);
            }
        }

     
        /// <summary>
        /// Refresh UI for single pin
        /// Set enable to true for init window, otherwise false
        /// </summary>
        private void SetUiStateForPin(GpioPinWrapper pin, bool enable = false)
        {
            try
            {
                //  do nothing for non header pins
                if (pin.PinNumber > 40)
                    return;

                CheckBox cb = null;

                _pinUi.TryGetValue(pin.PinNumber, out cb);

                if (enable)
                    cb.Enabled = true;

                switch (pin.Mode)
                {
                    case PinMode.Input:
                    case PinMode.Output:
                        SetCheckBoxCheck(cb, pin.Read() == 1);
                        break;

                    case PinMode.PWMOutput:
                        SetCheckBoxCheck(cb, pin.PwmRunning);
                        break;
                }     
            }
            catch (Exception e)
            {
                Console.WriteLine("! SetUiStateForPin : " + e.ToString());
            }
        }


        /// <summary>
        /// Set UI state for single pin by number
        /// </summary>
        private void SetUiStateForPin(int pinNumber)
        {
            SetUiStateForPin(_pinManager.GetPin(pinNumber));
        }


        /// <summary>
        /// Pin check box check changed event
        /// </summary>
        private void checkBoxPin_CheckedChanged(object sender, EventArgs e)
        {
            //  Cast sender to check box
            CheckBox checkedBox = (CheckBox)sender;

            try
            {
                int index;
                Int32.TryParse(checkedBox.Tag.ToString(), out index);
                GpioPinWrapper selectedPin = _pinManager.GetPin(index);

                if (selectedPin != null)
                {
                    switch (selectedPin.Mode)
                    {
                        case PinMode.Input:
                            //  Can not set state of input pin
                            MessageBox.Show("You can not change the value of input pins");
                            SetCheckBoxCheck(checkedBox, false);
                            break;

                        case PinMode.Output:
                            //  on or off for output pin
                            selectedPin.Write(selectedPin.Read() == 1 ? 0 : 1);
                            break;

                        case PinMode.PWMOutput:
                            //  pause or resume PWM
                            if ( ! selectedPin.PwmStarted )
                            {
                                if ( ! selectedPin.HardwarePwm )
                                    MessageBox.Show("Please enter a valid range and Start PWM");
                                SetCheckBoxCheck(checkedBox,false);
                                return;
                            }

                            if (selectedPin.PwmRunning)
                                selectedPin.PwmPause();
                            else
                                selectedPin.PwmResume();
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("!  CheckChange error " + ex.ToString());
            }
        }


        /// <summary>
        /// Pin Combo box selection changed event
        /// </summary>
        private void comboBoxSelectPin_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  get the selected item
            int selectedIndex;
            try
            {
                Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);

                GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

                if (selectedPin != null)
                {
                    ResetRadio(selectedPin);

                    if (selectedPin.HasJoystickAssignment)
                    {
                        checkBoxEnableJs.Show();
                        checkBoxEnableJs.CheckedChanged -= checkBoxEnableJs_CheckedChanged;
                        checkBoxEnableJs.Checked = selectedPin.JoystickAssignmentsEnabled;
                        checkBoxEnableJs.CheckedChanged += checkBoxEnableJs_CheckedChanged;
                    }
                    else
                        checkBoxEnableJs.Hide();

                    ShowPinOutputControls( (selectedPin.Mode == PinMode.PWMOutput || selectedPin.Mode == PinMode.Output), selectedPin);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("! SelectIndexChanged : " + ex.ToString());
            }
        }


        /// <summary>
        /// Pin Mode - Input radio button changed handler
        /// </summary>
        private void radioButtonIn_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxSelectPin.SelectedItem != null)
            {
                if (radioButtonIn.Checked == false)
                    return;

                //  get the selected item
                int selectedIndex;
                try
                {
                    Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);

                    GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

                    if (selectedPin != null)
                    {
                        //  are we already in this mode
                        if (selectedPin.Mode == PinMode.Input)
                            return;

                        if (selectedPin.HasJoystickAssignment)
                        {
                            //  can not change mode here
                            MessageBox.Show("This pin is assigned to a joystick input. You must reload the joystick configuraiton to change the pin mode.");
                            ResetRadio(selectedPin);
                            return;
                        }

                        //  shut down pwm if we are pwm pin
                        if (selectedPin.Mode == PinMode.PWMOutput)
                            selectedPin.PwmStop();
                        //  turn off if we are output pin 
                        if (selectedPin.Mode == PinMode.Output)
                            selectedPin.Write(0);

                        //  set new mode
                        selectedPin.Mode = PinMode.Input;

                        //  update UI
                        ShowPinOutputControls(false, null);

                        //  check box state
                        CheckBox cb = null;
                        if (!_pinUi.TryGetValue(selectedIndex, out cb))
                            return;
                        cb.Checked = selectedPin.Read() == 1;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("! InCheckChanged : " + ex.ToString());

                }
            }
        }


        /// <summary>
        /// Pin Mode - Output radio button changed handler
        /// </summary>
        private void radioButtonOut_CheckedChanged(object sender, EventArgs e)
        {
            if ( comboBoxSelectPin.SelectedItem != null )
            {
                if (radioButtonOut.Checked == false)
                    return;

                //  get the selected item
                int selectedIndex;
                try
                {
                    Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);
                    GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

                    if ( selectedPin != null )
                    {
                        //  are we already in this mode
                        if (selectedPin.Mode == PinMode.Output)
                            return;
                       
                        //  can't change mode of joystick assigned pins
                        if (selectedPin.HasJoystickAssignment)
                        {
                            //  can not change mode here
                            MessageBox.Show("This pin is assigned to a joystick input. You must reload the joystick configuraiton to change the pin mode.");
                            ResetRadio(selectedPin);
                            return;
                        }

                        //  shut down pwm if we are pwm pin
                        if (selectedPin.Mode == PinMode.PWMOutput)
                            selectedPin.PwmStop();

                        //  set new mode
                        selectedPin.Mode = PinMode.Output;

                        ShowPinOutputControls(true, selectedPin);

                        selectedPin.Write(0);
                        CheckBox cb = null;
                        if (!_pinUi.TryGetValue(selectedIndex, out cb))
                            return;
                        cb.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("! OutCheckChanged : " + ex.ToString());
                }
            }
        }


        /// <summary>
        /// Pin Mode - PWM radio button changed handler
        /// </summary>
        private void radioButtonPwm_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBoxSelectPin.SelectedItem != null)
            {
                if (radioButtonPwm.Checked == false)
                    return;

                //  get the selected item
                int selectedIndex;
                try
                {
                    Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);
                    GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

                    if (selectedPin != null)
                    {
                        //  are we already in this mode
                        if (selectedPin.Mode == PinMode.PWMOutput)
                            return;
                        
                        if (selectedPin.HasJoystickAssignment)
                        {
                            //  can not change mode here
                            MessageBox.Show("This pin is assigned to a joystick input. You must reload the joystick configuraiton to change the pin mode.");
                            ResetRadio(selectedPin);
                            return;
                        }

                        //  turn off if we are output pin 
                        if (selectedPin.Mode == PinMode.Output)
                            selectedPin.Write(0);

                        selectedPin.Mode = PinMode.PWMOutput;
                        //selectedPin.Write(1);

                        _pinManager.StartPwm(selectedPin.PinNumber);

                        //  show the controls
                        ShowPinOutputControls(true, selectedPin);

                        CheckBox cb = null;
                        if (!_pinUi.TryGetValue(selectedIndex, out cb))
                            return;
                        cb.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("! InCheckChanged : " + ex.ToString());
                }
            }
        }


        /// <summary>
        /// PWM Set Range Button handler
        /// </summary>
        private void buttonSetPwmRange_Click(object sender, EventArgs e)
        {
            //  start PWM or set range based on selected item
            try
            {
                int selectedIndex = -1;
                Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);
                GpioPinWrapper selectedPin = _pinManager.GetPin(selectedIndex);

                if (selectedPin != null)
                {
                    //  get the PWM range
                    try
                    {
                        int pwmRange = 100;
                        if (!selectedPin.HardwarePwm)
                        {
                            Int32.TryParse(textBoxPwmRange.Text, out pwmRange);

                            if (selectedPin.PwmStarted && pwmRange == selectedPin.PwmRange)
                                return;
                        }

                        if (selectedPin.PwmStart(0, pwmRange) == 0)
                        {
                            try
                            {
                                CheckBox cb = null;
                                _pinUi.TryGetValue(selectedIndex, out cb);
                                SetCheckBoxCheck(cb, true);
                            }
                            catch
                            {

                            }
                        }
                        ShowPinOutputControls(true, selectedPin);
                    }
                    catch
                    {
                        //  TODO - what is a good range
                        MessageBox.Show("Please enter an integer number between 0 - 100");
                    }
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// PWM track bar control changed handler
        /// </summary>
        private void trackBarPwm_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);
                GpioPinWrapper selectedPin = _pinManager.GetPin(selectedIndex);

                if (selectedPin != null)
                {
                    selectedPin.PwmSetValue(((double)trackBarPwm.Value/4096.0));
                    labelPwmValue.Text = selectedPin.PwmValue.ToString();
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// Enable joystick input check box handler
        /// </summary>
        private void checkBoxEnableJs_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = -1;
                Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);
                GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

                if (selectedPin != null)
                {
                    selectedPin.SetJoystickAssignmentEnabled(!selectedPin.JoystickAssignmentsEnabled);
                }
            }
            catch
            {

            }
        }


        /// <summary>
        /// Refresh button handler, update UI state for all pins
        /// </summary>
        private void Refresh_Click(object sender, EventArgs e)
        {
            SetUiStateForAll();
        }


        /// <summary>
        /// Set the proper state of the PWM control UI for pin mode
        /// </summary>
        private void ShowPinOutputControls(bool show, GpioPinWrapper pin)
        {
            if (show)
            {
                switch ( pin.Mode )
                {
                    case PinMode.PWMOutput:
                        {
                            textBoxPwmRange.Show();
                            labelPwmValue.Show();
                            labelPwmRange.Show();
                            trackBarPwm.Show();
                            buttonCenterFreq.Show();

                            if (pin.HardwarePwm)
                            {
                                textBoxPwmRange.Enabled = false;
                                buttonSetPwmRange.Hide();
                            }
                            else
                            {
                                textBoxPwmRange.Enabled = true;
                                buttonSetPwmRange.Show();
                            }

                            checkBoxOnOff.Hide();

                            //  text for start / set range button
                            buttonSetPwmRange.Text = pin.PwmStarted ? "Set Range" : "Start PWM";
                            textBoxPwmRange.Text = pin.PwmRange.ToString();

                            // set current value
                            labelPwmValue.Text = pin.PwmValue.ToString();
                            //  disconnect value changed event to update track bar control
                            trackBarPwm.ValueChanged -= trackBarPwm_ValueChanged;
                            trackBarPwm.Value = (pin.PwmValue/pin.PwmRange)*4096;
                            trackBarPwm.ValueChanged += trackBarPwm_ValueChanged;
                        }
                        break;

                    case PinMode.Output:
                        {
                            buttonCenterFreq.Hide();
                            labelPwmRange.Hide();
                            labelPwmValue.Hide();
                            textBoxPwmRange.Hide();
                            trackBarPwm.Hide();
                            buttonSetPwmRange.Hide();
                            if (pin.PinNumber > 40)
                            {
                                checkBoxOnOff.Show();

                                checkBoxOnOff.CheckedChanged -= checkBoxOnOff_CheckedChanged;
                                checkBoxOnOff.Checked = pin.Read() == 1;
                                checkBoxOnOff.CheckedChanged += checkBoxOnOff_CheckedChanged;
                            }
                            else
                                checkBoxOnOff.Hide();
                        }
                        break;
                }
            }
            else
            {
                labelPwmRange.Hide();
                labelPwmValue.Hide();
                textBoxPwmRange.Hide();
                trackBarPwm.Hide();
                buttonSetPwmRange.Hide();
                checkBoxOnOff.Hide();
                buttonCenterFreq.Show();
            }
        }


        /// <summary>
        /// Set Check Box State, short circuts the handler to update value manually
        /// </summary>
        private void SetCheckBoxCheck(CheckBox cb, bool setCheck)
        {
            cb.CheckedChanged -= checkBoxPin_CheckedChanged;
            cb.Checked = setCheck;
            cb.CheckedChanged += checkBoxPin_CheckedChanged;
        }


        /// <summary>
        /// Reset Radio to current pin mode
        /// </summary>
        private void ResetRadio(GpioPinWrapper selectedPin)
        {
            switch (selectedPin.Mode)
            {
                case PinMode.Input:
                    radioButtonIn.Checked = true;
                    radioButtonOut.Checked = false;
                    radioButtonPwm.Checked = false;
                    break;
                case PinMode.Output:
                    radioButtonIn.Checked = false;
                    radioButtonOut.Checked = true;
                    radioButtonPwm.Checked = false;
                    break;

                case PinMode.PWMOutput:
                    radioButtonIn.Checked = false;
                    radioButtonOut.Checked = false;
                    radioButtonPwm.Checked = true;
                    break;
            }
        }

        /// <summary>
        /// Checkbox on off changed
        /// </summary>
        private void checkBoxOnOff_CheckedChanged(object sender, EventArgs e)
        {
            int selectedIndex = 0;
            Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);

            GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

            if (selectedPin != null)
            {
                int read = selectedPin.Read();
                selectedPin.Write(read == 1 ? 0 : 1);
            }
        }


        /// <summary>
        /// Center servo frequency button
        /// </summary>
        private void buttonCenterFreq_Click(object sender, EventArgs e)
        {
            int selectedIndex = 0;
            Int32.TryParse(comboBoxSelectPin.SelectedItem.ToString(), out selectedIndex);

            GpioPinWrapperJs selectedPin = (GpioPinWrapperJs)_pinManager.GetPin(selectedIndex);

            if (selectedPin != null)
            {
                //  calculate the center tick, servo wants 1.5 milisecond pulse for stationary position
                double freq = _pinManager.PinPwmFrequency(selectedPin.PinNumber);
                double cycleMs = 1000.0 / freq;
                double pulseMs = 1.5;
                //  initial tick for this pwm range
                int initialTick = (int)(selectedPin.PwmRange * pulseMs / cycleMs + 0.5f);
                //  turn this back into a unit vector with the range
                double initialValue = (double)initialTick/selectedPin.PwmRange;

                //  set the servo at the center tick
                selectedPin.PwmSetValue(initialValue);

                //  update the UI
                trackBarPwm.ValueChanged -= trackBarPwm_ValueChanged;
                trackBarPwm.Value = (int)(initialValue * 4096);
                labelPwmValue.Text = ((int)(initialValue * selectedPin.PwmRange)).ToString();
                trackBarPwm.ValueChanged += trackBarPwm_ValueChanged;
            }
        }
    }
}
