using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceInfrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddDataBase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Img", "Name" },
                values: new object[,]
                {
                    { 1, "https://still.az/wp-content/uploads/2021/04/1500-300x300.jpg", "كنب" },
                    { 2, "https://st.hzcdn.com/fimgs/e4718fea03ad2931_6592-w186-h135-b1-p10--.jpg", "غرف نوم" },
                    { 3, "https://www.shutterstock.com/image-photo/wooden-table-chairs-on-white-600nw-2168176325.jpg", "سفرة" },
                    { 4, "https://st.hzcdn.com/fimgs/1cf15b22065759cb_3954-w186-h135-b1-p10--.jpg", "اثاث حدائق" },
                    { 5, "https://png.pngtree.com/background/20231117/original/pngtree-vibrant-wooden-educational-toys-on-white-background-perfect-for-kids-picture-image_6297092.jpg", "غرف اطفال" },
                    { 6, "https://st.hzcdn.com/fimgs/65a1489406b3edfc_8174-w186-h135-b1-p10--.jpg", "مزنون" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
