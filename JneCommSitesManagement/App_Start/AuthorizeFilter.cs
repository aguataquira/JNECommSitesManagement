using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JneCommSitesManagement.App_Start
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        string _controllerName;
        string _actionName;

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/AccessDenied");
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _controllerName = filterContext.Controller.ControllerContext.RouteData.Values["Controller"].ToString();
            _actionName = filterContext.Controller.ControllerContext.RouteData.Values["Action"].ToString();
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            JneCommSitesDataLayer.JneCommSitesDataBaseEntities context = new JneCommSitesDataLayer.JneCommSitesDataBaseEntities();
            bool ret = base.AuthorizeCore(httpContext);

            var rols = (from p in context.AspNetRoles
                             from d in p.AspNetUsers
                             where d.UserName == httpContext.User.Identity.Name
                             select p).FirstOrDefault();

            if (rols == null)
                return false;
            string rolNameQuery = rols.Name;
            var rolid = (from p in context.AspNetRoles where p.Name == rolNameQuery select p.Id).First();

            if (null == rolid)
                return false;

            //var operationsQuery = BOSDataLayer.ReadyQueries.getOperationByCtrlMethod(context, _controllerName, _actionName)
            //                       .Where(op => op.RoleId == rolid
            //                           && op.bIsActive == null ? false : (bool)op.bIsActive
            //                           && op.parentIsActive == null ? (bool)op.bIsActive : (bool)op.parentIsActive);

            var operationsQuery = (from p in context.AspNetRoles
                                   from d in p.T_Operations
                                   where p.Id == rolid
                                   && d.vControllerName == _controllerName
                                   && d.vOperationName == _actionName
                                   select d);

            var actualOperation = operationsQuery.FirstOrDefault(op => op.vOperationName == _actionName && op.vControllerName == _controllerName);

            ret = (actualOperation != null);
            if (operationsQuery.Count() > 0)
            {
                return true;
            }
            if (false == ret)
            {

                var parentOperation = operationsQuery.FirstOrDefault(op => op.iParentOper == null);
                ret = (parentOperation != null && (bool)parentOperation.bIsActive);
                ret = false;
            }
            else
            {
                if (!(bool)actualOperation.bRequieresPermission)
                {
                    var parentOperation = operationsQuery.FirstOrDefault(op => actualOperation.iParentOper == op.biOperationsId);
                    ret = ((parentOperation == null) || (parentOperation != null && (bool)parentOperation.bIsActive && !(bool)parentOperation.bRequieresPermission));
                }
                else
                {
                    ret = false;
                }
            }
            return ret;
        }
    }
}