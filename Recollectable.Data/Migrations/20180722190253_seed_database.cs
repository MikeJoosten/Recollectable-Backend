using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recollectable.Data.Migrations
{
    public partial class seed_database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CollectorValues",
                columns: new[] { "Id", "AU50Value", "F12Value", "G4Value", "MS60Value", "MS63Value", "PF63Value", "PF65Value", "VF20Value", "VG8Value", "XF40Value" },
                values: new object[,]
                {
                    { new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), null, 6.48, 6.48, 16.0, 18.0, null, null, 6.48, 6.48, 15.0 },
                    { new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), null, 50.0, 50.0, 200.0, 200.0, null, null, 50.0, 50.0, 50.0 },
                    { new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), null, 544.61, 544.61, 825.0, 850.0, 900.0, 1000.0, 544.61, 544.61, 544.61 },
                    { new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), null, 760.0, 760.0, 1650.0, 1650.0, null, null, 760.0, 760.0, 760.0 },
                    { new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), null, 4.0, 3.0, 40.0, 165.0, null, null, 4.5, 3.5, 13.5 }
                });

            migrationBuilder.InsertData(
                table: "Conditions",
                columns: new[] { "Id", "Grade" },
                values: new object[,]
                {
                    { new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca"), "MS62" },
                    { new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a"), "VF24" },
                    { new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a"), "Uncirculated" },
                    { new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d"), "Fine" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), "", "Mexico" },
                    { new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), "", "Canada" },
                    { new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), "", "Thailand" },
                    { new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), "", "United States of America" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), "ryan.haywood@gmail.com", "Ryan", "Haywood" },
                    { new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"), "jack.patillo@gmail.com", "Jack", "Patillo" },
                    { new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), "geoff.ramsey@gmail.com", "Geoff", "Ramsey" }
                });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CountryId", "Discriminator", "BackImagePath", "CollectorValueId", "FaceValue", "FrontImagePath", "ObverseDescription", "ReleaseDate", "ReverseDescription", "Size", "Type", "Designer", "EdgeLegend", "EdgeType", "Metal", "Mintage", "Note", "ObverseLegend", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("406b1c0f-5505-43eb-a780-6ae1b72cf91b"), "Coin", "", new Guid("3ba282c2-4648-49f1-83ad-045ed612f31a"), 5, "", "National arms, eagle left", 1957, "Head left", "36 mm.", "Pesos", "Manuel L. Negrete", "INDEPENCIA Y LIBERTAD", "", "0.720 Silver 0.4151 oz. ASW", 200000, "Mint mark Mo.", "", "", "100th Anniversary of Constitution", "18.05 g." });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CountryId", "Discriminator", "BackImagePath", "CollectorValueId", "FaceValue", "FrontImagePath", "ObverseDescription", "ReleaseDate", "ReverseDescription", "Size", "Type", "Color", "Signature", "Watermark" },
                values: new object[] { new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), new Guid("e8a1c283-2300-4f3f-b408-59d0f8ccd893"), "Banknote", "", new Guid("08aeaba0-a480-4dd8-b7be-8215ddb7fca4"), 1000, "", "Queen Elizabeth II, Parliament Library at right, Optical device with denomination at upper left, Arms at upper left center", 1988, "Pine grosbeak on branch at right", "", "Dollars", "Pink on multicolor underprint", "Thiessen-Crow", "" });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CountryId", "Discriminator", "BackImagePath", "CollectorValueId", "FaceValue", "FrontImagePath", "ObverseDescription", "ReleaseDate", "ReverseDescription", "Size", "Type", "Designer", "EdgeLegend", "EdgeType", "Metal", "Mintage", "Note", "ObverseLegend", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), new Guid("18d9e209-e798-44ed-bf2e-65798f8717c0"), "Coin", "", new Guid("26aabce7-03cb-470f-9e4e-2d65095a37c9"), 6000, "", "Kneeling and seated figures within circle", 1987, "Emblem", "26 mm.", "Baht", "", "", "Reeded", "0.900 Gold 0.4312 oz. AGW", 700, "", "", "", "Asian Institute of Technology", "15.00 g." });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CountryId", "Discriminator", "BackImagePath", "CollectorValueId", "FaceValue", "FrontImagePath", "ObverseDescription", "ReleaseDate", "ReverseDescription", "Size", "Type", "Color", "Signature", "Watermark" },
                values: new object[] { new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), "Banknote", "", new Guid("e92b30b7-5a08-41aa-8407-f10b6efa1571"), 50, "", "Ulysses S. Grant at center", 1993, "United States Capital Building", "", "Dollars", "", "", "" });

            migrationBuilder.InsertData(
                table: "Collectables",
                columns: new[] { "Id", "CountryId", "Discriminator", "BackImagePath", "CollectorValueId", "FaceValue", "FrontImagePath", "ObverseDescription", "ReleaseDate", "ReverseDescription", "Size", "Type", "Designer", "EdgeLegend", "EdgeType", "Metal", "Mintage", "Note", "ObverseLegend", "ReverseLegend", "Subject", "Weight" },
                values: new object[] { new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), new Guid("5626595c-a6b1-44ba-b60d-87b5b35fe208"), "Coin", "", new Guid("8bf1ae62-5493-4e08-83b8-65bf9c267c32"), 1, "", "", 1924, "", "17.8 mm.", "Dime", "Adolph A. Weinman", "", "", "0.900 Silver 0.0723 oz. ASW", 24010000, "", "", "", "", "2.5 g." });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "Id", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), "Coin" },
                    { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"), "Banknote" },
                    { new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"), "Coin" }
                });

            migrationBuilder.InsertData(
                table: "CollectionCollectables",
                columns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                values: new object[,]
                {
                    { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca") },
                    { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d") },
                    { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") },
                    { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") },
                    { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") },
                    { new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("4e6b10c3-0758-4a33-9b10-861d23b57ac2"), new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("84a3c9a9-f6e6-4b2f-b65d-1b82df56dc79"), new Guid("db0c31f2-5707-4111-8cb5-87f9201e7941"), new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("9e83160d-49e8-4c76-b264-709fb44b3b60"), new Guid("14db50bc-7b1a-4b65-8d6f-bf5e3412c610"), new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("4c8e3fe4-aa96-4c33-9e4e-7ab284a653d5"), new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a") });

            migrationBuilder.DeleteData(
                table: "CollectionCollectables",
                keyColumns: new[] { "CollectionId", "CollectableId", "ConditionId" },
                keyValues: new object[] { new Guid("e24235ad-b12d-40b9-8fbc-15d1c858dc3d"), new Guid("ad95d611-1778-4f9d-990f-ded3c914d7b1"), new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2e795c80-8c60-4d18-bd10-ca5832ab4158"));

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
                keyValue: new Guid("1f5713f4-3aec-4c6b-be0b-139e6221b1ca"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("3f7a2032-1301-427e-abe7-d450293a2d0d"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("58311fda-5c79-4beb-b8be-eb0799d3334a"));

            migrationBuilder.DeleteData(
                table: "Conditions",
                keyColumn: "Id",
                keyValue: new Guid("d8fd0831-f82e-40ec-a85a-71273ce26e8a"));

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

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4a9522da-66f9-4dfb-88b8-f92b950d1df1"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e640b01f-9eb8-407f-a8f9-68197a7fe48e"));
        }
    }
}
