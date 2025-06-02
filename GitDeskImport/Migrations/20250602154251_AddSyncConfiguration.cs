using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitDeskImport.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Repository = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZendeskDomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SyncFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncConfigurations_Businesses_BusinessModelId",
                        column: x => x.BusinessModelId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncConfigurations_BusinessModelId",
                table: "SyncConfigurations",
                column: "BusinessModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncConfigurations");
        }
    }
}
