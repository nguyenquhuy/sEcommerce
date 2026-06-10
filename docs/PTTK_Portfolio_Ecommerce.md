# PHÂN TÍCH & THIẾT KẾ NGHIỆP VỤ
## Website Bán Hàng — Portfolio Project

> **Tác giả:** Nguyễn Quang Huy
> **Mục đích:** Project portfolio để nhảy việc, target deal lương 22–25M
> **Thời lượng dự kiến:** 8 tuần (part-time, đi làm full-time)
> **Phiên bản:** 1.0
> **Ngày tạo:** 2026-06-02

---

## 📑 MỤC LỤC

1. [Tổng quan & Mục tiêu](#1-tổng-quan--mục-tiêu)
2. [Định hướng & Phạm vi](#2-định-hướng--phạm-vi)
3. [Domain Glossary](#3-domain-glossary)
4. [Actors & Phân quyền](#4-actors--phân-quyền)
5. [Use Cases](#5-use-cases)
6. [Business Rules](#6-business-rules)
7. [Edge Cases & Risk](#7-edge-cases--risk)
8. [Luồng nghiệp vụ chi tiết](#8-luồng-nghiệp-vụ-chi-tiết)
9. [State Machines](#9-state-machines)
10. [Data Model](#10-data-model)
11. [Phạm vi chức năng (MVP / Phase 2 / Stretch)](#11-phạm-vi-chức-năng)
12. [Twist để khác biệt portfolio](#12-twist-để-khác-biệt-portfolio)
13. [Kiến trúc đề xuất](#13-kiến-trúc-đề-xuất)
14. [Roadmap 8 tuần](#14-roadmap-8-tuần)
15. [Tiêu chí nghiệm thu](#15-tiêu-chí-nghiệm-thu)
16. [🤖 AI Chatbot + Real-time Customer Support](#16-ai-chatbot--real-time-customer-support)

---

## 1. TỔNG QUAN & MỤC TIÊU

### 1.1 Bối cảnh
- Dev có 1 năm kinh nghiệm tại VietSens (HIS — Hospital Information System).
- Stack hiện tại: .NET Framework 4.5, Oracle, WinForms + DevExpress.
- Cần project portfolio dùng **stack hiện đại** (.NET 8) để chứng minh năng lực với recruiter.
- Project phải build được trong 8 tuần khi đang đi làm full-time.

### 1.2 Mục tiêu kinh doanh (giả định)
Project được thiết kế như một **website bán hàng B2C thật** — tức là **không phải demo cliché**, mà phải đáp ứng đầy đủ luồng nghiệp vụ của shop online thực tế. Việc này quan trọng vì:
- Recruiter senior sẽ catch các thiếu sót cơ bản (snapshot price, idempotency, state machine…).
- Khi phỏng vấn, dev có thể kể câu chuyện về **vì sao chọn pattern X** thay vì chỉ "em làm CRUD".

### 1.3 Mục tiêu kỹ thuật
Cover được các skill modern mà recruiter tìm kiếm:

| Skill | Lý do quan trọng |
|-------|------------------|
| **.NET 8** | Lấp khoảng cách stack vs .NET Framework 4.5 hiện tại |
| **Clean Architecture** | Pattern senior-level, được hỏi nhiều khi phỏng vấn |
| **CQRS + MediatR** | Architecture trên junior level |
| **EF Core 8** | Khác biệt nhỏ với EF6 — cần proficient |
| **Docker + CI/CD** | DevOps cơ bản — đa số junior thiếu |
| **Unit Test (xUnit)** | Production-grade — đa số junior bỏ qua |
| **Cloud deploy** | Demo công khai cho recruiter xem |
| **SignalR / Redis / Background Job** | Tech showcase nâng cao |

### 1.4 Tiêu chí thành công

| # | Tiêu chí | Đo lường |
|---|----------|----------|
| 1 | Demo URL public chạy ổn định | Render/Azure deploy + UptimeRobot xanh |
| 2 | Đầy đủ flow business core | Customer mua hàng + Admin xử lý đơn end-to-end |
| 3 | Test coverage ≥ 60% ở Application layer | Coverlet + Codecov badge |
| 4 | Docker build < 5 phút, image < 250MB | GitHub Actions log |
| 5 | CV bullet point cụ thể, kèm tech keyword | Recruiter dừng cuộn |
| 6 | War stories sẵn sàng kể trong phỏng vấn | 3–4 câu chuyện kỹ thuật concrete |

---

## 2. ĐỊNH HƯỚNG & PHẠM VI

### 2.1 Quyết định scope quan trọng

| Quyết định | Lựa chọn | Lý do |
|-----------|----------|-------|
| **Mô hình kinh doanh** | B2C (single vendor) | Đơn giản nhất, đủ độ phức tạp |
| **Multi-vendor?** | KHÔNG | Tăng độ phức tạp gấp 3, không kịp deadline |
| **Niche sản phẩm** | Chọn 1 cụ thể (xem mục 2.2) | Tránh "general store" nhạt nhẽo |
| **Giao hàng vật lý** | CÓ | Tích hợp GHTK/GHN sandbox |
| **Payment** | COD + VNPay sandbox + (MoMo sandbox) | Đầy đủ flow online + offline |
| **Multi-currency** | KHÔNG | Out of scope MVP |
| **Multi-language** | KHÔNG (chỉ tiếng Việt) | Out of scope MVP |
| **Mobile app** | KHÔNG | Web responsive đủ cho portfolio |
| **Guest checkout** | CHO PHÉP | Tăng UX, đa số shop thật làm vậy |

### 2.2 Niche đề xuất (chọn 1)

| Niche | Feature đặc thù khả thi | Độ nổi bật |
|-------|--------------------------|------------|
| **Cafe đặc sản** | Ngày rang, mức rang, tasting notes, brew guide | ⭐⭐⭐⭐⭐ |
| **Sách cũ** | Tình trạng sách (5 mức), tem ghi chú, lịch sử | ⭐⭐⭐⭐ |
| **Đồ handmade** | Lead time, custom order, ảnh quá trình làm | ⭐⭐⭐⭐⭐ |
| **Mỹ phẩm organic** | % ingredients, quiz da, kết quả thật | ⭐⭐⭐⭐ |
| **Đồ chơi gỗ trẻ em** | Độ tuổi phù hợp, an toàn cert, video | ⭐⭐⭐ |

> **Khuyến nghị:** Chọn **Cafe đặc sản** hoặc **Đồ handmade** — nghiệp vụ đặc thù không thể fake, dễ tạo story phỏng vấn.

### 2.3 Out of scope (rõ ràng từ đầu)

- ❌ Multi-vendor marketplace
- ❌ Subscription / recurring billing
- ❌ Live streaming bán hàng
- ❌ Affiliate program
- ❌ Multi-warehouse logic (chỉ 1 kho)
- ❌ Tax computation phức tạp (chỉ ship + total)
- ❌ International shipping
- ❌ App mobile native

---

## 3. DOMAIN GLOSSARY

| Thuật ngữ | Định nghĩa | Ghi chú |
|-----------|-----------|---------|
| **Customer** | Người mua hàng | Có thể là Guest (chưa đăng ký) hoặc Member |
| **Guest** | Khách vãng lai chưa có tài khoản | Cart lưu theo session |
| **Member** | Customer đã đăng ký | Cart persist trong DB |
| **Product** | Sản phẩm bán | Là logical entity, không bán trực tiếp |
| **Variant** | Biến thể của product | Đây mới là thứ thực sự bán (có SKU, giá, stock) |
| **SKU** | Mã định danh duy nhất cho variant | VD: `COFFEE-ARABICA-250G-MEDIUM` |
| **Cart** | Giỏ hàng tạm | Tồn tại cả với Guest và Member |
| **Order** | Đơn hàng | Snapshot của cart tại thời điểm checkout |
| **OrderItem** | Dòng chi tiết đơn | Lưu snapshot info, không reference Product hiện tại |
| **Inventory** | Tồn kho theo variant | Có OnHand và Reserved |
| **Reserved** | Số lượng đang giữ chỗ cho cart đang checkout | Auto-release sau timeout |
| **Payment** | Bản ghi thanh toán | 1 order có thể có nhiều payment (deposit + final) |
| **Shipment** | Lô giao hàng | 1 order có thể split nhiều shipment |
| **Coupon** | Mã giảm giá | Có rule, có usage limit |
| **Promotion** | Khuyến mãi tự động | Áp dụng theo điều kiện, không cần code |
| **Review** | Đánh giá sản phẩm | Chỉ cho review khi đã mua + đã nhận |
| **Refund** | Hoàn tiền | Có thể partial (theo OrderItem) |

---

## 4. ACTORS & PHÂN QUYỀN

### 4.1 Sơ đồ Actors

```
┌──────────────────────────────────────────────────────────┐
│                    E-COMMERCE SYSTEM                       │
│                                                            │
│   ┌─────────┐    ┌─────────┐    ┌─────────┐   ┌────────┐ │
│   │ Guest   │    │Customer │    │ Staff   │   │ Admin  │ │
│   └─────────┘    └─────────┘    └─────────┘   └────────┘ │
│                                                            │
│   External Systems:                                        │
│   🤖 Payment Gateway (VNPay/MoMo)                         │
│   🤖 Shipping Provider (GHTK/GHN)                         │
│   🤖 Email Service (SMTP)                                 │
│   🤖 SMS Service (optional, sandbox)                      │
└──────────────────────────────────────────────────────────┘
```

### 4.2 Phân quyền chi tiết

| Module | Guest | Customer | Staff | Admin |
|--------|:-----:|:--------:|:-----:|:-----:|
| Xem sản phẩm, danh mục, search | ✅ | ✅ | ✅ | ✅ |
| Thêm/sửa cart | ✅ | ✅ | ❌ | ❌ |
| Checkout & đặt hàng | ❌ | ✅ | ❌ | ❌ |
| Xem lịch sử đơn của mình | ❌ | ✅ | ❌ | ❌ |
| Hủy đơn của mình | ❌ | ✅ | ❌ | ❌ |
| Review sản phẩm | ❌ | ✅ | ❌ | ❌ |
| Quản lý profile | ❌ | ✅ | ❌ | ❌ |
| Xem danh sách tất cả đơn | ❌ | ❌ | ✅ | ✅ |
| Chuyển trạng thái đơn | ❌ | ❌ | ✅ | ✅ |
| Tạo shipment | ❌ | ❌ | ✅ | ✅ |
| Xử lý return request | ❌ | ❌ | ✅ | ✅ |
| Nhập/xuất kho | ❌ | ❌ | ✅ | ✅ |
| CRUD Product, Category, Variant | ❌ | ❌ | ❌ | ✅ |
| CRUD Coupon, Promotion | ❌ | ❌ | ❌ | ✅ |
| Quản lý user, phân quyền | ❌ | ❌ | ❌ | ✅ |
| Xem báo cáo doanh thu | ❌ | ❌ | ❌ | ✅ |
| Cấu hình hệ thống | ❌ | ❌ | ❌ | ✅ |

---

## 5. USE CASES

### 5.1 Customer-facing

#### UC-01: Browse & Search
- **Actor**: Guest, Customer
- **Mô tả**: Xem catalog, filter theo category/price/attribute, search keyword
- **Pre**: Không
- **Post**: Trả về danh sách product phù hợp

#### UC-02: View Product Detail
- **Actor**: Guest, Customer
- **Mô tả**: Xem chi tiết sản phẩm, ảnh, giá theo variant, tồn kho, review
- **Pre**: Product `IsActive = true, IsDeleted = false`
- **Post**: Hiển thị đầy đủ thông tin

#### UC-03: Add to Cart
- **Actor**: Guest, Customer
- **Mô tả**: Thêm variant với quantity vào cart
- **Pre**: Variant available (BR-01)
- **Post**: Cart được cập nhật

#### UC-04: Manage Cart
- **Actor**: Guest, Customer
- **Mô tả**: Xem cart, đổi quantity, xóa item, áp coupon
- **Pre**: Cart tồn tại
- **Post**: Cart được cập nhật, subtotal recalculate

#### UC-05: Register Account
- **Actor**: Guest
- **Mô tả**: Đăng ký tài khoản mới
- **Pre**: Email chưa tồn tại
- **Post**: Account tạo (status Inactive), email verify được gửi

#### UC-06: Login / Logout
- **Actor**: Customer
- **Mô tả**: Đăng nhập bằng email + password
- **Pre**: Account active + verified
- **Post**: Session/JWT được cấp

#### UC-07: Checkout
- **Actor**: Customer
- **Mô tả**: Đi qua các bước: địa chỉ → shipping → payment → confirm
- **Pre**: Cart không rỗng
- **Post**: Order được tạo (status Pending), redirect payment hoặc về order detail

#### UC-08: Pay Order Online
- **Actor**: Customer
- **Mô tả**: Redirect sang VNPay/MoMo, thanh toán, callback về
- **Pre**: Order status = Pending
- **Post**: Order status = Confirmed (nếu success) hoặc Failed

#### UC-09: View Order History
- **Actor**: Customer
- **Mô tả**: List đơn của mình, filter theo status, thời gian
- **Pre**: Logged in
- **Post**: List đơn

#### UC-10: Cancel Order
- **Actor**: Customer
- **Mô tả**: Hủy đơn khi còn ở status cho phép
- **Pre**: Order status ∈ {Pending, Confirmed} + chưa Packed
- **Post**: Order status = Cancelled, restore stock

#### UC-11: Request Return
- **Actor**: Customer
- **Mô tả**: Yêu cầu trả hàng + chọn item, lý do, ảnh
- **Pre**: Order status = Delivered + trong window 7 ngày
- **Post**: ReturnRequest tạo, status = Pending

#### UC-12: Write Review
- **Actor**: Customer
- **Mô tả**: Viết review cho sản phẩm đã mua
- **Pre**: Order status = Completed + chưa review
- **Post**: Review được tạo, status = PendingModeration

#### UC-13: Manage Profile & Addresses
- **Actor**: Customer
- **Mô tả**: CRUD địa chỉ, đổi password, đổi info
- **Pre**: Logged in
- **Post**: Profile được cập nhật

### 5.2 Staff-facing

#### UC-20: Dashboard Orders
- **Actor**: Staff, Admin
- **Mô tả**: Xem danh sách đơn theo status, filter, search
- **Pre**: Logged in + role Staff/Admin
- **Post**: List đơn với KPI

#### UC-21: Process Order
- **Actor**: Staff, Admin
- **Mô tả**: Confirm đơn → Pack → Ship → Track
- **Pre**: Order ở status hợp lệ để chuyển
- **Post**: Order status mới, audit log

#### UC-22: Create Shipment
- **Actor**: Staff, Admin
- **Mô tả**: Tạo shipment, gọi API GHTK/GHN, lấy tracking number
- **Pre**: Order status = Packed
- **Post**: Shipment tạo, order chuyển Shipping

#### UC-23: Handle Return Request
- **Actor**: Staff, Admin
- **Mô tả**: Review request, approve/reject, xử lý refund khi nhận hàng
- **Pre**: ReturnRequest tồn tại
- **Post**: ReturnRequest status mới, refund executed nếu cần

#### UC-24: Inventory Management
- **Actor**: Staff, Admin
- **Mô tả**: Nhập/xuất kho, điều chỉnh tồn
- **Pre**: Variant tồn tại
- **Post**: Inventory cập nhật, log audit

### 5.3 Admin-only

#### UC-30: CRUD Product
- **Actor**: Admin
- **Mô tả**: Tạo product + variant + ảnh + thông tin SEO
- **Post**: Product tạo, status Inactive (chờ duyệt visible)

#### UC-31: CRUD Category
- **Actor**: Admin
- **Mô tả**: Tạo, sửa, xóa category (cho phép tree)
- **Post**: Category tree cập nhật

#### UC-32: CRUD Coupon / Promotion
- **Actor**: Admin
- **Mô tả**: Tạo coupon với rule (% / fixed, min order, max usage, time)
- **Post**: Coupon tạo

#### UC-33: User Management
- **Actor**: Admin
- **Mô tả**: Xem list user, đổi role, khóa/mở account
- **Post**: User updated

#### UC-34: Revenue Report
- **Actor**: Admin
- **Mô tả**: Báo cáo doanh thu theo ngày/tuần/tháng, top sản phẩm
- **Post**: Hiển thị chart + bảng

---

## 6. BUSINESS RULES

### 6.1 Rules core (BẮT BUỘC implement)

| ID | Rule | Mô tả chi tiết |
|----|------|----------------|
| **BR-01** | Stock không âm | Khi add cart hoặc checkout, kiểm tra `Inventory.Available = OnHand - Reserved ≥ ordered_qty` |
| **BR-02** | Reserve stock khi checkout | Khi user vào trang checkout (step địa chỉ), tăng `Reserved` để giữ chỗ 15 phút |
| **BR-03** | Auto-release Reserved | Background job (Hangfire) chạy mỗi 1 phút, release các reserved quá hạn |
| **BR-04** | Snapshot giá tại order | `OrderItem.UnitPriceSnapshot` lưu giá lúc đặt, **không join Product để lấy giá hiện tại** |
| **BR-05** | Snapshot tên sản phẩm | `OrderItem.ProductNameSnapshot` lưu tên lúc đặt — phòng admin đổi tên |
| **BR-06** | Min order amount cho COD | Đơn dưới 50,000 VND không cho COD (chống fake order) |
| **BR-07** | Coupon usage limit | Mỗi user dùng coupon X tối đa Y lần (cấu hình theo coupon) |
| **BR-08** | Coupon stacking | Tối đa 1 coupon/đơn (cho MVP); v2 có thể cho stack có rule |
| **BR-09** | Order cancel window | Customer chỉ hủy được khi status ∈ {Pending, Confirmed} và `!IsPacked` |
| **BR-10** | Return window | Yêu cầu trả hàng phải trong **7 ngày** từ `DeliveredAt` |
| **BR-11** | Review eligibility | Chỉ cho review khi: Order.status = Completed + OrderItem chưa được review |
| **BR-12** | Re-check stock at confirm | Ngay trước khi tạo Order, re-check stock lần cuối (race condition guard) |
| **BR-13** | Payment idempotency | Callback payment dùng `TxnRef` làm idempotency key — đã processed thì return OK |
| **BR-14** | Order audit log | Mọi thay đổi state của order phải log: ai, khi nào, từ → đến, lý do |
| **BR-15** | Email notification | Gửi email sau: order created, payment success, status changed (Confirmed/Shipping/Delivered/Cancelled) |
| **BR-16** | Soft delete Product | Product `IsDeleted = true`, không hard delete; order cũ vẫn reference được |
| **BR-17** | Verified email required | Customer phải verify email mới đăng nhập được |
| **BR-18** | Password policy | Min 8 ký tự, có chữ + số (không bắt buộc ký tự đặc biệt cho MVP) |
| **BR-19** | Auto-complete order | Cron job chạy mỗi ngày, chuyển Delivered → Completed sau 3 ngày không action |
| **BR-20** | Coupon validity check at checkout | Coupon phải còn hạn + còn lượt + đủ min order amount tại thời điểm checkout (không phải apply) |

### 6.2 Rules nâng cao (Phase 2)

| ID | Rule | Mô tả |
|----|------|-------|
| **BR-21** | Loyalty point earning | 1% giá trị đơn → loyalty point (sau Completed) |
| **BR-22** | Loyalty point spending | 1 point = 1,000 VND, tối đa 10% giá trị đơn |
| **BR-23** | Flash sale priority | Khi flash sale, FIFO theo queue, không tăng giá khi thiếu hàng |
| **BR-24** | Wishlist limit | Tối đa 100 sản phẩm/wishlist |
| **BR-25** | Review moderation | Admin duyệt review trước khi public (chống spam) |

---

## 7. EDGE CASES & RISK

> Đây là phần **PHÂN BIỆT Junior vs Mid-level**. Junior chỉ nghĩ "happy path", senior nghĩ về edge case. Đây cũng chính là **war story** để kể trong phỏng vấn.

### 7.1 Race condition & Concurrency

| Edge case | Hậu quả nếu không xử lý | Giải pháp |
|-----------|-------------------------|-----------|
| 2 user cùng add món còn 1 cái → cùng checkout | Oversell — đặt được 2 đơn với 1 món | Reserve cụ thể từng cart, dùng `Reserved` column + Redis distributed lock |
| Stock thay đổi giữa lúc apply coupon | Coupon mất rule "min order amount" | Re-validate coupon tại bước Confirm |
| Payment callback đến 2 lần (VNPay retry) | Stock bị trừ 2 lần | Idempotency key dựa trên `TxnRef`, check trước khi process |
| User đóng tab khi đang checkout | Stock bị reserve mãi mãi | Background job auto-release sau 15p |
| 2 staff cùng confirm 1 đơn | Status update race | Optimistic concurrency với `RowVersion` |
| User đặt đơn, ngay lúc đó admin xóa product | OrderItem reference nil | Soft delete + snapshot (BR-04, BR-16) |

### 7.2 Business edge cases

| Edge case | Giải pháp |
|-----------|-----------|
| User refund partial (chỉ 1 item) | OrderItem level refund, không phải Order level — Order có thể có nhiều refund |
| Coupon hết hạn giữa lúc checkout | Re-validate tại Confirm step, hiển thị message rõ ràng |
| Shipping fee thay đổi theo địa chỉ | Calculate dynamic mỗi khi đổi addr, snapshot khi confirm order |
| User xóa account khi đã có đơn | Anonymize data (User.Email = `deleted_{id}@deleted`), giữ order |
| Hệ thống crash giữa lúc payment success | Outbox pattern — pending event + retry job |
| Shipping provider trả về fail sau khi đã update order | Compensation: rollback order về Confirmed + alert admin |
| User dùng cùng email đăng ký nhiều lần | Unique constraint + friendly error |
| Cart guest → login → merge với cart member | Merge logic: cộng quantity, ưu tiên member |
| Sản phẩm khuyến mãi hết hạn nhưng còn trong cart | Hiển thị giá hiện tại + flag "giá đã thay đổi" |

### 7.3 Security & Compliance

| Risk | Giải pháp |
|------|-----------|
| SQL injection | EF Core parameterized query (mặc định) — không dùng raw SQL với string concat |
| XSS trong review/comment | HTML encode output, sanitize input |
| CSRF trong form | ASP.NET Core anti-forgery token |
| Password storage | BCrypt hoặc Argon2id, không phải SHA |
| JWT token leak | HttpOnly cookie, không lưu LocalStorage |
| Brute force login | Rate limiting (5 lần/phút) + captcha sau 3 lần fail |
| Payment signature spoofing | Verify HMAC-SHA512 cho callback (VNPay), HMAC-SHA256 (MoMo) |
| Sensitive log | Không log payment card info, password, OTP |
| File upload | Whitelist extension, max size 5MB, scan content |
| API rate limit | AspNetCoreRateLimit theo IP + theo user |

---

## 8. LUỒNG NGHIỆP VỤ CHI TIẾT

### 8.1 Luồng đăng ký tài khoản

```
[Customer]                [System]                   [Email Service]
    │                         │                            │
    │── 1. Truy cập /register │                            │
    │────────────────────────→│                            │
    │                         │                            │
    │ 2. Nhập email, pwd,     │                            │
    │    name, phone          │                            │
    │────────────────────────→│                            │
    │                         │                            │
    │                         │ 3. Validate:               │
    │                         │  - Email format            │
    │                         │  - Email chưa tồn tại      │
    │                         │  - Password mạnh           │
    │                         │  - Phone VN format         │
    │                         │                            │
    │                         │ 4. Hash password (BCrypt)  │
    │                         │ 5. Tạo User Inactive       │
    │                         │ 6. Gen verify token        │
    │                         │                            │
    │                         │── 7. Send verify email ───→│
    │                         │                            │
    │ 8. Show "Check email"   │                            │
    │←────────────────────────│                            │
    │                         │                            │
    │                                                      │
    │ 9. Click verify link    │                            │
    │ /verify?token=xxx       │                            │
    │────────────────────────→│                            │
    │                         │                            │
    │                         │ 10. Validate token         │
    │                         │ 11. User.Status = Active   │
    │                         │ 12. Auto-login (set cookie)│
    │                         │                            │
    │ 13. Redirect Home       │                            │
    │←────────────────────────│                            │
```

### 8.2 Luồng mua hàng (CORE - phải nắm chắc)

```
┌───────────────────────────────────────────────────────────┐
│                    BROWSE & SELECT                         │
└───────────────────────────────────────────────────────────┘
  Customer ──→ /products ──→ /products/{slug}
                 │              │
                 │              ├── Chọn variant (size, màu)
                 │              ├── Chọn quantity
                 │              └── Click "Add to Cart"

┌───────────────────────────────────────────────────────────┐
│                       ADD TO CART                          │
└───────────────────────────────────────────────────────────┘
  [System]:
  1. Validate variant tồn tại + IsActive
  2. Check BR-01: Available ≥ quantity
  3. If Guest: tạo/update Cart theo SessionId
     If Member: update Cart theo UserId
  4. Add/Update CartItem
  5. Return cart count + total

┌───────────────────────────────────────────────────────────┐
│                         VIEW CART                          │
└───────────────────────────────────────────────────────────┘
  Customer ──→ /cart
  [System]:
  1. List items + current price (không phải snapshot)
  2. Show subtotal
  3. Allow apply coupon
  4. Re-check stock từng item, flag "đã hết" nếu có

┌───────────────────────────────────────────────────────────┐
│                        CHECKOUT                            │
└───────────────────────────────────────────────────────────┘
  Step 1: /checkout/address
    [Customer]: Chọn/tạo địa chỉ ship
    [System]:
      - BR-02: Reserve stock cho 15 phút
      - Lưu ShippingAddress vào session

  Step 2: /checkout/shipping
    [Customer]: Chọn shipping method (GHTK Standard / Express)
    [System]:
      - Calculate shipping fee dynamic theo addr + items
      - Show breakdown: Subtotal + Shipping - Discount = Total

  Step 3: /checkout/payment
    [Customer]: Chọn payment method (COD / VNPay / MoMo)
    [System]:
      - BR-06: Validate min order amount cho COD
      - Show review summary

  Step 4: Confirm
    [Customer]: Click "Đặt hàng"
    [System]:
      1. BR-12: Re-check stock lần cuối
      2. BR-20: Re-validate coupon
      3. Begin transaction:
         a. Tạo Order với:
            - Status = Pending
            - Snapshot: address, prices, names, attributes
            - OrderNumber = generate unique
         b. Tạo OrderItems với snapshot
         c. Tạo Payment record (status Pending)
         d. Trừ stock thật + release reserved
         e. Increment coupon.UsedCount
         f. Insert OrderAuditLog
      4. Commit transaction
      5. Publish OrderCreatedEvent (outbox pattern)

  Step 5: Redirect theo payment method
    Nếu COD:
      → /orders/{number}/success
      → Background: send confirmation email
    Nếu VNPay/MoMo:
      → Redirect tới gateway URL
```

### 8.3 Luồng thanh toán VNPay (chi tiết)

```
┌─────────────┐                  ┌─────────────┐
│  Customer   │                  │   Backend   │
└──────┬──────┘                  └──────┬──────┘
       │                                │
       │ 1. POST /api/orders            │
       │───────────────────────────────→│
       │                                │
       │                                │ 2. Tạo Order + Payment
       │                                │    status = Pending
       │                                │ 3. Build VNPay URL với:
       │                                │    - vnp_TxnRef = unique
       │                                │    - vnp_Amount
       │                                │    - vnp_OrderInfo
       │                                │    - vnp_ReturnUrl
       │                                │    - vnp_SecureHash
       │ 4. {payment_url}               │
       │←───────────────────────────────│
       │                                │
       │ 5. Redirect tới VNPay          │
       ├──────────→ ┌──────────┐        │
       │            │  VNPay   │        │
       │            └─────┬────┘        │
       │ 6. Nhập thẻ, OTP │             │
       │ ←─→              │             │
       │                  │             │
       │ 7. VNPay xử lý   │             │
       │                  │             │
       │ 8a. Redirect về  │             │
       │ ReturnURL với    │             │
       │ params + hash    │             │
       │←─────────────────│             │
       │                                │
       │ 9. GET /payment/return         │
       │ ?vnp_*=...                     │
       │───────────────────────────────→│
       │                                │ 10. Show "Đang xử lý..."
       │                                │     (không update DB ở đây)
       │←───────────────────────────────│
       │                                │
       │                  │ 8b. ✅ IPN URL (server-to-server):
       │                  │ POST /payment/vnpay-ipn
       │                  ├────────────→│
       │                  │             │
       │                  │             │ 11. Verify HMAC signature
       │                  │             │ 12. Lookup Payment by TxnRef
       │                  │             │ 13. BR-13: Check idempotency
       │                  │             │     If Status == Success → return OK
       │                  │             │ 14. Update Payment.Status
       │                  │             │ 15. Update Order.Status = Confirmed
       │                  │             │ 16. Publish PaymentSucceededEvent
       │                  │             │ 17. Return HTTP 200 RspCode=00
       │                  │←────────────│
       │                                │
       │ 18. Poll GET /orders/{id}      │
       │───────────────────────────────→│
       │                                │
       │ 19. Status = Confirmed         │
       │←───────────────────────────────│
       │                                │
       │ 20. Show success page          │
       │                                │
       │     [Background worker]:       │
       │     - Send email confirm       │
       │     - Notify staff             │
       │     - Update SignalR dashboard │
```

**Quan trọng:**
- **KHÔNG** dựa vào ReturnURL (user click) để cập nhật trạng thái — user có thể đóng tab giữa chừng.
- **CHỈ** trust IPN URL (server-to-server) sau khi verify signature.
- Idempotency là CRITICAL — VNPay có thể gọi IPN 2-3 lần.

### 8.4 Luồng Admin xử lý đơn

```
[Staff]                  [System]                   [Customer]
   │                         │                          │
   │ 1. Vào /admin/orders    │                          │
   │ filter: status = Pending│                          │
   │────────────────────────→│                          │
   │ 2. List đơn mới         │                          │
   │←────────────────────────│                          │
   │                         │                          │
   │ 3. Click confirm đơn    │                          │
   │────────────────────────→│                          │
   │                         │                          │
   │                         │ 4. Validate:             │
   │                         │  - Status == Confirmed   │
   │                         │  - Payment đã success    │
   │                         │  hoặc method = COD       │
   │                         │                          │
   │                         │ 5. Update Status = Packed│
   │                         │ 6. Insert AuditLog       │
   │                         │ 7. Send email staff      │
   │                         │                          │
   │ 8. Show "Đã đóng gói"   │                          │
   │←────────────────────────│                          │
   │                         │                          │
   │ 9. Click "Tạo shipment" │                          │
   │────────────────────────→│                          │
   │                         │                          │
   │                         │ 10. Call GHTK/GHN API:   │
   │                         │  - Create shipment       │
   │                         │  - Get tracking number   │
   │                         │                          │
   │                         │ 11. Tạo Shipment record  │
   │                         │ 12. Status = Shipping    │
   │                         │ 13. Send email customer ─┼──→ (Email)
   │                         │     "Đơn đang giao..."   │
   │ 14. Show tracking       │                          │
   │←────────────────────────│                          │
   │                         │                          │
   │            ... (chờ giao hàng) ...                 │
   │                         │                          │
   │                         │ ← Webhook từ shipping ──│
   │                         │   "Đã giao thành công"   │
   │                         │                          │
   │                         │ 15. Status = Delivered   │
   │                         │ 16. Set DeliveredAt      │
   │                         │ 17. Send email customer ─┼──→ (Email)
   │                         │     "Đánh giá ngay"       │
   │                         │                          │
   │                         │   [Cron sau 3 ngày]:     │
   │                         │ 18. Auto Status =        │
   │                         │     Completed (BR-19)    │
```

### 8.5 Luồng hoàn trả

```
[Customer]                [System]                   [Staff]
    │                         │                          │
    │ 1. Vào order detail     │                          │
    │ status = Delivered      │                          │
    │ trong 7 ngày            │                          │
    │────────────────────────→│                          │
    │                         │                          │
    │ 2. Click "Trả hàng"     │                          │
    │ Chọn items + lý do      │                          │
    │ + upload ảnh            │                          │
    │────────────────────────→│                          │
    │                         │                          │
    │                         │ 3. BR-10: Check window   │
    │                         │ 4. Tạo ReturnRequest     │
    │                         │    + ReturnItems         │
    │                         │    Status = Pending      │
    │                         │ 5. Notify staff ─────────┼─→
    │                         │                          │
    │ 6. Show "Đã gửi yêu cầu"│                          │
    │←────────────────────────│                          │
    │                         │                          │
    │                         │ 7. Staff review          │
    │                         │←─────────────────────────│
    │                         │                          │
    │                         │ 8a. Approve              │
    │                         │   Status = Approved      │
    │                         │   Send instructions ─────┼─→
    │                         │                          │
    │                         │ 8b. Reject               │
    │                         │   Status = Rejected      │
    │                         │   + reason               │
    │                         │                          │
    │ (Nếu approved):         │                          │
    │ 9. Gửi hàng về kho      │                          │
    │ ──────────→ (vận chuyển vật lý)                   │
    │                         │                          │
    │                         │ 10. Staff nhận hàng,     │
    │                         │     check chất lượng     │
    │                         │←─────────────────────────│
    │                         │                          │
    │                         │ 11. Status = Received    │
    │                         │ 12. Process refund:      │
    │                         │   - COD: manual bank     │
    │                         │   - Online: VNPay refund │
    │                         │     API                  │
    │                         │ 13. Cộng lại Inventory   │
    │                         │ 14. Status = Refunded    │
    │                         │ 15. Send email customer  │
```

---

## 9. STATE MACHINES

### 9.1 Order State Machine

```
                    ┌──────────────┐
                    │   PENDING    │  (chờ thanh toán hoặc COD chưa confirm)
                    └──────┬───────┘
                           │
                ┌──────────┼──────────┐
                ↓          ↓          ↓
         payment_success cancel    timeout
         hoặc COD       by user    (15min)
         confirmed         │          │
                │          ↓          ↓
                │      ┌─────────────────┐
                │      │   CANCELLED     │
                │      │ (release stock) │
                │      └─────────────────┘
                ↓
         ┌─────────────┐
         │  CONFIRMED  │
         └──────┬──────┘
                │ staff click "Đóng gói"
                ↓
         ┌─────────────┐
         │   PACKED    │
         └──────┬──────┘
                │ staff click "Tạo shipment"
                ↓
         ┌─────────────┐
         │  SHIPPING   │ ←── webhook tracking
         └──────┬──────┘
                │ delivered
                ↓
         ┌─────────────┐
         │  DELIVERED  │
         └──────┬──────┘
                │
       ┌────────┼────────┐
       ↓        ↓        ↓
   3 days    customer  customer
   no act.   request   click
   (cron)    return    "Đã nhận"
       │        │           │
       ↓        ↓           ↓
  ┌─────────┐ ┌─────────┐ ┌─────────┐
  │COMPLETED│ │RETURNING│ │COMPLETED│
  └─────────┘ └────┬────┘ └─────────┘
                   │ staff nhận hàng
                   ↓
              ┌──────────┐
              │ RETURNED │
              └────┬─────┘
                   │ refund processed
                   ↓
              ┌──────────┐
              │ REFUNDED │
              └──────────┘
```

#### Bảng transitions chi tiết

| From | To | Trigger | Điều kiện | Action |
|------|-----|---------|-----------|--------|
| (none) | Pending | Customer confirm checkout | Stock OK, valid data | Create Order, reserve stock |
| Pending | Confirmed | Payment IPN success | HMAC valid, idempotency OK | Update payment, send email |
| Pending | Confirmed | Staff confirm COD | Manual action | Update by staff |
| Pending | Cancelled | Customer cancel | < 15min từ tạo | Release reserved |
| Pending | Cancelled | System timeout | > 15min không paid | Release reserved (cron) |
| Confirmed | Cancelled | Customer cancel | < 24h + chưa Packed (BR-09) | Release stock, refund nếu đã paid |
| Confirmed | Packed | Staff action | Order còn hợp lệ | Audit log |
| Packed | Shipping | Staff create shipment | Đã có địa chỉ + items | Call shipping API |
| Shipping | Delivered | Webhook shipping | Webhook signature valid | Set DeliveredAt |
| Delivered | Returning | Customer request return | Trong 7 ngày (BR-10) | Tạo ReturnRequest |
| Delivered | Completed | Cron 3 ngày | Không action | Audit log, enable review |
| Returning | Returned | Staff nhận hàng | Đã verify hàng | Audit log |
| Returned | Refunded | Refund executed | Refund success | Restore stock |

### 9.2 Payment State Machine

```
   PENDING ──→ PROCESSING ──→ SUCCESS
                  │
                  ↓
               FAILED ──→ (retry possible)
                  │
                  ↓
               EXPIRED
```

### 9.3 Cart Lifecycle

```
   CREATED (add first item)
      │
      ↓
   ACTIVE (có item, đang update)
      │
      ├──→ CONVERTED (checkout success → Order)
      │
      ├──→ EXPIRED (30 ngày không activity → cleanup)
      │
      └──→ MERGED (guest login → merge vào cart member)
```

---

## 10. DATA MODEL

### 10.1 Sơ đồ ER (high-level)

```
USER ─────────┬─── ADDRESS
              ├─── CART ──── CART_ITEM
              ├─── ORDER ─── ORDER_ITEM
              │      │
              │      ├─── PAYMENT
              │      ├─── SHIPMENT
              │      └─── RETURN_REQUEST ── RETURN_ITEM
              ├─── REVIEW
              └─── WISHLIST_ITEM

CATEGORY ──── PRODUCT ──── PRODUCT_VARIANT ──── INVENTORY
                                  │
                                  └── (referenced bởi CartItem, OrderItem)

COUPON
PROMOTION

ORDER_AUDIT_LOG  (history)
```

### 10.2 Bảng chi tiết các entity chính

#### USER
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| Email | nvarchar(256) | UNIQUE, NOT NULL | |
| PasswordHash | nvarchar(500) | NOT NULL | BCrypt |
| FullName | nvarchar(200) | NOT NULL | |
| Phone | nvarchar(20) | | VN format |
| Role | nvarchar(20) | NOT NULL | Customer/Staff/Admin |
| IsEmailVerified | bit | DEFAULT 0 | |
| EmailVerifyToken | nvarchar(200) | | |
| EmailVerifyExpiry | datetime2 | | |
| IsActive | bit | DEFAULT 1 | Khóa account |
| LoyaltyPoint | int | DEFAULT 0 | Phase 2 |
| CreatedAt | datetime2 | NOT NULL | |
| UpdatedAt | datetime2 | | |
| LastLoginAt | datetime2 | | |

#### PRODUCT
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| Slug | nvarchar(200) | UNIQUE, NOT NULL | URL SEO |
| Name | nvarchar(300) | NOT NULL | |
| Description | nvarchar(MAX) | | HTML |
| CategoryId | uniqueidentifier | FK | |
| BasePrice | decimal(18,2) | NOT NULL | |
| IsActive | bit | DEFAULT 1 | |
| IsDeleted | bit | DEFAULT 0 | BR-16 |
| SeoTitle | nvarchar(200) | | |
| SeoDescription | nvarchar(500) | | |
| CreatedAt | datetime2 | | |
| UpdatedAt | datetime2 | | |

**Index**: `(Slug)`, `(CategoryId, IsActive, IsDeleted)`, `(Name)` full-text

#### PRODUCT_VARIANT
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| ProductId | uniqueidentifier | FK, NOT NULL | |
| SKU | nvarchar(100) | UNIQUE, NOT NULL | |
| AttributesJson | nvarchar(MAX) | | `{color:"red",size:"M"}` |
| Price | decimal(18,2) | NOT NULL | Override BasePrice |
| ComparePrice | decimal(18,2) | | Giá gạch (sale) |
| ImageUrl | nvarchar(500) | | |
| Weight | decimal(10,2) | | Tính shipping |
| IsActive | bit | DEFAULT 1 | |

#### INVENTORY
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| VariantId | uniqueidentifier | PK, FK | 1-1 với Variant |
| OnHand | int | NOT NULL, DEFAULT 0 | Tồn thực tế |
| Reserved | int | NOT NULL, DEFAULT 0 | Đang giữ chỗ |
| RowVersion | rowversion | | Optimistic concurrency |
| LastUpdatedAt | datetime2 | | |

**Available** (computed): `OnHand - Reserved`

#### CART
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| UserId | uniqueidentifier | FK, NULLABLE | Member |
| SessionId | nvarchar(200) | NULLABLE | Guest |
| CouponId | uniqueidentifier | FK, NULLABLE | |
| ExpiresAt | datetime2 | | Cleanup |
| CreatedAt | datetime2 | | |
| UpdatedAt | datetime2 | | |

**Constraint**: `UserId IS NOT NULL OR SessionId IS NOT NULL`

#### CART_ITEM
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| CartId | uniqueidentifier | FK | |
| VariantId | uniqueidentifier | FK | |
| Quantity | int | NOT NULL, CHECK > 0 | |
| AddedAt | datetime2 | | |

#### ORDER
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| OrderNumber | varchar(20) | UNIQUE, NOT NULL | VD: `ORD20260602-0001` |
| UserId | uniqueidentifier | FK | |
| Status | varchar(20) | NOT NULL | Pending/Confirmed/... |
| Subtotal | decimal(18,2) | NOT NULL | |
| ShippingFee | decimal(18,2) | NOT NULL | |
| Discount | decimal(18,2) | NOT NULL, DEFAULT 0 | |
| Total | decimal(18,2) | NOT NULL | |
| CouponCode | nvarchar(50) | | Snapshot |
| ShippingAddressJson | nvarchar(MAX) | NOT NULL | Snapshot |
| Note | nvarchar(500) | | Customer note |
| PaymentMethod | varchar(20) | NOT NULL | COD/VNPay/MoMo |
| ConfirmedAt | datetime2 | | |
| ShippedAt | datetime2 | | |
| DeliveredAt | datetime2 | | |
| CompletedAt | datetime2 | | |
| CancelledAt | datetime2 | | |
| CancelReason | nvarchar(500) | | |
| RowVersion | rowversion | | |
| CreatedAt | datetime2 | | |

**Index**: `(UserId, CreatedAt DESC)`, `(Status, CreatedAt)`, `(OrderNumber)` unique

#### ORDER_ITEM
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| OrderId | uniqueidentifier | FK | |
| VariantId | uniqueidentifier | FK | Reference (soft) |
| ProductNameSnapshot | nvarchar(300) | NOT NULL | **BR-05** |
| VariantSkuSnapshot | nvarchar(100) | NOT NULL | |
| VariantAttributesSnapshot | nvarchar(MAX) | | JSON |
| UnitPriceSnapshot | decimal(18,2) | NOT NULL | **BR-04** |
| Quantity | int | NOT NULL | |
| LineTotal | decimal(18,2) | NOT NULL | |

#### PAYMENT
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| OrderId | uniqueidentifier | FK | |
| Method | varchar(20) | NOT NULL | |
| Amount | decimal(18,2) | NOT NULL | |
| Status | varchar(20) | NOT NULL | Pending/Success/Failed |
| TxnRef | nvarchar(100) | UNIQUE | **BR-13** idempotency key |
| GatewayResponseJson | nvarchar(MAX) | | Raw response |
| PaidAt | datetime2 | | |
| CreatedAt | datetime2 | | |

#### SHIPMENT
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| OrderId | uniqueidentifier | FK | |
| Provider | varchar(20) | NOT NULL | GHTK/GHN |
| TrackingNumber | nvarchar(100) | | |
| Status | varchar(20) | NOT NULL | |
| ShippedAt | datetime2 | | |
| DeliveredAt | datetime2 | | |
| CostFee | decimal(18,2) | | Fee thực tế |

#### COUPON
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| Code | nvarchar(50) | UNIQUE, NOT NULL | |
| Type | varchar(20) | NOT NULL | Percent/Fixed |
| Value | decimal(18,2) | NOT NULL | |
| MinOrderAmount | decimal(18,2) | DEFAULT 0 | |
| MaxDiscountAmount | decimal(18,2) | | Cap với Percent |
| MaxUsage | int | | NULL = unlimited |
| MaxUsagePerUser | int | DEFAULT 1 | |
| UsedCount | int | DEFAULT 0 | |
| StartAt | datetime2 | NOT NULL | |
| EndAt | datetime2 | NOT NULL | |
| IsActive | bit | DEFAULT 1 | |

#### REVIEW
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| ProductId | uniqueidentifier | FK | |
| UserId | uniqueidentifier | FK | |
| OrderItemId | uniqueidentifier | FK, UNIQUE | **BR-11** |
| Rating | tinyint | CHECK 1-5 | |
| Comment | nvarchar(2000) | | |
| IsApproved | bit | DEFAULT 0 | Moderation |
| CreatedAt | datetime2 | | |

#### ORDER_AUDIT_LOG
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| OrderId | uniqueidentifier | FK | |
| FromStatus | varchar(20) | | |
| ToStatus | varchar(20) | NOT NULL | |
| ChangedBy | uniqueidentifier | FK User | NULL = system |
| Reason | nvarchar(500) | | |
| ChangedAt | datetime2 | NOT NULL | |

#### OUTBOX_MESSAGE (cho Outbox Pattern - stretch)
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| EventType | nvarchar(200) | NOT NULL | `OrderCreatedEvent` |
| PayloadJson | nvarchar(MAX) | NOT NULL | |
| Status | varchar(20) | NOT NULL | Pending/Sent/Failed |
| Attempts | int | DEFAULT 0 | |
| LastAttemptAt | datetime2 | | |
| SentAt | datetime2 | | |
| CreatedAt | datetime2 | | |

---

## 11. PHẠM VI CHỨC NĂNG

### 11.1 MVP (Tuần 1–4) — Phải có

#### Customer side
- [x] Đăng ký + verify email
- [x] Đăng nhập + quên mật khẩu (gửi token reset)
- [x] Browse danh mục (tree)
- [x] List sản phẩm theo category, pagination, sort cơ bản
- [x] Search keyword (LIKE — chưa cần Elasticsearch)
- [x] Xem chi tiết sản phẩm + chọn variant
- [x] Giỏ hàng (Guest session + Member persist)
- [x] Checkout 4 step (address → shipping → payment → confirm)
- [x] COD + VNPay sandbox
- [x] Order history + chi tiết
- [x] Hủy đơn (trong điều kiện)
- [x] Email confirmation cho các status

#### Admin side
- [x] Login admin
- [x] CRUD Product (kèm variant, ảnh upload local)
- [x] CRUD Category (tree)
- [x] List + filter đơn hàng
- [x] Chuyển status đơn (manual)
- [x] CRUD User (chỉ change role + active/inactive)
- [x] Inventory adjust (nhập kho thủ công)

### 11.2 Phase 2 (Tuần 5–6) — Nice to have

- [ ] Wishlist
- [ ] Review + Rating + moderation
- [ ] CRUD Coupon + apply
- [ ] MoMo payment
- [ ] Tích hợp GHTK/GHN sandbox shipping
- [ ] Return request flow đầy đủ
- [ ] Filter nâng cao (price range, attribute, brand)
- [ ] Sort: bestseller, newest, rating
- [ ] Báo cáo doanh thu admin (chart + table)
- [ ] Forgot password / Reset password
- [ ] Profile + Address CRUD

### 11.3 Stretch (Tuần 7–8) — Wow factor

> Chọn **2–3 item** từ list dưới, không cố làm hết.

- [ ] **Real-time inventory với SignalR** — show "đã hết" khi user khác mua xong
- [ ] **Live admin dashboard** — Blazor + SignalR, hiển thị order count, revenue today
- [ ] **Outbox pattern** — reliable event publishing
- [ ] **Saga pattern** cho checkout flow
- [ ] **Redis distributed lock** cho stock reservation
- [ ] **Background jobs với Hangfire** (release reserved, send email, auto-complete)
- [ ] **AI semantic search** — embedding với OpenAI / local model
- [ ] **PWA** — offline cart, installable
- [ ] **Recommendation "Bạn có thể thích"** — collaborative filtering đơn giản
- [ ] **Flash sale countdown + queue**
- [ ] **Loyalty point system**
- [ ] **Multi-image gallery với zoom**
- [ ] **Live chat support** (SignalR)
- [ ] **Email template với MJML**
- [ ] **OpenTelemetry tracing** — observability

---

## 12. TWIST ĐỂ KHÁC BIỆT PORTFOLIO

### 12.1 Vấn đề: E-commerce là project nhàm nhất

Theo phân tích thị trường, **80% CV junior .NET có project e-commerce** (shop bán hàng, food order, booking…). Nếu bạn làm "shop chung chung", CV sẽ bị lẫn vào đám đông.

### 12.2 Combo recommended cho PORTFOLIO IMPACT cao nhất

#### 🏆 Combo 1: **Specialty Coffee Shop** (recommended)
- **Niche**: Cafe đặc sản (đậm chất story-telling)
- **Twist 1**: Real-time inventory + SignalR
- **Twist 2**: Outbox pattern + background job
- **Twist 3**: Live admin dashboard (Blazor + SignalR)

**Tại sao mạnh:**
- Niche cụ thể → recruiter biết bạn nghĩ sâu về domain
- Real-time inventory = use case business thật (không phải tech show-off vô nghĩa)
- Outbox pattern = senior thinking (reliability)
- Live dashboard = visible "wow" trong demo

**War stories sẵn:**
1. *"Em handle race condition khi 2 user cùng mua món còn 1 cái bằng Redis distributed lock kết hợp Reserved column…"*
2. *"Em dùng Outbox pattern để guarantee delivery email + notify staff sau khi payment success, vì SMTP có thể fail tạm thời…"*
3. *"SignalR push inventory update khi stock thay đổi — UX cải thiện vì user không phải reload mới biết đã hết hàng…"*

#### 🥈 Combo 2: **Handmade Marketplace (single vendor)**
- **Niche**: Đồ handmade với custom order
- **Twist 1**: Custom order flow với approval
- **Twist 2**: Saga pattern cho multi-step checkout
- **Twist 3**: Image processing (resize, watermark)

#### 🥉 Combo 3: **Tech-heavy minimal niche**
- **Niche**: Cafe / sách cũ
- **Twist 1**: AI semantic search (RAG)
- **Twist 2**: Event sourcing cho Order
- **Twist 3**: PWA + offline cart

**Lưu ý:** Combo 3 risky cho 8 tuần — AI integration + event sourcing là rabbit hole.

### 12.3 Anti-pattern phải tránh

| Anti-pattern | Hậu quả |
|--------------|---------|
| ❌ "General store" bán mọi mặt hàng | Nhạt nhẽo, không có business depth |
| ❌ Copy y nguyên Shopee/Tiki UI | Generic, không show được kỹ năng UX |
| ❌ Skip edge cases (race, idempotency) | Recruiter senior catch ngay |
| ❌ Không có demo data hay | Demo trống → recruiter không thấy gì |
| ❌ Không có README → không deploy | Recruiter không xem được gì |
| ❌ Stretch quá nhiều, MVP chưa xong | Project nửa vời = CV negative |

---

## 13. KIẾN TRÚC ĐỀ XUẤT

### 13.1 Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION                            │
│                                                              │
│   ┌─────────────────┐   ┌─────────────────┐                 │
│   │ Razor Pages     │   │ Blazor Server   │                 │
│   │ (Customer Site) │   │ (Admin Panel)   │                 │
│   └─────────────────┘   └─────────────────┘                 │
│                                                              │
│   ┌─────────────────────────────────────────┐               │
│   │       ASP.NET Core Minimal API           │               │
│   │   (Optional REST API cho mobile sau)    │               │
│   └─────────────────────────────────────────┘               │
└──────────────────────────┬───────────────────────────────────┘
                           │
┌──────────────────────────┴───────────────────────────────────┐
│                      APPLICATION LAYER                        │
│                                                               │
│   MediatR (CQRS):                                            │
│   ┌────────────────┐  ┌────────────────┐                     │
│   │ Commands       │  │ Queries        │                     │
│   │ ────────────── │  │ ────────────── │                     │
│   │ - AddToCart    │  │ - GetCart      │                     │
│   │ - Checkout     │  │ - GetProducts  │                     │
│   │ - PayOrder     │  │ - GetOrders    │                     │
│   │ - CancelOrder  │  │ - GetReport    │                     │
│   └────────────────┘  └────────────────┘                     │
│                                                               │
│   Pipeline Behaviors:                                        │
│   FluentValidation → Logging → Transaction → Caching         │
│                                                               │
│   Domain Event Handlers:                                     │
│   - OrderCreated → SendEmail, NotifyStaff, UpdateDashboard   │
│   - PaymentSucceeded → ConfirmOrder, ReleaseReserved         │
│   - StockBelowThreshold → AlertAdmin                         │
└──────────────────────────┬───────────────────────────────────┘
                           │
┌──────────────────────────┴───────────────────────────────────┐
│                       DOMAIN LAYER                            │
│                Pure C# — không framework                      │
│                                                               │
│   Aggregates:                                                │
│   ┌─────────────────┐  ┌─────────────────┐                   │
│   │ Order (root)    │  │ Product (root)  │                   │
│   │ + OrderItems    │  │ + Variants      │                   │
│   │ + Payments      │  │ + Inventory     │                   │
│   │ + AuditLogs     │  │                 │                   │
│   └─────────────────┘  └─────────────────┘                   │
│                                                               │
│   ┌─────────────────┐  ┌─────────────────┐                   │
│   │ Cart (root)     │  │ User (root)     │                   │
│   │ + CartItems     │  │ + Addresses     │                   │
│   └─────────────────┘  └─────────────────┘                   │
│                                                               │
│   Value Objects:                                             │
│   - Money, Address, OrderNumber, SKU                         │
│                                                               │
│   Domain Events:                                             │
│   - OrderCreatedEvent, OrderConfirmedEvent                   │
│   - PaymentSucceededEvent, StockReservedEvent                │
│                                                               │
│   Domain Services:                                           │
│   - OrderPricingService (compute total with coupon)          │
│   - ShippingFeeCalculator                                    │
└──────────────────────────┬───────────────────────────────────┘
                           │
┌──────────────────────────┴───────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                         │
│                                                               │
│   ┌──────────────┐ ┌──────────────┐ ┌──────────────┐         │
│   │ EF Core 8    │ │ Redis        │ │ Payment SDK  │         │
│   │ + SQL Server │ │ Cache+Lock   │ │ VNPay/MoMo   │         │
│   └──────────────┘ └──────────────┘ └──────────────┘         │
│                                                               │
│   ┌──────────────┐ ┌──────────────┐ ┌──────────────┐         │
│   │ Hangfire     │ │ SignalR Hub  │ │ Email Service│         │
│   │ Background   │ │ Realtime     │ │ MailKit+SMTP │         │
│   └──────────────┘ └──────────────┘ └──────────────┘         │
│                                                               │
│   ┌──────────────┐ ┌──────────────┐ ┌──────────────┐         │
│   │ Outbox       │ │ File Storage │ │ Shipping API │         │
│   │ Worker       │ │ Local/Azure  │ │ GHTK/GHN     │         │
│   └──────────────┘ └──────────────┘ └──────────────┘         │
└───────────────────────────────────────────────────────────────┘
```

### 13.2 Project Structure

```
ECommerce/
├── src/
│   ├── ECommerce.Domain/
│   │   ├── Common/                  # BaseEntity, ValueObject, AggregateRoot
│   │   ├── Entities/                # User, Product, Order, Cart, ...
│   │   ├── ValueObjects/            # Money, Address, OrderNumber, SKU
│   │   ├── Events/                  # OrderCreatedEvent, ...
│   │   ├── Enums/                   # OrderStatus, PaymentMethod, ...
│   │   ├── Exceptions/              # DomainException
│   │   └── Services/                # OrderPricingService, ...
│   │
│   ├── ECommerce.Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/           # Validation, Logging, Transaction
│   │   │   ├── Interfaces/          # IAppDbContext, IEmailService, ...
│   │   │   └── Models/              # PagedResult, Result<T>
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Register/
│   │   │   │   ├── Login/
│   │   │   │   └── VerifyEmail/
│   │   │   ├── Catalog/
│   │   │   │   ├── GetProductList/
│   │   │   │   ├── GetProductDetail/
│   │   │   │   └── SearchProducts/
│   │   │   ├── Cart/
│   │   │   │   ├── AddToCart/
│   │   │   │   ├── UpdateCartItem/
│   │   │   │   └── GetCart/
│   │   │   ├── Checkout/
│   │   │   │   ├── StartCheckout/
│   │   │   │   ├── ConfirmOrder/
│   │   │   │   └── ProcessPaymentCallback/
│   │   │   ├── Orders/
│   │   │   │   ├── ListMyOrders/
│   │   │   │   ├── CancelOrder/
│   │   │   │   └── RequestReturn/
│   │   │   └── Admin/
│   │   │       ├── Products/        # CRUD
│   │   │       ├── Orders/          # Manage
│   │   │       └── Reports/
│   │   ├── DomainEventHandlers/     # Subscribe domain events
│   │   └── DependencyInjection.cs
│   │
│   ├── ECommerce.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/      # Fluent API mapping
│   │   │   └── Migrations/
│   │   ├── Identity/                # User auth
│   │   ├── Payment/
│   │   │   ├── VNPayService.cs
│   │   │   └── MoMoService.cs
│   │   ├── Shipping/
│   │   │   ├── GhtkService.cs
│   │   │   └── GhnService.cs
│   │   ├── Email/                   # MailKit
│   │   ├── Caching/                 # Redis
│   │   ├── BackgroundJobs/          # Hangfire
│   │   ├── Realtime/                # SignalR
│   │   ├── Outbox/                  # Outbox worker
│   │   ├── FileStorage/
│   │   └── DependencyInjection.cs
│   │
│   ├── ECommerce.Web/               # Razor Pages (Customer)
│   │   ├── Pages/
│   │   ├── wwwroot/
│   │   └── Program.cs
│   │
│   ├── ECommerce.Admin/             # Blazor Server (Admin)
│   │   ├── Pages/
│   │   ├── Components/
│   │   └── Program.cs
│   │
│   └── ECommerce.Api/               # Minimal API (optional)
│       ├── Endpoints/
│       └── Program.cs
│
├── tests/
│   ├── ECommerce.Domain.UnitTests/
│   ├── ECommerce.Application.UnitTests/
│   └── ECommerce.IntegrationTests/  # Testcontainers
│
├── docker/
│   ├── Dockerfile
│   ├── docker-compose.yml           # App + SQL Server + Redis + Seq
│   └── docker-compose.prod.yml
│
├── .github/workflows/
│   ├── ci.yml                       # Build + test
│   └── deploy.yml                   # Deploy Render/Azure
│
├── docs/
│   ├── architecture.md
│   ├── domain-glossary.md
│   ├── api.md
│   └── screenshots/
│
├── README.md
└── ECommerce.sln
```

### 13.3 Tech Stack chi tiết

| Layer | Technology | Lý do chọn |
|-------|-----------|------------|
| **Runtime** | .NET 8 LTS | Modern, hỗ trợ dài hạn |
| **Web** | ASP.NET Core 8 (Razor Pages + Blazor Server) | Full .NET, không cần học React |
| **API** | Minimal API | Modern style, performance tốt |
| **Architecture** | Clean Architecture + CQRS | Senior pattern |
| **Mediator** | MediatR | Standard cho CQRS .NET |
| **ORM** | EF Core 8 | First-class .NET |
| **DB Local** | SQL Server (LocalDB) hoặc SQLite | Dev nhanh |
| **DB Prod** | PostgreSQL (Render free) | Free tier tốt |
| **Cache** | Redis (StackExchange.Redis) | Industry standard |
| **Realtime** | SignalR | First-class .NET |
| **Background** | Hangfire | Easy + dashboard built-in |
| **Logging** | Serilog → Seq (dev) / Console (prod) | Structured logging |
| **Validation** | FluentValidation | Standard, declarative |
| **Mapping** | Mapster | Nhẹ hơn AutoMapper |
| **Auth** | ASP.NET Core Identity + JWT | Mature, official |
| **Testing** | xUnit + FluentAssertions + Moq | Standard .NET |
| **Integration Test** | Testcontainers | Real DB trong test |
| **Coverage** | Coverlet + Codecov | Free badges |
| **Container** | Docker multi-stage | Standard |
| **CI/CD** | GitHub Actions | Free cho public repo |
| **Deploy** | Render.com (Docker native, free) | Tốt nhất cho .NET free |
| **Email** | MailKit + Mailtrap (dev) / Gmail SMTP (prod) | Reliable |
| **Payment** | VNPay SDK + MoMo SDK | Sandbox VN |
| **Shipping** | GHTK/GHN HTTP client (sandbox) | API thật |

---

## 14. ROADMAP 8 TUẦN

### Nguyên tắc

> **Quy tắc cốt lõi:**
> - **MVP first, polish later** — đừng cố làm Outbox/Saga ở tuần 2 khi chưa có Order flow chạy.
> - **Demo được = ưu tiên 1** — code đẹp nhưng không demo được = thua.
> - **Buffer tuần 8** — luôn dành 1 tuần xử lý overrun + viết doc.

### 14.1 Roadmap chi tiết

| Tuần | Theme | Goals | Deliverable kiểm tra được |
|:---:|------|-------|---------------------------|
| **1** | Foundation | Solution structure, DB design, seed data | Solution Clean Arch build pass · Migration tạo được DB · Seed 20 product · README skeleton · GitHub Actions CI xanh |
| **2** | Catalog + Cart | Browse + Search + Cart (Guest + Member) | Customer xem được product list/detail · Add cart hoạt động · Cart persist sau login |
| **3** | Checkout + COD | Checkout 4-step flow + Order với COD | Customer đặt đơn COD thành công · Stock trừ đúng · Email confirm gửi · Admin thấy đơn mới |
| **4** | Online Payment | VNPay integration + Payment IPN | Đặt đơn → VNPay sandbox → callback → order Confirmed · Idempotency check pass · Test concurrent payment OK |
| **5** | Order Management + Admin | State machine, audit log, admin pages | Admin chuyển status đơn end-to-end · Audit log đầy đủ · Cancel + restore stock · Báo cáo cơ bản |
| **6** | **Twist #1** (Real-time inventory) | SignalR + Hangfire background | Mở 2 tab → mua hết → tab kia thấy "đã hết" · Reserve auto-release sau 15p · Email job qua Hangfire |
| **7** | **Twist #2** (Outbox + Dashboard) | Outbox pattern + Live admin dashboard | Order event publish reliable · Live dashboard hiển thị realtime order count + revenue today |
| **8** | **BUFFER + Polish** | Deploy + Docs + Demo | Live URL trên Render · README đẹp + GIF demo · Architecture diagram · 1 blog post Dev.to · CV update |

### 14.2 Milestone checkpoints

| Tuần | Milestone | Nếu trễ → cắt gì |
|:---:|-----------|-------------------|
| 2 | Browse + Cart hoạt động | Skip search nâng cao, chỉ LIKE |
| 4 | End-to-end order với cả COD và VNPay | Skip MoMo, chỉ VNPay |
| 6 | MVP đầy đủ deploy được | Skip 1 twist, ưu tiên deploy |
| 8 | Demo URL live + Docs đẹp | Skip blog post, chỉ README |

### 14.3 Daily/Weekly habit

- **Mỗi tuần**: commit ≥ 10 lần với Conventional Commits format
- **Mỗi tuần**: viết 1 ADR (Architecture Decision Record) trong `docs/adr/`
- **Mỗi 2 tuần**: review code, refactor 1 chỗ
- **Mỗi tuần**: update README với progress

---

## 15. TIÊU CHÍ NGHIỆM THU

### 15.1 Code quality

- [ ] Build pass với `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
- [ ] `<Nullable>enable</Nullable>` toàn solution
- [ ] Không có TODO/FIXME còn sót trong main branch
- [ ] Naming theo convention .NET (PascalCase cho public, _camelCase cho private field)
- [ ] Không có method > 50 dòng (trừ migration generated)
- [ ] Không có class > 300 dòng
- [ ] Không có magic number — dùng constant hoặc config

### 15.2 Testing

- [ ] Unit test cho Domain layer ≥ 80% coverage
- [ ] Unit test cho Application layer ≥ 60% coverage
- [ ] Integration test cho ít nhất 3 flow chính (checkout, payment callback, return)
- [ ] Test pass trong CI

### 15.3 Documentation

- [ ] README đầy đủ: tech stack, setup, run, demo account, screenshots
- [ ] Architecture diagram (Excalidraw export)
- [ ] Demo GIF ≥ 1 cái (30s)
- [ ] ADR cho 3 quyết định quan trọng (chọn Clean Arch, chọn Outbox, chọn DB...)
- [ ] API documentation (Swagger UI public)

### 15.4 Deploy

- [ ] Live URL chạy ổn định ≥ 7 ngày liên tục
- [ ] HTTPS valid (Let's Encrypt qua Render)
- [ ] UptimeRobot monitoring xanh
- [ ] Seed data đủ để demo (20+ product, 5+ order ở các status khác nhau)
- [ ] Demo account: 1 customer, 1 staff, 1 admin

### 15.5 Portfolio readiness

- [ ] CV bullet point cụ thể, có metric
- [ ] LinkedIn post about project
- [ ] 1 blog post technical (về quyết định architecture)
- [ ] GitHub repo: stars/badges/topics đầy đủ
- [ ] 3 war stories chuẩn bị sẵn cho phỏng vấn

---

---

## 16. 🤖 AI CHATBOT + REAL-TIME CUSTOMER SUPPORT

> **Tại sao thêm phần này:** AI integration + Real-time chat là **2 tech trend hot nhất 2026**. Kết hợp chúng trong project portfolio sẽ tạo **wow factor cực mạnh** — đa số CV junior không có. Đây là feature **"đáng giá nhất"** trong tất cả twist của doc này.

### 16.1 Tổng quan giải pháp

**Concept**: Hybrid AI-Human Customer Support
- **AI bot** là **first responder** — xử lý 60–80% query thông thường
- **Human agent** xử lý khi AI escalate hoặc khách yêu cầu
- **Smooth handoff** — AI bot chuyển context cho human, không bắt khách lặp lại

```
┌─────────────────────────────────────────────────────────────┐
│            HYBRID AI-HUMAN SUPPORT FLOW                      │
└─────────────────────────────────────────────────────────────┘

  Customer ──→ Open chat widget
         │
         ↓
   ┌──────────────┐
   │ AI Bot greet │  "Chào anh/chị! Em là Coffee AI Assistant.
   └──────┬───────┘   Em có thể giúp gì ạ?"
          │
          ↓ user typing...
   ┌──────────────────────────────────┐
   │   AI processes message            │
   │   - Intent classification         │
   │   - RAG retrieve context          │
   │   - Tool calling (lookup order…)  │
   │   - Generate response (streaming) │
   └──────┬───────────────────────────┘
          │
          ├──→ Auto-handle (FAQ, order status, product info)
          │
          └──→ Escalate triggers:
                ├── User explicit: "gặp nhân viên"
                ├── AI confidence < 0.7
                ├── Sensitive topic (complaint, refund)
                ├── Repeated misunderstanding (3+ lần)
                └── After hours: collect info → assign next day
                       │
                       ↓
                ┌──────────────────┐
                │ Routing to Agent │  ← Tính rule routing:
                └──────┬───────────┘    - Round-robin trong shift
                       │                - Skill-based (refund vs technical)
                       ↓                - Load balance
                ┌──────────────────┐
                │ Support Agent    │
                │ - Sees full      │
                │   AI conversation│
                │ - Takes over     │
                │ - AI suggests    │
                │   replies (RAG)  │
                └──────────────────┘
```

### 16.2 Capabilities của AI Bot

| Capability | Mô tả | Ví dụ query | Implementation |
|------------|-------|-------------|----------------|
| **1. FAQ Auto-answer** | Trả lời câu hỏi thường gặp về shop | "Phí ship bao nhiêu?", "Có miễn ship không?" | RAG với FAQ database |
| **2. Order Status Lookup** | Tra cứu trạng thái đơn | "Đơn ORD20260602-0001 đến đâu rồi?" | Tool calling → DB lookup |
| **3. Product Recommendation** | Gợi ý sản phẩm theo nhu cầu | "Tôi cần cà phê đậm cho buổi sáng" | Semantic search + LLM ranking |
| **4. Product Q&A** | Trả lời câu hỏi về product | "Cà phê Arabica này có chua không?" | RAG với product description |
| **5. Shipping Estimation** | Ước lượng thời gian giao | "Ship Hà Nội mất bao lâu?" | Tool calling → shipping rules |
| **6. Coupon Suggestion** | Gợi ý mã giảm giá | "Có voucher gì không?" | Tool calling → active coupons |
| **7. Cart Assistance** | Giúp thêm sản phẩm vào cart | "Add 2 gói Arabica vào giỏ giúp tôi" | Tool calling → cart API |
| **8. Complaint Detection** | Phát hiện khiếu nại → escalate | "Tao bực quá, đơn hàng tệ" | Sentiment + intent classifier |

### 16.3 Actors mở rộng

```
┌──────────────────────────────────────────────────┐
│              SUPPORT SYSTEM ACTORS                 │
│                                                    │
│   👤 Customer (đã có)                              │
│   👤 Support Agent (NEW — extends Staff)           │
│   🤖 AI Bot (NEW — system actor)                   │
│   👤 Support Lead (NEW — manage agents)            │
└──────────────────────────────────────────────────┘
```

**Support Agent** = Staff với thêm permission:
- View/handle chat conversations
- Quick response templates
- AI assist suggestions
- Mark conversation as resolved

**Support Lead**:
- Tất cả Agent permission
- View conversation analytics (avg response time, satisfaction)
- Assign chat manually
- Manage FAQ database for AI

### 16.4 Use Cases mới

#### UC-40: Customer mở chat
- **Actor**: Customer (Guest hoặc Member)
- **Tiền điều kiện**: Không
- **Flow**:
  1. Click widget chat ở góc phải dưới
  2. Nhập tên + email (nếu guest)
  3. AI bot greet message
  4. Conversation bắt đầu, status = AI_HANDLING

#### UC-41: AI trả lời tự động
- **Actor**: AI Bot
- **Mô tả**: Xử lý message của customer
- **Flow**:
  1. Receive message qua SignalR
  2. Classify intent (FAQ / Order / Recommend / Complaint / Other)
  3. RAG retrieve relevant context (FAQ + product + policy)
  4. Tool calling nếu cần (lookup order, search product)
  5. Generate response qua LLM (streaming)
  6. Send response qua SignalR
  7. Update conversation history
  8. Check escalation triggers (BR-30) → handoff nếu cần

#### UC-42: Handoff sang human agent
- **Actor**: System
- **Mô tả**: Chuyển conversation từ AI sang human
- **Triggers**: BR-30 (xem dưới)
- **Flow**:
  1. AI generate handoff message: "Em chuyển anh/chị sang nhân viên hỗ trợ"
  2. Conversation status = WAITING_AGENT
  3. Push notification vào agent queue
  4. Routing logic chọn agent
  5. Agent claim conversation → status = AGENT_HANDLING
  6. Agent thấy full AI conversation history

#### UC-43: Agent xử lý chat
- **Actor**: Support Agent
- **Mô tả**: Reply customer trong real-time
- **Flow**:
  1. Agent nhận notification có chat mới
  2. Click claim → vào conversation
  3. Đọc context AI đã chat
  4. Reply customer
  5. (Optional) Sử dụng AI suggestion: AI đọc context + gợi ý 3 câu reply
  6. (Optional) Sử dụng quick template
  7. Mark resolved khi xong

#### UC-44: Agent dùng AI suggest reply
- **Actor**: Support Agent
- **Flow**:
  1. Agent thấy 3 gợi ý từ AI (RAG-based)
  2. Click chọn 1 gợi ý → edit nếu cần → send

#### UC-45: Customer rating conversation
- **Actor**: Customer
- **Flow**:
  1. Sau khi conversation resolved → hiện popup rating 1–5 sao
  2. Optional comment
  3. Submit → lưu để training data

### 16.5 Business Rules mở rộng

| ID | Rule | Mô tả |
|----|------|-------|
| **BR-30** | Escalation triggers | AI handoff sang human khi: (a) user explicit "gặp nhân viên", (b) AI confidence < 0.7, (c) sensitive intent (complaint/refund/legal), (d) misunderstand 3 lần, (e) order >5M VND |
| **BR-31** | AI response time | AI phải bắt đầu stream response trong 3 giây từ khi nhận message |
| **BR-32** | Human response SLA | Human agent phải reply trong 5 phút khi claim conversation |
| **BR-33** | Conversation timeout | Inactive 30 phút → auto close + email summary |
| **BR-34** | After-hours queue | Ngoài giờ làm việc (sau 22h): AI greet → collect info → assign agent next day |
| **BR-35** | Rate limit chat | Mỗi customer max 10 conversation/ngày (chống spam) |
| **BR-36** | AI cost guardrail | Tổng token AI usage/ngày có cap → nếu vượt → fallback FAQ matching only |
| **BR-37** | Privacy filter | AI không được lưu hoặc log: password, payment card, OTP, CCCD |
| **BR-38** | Hallucination guard | AI chỉ được trả lời dựa trên RAG context — nếu không có context relevant → "em không chắc, cho em chuyển nhân viên" |
| **BR-39** | Order action confirmation | AI thực hiện action ảnh hưởng order (cancel, change) phải có **confirmation step** từ user |
| **BR-40** | Conversation retention | Lưu conversation 90 ngày (analytics + training), sau đó anonymize |

### 16.6 Edge cases

| Edge case | Hậu quả | Giải pháp |
|-----------|---------|-----------|
| AI API timeout | User thấy "loading" mãi | Timeout 10s → fallback FAQ matching → escalate |
| AI hallucinate sản phẩm không có | User confused, mất trust | BR-38 + tool calling verify trước khi suggest |
| User spam 100 message/phút | DOS hệ thống + tốn AI cost | Rate limit + captcha sau 5 message/30s |
| User trêu AI để xuất prompt | Prompt injection | System prompt rõ ràng + filter input |
| Agent đang chat thì offline | Customer chờ vô tận | Auto re-assign sau 2 phút không response |
| 2 agent cùng claim 1 conversation | Race condition | Optimistic lock với RowVersion |
| AI tốn quá nhiều token vì user spam dài | Cost blow up | Truncate input ≥ 2000 chars + warning user |
| User upload ảnh để hỏi AI | Cần vision model | MVP: reject ảnh, v2: tích hợp Vision API |
| AI suggest sản phẩm hết stock | UX kém | Filter inventory trong retrieval step |
| Customer hỏi tiếng Anh | AI tiếng Việt fail | Detect language + multi-lingual prompt |

### 16.7 Conversation Flow chi tiết

```
[Customer]            [SignalR]          [ChatService]        [AI Service]
    │                      │                    │                  │
    │ 1. Open chat widget  │                    │                  │
    │─────────────────────→│                    │                  │
    │                      │ 2. Connect Hub     │                  │
    │                      │ ConnectionId       │                  │
    │                      │───────────────────→│                  │
    │                      │                    │                  │
    │                      │                    │ 3. Tạo Conv:     │
    │                      │                    │  - status=AI_    │
    │                      │                    │    HANDLING      │
    │                      │                    │  - customerId    │
    │                      │                    │  - sessionId     │
    │                      │                    │                  │
    │                      │ 4. AI greet msg    │                  │
    │←─────────────────────│←───────────────────│                  │
    │                      │                    │                  │
    │ 5. Send "đơn của tôi đến đâu"             │                  │
    │─────────────────────→│                    │                  │
    │                      │ Forward            │                  │
    │                      │───────────────────→│                  │
    │                      │                    │ 6. Save msg DB   │
    │                      │                    │ 7. Build context:│
    │                      │                    │  - history       │
    │                      │                    │  - user profile  │
    │                      │                    │  - recent orders │
    │                      │                    │                  │
    │                      │                    │ 8. Call AI ─────→│
    │                      │                    │                  │
    │                      │                    │                  │ 9. Classify:
    │                      │                    │                  │   intent =
    │                      │                    │                  │   "order_status"
    │                      │                    │                  │
    │                      │                    │                  │ 10. Tool call:
    │                      │                    │                  │   getOrderStatus
    │                      │                    │                  │   (userId)
    │                      │                    │←─────────────────│
    │                      │                    │                  │
    │                      │                    │ 11. Lookup DB    │
    │                      │                    │  → ORD-001       │
    │                      │                    │    Shipping      │
    │                      │                    │  → ORD-002       │
    │                      │                    │    Delivered     │
    │                      │                    │                  │
    │                      │                    │ 12. Return ─────→│
    │                      │                    │                  │
    │                      │                    │                  │ 13. Generate
    │                      │                    │                  │   response with
    │                      │                    │                  │   order data
    │                      │                    │                  │ (STREAMING)
    │                      │                    │←─────────────────│
    │                      │                    │ chunk 1          │
    │                      │←───────────────────│ chunk 2          │
    │← stream chunk 1 ─────│                    │ chunk 3...       │
    │← stream chunk 2 ─────│                    │                  │
    │← stream chunk N ─────│                    │                  │
    │                      │                    │                  │
    │                      │                    │ 14. Save full    │
    │                      │                    │   response DB    │
    │                      │                    │                  │
    │                      │                    │ 15. Check escalate?
    │                      │                    │   confidence=0.9 │
    │                      │                    │   sentiment=neutral
    │                      │                    │   → continue AI  │
```

### 16.8 Handoff Flow

```
[Customer] [AI Bot] [Chat Service] [Routing Engine] [Agent Pool]
    │           │           │              │              │
    │ "Tao bực quá │         │              │              │
    │  trả tiền   │         │              │              │
    │  cho tao!"  │         │              │              │
    │──────────────────────→│              │              │
    │           │           │              │              │
    │           │ AI process:              │              │
    │           │ - intent: complaint       │              │
    │           │ - sentiment: angry        │              │
    │           │ - BR-30 trigger (c)       │              │
    │           │                          │              │
    │           │ AI message:              │              │
    │           │ "Em rất tiếc về trải nghiệm của anh/chị.│
    │           │ Em chuyển sang nhân viên chăm sóc       │
    │           │ ngay ạ. Vui lòng đợi 1 phút..."         │
    │←──────────────────────│              │              │
    │           │           │              │              │
    │           │           │ Status =     │              │
    │           │           │ WAITING_AGENT│              │
    │           │           │              │              │
    │           │           │──────────────→ 1. Find agent:
    │           │           │              │  - online    │
    │           │           │              │  - skill=    │
    │           │           │              │    complaint │
    │           │           │              │  - load <max │
    │           │           │              │              │
    │           │           │              │ 2. Push notify
    │           │           │              │ "New chat to claim"
    │           │           │              │───────────────→
    │           │           │              │              │
    │           │           │              │ 3. Agent A   │
    │           │           │              │   claims     │
    │           │           │              │←─────────────│
    │           │           │              │              │
    │           │           │←─────────────│ Assigned to A│
    │           │           │              │              │
    │           │           │ Status = AGENT_HANDLING     │
    │           │           │ AgentId = A  │              │
    │           │           │              │              │
    │           │           │ Agent A      │              │
    │           │           │ joins chat   │              │
    │ Show: "Nguyễn Anh đã  │              │              │
    │  tham gia cuộc trò chuyện"           │              │
    │←──────────────────────│              │              │
    │           │           │              │              │
    │           │           │ Agent thấy:  │              │
    │           │           │ - Full AI    │              │
    │           │           │   conversation│             │
    │           │           │ - Customer   │              │
    │           │           │   info       │              │
    │           │           │ - Recent     │              │
    │           │           │   orders     │              │
    │           │           │ - AI suggested              │
    │           │           │   3 replies  │              │
    │           │           │              │              │
    │ "Em xin lỗi anh/chị,  │              │              │
    │  em check ngay đơn..." │              │              │
    │← chat continues ──────│              │              │
```

### 16.9 Data Model bổ sung

#### CHAT_CONVERSATION
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| CustomerId | uniqueidentifier | FK, NULLABLE | NULL nếu guest |
| GuestEmail | nvarchar(256) | | Nếu guest |
| GuestName | nvarchar(200) | | Nếu guest |
| Status | varchar(30) | NOT NULL | AI_HANDLING / WAITING_AGENT / AGENT_HANDLING / RESOLVED / CLOSED / TIMEOUT |
| AssignedAgentId | uniqueidentifier | FK, NULLABLE | |
| StartedAt | datetime2 | NOT NULL | |
| LastMessageAt | datetime2 | NOT NULL | Cho timeout |
| ResolvedAt | datetime2 | | |
| Rating | tinyint | NULLABLE, CHECK 1-5 | |
| RatingComment | nvarchar(1000) | | |
| TopicTag | varchar(50) | | order_status/complaint/product_info... |
| AiTokensUsed | int | DEFAULT 0 | Cost tracking |
| RowVersion | rowversion | | Optimistic concurrency cho claim |

#### CHAT_MESSAGE
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| ConversationId | uniqueidentifier | FK | |
| Sender | varchar(20) | NOT NULL | Customer / AI / Agent / System |
| SenderUserId | uniqueidentifier | NULLABLE | NULL khi Sender=AI hoặc System |
| Content | nvarchar(MAX) | NOT NULL | |
| ContentType | varchar(20) | DEFAULT 'text' | text/image/quick_reply/system |
| AiMetadataJson | nvarchar(MAX) | | intent, confidence, tools_used |
| CreatedAt | datetime2 | NOT NULL | |
| ReadAt | datetime2 | | |

#### AGENT_PROFILE
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| UserId | uniqueidentifier | PK, FK | 1-1 với User |
| Skills | nvarchar(500) | | CSV: complaint,refund,technical |
| Status | varchar(20) | NOT NULL | Online / Busy / Offline / OnBreak |
| MaxConcurrent | int | DEFAULT 3 | Max chat đồng thời |
| CurrentLoad | int | DEFAULT 0 | Số chat đang xử lý |
| LastActiveAt | datetime2 | | |

#### FAQ_ENTRY (cho RAG)
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| Question | nvarchar(500) | NOT NULL | |
| Answer | nvarchar(MAX) | NOT NULL | |
| Category | varchar(50) | | shipping/payment/product/policy |
| Embedding | vector(1536) | | OpenAI ada-002 embedding |
| UsageCount | int | DEFAULT 0 | |
| IsActive | bit | DEFAULT 1 | |
| LastUpdatedAt | datetime2 | | |

> 💡 Với SQL Server, vector column dùng `varbinary(MAX)` hoặc tích hợp **pgvector** với PostgreSQL. Hoặc dùng **Qdrant** external.

#### QUICK_REPLY_TEMPLATE
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| Title | nvarchar(200) | | "Xin lỗi vì sự bất tiện" |
| Content | nvarchar(MAX) | | Template với placeholder |
| Category | varchar(50) | | |
| UsageCount | int | DEFAULT 0 | |

#### AI_CONVERSATION_LOG (audit)
| Field | Type | Constraint | Note |
|-------|------|-----------|------|
| Id | uniqueidentifier | PK | |
| ConversationId | uniqueidentifier | FK | |
| MessageId | uniqueidentifier | FK | |
| Prompt | nvarchar(MAX) | | Full prompt sent to LLM |
| Response | nvarchar(MAX) | | Full LLM response |
| Model | varchar(50) | | gpt-4o-mini / claude-3-haiku |
| InputTokens | int | | |
| OutputTokens | int | | |
| LatencyMs | int | | |
| ToolsCalled | nvarchar(MAX) | | JSON array |
| CreatedAt | datetime2 | | |

### 16.10 State Machine — Conversation

```
                  ┌──────────────────┐
                  │  AI_HANDLING     │  (mặc định khi mở)
                  └────────┬─────────┘
                           │
              ┌────────────┼────────────┬─────────────┐
              ↓            ↓            ↓             ↓
        AI escalate   user request  resolved      timeout
              │            │            │             │
              ↓            ↓            ↓             ↓
       ┌──────────────┐   │       ┌──────────┐  ┌─────────┐
       │WAITING_AGENT │   │       │ RESOLVED │  │ TIMEOUT │
       └──────┬───────┘   │       └──────────┘  └─────────┘
              │           │
              │           ↓
              │      ┌──────────────┐
              │      │WAITING_AGENT │
              │      └──────┬───────┘
              │             │
              ↓             ↓
       ┌──────────────────────┐
       │  AGENT_HANDLING       │
       └──────────┬───────────┘
                  │
       ┌──────────┼──────────┐
       ↓          ↓          ↓
   resolved   agent       customer
              transfer    leaves
       │          │          │
       ↓          ↓          ↓
   ┌──────────┐  WAITING  ┌────────┐
   │ RESOLVED │  _AGENT    │ CLOSED │
   └──────────┘            └────────┘
```

| From | To | Trigger | Điều kiện |
|------|-----|---------|-----------|
| (none) | AI_HANDLING | Customer mở chat | — |
| AI_HANDLING | WAITING_AGENT | AI escalate (BR-30) | — |
| AI_HANDLING | RESOLVED | Customer click "resolved" | — |
| AI_HANDLING | TIMEOUT | 30p inactive | BR-33 |
| WAITING_AGENT | AGENT_HANDLING | Agent claim | Agent online + load < max |
| WAITING_AGENT | TIMEOUT | 10p không có agent | Auto re-assign |
| AGENT_HANDLING | RESOLVED | Agent mark resolved | — |
| AGENT_HANDLING | WAITING_AGENT | Agent transfer | Cần skill khác |
| AGENT_HANDLING | CLOSED | Customer close tab | — |

### 16.11 Architecture cho AI Chat

```
┌──────────────────────────────────────────────────────────────┐
│                      CHAT WIDGET (Customer)                    │
│                                                                │
│   ┌────────────────────────────────────────────────────┐      │
│   │  Razor Component / React widget                     │      │
│   │  ┌──────────────┐  ┌──────────────┐                │      │
│   │  │ Message List │  │ Input + Send │                │      │
│   │  └──────────────┘  └──────────────┘                │      │
│   └──────────────────────┬─────────────────────────────┘      │
│                          │ SignalR WebSocket                  │
└──────────────────────────┼────────────────────────────────────┘
                           │
┌──────────────────────────┴────────────────────────────────────┐
│                    SIGNALR HUB (ChatHub)                       │
│                                                                │
│   - OnConnectAsync → register connection                       │
│   - SendMessage(convId, content) → forward                     │
│   - AgentClaim(convId) → routing                               │
│   - StreamAiResponse(convId) → push chunks                     │
│   - TypingIndicator(convId)                                    │
└──────────────────────────┬────────────────────────────────────┘
                           │
┌──────────────────────────┴────────────────────────────────────┐
│                   CHAT APPLICATION SERVICE                      │
│                                                                │
│   - HandleIncomingMessage(convId, content)                     │
│       1. Save message DB                                       │
│       2. If status = AI_HANDLING → call AIOrchestrator         │
│       3. If status = AGENT_HANDLING → notify agent             │
│   - EscalateToHuman(convId, reason)                            │
│   - AssignAgent(convId) → call RoutingEngine                   │
└──────────────────────────┬────────────────────────────────────┘
                           │
            ┌──────────────┼──────────────┐
            ↓              ↓              ↓
┌─────────────────┐ ┌────────────────┐ ┌────────────────┐
│ AI Orchestrator │ │ Routing Engine │ │ Agent Manager  │
│                 │ │                │ │                │
│ - Intent class. │ │ - Find agent   │ │ - Status track │
│ - RAG retrieval │ │ - Round-robin  │ │ - Load balance │
│ - Tool calling  │ │ - Skill-based  │ │ - Push notify  │
│ - LLM call      │ │                │ │                │
│ - Stream resp.  │ │                │ │                │
└────────┬────────┘ └────────────────┘ └────────────────┘
         │
         ├──→ LLM Provider (OpenAI / Anthropic / Ollama)
         │
         ├──→ Vector DB (Qdrant / pgvector / Pinecone)
         │
         └──→ Tools:
              - getOrderStatus(userId)
              - searchProducts(query)
              - getCoupons()
              - addToCart(variantId, qty)
              - getShippingFee(addr)
```

### 16.12 AI Orchestrator chi tiết

#### Components
1. **Intent Classifier** — phân loại message:
   - `faq`, `order_status`, `product_inquiry`, `recommendation`, `complaint`, `greeting`, `other`
   - MVP: dùng prompt engineering với LLM nhỏ
   - V2: fine-tuned classifier riêng

2. **Context Builder** — gom data cho LLM:
   - Conversation history (last 10 messages)
   - User profile (name, recent orders, loyalty point)
   - Product context (nếu user đang xem trang sản phẩm)
   - Retrieved FAQ chunks (top 3 from vector search)

3. **Tool Registry** — danh sách function LLM có thể call:
   ```
   - getOrderStatus(orderId: string) → OrderStatus
   - searchProducts(query: string, filters: object) → Product[]
   - getActiveCoupons(userId: string) → Coupon[]
   - addToCart(variantId: string, quantity: number) → Cart
   - calculateShipping(address: string, items: array) → Money
   - escalateToHuman(reason: string) → void
   ```

4. **LLM Caller** — gọi API với:
   - Function calling enabled
   - Streaming response
   - Timeout 10s
   - Retry 2 lần với exponential backoff
   - Token usage tracking

5. **Safety Filter** — sanitize output:
   - Remove if contains PII (CCCD, card number…)
   - Refuse if attempt to extract system prompt
   - Detect hallucination — verify mentioned products exist

#### Pseudo-code flow

```csharp
public async Task<AiResponseStream> ProcessMessage(
    Guid convId,
    string userMessage,
    CancellationToken ct)
{
    // 1. Classify intent
    var intent = await _classifier.ClassifyAsync(userMessage, ct);

    // 2. Check escalation early
    if (intent.RequiresHuman) {
        await _chatService.EscalateAsync(convId, intent.Reason, ct);
        return AiResponseStream.HandoffMessage();
    }

    // 3. Build context
    var context = await _contextBuilder.BuildAsync(convId, userMessage, ct);
    // context = { history, userProfile, retrievedFaqs, productContext }

    // 4. Compose prompt
    var prompt = _promptBuilder.Compose(SYSTEM_PROMPT, context, userMessage);

    // 5. Call LLM with tools
    var llmStream = _llmClient.StreamAsync(
        prompt,
        tools: _toolRegistry.GetAll(),
        maxTokens: 1500,
        timeout: TimeSpan.FromSeconds(10),
        ct);

    // 6. Process stream + handle tool calls
    await foreach (var chunk in llmStream.WithCancellation(ct)) {
        if (chunk.IsToolCall) {
            var toolResult = await _toolExecutor.ExecuteAsync(chunk.ToolCall, ct);
            llmStream.SubmitToolResult(toolResult);
        } else {
            // 7. Safety filter
            var safe = _safetyFilter.Filter(chunk.Content);
            // 8. Push to SignalR
            await _hub.Clients.Group(convId.ToString())
                .SendAsync("StreamChunk", safe, ct);
        }
    }

    // 9. Save full response + log tokens
    await _conversationRepo.SaveAiMessage(convId, fullResponse, tokensUsed, ct);

    // 10. Post-process: check if should escalate based on confidence
    if (response.Confidence < 0.7) {
        await _chatService.EscalateAsync(convId, "low_confidence", ct);
    }
}
```

### 16.13 Tech Stack mở rộng

| Component | Technology | Free tier? | Lý do |
|-----------|-----------|:----------:|-------|
| **Real-time** | SignalR (.NET native) | ✅ | First-class .NET, không cần thêm service |
| **LLM Provider (chính)** | OpenAI `gpt-4o-mini` | ❌ ($0.15/1M input) | Cost rẻ, performance tốt VN |
| **LLM Provider (backup)** | Anthropic Claude Haiku | ❌ ($0.25/1M input) | Backup khi OpenAI down |
| **LLM Provider (free option)** | Ollama + Llama 3.2 3B/8B | ✅ | Self-host, dev/demo |
| **Embedding** | OpenAI `text-embedding-3-small` | ❌ ($0.02/1M) | Standard, cheap |
| **Vector DB** | **Qdrant** (self-host Docker) | ✅ | Lightweight, .NET client tốt |
| **Vector DB alt** | PostgreSQL + **pgvector** | ✅ | Tích hợp luôn với DB chính |
| **Caching** | Redis | ✅ | Cache common FAQ responses |
| **Background Job** | Hangfire | ✅ | Process AI logs, training data |
| **Cost Tracking** | Custom + Serilog | ✅ | Log token usage |

#### NuGet packages chính
```
Microsoft.AspNetCore.SignalR (built-in)
OpenAI (official) — gọi GPT
Anthropic.SDK — gọi Claude
Microsoft.SemanticKernel — orchestration framework (optional)
Qdrant.Client — vector DB client
Tiktoken — count tokens trước khi gọi (cost guard)
```

### 16.14 Cost Estimation (cho demo)

Giả định project portfolio chạy demo 1 tháng với traffic thật ít:

| Item | Volume/tháng | Cost (USD) | Cost (VND) |
|------|--------------|-----------|-----------|
| GPT-4o-mini input | 1M tokens | $0.15 | ~3,800đ |
| GPT-4o-mini output | 0.5M tokens | $0.30 | ~7,500đ |
| Embedding | 0.1M tokens | $0.002 | ~50đ |
| **Tổng AI cost demo** | — | **~$0.45** | **~12,000đ** |

→ **Cực rẻ** cho demo. Cao hơn khi production thật.

**Cost guard implementation**:
- Set hard cap $5/tháng cho demo
- Khi đạt 80% → switch sang Llama local (Ollama)
- Khi đạt 100% → AI disabled, chỉ FAQ matching

### 16.15 MVP vs Stretch cho AI Chat

#### MVP (Tuần 6 — nếu chọn AI Chat làm twist chính)
- [ ] SignalR Hub với ChatService
- [ ] Customer widget cơ bản (open/send/receive)
- [ ] AI bot trả lời FAQ cứng (rule-based first, không cần LLM)
- [ ] Conversation persist DB
- [ ] Agent console cơ bản (list + reply)
- [ ] Manual handoff (user click "gặp nhân viên")

#### Phase 2 (Tuần 7)
- [ ] LLM integration (OpenAI gpt-4o-mini) với streaming
- [ ] RAG: FAQ embedding + vector search (Qdrant)
- [ ] Tool calling: getOrderStatus, searchProducts
- [ ] Auto escalation (BR-30 implementation)
- [ ] AI suggest reply cho agent

#### Stretch (Tuần 8)
- [ ] Conversation rating + analytics
- [ ] Multi-skill routing
- [ ] Quick reply templates
- [ ] Conversation transfer giữa agents
- [ ] Voice input/output
- [ ] Multi-language support
- [ ] AI training data export

### 16.16 Roadmap cập nhật (chèn AI Chat vào tuần 6-8)

| Tuần | Theme | Goals | Deliverable |
|:---:|------|-------|-------------|
| **1-5** | (giữ nguyên) | Foundation → Order Management | MVP e-commerce hoàn chỉnh |
| **6** | **AI Chat Foundation** | SignalR Hub + Conversation + Rule-based bot + Agent console | Customer chat được, agent reply được, basic FAQ tự động |
| **7** | **AI Chat Intelligence** | LLM integration + RAG + Tool calling + Escalation | AI trả lời thông minh, tự gọi tool, auto handoff complaint |
| **8** | **Polish + Deploy** | Vector DB seed, rating system, deploy, demo data | Demo URL với 10+ FAQ seed, agent demo account, demo conversation |

### 16.17 Business Rules update

Bổ sung vào table chính (Section 6):

| ID | Rule | Mô tả |
|----|------|-------|
| **BR-30 → BR-40** | (đã list ở 16.5) | Xem mục 16.5 |

### 16.18 Wow factor — Khi recruiter mở demo

```
┌──────────────────────────────────────────────────────────────┐
│   Recruiter mở demo URL → Coffee Shop                         │
│                                                                │
│   Trong 60 giây đầu thấy:                                     │
│   ┌─────────────────────────────────────────────────────────┐│
│   │  1. Shop UI đẹp với product list                         ││
│   │  2. Bottom-right corner: chat widget với badge "AI"      ││
│   │  3. Click → AI greeting realtime                         ││
│   │  4. Type: "tôi cần cà phê đậm vị chocolate"              ││
│   │  5. AI streaming response 3 sản phẩm phù hợp + ảnh       ││
│   │  6. Click "Add to cart" → AI confirm                     ││
│   │  7. Type: "đơn ORD-001 đến đâu"                          ││
│   │  8. AI lookup → "Đơn của bạn đang ở Hà Nội, dự kiến..."  ││
│   │  9. Type: "tao bực, hoàn tiền"                           ││
│   │ 10. AI: "em chuyển sang nhân viên" → trong 30s có agent  ││
│   │     "Sarah" join (em tự đóng vai)                        ││
│   └─────────────────────────────────────────────────────────┘│
│                                                                │
│   → Recruiter sẽ nói: "Đây là project hay nhất tôi xem        │
│      tuần này. Schedule phỏng vấn ngay."                      │
└──────────────────────────────────────────────────────────────┘
```

### 16.19 War stories sẵn cho phỏng vấn

Sau khi build xong, bạn có sẵn 4-5 war story cực mạnh:

1. **Streaming response với SignalR**:
   *"Em handle LLM streaming bằng IAsyncEnumerable trong .NET 8 và push chunk qua SignalR. Challenge là backpressure khi user disconnect giữa chừng — em dùng CancellationToken propagate xuống tận OpenAI API call để stop generation, tiết kiệm token."*

2. **RAG implementation**:
   *"Em build RAG với Qdrant: chunking FAQ (500 token + overlap 50), embed với text-embedding-3-small, retrieve top-3 với cosine similarity. Có rerank step để filter relevance score < 0.7 — tránh hallucination."*

3. **Tool calling design**:
   *"Em define 6 tool theo OpenAI function calling spec. Tricky nhất là handle khi LLM call nested tool — em wrap trong Polly retry với circuit breaker. Tool execution có timeout 5s + fallback."*

4. **Auto escalation logic**:
   *"Em implement sentiment + intent classifier trước LLM gen — nếu detect complaint hoặc confidence < 0.7 → escalate. Save 30% cost vì không cần gọi full LLM cho case này."*

5. **Cost guardrail**:
   *"Em track token usage per conversation với Hangfire job aggregate daily. Khi reach 80% cap → switch sang Llama 3 local qua Ollama. UX vẫn ok vì FAQ phổ biến đã được cache."*

### 16.20 Tài nguyên học

| Topic | Resource |
|-------|----------|
| SignalR streaming | Microsoft Docs — Streaming in SignalR |
| OpenAI .NET SDK | github.com/openai/openai-dotnet |
| Semantic Kernel | microsoft.com/semantic-kernel |
| RAG patterns | Anthropic blog "Contextual Retrieval" |
| Qdrant với .NET | qdrant.tech/documentation/quickstart/ |
| Tool calling | platform.openai.com/docs/guides/function-calling |
| Streaming with IAsyncEnumerable | David Fowler's blog |
| Prompt engineering | OpenAI cookbook + Anthropic guide |

### 16.21 Risk & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|:----------:|:------:|-----------|
| AI API key bị leak | Low | High | Dùng env variable + GitHub Secret, không commit |
| Cost blow up vì spam | Medium | Medium | BR-35, BR-36 — rate limit + cap |
| Prompt injection | Medium | Medium | System prompt cứng + input filter |
| LLM hallucinate | High | Medium | BR-38 + RAG verify + tool calling |
| API timeout | Medium | Low | Timeout 10s + fallback FAQ |
| Vector DB chậm khi scale | Low (demo) | Low | Chỉ index 50-100 FAQ cho demo |
| User experience kém vì AI dở | Medium | High | Test prompt kỹ + escalate sớm |
| Privacy concern (PII trong conv) | Medium | High | BR-37 + BR-40, anonymize sau 90 ngày |

### 16.22 Tích hợp vào Twist Combos (cập nhật Section 12)

#### 🏆 Combo MỚI — RECOMMENDED HIGHEST IMPACT
**Specialty Coffee Shop + AI Customer Support**

- **Niche**: Cafe đặc sản
- **Twist 1**: 🤖 **AI Chatbot + Hybrid AI-Human Support** (SignalR + LLM + RAG)
- **Twist 2**: Real-time inventory (SignalR)
- **Twist 3**: Outbox pattern + Background jobs

**Tại sao đây là combo MẠNH NHẤT 2026:**
- AI Chat = trend nóng nhất, recruiter sẽ ấn tượng ngay
- Tích hợp 100% với business flow (không tách rời)
- SignalR đã dùng cho inventory → reuse cho chat → cost-effective
- War stories sẵn 4-5 câu chuyện kỹ thuật đặc sắc

**Tech keyword phong phú trong CV:**
```
.NET 8 · Clean Architecture · CQRS · MediatR · EF Core 8 ·
SignalR (Real-time + Streaming) · OpenAI API · RAG ·
Vector DB (Qdrant) · Tool Calling · Sentiment Analysis ·
Hangfire · Redis · Outbox Pattern · Docker · GitHub Actions
```

→ 1 project claim được **20+ skill modern**. Đây là CV jump tier.

---

## 📌 LỜI CUỐI

### Điều quan trọng nhất

1. **PICK ONE, SHIP ONE.** Đừng làm 2-3 project song song. 1 project polish > 5 project nửa vời.

2. **DEMO HOẠT ĐỘNG > Code đẹp.** Recruiter mở demo trong 30 giây — nếu không chạy, không có cơ hội thứ 2.

3. **WAR STORIES > Skill list.** Skill list có thể giả, war story phải thật. Mỗi twist bạn implement → 1 câu chuyện để kể.

4. **CẮT SCOPE QUYẾT LIỆT** khi deadline gần. Khi tuần 6 mà MVP chưa xong → skip stretch, tập trung ship.

5. **DOC LÀ DEAL-BREAKER.** README xấu = recruiter đóng tab. README đẹp = +20% chance được PV.

### Câu chuyện cuối

Sau 8 tuần, khi recruiter mở GitHub bạn:

> *"Specialty Coffee Shop — full-featured e-commerce với .NET 8, Clean Architecture, CQRS với MediatR, EF Core, SignalR real-time inventory, Outbox pattern, Hangfire background jobs, deployed on Render với Docker. Demo live: https://coffee-shop.huyqdev.com. 65% test coverage."*

Recruiter sẽ nghĩ: **"Đây là dev đã làm việc thật, không phải bài tập về nhà."** Đó là khoảng cách giữa offer 15M và offer 25M.

---

> 📅 *Document version 1.0 — 2026-06-02. Cập nhật khi có thay đổi scope hoặc architecture.*
