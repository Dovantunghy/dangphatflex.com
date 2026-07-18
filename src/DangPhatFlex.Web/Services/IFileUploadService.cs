using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Services;

public interface IFileUploadService
{
    Task<string> SaveAsync(IFormFile file, string subfolder);
}
