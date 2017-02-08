using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JneCommSitesManagement.Helper
{
    public static class HelperUI
    {
        public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
        {
            string checkBoxWithHidden = htmlHelper.CheckBox(name, isChecked, htmlAttributes).ToHtmlString().Trim();
            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1));
            return new MvcHtmlString(pureCheckBox);
        }

    }
}