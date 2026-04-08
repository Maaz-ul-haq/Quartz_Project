using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuartzData.Migrations
{
    /// <inheritdoc />
    public partial class FixApiHistoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ApiCallHistories",
                newName: "ExecutedAt");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "ApiCallHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExecutedAt",
                table: "ApiCallHistories",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "ApiCallHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
