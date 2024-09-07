using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PZ_PROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class AFAF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemTransactions_ItemTransactionId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_ItemTransactionId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "ItemTransactionId",
                table: "Item");

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "ItemTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemItemTransaction",
                columns: table => new
                {
                    ItemTransactionId = table.Column<int>(type: "int", nullable: false),
                    ItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemItemTransaction", x => new { x.ItemTransactionId, x.ItemsId });
                    table.ForeignKey(
                        name: "FK_ItemItemTransaction_ItemTransactions_ItemTransactionId",
                        column: x => x.ItemTransactionId,
                        principalTable: "ItemTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemItemTransaction_Item_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemItemTransaction_ItemsId",
                table: "ItemItemTransaction",
                column: "ItemsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemItemTransaction");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "ItemTransactions");

            migrationBuilder.AddColumn<int>(
                name: "ItemTransactionId",
                table: "Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemTransactionId",
                table: "Item",
                column: "ItemTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemTransactions_ItemTransactionId",
                table: "Item",
                column: "ItemTransactionId",
                principalTable: "ItemTransactions",
                principalColumn: "Id");
        }
    }
}
