using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasurments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Sensor_Sensor_ID",
                table: "Measurements");

            migrationBuilder.RenameColumn(
                name: "Sensor_ID",
                table: "Measurements",
                newName: "SensorId");

            migrationBuilder.RenameColumn(
                name: "MeasurmentAt",
                table: "Measurements",
                newName: "MeasuredAt");

            migrationBuilder.RenameIndex(
                name: "IX_Measurements_Sensor_ID",
                table: "Measurements",
                newName: "IX_Measurements_SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Sensor_SensorId",
                table: "Measurements",
                column: "SensorId",
                principalTable: "Sensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Sensor_SensorId",
                table: "Measurements");

            migrationBuilder.RenameColumn(
                name: "SensorId",
                table: "Measurements",
                newName: "Sensor_ID");

            migrationBuilder.RenameColumn(
                name: "MeasuredAt",
                table: "Measurements",
                newName: "MeasurmentAt");

            migrationBuilder.RenameIndex(
                name: "IX_Measurements_SensorId",
                table: "Measurements",
                newName: "IX_Measurements_Sensor_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Sensor_Sensor_ID",
                table: "Measurements",
                column: "Sensor_ID",
                principalTable: "Sensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
