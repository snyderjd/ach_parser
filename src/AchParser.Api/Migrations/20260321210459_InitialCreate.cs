using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchParser.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ach_file",
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
                    table.PrimaryKey("pk_ach_file", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batch_header",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_class_code = table.Column<string>(type: "text", nullable: false),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    company_identification = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_header", x => x.id);
                    table.ForeignKey(
                        name: "fk_batch_header_ach_file_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_control",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_count = table.Column<int>(type: "integer", nullable: false),
                    block_count = table.Column<int>(type: "integer", nullable: false),
                    entry_addenda_count = table.Column<int>(type: "integer", nullable: false),
                    total_debit = table.Column<decimal>(type: "numeric", nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_control", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_control_ach_file_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_header",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ach_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    immediate_destination = table.Column<string>(type: "text", nullable: false),
                    immediate_origin = table.Column<string>(type: "text", nullable: false),
                    file_creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    file_creation_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    immediate_destination_name = table.Column<string>(type: "text", nullable: false),
                    immediate_origin_name = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_header", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_header_ach_file_ach_file_id",
                        column: x => x.ach_file_id,
                        principalTable: "ach_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batch_control",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_header_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_addenda_count = table.Column<int>(type: "integer", nullable: false),
                    total_debit = table.Column<decimal>(type: "numeric", nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batch_control", x => x.id);
                    table.ForeignKey(
                        name: "fk_batch_control_batch_header_batch_header_id",
                        column: x => x.batch_header_id,
                        principalTable: "batch_header",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entry_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_header_id = table.Column<Guid>(type: "uuid", nullable: true),
                    routing_number = table.Column<string>(type: "text", nullable: false),
                    account_number = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    individual_name = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entry_detail", x => x.id);
                    table.ForeignKey(
                        name: "fk_entry_detail_batch_header_batch_header_id",
                        column: x => x.batch_header_id,
                        principalTable: "batch_header",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "addenda",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_detail_id = table.Column<Guid>(type: "uuid", nullable: true),
                    information = table.Column<string>(type: "text", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    unparsed_record = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addenda", x => x.id);
                    table.ForeignKey(
                        name: "fk_addenda_entry_detail_entry_detail_id",
                        column: x => x.entry_detail_id,
                        principalTable: "entry_detail",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_addenda_entry_detail_id",
                table: "addenda",
                column: "entry_detail_id");

            migrationBuilder.CreateIndex(
                name: "ix_batch_control_batch_header_id",
                table: "batch_control",
                column: "batch_header_id");

            migrationBuilder.CreateIndex(
                name: "ix_batch_header_ach_file_id",
                table: "batch_header",
                column: "ach_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_entry_detail_batch_header_id",
                table: "entry_detail",
                column: "batch_header_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_control_ach_file_id",
                table: "file_control",
                column: "ach_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_file_header_ach_file_id",
                table: "file_header",
                column: "ach_file_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addenda");

            migrationBuilder.DropTable(
                name: "batch_control");

            migrationBuilder.DropTable(
                name: "file_control");

            migrationBuilder.DropTable(
                name: "file_header");

            migrationBuilder.DropTable(
                name: "entry_detail");

            migrationBuilder.DropTable(
                name: "batch_header");

            migrationBuilder.DropTable(
                name: "ach_file");
        }
    }
}
