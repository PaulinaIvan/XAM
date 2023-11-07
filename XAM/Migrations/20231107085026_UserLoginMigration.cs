﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XAM.Migrations
{
    /// <inheritdoc />
    public partial class UserLoginMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerUsername",
                table: "DataHoldersTable",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerUsername",
                table: "DataHoldersTable");
        }
    }
}
