using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTwoUsersPerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 7, "mtorres@unaj.com", "Mariana Torres", "$2a$11$UkavkV5pLlUiGv3G0/BG8.SFCu2MDF69WeefFePdUe3q258G.P/aq", 1 },
                    { 8, "cmendoza@unaj.com", "Carlos Mendoza", "$2a$11$UkavkV5pLlUiGv3G0/BG8.SFCu2MDF69WeefFePdUe3q258G.P/aq", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
