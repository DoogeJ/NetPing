# NetPing
NetPing is a simple utility to ping any given address and show the output in a nice graph.

It's a great way to monitor a connection or host and gives feedback in an intuitive user interface.

## Screenshot:
![Alt text](/netping.png?raw=true "NetPing screenshot")

## User interface options:
**Target:** Obviously this is the target that you want to ping  
**TTL:** This is the maximum TTL (Time-To-Live) for a packet  
**Timeout:** This is the ping timeout in ms (milliseconds). Pings that take longer are considered timed out  
**Interval:** This is the ping interval in ms (milliseconds)  
**Display:** This is the amount of pings to display in the graph

# Development
This section is focussed on development of NetPing, in case you want to build your own version(s).

## Environment
This application was written using Visual Studio 2015 and .NET Framework 4.5.2 on Windows 10 1511.  
It will most likely build fine on different configurations, but might require some modifications.

## Updating the version number
The version number is stored in two locations:
* In the projects assembly info file: **'NetPing\Properties\AssemblyInfo.cs'**
* In the installer setup.iss file (only used for the installer): **'Installer\setup.iss'**

## Building the installer
> Note: *This installer uses 3rd party components from [innodependencyinstaller](https://github.com/stfx/innodependencyinstaller) to automatically download and install the correct .NET framework if required. See* ***'Installer\innodependencyinstaller LICENSE.md'*** *for the license of innodependencyinstaller.*

NetPing uses the [Inno Setup Compiler](http://www.jrsoftware.org/isinfo.php) to generate an executable installer.  
The installer project files are located in the **'Installer'**-folder.  

The scripts are based on [Inno Setup 5.5.9](http://www.jrsoftware.org/isinfo.php) but will probably work with any recent version.

To build the installer:
* Make sure the project is compiled and there is a working executable named **'NetPing.exe'** in the **'NetPing\bin\Release'**-folder 
* Open **'Installer\setup.iss'** in the [Inno Setup Compiler](http://www.jrsoftware.org/isinfo.php) 
* Hit *Build* -> *Compile* (Ctrl+F9)
* Done, the installer should be in the **'Installer\bin'**-folder
