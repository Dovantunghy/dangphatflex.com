using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(IFormFile file, string subfolder)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subfolder}/{fileName}";
    }
}
