# Project Title
OS_Square

## Getting Started
This is a payment provider plugin for Open Store. It will enable any 
DNN 9.4+ site running Open Store to accept CC payments on their site.

### Installing
1. Install into DNN as a normal module.
2. Go into Open-Store BO>Admin, the "OS_Square" option should be listed.
3. See Square's developer portal for your Application ID & API Access Token.
4. Configure your settings for the Square plugin with the credentials from step 3. 
5. The provider by default uses the first location returned from your account but if you have more 
	than one location you can optionally specify it by Name in the Location input.

The gateway should now be ready.


NOTE: This install includes a version of Square.Connect dll which the installer should place in the bin directory.
 

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
 i502.club

 ## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments
* All the contributors to DNN & the Open Store projects.