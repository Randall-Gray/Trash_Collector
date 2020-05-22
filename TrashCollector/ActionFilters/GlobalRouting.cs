using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TrashCollector.ActionFilters
{
    public class GlobalRouting : IActionFilter
    {
        private readonly ClaimsPrincipal _claimsPrincipal; 

        public GlobalRouting(ClaimsPrincipal claimsPrincipal) 
        { 
            _claimsPrincipal = claimsPrincipal; 
        }

        public void OnActionExecuting(ActionExecutingContext context) 
        { 
            var controller = context.RouteData.Values["controller"]; 
            string action = context.RouteData.Values["action"].ToString();

            if (controller.Equals("Home") && action.ToLower() == "index") 
            { 
                if (_claimsPrincipal.IsInRole("Customer")) 
                { 
                    context.Result = new RedirectToActionResult(action, "Customers", null); 
                } 
                else if (_claimsPrincipal.IsInRole("Employee")) 
                { 
                    context.Result = new RedirectToActionResult(action, "Employees", null); 
                } 
            } 
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
