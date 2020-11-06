# Project Title
OS_Square

## Getting Started
This is a payment provider plugin for Open Store. It will enable any 
DNN 9.4+ site running Open Store to accept CC payments into their  
&copy;Square account.

### Installing
1. Install into DNN as a normal module.  Ensure that your DNN Open Store installation is using 
   at least v8.5.2 of the NBrightTemplateSys.
2. Go into Open-Store BO>Admin, the "OS_Square" option should be listed.
3. See Square's developer portal for your Application ID & API Access Token.
4. Configure your settings for the Square plugin with the credentials from step 3. 
5. The provider by default uses the first location returned from your account but if you have more 
	than one location you can optionally specify it by Name in the Location input.

The gateway should now be ready.

### Dependencies
Libraries that get installed with the module:

 Square.Connect 2.25.0.0
 System.ComponentModel.Annotations 4.2.1.0
 System.ComponentModel.DataAnnotations 4.0.0.0
 RestSharp 106.3.1.0

 DotNetNuke.DependecncyInjection 9.7.1.0
 Microsoft.Extensions.DependencyInjection 2.1.1.0
 Microsoft.Extensions.DependencyInjection.Abstractions 2.1.1.0


NOTE: This install includes a version of Square.Connect dll and 3 supporting dlls 
which the installer should place in the bin directory.  Please back up your bin directory as 
a precaution in the event there is some unforeseen incompatability. This module is the 
evolution of an earlier version that worked with the NBrightBuy system before it's 
change to Open Store.  The project also references the DotNetNuke.DependencyInjection 
library in preperation for core support.  Version 2.0.1 is the 1st public version 
which is based on internal work that began when Open Store was known as NBStore. 
 

 Development
 ===========

 1. Install the module into your development enviroment.
 2. Clone the repo to your /DesktopModules/i502Club/ directory.
 2. Your development environment IIS server must bind your DNN site to localhost 
	otherwise the payment form & Square.Connect assembly will not work using the sandbox.  
 3. See Square's developer portal for your Application ID, API Access Token and test cc card information.
 4. Configure your settings for the Square plugin.  You will need an Application ID and API Access Token.
	The provider by default uses the first location returned from your account but if you have more 
	than one location you can optionally specify it by Name in the Location input.
 5. You should be able to compile and attach the debugger at this point.

 ## Authors
 i502 Club

 ## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details

## Acknowledgments
* All the contributors to DNN & the Open Store projects.