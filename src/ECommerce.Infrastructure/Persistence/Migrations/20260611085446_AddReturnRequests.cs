using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RETURN_REQUESTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    REASON = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    STAFF_NOTE = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    REFUND_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RETURN_REQUESTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RETURN_REQUESTS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "RETURN_ITEMS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    RETURN_REQUEST_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ORDER_ITEM_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RETURN_ITEMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RETURN_ITEMS_ORDER_ITEMS_ORDER_ITEM_ID",
                        column: x => x.ORDER_ITEM_ID,
                        principalTable: "ORDER_ITEMS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RETURN_ITEMS_RETURN_REQUESTS_RETURN_REQUEST_ID",
                        column: x => x.RETURN_REQUEST_ID,
                        principalTable: "RETURN_REQUESTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RETURN_ITEMS_ORDER_ITEM_ID",
                table: "RETURN_ITEMS",
                column: "ORDER_ITEM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RETURN_ITEMS_REQUEST",
                table: "RETURN_ITEMS",
                column: "RETURN_REQUEST_ID");

            migrationBuilder.CreateIndex(
                name: "IX_RETURN_REQUESTS_ORDER",
                table: "RETURN_REQUESTS",
                column: "ORDER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RETURN_ITEMS");

            migrationBuilder.DropTable(
                name: "RETURN_REQUESTS");
        }
    }
}
