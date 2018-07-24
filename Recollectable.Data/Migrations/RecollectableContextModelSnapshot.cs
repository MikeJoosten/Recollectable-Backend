﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Recollectable.Data;

namespace Recollectable.Data.Migrations
{
    [DbContext(typeof(RecollectableContext))]
    partial class RecollectableContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Recollectable.Domain.Collectable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CountryId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Collectables");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Collectable");
                });

            modelBuilder.Entity("Recollectable.Domain.Collection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type");

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

            modelBuilder.Entity("Recollectable.Domain.CollectionCollectable", b =>
                {
                    b.Property<Guid>("CollectionId");

                    b.Property<Guid>("CollectableId");

                    b.Property<Guid>("ConditionId");

                    b.HasKey("CollectionId", "CollectableId", "ConditionId");

                    b.HasIndex("CollectableId");

                    b.HasIndex("ConditionId");

                    b.ToTable("CollectionCollectables");

                    b.HasData(
                        new { CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), ConditionId = new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca") },
                        new { CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), CollectableId = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), ConditionId = new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") },
                        new { CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), CollectableId = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), ConditionId = new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d") },
                        new { CollectionId = new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), CollectableId = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), ConditionId = new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") },
                        new { CollectionId = new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), CollectableId = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), ConditionId = new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") },
                        new { CollectionId = new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), CollectableId = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), ConditionId = new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.CollectorValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("AU50Value");

                    b.Property<double?>("F12Value");

                    b.Property<double?>("G4Value");

                    b.Property<double?>("MS60Value");

                    b.Property<double?>("MS63Value");

                    b.Property<double?>("PF63Value");

                    b.Property<double?>("PF65Value");

                    b.Property<double?>("VF20Value");

                    b.Property<double?>("VG8Value");

                    b.Property<double?>("XF40Value");

                    b.HasKey("Id");

                    b.ToTable("CollectorValues");

                    b.HasData(
                        new { Id = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), F12Value = 6.48, G4Value = 6.48, MS60Value = 16.0, MS63Value = 18.0, VF20Value = 6.48, VG8Value = 6.48, XF40Value = 15.0 },
                        new { Id = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), F12Value = 50.0, G4Value = 50.0, MS60Value = 200.0, MS63Value = 200.0, VF20Value = 50.0, VG8Value = 50.0, XF40Value = 50.0 },
                        new { Id = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), F12Value = 544.61, G4Value = 544.61, MS60Value = 825.0, MS63Value = 850.0, PF63Value = 900.0, PF65Value = 1000.0, VF20Value = 544.61, VG8Value = 544.61, XF40Value = 544.61 },
                        new { Id = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), F12Value = 760.0, G4Value = 760.0, MS60Value = 1650.0, MS63Value = 1650.0, VF20Value = 760.0, VG8Value = 760.0, XF40Value = 760.0 },
                        new { Id = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), F12Value = 4.0, G4Value = 3.0, MS60Value = 40.0, MS63Value = 165.0, VF20Value = 4.5, VG8Value = 3.5, XF40Value = 13.5 }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.Condition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Grade");

                    b.HasKey("Id");

                    b.ToTable("Conditions");

                    b.HasData(
                        new { Id = new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca"), Grade = "MS62" },
                        new { Id = new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a"), Grade = "VF24" },
                        new { Id = new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a"), Grade = "Uncirculated" },
                        new { Id = new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d"), Grade = "Fine" }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Countries");

                    b.HasData(
                        new { Id = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), Description = "", Name = "Mexico" },
                        new { Id = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), Description = "", Name = "Canada" },
                        new { Id = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), Description = "", Name = "Thailand" },
                        new { Id = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), Description = "", Name = "United States of America" }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new { Id = new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), Email = "ryan.haywood@gmail.com", FirstName = "Ryan", LastName = "Haywood" },
                        new { Id = new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), Email = "jack.patillo@gmail.com", FirstName = "Jack", LastName = "Patillo" },
                        new { Id = new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), Email = "geoff.ramsey@gmail.com", FirstName = "Geoff", LastName = "Ramsey" }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.Currency", b =>
                {
                    b.HasBaseType("Recollectable.Domain.Collectable");

                    b.Property<string>("BackImagePath");

                    b.Property<Guid>("CollectorValueId");

                    b.Property<int>("FaceValue");

                    b.Property<string>("FrontImagePath");

                    b.Property<string>("ObverseDescription");

                    b.Property<int>("ReleaseDate");

                    b.Property<string>("ReverseDescription");

                    b.Property<string>("Size");

                    b.Property<string>("Type");

                    b.HasIndex("CollectorValueId");

                    b.ToTable("Currency");

                    b.HasDiscriminator().HasValue("Currency");
                });

            modelBuilder.Entity("Recollectable.Domain.Banknote", b =>
                {
                    b.HasBaseType("Recollectable.Domain.Currency");

                    b.Property<string>("Color");

                    b.Property<string>("Signature");

                    b.Property<string>("Watermark");

                    b.ToTable("Banknote");

                    b.HasDiscriminator().HasValue("Banknote");

                    b.HasData(
                        new { Id = new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), BackImagePath = "", CollectorValueId = new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), FaceValue = 50, FrontImagePath = "", ObverseDescription = "Ulysses S. Grant at center", ReleaseDate = 1993, ReverseDescription = "United States Capital Building", Size = "", Type = "Dollars", Color = "", Signature = "", Watermark = "" },
                        new { Id = new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), CountryId = new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), BackImagePath = "", CollectorValueId = new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), FaceValue = 1000, FrontImagePath = "", ObverseDescription = "Queen Elizabeth II, Parliament Library at right, Optical device with denomination at upper left, Arms at upper left center", ReleaseDate = 1988, ReverseDescription = "Pine grosbeak on branch at right", Size = "", Type = "Dollars", Color = "Pink on multicolor underprint", Signature = "Thiessen-Crow", Watermark = "" }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.Coin", b =>
                {
                    b.HasBaseType("Recollectable.Domain.Currency");

                    b.Property<string>("Designer");

                    b.Property<string>("EdgeLegend");

                    b.Property<string>("EdgeType");

                    b.Property<string>("Metal");

                    b.Property<int>("Mintage");

                    b.Property<string>("Note");

                    b.Property<string>("ObverseLegend");

                    b.Property<string>("ReverseLegend");

                    b.Property<string>("Subject");

                    b.Property<string>("Weight");

                    b.ToTable("Coin");

                    b.HasDiscriminator().HasValue("Coin");

                    b.HasData(
                        new { Id = new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), CountryId = new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), BackImagePath = "", CollectorValueId = new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), FaceValue = 5, FrontImagePath = "", ObverseDescription = "National arms, eagle left", ReleaseDate = 1957, ReverseDescription = "Head left", Size = "36 mm.", Type = "Pesos", Designer = "Manuel L. Negrete", EdgeLegend = "INDEPENCIA Y LIBERTAD", EdgeType = "", Metal = "0.720 Silver 0.4151 oz. ASW", Mintage = 200000, Note = "Mint mark Mo.", ObverseLegend = "", ReverseLegend = "", Subject = "100th Anniversary of Constitution", Weight = "18.05 g." },
                        new { Id = new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), CountryId = new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), BackImagePath = "", CollectorValueId = new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), FaceValue = 6000, FrontImagePath = "", ObverseDescription = "Kneeling and seated figures within circle", ReleaseDate = 1987, ReverseDescription = "Emblem", Size = "26 mm.", Type = "Baht", Designer = "", EdgeLegend = "", EdgeType = "Reeded", Metal = "0.900 Gold 0.4312 oz. AGW", Mintage = 700, Note = "", ObverseLegend = "", ReverseLegend = "", Subject = "Asian Institute of Technology", Weight = "15.00 g." },
                        new { Id = new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), CountryId = new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), BackImagePath = "", CollectorValueId = new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), FaceValue = 1, FrontImagePath = "", ObverseDescription = "", ReleaseDate = 1924, ReverseDescription = "", Size = "17.8 mm.", Type = "Dime", Designer = "Adolph A. Weinman", EdgeLegend = "", EdgeType = "", Metal = "0.900 Silver 0.0723 oz. ASW", Mintage = 24010000, Note = "", ObverseLegend = "", ReverseLegend = "", Subject = "", Weight = "2.5 g." }
                    );
                });

            modelBuilder.Entity("Recollectable.Domain.Collectable", b =>
                {
                    b.HasOne("Recollectable.Domain.Country", "Country")
                        .WithMany("Collectables")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Domain.Collection", b =>
                {
                    b.HasOne("Recollectable.Domain.User", "User")
                        .WithMany("Collections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Domain.CollectionCollectable", b =>
                {
                    b.HasOne("Recollectable.Domain.Collectable", "Collectable")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectableId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Domain.Collection", "Collection")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Recollectable.Domain.Condition", "Condition")
                        .WithMany("CollectionCollectables")
                        .HasForeignKey("ConditionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Recollectable.Domain.Currency", b =>
                {
                    b.HasOne("Recollectable.Domain.CollectorValue", "CollectorValue")
                        .WithMany("Currencies")
                        .HasForeignKey("CollectorValueId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
