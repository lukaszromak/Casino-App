using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PZ_PROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class FF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemItemTransaction");

            migrationBuilder.CreateTable(
                name: "TransactionItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ItemTransactionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionItem_ItemTransactions_ItemTransactionId",
                        column: x => x.ItemTransactionId,
                        principalTable: "ItemTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionItem_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItem_ItemId",
                table: "TransactionItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItem_ItemTransactionId",
                table: "TransactionItem",
                column: "ItemTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionItem");

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
    }
}
