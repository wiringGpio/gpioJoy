using System;
using System.Xml;
using GpioManagerObjects;
using wiringGpioExtensions;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace GpioJoy
{
    class LoadConfiguration
    {

        /// <summary>
        /// Load Config File
        /// loads the pin configuration with joystick assignments from the config.xml file
        /// </summary>
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

                //  Get the root node for our pin mode settings
                //  <PinModes>
                var controlPinsNode = root.SelectSingleNode("PinModes");
                LoadPinModes(configName, controlPinsNode, form, pinManager, jsManager);

                //  Load stepper drivers
                //  <StepperDrivers>
                var steppersElement = root.SelectSingleNode("StepperDrivers");
                LoadStepperDrivers(configName, steppersElement, form, pinManager, jsManager);

                //  Load HBridges
                //  <HBridgeMotors>
                var hbridgesElement = root.SelectSingleNode("HBridgeMotors");
                LoadHBridgeMotors(configName, hbridgesElement, form, pinManager, jsManager);

                //  Load Displays
                //  <SevenSegDisplays>
                var sevenSegDisplaysElement = root.SelectSingleNode("SevenSegDisplays");
                LoadSevenSegDisplayDrivers(configName, sevenSegDisplaysElement, form, pinManager, jsManager);

                //  Load joystick input funcitons
                //  <JoystickFunctions>
                var functionsElement = root.SelectSingleNode("JoystickFunctions");
                LoadJoystickFunctionAssemblies(configName, functionsElement, form, jsManager);
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
                try
                {
                    var typeNode = nextMcp.SelectSingleNode("Type");
                    if (typeNode == null)
                        continue;

                    Mcp230xxType type;
                    type = (Mcp230xxType)Enum.Parse(typeof(Mcp230xxType), typeNode.InnerText);

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

                    int bus = 1;
                    var busNode = nextMcp.SelectSingleNode("Bus");
                    if (busNode != null)
                        bus = Int32.Parse(busNode.InnerText);
                    
                    pinManager.SetupMcp(type, bus, baseNumber, address);
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

                    int bus = 1;
                    var busNode = nextMcp.SelectSingleNode("Bus");
                    if (busNode != null)
                        bus = Int32.Parse(busNode.InnerText);

                    pinManager.SetupPca(bus, baseNumber, address, frequency);
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
        public static void LoadPinModes(string configName, XmlNode controlPinsNode, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
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
                string displayName = "";
                XmlNode nameNode = nextPin.SelectSingleNode("DisplayName");
                if (nameNode != null)
                    displayName = nameNode.InnerText;

                //  Pin Mode
                //  <Mode>
                XmlNode modeNode = nextPin.SelectSingleNode("Mode");
                if (modeNode == null)
                    continue;
                PinMode pinMode;
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
                        newPin = new GpioPinWrapperPcaJs(pinNumber, displayName, true);
                    }
                    else
                    {
                        newPin = new GpioPinWrapperJs(pinNumber, displayName, GPIO.IsHardwarePwmPin(pinNumber) == 1);
                    }

                    //  initialize this pin in the pin manager
                    pinManager.AddPinToManager(newPin);

                    //  we can create a pin object for this now
                    newPin.Mode = pinMode;


                    //  if we are PWM we need range
                    //  <Range>
                    int pwmRange = 0;
                    if (pinMode == PinMode.PWMOutput)
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
                                pwmRange = LoadConfiguration.PwmRange(pinNumber, pinManager.IsPcaPin(pinNumber));   //  TODO replace this with pwmGetRange()
                            }
                        }
                        else
                        {
                            pwmRange = LoadConfiguration.PwmRange(pinNumber, pinManager.IsPcaPin(pinNumber));   //  TODO - replace this
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
                                newInput = jsManager.AddJoystickAssignment(configName, form, servoWrapper, displayName, direction, assignment);
                            }
                            else
                            {
                                newInput = jsManager.AddJoystickAssignment(configName, form, newPin, displayName, assignment, scale, reverse);
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
        public static void LoadHBridgeMotors(string configName, XmlNode hbridgeElement, MainForm form, GpioManager pinManager, JoystickManager jsManager)
        {
            if (hbridgeElement == null)
                return;

            var hbridgeNodes = hbridgeElement.SelectNodes("HBridgeMotor");
            foreach (XmlNode nextHBridge in hbridgeNodes)
            {
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
                    string displayName = "";
                    XmlNode displayNameNode = nextHBridge.SelectSingleNode("DisplayName");
                    if (displayNameNode != null)
                    {
                        XmlElement displayNameElement = (XmlElement)displayNameNode;
                        displayName = displayNameElement.InnerText;
                    }

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

                            jsManager.AddJoystickAssignment(configName, form, newHBridge, displayName, dir, assignment, scale);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"LoadHBridgeControllers exception: {e}");
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
                    string displayName = "";
                    XmlNode displayNameNode = nextStepper.SelectSingleNode("DisplayName");
                    if (displayNameNode != null)
                    {
                        XmlElement displayNameElement = (XmlElement)displayNameNode;
                        displayName = displayNameElement.InnerText;
                    }

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

                            jsManager.AddJoystickAssignment(configName, form, newStepperDriver, displayName, dir, assignment);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"LoadStepperDrivers exception: {e}");
                        continue;
                    }
                }
            }
        }


        /// <summary>
        /// Load Seven Segment Display Drivers
        /// </summary>
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
                    string displayName = "";
                    XmlNode displayNameNode = nextDisplay.SelectSingleNode("DisplayName");
                    if (displayNameNode != null)
                    {
                        XmlElement displayNameElement = (XmlElement)displayNameNode;
                        displayName = displayNameElement.InnerText;
                    }

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

                            jsManager.AddJoystickAssignment(configName, form, newSevenSegDisplay, displayName, dir, assignment);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"LoadSevenSegDisplayDrivers exception: {e}");
                        continue;
                    }
                }
            }
        }


        /// <summary>
        /// Get PWM range for a pin
        /// </summary>
        public static int PwmRange(int pinNumber, bool isPca)
        {
            return GPIO.PwmGetRange(pinNumber);
        }


        /// <summary>
        /// Load user assemblies for function assignments
        /// </summary>
        public static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();
        public static Dictionary<string, Dictionary<string, List<string>>> LoadedAssembliesSetupFunctionsCalled = new Dictionary<string, Dictionary<string, List<string>>>();


        /// <summary>
        /// Load Assemblies
        /// </summary>
        public static void LoadJoystickFunctionAssemblies(string configName, XmlNode functionsElement, MainForm form, JoystickManager jsManager)
        {
            //  add ourselves to the loaded assemblies collection
            var ourselves = System.Reflection.Assembly.GetExecutingAssembly();
            if (!LoadedAssemblies.ContainsKey(ourselves.ManifestModule.Name))
            {
                LoadedAssemblies.Add(ourselves.ManifestModule.Name, ourselves);
            }

            //  see if we need to load any other assemblies
            var assemblyNodes = functionsElement.SelectNodes("Assembly");
            foreach (XmlNode nextAssembly in assemblyNodes)
            {
                var nameNode = (XmlElement)nextAssembly.SelectSingleNode("Name");
                if (nameNode == null)
                {
                    Console.WriteLine($"No name node for {nextAssembly.InnerText}");
                    continue;
                }
               
                //  Load the assembly and call the setup functions if we need to
                if ( ! LoadedAssemblies.ContainsKey(nameNode.InnerText) )
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(nameNode.InnerText);
                        LoadedAssemblies.Add(nameNode.InnerText, assembly);
                    }
                    catch ( Exception e)
                    {
                        Console.WriteLine($"Unable to load assembly {nameNode.InnerText}:  {e}");
                    }
                }

                if (LoadedAssemblies.ContainsKey(nameNode.InnerText))
                {
                    try
                    {
                        CallSetupMethods(nameNode.InnerText, nextAssembly, LoadedAssemblies[nameNode.InnerText]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unable to load assembly {nameNode.InnerText}:  {e}");
                    }
                }
            }
        }


        /// <summary>
        /// Call setup methods for loaded assemblies
        /// </summary>
        static void CallSetupMethods(string assemblyName, XmlNode assemblyNode, Assembly assembly)
        {
            var setupNodes = assemblyNode.SelectNodes("Setup");
            foreach (XmlNode nextSetupFn in setupNodes)
            {
                var setupClass = nextSetupFn.Attributes["class"]?.InnerText;
                var setupFunction = nextSetupFn.InnerText;
                if (setupClass == null || setupFunction == null)
                {
                    Console.WriteLine($"No setup class for setup function {nextSetupFn.InnerText}");
                    continue;
                }

                if ( ! LoadedAssembliesSetupFunctionsCalled.ContainsKey(assemblyName) )
                {
                    LoadedAssembliesSetupFunctionsCalled.Add(assemblyName, new Dictionary<string, List<string>>());
                }

                var assemblySetupFunctions = LoadedAssembliesSetupFunctionsCalled[assemblyName];

                if ( assemblySetupFunctions.ContainsKey(setupClass)  && assemblySetupFunctions[setupClass].Contains(setupFunction) )
                {
                    Console.WriteLine($"Skipping {setupClass} {setupFunction} because it has been called already.");
                    continue;
                }

                //  get this method and invoke it
                var setupType = assembly.GetType(setupClass);
                var method = setupType?.GetMethod(setupFunction);
                if (method == null)
                {
                    Console.WriteLine($"Unable to get method for {setupClass} and {setupFunction}");
                    continue;
                }
                try
                {
                    method.Invoke(null, null);
                    if (!assemblySetupFunctions.ContainsKey(setupClass))
                        assemblySetupFunctions.Add(setupClass, new List<string>() { setupFunction });
                    else
                        assemblySetupFunctions[setupClass].Add(setupFunction);
                }
                catch (Exception e)
                {
                    //  TODO - Log it
                    Console.WriteLine($"Exception in LoadJoystickFunctionAssemblies: {e}");
                }

            }
        }


        /// <summary>
        /// Load Joystick Functions
        /// </summary>
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

                    var assemblyName = methodElement.Attributes["assembly"]?.InnerText;
                    var className = methodElement.Attributes["class"]?.InnerText;

                    if (assemblyName != null && className != null && LoadedAssemblies.ContainsKey(assemblyName))
                    {
                        var assembly = LoadedAssemblies[assemblyName];
                        var type = assembly.GetType(className);
                        var method = type.GetMethod(methodElement.InnerText);

                        if (method != null)
                        {
                            XmlNodeList joystickNodes = nextFunction.SelectNodes("Joystick");
                            if (joystickNodes == null)
                                continue;
                            foreach (XmlNode nextJsNode in joystickNodes)
                            {
                                string displayName = "";
                                XmlElement nameElement = (XmlElement)nextFunction.SelectSingleNode("DisplayName");
                                if (nameElement != null)
                                    displayName = nameElement.InnerText;
                                

                                var assignment = (JoystickControl)Enum.Parse(typeof(JoystickControl), nextJsNode.InnerText);
                                jsManager.AddFunctionAssignment(configName, displayName, form, assignment, method);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unable to load method {methodElement.InnerText}");
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
    }
}
