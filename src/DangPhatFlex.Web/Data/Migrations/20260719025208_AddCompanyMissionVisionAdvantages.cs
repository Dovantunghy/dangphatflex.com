using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DangPhatFlex.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyMissionVisionAdvantages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Advantages",
                table: "CompanyInfos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mission",
                table: "CompanyInfos",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vision",
                table: "CompanyInfos",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Advantages",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "Mission",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "Vision",
                table: "CompanyInfos");
        }
    }
}
