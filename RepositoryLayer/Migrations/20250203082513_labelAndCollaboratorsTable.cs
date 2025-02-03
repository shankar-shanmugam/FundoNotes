using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class labelAndCollaboratorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collaboratortable",
                columns: table => new
                {
                    CollaboratorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    NotesId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaboratortable", x => x.CollaboratorId);
                    table.ForeignKey(
                        name: "FK_Collaboratortable_Notestable_NotesId",
                        column: x => x.NotesId,
                        principalTable: "Notestable",
                        principalColumn: "NotesId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Collaboratortable_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Labeltable",
                columns: table => new
                {
                    LabelId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabelName = table.Column<string>(nullable: true),
                    NoteId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labeltable", x => x.LabelId);
                    table.ForeignKey(
                        name: "FK_Labeltable_Notestable_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notestable",
                        principalColumn: "NotesId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Labeltable_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collaboratortable_NotesId",
                table: "Collaboratortable",
                column: "NotesId");

            migrationBuilder.CreateIndex(
                name: "IX_Collaboratortable_UserId",
                table: "Collaboratortable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Labeltable_NoteId",
                table: "Labeltable",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Labeltable_UserId",
                table: "Labeltable",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Collaboratortable");

            migrationBuilder.DropTable(
                name: "Labeltable");
        }
    }
}
