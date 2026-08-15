using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consolidation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyConsolidate",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    AccumulatedBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyConsolidate", x => x.Date);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyConsolidate");
        }
    }
}
