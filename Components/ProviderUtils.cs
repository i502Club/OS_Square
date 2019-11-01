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
        public static NBrightInfo GetProviderSettings()
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", Utils.GetCurrentCulture());
            return info;
        }
        
        public static ChargeResponse GetChargeResponse(OrderData orderData, string nonce)
        {
            var settings = ProviderUtils.GetProviderSettings();

            // Below requires the settings field to be using the @TextBox token with the encrypted param set to true.
            var accessToken = NBrightCore.common.Security.Decrypt(PortalController.Instance.GetCurrentPortalSettings().GUID.ToString(), settings.GetXmlProperty("genxml/textbox/accesstoken"));
            var squareLocationName = settings.GetXmlProperty("genxml/textbox/locationname");

            // Get the default location or an exact name match as specified in the plugin settings
            var locationId = GetLocationId(accessToken, squareLocationName);

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

            var _transactionApi = new TransactionApi();

            var body = new ChargeRequest(AmountMoney: amount, IdempotencyKey: uuid, CardNonce: nonce);

            // TODO: Consider adding try catch with a delayed re-charge using the nonce..??
            var response = _transactionApi.Charge(accessToken, locationId, body);

            return response;
        }

        private static string GetLocationId(string accessToken, string squareLocationName)
        {
            //Get Square Account Locations            
            LocationApi _locationApi = new LocationApi();

            var locationList = _locationApi.ListLocations(accessToken);

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
            return new Money(amount, Money.ToCurrencyEnum(currency));
        }

        public static string NewIdempotencyKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
