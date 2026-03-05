using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchParser.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class MinimalAchFileOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "ix_ach_files_hash",
                table: "ach_files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_headers",
                table: "file_headers");

            migrationBuilder.DropIndex(
                name: "ix_file_headers_ach_file_id",
                table: "file_headers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_controls",
                table: "file_controls");

            migrationBuilder.DropIndex(
                name: "ix_file_controls_ach_file_id",
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

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "file_header",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "file_creation_time",
                table: "file_header",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "file_creation_date",
                table: "file_header",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "file_control",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit",
                table: "file_control",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit",
                table: "file_control",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "entry_detail",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

            migrationBuilder.AlterColumn<string>(
                name: "routing_number",
                table: "entry_detail",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "entry_detail",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "batch_header",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

            migrationBuilder.AlterColumn<string>(
                name: "service_class_code",
                table: "batch_header",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "batch_control",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit",
                table: "batch_control",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit",
                table: "batch_control",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "addenda",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(94)");

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

            migrationBuilder.CreateIndex(
                name: "ix_file_header_ach_file_id",
                table: "file_header",
                column: "ach_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_control_ach_file_id",
                table: "file_control",
                column: "ach_file_id");

            migrationBuilder.AddForeignKey(
                name: "fk_addenda_entry_detail_entry_detail_id",
                table: "addenda",
                column: "entry_detail_id",
                principalTable: "entry_detail",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_entry_detail_batch_header_batch_header_id",
                table: "entry_detail",
                column: "batch_header_id",
                principalTable: "batch_header",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_file_control_ach_file_ach_file_id",
                table: "file_control",
                column: "ach_file_id",
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_file_header_ach_file_ach_file_id",
                table: "file_header",
                column: "ach_file_id",
                principalTable: "ach_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "ix_file_header_ach_file_id",
                table: "file_header");

            migrationBuilder.DropPrimaryKey(
                name: "pk_file_control",
                table: "file_control");

            migrationBuilder.DropIndex(
                name: "ix_file_control_ach_file_id",
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

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "file_headers",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "file_creation_time",
                table: "file_headers",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<DateTime>(
                name: "file_creation_date",
                table: "file_headers",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "file_controls",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit",
                table: "file_controls",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit",
                table: "file_controls",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "entry_details",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "routing_number",
                table: "entry_details",
                type: "char(9)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "entry_details",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "batch_headers",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "service_class_code",
                table: "batch_headers",
                type: "char(3)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "batch_controls",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_debit",
                table: "batch_controls",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_credit",
                table: "batch_controls",
                type: "numeric(12,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "unparsed_record",
                table: "addendas",
                type: "char(94)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.CreateIndex(
                name: "ix_ach_files_hash",
                table: "ach_files",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_headers_ach_file_id",
                table: "file_headers",
                column: "ach_file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_controls_ach_file_id",
                table: "file_controls",
                column: "ach_file_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_addendas_entry_details_entry_detail_id",
                table: "addendas",
                column: "entry_detail_id",
                principalTable: "entry_details",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
