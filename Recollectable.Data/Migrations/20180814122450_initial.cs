using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recollectable.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectorValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    G4 = table.Column<double>(nullable: true),
                    VG8 = table.Column<double>(nullable: true),
                    F12 = table.Column<double>(nullable: true),
                    VF20 = table.Column<double>(nullable: true),
                    XF40 = table.Column<double>(nullable: true),
                    AU50 = table.Column<double>(nullable: true),
                    MS60 = table.Column<double>(nullable: true),
                    MS63 = table.Column<double>(nullable: true),
                    PF60 = table.Column<double>(nullable: true),
                    PF63 = table.Column<double>(nullable: true),
                    PF65 = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectorValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Grade = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collectables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ReleaseDate = table.Column<string>(maxLength: 100, nullable: false),
                    CountryId = table.Column<Guid>(nullable: false),
                    CollectorValueId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FaceValue = table.Column<int>(nullable: true),
                    Type = table.Column<string>(maxLength: 100, nullable: true),
                    Size = table.Column<string>(maxLength: 25, nullable: true),
                    Designer = table.Column<string>(maxLength: 250, nullable: true),
                    HeadOfState = table.Column<string>(maxLength: 250, nullable: true),
                    ObverseDescription = table.Column<string>(maxLength: 250, nullable: true),
                    ReverseDescription = table.Column<string>(maxLength: 250, nullable: true),
                    FrontImagePath = table.Column<string>(maxLength: 250, nullable: true),
                    BackImagePath = table.Column<string>(maxLength: 250, nullable: true),
                    Color = table.Column<string>(maxLength: 250, nullable: true),
                    Watermark = table.Column<string>(maxLength: 250, nullable: true),
                    Signature = table.Column<string>(maxLength: 250, nullable: true),
                    Mintage = table.Column<int>(nullable: true),
                    Weight = table.Column<string>(maxLength: 25, nullable: true),
                    Metal = table.Column<string>(maxLength: 50, nullable: true),
                    Note = table.Column<string>(maxLength: 250, nullable: true),
                    Subject = table.Column<string>(maxLength: 250, nullable: true),
                    ObverseInscription = table.Column<string>(maxLength: 100, nullable: true),
                    ObverseLegend = table.Column<string>(maxLength: 100, nullable: true),
                    ReverseInscription = table.Column<string>(maxLength: 100, nullable: true),
                    ReverseLegend = table.Column<string>(maxLength: 100, nullable: true),
                    EdgeType = table.Column<string>(maxLength: 50, nullable: true),
                    EdgeLegend = table.Column<string>(maxLength: 100, nullable: true),
                    MintMark = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collectables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collectables_CollectorValues_CollectorValueId",
                        column: x => x.CollectorValueId,
                        principalTable: "CollectorValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collectables_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 25, nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionCollectables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false),
                    CollectableId = table.Column<Guid>(nullable: false),
                    ConditionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionCollectables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionCollectables_Collectables_CollectableId",
                        column: x => x.CollectableId,
                        principalTable: "Collectables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionCollectables_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionCollectables_Conditions_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Conditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collectables_CollectorValueId",
                table: "Collectables",
                column: "CollectorValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Collectables_CountryId",
                table: "Collectables",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCollectables_CollectableId",
                table: "CollectionCollectables",
                column: "CollectableId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCollectables_CollectionId",
                table: "CollectionCollectables",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionCollectables_ConditionId",
                table: "CollectionCollectables",
                column: "ConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collections_UserId",
                table: "Collections",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionCollectables");

            migrationBuilder.DropTable(
                name: "Collectables");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.DropTable(
                name: "CollectorValues");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
