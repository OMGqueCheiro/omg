using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMG.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTrackingToEventChangeStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioEmail",
                table: "EventChangeStatus",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNome",
                table: "EventChangeStatus",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioEmail",
                table: "EventChangeStatus");

            migrationBuilder.DropColumn(
                name: "UsuarioNome",
                table: "EventChangeStatus");
        }
    }
}
