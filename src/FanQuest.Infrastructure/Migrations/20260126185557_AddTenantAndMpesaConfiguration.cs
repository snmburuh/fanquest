using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FanQuest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantAndMpesaConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Quests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MpesaReceipt",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CheckoutRequestId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConversationId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastStatusCheckAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchantRequestId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MpesaReceiptNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginatorConversationId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StatusCheckAttempts",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MpesaConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MpesaConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsumerSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessShortCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Passkey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Environment = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MpesaConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MpesaConfigurations_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MpesaConfigurations_TenantId",
                table: "MpesaConfigurations",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ApiKey",
                table: "Tenants",
                column: "ApiKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MpesaConfigurations");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "CheckoutRequestId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastStatusCheckAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MerchantRequestId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MpesaReceiptNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OriginatorConversationId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StatusCheckAttempts",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Payments");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "MpesaReceipt",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
