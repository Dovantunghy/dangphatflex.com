using DangPhatFlex.Web.Services;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class SlugServiceTests
{
    private readonly SlugService _sut = new();

    [Theory]
    [InlineData("Khớp nối mềm inox", "khop-noi-mem-inox")]
    [InlineData("Đăng Phát Flex DP25UB", "dang-phat-flex-dp25ub")]
    [InlineData("  Nhiều   khoảng trắng  ", "nhieu-khoang-trang")]
    [InlineData("Có Dấu Gạch-Ngang!", "co-dau-gach-ngang")]
    public void GenerateSlug_ProducesUrlSafeLowercaseSlug(string input, string expected)
    {
        var result = _sut.GenerateSlug(input);
        Assert.Equal(expected, result);
    }
}
