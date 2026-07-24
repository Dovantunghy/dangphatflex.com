using DangPhatFlex.Web.Models;
using DangPhatFlex.Web.Services;

namespace DangPhatFlex.Web.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context, ISlugService slugService)
    {
        if (context.CompanyInfos.Any())
            return;

        context.CompanyInfos.Add(new CompanyInfo
        {
            LegalName = "CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT",
            BrandName = "Đăng Phát Flex",
            Tagline = "Giải pháp khớp nối mềm inox cho ngành cơ điện M&E",
            AboutContent = "CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT là doanh nghiệp chuyên sản xuất, nhập khẩu và " +
                "phân phối các giải pháp khớp nối mềm inox và ống mềm inox dùng trong hệ thống cơ điện " +
                "(M&E), đặc biệt là hệ thống phòng cháy chữa cháy (Fire Protection), HVAC và cấp thoát " +
                "nước công nghiệp. Công ty tập trung cung cấp những sản phẩm đạt tiêu chuẩn kỹ thuật quốc " +
                "tế với khả năng giao hàng nhanh, giá cạnh tranh và dịch vụ kỹ thuật chuyên sâu. Chúng tôi " +
                "tạo dựng uy tín trên thị trường bằng phương châm hoạt động xoay quanh ba yếu tố cơ bản: " +
                "Nhanh nhất – Tốt nhất – Giá cả cạnh tranh nhất.",
            Mission = "Mang đến cho thị trường Việt Nam các giải pháp khớp nối mềm inox chất lượng cao, " +
                "giúp tăng độ an toàn cho công trình, giảm thời gian thi công, dễ dàng bảo trì, đáp ứng " +
                "các tiêu chuẩn quốc tế và tối ưu chi phí cho nhà thầu và chủ đầu tư.",
            Vision = "Trở thành thương hiệu hàng đầu Việt Nam về giải pháp ống mềm inox và phụ kiện cơ " +
                "điện cho ngành M&E.",
            Advantages = "Kho hàng lớn — luôn có sẵn hàng, đầy đủ chủng loại\n" +
                "Giao hàng nhanh — có khả năng giao ngay đến công trình\n" +
                "Chủ động nguồn hàng — nhập khẩu trực tiếp, không phụ thuộc nhiều vào trung gian\n" +
                "Giá cạnh tranh — tối ưu chi phí cho nhà thầu\n" +
                "Chất lượng — đạt tiêu chuẩn kỹ thuật quốc tế\n" +
                "Hỗ trợ kỹ thuật — tư vấn lựa chọn sản phẩm phù hợp với từng hệ thống",
            Address = "Tầng 2, Khu X3-2 Ngõ 68/45, Đường Nguyễn Văn Linh, P. Long Biên, TP. Hà Nội",
            Hotline = "0364.983.444",
            Email = "Info.dangphat@gmail.com",
            CoreValueFast = "Hàng hóa tại kho luôn đầy đủ chủng loại, giao hàng nhanh chóng đến tận chân công trình, đáp ứng nhanh các dự án.",
            CoreValueBest = "Nghiên cứu sản phẩm liên tục, đạt tiêu chuẩn quốc tế, tư vấn kỹ thuật chuyên nghiệp, phù hợp nhiều loại công trình.",
            CoreValueCompetitivePrice = "Chủ động nguồn hàng, nhập khẩu trực tiếp, tối ưu chi phí, mang lại giá bán cạnh tranh nhất.",
            MapEmbedUrl = "https://www.google.com/maps?q=Nguyen+Van+Linh,+Long+Bien,+Ha+Noi&output=embed"
        });

        var category = new ProductCategory
        {
            Name = "Khớp nối mềm inox",
            Slug = slugService.GenerateSlug("Khớp nối mềm inox"),
            Description = "Khớp nối mềm inox, hay còn gọi là ống mềm nối đầu phun sprinkler / dây mềm nối đầu phun " +
                "sprinkler, là giải pháp kết nối linh hoạt giữa đường ống chính và đầu phun (sprinkler head) " +
                "trong hệ thống chữa cháy tự động.",
            MetaTitle = "Ống mềm nối đầu phun sprinkler - Khớp nối mềm inox | Đăng Phát Flex",
            MetaDescription = "Chuyên cung cấp ống mềm nối đầu phun sprinkler, dây mềm nối đầu phun sprinkler đạt chuẩn UL/FM/TCVN. Giao hàng nhanh, giá cạnh tranh."
        };

        // Mỗi Accessory thuộc về đúng một Product, nên tạo mới danh sách phụ kiện cho từng
        // sản phẩm thay vì dùng chung một tập instance.
        List<Accessory> BuildStandardAccessories() =>
        [
            new()
            {
                Name = "Côn giảm",
                DefaultQuantity = 1,
                ImageUrl = "/images/products/accessory-reducer.jpg",
                ImageAlt = "Côn giảm ren 1/2\" - 3/4\" cho ống mềm nối đầu phun sprinkler Đăng Phát Flex"
            },
            new()
            {
                Name = "Đai ốc",
                DefaultQuantity = 2,
                ImageUrl = "/images/products/product-dp25-label-detail.jpg",
                ImageAlt = "Đai ốc siết đầu nối ống mềm nối đầu phun sprinkler Đăng Phát Flex"
            },
            new()
            {
                Name = "Gioăng cao su",
                DefaultQuantity = 2,
                ImageUrl = "/images/products/product-nipple-gasket-group.jpg",
                ImageAlt = "Gioăng cao su làm kín đầu nối ống mềm inox Đăng Phát Flex"
            },
            new()
            {
                Name = "Vòng đệm nhựa",
                DefaultQuantity = 2,
                ImageUrl = "/images/products/accessory-nipple-reducer-washer.jpg",
                ImageAlt = "Vòng đệm nhựa phụ kiện ống mềm nối đầu phun sprinkler Đăng Phát Flex"
            },
            new()
            {
                Name = "Thanh ngang",
                DefaultQuantity = 1,
                ImageUrl = "/images/products/accessory-square-bar.jpg",
                ImageAlt = "Thanh ngang (square bar) gá ống mềm nối đầu phun trên khung trần"
            },
            new()
            {
                Name = "Kẹp giữa",
                DefaultQuantity = 1,
                ImageUrl = "/images/products/accessory-clamp-assembled.jpg",
                ImageAlt = "Kẹp giữa phụ kiện khớp nối mềm inox Đăng Phát Flex"
            },
            new()
            {
                Name = "Kẹp bên",
                DefaultQuantity = 2,
                ImageUrl = "/images/products/accessory-clamp-parts-1.jpg",
                ImageAlt = "Kẹp bên phụ kiện khớp nối mềm inox Đăng Phát Flex"
            },
            new()
            {
                Name = "Nipple",
                DefaultQuantity = 1,
                ImageUrl = "/images/products/accessory-nipple-reducer-washer.jpg",
                ImageAlt = "Nipple, côn giảm và vòng đệm nhựa phụ kiện Đăng Phát Flex"
            },
        ];

        static void AddVariants(Product p, (string Code, string InletOutlet, int LengthMm, int MaxBends)[] data)
        {
            foreach (var v in data)
            {
                p.Variants.Add(new ProductVariant
                {
                    ProductCode = v.Code,
                    InletOutlet = v.InletOutlet,
                    InstallLengthMm = v.LengthMm,
                    MaxBends90 = v.MaxBends,
                    MinBendRadiusIn = "4",
                });
            }
        }

        // Dải mã đầy đủ theo bảng "List sp" trong báo giá của công ty. MaxBends = 0 nghĩa là
        // chưa có số liệu công bố cho chiều dài đó (view sẽ hiển thị "—" thay vì số bịa).
        var ubVariants = new (string Code, string InletOutlet, int LengthMm, int MaxBends)[]
        {
            ("DP25UB-15-700", "1x1/2", 700, 2),
            ("DP25UB-15-1000", "1x1/2", 1000, 3),
            ("DP25UB-15-1200", "1x1/2", 1200, 3),
            ("DP25UB-15-1500", "1x1/2", 1500, 3),
            ("DP25UB-15-1800", "1x1/2", 1800, 3),
            ("DP25UB-15-2000", "1x1/2", 2000, 0),
            ("DP25UB-15-2500", "1x1/2", 2500, 0),
            ("DP25UB-15-3000", "1x1/2", 3000, 0),
            ("DP25UB-20-700", "1x3/4", 700, 2),
            ("DP25UB-20-1000", "1x3/4", 1000, 3),
            ("DP25UB-20-1200", "1x3/4", 1200, 3),
            ("DP25UB-20-1500", "1x3/4", 1500, 3),
            ("DP25UB-20-1800", "1x3/4", 1800, 3),
            ("DP25UB-20-2000", "1x3/4", 2000, 0),
            ("DP25UB-20-2500", "1x3/4", 2500, 0),
            ("DP25UB-20-3000", "1x3/4", 3000, 0),
        };

        var bVariants = new (string Code, string InletOutlet, int LengthMm, int MaxBends)[]
        {
            ("DP25B-15-700", "1x1/2", 700, 2),
            ("DP25B-15-1000", "1x1/2", 1000, 3),
            ("DP25B-15-1200", "1x1/2", 1200, 3),
            ("DP25B-15-1500", "1x1/2", 1500, 3),
            ("DP25B-15-1800", "1x1/2", 1800, 3),
            ("DP25B-20-700", "1x3/4", 700, 2),
            ("DP25B-20-1000", "1x3/4", 1000, 3),
            ("DP25B-20-1200", "1x3/4", 1200, 3),
            ("DP25B-20-1500", "1x3/4", 1500, 3),
            ("DP25B-20-1800", "1x3/4", 1800, 3),
        };

        // Sản phẩm 1: DP25UB (không bện). Giữ nguyên slug "dang-phat-flex-dp25" để không phá
        // vỡ URL/SEO đã công bố.
        var dp25ub = new Product
        {
            ProductCategory = category,
            Name = "Đăng Phát Flex DP25UB",
            Slug = slugService.GenerateSlug("Dang Phat Flex DP25"),
            Description = "Ống mềm nối đầu phun sprinkler DP25UB (không bện) — thân ống gân xoắn inox 304 " +
                "(Helical Corrugated Hose), đầu vào ren 1\", đầu ra 1/2\" hoặc 3/4\", đủ chiều dài từ 700mm " +
                "đến 3000mm. Trọng lượng nhẹ, thi công nhanh, phù hợp công trình dân dụng, văn phòng và " +
                "trung tâm thương mại.",
            InnerDiameter = "24.2mm",
            OuterDiameter = "24.8mm",
            HoseType = "Ống gân xoắn không bện (Unbraided Helical Corrugated Hose), loại ren (Threaded)",
            MaxTemperature = "107°C (225°F)",
            MaxPressure = "14kg/cm² (TCVN) / 200 psi (UL) / 200 psi (FM)",
            MinBendRadius = "4 inch (UL/ULC) / 9 inch (FM)",
            Standards = "UL, ULC, FM, TCVN",
            MainImageUrl = "/images/products/product-dp25-lineup.jpg",
            MainImageAlt = "Ống mềm nối đầu phun sprinkler DP25UB Đăng Phát Flex - dây mềm nối đầu phun sprinkler không bện",
            MetaTitle = "Ống mềm nối đầu phun sprinkler DP25UB (không bện) - Đạt chuẩn UL/FM/TCVN | Đăng Phát Flex",
            MetaDescription = "Ống mềm nối đầu phun sprinkler DP25UB không bện: đầy đủ thông số áp suất, nhiệt độ, bán kính uốn cong, đạt chuẩn UL/FM/TCVN."
        };
        AddVariants(dp25ub, ubVariants);
        dp25ub.Accessories.AddRange(BuildStandardAccessories());

        // Sản phẩm 2: DP25B (có bện).
        var dp25b = new Product
        {
            ProductCategory = category,
            Name = "Đăng Phát Flex DP25B",
            Slug = slugService.GenerateSlug("Dang Phat Flex DP25B"),
            Description = "Ống mềm nối đầu phun sprinkler DP25B (có bện) — bổ sung lớp lưới thép inox bện quanh " +
                "thân ống, tăng khả năng chịu áp lực đột ngột và chống rung động, chiều dài 700–1800mm. " +
                "Khuyến nghị cho nhà xưởng, kho vận và công trình yêu cầu chứng nhận FM.",
            InnerDiameter = "24.2mm",
            OuterDiameter = "24.8mm",
            HoseType = "Ống gân xoắn có bện lưới thép (Braided Helical Corrugated Hose), loại ren (Threaded)",
            MaxTemperature = "107°C (225°F)",
            MaxPressure = "14kg/cm² (TCVN) / 200 psi (UL) / 200 psi (FM)",
            MinBendRadius = "4 inch (UL/ULC) / 9 inch (FM)",
            Standards = "UL, ULC, FM, TCVN",
            MainImageUrl = "/images/products/product-packing-box.jpg",
            MainImageAlt = "Ống mềm nối đầu phun sprinkler DP25B Đăng Phát Flex - dây mềm nối đầu phun sprinkler có bện",
            MetaTitle = "Ống mềm nối đầu phun sprinkler DP25B (có bện) - Đạt chuẩn UL/FM/TCVN | Đăng Phát Flex",
            MetaDescription = "Ống mềm nối đầu phun sprinkler DP25B có bện lưới thép: chịu áp lực và chống rung tốt, đạt chuẩn UL/FM/TCVN cho nhà xưởng, kho vận."
        };
        AddVariants(dp25b, bVariants);
        dp25b.Accessories.AddRange(BuildStandardAccessories());

        // Sản phẩm 3: bộ phụ kiện & giá đỡ (không có biến thể) — trang chi tiết tự ẩn bảng mã
        // sản phẩm và các dòng thông số bỏ trống.
        var accessoriesKit = new Product
        {
            ProductCategory = category,
            Name = "Phụ kiện & giá đỡ khớp nối mềm inox",
            Slug = slugService.GenerateSlug("Phu kien gia do khop noi mem inox"),
            Description = "Bộ phụ kiện đồng bộ cho hệ khớp nối mềm inox: giá đỡ (kẹp giữa / kẹp bên), nipple, " +
                "côn giảm, đai ốc, gioăng cao su và vòng đệm nhựa — đầy đủ chi tiết để lắp đặt hoàn thiện một " +
                "điểm đầu phun sprinkler.",
            HoseType = "Giá đỡ, nipple, côn giảm, đai ốc, gioăng cao su, vòng đệm nhựa",
            Standards = "UL, FM, TCVN",
            MainImageUrl = "/images/products/product-nipple-gasket-group.jpg",
            MainImageAlt = "Bộ phụ kiện và giá đỡ khớp nối mềm inox Đăng Phát Flex - nipple, côn giảm, gioăng",
            MetaTitle = "Phụ kiện & giá đỡ khớp nối mềm inox - Nipple, côn giảm, kẹp giữ | Đăng Phát Flex",
            MetaDescription = "Bộ phụ kiện đồng bộ cho ống mềm nối đầu phun sprinkler: giá đỡ, nipple, côn giảm, gioăng, vòng đệm — đầy đủ để lắp đặt hoàn thiện."
        };
        accessoriesKit.Accessories.AddRange(BuildStandardAccessories());

        context.ProductCategories.Add(category);
        context.Products.AddRange(dp25ub, dp25b, accessoriesKit);
        context.SaveChanges();
    }

    public static void SeedNewsArticles(AppDbContext context, ISlugService slugService)
    {
        if (context.NewsArticles.Any())
            return;

        var articles = new[]
        {
            new NewsArticle
            {
                Title = "Ống mềm nối đầu phun là gì? Cấu tạo, phân loại và cách chọn mua",
                Summary = "Ống mềm nối đầu phun là bộ phận kết nối linh hoạt giữa đường ống chính và đầu phun sprinkler. " +
                    "Tìm hiểu cấu tạo, phân loại và tiêu chí chọn mua ống mềm nối đầu phun đạt chuẩn.",
                Content = "Ống mềm nối đầu phun (flexible sprinkler hose) là đoạn ống kim loại dẻo dùng để kết nối " +
                    "giữa đường ống chính (branch line) của hệ thống chữa cháy và đầu phun sprinkler gắn trên trần " +
                    "nhà. Thay vì dùng ống thép cứng hàn cố định, ống mềm nối đầu phun cho phép lắp đặt nhanh hơn, " +
                    "linh hoạt điều chỉnh vị trí đầu phun theo trần thả (trần thạch cao) mà không cần định vị " +
                    "chính xác tuyệt đối vị trí ống chính từ đầu.\n\n" +
                    "Cấu tạo của một bộ ống mềm nối đầu phun tiêu chuẩn gồm: thân ống dạng gân xoắn (helical " +
                    "corrugated hose) làm từ thép không gỉ (inox 304), hai đầu ren kết nối (đầu vào 1 inch nối vào " +
                    "ống chính, đầu ra 1/2 inch hoặc 3/4 inch nối vào đầu phun), cùng bộ phụ kiện đi kèm gồm côn " +
                    "giảm, đai ốc, gioăng cao su làm kín và giá treo cố định (bracket).\n\n" +
                    "Về phân loại, ống mềm nối đầu phun trên thị trường hiện có hai dạng chính: loại không bện " +
                    "(unbraided) và loại có bện (braided) lớp lưới thép bên ngoài để tăng khả năng chịu áp lực và " +
                    "chống rung động. Ống mềm nối đầu phun cũng được phân theo chiều dài lắp đặt (phổ biến từ 700mm " +
                    "đến 1800mm) và theo cỡ đầu ra (1/2 inch hoặc 3/4 inch) tùy loại đầu phun sử dụng.\n\n" +
                    "Khi chọn mua ống mềm nối đầu phun cho công trình, cần lưu ý các tiêu chí sau: sản phẩm phải " +
                    "có chứng nhận UL Listed hoặc FM Approved (tiêu chuẩn quốc tế cho thiết bị phòng cháy chữa " +
                    "cháy) hoặc đạt TCVN nếu áp dụng tiêu chuẩn Việt Nam; áp suất làm việc tối thiểu 175psi, nhiệt " +
                    "độ hoạt động ổn định đến trên 100°C; bán kính uốn cong tối thiểu phải được nhà sản xuất công " +
                    "bố rõ ràng để tránh gãy gập làm giảm lưu lượng nước khi chữa cháy.\n\n" +
                    "Đăng Phát Flex hiện cung cấp dòng ống mềm nối đầu phun DP25UB (không bện) và DP25B (có bện), " +
                    "đạt đầy đủ chứng nhận UL, FM và TCVN, đa dạng chiều dài và cỡ đầu ra, sẵn hàng giao nhanh toàn " +
                    "quốc.",
                CoverImageAlt = "Cấu tạo ống mềm nối đầu phun sprinkler DP25 Đăng Phát Flex",
                PublishedAt = new DateTime(2026, 6, 2, 0, 0, 0, DateTimeKind.Utc),
                MetaTitle = "Ống mềm nối đầu phun là gì? Cấu tạo, phân loại, cách chọn mua | Đăng Phát Flex",
                MetaDescription = "Ống mềm nối đầu phun là gì, cấu tạo gồm những bộ phận nào, phân loại và tiêu chí chọn mua ống mềm nối đầu phun đạt chuẩn UL/FM/TCVN."
            },
            new NewsArticle
            {
                Title = "Ống mềm nối đầu phun sprinkler: Tiêu chuẩn UL/FM/TCVN và hướng dẫn lắp đặt",
                Summary = "Hướng dẫn đọc hiểu tiêu chuẩn UL, FM, TCVN áp dụng cho ống mềm nối đầu phun sprinkler " +
                    "và các bước lắp đặt đúng kỹ thuật để đảm bảo hệ thống chữa cháy vận hành an toàn.",
                Content = "Trong hệ thống chữa cháy tự động (automatic sprinkler system), ống mềm nối đầu phun " +
                    "sprinkler là thiết bị bắt buộc phải qua kiểm định nghiêm ngặt trước khi đưa vào sử dụng, vì " +
                    "đây là điểm kết nối trực tiếp với đầu phun — nơi nước phải thoát ra tức thời khi có cháy. Ba " +
                    "tiêu chuẩn phổ biến nhất cho ống mềm nối đầu phun sprinkler tại Việt Nam hiện nay là UL " +
                    "(Underwriters Laboratories - Mỹ), FM (FM Approvals - Mỹ) và TCVN (Tiêu chuẩn Việt Nam).\n\n" +
                    "Chứng nhận UL Listed xác nhận ống mềm nối đầu phun sprinkler đã được kiểm tra độc lập về khả " +
                    "năng chịu áp suất, độ bền vật liệu và khả năng chống cháy trong điều kiện thử nghiệm tiêu " +
                    "chuẩn hóa. FM Approved là chứng nhận song song, thường được các công trình có yêu cầu bảo " +
                    "hiểm tài sản cao (nhà máy, kho hàng, trung tâm dữ liệu) yêu cầu bắt buộc. TCVN là bộ tiêu " +
                    "chuẩn Việt Nam hóa, áp dụng cho các công trình cần nghiệm thu phòng cháy chữa cháy trong " +
                    "nước theo quy định của Cục Cảnh sát PCCC.\n\n" +
                    "Về thông số kỹ thuật, một bộ ống mềm nối đầu phun sprinkler đạt chuẩn thường có: áp suất làm " +
                    "việc tối đa 200psi (đạt UL/FM) hoặc 14kg/cm² (đạt TCVN), nhiệt độ hoạt động tối đa khoảng " +
                    "107°C (225°F), và bán kính uốn cong tối thiểu 4 inch theo UL/ULC hoặc 9 inch theo FM.\n\n" +
                    "Về hướng dẫn lắp đặt: bước 1, xác định vị trí đầu phun theo bản vẽ hệ thống PCCC đã duyệt; " +
                    "bước 2, lắp giá treo (bracket) cố định lên trần hoặc khung xương trần thả tại đúng vị trí đầu " +
                    "phun; bước 3, nối đầu vào (1 inch) của ống mềm nối đầu phun sprinkler vào cút chờ trên đường " +
                    "ống chính bằng ren tiêu chuẩn NPT/BSPT, siết chặt và dùng băng tan hoặc keo chuyên dụng để làm " +
                    "kín; bước 4, luồn ống qua giá treo, tránh gập ống nhỏ hơn bán kính uốn cong tối thiểu cho " +
                    "phép; bước 5, lắp đầu phun vào đầu ra (1/2 hoặc 3/4 inch) của ống mềm, căn chỉnh đầu phun " +
                    "vuông góc với trần; bước 6, xả khí và thử áp suất toàn hệ thống trước khi hoàn thiện trần.\n\n" +
                    "Đăng Phát Flex cung cấp ống mềm nối đầu phun sprinkler đạt đồng thời cả ba chứng nhận UL, FM " +
                    "và TCVN, kèm hồ sơ kỹ thuật đầy đủ để phục vụ nghiệm thu công trình.",
                CoverImageAlt = "Lắp đặt ống mềm nối đầu phun sprinkler đạt chuẩn UL FM TCVN",
                PublishedAt = new DateTime(2026, 6, 16, 0, 0, 0, DateTimeKind.Utc),
                MetaTitle = "Ống mềm nối đầu phun sprinkler: Chuẩn UL/FM/TCVN & hướng dẫn lắp đặt | Đăng Phát Flex",
                MetaDescription = "Hướng dẫn tiêu chuẩn UL/FM/TCVN và các bước lắp đặt ống mềm nối đầu phun sprinkler đúng kỹ thuật, đảm bảo an toàn hệ thống chữa cháy."
            },
            new NewsArticle
            {
                Title = "Dây mềm nối đầu phun sprinkler DP25UB và DP25B: Nên chọn loại nào?",
                Summary = "So sánh dây mềm nối đầu phun sprinkler DP25UB (không bện) và DP25B (có bện) về cấu tạo, " +
                    "khả năng chịu lực và trường hợp sử dụng phù hợp cho từng loại công trình.",
                Content = "Dây mềm nối đầu phun sprinkler là tên gọi khác của ống mềm nối đầu phun, thường được " +
                    "gọi theo thói quen của đội thi công cơ điện (M&E) tại công trình. Trên thị trường, dây mềm " +
                    "nối đầu phun sprinkler phổ biến nhất hiện có hai dòng: loại không bện (unbraided, ký hiệu " +
                    "DP25UB) và loại có bện lưới thép bên ngoài (braided, ký hiệu DP25B). Nhiều chủ đầu tư và nhà " +
                    "thầu thường phân vân không biết nên chọn loại nào cho công trình của mình.\n\n" +
                    "Dây mềm nối đầu phun sprinkler DP25UB (không bện) có cấu tạo đơn giản hơn: chỉ gồm một lớp " +
                    "ống gân xoắn inox. Ưu điểm của loại này là giá thành thấp hơn, trọng lượng nhẹ, dễ thi công " +
                    "trong không gian trần hẹp. DP25UB phù hợp với các công trình dân dụng, văn phòng, trung tâm " +
                    "thương mại có tải trọng rung động thấp và không yêu cầu khắt khe về khả năng chịu va đập cơ " +
                    "học.\n\n" +
                    "Dây mềm nối đầu phun sprinkler DP25B (có bện) được bổ sung thêm một lớp lưới thép không gỉ " +
                    "bện quanh thân ống. Lớp bện này giúp tăng đáng kể khả năng chịu áp lực đột ngột, chống rung " +
                    "động và hạn chế giãn nở khi có dòng nước áp suất cao đi qua đột ngột lúc kích hoạt chữa cháy. " +
                    "DP25B thường được khuyến nghị cho nhà xưởng công nghiệp, kho hàng có xe nâng hoạt động (rung " +
                    "chấn liên tục), hoặc các công trình yêu cầu chứng nhận FM Approved nghiêm ngặt.\n\n" +
                    "Về thông số kỹ thuật, cả hai dòng dây mềm nối đầu phun sprinkler DP25UB và DP25B của Đăng " +
                    "Phát Flex đều có đường kính trong 24.2mm, đường kính ngoài 24.8mm, đầu vào ren 1 inch, đầu ra " +
                    "1/2 hoặc 3/4 inch, áp suất làm việc tối đa 200psi, nhiệt độ hoạt động tối đa 107°C, và đều đạt " +
                    "chứng nhận UL, FM, TCVN. Sự khác biệt chính nằm ở khả năng chịu lực cơ học và giá thành — " +
                    "DP25B cao hơn DP25UB khoảng do chi phí vật liệu lớp bện bổ sung.\n\n" +
                    "Lời khuyên: nếu công trình là văn phòng, chung cư, khách sạn — dây mềm nối đầu phun sprinkler " +
                    "DP25UB là lựa chọn kinh tế và đủ đáp ứng yêu cầu kỹ thuật. Nếu công trình là nhà xưởng, kho " +
                    "vận, hoặc yêu cầu chứng nhận FM cho mục đích bảo hiểm — nên chọn DP25B để đảm bảo độ bền lâu " +
                    "dài. Liên hệ Đăng Phát Flex để được tư vấn chọn đúng loại dây mềm nối đầu phun sprinkler cho " +
                    "công trình cụ thể của bạn.",
                CoverImageAlt = "So sánh dây mềm nối đầu phun sprinkler DP25UB và DP25B",
                PublishedAt = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                MetaTitle = "Dây mềm nối đầu phun sprinkler DP25UB vs DP25B: Nên chọn loại nào? | Đăng Phát Flex",
                MetaDescription = "So sánh dây mềm nối đầu phun sprinkler DP25UB (không bện) và DP25B (có bện) - nên chọn loại nào phù hợp với công trình của bạn."
            }
        };

        foreach (var article in articles)
            article.Slug = slugService.GenerateSlug(article.Title);

        context.NewsArticles.AddRange(articles);
        context.SaveChanges();
    }
}
