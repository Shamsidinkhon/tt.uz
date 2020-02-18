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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>()
                .HasIndex(p => p.OwnerId);
            modelBuilder.Entity<VendorFavourite>()
                .HasIndex(p => p.TargetUserId);
            modelBuilder.Entity<UserFavourites>()
                .HasIndex(p => p.NewsId);
            modelBuilder.Entity<UserProfile>()
                .HasIndex(p => p.UserId);
            modelBuilder.Entity<ExternalLogin>()
                .HasIndex(p => p.UserId);
            modelBuilder.Entity<NewsAttribute>()
                .HasIndex(p => p.AttributeId);
            modelBuilder.Entity<Image>()
                .HasIndex(p => p.NewsId);
            modelBuilder.Entity<VendorReviews>()
                .HasIndex(p => p.TargetUserId);
            modelBuilder.Entity<NewsAttribute>()
                .HasIndex(p => p.NewsId);
        }

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
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<AttributeOption> AttributeOption { get; set; }
        public DbSet<CoreAttribute> CoreAttribute { get; set; }
        public DbSet<AttributeLink> AttributeLink { get; set; }
        public DbSet<NewsAttribute> NewsAttribute { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

    }
}
