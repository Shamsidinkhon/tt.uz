using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<TempUser> TempUsers { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<ExternalLogin> ExternalLogin { get; set; }
        public DbSet<UserFavourites> UserFavourites { get; set; }
        public DbSet<Tariff> Tariff { get; set; }
        public DbSet<VendorFavourite> VendorFavourite { get; set; }
        public DbSet<VendorReviews> VendorReviews { get; set; }

    }
}
