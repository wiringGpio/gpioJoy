using System;
using System.Xml;
using GpioManagerObjects;
using static WiringPiWrapper.MCP;
using WiringPiWrapper;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace GpioJoyUi
{
    class LoadConfiguration
    {

        /// <summary>
        /// Load Config File
        /// loads the pin configuration with joystick assignments from the config.xml file
        /// </summary>
        /// <param name="form">main form</param>
        /// <param name="pinManager">Pin Manager</param>
        /// <param name="jsManager">Joystick Manager</param>
        public static void LoadConfigFile(string path, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            string configName = Path.GetFileNameWithoutExtension(path);
            jsManager.CreateConfiguration(configName);
            
            try
            {
                XmlDocument configFile = new XmlDocument();
                configFile.Load(path);
             

                //  Document Element
                //  <GpioConfig>
                XmlElement root = configFile.DocumentElement;

                //  Load the MCP Chips
                //  <MCPs>
                var mcpsElement = root.SelectSingleNode("MCPs");
                LoadMcps(mcpsElement, form, pinManager, jsManager);

                //  Load the PCA Chips
                //  <PCAs>
                var pcasElement = root.SelectSingleNode("PCAs");
                LoadPcas(pcasElement, form, pinManager, jsManager);

                //  Get the root node for our control pins
                //  <ControlPins>
                var controlPinsNode = root.SelectSingleNode("ControlPins");
                LoadControlPins(configName, controlPinsNode, form, pinManager, jsManager);

                //  Load stepper drivers
                //  <StepperDrivers>
                var steppersElement = root.SelectSingleNode("StepperDrivers");
                LoadStepperDrivers(configName, steppersElement, form, pinManager, jsManager);

                //  Load HBridges
                //  <HBridges>
                var hbridgesElement = root.SelectSingleNode("HBridges");
                LoadHBridgeControllers(configName, hbridgesElement, form, pinManager, jsManager);

                //  Load Displays
                //  <SevenSegDisplays>
                var sevenSegDisplaysElement = root.SelectSingleNode("SevenSegDisplays");
                LoadSevenSegDisplayDrivers(configName, sevenSegDisplaysElement, form, pinManager, jsManager);

                //  Load joystick input funcitons
                var functionsElement = root.SelectSingleNode("JoystickFunctions");
                LoadJoystickFunctions(configName, functionsElement, form, jsManager);

                jsManager.SetConfiguration(configName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }


        /// <summary>
        /// Setup MCP Chip drivers
        /// </summary>
        public static void LoadMcps(XmlNode mcpsElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (mcpsElement == null)
                return;

            //  parse the stepper drivers
            var mcpNodes = mcpsElement.SelectNodes("MCP");
            foreach (XmlNode nextMcp in mcpNodes)
            {
                //  parse this
                /*
                  <MCP>
                    <Type>Mcp230xx</Type>   //  enum McpType
                    <Address>32</Address>   //  address should be in decimal
                    <Base>100</Base>        //  pin base
                  </MCP>
                */

                try
                {
                    var typeNode = nextMcp.SelectSingleNode("Type");
                    if (typeNode == null)
                        continue;

                    McpType type;
                    type = (McpType)Enum.Parse(typeof(McpType), typeNode.InnerText);

                    int address;
                    var addressNode = nextMcp.SelectSingleNode("Address");
                    if (addressNode == null)
                        continue;
                    address = Int32.Parse(addressNode.InnerText);

                    int baseNumber;
                    var baseNode = nextMcp.SelectSingleNode("Base");
                    if (baseNode == null)
                        continue;
                    baseNumber = Int32.Parse(baseNode.InnerText);

                    pinManager.SetupMcp(type, baseNumber, address);
                }
                catch
                {
                    continue;
                }
            }
        }


        /// <summary>
        /// Setup PCA Chip drivers
        /// </summary>
        public static void LoadPcas(XmlNode pcasElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (pcasElement == null)
                return;

            //  parse the stepper drivers
            var pcaNodes = pcasElement.SelectNodes("PCA");
            foreach (XmlNode nextMcp in pcaNodes)
            {
                //  parse this
                /*
                   <PCA>
                    <Address>35</Address>       //  address should be in decimal
                    <Base>300</Base>            //  pin base
                    <Frequency>50</Frequency>   //  frequency in hertz
                   </PCA>
                */

                try
                {
                    int address;
                    var addressNode = nextMcp.SelectSingleNode("Address");
                    if (addressNode == null)
                        continue;
                    address = Int32.Parse(addressNode.InnerText);

                    int baseNumber;
                    var baseNode = nextMcp.SelectSingleNode("Base");
                    if (baseNode == null)
                        continue;
                    baseNumber = Int32.Parse(baseNode.InnerText);

                    int frequency;
                    var freqNode = nextMcp.SelectSingleNode("Frequency");
                    if (freqNode == null)
                        continue;
                    frequency = Int32.Parse(freqNode.InnerText);

                    pinManager.SetupPca(baseNumber, address, frequency);
                }
                catch
                {
                    continue;
                }
            }
        }


        /// <summary>
        /// Load Control Pins
        /// defines how you want to use each pin
        /// </summary>
        public static void LoadControlPins(string configName, XmlNode controlPinsNode, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            //  Parse This
            /*
             
            <Pin>                <Name>SomeName</Name>                <Number>206</Number>                <Mode>Output</Mode>                <Joystick>ABtn</Joystick>              </Pin>                            or this            <Pin>                <Number>207</Number>                <Mode>PWMOutput</Mode>                <Joystick scale="0.5" reverse="true">LeftTrigger</Joystick>     //  PWM joystick vector can have optional scale and reversed attributes              </Pin>             
            //  see ./Config/GpioConfig.xml for more examples and options
             */

            if (controlPinsNode == null)
                return;

            //  <Pin>
            var pinNodes = controlPinsNode.SelectNodes("Pin");
            foreach (XmlNode nextPin in pinNodes)
            {
                //  Pin Number
                //  <Number>
                XmlNode pinNumberNode = nextPin.SelectSingleNode("Number");
                if (pinNumberNode == null)
                    continue;
                int pinNumber;
                try
                {
                    Int32.TryParse(pinNumberNode.InnerText, out pinNumber);
                }
                catch
                {
                    continue; //  Must have pin number
                }

                //  Pin name (optional)
                string name = "Pin " + pinNumber.ToString();
                XmlNode nameNode = nextPin.SelectSingleNode("Name");
                if (nameNode != null)
                    name = nameNode.InnerText;

                //  Pin Mode
                //  <Mode>
                XmlNode modeNode = nextPin.SelectSingleNode("Mode");
                if (modeNode == null)
                    continue;
                GPIO.GPIOpinmode pinMode;
                try
                {
                    Enum.TryParse(modeNode.InnerText, out pinMode);
                }
                catch
                {
                    continue;   // must have mode
                }

                //  creaet a new wrapper for it, depending on the type of the pin it is
                GpioPinWrapperJs newPin = (GpioPinWrapperJs)pinManager.GetPin(pinNumber);
                if (newPin == null)
                {
                    if (pinManager.IsPcaPin(pinNumber))
                    {
                        //  setup a new PCA pin, hardware PWM
                        newPin = new GpioPinWrapperPcaJs(pinNumber, name, true);
                    }
                    else
                    {
                        //  setup a new pin, hardware PWM on raspberry pi pin 12
                        newPin = new GpioPinWrapperJs(pinNumber, name, pinNumber == 12);
                    }

                    //  initialize this pin in the pin manager
                    pinManager.AddPinToManager(newPin);

                    //  we can create a pin object for this now
                    newPin.Mode = pinMode;


                    //  if we are PWM we need range
                    //  <Range>
                    int pwmRange = 0;
                    if (pinMode == GPIO.GPIOpinmode.PWMOutput)
                    {
                        XmlNode rangeNode = nextPin.SelectSingleNode("Range");
                        if (rangeNode != null)
                        {
                            try
                            {
                                Int32.TryParse(rangeNode.InnerText, out pwmRange);
                            }
                            catch
                            {
                                pwmRange = LoadConfiguration.PwmRange(pinNumber, pinManager.IsPcaPin(pinNumber));
                            }
                        }
                        else
                        {
                            pwmRange = LoadConfiguration.PwmRange(pinNumber, pinManager.IsPcaPin(pinNumber));
                        }

                        newPin.PwmStart(0, pwmRange);
                    }
                }

                //  Map to joystick input
                JoystickInput newInput = null;
                //  <Joystick>
                XmlNodeList joystickNodes = nextPin.SelectNodes("Joystick");
                if (joystickNodes != null)
                {

                    JoystickControl assignment;
                    try
                    {
                        foreach (XmlNode joystickNode in joystickNodes)
                        {
                            Enum.TryParse(joystickNode.InnerText, out assignment);
                            var jsElement = (XmlElement)joystickNode;
                            string scaleText = jsElement.GetAttribute("scale");
                            double scale = 1.0;
                            if (scaleText != null && scaleText.Length > 0)
                                scale = double.Parse(scaleText);

                            bool reverse = false;
                            var reverseText = jsElement.GetAttribute("reverse");
                            if (reverseText != null && reverseText.Length > 0)
                                reverse = bool.Parse(reverseText);

                            bool isServo = false;
                            var servoText = jsElement.GetAttribute("servo");
                            if (servoText != null && servoText.Length > 0)
                                isServo = bool.Parse(servoText);

                            int direction = 1;
                            var directionText = jsElement.GetAttribute("direction");
                            if (directionText != null && directionText.Length > 0)
                                direction = int.Parse(directionText);

                            if (isServo)
                            {
                                //  create the servo
                                ServoWrapper servoWrapper = pinManager.GetServoDriver(newPin.PinNumber);
                                if (servoWrapper == null)
                                {
                                    servoWrapper = pinManager.CreateServoDriver(newPin);
                                }
                                newInput = jsManager.AddJoystickAssignment(configName, form, servoWrapper, direction, assignment);
                            }
                            else
                            {
                                newInput = jsManager.AddJoystickAssignment(configName, form, newPin, name, assignment, scale, reverse);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }   //  foreach pin node
        }


        /// <summary>
        /// Load HBridge Controllers
        /// </summary>
        public static void LoadHBridgeControllers(string configName, XmlNode hbridgeElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (hbridgeElement == null)
                return;

            var hbridgeNodes = hbridgeElement.SelectNodes("HBridge");
            foreach (XmlNode nextHBridge in hbridgeNodes)
            {
                /*
                <HBridge>
                   <Polarity>608,610,609</Polarity>    
                   <Joystick direction="1">RightStickUp</Joystick>    //  Joystick optional
                   <Joystick direction="-1">RightStickDown</Joystick>
               </HBridge>
               */

                HBridgeWrapper newHBridge = null;
                XmlNode byNameNode = nextHBridge.SelectSingleNode("ByName");
                if (byNameNode == null)
                {
                    newHBridge = pinManager.CreateHBridgeDriver(nextHBridge);
                }
                else
                {
                    newHBridge = pinManager.GetHBridgeDriver(byNameNode.InnerText);
                }

                if (newHBridge != null)
                {
                   
                    //  Map to joystick input
                    //  <Joystick>
                    XmlNodeList joystickNodes = nextHBridge.SelectNodes("Joystick");
                    if (joystickNodes == null)
                        continue;
                    try
                    {
                        foreach (XmlNode nextJsNode in joystickNodes)
                        {
                            JoystickControl assignment;

                            assignment = (JoystickControl)Enum.Parse(typeof(JoystickControl), nextJsNode.InnerText);
                            XmlElement jsElement = (XmlElement)nextJsNode;
                            string dirText = jsElement.GetAttribute("direction");
                            int dir = Int32.Parse(dirText);
                            dir = (dir > 0 ? 1 : -1);
                            string scaleText = jsElement.GetAttribute("scale");
                            double scale = 1.0;
                            if (scaleText != null && scaleText.Length > 0)
                                scale = double.Parse(scaleText);

                            jsManager.AddJoystickAssignment(configName, form, newHBridge, dir, assignment, scale);
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }


        /// <summary>
        /// Load Stepper Drivers
        /// </summary>
        public static void LoadStepperDrivers(string configName, XmlNode steppersElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (steppersElement == null)
                return;

            var stepperNodes = steppersElement.SelectNodes("StepperDriver");
            foreach (XmlNode nextStepper in stepperNodes)
            {
                //  Parse This
                /*
                 <StepperDriver>
                    <Sequence>1</Sequence>
                    <Pins>608,-613,-608, 613</Pins> 
                    <Polarity>608,610,609</Polarity>    //  Polarity only required if using bipolar stepper
			        <Polarity>613,611,612</Polarity>    
                    <Joystick direction="1">RightStickUp</Joystick>    //  Joystick optional
                    <Joystick direction="-1">RightStickDown</Joystick>
                </StepperDriver>
                */

                //  get the stepper
                StepperWrapper newStepperDriver = null;
                //  check to see if this stepper is defined by name
                XmlNode byNameNode = nextStepper.SelectSingleNode("ByName");
                if (byNameNode == null)
                {
                    //  we need the sequence referenced by ID 
                    XmlNode sequenceIdNode = nextStepper.SelectSingleNode("Sequence");
                    if (sequenceIdNode == null)
                        continue;

                    //  find this stepper sequence
                    string xpath = string.Format("StepperSequence[Id = '{0}']", sequenceIdNode.InnerText);
                    var sequenceNode = steppersElement.SelectSingleNode(xpath);
                    if (sequenceNode == null)
                        continue;

                    //  create the stepper motor driver, the library will take care of parsing everything in the two xml blobs
                    newStepperDriver = pinManager.CreateStepperDriver(sequenceNode, nextStepper);
                }
                else
                {
                    //  get this stepper from the GpioManager
                    newStepperDriver =  pinManager.GetStepperDriver(byNameNode.InnerText);
                }

                if (newStepperDriver != null)
                {
                    //  Map to joystick input
                    //  <Joystick>
                    XmlNodeList joystickNodes = nextStepper.SelectNodes("Joystick");
                    if (joystickNodes == null)
                        continue;
                    try
                    {
                        foreach (XmlNode nextJsNode in joystickNodes)
                        {
                            JoystickControl assignment;

                            assignment = (JoystickControl)Enum.Parse(typeof(JoystickControl), nextJsNode.InnerText);
                            XmlElement jsElement = (XmlElement)nextJsNode;
                            string dirText = jsElement.GetAttribute("direction");
                            int dir = Int32.Parse(dirText);
                            dir = (dir > 0 ? 1 : -1);

                            jsManager.AddJoystickAssignment(configName, form, newStepperDriver, dir, assignment);
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }


        public static void LoadSevenSegDisplayDrivers(string configName, XmlNode sevenSegDisplaysElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (sevenSegDisplaysElement == null)
                return;

            var displayNodes = sevenSegDisplaysElement.SelectNodes("SevenSegDisplay");
            foreach (XmlNode nextDisplay in displayNodes)
            {
                //  check to see if this stepper is defined by name
                XmlNode byNameNode = nextDisplay.SelectSingleNode("ByName");
                SevenSegDisplayWrapper newSevenSegDisplay = null;
                if (byNameNode == null)
                {
                    newSevenSegDisplay = pinManager.CreateSevenSegDisplayDriver(nextDisplay);
                }
                else
                {
                    newSevenSegDisplay = pinManager.GetSevenSegDisplayDriver(byNameNode.InnerText);
                }

                if ( newSevenSegDisplay != null )
                {
                    //  Map to joystick input
                    //  <Joystick>
                    XmlNodeList joystickNodes = nextDisplay.SelectNodes("Joystick");
                    if (joystickNodes == null)
                        continue;
                    try
                    {
                        foreach (XmlNode nextJsNode in joystickNodes)
                        {
                            JoystickControl assignment;

                            assignment = (JoystickControl)Enum.Parse(typeof(JoystickControl), nextJsNode.InnerText);
                            XmlElement jsElement = (XmlElement)nextJsNode;
                            string dirText = jsElement.GetAttribute("direction");
                            int dir = Int32.Parse(dirText);
                            dir = (dir > 0 ? 1 : -1);

                            jsManager.AddJoystickAssignment(configName, form, newSevenSegDisplay, dir, assignment);
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
        }


        public static void LoadJoystickFunctions(string configName, XmlNode functionsElement, MainForm form, JoystickManager jsManager)
        {
             var functionNodes = functionsElement.SelectNodes("Function");
            foreach (XmlNode nextFunction in functionNodes)
            {  
                try
                {
                    //  get the method
                    XmlElement methodElement = (XmlElement)nextFunction.SelectSingleNode("Method");
                    if (methodElement == null)
                        continue;
                    string[] fnStrings = Regex.Split(methodElement.InnerText, ",");

                    MethodInfo method = null;
                    if (fnStrings.Length > 1)
                    {
                        Type thisType = jsManager.GetType();
                        method = thisType.GetMethod(fnStrings[1]);
                    }
                    if (method == null)
                        continue;

                    XmlNodeList joystickNodes = nextFunction.SelectNodes("Joystick");
                    if (joystickNodes == null)
                        continue;
                    foreach (XmlNode nextJsNode in joystickNodes)
                    {
                        var assignment = (JoystickControl)Enum.Parse(typeof(JoystickControl), nextJsNode.InnerText);

                        jsManager.AddFunctionAssignment(configName, form, assignment, method);
                    }
                }
                catch (Exception)
                {

                    continue;
                }
                ////  check for function to associate with joystick inputs
                //XmlNodeList functionNodes = nextPin.SelectNodes("JoystickFunction");

                //if ( functionNodes != null && newInput != null )
                //{
                //    foreach (var nextFn in functionNodes )
                //    {
                //        var nextFnElement = (XmlElement)nextFn;
                //        string[] fnStrings = Regex.Split(nextFnElement.InnerText, ",");

                //        if (fnStrings.Length > 1)
                //        {
                //            Type thisType = jsManager.GetType();
                //            MethodInfo method = thisType.GetMethod(fnStrings[1]);
                //            if (method != null )
                //            {
                //                newInput.AddFunction(jsManager, method);
                //            }
                //        }
                //    }
                //}
            }
        }


        /// <summary>
        /// Get PWM range for a pin
        /// </summary>
        /// <param name="pinNumber"></param>
        /// <param name="isPca"></param>
        /// <returns></returns>
        public static int PwmRange(int pinNumber, bool isPca)
        {
            switch (pinNumber)
            {
                case 12:
                    //  raspberry pi, default range is 1024 on hardware PWM pin
                    return 1024;

                default:
                    
                    if (isPca)
                        // PCA always has a range of 4096
                        return 4096;
                    else
                        //  software PWM default range is 200 (for 50Hz)
                        return 200;
            }
        }
    }
}
