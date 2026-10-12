using GestionInterventions.Application.Common.Interfaces;

namespace GestionInterventions.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;

    public FileStorageService(string webRootPath)
    {
        _uploadDirectory = Path.Combine(webRootPath, "uploads", "equipements");
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_uploadDirectory);

        var extension = Path.GetExtension(fileName);

        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(_uploadDirectory, storedFileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);

        await content.CopyToAsync(fileStream, cancellationToken);

        return Path.Combine("uploads", "equipements", storedFileName)
                   .Replace("\\", "/");
    }
}