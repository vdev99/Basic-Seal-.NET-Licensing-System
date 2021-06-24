using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }


        public DbSet<SoftwareLicenses> SoftwareLicenses { get; set; }
        public DbSet<Softwares> Softwares { get; set; }
        public DbSet<SoftwareVersions> SoftwareVersions { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
