using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebdeskEA.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mg1_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");

        }
    }
}
