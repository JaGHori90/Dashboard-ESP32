using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Airpressure",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "Temperatur",
                table: "Sensor");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sensor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Measurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sensor_ID = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<double>(type: "float", nullable: false),
                    AirPressure = table.Column<double>(type: "float", nullable: false),
                    MeasurmentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Measurements_Sensor_Sensor_ID",
                        column: x => x.Sensor_ID,
                        principalTable: "Sensor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_Sensor_ID",
                table: "Measurements",
                column: "Sensor_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurements");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sensor");

            migrationBuilder.AddColumn<double>(
                name: "Airpressure",
                table: "Sensor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Humidity",
                table: "Sensor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperatur",
                table: "Sensor",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
