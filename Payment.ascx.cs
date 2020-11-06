using NBrightCore.common;
using Nevoweb.DNN.NBrightBuy.Base;
using Nevoweb.DNN.NBrightBuy.Components;
using System;
using System.Web.UI.WebControls;

namespace OS_Square
{

    public partial class OS_SquarePayment : NBrightBuyAdminBase
    {

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack == false)
            {
                PageLoad();
            }
        }

        private void PageLoad()
        {
            if (NBrightBuyUtils.CheckRights())
            {
                var objCtrl = new NBrightBuyController();
                var info = objCtrl.GetPluginSinglePageData("OS_Squarepayment", "OS_SquarePAYMENT", Utils.GetCurrentCulture());
                var strOut = NBrightBuyUtils.RazorTemplRender("settings.cshtml", 0, "", info, ControlPath, "config", Utils.GetCurrentCulture(), StoreSettings.Current.Settings());
                var l = new Literal();
                l.Text = strOut;
                Controls.Add(l);
            }
        }

        #endregion

    }

}
