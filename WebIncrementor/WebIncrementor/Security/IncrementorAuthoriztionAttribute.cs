using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using WebIncrementor.Models;
using WebIncrementor.Models.ViewModels;

namespace WebIncrementor.Security
{
    public class IncrementorAuthoriztionAttribute : TypeFilterAttribute
    {
        public IncrementorAuthoriztionAttribute() : base(typeof(IncrementorAuthorizationFilter))
        {

        }
    }

    public class IncrementorAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var header = context.HttpContext.Request.Headers;

            if (header.ContainsKey("Authorization") && !string.IsNullOrEmpty(header["Authorization"].FirstOrDefault()))
            {
                char[] delimiters = { ' ', '\t'};
                string[] credentials = (header["Authorization"].FirstOrDefault()).Split(delimiters);

                if(credentials.Length < 2)
                {
                    throw new AuthenticationException("Invalid number of authorization parameters.");
                }


            }

            throw new AuthenticationException("No 'Authorization' entry found inside the request header.");
        }
    }
}
