using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RostamBot.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            SuspicousAccountBlockHistories = new HashSet<SuspicousAccountBlockHistory>();
        }

        public virtual IEnumerable<SuspicousAccountBlockHistory> SuspicousAccountBlockHistories { get; private set; }

    }
}
