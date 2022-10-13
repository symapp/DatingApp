using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class MessageEntityEditedBecauseICanNotSpell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageSend",
                table: "Messages",
                newName: "MessageSent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MessageSent",
                table: "Messages",
                newName: "MessageSend");
        }
    }
}
