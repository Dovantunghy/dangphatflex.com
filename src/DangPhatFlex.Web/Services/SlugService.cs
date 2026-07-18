using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DangPhatFlex.Web.Services;

public interface ISlugService
{
    string GenerateSlug(string input);
}

public class SlugService : ISlugService
{
    public string GenerateSlug(string input)
    {
        var normalized = RemoveDiacritics(input.Trim().ToLowerInvariant());
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        normalized = Regex.Replace(normalized, @"\s+", "-");
        normalized = Regex.Replace(normalized, @"-+", "-");
        return normalized.Trim('-');
    }

    private static string RemoveDiacritics(string text)
    {
        text = text.Replace('đ', 'd').Replace('Đ', 'D');
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
