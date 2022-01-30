using Files.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files.Repository
{
    public interface IFilesRepository
    {
        Task<IEnumerable<Files.Models.File>> GetFilesAsync();
        Task<IEnumerable<Files.Models.File>> GetDefaultFilesAsync();
        Task<Files.Models.File> GetFileAsync(int fileId);
        Task<Files.Models.File> GetFileAsync(string fileExternalId);
        Task<IEnumerable<Files.Models.File>> GetUserFilesAsync(string userExternalId);
        Task<bool> FileExistsAsync(string fileExternalId);

        bool IsFileDefault(Files.Models.File file);

        Task InsertFileAsync(Files.Models.File file);

        void UpdateFile(Files.Models.File file);

        void DeleteFile(Files.Models.File file);
        Task DeleteFileAsync(int fileId);
        Task DeleteFileAsync(string fileExternalId);
        Task SaveChangesAsync();
    }
}
