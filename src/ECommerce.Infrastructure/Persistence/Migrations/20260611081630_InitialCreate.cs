using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PARENT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SLUG = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SORT_ORDER = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CATEGORIES_CATEGORIES_PARENT_ID",
                        column: x => x.PARENT_ID,
                        principalTable: "CATEGORIES",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "COUPONS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TYPE = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    VALUE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MIN_ORDER_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    MAX_DISCOUNT_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MAX_USAGE = table.Column<int>(type: "int", nullable: true),
                    MAX_USAGE_PER_USER = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    USED_COUNT = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    START_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    END_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUPONS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EMAIL = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FULL_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ROLE = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Customer"),
                    IS_EMAIL_VERIFIED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EMAIL_VERIFY_TOKEN = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EMAIL_VERIFY_EXPIRY = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LOYALTY_POINT = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LAST_LOGIN_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    SLUG = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CATEGORY_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BASE_PRICE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IS_DELETED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SEO_TITLE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SEO_DESCRIPTION = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PRODUCTS_CATEGORIES_CATEGORY_ID",
                        column: x => x.CATEGORY_ID,
                        principalTable: "CATEGORIES",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ADDRESSES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RECIPIENT_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PROVINCE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DISTRICT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WARD = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    STREET = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IS_DEFAULT = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADDRESSES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ADDRESSES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CARTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SESSION_ID = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    COUPON_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EXPIRES_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CARTS_COUPONS_COUPON_ID",
                        column: x => x.COUPON_ID,
                        principalTable: "COUPONS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CARTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_NUMBER = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    SUBTOTAL = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SHIPPING_FEE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    DISCOUNT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    TOTAL = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    COUPON_CODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SHIPPING_ADDRESS_JSON = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NOTE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PAYMENT_METHOD = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CONFIRMED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SHIPPED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DELIVERED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    COMPLETED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CANCELLED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CANCEL_REASON = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ROW_VERSION = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ORDERS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_VARIANTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PRODUCT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ATTRIBUTES_JSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRICE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    COMPARE_PRICE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IMAGE_URL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WEIGHT = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_VARIANTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PRODUCT_VARIANTS_PRODUCTS_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_AUDIT_LOGS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FROM_STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    TO_STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CHANGED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    REASON = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CHANGED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_AUDIT_LOGS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ORDER_AUDIT_LOGS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_AUDIT_LOGS_USERS_CHANGED_BY",
                        column: x => x.CHANGED_BY,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PAYMENTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    METHOD = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    AMOUNT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    TXN_REF = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GATEWAY_RESPONSE_JSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAID_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PAYMENTS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SHIPMENTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROVIDER = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    TRACKING_NUMBER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    SHIPPED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DELIVERED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    COST_FEE = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHIPMENTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SHIPMENTS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CART_ITEMS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CART_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VARIANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CART_ITEMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CART_ITEMS_CARTS_CART_ID",
                        column: x => x.CART_ID,
                        principalTable: "CARTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CART_ITEMS_PRODUCT_VARIANTS_VARIANT_ID",
                        column: x => x.VARIANT_ID,
                        principalTable: "PRODUCT_VARIANTS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "INVENTORIES",
                columns: table => new
                {
                    VARIANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ON_HAND = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RESERVED = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AVAILABLE = table.Column<int>(type: "int", nullable: false, computedColumnSql: "[ON_HAND] - [RESERVED]", stored: true),
                    ROW_VERSION = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVENTORIES", x => x.VARIANT_ID);
                    table.ForeignKey(
                        name: "FK_INVENTORIES_PRODUCT_VARIANTS_VARIANT_ID",
                        column: x => x.VARIANT_ID,
                        principalTable: "PRODUCT_VARIANTS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_ITEMS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ORDER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VARIANT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PRODUCT_NAME_SNAPSHOT = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    VARIANT_SKU_SNAPSHOT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VARIANT_ATTRIBUTES_SNAPSHOT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UNIT_PRICE_SNAPSHOT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    LINE_TOTAL = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_ITEMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ORDER_ITEMS_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_ITEMS_PRODUCT_VARIANTS_VARIANT_ID",
                        column: x => x.VARIANT_ID,
                        principalTable: "PRODUCT_VARIANTS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "REVIEWS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    PRODUCT_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ORDER_ITEM_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RATING = table.Column<byte>(type: "tinyint", nullable: false),
                    COMMENT = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IS_APPROVED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CREATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UPDATED_BY = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REVIEWS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_REVIEWS_ORDER_ITEMS_ORDER_ITEM_ID",
                        column: x => x.ORDER_ITEM_ID,
                        principalTable: "ORDER_ITEMS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_REVIEWS_PRODUCTS_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCTS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_REVIEWS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ADDRESSES_USER",
                table: "ADDRESSES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEMS_CART",
                table: "CART_ITEMS",
                column: "CART_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEMS_VARIANT_ID",
                table: "CART_ITEMS",
                column: "VARIANT_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_CART_ITEMS_CART_VARIANT",
                table: "CART_ITEMS",
                columns: new[] { "CART_ID", "VARIANT_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CARTS_COUPON_ID",
                table: "CARTS",
                column: "COUPON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CARTS_SESSION",
                table: "CARTS",
                column: "SESSION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CARTS_USER",
                table: "CARTS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORIES_PARENT_ID",
                table: "CATEGORIES",
                column: "PARENT_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_CATEGORIES_SLUG",
                table: "CATEGORIES",
                column: "SLUG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_COUPONS_CODE",
                table: "COUPONS",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_AUDIT_LOGS_CHANGED_BY",
                table: "ORDER_AUDIT_LOGS",
                column: "CHANGED_BY");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_AUDIT_LOGS_ORDER",
                table: "ORDER_AUDIT_LOGS",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_ITEMS_ORDER",
                table: "ORDER_ITEMS",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_ITEMS_VARIANT_ID",
                table: "ORDER_ITEMS",
                column: "VARIANT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_STATUS",
                table: "ORDERS",
                columns: new[] { "STATUS", "CREATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_USER",
                table: "ORDERS",
                columns: new[] { "USER_ID", "CREATED_AT" });

            migrationBuilder.CreateIndex(
                name: "UQ_ORDERS_ORDER_NUMBER",
                table: "ORDERS",
                column: "ORDER_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENTS_ORDER",
                table: "PAYMENTS",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "UX_PAYMENTS_TXN_REF",
                table: "PAYMENTS",
                column: "TXN_REF",
                unique: true,
                filter: "[TXN_REF] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_VARIANTS_PRODUCT",
                table: "PRODUCT_VARIANTS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_PRODUCT_VARIANTS_SKU",
                table: "PRODUCT_VARIANTS",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_CATEGORY",
                table: "PRODUCTS",
                columns: new[] { "CATEGORY_ID", "IS_ACTIVE", "IS_DELETED" });

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_NAME",
                table: "PRODUCTS",
                column: "NAME");

            migrationBuilder.CreateIndex(
                name: "UQ_PRODUCTS_SLUG",
                table: "PRODUCTS",
                column: "SLUG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_REVIEWS_PRODUCT",
                table: "REVIEWS",
                columns: new[] { "PRODUCT_ID", "IS_APPROVED" });

            migrationBuilder.CreateIndex(
                name: "IX_REVIEWS_USER_ID",
                table: "REVIEWS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_REVIEWS_ORDER_ITEM",
                table: "REVIEWS",
                column: "ORDER_ITEM_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SHIPMENTS_ORDER",
                table: "SHIPMENTS",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADDRESSES");

            migrationBuilder.DropTable(
                name: "CART_ITEMS");

            migrationBuilder.DropTable(
                name: "INVENTORIES");

            migrationBuilder.DropTable(
                name: "ORDER_AUDIT_LOGS");

            migrationBuilder.DropTable(
                name: "PAYMENTS");

            migrationBuilder.DropTable(
                name: "REVIEWS");

            migrationBuilder.DropTable(
                name: "SHIPMENTS");

            migrationBuilder.DropTable(
                name: "CARTS");

            migrationBuilder.DropTable(
                name: "ORDER_ITEMS");

            migrationBuilder.DropTable(
                name: "COUPONS");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "PRODUCT_VARIANTS");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "PRODUCTS");

            migrationBuilder.DropTable(
                name: "CATEGORIES");
        }
    }
}
