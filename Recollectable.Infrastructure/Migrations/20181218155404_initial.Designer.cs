﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Recollectable.Infrastructure.Data;

namespace Recollectable.Infrastructure.Migrations
{
    [DbContext(typeof(RecollectableContext))]
    [Migration("20181218155404_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Collectable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CollectorValueId");

                    b.Property<Guid>("CountryId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("ReleaseDate");

                    b.HasKey("Id");

                    b.HasIndex("CollectorValueId");

                    b.HasIndex("CountryId");

                    b.ToTable("Collectables");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Collectable");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.CollectorValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("AU50");

                    b.Property<double>("F12");

                    b.Property<double>("G4");

                    b.Property<double>("MS60");

                    b.Property<double>("MS63");

                    b.Property<double>("PF60");

                    b.Property<double>("PF63");

                    b.Property<double>("PF65");

                    b.Property<double>("VF20");

                    b.Property<double>("VG8");

                    b.Property<double>("XF40");

                    b.HasKey("Id");

                    b.ToTable("CollectorValues");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.Collection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.CollectionCollectable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CollectableId");

                    b.Property<Guid>("CollectionId");

                    b.Property<Guid>("ConditionId");

                    b.HasKey("Id");

                    b.HasIndex("CollectableId");

                    b.HasIndex("CollectionId");

                    b.HasIndex("ConditionId");

                    b.ToTable("CollectionCollectables");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.Condition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Grade");

                    b.Property<string>("LanguageCode");

                    b.HasKey("Id");

                    b.ToTable("Conditions");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Locations.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Users.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Currency", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Collectable");

                    b.Property<string>("BackImagePath");

                    b.Property<string>("Designer");

                    b.Property<int>("FaceValue");

                    b.Property<string>("FrontImagePath");

                    b.Property<string>("HeadOfState");

                    b.Property<string>("ObverseDescription");

                    b.Property<string>("ReverseDescription");

                    b.Property<string>("Size");

                    b.Property<string>("Type");

                    b.ToTable("Currency");

                    b.HasDiscriminator().HasValue("Currency");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Banknote", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Currency");

                    b.Property<string>("Color");

                    b.Property<string>("Signature");

                    b.Property<string>("Watermark");

                    b.ToTable("Banknote");

                    b.HasDiscriminator().HasValue("Banknote");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Coin", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Currency");

                    b.Property<string>("EdgeLegend");

                    b.Property<string>("EdgeType");

                    b.Property<string>("Metal");

                    b.Property<string>("MintMark");

                    b.Property<int>("Mintage");

                    b.Property<string>("Note");

                    b.Property<string>("ObverseInscription");

                    b.Property<string>("ObverseLegend");

                    b.Property<string>("ReverseInscription");

                    b.Property<string>("ReverseLegend");

                    b.Property<string>("Subject");

                    b.Property<string>("Weight");

                    b.ToTable("Coin");

                    b.HasDiscriminator().HasValue("Coin");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Core.Entities.Users.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Collectable", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Collectables.CollectorValue", "CollectorValue")
                        .WithMany("Collectables")
                        .HasForeignKey("CollectorValueId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Core.Entities.Locations.Country", "Country")
                        .WithMany("Collectables")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.Collection", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.User", "User")
                        .WithMany("Collections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.CollectionCollectable", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Collectables.Collectable", "Collectable")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectableId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Core.Entities.Collections.Collection", "Collection")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Core.Entities.Collections.Condition", "Condition")
                        .WithMany()
                        .HasForeignKey("ConditionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
