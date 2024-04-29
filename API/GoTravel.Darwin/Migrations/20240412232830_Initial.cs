using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GoTravel.Darwin.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "TimetableEntries",
                columns: table => new
                {
                    RID = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    TrainHeadcode = table.Column<string>(type: "text", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    LateReason = table.Column<int>(type: "integer", nullable: true),
                    CancelledReason = table.Column<int>(type: "integer", nullable: true),
                    Cancelled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableEntries", x => x.RID);
                });

            migrationBuilder.CreateTable(
                name: "TimetableEntryLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RID = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    PredictedArrival = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    PredictedDeparture = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ScheduledArrival = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ScheduledDeparture = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ActivityType = table.Column<string>(type: "text", nullable: false),
                    Platform = table.Column<string>(type: "text", nullable: true),
                    Cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    Delayed = table.Column<bool>(type: "boolean", nullable: false),
                    TimetableEntryRID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableEntryLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableEntryLocations_TimetableEntries_RID",
                        column: x => x.RID,
                        principalTable: "TimetableEntries",
                        principalColumn: "RID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimetableEntryLocations_TimetableEntries_TimetableEntryRID",
                        column: x => x.TimetableEntryRID,
                        principalTable: "TimetableEntries",
                        principalColumn: "RID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntries_Operator",
                table: "TimetableEntries",
                column: "Operator");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntryLocations_Location",
                table: "TimetableEntryLocations",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntryLocations_RID",
                table: "TimetableEntryLocations",
                column: "RID");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableEntryLocations_TimetableEntryRID",
                table: "TimetableEntryLocations",
                column: "TimetableEntryRID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimetableEntryLocations");

            migrationBuilder.DropTable(
                name: "TimetableEntries");
        }
    }
}
