using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class changedtableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_User_Id",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "Notestable");

            migrationBuilder.RenameIndex(
                name: "IX_Notes_Id",
                table: "Notestable",
                newName: "IX_Notestable_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notestable",
                table: "Notestable",
                column: "NotesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notestable_User_Id",
                table: "Notestable",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notestable_User_Id",
                table: "Notestable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notestable",
                table: "Notestable");

            migrationBuilder.RenameTable(
                name: "Notestable",
                newName: "Notes");

            migrationBuilder.RenameIndex(
                name: "IX_Notestable_Id",
                table: "Notes",
                newName: "IX_Notes_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "NotesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_User_Id",
                table: "Notes",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
