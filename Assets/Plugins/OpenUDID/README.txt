##########################################################################################
  _____                          _             _       
 / ____|                        | |           (_)      
| |  __  __ _ _ __ ___   ___  __| | ___  _ __  _  __ _ 
| | |_ |/ _` | '_ ` _ \ / _ \/ _` |/ _ \| '_ \| |/ _` |
| |__| | (_| | | | | | |  __/ (_| | (_) | | | | | (_| |
 \_____|\__,_|_| |_| |_|\___|\__,_|\___/|_| |_|_|\__,_|
                                                       
##########################################################################################

##########################################################################################
##### iOS OpenUDID Plugin
##########################################################################################
Informational:  http://www.gamedonia.com
Documentation:  http://docs.gamedonia.com
Support:		support@gamedonia.com
Twitter:        https://twitter.com/gamedonia

##########################################################################################
##### Quick Start
##########################################################################################
 1. Download the "iOS OpenUDID Plugin" from the Asset Store.
 2. Load the included test scene "OpenUDIDTest".
 3. Setup the build settings of your project to make the first scene to load this one.
 4. Run the project on your iOS device.
 5. That's all you are seeing your device universal identifier on screen.

Open the file "OpenUDIDTest.cs" and check the sample code, yeah! it's online one line of 
code!!.
 
##########################################################################################
##### Detailed usage
##########################################################################################
This is a plugin for iOS so you'll get the right identifier only when running your app in
your iOS device.

If you test the scene in the Unity Editor you'll get a default string value of "10000" to
allow you test without worring about compiling issues when testing inside the editor.

You can access the OpenUDID of the device by calling:

    string openUdid = OpenUDIDPlugin.GetOpenUDID();


##########################################################################################
##### FAQ
##########################################################################################
1. I'm getting a compilation "error OpenUDID.m:100:9: Cannot use '@try' with Objective-C 
   exceptions disabled"
   - Check that python is installed on your system. Gamedonia build Postprocessor relies 
     on it to processo your XCode project. Install it an build the project again.

2. I have python installed but i still have the above compilation error.
   - If your project has it's own post build process mechanism may be the Gamedonia one is 
     not being executed. Our post processor source is included feel free to modify it. If
     oyu need help contact us!.

   - How ever you can enable Object C exceptions on the build settings of your project
     manually.


  
  

