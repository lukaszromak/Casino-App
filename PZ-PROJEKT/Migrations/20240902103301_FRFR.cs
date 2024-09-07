using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PZ_PROJEKT.Migrations
{
    /// <inheritdoc />
    public partial class FRFR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_UserId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Item");

            migrationBuilder.CreateTable(
                name: "ItemUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserItemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUser", x => new { x.UserId, x.UserItemsId });
                    table.ForeignKey(
                        name: "FK_ItemUser_Item_UserItemsId",
                        column: x => x.UserItemsId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemUser_UserItemsId",
                table: "ItemUser",
                column: "UserItemsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemUser");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_UserId",
                table: "Item",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_User_UserId",
                table: "Item",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
