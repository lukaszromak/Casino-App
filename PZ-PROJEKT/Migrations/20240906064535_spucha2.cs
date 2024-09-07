using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PZ_PROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class spucha2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemTransactionId",
                table: "Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemTransactions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemTransactionId",
                table: "Item",
                column: "ItemTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTransactions_UserId",
                table: "ItemTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemTransactions_ItemTransactionId",
                table: "Item",
                column: "ItemTransactionId",
                principalTable: "ItemTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemTransactions_ItemTransactionId",
                table: "Item");

            migrationBuilder.DropTable(
                name: "ItemTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Item_ItemTransactionId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "ItemTransactionId",
                table: "Item");
        }
    }
}
