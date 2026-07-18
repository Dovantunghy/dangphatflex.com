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
            Tagline = "Giải pháp khớp nối mềm inox",
            AboutContent = "CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT là đơn vị chuyên sản xuất, " +
                "nhập khẩu, phân phối và cung cấp các sản phẩm đầu nối, ống kim loại dẻo " +
                "bằng thép không gỉ. Chúng tôi tạo dựng uy tín trên thị trường bằng " +
                "phương châm hoạt động xoay quanh ba yếu tố cơ bản: Nhanh nhất – Tốt nhất " +
                "– Giá cả cạnh tranh nhất.",
            Address = "Tầng 2, Khu X3-2 Ngõ 68/45, Đường Nguyễn Văn Linh, P. Long Biên, TP. Hà Nội",
            Hotline = "0364.983.444",
            Email = "Info.dangphat@gmail.com",
            CoreValueFast = "Hàng hóa tại kho luôn đầy đủ chủng loại, giao hàng nhanh chóng đến tận chân công trình.",
            CoreValueBest = "Đội ngũ luôn đi đầu nghiên cứu, tìm kiếm hàng hóa đạt chuẩn quốc tế, phù hợp mọi công trình.",
            CoreValueCompetitivePrice = "Chủ động nguồn hàng giúp chúng tôi tự tin với giá cả cạnh tranh nhất."
        });

        var category = new ProductCategory
        {
            Name = "Khớp nối mềm inox",
            Slug = slugService.GenerateSlug("Khớp nối mềm inox"),
            Description = "Khớp nối mềm inox (ống mềm dạng gân xoắn) dùng cho hệ thống chữa cháy sprinkler.",
            MetaTitle = "Khớp nối mềm inox chữa cháy | Đăng Phát Flex",
            MetaDescription = "Khớp nối mềm inox đạt chuẩn UL/FM/TCVN cho hệ thống sprinkler, giao hàng nhanh, giá cạnh tranh."
        };

        var product = new Product
        {
            ProductCategory = category,
            Name = "Đăng Phát Flex DP25",
            Slug = slugService.GenerateSlug("Dang Phat Flex DP25"),
            Description = "Ống mềm inox gân xoắn (Helical Corrugated Hose) dùng cho đầu phun sprinkler, " +
                "có 2 phiên bản: DP25UB (không bện) và DP25B (có bện).",
            InnerDiameter = "24.2mm",
            OuterDiameter = "24.8mm",
            HoseType = "Ống gân xoắn (Helical Corrugated Hose), loại ren (Threaded)",
            MaxTemperature = "107°C (225°F)",
            MaxPressure = "14kg/cm² (TCVN) / 200 psi (UL) / 200 psi (FM)",
            MinBendRadius = "4 inch (UL/ULC) / 9 inch (FM)",
            Standards = "UL, ULC, FM, TCVN",
            MetaTitle = "Ống mềm inox DP25UB / DP25B | Đăng Phát Flex",
            MetaDescription = "Thông số kỹ thuật đầy đủ dòng ống mềm inox DP25UB/DP25B: áp suất, nhiệt độ, bán kính uốn cong, đạt chuẩn UL/FM/TCVN."
        };

        var variantData = new (string Code, string InletOutlet, int LengthMm, int MaxBends)[]
        {
            ("DP25UB-15-700", "1x1/2", 700, 2),
            ("DP25UB-15-1000", "1x1/2", 1000, 3),
            ("DP25UB-15-1200", "1x1/2", 1200, 3),
            ("DP25UB-15-1500", "1x1/2", 1500, 3),
            ("DP25UB-15-1800", "1x1/2", 1800, 3),
            ("DP25UB-20-700", "1x3/4", 700, 2),
            ("DP25UB-20-1000", "1x3/4", 1000, 3),
            ("DP25UB-20-1200", "1x3/4", 1200, 3),
            ("DP25UB-20-1500", "1x3/4", 1500, 3),
            ("DP25UB-20-1800", "1x3/4", 1800, 3),
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

        foreach (var v in variantData)
        {
            product.Variants.Add(new ProductVariant
            {
                ProductCode = v.Code,
                InletOutlet = v.InletOutlet,
                InstallLengthMm = v.LengthMm,
                MaxBends90 = v.MaxBends,
                MinBendRadiusIn = "4",
            });
        }

        product.Accessories.Add(new Accessory { Name = "Côn giảm", DefaultQuantity = 1 });
        product.Accessories.Add(new Accessory { Name = "Đai ốc", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Gioăng cao su", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Vòng đệm nhựa", DefaultQuantity = 2 });
        product.Accessories.Add(new Accessory { Name = "Nipple", DefaultQuantity = 1 });

        context.ProductCategories.Add(category);
        context.Products.Add(product);
        context.SaveChanges();
    }
}
