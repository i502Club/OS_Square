using System.Web;
using Nevoweb.DNN.NBrightBuy.Components;
using Nevoweb.DNN.NBrightBuy.Components.Interfaces;

namespace OS_Square
{
    public class AjaxProvider : AjaxInterface
    {
        public override string Ajaxkey { get; set; }

        public override string ProcessCommand(string paramCmd, HttpContext context, string editlang = "")
        {
            var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
            var lang = NBrightBuyUtils.SetContextLangauge(ajaxInfo); // Ajax breaks context with DNN, so reset the context language to match the client.
            var objCtrl = new NBrightBuyController();

            var strOut = "OS_Square Ajax Error";

            // NOTE: The paramCmd MUST start with the plugin ref. in lowercase. (links ajax provider to cmd)
            switch (paramCmd)
            {
                case "os_square_savesettings":
                    strOut = objCtrl.SavePluginSinglePageData(context);
                    break;
                case "os_square_selectlang":
                    objCtrl.SavePluginSinglePageData(context);
                    var nextlang = ajaxInfo.GetXmlProperty("genxml/hidden/nextlang");
                    var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", nextlang);
                    strOut = NBrightBuyUtils.RazorTemplRender("settingsfields.cshtml", 0, "", info, "/DesktopModules/i502Club/OS_Square", "config", nextlang, StoreSettings.Current.Settings());
                    break;
            }

            return strOut;

        }

        public override void Validate()
        {
        }

    }
}
