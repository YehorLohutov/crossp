using Files.Models;
using Microsoft.EntityFrameworkCore;

namespace Files.Repository
{
    public class FilesRepository : IFilesRepository
    {
        private readonly FilesDatabaseContext databaseContext = default;
        public FilesRepository(FilesDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        #region IFilesRepository
        public async Task<IEnumerable<Files.Models.File>> GetFilesAsync() => await databaseContext.Files.ToArrayAsync();
        public async Task<IEnumerable<Files.Models.File>> GetDefaultFilesAsync() => await databaseContext.GetDefaultFilesAsync();

        public async Task<Files.Models.File> GetFileAsync(int fileId)
        {
            Files.Models.File file = await databaseContext.Files.FirstOrDefaultAsync(t => t.Id == fileId);

            if (file == default)
                throw new NullReferenceException($"{nameof(Files.Models.File)} with {nameof(fileId)} = {fileId} not exist.");

            return file;
        }

        public async Task<Files.Models.File> GetFileAsync(string fileExternalId)
        {
            Files.Models.File file = await databaseContext.Files.FirstOrDefaultAsync(t => t.ExternalId == fileExternalId);

            if (file == default)
                throw new NullReferenceException($"{nameof(Files.Models.File)} with {nameof(fileExternalId)} = {fileExternalId} not exist.");

            return file;
        }

        public async Task<bool> FileExistsAsync(string fileExternalId) =>
            await databaseContext.Files.AnyAsync(t => t.ExternalId.Equals(fileExternalId));

        

        public bool IsFileDefault(Files.Models.File file) => databaseContext.IsFileDefault(file);

        public async Task<IEnumerable<Files.Models.File>> GetUserFilesAsync(string userExternalId)
        {
            if (string.IsNullOrWhiteSpace(userExternalId))
                throw new ArgumentNullException($"Parameter {nameof(userExternalId)} passed into {nameof(GetUserFilesAsync)} is null or empty.");

            return await databaseContext.Files.Where(file => file.UserExternalId.Equals(userExternalId)).ToArrayAsync();
        }

        public async Task InsertFileAsync(Files.Models.File file)
        {
            if (file == default)
                throw new NullReferenceException($"Parameter {nameof(file)} passed into {nameof(InsertFileAsync)} is null.");
            await databaseContext.AddAsync(file);
        }

        public void UpdateFile(Files.Models.File file)
        {
            if (file == default)
                throw new NullReferenceException($"Parameter {nameof(file)} passed into {nameof(UpdateFile)} is null.");

            databaseContext.Files.Update(file);
        }

        public void DeleteFile(Files.Models.File file)
        {
            if (file == default)
                throw new NullReferenceException($"Parameter {nameof(file)} passed into {nameof(DeleteFile)} is null.");

            databaseContext.Files.Remove(file);
        }

        public async Task DeleteFileAsync(int fileId)
        {
            Files.Models.File file = await GetFileAsync(fileId);
            DeleteFile(file);
        }

        public async Task DeleteFileAsync(string fileExternalId)
        {
            Files.Models.File file = await GetFileAsync(fileExternalId);
            DeleteFile(file);
        }
        #endregion

        public async Task SaveChangesAsync() => await databaseContext.SaveChangesAsync();
    }
}
