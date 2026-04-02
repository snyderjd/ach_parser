using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchParser.Api.Migrations
{
    /// <inheritdoc />
    public partial class PluralizeTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_addenda_entry_detail_entry_detail_id",
                table: "addenda");

            migrationBuilder.DropForeignKey(
                name: "fk_batch_control_batch_header_batch_header_id",
                table: "batch_control");

            migrationBuilder.DropForeignKey(
                name: "fk_batch_header_ach_file_ach_file_id",
                table: "batch_header");

            migrationBuilder.DropForeignKey(
                name: "fk_entry_detail_batch_header_batch_header_id",
                table: "entry_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_file_control_ach_file_ach_file_id",
                table: "file_control");

            migrationBuilder.DropForeignKey(
                name: "fk_file_header_ach_file_ach_file_id",
                table: "file_header");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_header",
                table: "file_header");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_control",
                table: "file_control");

            migrationBuilder.DropPrimaryKey(
                name: "pk_entry_detail",
                table: "entry_detail");

            migrationBuilder.DropPrimaryKey(
                name: "pk_batch_header",
                table: "batch_header");

            migrationBuilder.DropPrimaryKey(
                name: "pk_batch_control",
                table: "batch_control");

            migrationBuilder.DropPrimaryKey(
                name: "pk_addenda",
                table: "addenda");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ach_file",
                table: "ach_file");

            migrationBuilder.RenameTable(
                name: "file_header",
                newName: "file_headers");

            migrationBuilder.RenameTable(
                name: "file_control",
                newName: "file_controls");

            migrationBuilder.RenameTable(
                name: "entry_detail",
                newName: "entry_details");

            migrationBuilder.RenameTable(
                name: "batch_header",
                newName: "batch_headers");

            migrationBuilder.RenameTable(
                name: "batch_control",
                newName: "batch_controls");

            migrationBuilder.RenameTable(
                name: "addenda",
                newName: "addendas");

            migrationBuilder.RenameTable(
                name: "ach_file",
                newName: "ach_files");

            migrationBuilder.RenameIndex(
                name: "ix_file_header_ach_file_id",
                table: "file_headers",
                newName: "ix_file_headers_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_file_control_ach_file_id",
                table: "file_controls",
                newName: "ix_file_controls_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_entry_detail_batch_header_id",
                table: "entry_details",
                newName: "ix_entry_details_batch_header_id");

            migrationBuilder.RenameIndex(
                name: "ix_batch_header_ach_file_id",
                table: "batch_headers",
                newName: "ix_batch_headers_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_batch_control_batch_header_id",
                table: "batch_controls",
                newName: "ix_batch_controls_batch_header_id");

            migrationBuilder.RenameIndex(
                name: "ix_addenda_entry_detail_id",
                table: "addendas",
                newName: "ix_addendas_entry_detail_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_headers",
                table: "file_headers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_controls",
                table: "file_controls",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_entry_details",
                table: "entry_details",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_batch_headers",
                table: "batch_headers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_batch_controls",
                table: "batch_controls",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_addendas",
                table: "addendas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ach_files",
                table: "ach_files",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_addendas_entry_details_entry_detail_id",
                table: "addendas",
                column: "entry_detail_id",
                principalTable: "entry_details",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_batch_controls_batch_headers_batch_header_id",
                table: "batch_controls",
                column: "batch_header_id",
                principalTable: "batch_headers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_batch_headers_ach_files_ach_file_id",
                table: "batch_headers",
                column: "ach_file_id",
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_entry_details_batch_headers_batch_header_id",
                table: "entry_details",
                column: "batch_header_id",
                principalTable: "batch_headers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_file_controls_ach_files_ach_file_id",
                table: "file_controls",
                column: "ach_file_id",
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_file_headers_ach_files_ach_file_id",
                table: "file_headers",
                column: "ach_file_id",
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_addendas_entry_details_entry_detail_id",
                table: "addendas");

            migrationBuilder.DropForeignKey(
                name: "fk_batch_controls_batch_headers_batch_header_id",
                table: "batch_controls");

            migrationBuilder.DropForeignKey(
                name: "fk_batch_headers_ach_files_ach_file_id",
                table: "batch_headers");

            migrationBuilder.DropForeignKey(
                name: "fk_entry_details_batch_headers_batch_header_id",
                table: "entry_details");

            migrationBuilder.DropForeignKey(
                name: "fk_file_controls_ach_files_ach_file_id",
                table: "file_controls");

            migrationBuilder.DropForeignKey(
                name: "fk_file_headers_ach_files_ach_file_id",
                table: "file_headers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_headers",
                table: "file_headers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_controls",
                table: "file_controls");

            migrationBuilder.DropPrimaryKey(
                name: "pk_entry_details",
                table: "entry_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_batch_headers",
                table: "batch_headers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_batch_controls",
                table: "batch_controls");

            migrationBuilder.DropPrimaryKey(
                name: "pk_addendas",
                table: "addendas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ach_files",
                table: "ach_files");

            migrationBuilder.RenameTable(
                name: "file_headers",
                newName: "file_header");

            migrationBuilder.RenameTable(
                name: "file_controls",
                newName: "file_control");

            migrationBuilder.RenameTable(
                name: "entry_details",
                newName: "entry_detail");

            migrationBuilder.RenameTable(
                name: "batch_headers",
                newName: "batch_header");

            migrationBuilder.RenameTable(
                name: "batch_controls",
                newName: "batch_control");

            migrationBuilder.RenameTable(
                name: "addendas",
                newName: "addenda");

            migrationBuilder.RenameTable(
                name: "ach_files",
                newName: "ach_file");

            migrationBuilder.RenameIndex(
                name: "ix_file_headers_ach_file_id",
                table: "file_header",
                newName: "ix_file_header_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_file_controls_ach_file_id",
                table: "file_control",
                newName: "ix_file_control_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_entry_details_batch_header_id",
                table: "entry_detail",
                newName: "ix_entry_detail_batch_header_id");

            migrationBuilder.RenameIndex(
                name: "ix_batch_headers_ach_file_id",
                table: "batch_header",
                newName: "ix_batch_header_ach_file_id");

            migrationBuilder.RenameIndex(
                name: "ix_batch_controls_batch_header_id",
                table: "batch_control",
                newName: "ix_batch_control_batch_header_id");

            migrationBuilder.RenameIndex(
                name: "ix_addendas_entry_detail_id",
                table: "addenda",
                newName: "ix_addenda_entry_detail_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_header",
                table: "file_header",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_file_control",
                table: "file_control",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_entry_detail",
                table: "entry_detail",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_batch_header",
                table: "batch_header",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_batch_control",
                table: "batch_control",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_addenda",
                table: "addenda",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ach_file",
                table: "ach_file",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_addenda_entry_detail_entry_detail_id",
                table: "addenda",
                column: "entry_detail_id",
                principalTable: "entry_detail",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_batch_control_batch_header_batch_header_id",
                table: "batch_control",
                column: "batch_header_id",
                principalTable: "batch_header",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_batch_header_ach_file_ach_file_id",
                table: "batch_header",
                column: "ach_file_id",
                principalTable: "ach_file",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_entry_detail_batch_header_batch_header_id",
                table: "entry_detail",
                column: "batch_header_id",
                principalTable: "batch_header",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_file_control_ach_file_ach_file_id",
                table: "file_control",
                column: "ach_file_id",
                principalTable: "ach_file",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_file_header_ach_file_ach_file_id",
                table: "file_header",
                column: "ach_file_id",
                principalTable: "ach_file",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
