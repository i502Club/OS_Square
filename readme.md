# Project Title
OS_Square

## Getting Started
This is a payment provider plugin for Open Store. It will enable any 
DNN 9.4+ site running Open Store to accept CC payments into their  
&copy;Square account.  You must have a valid &copy;Square account and 
a [developers](https://developer.squareup.com/) api key for this provider to work.

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

 * Square.Connect 2.25.0.0
 * System.ComponentModel.Annotations 4.2.1.0
 * System.ComponentModel.DataAnnotations 4.0.0.0
 * RestSharp 106.3.1.0

 * DotNetNuke.DependencyInjection 9.7.1.0
 * Microsoft.Extensions.DependencyInjection 2.1.1.0
 * Microsoft.Extensions.DependencyInjection.Abstractions 2.1.1.0


NOTE The installation includes a version of Square.Connect dll and 3 supporting dlls 
which the installer places in the bin directory.  The installation also installs the 
DotNetNuke.DependencyInjection & 2 supporting libraries.  Please back up your bin directory 
as a precaution. There is no sql provider and therefore no db changes happening with this module 
install.  This module is the evolution of an earlier version(v1) that worked with the NBStore system before it's name 
change to OpenStore.  The project also references the DotNetNuke.DependencyInjection 
library in preparation for core support.  The v1 version of this plugin began when 
OpenStore was called NBStore. We bumped it's major version to v2 when OpenStore rolled out.
 

 ### Development

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
 i502 Club, Reggae

 ## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details

## Acknowledgments
* All the contributors to [DNN](https://github.com/dnnsoftware/Dnn.Platform) & [OpenStore]( https://github.com/openstore-ecommerce/OpenStore) 

 ## Contribute
 * Contributions are awesome.  You can create an issue or submit a pull request
 to help make the plugin work better.
