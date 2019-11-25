using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using Square.Connect.Api;
using Square.Connect.Model;
using System;
using System.Linq;

namespace OS_Square
{
    public class ProviderUtils
    {
        readonly static Square.Connect.Client.Configuration _config;
        readonly static string _locationId;

        static ProviderUtils() {
            var settings = ProviderUtils.GetProviderSettings();
            var accessToken = NBrightCore.common.Security.Decrypt(PortalController.Instance.GetCurrentPortalSettings().GUID.ToString(), settings.GetXmlProperty("genxml/textbox/accesstoken"));
            var url = settings.GetXmlPropertyBool("genxml/checkbox/sandboxmode") == true ? "https://connect.squareupsandbox.com" : "https://connect.squareupsandbox.com";

            _config = new Square.Connect.Client.Configuration(new Square.Connect.Client.ApiClient(url)){ AccessToken = accessToken };

            // Get the default location or an exact name match as specified in the plugin settings
            var squareLocationName = settings.GetXmlProperty("genxml/textbox/locationname");
            _locationId = GetLocationId(squareLocationName);
        }
        public static NBrightInfo GetProviderSettings()
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", Utils.GetCurrentCulture());
            return info;
        }
        
        public static CreatePaymentResponse GetChargeResponse(OrderData orderData, string nonce)
        {
            var settings = ProviderUtils.GetProviderSettings();

            // Every payment you process must have a unique idempotency key.
            // If you're unsure whether a particular payment succeeded, you can reattempt
            // it with the same idempotency key without worrying about double charging
            var uuid = NewIdempotencyKey();

            var appliedtotal = orderData.PurchaseInfo.GetXmlPropertyDouble("genxml/appliedtotal");
            var alreadypaid = orderData.PurchaseInfo.GetXmlPropertyDouble("genxml/alreadypaid");

            // Square uses the smallest denomination of the Currency being used
            // ie. when we take 1.00 USD then we will need to convert to 100
            var currencyFactor = 100;
            var currencyCode = settings.GetXmlProperty("genxml/dropdownlist/currencycode");

            //TODO: verify only yen requires an adjustment to the currency factor
            if (currencyCode == "JPY")
            {
                currencyFactor = 1;
            }

            var orderTotal = (int)((appliedtotal - alreadypaid) * currencyFactor);
            var amount = NewMoney(orderTotal, currencyCode);
            
            var storename = StoreSettings.Current.SettingsInfo.GetXmlProperty("genxml/textbox/storename");
            var note = storename + " Order Number: " + orderData.OrderNumber;

            // Creating Payment Request
            // NOTE: OS Order Id is passed in both the reference_id & note field
            // because the reference_id is not visible in the ui but can be found via the 
            // transactions api.
            var _paymentsApi = new PaymentsApi(_config);
            var body = new CreatePaymentRequest(AmountMoney: amount, IdempotencyKey: uuid,  SourceId: nonce, LocationId: _locationId, ReferenceId: orderData.OrderNumber, Note: note );
            
            var response = _paymentsApi.CreatePayment(body);

            return response;
        }

        private static string GetLocationId(string squareLocationName)
        {

            LocationsApi _locationApi = new LocationsApi(_config);
            
            var locationList = _locationApi.ListLocations();

            if (locationList == null || locationList.Locations.Count < 1)
            {
                throw new Exception("No Locations found. Charge aborted.");
            }

            // Set the default location to the 1st location
            var myLocation = locationList.Locations[0];

            // Check if the plugin settings have an location name matching one in Square's list
            if (!string.IsNullOrWhiteSpace(squareLocationName))
            {
                var l = locationList.Locations.Where(x => x.Name == squareLocationName).FirstOrDefault();
                if (l != null)
                {
                    myLocation = l;
                }
            }

            return myLocation.Id;
        }

        public static Money NewMoney(int amount, string currency)
        {
            //return new Money(amount, Money.ToCurrencyEnum(currency));
            return new Money(amount, currency);
        }

        public static string NewIdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
