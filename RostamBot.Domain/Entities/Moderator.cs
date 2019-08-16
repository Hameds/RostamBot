using System;
using System.Collections.Generic;

namespace RostamBot.Domain.Entities
{
    public class Moderator
    {
        public Moderator()
        {
            Supervisions = new HashSet<Supervision>();
        }


        public Guid Id { get; set; }

        public string TwitterScreenName { get; set; }

        public long TwitterUserId { get; set; }



        public virtual IEnumerable<Supervision> Supervisions { get; private set; }

    }
}
