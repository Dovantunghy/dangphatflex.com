# Đăng Phát Flex — Website giới thiệu công ty & sản phẩm

Date: 2026-07-18
Status: Approved

## Bối cảnh

CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT (thương hiệu **Đăng Phát Flex**) cần một website giới
thiệu công ty và sản phẩm, chuẩn SEO, thể hiện năng lực chuyên nghiệp. Công ty sản
xuất/nhập khẩu/phân phối khớp nối mềm inox (ống mềm inox dạng gân xoắn) dùng cho hệ
thống chữa cháy sprinkler.

Nguồn nội dung: `Catalogue Dang Phat.pdf` (thông tin công ty + thông số kỹ thuật sản
phẩm) và `Logo Đăng Phát Flex.png` đã có trong thư mục dự án. Nội dung công ty/sản
phẩm ban đầu lấy trực tiếp từ catalog; nội dung có thể chỉnh sửa qua trang Admin sau
khi website hoàn thành.

### Thông tin công ty (từ catalog)

- Tên pháp lý: CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT
- Thương hiệu: ĐĂNG PHÁT FLEX — "Giải pháp khớp nối mềm inox"
- Định vị: Nhanh nhất – Tốt nhất – Giá cả cạnh tranh nhất
- Địa chỉ: Tầng 2, Khu X3-2 Ngõ 68/45, Đường Nguyễn Văn Linh, P. Long Biên, TP. Hà Nội
- Hotline: 0364.983.444 — Email: Info.dangphat@gmail.com

### Sản phẩm (từ catalog)

Dòng ống mềm inox mã `DP25UB-XX-YY` (không bện) và `DP25B-XX-YY` (có bện), XX = cỡ
đầu ra kết nối, YY = chiều dài. Chiều dài tiêu chuẩn: 700/1000/1200/1500/1800mm. Đầu
ra: 1/2" hoặc 3/4" NPT/BSPT. Đầu vào: 1" NPT/BSPT. Đạt chuẩn UL/ULC/FM/TCVN, nhiệt độ
hoạt động tối đa 107°C (225°F), áp suất tối đa 200psi/14kg/cm². Kèm phụ kiện: côn
giảm, đai ốc, gioăng cao su, vòng đệm nhựa, nipple, kẹp giữa/kẹp bên, thanh ngang.

## Kiến trúc

Một solution **ASP.NET Core MVC (.NET 8)** duy nhất, chia 2 Area:

- **Public** — trang giới thiệu/sản phẩm/liên hệ, render phía server bằng Razor để
  tối ưu SEO (không dùng SPA).
- **Admin** — CRUD nội dung, bảo vệ bằng ASP.NET Core Identity (1 role Admin).

EF Core làm ORM, migration-based schema. DB mặc định: **SQLite** (không cần cài đặt
SQL Server, dễ chạy dev/deploy; đổi sang SQL Server sau này chỉ cần đổi connection
string + provider, không đổi code).

## Mô hình dữ liệu

- `ProductCategory` — Id, Tên, Slug, Mô tả, SEO fields
- `Product` — Id, CategoryId, Tên, Slug, MôTả (rich text), ẢnhChính, DatasheetPdfUrl,
  ĐườngKínhTrong/Ngoài, LoạiỐng, NhiệtĐộTốiĐa, ÁpSuấtTốiĐa, BánKínhUốnCongNhỏNhất,
  TiêuChuẩn (UL/FM/TCVN), SEO fields (MetaTitle, MetaDescription, OgImage)
- `ProductVariant` — Id, ProductId, MãSảnPhẩm (vd DP25UB-15-700), Inlet, Outlet,
  ChiềuDàiLắpĐặt(mm), SốLầnUốn90ToiDa, BánKínhUốnCongTốiThiểu, ChiềuDàiTươngĐươngỐngThép
- `Accessory` — Id, ProductId (nullable, có thể dùng chung), Tên, Ảnh, SốLượngMặcĐịnh
- `CompanyInfo` — singleton row: GiớiThiệu, ĐịaChỉ, Hotline, Email, BảnĐồEmbedUrl,
  BaGiáTrịCốtLõi (Nhanh nhất/Tốt nhất/Giá cạnh tranh nhất)
- `ContactSubmission` — Id, Họ Tên, SĐT, Email, NộiDung, NgàyGửi, ĐãXửLý(bool)

Mỗi entity có nội dung public (`Product`, `ProductCategory`, trang tĩnh) đều có field
SEO riêng: `MetaTitle`, `MetaDescription`, `Slug` (unique, tuỳ chỉnh được).

## Chiến lược SEO

- URL thân thiện: `/san-pham/{category-slug}/{product-slug}`
- Title/meta description động lấy từ DB cho từng trang, fallback hợp lý nếu trống
- Open Graph + Twitter Card tags
- JSON-LD structured data: `Organization` (trang chủ), `Product` (trang chi tiết SP)
- `sitemap.xml` tự sinh từ DB (cập nhật khi thêm/sửa sản phẩm), `robots.txt` tĩnh
- Ảnh bắt buộc có `alt` text khi upload qua Admin
- Responsive image, lazy-load ảnh dưới fold
- Bundle & minify CSS/JS, tối thiểu hoá JS phía client (không SPA nặng) để tối ưu
  Core Web Vitals

## Trang & UX (Public)

- **Trang chủ** — hero giới thiệu năng lực, 3 giá trị cốt lõi, sản phẩm nổi bật, CTA
  liên hệ/hotline
- **Giới thiệu** — nội dung công ty từ catalog, năng lực sản xuất/phân phối
- **Sản phẩm** — danh sách theo category → trang chi tiết: bảng thông số kỹ thuật đầy
  đủ (responsive/dễ đọc mobile), bảng mã sản phẩm/biến thể (variant), sơ đồ phụ kiện,
  nút tải datasheet PDF
- **Liên hệ** — form gửi yêu cầu (lưu DB + gửi email thông báo), bản đồ nhúng, hotline
  và email nổi bật

Phong cách thiết kế: corporate hiện đại, chuyên nghiệp, dùng bảng màu từ logo (xanh
dương đậm ~#0B5FA8 + dải vàng kim gradient làm điểm nhấn), nhiều khoảng trắng, layout
rõ ràng, nhấn mạnh năng lực kỹ thuật/công nghiệp. Hệ thống UI cụ thể (component,
spacing, typography) sẽ được định nghĩa khi triển khai qua skill `ui-ux-pro-max`.

## Admin panel

- Đăng nhập qua ASP.NET Core Identity (1 role: Admin)
- Dashboard đơn giản (số liên hệ mới, số sản phẩm)
- CRUD: ProductCategory, Product (+ ProductVariant, Accessory con), CompanyInfo
- Upload ảnh/PDF vào `wwwroot/uploads`, yêu cầu nhập alt text khi upload ảnh
- Rich-text editor (TinyMCE hoặc tương đương) cho các trường mô tả dài
- Xem danh sách `ContactSubmission`, đánh dấu đã xử lý

## Ngoài phạm vi (Out of scope)

- Không giỏ hàng / thanh toán online
- Không đa ngôn ngữ (chỉ tiếng Việt)
- Không blog/tin tức
- Không tích hợp CMS mã nguồn mở bên thứ ba
- Không cấu hình hosting/deploy cụ thể (chưa chốt nền tảng, build portable)

## Kiểm thử

- Unit test cho service tầng nghiệp vụ (SEO slug generation, sitemap generation)
- Integration test cho các route Public quan trọng (trang chủ, chi tiết sản phẩm, form
  liên hệ) trả về đúng status code và chứa meta tag SEO
- Test thủ công Admin CRUD qua trình duyệt trước khi bàn giao
