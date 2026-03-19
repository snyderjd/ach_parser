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
                name: "ach_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Filename = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    UnparsedFile = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ach_files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "batch_headers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AchFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceClassCode = table.Column<string>(type: "char(3)", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    CompanyIdentification = table.Column<string>(type: "text", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_batch_headers_ach_files_AchFileId",
                        column: x => x.AchFileId,
                        principalTable: "ach_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_controls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AchFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchCount = table.Column<int>(type: "integer", nullable: false),
                    BlockCount = table.Column<int>(type: "integer", nullable: false),
                    EntryAddendaCount = table.Column<int>(type: "integer", nullable: false),
                    TotalDebit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_controls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_controls_ach_files_AchFileId",
                        column: x => x.AchFileId,
                        principalTable: "ach_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_headers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AchFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImmediateDestination = table.Column<string>(type: "text", nullable: false),
                    ImmediateOrigin = table.Column<string>(type: "text", nullable: false),
                    FileCreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    FileCreationTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ImmediateDestinationName = table.Column<string>(type: "text", nullable: false),
                    ImmediateOriginName = table.Column<string>(type: "text", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_headers_ach_files_AchFileId",
                        column: x => x.AchFileId,
                        principalTable: "ach_files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batch_controls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchHeaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryAddendaCount = table.Column<int>(type: "integer", nullable: false),
                    TotalDebit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_controls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_batch_controls_batch_headers_BatchHeaderId",
                        column: x => x.BatchHeaderId,
                        principalTable: "batch_headers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entry_details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchHeaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoutingNumber = table.Column<string>(type: "char(9)", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    IndividualName = table.Column<string>(type: "text", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entry_details_batch_headers_BatchHeaderId",
                        column: x => x.BatchHeaderId,
                        principalTable: "batch_headers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "addendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntryDetailId = table.Column<Guid>(type: "uuid", nullable: false),
                    Information = table.Column<string>(type: "text", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    UnparsedRecord = table.Column<string>(type: "char(94)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_addendas_entry_details_EntryDetailId",
                        column: x => x.EntryDetailId,
                        principalTable: "entry_details",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ach_files_Hash",
                table: "ach_files",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_addendas_EntryDetailId",
                table: "addendas",
                column: "EntryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_batch_controls_BatchHeaderId",
                table: "batch_controls",
                column: "BatchHeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_batch_headers_AchFileId",
                table: "batch_headers",
                column: "AchFileId");

            migrationBuilder.CreateIndex(
                name: "IX_entry_details_BatchHeaderId",
                table: "entry_details",
                column: "BatchHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_file_controls_AchFileId",
                table: "file_controls",
                column: "AchFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_file_headers_AchFileId",
                table: "file_headers",
                column: "AchFileId",
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
