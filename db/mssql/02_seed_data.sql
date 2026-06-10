/* =====================================================================
   ECommerce - Specialty Coffee Shop
   Seed data (demo)  |  Microsoft SQL Server (T-SQL)
   Run AFTER 01_create_core_tables.sql.

   Contents: 3 demo accounts (admin/staff/customer) + 2 extra customers,
   4 categories, 20 coffee products (x2 variants = 40 variants) + inventory,
   3 coupons, 3 addresses, 6 orders across statuses, payments, shipments,
   1 review, order audit logs.

   IMPORTANT - PASSWORD_HASH below are PLACEHOLDERS (not real BCrypt).
   For working login, seed users via the app (BCrypt.Net) or replace with a
   real hash, e.g. in C#:  BCrypt.Net.BCrypt.HashPassword("Password123!");
   Re-runnable: aborts if USERS already has rows.
   ===================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF EXISTS (SELECT 1 FROM dbo.USERS)
BEGIN
    PRINT 'Seed skipped: data already present.';
    RETURN;
END

BEGIN TRY
BEGIN TRAN;

/* ---------- Fixed IDs for cross-references ---------- */
DECLARE @ADMIN     UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @STAFF     UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @CUSTOMER1 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @CUSTOMER2 UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @CUSTOMER3 UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';

DECLARE @PWD NVARCHAR(500) = N'$2a$11$PLACEHOLDER.PLACEHOLDER.PLACEHOLDER.PLACEHOLDER.PLACEH';
DECLARE @NOW DATETIME2 = SYSUTCDATETIME();

/* ---------- 1. USERS ---------- */
INSERT INTO dbo.USERS (ID, EMAIL, PASSWORD_HASH, FULL_NAME, PHONE, ROLE, IS_EMAIL_VERIFIED, CREATED_BY) VALUES
 (@ADMIN,     N'admin@coffee.local',    @PWD, N'Quản trị viên',   N'0900000001', 'Admin',    1, NULL),
 (@STAFF,     N'staff@coffee.local',    @PWD, N'Nhân viên kho',    N'0900000002', 'Staff',    1, @ADMIN),
 (@CUSTOMER1, N'an.nguyen@example.com', @PWD, N'Nguyễn Văn An',    N'0901234567', 'Customer', 1, NULL),
 (@CUSTOMER2, N'binh.tran@example.com', @PWD, N'Trần Thị Bình',    N'0907654321', 'Customer', 1, NULL),
 (@CUSTOMER3, N'cuong.le@example.com',  @PWD, N'Lê Văn Cường',     N'0912345678', 'Customer', 1, NULL);

/* ---------- 2. CATEGORIES ---------- */
INSERT INTO dbo.CATEGORIES (NAME, SLUG, SORT_ORDER, CREATED_BY) VALUES
 (N'Cà phê Arabica', 'arabica', 1, @ADMIN),
 (N'Cà phê Robusta', 'robusta', 2, @ADMIN),
 (N'Cà phê Blend',   'blend',   3, @ADMIN),
 (N'Cà phê Decaf',   'decaf',   4, @ADMIN);

/* ---------- 3. PRODUCTS (20) ---------- */
INSERT INTO dbo.PRODUCTS (SLUG, NAME, DESCRIPTION, CATEGORY_ID, BASE_PRICE, CREATED_BY)
SELECT v.SLUG, v.NAME, v.DESCR, c.ID, v.BASE_PRICE, @ADMIN
FROM (VALUES
 ('ca-phe-arabica-cau-dat',        N'Cà phê Arabica Cầu Đất',        N'Hạt rang vừa, hương sô-cô-la và hạnh nhân.',      'arabica', CAST(185000 AS DECIMAL(18,2))),
 ('ca-phe-arabica-lam-dong',       N'Cà phê Arabica Lâm Đồng',       N'Vị chua thanh, hậu vị ngọt dịu.',                 'arabica', 175000),
 ('ca-phe-arabica-son-la',         N'Cà phê Arabica Sơn La',         N'Hương hoa nhẹ, body cân bằng.',                   'arabica', 169000),
 ('ca-phe-arabica-typica',         N'Cà phê Arabica Typica',         N'Giống cổ điển, hương trái cây tinh tế.',          'arabica', 220000),
 ('ca-phe-arabica-bourbon',        N'Cà phê Arabica Bourbon',        N'Ngọt caramel, độ chua sáng.',                     'arabica', 235000),
 ('ca-phe-arabica-honey',          N'Cà phê Arabica Honey Process',  N'Sơ chế honey, ngọt mật và mọng nước.',            'arabica', 245000),
 ('ca-phe-robusta-buon-ma-thuot',  N'Cà phê Robusta Buôn Ma Thuột',  N'Đậm đắng truyền thống, nhiều crema.',             'robusta', 120000),
 ('ca-phe-robusta-gia-lai',        N'Cà phê Robusta Gia Lai',        N'Mạnh mẽ, hậu vị gỗ.',                             'robusta', 110000),
 ('ca-phe-robusta-honey',          N'Cà phê Robusta Honey',          N'Robusta sơ chế honey, bớt đắng gắt.',             'robusta', 150000),
 ('ca-phe-robusta-natural',        N'Cà phê Robusta Natural',        N'Phơi tự nhiên, hương trái cây khô.',              'robusta', 135000),
 ('ca-phe-culi-robusta',           N'Cà phê Culi Robusta',           N'Hạt culi tròn, đậm và sánh.',                     'robusta', 140000),
 ('ca-phe-blend-espresso',         N'Cà phê Blend Espresso',         N'Phối trộn cho máy espresso, crema dày.',          'blend',   160000),
 ('ca-phe-blend-house',            N'Cà phê Blend House',            N'Công thức của quán, dễ uống mỗi ngày.',           'blend',   145000),
 ('ca-phe-blend-sang-tao',         N'Cà phê Blend Sáng Tạo',         N'Arabica + Robusta cân bằng, đậm đà.',             'blend',   155000),
 ('ca-phe-blend-truyen-thong',     N'Cà phê Blend Truyền Thống',     N'Gu Việt đậm đắng, hợp pha phin.',                 'blend',   130000),
 ('ca-phe-blend-moka',             N'Cà phê Blend Moka',             N'Thêm Moka cho hương thơm đặc trưng.',             'blend',   175000),
 ('ca-phe-blend-phin-viet',        N'Cà phê Blend Phin Việt',        N'Tối ưu cho phin, nước cốt sánh.',                 'blend',   125000),
 ('ca-phe-decaf-arabica',          N'Cà phê Decaf Arabica',          N'Đã khử caffeine, giữ hương Arabica.',             'decaf',   195000),
 ('ca-phe-decaf-blend',            N'Cà phê Decaf Blend',            N'Decaf phối trộn, vị tròn đầy.',                   'decaf',   165000),
 ('ca-phe-decaf-swiss-water',      N'Cà phê Decaf Swiss Water',      N'Khử caffeine bằng nước, không hóa chất.',         'decaf',   210000)
) AS v(SLUG, NAME, DESCR, CAT_SLUG, BASE_PRICE)
JOIN dbo.CATEGORIES c ON c.SLUG = v.CAT_SLUG;

/* ---------- 4. PRODUCT_VARIANTS (250g + 500g per product) ---------- */
INSERT INTO dbo.PRODUCT_VARIANTS (PRODUCT_ID, SKU, ATTRIBUTES_JSON, PRICE, COMPARE_PRICE, WEIGHT, CREATED_BY)
SELECT p.ID,
       UPPER(REPLACE(p.SLUG, '-', '_')) + '_' + s.CODE,
       s.ATTR,
       ROUND(p.BASE_PRICE * s.MULT, -3),
       CASE WHEN s.CODE = '500G' THEN ROUND(p.BASE_PRICE * 2.1, -3) END,
       s.WEIGHT,
       @ADMIN
FROM dbo.PRODUCTS p
CROSS JOIN (VALUES
   ('250G', N'{"weight":"250g","grind":"whole-bean"}', CAST(1.0 AS DECIMAL(5,2)), CAST(250 AS DECIMAL(10,2))),
   ('500G', N'{"weight":"500g","grind":"whole-bean"}', CAST(1.9 AS DECIMAL(5,2)), CAST(500 AS DECIMAL(10,2)))
) AS s(CODE, ATTR, MULT, WEIGHT);

/* ---------- 5. INVENTORIES (1-1 with variant) ---------- */
INSERT INTO dbo.INVENTORIES (VARIANT_ID, ON_HAND, RESERVED, CREATED_BY)
SELECT v.ID, 40 + (ABS(CHECKSUM(NEWID())) % 160), 0, @ADMIN
FROM dbo.PRODUCT_VARIANTS v;

/* ---------- 6. COUPONS ---------- */
INSERT INTO dbo.COUPONS (CODE, [TYPE], VALUE, MIN_ORDER_AMOUNT, MAX_DISCOUNT_AMOUNT, MAX_USAGE, MAX_USAGE_PER_USER, START_AT, END_AT, CREATED_BY) VALUES
 (N'WELCOME10', 'Percent', 10,    200000, 50000, 1000, 1, DATEADD(DAY,-30,@NOW), DATEADD(DAY,30,@NOW),  @ADMIN),
 (N'FREESHIP',  'Fixed',   30000, 300000, NULL,  500,  2, DATEADD(DAY,-10,@NOW), DATEADD(DAY,20,@NOW),  @ADMIN),
 (N'COFFEE50K', 'Fixed',   50000, 500000, NULL,  200,  1, DATEADD(DAY,-5,@NOW),  DATEADD(DAY,25,@NOW),  @ADMIN);

/* ---------- 7. ADDRESSES ---------- */
INSERT INTO dbo.ADDRESSES (USER_ID, RECIPIENT_NAME, PHONE, PROVINCE, DISTRICT, WARD, STREET, IS_DEFAULT, CREATED_BY) VALUES
 (@CUSTOMER1, N'Nguyễn Văn An', N'0901234567', N'Hà Nội',     N'Cầu Giấy', N'Dịch Vọng',  N'123 Trần Thái Tông', 1, @CUSTOMER1),
 (@CUSTOMER2, N'Trần Thị Bình', N'0907654321', N'TP.HCM',     N'Quận 1',   N'Bến Nghé',   N'45 Lê Lợi',          1, @CUSTOMER2),
 (@CUSTOMER3, N'Lê Văn Cường',  N'0912345678', N'Đà Nẵng',    N'Hải Châu', N'Thạch Thang',N'78 Bạch Đằng',       1, @CUSTOMER3);

/* ---------- 8. ORDERS (subtotal/total recomputed after items) ---------- */
DECLARE @ORDER1 UNIQUEIDENTIFIER = NEWID();  -- Pending  (COD)
DECLARE @ORDER2 UNIQUEIDENTIFIER = NEWID();  -- Confirmed(VNPay + coupon)
DECLARE @ORDER3 UNIQUEIDENTIFIER = NEWID();  -- Shipping (COD)
DECLARE @ORDER4 UNIQUEIDENTIFIER = NEWID();  -- Delivered(VNPay)
DECLARE @ORDER5 UNIQUEIDENTIFIER = NEWID();  -- Completed(COD)
DECLARE @ORDER6 UNIQUEIDENTIFIER = NEWID();  -- Cancelled(VNPay)

DECLARE @ADDR1 NVARCHAR(MAX) = N'{"recipientName":"Nguyễn Văn An","phone":"0901234567","province":"Hà Nội","district":"Cầu Giấy","ward":"Dịch Vọng","street":"123 Trần Thái Tông"}';
DECLARE @ADDR2 NVARCHAR(MAX) = N'{"recipientName":"Trần Thị Bình","phone":"0907654321","province":"TP.HCM","district":"Quận 1","ward":"Bến Nghé","street":"45 Lê Lợi"}';
DECLARE @ADDR3 NVARCHAR(MAX) = N'{"recipientName":"Lê Văn Cường","phone":"0912345678","province":"Đà Nẵng","district":"Hải Châu","ward":"Thạch Thang","street":"78 Bạch Đằng"}';

INSERT INTO dbo.ORDERS
 (ID, ORDER_NUMBER, USER_ID, STATUS, SUBTOTAL, SHIPPING_FEE, DISCOUNT, TOTAL, COUPON_CODE, SHIPPING_ADDRESS_JSON, NOTE, PAYMENT_METHOD,
  CONFIRMED_AT, SHIPPED_AT, DELIVERED_AT, COMPLETED_AT, CANCELLED_AT, CANCEL_REASON, CREATED_AT, CREATED_BY)
VALUES
 (@ORDER1, 'ORD20260610-0001', @CUSTOMER1, 'Pending',   0, 30000,     0, 0, NULL,        @ADDR1, N'Giao giờ hành chính', 'COD',
   NULL, NULL, NULL, NULL, NULL, NULL, @NOW, @CUSTOMER1),
 (@ORDER2, 'ORD20260609-0002', @CUSTOMER1, 'Confirmed', 0, 30000, 20000, 0, N'WELCOME10', @ADDR1, NULL, 'VNPay',
   DATEADD(DAY,-1,@NOW), NULL, NULL, NULL, NULL, NULL, DATEADD(DAY,-1,@NOW), @CUSTOMER1),
 (@ORDER3, 'ORD20260607-0003', @CUSTOMER2, 'Shipping',  0, 35000,     0, 0, NULL,        @ADDR2, NULL, 'COD',
   DATEADD(DAY,-3,@NOW), DATEADD(DAY,-1,@NOW), NULL, NULL, NULL, NULL, DATEADD(DAY,-3,@NOW), @CUSTOMER2),
 (@ORDER4, 'ORD20260603-0004', @CUSTOMER2, 'Delivered', 0, 30000,     0, 0, NULL,        @ADDR2, NULL, 'VNPay',
   DATEADD(DAY,-7,@NOW), DATEADD(DAY,-6,@NOW), DATEADD(DAY,-4,@NOW), NULL, NULL, NULL, DATEADD(DAY,-7,@NOW), @CUSTOMER2),
 (@ORDER5, 'ORD20260529-0005', @CUSTOMER3, 'Completed', 0, 30000,     0, 0, NULL,        @ADDR3, NULL, 'COD',
   DATEADD(DAY,-12,@NOW), DATEADD(DAY,-11,@NOW), DATEADD(DAY,-9,@NOW), DATEADD(DAY,-6,@NOW), NULL, NULL, DATEADD(DAY,-12,@NOW), @CUSTOMER3),
 (@ORDER6, 'ORD20260608-0006', @CUSTOMER1, 'Cancelled', 0, 30000,     0, 0, NULL,        @ADDR1, NULL, 'VNPay',
   NULL, NULL, NULL, NULL, DATEADD(DAY,-2,@NOW), N'Khách đổi ý', DATEADD(DAY,-2,@NOW), @CUSTOMER1);

/* ---------- 9. ORDER_ITEMS (snapshot from current variant) ---------- */
-- helper inline: insert one line by SKU
INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER1, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 2, v.PRICE*2, @CUSTOMER1
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_ARABICA_CAU_DAT_250G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER2, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 1, v.PRICE*1, @CUSTOMER1
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_ARABICA_BOURBON_500G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER2, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 2, v.PRICE*2, @CUSTOMER1
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_BLEND_ESPRESSO_250G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER3, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 3, v.PRICE*3, @CUSTOMER2
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_ROBUSTA_BUON_MA_THUOT_250G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER4, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 2, v.PRICE*2, @CUSTOMER2
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_ARABICA_HONEY_250G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER5, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 1, v.PRICE*1, @CUSTOMER3
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_BLEND_HOUSE_500G';

INSERT INTO dbo.ORDER_ITEMS (ORDER_ID, VARIANT_ID, PRODUCT_NAME_SNAPSHOT, VARIANT_SKU_SNAPSHOT, VARIANT_ATTRIBUTES_SNAPSHOT, UNIT_PRICE_SNAPSHOT, QUANTITY, LINE_TOTAL, CREATED_BY)
SELECT @ORDER6, v.ID, p.NAME, v.SKU, v.ATTRIBUTES_JSON, v.PRICE, 1, v.PRICE*1, @CUSTOMER1
FROM dbo.PRODUCT_VARIANTS v JOIN dbo.PRODUCTS p ON p.ID = v.PRODUCT_ID
WHERE v.SKU = 'CA_PHE_DECAF_SWISS_WATER_250G';

/* recompute SUBTOTAL/TOTAL from items */
UPDATE o
SET SUBTOTAL = x.S,
    TOTAL    = x.S + o.SHIPPING_FEE - o.DISCOUNT
FROM dbo.ORDERS o
JOIN (SELECT ORDER_ID, SUM(LINE_TOTAL) AS S FROM dbo.ORDER_ITEMS GROUP BY ORDER_ID) x
  ON x.ORDER_ID = o.ID;

/* ---------- 10. PAYMENTS ---------- */
INSERT INTO dbo.PAYMENTS (ORDER_ID, METHOD, AMOUNT, STATUS, TXN_REF, GATEWAY_RESPONSE_JSON, PAID_AT, CREATED_BY) VALUES
 (@ORDER1, 'COD',   (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER1), 'Pending', NULL,                NULL,                                          NULL,                    @CUSTOMER1),
 (@ORDER2, 'VNPay', (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER2), 'Success', N'VNP16012345602',   N'{"vnp_ResponseCode":"00","vnp_TxnRef":"VNP16012345602"}', DATEADD(DAY,-1,@NOW), @CUSTOMER1),
 (@ORDER3, 'COD',   (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER3), 'Pending', NULL,                NULL,                                          NULL,                    @CUSTOMER2),
 (@ORDER4, 'VNPay', (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER4), 'Success', N'VNP16012345604',   N'{"vnp_ResponseCode":"00","vnp_TxnRef":"VNP16012345604"}', DATEADD(DAY,-7,@NOW), @CUSTOMER2),
 (@ORDER5, 'COD',   (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER5), 'Success', NULL,                N'{"collectedOnDelivery":true}',               DATEADD(DAY,-9,@NOW),    @STAFF),
 (@ORDER6, 'VNPay', (SELECT TOTAL FROM dbo.ORDERS WHERE ID=@ORDER6), 'Failed',  N'VNP16012345606',   N'{"vnp_ResponseCode":"24","message":"Customer cancelled"}', NULL,           @CUSTOMER1);

/* ---------- 11. SHIPMENTS ---------- */
INSERT INTO dbo.SHIPMENTS (ORDER_ID, PROVIDER, TRACKING_NUMBER, STATUS, SHIPPED_AT, DELIVERED_AT, COST_FEE, CREATED_BY) VALUES
 (@ORDER3, 'GHTK', N'GHTK20260607001', 'Shipping',  DATEADD(DAY,-1,@NOW), NULL,                28000, @STAFF),
 (@ORDER4, 'GHN',  N'GHN20260603001',  'Delivered', DATEADD(DAY,-6,@NOW), DATEADD(DAY,-4,@NOW), 30000, @STAFF);

/* ---------- 12. REVIEWS (order 5 completed) ---------- */
INSERT INTO dbo.REVIEWS (PRODUCT_ID, USER_ID, ORDER_ITEM_ID, RATING, COMMENT, IS_APPROVED, CREATED_AT, CREATED_BY)
SELECT pv.PRODUCT_ID, @CUSTOMER3, oi.ID, 5, N'Cà phê thơm đậm, đóng gói đẹp, giao nhanh!', 1, DATEADD(DAY,-5,@NOW), @CUSTOMER3
FROM dbo.ORDER_ITEMS oi
JOIN dbo.PRODUCT_VARIANTS pv ON pv.ID = oi.VARIANT_ID
WHERE oi.ORDER_ID = @ORDER5;

/* ---------- 13. ORDER_AUDIT_LOGS ---------- */
INSERT INTO dbo.ORDER_AUDIT_LOGS (ORDER_ID, FROM_STATUS, TO_STATUS, CHANGED_BY, REASON, CHANGED_AT) VALUES
 (@ORDER4, NULL,        'Pending',   @CUSTOMER2, N'Đặt hàng',                  DATEADD(DAY,-7,@NOW)),
 (@ORDER4, 'Pending',   'Confirmed', NULL,       N'Thanh toán VNPay thành công',DATEADD(DAY,-7,@NOW)),
 (@ORDER4, 'Confirmed', 'Packed',    @STAFF,     N'Đóng gói',                  DATEADD(DAY,-6,@NOW)),
 (@ORDER4, 'Packed',    'Shipping',  @STAFF,     N'Tạo vận đơn GHN',           DATEADD(DAY,-6,@NOW)),
 (@ORDER4, 'Shipping',  'Delivered', NULL,       N'Webhook giao thành công',   DATEADD(DAY,-4,@NOW)),
 (@ORDER6, NULL,        'Pending',   @CUSTOMER1, N'Đặt hàng',                  DATEADD(DAY,-2,@NOW)),
 (@ORDER6, 'Pending',   'Cancelled', @CUSTOMER1, N'Khách đổi ý',               DATEADD(DAY,-2,@NOW));

COMMIT TRAN;

PRINT 'Seed completed.';
SELECT
    (SELECT COUNT(*) FROM dbo.USERS)            AS Users,
    (SELECT COUNT(*) FROM dbo.CATEGORIES)       AS Categories,
    (SELECT COUNT(*) FROM dbo.PRODUCTS)         AS Products,
    (SELECT COUNT(*) FROM dbo.PRODUCT_VARIANTS) AS Variants,
    (SELECT COUNT(*) FROM dbo.INVENTORIES)      AS Inventories,
    (SELECT COUNT(*) FROM dbo.COUPONS)          AS Coupons,
    (SELECT COUNT(*) FROM dbo.ORDERS)           AS Orders,
    (SELECT COUNT(*) FROM dbo.ORDER_ITEMS)      AS OrderItems,
    (SELECT COUNT(*) FROM dbo.PAYMENTS)         AS Payments,
    (SELECT COUNT(*) FROM dbo.SHIPMENTS)        AS Shipments,
    (SELECT COUNT(*) FROM dbo.REVIEWS)          AS Reviews,
    (SELECT COUNT(*) FROM dbo.ORDER_AUDIT_LOGS) AS AuditLogs;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    PRINT 'Seed failed: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
