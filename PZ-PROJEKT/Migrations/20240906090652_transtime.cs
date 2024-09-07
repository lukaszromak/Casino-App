using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PZ_PROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class transtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionTime",
                table: "ItemTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionTime",
                table: "ItemTransactions");
        }
    }
}
