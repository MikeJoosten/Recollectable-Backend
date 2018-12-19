using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recollectable.Infrastructure.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0257e71c-37ee-4eca-8ed4-dee17f4d2cea"), "5846a5a9-3362-4a0d-b30f-de28c60267eb", "Admin", "ADMIN" },
                    { new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c"), "3e08623b-1f76-49ba-b2d0-bed011492b99", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), 0, "396c0691-a06a-4637-9f73-ab3cc3912b38", "geoff.ramsey@gmail.com", true, "Geoff", "Ramsey", true, null, "GEOFF.RAMSEY@GMAIL.COM", "GEOFF", "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A==", null, false, "EI5SZZYU4EEWLBVXIJGX6PFPIHJETER3", false, "Geoff" },
                    { new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), 0, "d3f00b4c-49ce-4051-bf7b-8360dc5929dc", "ryan.haywood@gmail.com", true, "Ryan", "Haywood", true, null, "RYAN.HAYWOOD@GMAIL.COM", "ryan", "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A==", null, false, "EI5SZZYU4EEWLBVXIJGX6PFPIHJETER3", false, "Ryan" },
                    { new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), 0, "65f91f23-e371-4a8a-80a6-8becc57b0350", "jack.patillo@gmail.com", true, "Jack", "Patillo", true, null, "JACK.PATILLO@GMAIL.COM", "JACK", "AQAAAAEAACcQAAAAELwS6EP+EIxLwIETUOFZqrcBwoIGtfFj8jZfzxvARPsm9FJxn3HIWgxrq5+A8Rie7A==", null, false, "EI5SZZYU4EEWLBVXIJGX6PFPIHJETER3", false, "Jack" }
                });

            migrationBuilder.InsertData(
                table: "CollectorValues",
                columns: new[] { "Id", "AU50", "F12", "G4", "MS60", "MS63", "PF60", "PF63", "PF65", "VF20", "VG8", "XF40" },
                values: new object[,]
                {
                    { new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), 0.0, 6.48, 6.48, 16.0, 18.0, 0.0, 0.0, 0.0, 6.48, 6.48, 15.0 },
                    { new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), 0.0, 4.0, 3.0, 40.0, 165.0, 0.0, 0.0, 0.0, 4.5, 3.5, 13.5 },
                    { new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), 0.0, 760.0, 760.0, 1650.0, 1650.0, 0.0, 0.0, 0.0, 760.0, 760.0, 760.0 },
                    { new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), 0.0, 0.0, 0.0, 0.0, 0.0, 75.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                    { new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), 0.0, 50.0, 50.0, 200.0, 200.0, 0.0, 0.0, 0.0, 50.0, 50.0, 50.0 }
                });

            migrationBuilder.InsertData(
                table: "Conditions",
                columns: new[] { "Id", "Grade", "LanguageCode" },
                values: new object[,]
                {
                    { new Guid("b5ef8ac8-c2ce-4926-a66a-e5f66f7b0dcb"), "Uncirculated", "en-GB" },
                    { new Guid("0853d1fe-a59f-4e5f-8e93-e31ec69fd732"), "VF24", "en-US" },
                    { new Guid("24f7b017-43cb-4fdb-a7a0-2d0169c4d5ae"), "Fine", "en-GB" },
                    { new Guid("4e35ee38-1778-41ca-a858-5a2414de499c"), "MS62", "en-US" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), null, "Mexico" },
                    { new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), null, "Canada" },
                    { new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), null, "Ecuador" },
                    { new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), null, "United States of America" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), new Guid("0257e71c-37ee-4eca-8ed4-dee17f4d2cea") },
                    { new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c") },
                    { new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c") }
                });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CollectorValueId", "CountryId", "Discriminator", "ReleaseDate", "BackImagePath", "Designer", "FaceValue", "FrontImagePath", "HeadOfState", "ObverseDescription", "ReverseDescription", "Size", "Type", "EdgeLegend", "EdgeType", "Metal", "MintMark", "Mintage", "Note", "ObverseInscription", "ObverseLegend", "ReverseInscription", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), "Coin", "1957", null, "Manuel L. Negrete", 5, null, "Adolfo Ruiz Cortines", "National arms, eagle left", "Head left", "36 mm.", "Pesos", "INDEPENCIA Y LIBERTAD", null, "0.720 Silver 0.4151 oz. ASW", "Mo.", 200000, null, null, null, null, null, "100th Anniversary of Constitution", "18.05 g." });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CollectorValueId", "CountryId", "Discriminator", "ReleaseDate", "BackImagePath", "Designer", "FaceValue", "FrontImagePath", "HeadOfState", "ObverseDescription", "ReverseDescription", "Size", "Type", "Color", "Signature", "Watermark" },
                values: new object[] { new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), "Banknote", "1988", null, null, 1000, null, "Queen Elizabeth II", "Queen Elizabeth II, Parliament Library at right, Optical device with denomination at upper left, Arms at upper left center", "Pine grosbeak on branch at right", null, "Dollars", "Pink on multicolor underprint", "Thiessen-Crow", null });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CollectorValueId", "CountryId", "Discriminator", "ReleaseDate", "BackImagePath", "Designer", "FaceValue", "FrontImagePath", "HeadOfState", "ObverseDescription", "ReverseDescription", "Size", "Type", "EdgeLegend", "EdgeType", "Metal", "MintMark", "Mintage", "Note", "ObverseInscription", "ObverseLegend", "ReverseInscription", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), "Coin", "2009", null, null, 1, null, "Rafael Correa", null, null, "39 mm.", "Sucre", null, null, "0.999 Silver 0.9925 oz. ASW", null, 200, null, null, null, null, null, "Independence 200th Anniversary", "31.10 g." });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CollectorValueId", "CountryId", "Discriminator", "ReleaseDate", "BackImagePath", "Designer", "FaceValue", "FrontImagePath", "HeadOfState", "ObverseDescription", "ReverseDescription", "Size", "Type", "Color", "Signature", "Watermark" },
                values: new object[] { new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), "Banknote", "1993", null, null, 50, null, "Bill Clinton", "Ulysses S. Grant at center", "United States Capital Building", null, "Dollars", null, null, null });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CollectorValueId", "CountryId", "Discriminator", "ReleaseDate", "BackImagePath", "Designer", "FaceValue", "FrontImagePath", "HeadOfState", "ObverseDescription", "ReverseDescription", "Size", "Type", "EdgeLegend", "EdgeType", "Metal", "MintMark", "Mintage", "Note", "ObverseInscription", "ObverseLegend", "ReverseInscription", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), "Coin", "1924", null, "Adolph A. Weinman", 1, null, "Calvin Coolidge", null, null, "17.8 mm.", "Dime", null, null, "0.900 Silver 0.0723 oz. ASW", null, 24010000, null, null, null, null, null, null, "2.5 g." });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "Id", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), "Coin", new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1") },
                    { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), "Banknote", new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1") },
                    { new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), "Coin", new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e") }
                });

            migrationBuilder.InsertData(
                table: "CollectionCollectables",
                columns: new[] { "Id", "CollectableId", "CollectionId", "ConditionId" },
                values: new object[,]
                {
                    { new Guid("1078b50b-1d89-4b24-b071-67af06348875"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("4e35ee38-1778-41ca-a858-5a2414de499c") },
                    { new Guid("c46c2819-af81-4a35-8e50-96f16abe6614"), new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("b5ef8ac8-c2ce-4926-a66a-e5f66f7b0dcb") },
                    { new Guid("c2781a82-f8e9-45c8-84ef-c2643b11c20f"), new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("0853d1fe-a59f-4e5f-8e93-e31ec69fd732") },
                    { new Guid("b9104c81-4779-404f-95be-bd2605d3cbc8"), new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("24f7b017-43cb-4fdb-a7a0-2d0169c4d5ae") },
                    { new Guid("583a957b-124f-49cb-955c-87d758819e87"), new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("0853d1fe-a59f-4e5f-8e93-e31ec69fd732") },
                    { new Guid("6138b11e-769a-4a97-9e82-1ea5538cea92"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), new Guid("24f7b017-43cb-4fdb-a7a0-2d0169c4d5ae") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), new Guid("0257e71c-37ee-4eca-8ed4-dee17f4d2cea") });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("1078b50b-1d89-4b24-b071-67af06348875"));

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("583a957b-124f-49cb-955c-87d758819e87"));

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("6138b11e-769a-4a97-9e82-1ea5538cea92"));

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("b9104c81-4779-404f-95be-bd2605d3cbc8"));

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("c2781a82-f8e9-45c8-84ef-c2643b11c20f"));

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumn: "Id",
                keyValue: new Guid("c46c2819-af81-4a35-8e50-96f16abe6614"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { new Guid("0257e71c-37ee-4eca-8ed4-dee17f4d2cea"), "5846a5a9-3362-4a0d-b30f-de28c60267eb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { new Guid("0e031ce4-ce3f-4b73-b3fb-75e4703b8d3c"), "3e08623b-1f76-49ba-b2d0-bed011492b99" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), "65f91f23-e371-4a8a-80a6-8becc57b0350" });

            migrationBuilder.DeleteData(
                table: "Collectables",
                keyColumn: "Id",
                keyValue: new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"));

            migrationBuilder.DeleteData(
                table: "Collectables",
                keyColumn: "Id",
                keyValue: new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"));

            migrationBuilder.DeleteData(
                table: "Collectables",
                keyColumn: "Id",
                keyValue: new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"));

            migrationBuilder.DeleteData(
                table: "Collectables",
                keyColumn: "Id",
                keyValue: new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"));

            migrationBuilder.DeleteData(
                table: "Collectables",
                keyColumn: "Id",
                keyValue: new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"));

            migrationBuilder.DeleteData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"));

            migrationBuilder.DeleteData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"));

            migrationBuilder.DeleteData(
                table: "Collections",
                keyColumn: "Id",
                keyValue: new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("0853d1fe-a59f-4e5f-8e93-e31ec69fd732"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("24f7b017-43cb-4fdb-a7a0-2d0169c4d5ae"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("4e35ee38-1778-41ca-a858-5a2414de499c"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("b5ef8ac8-c2ce-4926-a66a-e5f66f7b0dcb"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), "d3f00b4c-49ce-4051-bf7b-8360dc5929dc" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumns: new[] { "Id", "ConcurrencyStamp" },
                keyValues: new object[] { new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), "396c0691-a06a-4637-9f73-ab3cc3912b38" });

            migrationBuilder.DeleteData(
                table: "CollectorValues",
                keyColumn: "Id",
                keyValue: new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"));

            migrationBuilder.DeleteData(
                table: "CollectorValues",
                keyColumn: "Id",
                keyValue: new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"));

            migrationBuilder.DeleteData(
                table: "CollectorValues",
                keyColumn: "Id",
                keyValue: new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"));

            migrationBuilder.DeleteData(
                table: "CollectorValues",
                keyColumn: "Id",
                keyValue: new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"));

            migrationBuilder.DeleteData(
                table: "CollectorValues",
                keyColumn: "Id",
                keyValue: new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"));
        }
    }
}
