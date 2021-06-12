using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using Square;
using Square.Models;
using System;
using System.Linq;

namespace OS_Square
{
    public class ProviderUtils
    {
        private static readonly string _locationId;
        private static readonly SquareClient _client;
        private static readonly NBrightInfo _settings;

        static ProviderUtils() {
            _settings = ProviderUtils.GetProviderSettings();
            var accessToken = NBrightCore.common.Security.Decrypt(PortalController.Instance.GetCurrentSettings().GUID.ToString(), _settings.GetXmlProperty("genxml/textbox/accesstoken"));
            var sandboxMode = _settings.GetXmlPropertyBool("genxml/checkbox/sandboxmode");
            
            var env = Square.Environment.Production;
            if (sandboxMode) { env = Square.Environment.Sandbox;}
            
            _client = new Square.SquareClient.Builder()
                    .Environment(env)
                    .AccessToken(accessToken).Build();
            
            // Get the default location or an exact name match
            // as specified in the plugin settings
            _locationId = GetLocation(_settings.GetXmlProperty("genxml/textbox/locationname")).Id;
        }

        public static NBrightInfo GetProviderSettings()
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", Utils.GetCurrentCulture());
            return info;
        }
        
        public static CreatePaymentResponse GetChargeResponse(OrderData orderData, string nonce)
        {
            // Every payment you process must have a unique idempotency key.
            // If you're unsure whether a particular payment succeeded, you can reattempt
            // it with the same idempotency key without worrying about double charging
            var idempotencyKey = IdempotencyKey();

            var appliedtotal = orderData.PurchaseInfo.GetXmlPropertyDouble("genxml/appliedtotal");
            var alreadypaid = orderData.PurchaseInfo.GetXmlPropertyDouble("genxml/alreadypaid");

            // Square uses the smallest denomination of the Currency being used
            // ie. when we take 1.00 USD then we will need to convert to 100
            var currencyFactor = 100;
            var currencyCode = _settings.GetXmlProperty("genxml/dropdownlist/currencycode");

            //TODO: verify only yen requires an adjustment to the currency factor
            if (currencyCode == "JPY")
            {
                currencyFactor = 1;
            }

            var orderTotal = (long)((appliedtotal - alreadypaid) * currencyFactor);
            var amount = new Money(orderTotal, currencyCode);

            var note = StoreSettings.Current.SettingsInfo.GetXmlProperty("genxml/textbox/storename") + " Order Number: " + orderData.OrderNumber;

            var bodyAmountMoney = new Money.Builder()
            .Amount(orderTotal)
            .Currency(currencyCode)
            .Build();

            // Creating Payment Request
            // NOTE: OS Order Id is passed in both the ReferenceId & Note field
            // because the reference_id is not visible in the square ui but can be 
            // found via the transactions api.
            var body = new CreatePaymentRequest.Builder(
                nonce,
                idempotencyKey,
                bodyAmountMoney)
            .Autocomplete(true)
            .LocationId(_locationId)
            .ReferenceId(orderData.OrderNumber)
            .Note(note)
            .Build();

            return _client.PaymentsApi.CreatePayment(body);

        }

        private static Location GetLocation(string squareLocationName)
        {

            var locationList = _client.LocationsApi.ListLocations();

            // Defensive check to ensure we have at least 1 location
            if (locationList == null || locationList.Locations.Count < 1)
            {
                throw new Exception("No Locations found. Charge aborted.");
            }

            // Set the default location to the 1st location
            var myLocation = locationList.Locations[0];

            // Check if plugin settings have a location name that matches 
            // a name in the Square location list
            if (!string.IsNullOrWhiteSpace(squareLocationName)) {
                myLocation = locationList.Locations.Where(x => x.Name.Equals(squareLocationName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (myLocation == null) {

                    // Throw an error to avoid a user with multiple locations
                    // from accidently charging their default location.
                    throw new Exception("Invalid Location Name. Charge aborted. Check plugin settings.");
                }
            }

            return myLocation;
        }

        public static string IdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
