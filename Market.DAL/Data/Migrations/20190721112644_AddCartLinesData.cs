using Microsoft.EntityFrameworkCore.Migrations;

namespace Market.DAL.Data.Migrations
{
    public partial class AddCartLinesData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartLines",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartLines", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CartLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartLines_ProductId",
                table: "CartLines",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartLines");
        }
    }
}
