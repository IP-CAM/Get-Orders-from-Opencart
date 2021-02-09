using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetOrdersFromOpencart.Model
{
    public class ConnectIdentity : IdentityDbContext<AppUser>
    {
        public ConnectIdentity(DbContextOptions<ConnectIdentity> options) : base(options)
        {

        }
    }
}
