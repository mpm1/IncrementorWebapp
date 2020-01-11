using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIncrementor.Models;

namespace WebIncrementor.Services
{
    public class IncrementorService
    {
        protected IncrementorDBContext Db;

        public IncrementorService(IncrementorDBContext db)
        {
            this.Db = db;
        }

        /// <summary>
        /// Obtains an Incrementor from the database. If one does not exists and the <param name="addUserIfNotExists"/> is true, we create the Incrementor.
        /// </summary>
        /// <param name="userId">The user to obtain the incrementor for.</param>
        /// <param name="addUserIfNotExists">Whether or not to create the user if needed.</param>
        /// <returns>The Incrementor for the user, or a newly created one if requested. Null otherwise.</returns>
        public Incrementor GetIncrementor(string userId, bool addUserIfNotExists = false)
        {
            Incrementor result = Db.Incrementors.Where(inc => inc.UserId == userId).FirstOrDefault();

            if(result == null && addUserIfNotExists)
            {
                result = new Incrementor()
                {
                    UserId = userId,
                    Value = 0
                };
                Db.Incrementors.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Obtrains an incrementor from the database and increments it's Value proptery by 1. Note: To avoid a race condition, wrap this call in a transaction from the Controller.
        /// </summary>
        /// <param name="userId">The user to increment a value for.</param>
        /// <param name="addUserIfNotExists">Creates a new incrementor if one for the current user does not exist.</param>
        /// <returns>The newly updated incrementor if found or created. Null otherwise.</returns>
        public Incrementor IncrementValue(string userId, bool addUserIfNotExists = false)
        {
            Incrementor incrementor = GetIncrementor(userId, addUserIfNotExists);

            if (incrementor != null)
            {
                incrementor.Value++;

                if (Db.Entry(incrementor).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                {
                    Db.Entry(incrementor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            return incrementor;
        }

        public Incrementor SetIncrementorValue(string userId, ulong value, bool addUserIfNotExists = false)
        {
            Incrementor incrementor = GetIncrementor(userId, addUserIfNotExists);

            if (incrementor != null)
            {
                incrementor.Value = value;

                if (Db.Entry(incrementor).State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                {
                    Db.Entry(incrementor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            return incrementor;
        }
    }
}
