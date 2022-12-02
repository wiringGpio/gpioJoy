# GPIO Joy #

GPIOJoy is an application for the Raspberry Pi or NVIDIA Jetson computer that allows you to control the GPIO pins with an XBox joystick.  

You can map the buttons to turn pins on and off or to actuate a function by name in your code.  You can map the joysticks to control pin pulse width modulation (PWM),  unipolar and bipolar stepper motors, HBridge motor circuits, and seven segment displays.

You do not need to write any code to configure the application to control your specific Raspberry Pi creation. The buttons and joysticks are mapped to the GPIO pins using an XML file.  Simply put your configuration in an XML file and copy it to the /home/<user>/gpioJoy/Config folder on the Raspberry Pi of the Jetson, and then run the program.

Joystick control is implemented in the SimpleJoy assembly, thakks to [this shared code](http://mpolaczyk.pl/raspberry-pi-mono-c-joystick-handler/).

For more informations, please see our website at:  [LittleBytesOfPi.com/GPIOJoy](http://littlebytesofpi.com/gpiojoy)