using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebIncrementor.Models;
using WebIncrementor.Models.ViewModels;
using WebIncrementor.Security;
using WebIncrementor.Services;

namespace WebIncrementor.Controllers
{
    [Route("v1/[action]")]
    [ApiController]
    public class IncrementorController : ControllerBase
    {
        private static Object IncrementLock = new object();

        private IncrementorDBContext Db = new IncrementorDBContext();

        private IncrementorService mIncrementorSrv = null;
        public IncrementorService IncrementorSrv { get { if (mIncrementorSrv == null){ mIncrementorSrv = new IncrementorService(Db); } return mIncrementorSrv; } }

        private UserManager<ApplicationUser> mUserManager;
        private SignInManager<ApplicationUser> mSignInManager;


        public IncrementorController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        private async Task<string> GetCurrentUser()
        {
            var header = HttpContext.Request.Headers;

            if (header.ContainsKey("Authorization") && !string.IsNullOrEmpty(header["Authorization"].FirstOrDefault()))
            {
                char[] delimiters = { ' ', '\t' };
                string[] credentials = (header["Authorization"].FirstOrDefault()).Split(delimiters);
                
                if (credentials.Length < 2)
                {
                    throw new AuthenticationException("Invalid number of authorization parameters.");
                }

                ApplicationUser user = mUserManager.Users.Where(u => u.UserName == credentials[0]).FirstOrDefault();

                if (user != null)
                {
                    var result = await mSignInManager.PasswordSignInAsync(user, credentials[1], false, false);
                    if (result.Succeeded)
                    {
                        return user.Id;
                    }
                }

                throw new AuthenticationException("Could not login with the provided credentials.");
            }

            throw new AuthenticationException("No 'Authorization' entry found inside the request header.");
        }

        // GET v1/error
        [HttpGet]
        public ActionResult Error()
        {
            //TODO: GET ERROR

            JsonResult result = new JsonResult("ERROR");

            result.ContentType = "application/vnd.api+json";

            return result;
        }

        // POST v1/createUser
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] UserLogin userLogin)
        {
            if(mUserManager.Users.Where(user => user.UserName == userLogin.username).Any())
            {
                throw new ArgumentException("Cannot create new user as it already exists.");
            }

            ApplicationUser newUser = new ApplicationUser { UserName = userLogin.username };
            var result = await mUserManager.CreateAsync(newUser, userLogin.password);
            if (result.Succeeded)
            {
                return new JsonResult(new { data = new { message = "User created successfully" } });
            }

            return new JsonResult(new ErrorViewModel(result.Errors.Select(e => e.Description).FirstOrDefault()));
        }

        // GET v1/next
        [HttpGet]
        public async Task<IActionResult> Next()
        {
            Incrementor incrementor = null;
            string userId = await GetCurrentUser();

            if (Db.CanUseTransactions)
            {
                using (var transaction = Db.Database.BeginTransaction())
                {
                    incrementor = IncrementorSrv.IncrementValue(userId, true);
                    Db.SaveChanges();

                    transaction.Commit();
                }
            }
            else
            {
                lock (IncrementLock)
                {
                    incrementor = IncrementorSrv.IncrementValue(userId, true);
                    Db.SaveChanges();
                }
            }

            JsonResult result = null;
            if (incrementor == null)
            {
                result = new JsonResult(new ErrorViewModel(string.Format("Could not increment the value for {0}", userId)));
            }
            else
            {
                result = new JsonResult(new IncrementorViewModel(incrementor));
            }

            result.ContentType = "application/vnd.api+json";

            return result;
        }

        // GET v1/current
        [HttpGet]
        public async Task<IActionResult> Current()
        {
            Incrementor incrementor = null;
            string userId = await GetCurrentUser();
            
            incrementor = IncrementorSrv.GetIncrementor(userId, true);
            Db.SaveChanges();

            JsonResult result = null;
            if (incrementor == null)
            {
                result = new JsonResult(new ErrorViewModel(string.Format("Could not obtain the Incrementor for {0}", userId)));
            }
            else
            {
                result = new JsonResult(new IncrementorViewModel(incrementor));
            }

            result.ContentType = "application/vnd.api+json";

            return result;
        }

        // GET v1/get
        [HttpPut]
        public async Task<IActionResult> Current([FromForm] IncrementSetter setter)
        {
            Incrementor incrementor = null;
            string userId = await GetCurrentUser();

            if (Db.CanUseTransactions)
            {
                using (var transaction = Db.Database.BeginTransaction())
                {
                    incrementor = IncrementorSrv.SetIncrementorValue(userId, setter.current, true);
                    Db.SaveChanges();

                    transaction.Commit();
                }
            }
            else
            {
                lock (IncrementLock)
                {
                    incrementor = IncrementorSrv.SetIncrementorValue(userId, setter.current, true);
                    Db.SaveChanges();
                }
            }

            JsonResult result = null;
            if (incrementor == null)
            {
                result = new JsonResult(new ErrorViewModel(string.Format("Could not set the value to {1} for {0}", userId, setter.current)));
            }
            else
            {
                result = new JsonResult(new IncrementorViewModel(incrementor));
            }

            result.ContentType = "application/vnd.api+json";

            return result;
        }
    }

    public class UserLogin
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class IncrementSetter
    {
        public ulong current { get; set; }
    }
}
