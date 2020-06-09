using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vectio.auth.api.db.entities
{
    public class User : IdentityUser
    {
        [StringLength(100)]
        public string Fullname { get; set; }

    }
}
