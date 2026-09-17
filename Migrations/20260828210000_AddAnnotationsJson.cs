using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testWPF.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnotationsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnnotationsJson",
                table: "Cases",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnotationsJson",
                table: "Cases");
        }
    }
}
