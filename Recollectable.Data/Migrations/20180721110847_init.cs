using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recollectable.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectorValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    G4Value = table.Column<double>(nullable: false),
                    VG8Value = table.Column<double>(nullable: false),
                    F12Value = table.Column<double>(nullable: false),
                    VF20Value = table.Column<double>(nullable: false),
                    XF40Value = table.Column<double>(nullable: false),
                    AU50Value = table.Column<double>(nullable: false),
                    MS60Value = table.Column<double>(nullable: false),
                    MS65Value = table.Column<double>(nullable: false),
                    PF63Value = table.Column<double>(nullable: false),
                    PF65Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectorValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Condition",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Grade = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
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
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Collectables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CountryId = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FaceValue = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<int>(nullable: true),
                    CollectorValueId = table.Column<Guid>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    ObverseDescription = table.Column<string>(nullable: true),
                    ReverseDescription = table.Column<string>(nullable: true),
                    FrontImagePath = table.Column<string>(nullable: true),
                    BackImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collectables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Collectables_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collectables_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Collectables_CollectorValues_CollectorValueId",
                        column: x => x.CollectorValueId,
                        principalTable: "CollectorValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectableCondition",
                columns: table => new
                {
                    ConditionId = table.Column<Guid>(nullable: false),
                    CollectableId = table.Column<Guid>(nullable: false),
                    CollectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectableCondition", x => new { x.ConditionId, x.CollectableId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_CollectableCondition_Collectables_CollectableId",
                        column: x => x.CollectableId,
                        principalTable: "Collectables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectableCondition_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectableCondition_Condition_ConditionId",
                        column: x => x.ConditionId,
                        principalTable: "Condition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectableCondition_CollectableId",
                table: "CollectableCondition",
                column: "CollectableId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectableCondition_CollectionId",
                table: "CollectableCondition",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collectables_CollectionId",
                table: "Collectables",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Collectables_CountryId",
                table: "Collectables",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Collectables_CollectorValueId",
                table: "Collectables",
                column: "CollectorValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CollectionId",
                table: "Users",
                column: "CollectionId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectableCondition");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Collectables");

            migrationBuilder.DropTable(
                name: "Condition");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "CollectorValues");
        }
    }
}
