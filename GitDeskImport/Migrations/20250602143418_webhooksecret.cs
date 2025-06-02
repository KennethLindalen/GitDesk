using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitDeskImport.Migrations
{
    /// <inheritdoc />
    public partial class webhooksecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ZendeskTicketId",
                table: "SyncMappings",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "WebhookSecret",
                table: "Businesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebhookSecret",
                table: "Businesses");

            migrationBuilder.AlterColumn<string>(
                name: "ZendeskTicketId",
                table: "SyncMappings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
