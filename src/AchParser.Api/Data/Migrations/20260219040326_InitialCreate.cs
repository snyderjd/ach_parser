using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchParser.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ach_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    filename = table.Column<string>(type: "text", nullable: false),
                    hash = table.Column<string>(type: "text", nullable: false),
                    unparsed_file = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ach_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batch_headers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_class_code = table.Column<string>(type: "char(3)", nullable: false),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    company_identification = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_headers", x => x.id);
                    table.ForeignKey(
                        name: "fk_batch_headers_ach_files_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_controls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_count = table.Column<int>(type: "integer", nullable: false),
                    block_count = table.Column<int>(type: "integer", nullable: false),
                    entry_addenda_count = table.Column<int>(type: "integer", nullable: false),
                    total_debit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_controls", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_controls_ach_files_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_headers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    immediate_destination = table.Column<string>(type: "text", nullable: false),
                    immediate_origin = table.Column<string>(type: "text", nullable: false),
                    file_creation_date = table.Column<DateTime>(type: "date", nullable: false),
                    file_creation_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    immediate_destination_name = table.Column<string>(type: "text", nullable: false),
                    immediate_origin_name = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_headers", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_headers_ach_files_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batch_controls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_header_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_addenda_count = table.Column<int>(type: "integer", nullable: false),
                    total_debit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_controls", x => x.id);
                    table.ForeignKey(
                        name: "fk_batch_controls_batch_headers_batch_header_id",
                        column: x => x.batch_header_id,
                        principalTable: "batch_headers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entry_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_header_id = table.Column<Guid>(type: "uuid", nullable: false),
                    routing_number = table.Column<string>(type: "char(9)", nullable: false),
                    account_number = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    individual_name = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entry_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_entry_details_batch_headers_batch_header_id",
                        column: x => x.batch_header_id,
                        principalTable: "batch_headers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addendas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_detail_id = table.Column<Guid>(type: "uuid", nullable: false),
                    information = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addendas", x => x.id);
                    table.ForeignKey(
                        name: "fk_addendas_entry_details_entry_detail_id",
                        column: x => x.entry_detail_id,
                        principalTable: "entry_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ach_files_hash",
                table: "ach_files",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_addendas_entry_detail_id",
                table: "addendas",
                column: "entry_detail_id");

            migrationBuilder.CreateIndex(
                name: "ix_batch_controls_batch_header_id",
                table: "batch_controls",
                column: "batch_header_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_batch_headers_ach_file_id",
                table: "batch_headers",
                column: "ach_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_entry_details_batch_header_id",
                table: "entry_details",
                column: "batch_header_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_controls_ach_file_id",
                table: "file_controls",
                column: "ach_file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_headers_ach_file_id",
                table: "file_headers",
                column: "ach_file_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addendas");

            migrationBuilder.DropTable(
                name: "batch_controls");

            migrationBuilder.DropTable(
                name: "file_controls");

            migrationBuilder.DropTable(
                name: "file_headers");

            migrationBuilder.DropTable(
                name: "entry_details");

            migrationBuilder.DropTable(
                name: "batch_headers");

            migrationBuilder.DropTable(
                name: "ach_files");
        }
    }
}
