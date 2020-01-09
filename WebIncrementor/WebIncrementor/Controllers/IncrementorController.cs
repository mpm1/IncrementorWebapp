using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebIncrementor.Models;
using WebIncrementor.Models.ViewModels;
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

        private string GetCurrentUser()
        {
            return "TODO: Change";
        }

        // GET v1/next
        [HttpGet]
        public ActionResult Next()
        {
            Incrementor incrementor = null;
            string userId = GetCurrentUser();

            if (Db.CanUseTransactions)
            {
                using (var transaction = Db.Database.BeginTransaction())
                {
                    incrementor = IncrementorSrv.IncrementValue(GetCurrentUser(), true);
                    Db.SaveChanges();

                    transaction.Commit();
                }
            }
            else
            {
                lock (IncrementLock)
                {
                    incrementor = IncrementorSrv.IncrementValue(GetCurrentUser(), true);
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
        public ActionResult Current()
        {
            Incrementor incrementor = null;
            string userId = GetCurrentUser();
            
            incrementor = IncrementorSrv.GetIncrementor(GetCurrentUser(), true);
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
        public ActionResult Current(int current)
        {
            throw new NotImplementedException();
        }
    }
}
