using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tt.uz.Entities;

namespace tt.admin.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<tt.uz.Entities.News> News { get; set; }
        public DbSet<tt.uz.Entities.Category> Categories { get; set; }
        public DbSet<tt.uz.Entities.Price> Prices { get; set; }
        public DbSet<tt.uz.Entities.Location> Locations { get; set; }
        public DbSet<tt.uz.Entities.ContactDetail> ContactDetails { get; set; }
        public DbSet<tt.uz.Entities.Image> Images { get; set; }
        public DbSet<tt.uz.Entities.User> User { get; set; }
        public DbSet<tt.uz.Entities.CoreAttribute> CoreAttribute { get; set; }
        public DbSet<tt.uz.Entities.AttributeOption> AttributeOption { get; set; }
        public DbSet<tt.uz.Entities.AttributeLink> AttributeLink { get; set; }

    }
}
