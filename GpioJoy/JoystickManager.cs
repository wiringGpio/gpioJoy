using System;
using System.Collections.Generic;
using SimpleJoy;
using MathNet.Spatial;
using System.Windows.Forms;
using GpioManagerObjects;
using System.Reflection;
using System.Linq;

namespace GpioJoy
{
    public class FunctionAssignmentContainer
    {
        public FunctionAssignmentContainer(MethodInfo method, JoystickManager jsManager)
        {
            Method = method;
            ActiveConfiguration = false;
            JsManager = jsManager;
        }

        public MethodInfo Method;
        public bool ActiveConfiguration;
        public JoystickManager JsManager;

        public virtual void HandleMouseDown(object sender, MouseEventArgs e)
        {
            return;
        }
        public virtual void HandleMouseUp(object sender, MouseEventArgs e)
        {
            return;
        }
    }

    public class FunctionButtonAssignmentContainer : FunctionAssignmentContainer
    {
        public FunctionButtonAssignmentContainer(MethodInfo method, Button control, JoystickManager jsManager) : base(method, jsManager)
        {
            if ( control != null )
            {
                control.MouseUp += HandleMouseUp;
                control.MouseDown += HandleMouseDown;
            }
        }


        public override void HandleMouseDown(object sender, MouseEventArgs e)
        {
            if (ActiveConfiguration)
                Method.Invoke(JsManager, new object[] { true });
        }
        public override void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (ActiveConfiguration)
                Method.Invoke(JsManager, new object[] { false });
        }
    }

    public class FunctionStickAssignmentContainer : FunctionAssignmentContainer
    {
        public FunctionStickAssignmentContainer(MethodInfo method, TrackBar control, JoystickManager jsManager) : base(method, jsManager)
        {
            Control = control;
            Control.MouseUp += HandleMouseUp;
        }

        TrackBar Control;


       
        public override void HandleMouseUp(object sender, MouseEventArgs e)
        {
            if (ActiveConfiguration)
                Method.Invoke(JsManager, new object[] { (double)Control.Value/(double)Control.Maximum });
        }
    }


    public delegate void StateChangedEvent(object sender, EventArgs e);


    /// <summary>
    /// Joystick Manager Class
    /// </summary>
    public partial class JoystickManager
    {
        //  Events
        public event StateChangedEvent OnStateChanged;

        //  Public Interface
        //

        /// <summary>
        /// Current configuration name
        /// </summary>
        public string ConfigName
        {
            get
            {
                if (CurrentControlAssignments == null)
                    return "";

                return ControlAssignments.FirstOrDefault(x => x.Value == CurrentControlAssignments).Key;
            }
        }

        /// <summary>
        /// Shut Down
        /// </summary>
        public void ShutDown()
        {
            //  clean up joystick
            if (Joystick.IsConnected)
                Joystick.Disconnect();
        }


        /// <summary>
        /// Create configuration by name
        /// </summary>
        public void CreateConfiguration(string configName)
        {
            if ( ControlAssignments.ContainsKey(configName) || FunctionAssignments.ContainsKey(configName))
            {
                return;
            }

            ControlAssignments[configName] = new List<JoystickInput>();
            ControlAssignmentsList.Add(ControlAssignments[configName]);

            FunctionAssignments[configName] = new Dictionary<JoystickControl, List<FunctionAssignmentContainer>>();
            FunctionAssignmentsList.Add(FunctionAssignments[configName]);
        }


        /// <summary>
        /// Toggle configuration forward or back
        /// </summary>
        public void ToggleConfiguration(int direction)
        {
            if (CurrentControlAssignments != null)
            {
                //  get the index of the current configuration
                int index = ControlAssignmentsList.IndexOf(CurrentControlAssignments);
                int nextIndex = index + direction;

                if (nextIndex >= ControlAssignmentsList.Count)
                {
                    nextIndex = 0;
                }
                else if (nextIndex < 0)
                {
                    nextIndex = ControlAssignmentsList.Count-1;
                }

                SetCurrentAssignmentsInactive();
                CurrentControlAssignments = ControlAssignmentsList[nextIndex];
                CurrentFunctionAssignments = FunctionAssignmentsList[nextIndex];
                SetCurrentAssignmentsActive();
            }
        }

        /// <summary>
        /// Go to home configuration (first one loaded)
        /// </summary>
        public void HomeConfiguration()
        {
            if (CurrentControlAssignments != null && ControlAssignmentsList.Count > 0)
            {
                SetCurrentAssignmentsInactive();
                CurrentControlAssignments = ControlAssignmentsList[0];
                CurrentFunctionAssignments = FunctionAssignmentsList[0];
                SetCurrentAssignmentsActive();
            }
        }


        /// <summary>
        /// Set configuration by name
        /// </summary>
        public void SetConfiguration(string configName)
        {
            SetCurrentAssignmentsInactive();
            SetControlAssignments(configName);
            SetFunctionAssignments(configName);
            SetCurrentAssignmentsActive();
        }

                      
        /// <summary>
        /// Set control assignments for configuraiton name
        /// </summary>
        public void SetControlAssignments(string configName)
        {
            try
            {
                List<JoystickInput> assignments;
                if (ControlAssignments.TryGetValue(configName, out assignments))
                {
                    CurrentControlAssignments = assignments;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"SetControlAssignments exception: {e}");
                throw e;
            }
        }

      

        /// <summary>
        /// Set function assignments for configuration name
        /// </summary>
        public void SetFunctionAssignments(string configName)
        {
            Dictionary<JoystickControl, List<FunctionAssignmentContainer>> assignments;
            if (FunctionAssignments.TryGetValue(configName, out assignments))
            {
                CurrentFunctionAssignments = assignments;
            }
        }


        /// <summary>
        /// Set the control labels for the current assignments
        /// </summary>
        public void SetControlLabels()
        {
            if (CurrentControlAssignments != null)
            {
                foreach (var nextControl in CurrentControlAssignments)
                {
                    nextControl.SetupControlUi();
                }
            }
        }



        /// <summary>
        /// Constructor
        /// </summary>
        public JoystickManager(GpioManager pinManager)
        {
            ControlAssignments = new Dictionary<string, List<JoystickInput>>();
            ControlAssignmentsList = new List<List<JoystickInput>>();
            CurrentControlAssignments = null;

            FunctionAssignments = new Dictionary<string, Dictionary<JoystickControl, List<FunctionAssignmentContainer>>>();
            FunctionAssignmentsList = new List<Dictionary<JoystickControl, List<FunctionAssignmentContainer>>>();
            CurrentFunctionAssignments = null;

            PinManager = pinManager;

            Joystick = new SimpleJoystick();
            Joystick.XboxJoystickEventHandler += Joystick_XboxJoystickEventHandler;


        }

        //  References to objects
        public SimpleJoystick Joystick { get; protected set; }
        //  Reference to the pin  manager
        public GpioManager PinManager { get; protected set; }


        //  Joystick Pin Control Assignments
        //  dictioinary of configuration name to list of pin control assignments
        protected Dictionary<string, List<JoystickInput>> ControlAssignments { get; set; }
        //  store the different configuration assignments in an ordered list to enable toggle
        List<List<JoystickInput>> ControlAssignmentsList;
        //  reference to the current configuration control assignments
        List<JoystickInput> CurrentControlAssignments;

        //  Joystick function assignments
        //  dictionary of configuration name to list of function assignments
        protected Dictionary<string, Dictionary<JoystickControl, List<FunctionAssignmentContainer>>> FunctionAssignments { get; set; }
        //  store different configurations in a list to enable toggle
        List<Dictionary<JoystickControl, List<FunctionAssignmentContainer>>> FunctionAssignmentsList;
        //  reference to the current configuration function assignments
        Dictionary<JoystickControl, List<FunctionAssignmentContainer>> CurrentFunctionAssignments;

        /// <summary>
        /// Set current control assignments as inactive
        /// active flag is used to determine if MouseUp / MouseDown is to be handled by this assignment
        /// </summary>
        void SetCurrentAssignmentsInactive()
        {
            if (CurrentControlAssignments != null)
            {
                foreach (var nextControl in CurrentControlAssignments)
                {
                    nextControl.Active = false;
                }
            }

            if ( CurrentFunctionAssignments != null )
            {
                foreach ( var nextFnAssignment in CurrentFunctionAssignments)
                {
                    foreach ( var nextFn in nextFnAssignment.Value)
                    {
                        nextFn.ActiveConfiguration = false;
                    }
                }
            }
        }


        /// <summary>
        /// Set current control assignments as active
        /// active flag is used to determine if MouseUp / MouseDown is to be handled by this assignment
        /// </summary>
        void SetCurrentAssignmentsActive()
        {
            if (CurrentControlAssignments != null)
            {
                foreach (var nextControl in CurrentControlAssignments)
                {
                    nextControl.Active = true;
                }
            }

            if (CurrentFunctionAssignments != null)
            {
                foreach (var nextFnAssignment in CurrentFunctionAssignments)
                {
                    foreach (var nextFn in nextFnAssignment.Value)
                    {
                        nextFn.ActiveConfiguration = true;
                    }
                }
            }
        }




        /// <summary>
        /// XBox Controller event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Joystick_XboxJoystickEventHandler(object sender, SimpleJoy.XBoxJoystickEventArgs e)
        {
            if (CurrentControlAssignments != null)
            {
                // JoystickUserInterfaceControls(e);
                foreach (var nextControlAssignment in CurrentControlAssignments)
                {
                    switch (nextControlAssignment.Assignment)
                    {
                        case JoystickControl.LeftStickUp:
                        case JoystickControl.LeftStickDown:
                            nextControlAssignment.ProcessInput(e.Data.LeftStick.Y, true);
                            break;
                        case JoystickControl.LeftStickRight:
                        case JoystickControl.LeftStickLeft:
                            nextControlAssignment.ProcessInput(e.Data.LeftStick.X, true);
                            break;

                        case JoystickControl.LeftTrigger:
                            nextControlAssignment.ProcessInput(e.Data.LeftTrigger, true);
                            break;

                        case JoystickControl.RightStickUp:
                        case JoystickControl.RightStickDown:
                            nextControlAssignment.ProcessInput(e.Data.RightStick.Y, true);
                            break;

                        case JoystickControl.RightStickRight:
                        case JoystickControl.RightStickLeft:
                            nextControlAssignment.ProcessInput(e.Data.RightStick.X, true);
                            break;

                        case JoystickControl.RightTrigger:
                            nextControlAssignment.ProcessInput(e.Data.RightTrigger, true);
                            break;
                        //
                        case JoystickControl.ABtn:
                            nextControlAssignment.ProcessInput(e.Data.ABtn, true);
                            break;
                        case JoystickControl.BBtn:
                            nextControlAssignment.ProcessInput(e.Data.BBtn, true);
                            break;
                        case JoystickControl.XBtn:
                            nextControlAssignment.ProcessInput(e.Data.XBtn, true);
                            break;
                        case JoystickControl.YBtn:
                            nextControlAssignment.ProcessInput(e.Data.YBtn, true);
                            break;
                        case JoystickControl.LeftBumper:
                            nextControlAssignment.ProcessInput(e.Data.LeftBumper, true);
                            break;
                        case JoystickControl.RightBumper:
                            nextControlAssignment.ProcessInput(e.Data.RightBumper, true);
                            break;
                        case JoystickControl.BackBtn:
                            nextControlAssignment.ProcessInput(e.Data.BackBtn, true);
                            break;
                        case JoystickControl.StartBtn:
                            nextControlAssignment.ProcessInput(e.Data.StartBtn, true);
                            break;
                        case JoystickControl.HomeBtn:
                            nextControlAssignment.ProcessInput(e.Data.HomeBtn, true);
                            break;
                        case JoystickControl.LeftStickBtn:
                            nextControlAssignment.ProcessInput(e.Data.LeftStickBtn, true);
                            break;
                        case JoystickControl.RightStickBtn:
                            nextControlAssignment.ProcessInput(e.Data.RightStickBtn, true);
                            break;
                        case JoystickControl.DpadUp:
                            nextControlAssignment.ProcessInput(e.Data.DpadUp, true);
                            break;
                        case JoystickControl.DpadDown:
                            nextControlAssignment.ProcessInput(e.Data.DpadDown, true);
                            break;
                        case JoystickControl.DpadLeft:
                            nextControlAssignment.ProcessInput(e.Data.DpadLeft, true);
                            break;
                        case JoystickControl.DpadRight:
                            nextControlAssignment.ProcessInput(e.Data.DpadRight, true);
                            break;
                    }
                }
            }

            if (CurrentFunctionAssignments != null)
            {
                // now do the function assignments
                foreach (var nextFnAssignment in CurrentFunctionAssignments)
                {
                    switch (nextFnAssignment.Key)
                    {
                        case JoystickControl.LeftStickUp:
                        case JoystickControl.LeftStickDown:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.LeftStick.Y });
                            }
                            break;
                        case JoystickControl.LeftStickRight:
                        case JoystickControl.LeftStickLeft:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.LeftStick.X });
                            }
                            break;

                        case JoystickControl.LeftTrigger:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.LeftTrigger });
                            }
                            break;

                        case JoystickControl.RightStickUp:
                        case JoystickControl.RightStickDown:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.RightStick.Y });
                            }
                            break;

                        case JoystickControl.RightStickRight:
                        case JoystickControl.RightStickLeft:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.RightStick.X });
                            }
                            break;

                        case JoystickControl.RightTrigger:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.RightTrigger });
                            }
                            break;
                        //
                        case JoystickControl.ABtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.ABtn });
                            }
                            break;
                        case JoystickControl.BBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.BBtn });
                            }
                            break;
                        case JoystickControl.XBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.XBtn });
                            }
                            break;
                        case JoystickControl.YBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.YBtn });
                            }
                            break;
                        case JoystickControl.LeftBumper:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.LeftBumper });
                            }
                            break;
                        case JoystickControl.RightBumper:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.RightBumper });
                            }
                            break;
                        case JoystickControl.BackBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.BackBtn });
                            }
                            break;
                        case JoystickControl.StartBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.StartBtn });
                            }
                            break;
                        case JoystickControl.HomeBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.HomeBtn });
                            }
                            break;
                        case JoystickControl.LeftStickBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.LeftStickBtn });
                            }
                            break;
                        case JoystickControl.RightStickBtn:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.RightStickBtn });
                            }
                            break;
                        case JoystickControl.DpadUp:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.DpadUp });
                            }
                            break;
                        case JoystickControl.DpadDown:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.DpadDown });
                            }
                            break;
                        case JoystickControl.DpadLeft:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.DpadLeft });
                            }
                            break;
                        case JoystickControl.DpadRight:
                            foreach (var nextFn in nextFnAssignment.Value)
                            {
                                nextFn.Method.Invoke(this, new object[] { e.Data.DpadRight });
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Create a joystick assignment object, and connect it to the right control on the UI form
        /// This case is for simple output pin, or pwm pin
        /// </summary>
        public JoystickInput AddJoystickAssignment(string configName, MainForm form, GpioPinWrapperJs pin, string displayName, JoystickControl assignment, double scale, bool reverse)
        {
            JoystickInput newInput = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    //  create a new input for this stick (vector) assignment
                    newInput = new JoystickPwmStickInput(pin, displayName, form.GetLabelForStickAssignment(assignment), form.GetControlForStickAssignment(assignment), assignment, scale, reverse);
                    //  add the assignment to the pin wrapper
                    pin.AddJoystickAssignment(newInput);
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    //  create a new input for this button (boolean) assignment
                    newInput = new JoystickButtonInput(pin, displayName, form.GetLabelForStickAssignment(assignment), form.GetControlForButtonAssignment(assignment), assignment);
                    //  add the assignment to the pin wrapper
                    pin.AddJoystickAssignment(newInput);
                    break;
            }
            
            ControlAssignments[configName].Add(newInput);

            return newInput;
        }

        /// <summary>
        /// Create a joystick assignment object, and connect it to the right control on the UI form
        /// This case is for stepper driver pin collection
        /// </summary>
        public JoystickInput AddJoystickAssignment(string configName, MainForm form, StepperWrapper stepper, int direction, JoystickControl assignment)
        {
            JoystickInput newInput = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    //  create a new input for this stick (vector) assignment
                    newInput = new JoystickStepperStickInput(stepper, direction, form.GetLabelForStickAssignment(assignment), form.GetControlForStickAssignment(assignment), assignment);
                    //  add the assignment to all pins used for this stepper motor
                    foreach (var nextPin in stepper.Pins)
                    {
                        var nextPinWrapper = (GpioPinWrapperJs)nextPin;
                        nextPinWrapper.AddJoystickAssignment(newInput);
                    }
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    //  Stepper motor button inputs do not have context (yet?)
                    break;
            }
            
            ControlAssignments[configName].Add(newInput);
            return newInput;
        }


        /// <summary>
        /// Create a joystick assignment object, and connect it to the right control on the UI form
        /// This case is for stepper driver pin collection
        /// </summary>
        public JoystickInput AddJoystickAssignment(string configName, MainForm form, HBridgeWrapper hbridge, int direction, JoystickControl assignment, double scale = 1.0)
        {
            JoystickInput newInput = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    //  create a new input for this stick (vector) assignment
                    newInput = new JoystickHBridgeStickInput(hbridge, direction, form.GetLabelForStickAssignment(assignment), form.GetControlForStickAssignment(assignment), assignment, scale);
                    //  add the assignment to all pins used for this stepper motor
                    foreach (var nextPin in hbridge.Pins)
                    {
                        var nextPinWrapper = (GpioPinWrapperJs)nextPin;
                        nextPinWrapper.AddJoystickAssignment(newInput);
                    }
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    //  Stepper motor button inputs do not have button context (yet?)
                    break;
            }

            ControlAssignments[configName].Add(newInput);
            return newInput;
        }


        /// <summary>
        /// Create a joystick assignment object, and connect it to the right control on the UI form
        /// This case is for stepper driver pin collection
        /// </summary>
        public JoystickInput AddJoystickAssignment(string configName, MainForm form, ServoWrapper servo, int direction, JoystickControl assignment)
        {
            JoystickInput newInput = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    //  create a new input for this stick (vector) assignment
                    newInput = new JoystickServoStickInput(servo, direction, form.GetLabelForStickAssignment(assignment), form.GetControlForStickAssignment(assignment), assignment);
                    //  add the assignment to all pins used for this stepper motor
                    var pin = (GpioPinWrapperJs)servo.Pin;
                    pin.AddJoystickAssignment(newInput);
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    //  Stepper motor button inputs do not have button context (yet?)
                    break;
            }

            ControlAssignments[configName].Add(newInput);
            return newInput;
        }


        public JoystickInput AddJoystickAssignment(string configName, MainForm form, SevenSegDisplayWrapper display, int direction, JoystickControl assignment)
        {
            JoystickInput newInput = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    //  create a new input for this stick (vector) assignment
                    newInput = new JoystickSevenSegDisplay(display, direction, assignment);
                    //  add the assignment to all pins used for this seven segment display
                    foreach (var nextPin in display.Pins)
                    {
                        var nextPinWrapper = (GpioPinWrapperJs)nextPin;
                        nextPinWrapper.AddJoystickAssignment(newInput);
                    }
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    //  seven seg display button inputs do not have button context (yet?)
                    break;
            }

            ControlAssignments[configName].Add(newInput);
            return newInput;
        }


        public void AddFunctionAssignment(string configName, MainForm form, JoystickControl assignment, MethodInfo method)
        {
            if (!FunctionAssignments[configName].ContainsKey(assignment))
            {
                FunctionAssignments[configName][assignment] = new List<FunctionAssignmentContainer>();
            }

            FunctionAssignmentContainer newContainer = null;

            switch (assignment)
            {
                case JoystickControl.LeftStickUp:
                case JoystickControl.LeftStickDown:
                case JoystickControl.LeftStickRight:
                case JoystickControl.LeftStickLeft:
                case JoystickControl.LeftTrigger:
                case JoystickControl.RightStickUp:
                case JoystickControl.RightStickDown:
                case JoystickControl.RightStickRight:
                case JoystickControl.RightStickLeft:
                case JoystickControl.RightTrigger:
                    newContainer = new FunctionStickAssignmentContainer(method, form.GetControlForStickAssignment(assignment), this);
                    break;
                //
                case JoystickControl.ABtn:
                case JoystickControl.BBtn:
                case JoystickControl.XBtn:
                case JoystickControl.YBtn:
                case JoystickControl.LeftBumper:
                case JoystickControl.RightBumper:
                case JoystickControl.BackBtn:
                case JoystickControl.StartBtn:
                case JoystickControl.HomeBtn:
                case JoystickControl.LeftStickBtn:
                case JoystickControl.RightStickBtn:
                case JoystickControl.DpadUp:
                case JoystickControl.DpadDown:
                case JoystickControl.DpadLeft:
                case JoystickControl.DpadRight:
                    newContainer = new FunctionButtonAssignmentContainer(method, form.GetControlForButtonAssignment(assignment), this);
                    break;
            }


            FunctionAssignments[configName][assignment].Add(newContainer);
        }

    }
}
