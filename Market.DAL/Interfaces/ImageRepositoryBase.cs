using System.IO;
using System.Threading.Tasks;
using Market.DAL.Results;
using Microsoft.AspNetCore.Http;

namespace Market.DAL.Interfaces
{
    /// <typeparam name="TModel">Имя параметра-типа может
    /// быть задействовано как имя папки для IO операций.</typeparam>
    public abstract class ImageRepositoryBase<TModel> where TModel : class
    {
        protected const string FolderNamePrefix = @"\Images\";

        /// <summary>
        /// Возвращает имя папки с префиксом <see cref="FolderNamePrefix"/>.
        /// </summary>
        protected string FolderName => Path.Combine(FolderNamePrefix, typeof(TModel).Name);

        public abstract Task<ImageSaveResult> Save(IFormFile imageFormFile);

        public abstract OperationResult Delete(string imgFilePath);
    }
}