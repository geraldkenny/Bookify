using Microsoft.EntityFrameworkCore.Migrations;

namespace Entity.Migrations
{
    public partial class UpdatedUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "BookifyUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BookifyUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "BookifyUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "BookifyUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BookifyUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "BookifyUsers");
        }
    }
}
