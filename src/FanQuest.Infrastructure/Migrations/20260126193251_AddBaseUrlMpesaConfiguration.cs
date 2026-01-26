using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseUrlMpesaConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "B2CQueueTimeoutUrl",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "B2CResultUrl",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BaseUrl",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CallbackTimeoutMinutes",
                table: "MpesaConfigurations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InitiatorName",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecurityCredential",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StkCallbackUrl",
                table: "MpesaConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "B2CQueueTimeoutUrl",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "B2CResultUrl",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "BaseUrl",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "CallbackTimeoutMinutes",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "InitiatorName",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "SecurityCredential",
                table: "MpesaConfigurations");

            migrationBuilder.DropColumn(
                name: "StkCallbackUrl",
                table: "MpesaConfigurations");
        }
    }
}
