using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaBlog.Data.Migrations
{
    public partial class _002_ModifyBlogPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogPostStatus",
                table: "BlogPosts");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "BlogPosts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "BlogPosts");

            migrationBuilder.AddColumn<int>(
                name: "BlogPostStatus",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
