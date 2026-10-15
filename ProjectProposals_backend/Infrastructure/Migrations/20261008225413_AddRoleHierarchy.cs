using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HierarchyLevel",
                table: "ApproverRole",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecializedRole",
                table: "ApproverRole",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ApproverRole",
                keyColumn: "Id",
                keyValue: 1,
                column: "HierarchyLevel",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ApproverRole",
                keyColumn: "Id",
                keyValue: 2,
                column: "HierarchyLevel",
                value: 20);

            migrationBuilder.UpdateData(
                table: "ApproverRole",
                keyColumn: "Id",
                keyValue: 3,
                column: "HierarchyLevel",
                value: 30);

            migrationBuilder.UpdateData(
                table: "ApproverRole",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "HierarchyLevel", "IsSpecializedRole" },
                values: new object[] { 15, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HierarchyLevel",
                table: "ApproverRole");

            migrationBuilder.DropColumn(
                name: "IsSpecializedRole",
                table: "ApproverRole");
        }
    }
}
