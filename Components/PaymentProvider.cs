using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using System;
using System.Web;

namespace OS_Square
{
    public class OS_SquarePaymentProvider : Nevoweb.DNN.NBrightBuy.Components.Interfaces.PaymentsInterface
    {
        public override string Paymentskey { get; set; }

        public override string GetTemplate(NBrightInfo cartInfo)
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", Utils.GetCurrentCulture());
            var templateName = info.GetXmlProperty("genxml/textbox/checkouttemplate");
            var passSettings = info.ToDictionary();
            foreach (var s in StoreSettings.Current.Settings()) // copy store setting, otherwise we get a byRef assignement
            {
                if (passSettings.ContainsKey(s.Key))
                    passSettings[s.Key] = s.Value;
                else
                    passSettings.Add(s.Key, s.Value);
            }
            var templ = NBrightBuyUtils.RazorTemplRender(templateName, 0, "", info, "/DesktopModules/i502Club/OS_Square", "config", Utils.GetCurrentCulture(), passSettings);

            return templ;
        }

        public override string RedirectForPayment(OrderData orderData)
        {
            orderData.OrderStatus = "020";
            orderData.PurchaseInfo.SetXmlProperty("genxml/paymenterror", "");
            orderData.PurchaseInfo.Lang = Utils.GetCurrentCulture();
            orderData.SavePurchaseData();
            try
            {
                var nonce = HttpContext.Current.Request.Cookies.Get("nonce") != null ? HttpContext.Current.Request.Cookies.Get("nonce").Value : "";
                
                if (string.IsNullOrWhiteSpace(nonce))
                {
                    HttpContext.Current.Request.Cookies.Get("nonce").Expires = DateTime.Now.AddDays(-1d);

                    //No Nonce Return to Payment Tab with Failure message;
                    var param = new string[2];
                    param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
                    param[1] = "status=0";
                    return Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param);
                }
                else
                {
                    // 010 = Incomplete, 020 = Waiting for Bank,030 = Cancelled,040 = Payment OK,050 = Payment Not Verified,060 = Waiting for Payment,070 = Waiting for Stock,080 = Waiting,090 = Shipped,010 = Closed,011 = Archived

                    HttpContext.Current.Response.Clear();

                    var response = ProviderUtils.GetChargeResponse(orderData, nonce);

                    var param = new string[2];
                    param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
                    
                    if (response.Errors == null || response.Errors.Count == 0)
                    {
                        // successful transaction
                        orderData.OrderStatus = "040";
                        orderData.PaymentOk("040");
                        param[1] = "status=1";

                        NBrightBuyUtils.SendOrderEmail("OrderCreatedClient", orderData.PurchaseInfo.ItemID, "ordercreatedemailsubject");

                    }
                    else {
                        // failed transaction
                        orderData.OrderStatus = "030";
                        param[1] = "status=0";

                        // create error string for output to the order audit log
                        var errorString = "";
                        if (response.Errors.Count > 0)
                        {
                            foreach (var e in response.Errors)
                            {
                                errorString += e.Detail;
                                errorString += " ";
                            };
                        }
                        
                        //add message for admins to view in the order audit log
                        orderData.AddAuditMessage(errorString, "notes", UserController.Instance.GetCurrentUserInfo().Username, "False");
                    }

                    orderData.SavePurchaseData();
                    HttpContext.Current.Response.Redirect(Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param),false);
                }
            }
            catch (Exception ex)
            {
                // rollback transaction
                // NOTE: The errors returned by the gateway are not shown to the user
                //      DNN admin must be able to review the cart data for a user.
                orderData.PurchaseInfo.SetXmlProperty("genxml/paymenterror", "<div>ERROR: Invalid payment data </div><div>" + ex + "</div>");
                orderData.PaymentFail();
                var param = new string[2];
                param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
                param[1] = "status=0";
                HttpContext.Current.Response.Redirect(Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param));
            }

            try
            {
                HttpContext.Current.Response.End();
            }
            catch (Exception)
            {
                // this try/catch to avoid sending error 'ThreadAbortException'  
            }

            return "";
        }

        public override string ProcessPaymentReturn(HttpContext context)
        {
            var orderid = Utils.RequestQueryStringParam(context, "orderid");
            if (Utils.IsNumeric(orderid))
            {
                var orderData = new OrderData(Convert.ToInt32(orderid));
                var status = Utils.RequestQueryStringParam(context, "status");
                if (status == "0")
                {
                    var rtnerr = "";
                    if (orderData.OrderStatus == "020") // check we have a waiting for bank status, IPN may have already altered this. 
                    {
                        rtnerr = orderData.PurchaseInfo.GetXmlProperty("genxml/paymenterror");
                        orderData.PaymentFail();
                    }
                    return GetReturnTemplate(orderData, false, rtnerr);
                }
                // check we have a waiting for bank status (IPN may have altered status already + help stop hack)
                if (orderData.OrderStatus == "020")
                {
                    orderData.PaymentOk("050"); // order paid, but NOT verified
                }
                return GetReturnTemplate(orderData, true, "");
            }
            return "";
        }

        private string GetReturnTemplate(OrderData orderData, bool paymentok, string paymenterror)
        {
            var info = ProviderUtils.GetProviderSettings();
            info.UserId = UserController.Instance.GetCurrentUserInfo().UserID;
            var passSettings = NBrightBuyUtils.GetPassSettings(info);
            if (passSettings.ContainsKey("paymenterror"))
            {
                passSettings.Add("paymenterror", paymenterror);
            }
            var displaytemplate = "payment_ok.cshtml";
            string templ;
            if (paymentok)
            {
                info.SetXmlProperty("genxml/ordernumber", orderData.OrderNumber);
                templ = NBrightBuyUtils.RazorTemplRender(displaytemplate, 0, "", info, "/DesktopModules/i502Club/OS_Square", "config", Utils.GetCurrentCulture(), passSettings);
            }
            else
            {
                displaytemplate = "payment_fail.cshtml";
                templ = NBrightBuyUtils.RazorTemplRender(displaytemplate, 0, "", info, "/DesktopModules/i502Club/OS_Square", "config", Utils.GetCurrentCulture(), passSettings);
            }

            return templ;
        }

    }
}