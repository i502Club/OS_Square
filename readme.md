# OS_Square
A DNN OpenStore payment provider plugin

## Getting Started
This is a payment provider plugin for [![OpenStore Ecommerce](assets/images/os_logo_150X29.png)](https://www.openstore-ecommerce.com/en-gb/OpenStore). It will enable any 
DNN 9.4+ site running OpenStore to accept CC payments into their Square account.  You must 
have a valid Square account and a [developers](https://developer.squareup.com/) 
api key for this provider to work.  



### Installing
1. Install into DNN as a normal module.  Ensure that your DNN OpenStore installation is using 
   at least v8.5.2 of the [NBrightTemplateSys](https://github.com/nbrightproject/NBrightTS).



2. Go into Open-Store BO>Admin, the "OS_Square" option should be listed.
![OpenStore Back Office Admin Panel](assets/images/plugin_installed.png)



3. ![Square logo](assets/images/square-logo.png)  See Square's [developers](https://developer.squareup.com/) portal for your Application ID & API Access Token.



4. Configure your OpenStore Back Office plugin settings for the Square plugin with the credentials from step 3. 
![OS_Square Plugin Settings](assets/images/settings.png)



5. Select the strain(i502 warm up pun) of currency code that is relevant to your Square account.  Presently there is 
support for USD, CAD, AUD, GBP, JPY.

 ![OS_Square supported currency flags](assets/images/flags_292X40.png)


6. Optionally enter a Location Name.  The provider by default uses the first location 
returned from your account via the ListLocations endpoint but if you have more than one 
location you can optionally specify it by Name in the Location input.  *If the name 
you entered does not match a Location Name in your Square account you will receive an 
error*


7. Select the sandbox mode when you are testing against your sandbox account.  Uncheck this 
box when you are ready to send requests to your actual Square account.  You must have your 
IIS bound to localhost for testing.  Urls such as dnndev.me or dnn.local won't work.

---

*Congratulations*! The gateway should now be ready and your customers can purchase securely with the (c)Square 
payment form during your OpenStore checkout process.

![Square payment form](assets/images/cc_form.png)

---

### Development
 1. Install the module into your development enviroment.
 2. Clone(i502 low hanging pun fruit) the repo to your /DesktopModules/i502Club/ directory.
 2. Your development environment IIS server must bind your DNN site to localhost 
	otherwise the payment form & Square assembly will not work using the sandbox.  
 3. See Square's developer portal for your Application ID, API Access Token and test cc card information.
 4. Configure your settings for the Square plugin.  You will need an Application ID and API Access Token.
	The provider by default uses the first location returned from your account but if you have more 
	than one location you can optionally specify it by Name in the Location input.
 5. You should be able to compile and attach the debugger at this point.


### Dependencies

 * Square v6.5.0.0
 * NewtonSoft v12 
 
 Note: Currently the DNN default install does not have a high enough(i502 accidental pun)
 version of Newtonsoft.Json for the Square lib to work. Therefore the module installation 
 will create a bin/Newtonsoft.Json/v12 directory and update the web.config to include 
 the binding redirects that enable the Square library to locate it. There is no sql 
 provider with this module install. *Please follow best practice and back up both 
 your db & file system before installing*.


## History
This module is the evolution of an earlier version that worked with the NBStore system 
before it's name change to OpenStore. The v2 version of this plugin began when 
breaking changes from Square.Connect 2.25 were mitigated. v3 represents the migration 
from the deprecated Square.Connect library to it's successor, the Square library 
which is currently at v6.5. 

This current version 3.0.1-rc is to allow for some testing, feedback particularly 
on foreign currencies, and for smoke testing(i502 technical pun) for 
case scenarios that may have been overlooked or weren't relevant to our initial goals.  
The plugin provides support for USD, AUD, GBP, CAD and JPY currencies.  You can set 
your currency code from the OS Back Office.  v3.0.1-rc is the first public release.


## Authors
[![OpenStore Ecommerce](assets/images/icon_extension.png)](https://www.i502.club) [i502 Club](https://www.i502.club)

This project was built using templates provided for the OpenStore community by it's creator David Lee. Disons merci.

## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details

## Acknowledgments
* All the contributors to [DNN](https://github.com/dnnsoftware/Dnn.Platform) & [OpenStore]( https://github.com/openstore-ecommerce/OpenStore) 

## Contribute
 Don't bogart the code(i502 forced pun). Pass it around(last one I promise). You can create an issue or submit a pull request
 to help make the plugin work better.
