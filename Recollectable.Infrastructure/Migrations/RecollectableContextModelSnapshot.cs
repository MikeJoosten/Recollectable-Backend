﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Recollectable.Infrastructure.Data;

namespace Recollectable.Infrastructure.Migrations
{
    [DbContext(typeof(RecollectableContext))]
    partial class RecollectableContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("ReleaseDate")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CollectorValueId");

                    b.HasIndex("CountryId");

                    b.ToTable("Collectables");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Collectable");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.CollectionCollectable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CollectableId");

                    b.Property<Guid>("CollectionId");

                    b.Property<string>("Condition");

                    b.HasKey("Id");

                    b.HasIndex("CollectableId");

                    b.HasIndex("CollectionId");

                    b.ToTable("CollectionCollectables");

                    b.HasData(
                        new { Id = new Guid("1078b50b-1d89-4b24-b071-67af06348875"), CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), Condition = "MS62" },
                        new { Id = new Guid("b9104c81-4779-404f-95be-bd2605d3cbc8"), CollectableId = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), Condition = "Fine" },
                        new { Id = new Guid("c46c2819-af81-4a35-8e50-96f16abe6614"), CollectableId = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), Condition = "Uncirculated" },
                        new { Id = new Guid("583a957b-124f-49cb-955c-87d758819e87"), CollectableId = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), Condition = "VF24" },
                        new { Id = new Guid("6138b11e-769a-4a97-9e82-1ea5538cea92"), CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), CollectionId = new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), Condition = "Fine" },
                        new { Id = new Guid("c2781a82-f8e9-45c8-84ef-c2643b11c20f"), CollectableId = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), Condition = "VF24" }
                    );
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.CollectorValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("AU50");

                    b.Property<double?>("F12");

                    b.Property<double?>("G4");

                    b.Property<double?>("MS60");

                    b.Property<double?>("MS63");

                    b.Property<double?>("PF60");

                    b.Property<double?>("PF63");

                    b.Property<double?>("PF65");

                    b.Property<double?>("VF20");

                    b.Property<double?>("VG8");

                    b.Property<double?>("XF40");

                    b.HasKey("Id");

                    b.ToTable("CollectorValues");

                    b.HasData(
                        new { Id = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), F12 = 6.48, G4 = 6.48, MS60 = 16.0, MS63 = 18.0, VF20 = 6.48, VG8 = 6.48, XF40 = 15.0 },
                        new { Id = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), F12 = 50.0, G4 = 50.0, MS60 = 200.0, MS63 = 200.0, VF20 = 50.0, VG8 = 50.0, XF40 = 50.0 },
                        new { Id = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), PF60 = 75.0 },
                        new { Id = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), F12 = 760.0, G4 = 760.0, MS60 = 1650.0, MS63 = 1650.0, VF20 = 760.0, VG8 = 760.0, XF40 = 760.0 },
                        new { Id = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), F12 = 4.0, G4 = 3.0, MS60 = 40.0, MS63 = 165.0, VF20 = 4.5, VG8 = 3.5, XF40 = 13.5 }
                    );
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.Collection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Collections");

                    b.HasData(
                        new { Id = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), Type = "Coin", UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1") },
                        new { Id = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), Type = "Banknote", UserId = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1") },
                        new { Id = new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), Type = "Coin", UserId = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e") }
                    );
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Locations.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Countries");

                    b.HasData(
                        new { Id = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), Name = "Mexico" },
                        new { Id = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), Name = "Canada" },
                        new { Id = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), Name = "Ecuador" },
                        new { Id = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), Name = "United States of America" }
                    );
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
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

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

                    b.HasData(
                        new { Id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), AccessFailedCount = 0, ConcurrencyStamp = "b9d4206c-8db4-4838-a2ac-7514559631eb", Email = "ryan.haywood@gmail.com", EmailConfirmed = false, FirstName = "Ryan", LastName = "Haywood", LockoutEnabled = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false },
                        new { Id = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), AccessFailedCount = 0, ConcurrencyStamp = "09d21472-b0f1-4bf8-ba8d-e67767c1538e", Email = "jack.patillo@gmail.com", EmailConfirmed = false, FirstName = "Jack", LastName = "Patillo", LockoutEnabled = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false },
                        new { Id = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), AccessFailedCount = 0, ConcurrencyStamp = "6d6284be-8d03-4fc5-a803-5665340aa38e", Email = "geoff.ramsey@gmail.com", EmailConfirmed = false, FirstName = "Geoff", LastName = "Ramsey", LockoutEnabled = false, PhoneNumberConfirmed = false, TwoFactorEnabled = false }
                    );
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Currency", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Collectable");

                    b.Property<string>("BackImagePath")
                        .HasMaxLength(250);

                    b.Property<string>("Designer")
                        .HasMaxLength(250);

                    b.Property<int>("FaceValue");

                    b.Property<string>("FrontImagePath")
                        .HasMaxLength(250);

                    b.Property<string>("HeadOfState")
                        .HasMaxLength(250);

                    b.Property<string>("ObverseDescription")
                        .HasMaxLength(250);

                    b.Property<string>("ReverseDescription")
                        .HasMaxLength(250);

                    b.Property<string>("Size")
                        .HasMaxLength(25);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.ToTable("Currency");

                    b.HasDiscriminator().HasValue("Currency");
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Banknote", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Currency");

                    b.Property<string>("Color")
                        .HasMaxLength(250);

                    b.Property<string>("Signature")
                        .HasMaxLength(250);

                    b.Property<string>("Watermark")
                        .HasMaxLength(250);

                    b.ToTable("Banknote");

                    b.HasDiscriminator().HasValue("Banknote");

                    b.HasData(
                        new { Id = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), CollectorValueId = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), ReleaseDate = "1993", FaceValue = 50, HeadOfState = "Bill Clinton", ObverseDescription = "Ulysses S. Grant at center", ReverseDescription = "United States Capital Building", Type = "Dollars" },
                        new { Id = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), CollectorValueId = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), CountryId = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), ReleaseDate = "1988", FaceValue = 1000, HeadOfState = "Queen Elizabeth II", ObverseDescription = "Queen Elizabeth II, Parliament Library at right, Optical device with denomination at upper left, Arms at upper left center", ReverseDescription = "Pine grosbeak on branch at right", Type = "Dollars", Color = "Pink on multicolor underprint", Signature = "Thiessen-Crow" }
                    );
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.Coin", b =>
                {
                    b.HasBaseType("Recollectable.Core.Entities.Collectables.Currency");

                    b.Property<string>("EdgeLegend")
                        .HasMaxLength(100);

                    b.Property<string>("EdgeType")
                        .HasMaxLength(50);

                    b.Property<string>("Metal")
                        .HasMaxLength(50);

                    b.Property<string>("MintMark")
                        .HasMaxLength(50);

                    b.Property<int>("Mintage");

                    b.Property<string>("Note")
                        .HasMaxLength(250);

                    b.Property<string>("ObverseInscription")
                        .HasMaxLength(100);

                    b.Property<string>("ObverseLegend")
                        .HasMaxLength(100);

                    b.Property<string>("ReverseInscription")
                        .HasMaxLength(100);

                    b.Property<string>("ReverseLegend")
                        .HasMaxLength(100);

                    b.Property<string>("Subject")
                        .HasMaxLength(250);

                    b.Property<string>("Weight")
                        .HasMaxLength(25);

                    b.ToTable("Coin");

                    b.HasDiscriminator().HasValue("Coin");

                    b.HasData(
                        new { Id = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), CollectorValueId = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), CountryId = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), ReleaseDate = "1957", Designer = "Manuel L. Negrete", FaceValue = 5, HeadOfState = "Adolfo Ruiz Cortines", ObverseDescription = "National arms, eagle left", ReverseDescription = "Head left", Size = "36 mm.", Type = "Pesos", EdgeLegend = "INDEPENCIA Y LIBERTAD", Metal = "0.720 Silver 0.4151 oz. ASW", MintMark = "Mo.", Mintage = 200000, Subject = "100th Anniversary of Constitution", Weight = "18.05 g." },
                        new { Id = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), CollectorValueId = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), CountryId = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), ReleaseDate = "2009", FaceValue = 1, HeadOfState = "Rafael Correa", Size = "39 mm.", Type = "Sucre", Metal = "0.999 Silver 0.9925 oz. ASW", Mintage = 200, Subject = "Independence 200th Anniversary", Weight = "31.10 g." },
                        new { Id = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), CollectorValueId = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), ReleaseDate = "1924", Designer = "Adolph A. Weinman", FaceValue = 1, HeadOfState = "Calvin Coolidge", Size = "17.8 mm.", Type = "Dime", Metal = "0.900 Silver 0.0723 oz. ASW", Mintage = 24010000, Weight = "2.5 g." }
                    );
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

            modelBuilder.Entity("Recollectable.Core.Entities.Collectables.CollectionCollectable", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Collectables.Collectable", "Collectable")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectableId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Core.Entities.Collections.Collection", "Collection")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Core.Entities.Collections.Collection", b =>
                {
                    b.HasOne("Recollectable.Core.Entities.Users.User", "User")
                        .WithMany("Collections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
