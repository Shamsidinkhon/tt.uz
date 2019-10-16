﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using tt.uz.Helpers;

namespace tt.uz.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20191016075738_ExternalLoginCreate")]
    partial class ExternalLoginCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("tt.uz.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Icon");

                    b.Property<string>("Image");

                    b.Property<string>("MetaDescription");

                    b.Property<string>("MetaKeywords");

                    b.Property<string>("MetaTitle");

                    b.Property<string>("Name");

                    b.Property<int?>("ParentId");

                    b.Property<string>("ShortDescription");

                    b.Property<string>("Slug");

                    b.Property<int?>("Sort");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("tt.uz.Entities.ContactDetail", b =>
                {
                    b.Property<int>("ContactDetailId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email");

                    b.Property<bool>("IsIndividual");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("ContactDetailId");

                    b.ToTable("ContactDetails");
                });

            modelBuilder.Entity("tt.uz.Entities.ExternalLogin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClientId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FirstName");

                    b.Property<string>("FullName");

                    b.Property<string>("LastName");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ExternalLogin");
                });

            modelBuilder.Entity("tt.uz.Entities.Image", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("NewsId");

                    b.Property<string>("Path");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("ImageId");

                    b.HasIndex("NewsId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("tt.uz.Entities.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DistrictId");

                    b.Property<string>("Latitude");

                    b.Property<string>("Longtitude");

                    b.Property<int>("RegionId");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("LocationId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("tt.uz.Entities.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<int>("ContactDetailId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<int>("LocationId");

                    b.Property<int>("OwnerId");

                    b.Property<int>("PriceId");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ContactDetailId");

                    b.HasIndex("LocationId");

                    b.HasIndex("PriceId");

                    b.ToTable("News");
                });

            modelBuilder.Entity("tt.uz.Entities.Price", b =>
                {
                    b.Property<int>("PriceId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Currency");

                    b.Property<bool>("Exchange");

                    b.Property<bool>("Free");

                    b.Property<bool>("Negotiable");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("PriceId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("tt.uz.Entities.TempUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Phone");

                    b.HasKey("Id");

                    b.ToTable("TempUsers");
                });

            modelBuilder.Entity("tt.uz.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Phone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("tt.uz.Entities.VerificationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Code");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ExpireDate");

                    b.Property<string>("FieldType");

                    b.Property<string>("FieldValue");

                    b.HasKey("Id");

                    b.ToTable("VerificationCodes");
                });

            modelBuilder.Entity("tt.uz.Entities.Image", b =>
                {
                    b.HasOne("tt.uz.Entities.News", "News")
                        .WithMany()
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("tt.uz.Entities.News", b =>
                {
                    b.HasOne("tt.uz.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tt.uz.Entities.ContactDetail", "ContactDetail")
                        .WithMany()
                        .HasForeignKey("ContactDetailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tt.uz.Entities.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("tt.uz.Entities.Price", "Price")
                        .WithMany()
                        .HasForeignKey("PriceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
