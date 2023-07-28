using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseCodeBackground : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackgroundCheck_CharacterReference_CharReferenceId",
                table: "BackgroundCheck");

            migrationBuilder.DropIndex(
                name: "IX_UserSchedule_ApplicationId",
                table: "UserSchedule");

            migrationBuilder.DropIndex(
                name: "IX_UserSchedule_UserId",
                table: "UserSchedule");

            migrationBuilder.DropIndex(
                name: "IX_BackgroundCheck_CharReferenceId",
                table: "BackgroundCheck");

            migrationBuilder.RenameColumn(
                name: "UserHRId",
                table: "BackgroundCheck",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CharReferenceId",
                table: "BackgroundCheck",
                newName: "CharacterReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSchedule_ApplicationId",
                table: "UserSchedule",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSchedule_UserId",
                table: "UserSchedule",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interview_ApplicationId",
                table: "Interview",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_UserId",
                table: "Interview",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Examination_UserId",
                table: "Examination",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheck_CharacterReferenceId",
                table: "BackgroundCheck",
                column: "CharacterReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheck_UserId",
                table: "BackgroundCheck",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackgroundCheck_CharacterReference_CharacterReferenceId",
                table: "BackgroundCheck",
                column: "CharacterReferenceId",
                principalTable: "CharacterReference",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BackgroundCheck_User_UserId",
                table: "BackgroundCheck",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Examination_User_UserId",
                table: "Examination",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interview_Application_ApplicationId",
                table: "Interview",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interview_User_UserId",
                table: "Interview",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackgroundCheck_CharacterReference_CharacterReferenceId",
                table: "BackgroundCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_BackgroundCheck_User_UserId",
                table: "BackgroundCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_Examination_User_UserId",
                table: "Examination");

            migrationBuilder.DropForeignKey(
                name: "FK_Interview_Application_ApplicationId",
                table: "Interview");

            migrationBuilder.DropForeignKey(
                name: "FK_Interview_User_UserId",
                table: "Interview");

            migrationBuilder.DropIndex(
                name: "IX_UserSchedule_ApplicationId",
                table: "UserSchedule");

            migrationBuilder.DropIndex(
                name: "IX_UserSchedule_UserId",
                table: "UserSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Interview_ApplicationId",
                table: "Interview");

            migrationBuilder.DropIndex(
                name: "IX_Interview_UserId",
                table: "Interview");

            migrationBuilder.DropIndex(
                name: "IX_Examination_UserId",
                table: "Examination");

            migrationBuilder.DropIndex(
                name: "IX_BackgroundCheck_CharacterReferenceId",
                table: "BackgroundCheck");

            migrationBuilder.DropIndex(
                name: "IX_BackgroundCheck_UserId",
                table: "BackgroundCheck");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BackgroundCheck",
                newName: "UserHRId");

            migrationBuilder.RenameColumn(
                name: "CharacterReferenceId",
                table: "BackgroundCheck",
                newName: "CharReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSchedule_ApplicationId",
                table: "UserSchedule",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSchedule_UserId",
                table: "UserSchedule",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundCheck_CharReferenceId",
                table: "BackgroundCheck",
                column: "CharReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackgroundCheck_CharacterReference_CharReferenceId",
                table: "BackgroundCheck",
                column: "CharReferenceId",
                principalTable: "CharacterReference",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
