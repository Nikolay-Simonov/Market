using Market.DAL.Enums;
using Market.DAL.Interfaces;
using Market.DAL.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Market.DAL.Repositories
{
    internal class ImageRepository<TModel> : ImageRepositoryBase<TModel> where TModel : class
    {
        private readonly IContentEnvironment _contentEnvironment;

        public ImageRepository(IContentEnvironment contentEnvironment)
        {
            _contentEnvironment = contentEnvironment;
        }

        private string ImagesDirectory => Path.Combine(_contentEnvironment.Path, FolderName);

        /// <summary>
        /// Возвращает результат сохранения файла изображения.
        /// </summary>
        /// <param name="imageFormFile">Файл изображения.</param>
        public override async Task<ImageSaveResult> Save(IFormFile imageFormFile)
        {
            var resultType = ResultType.Error;
            var messages = new List<string>();
            var outPath = string.Empty;

            try
            {
                if (imageFormFile == null)
                {
                    throw new ArgumentNullException();
                }

                string imageExtension = Path.GetExtension(imageFormFile.FileName)
                    .Replace(".", string.Empty);
                var allowExt = Enum.GetNames(typeof(AllowableExtension))
                    .Select(e => e.ToUpperInvariant());

                if (!allowExt.Contains(imageExtension.ToUpperInvariant()))
                {
                    throw new ArgumentException();
                }

                string imageFileName = Guid.NewGuid().ToString() + "." + imageExtension;
                string outputDirectory = _contentEnvironment.Path + ImagesDirectory;
                string writePath = outputDirectory + "\\" + imageFileName;
                string outputPath = ImagesDirectory + "\\" + imageFileName;
                Directory.CreateDirectory(outputDirectory);

                await using (var fileStream = new FileStream(writePath, FileMode.Create))
                await imageFormFile.CopyToAsync(fileStream);

                resultType = ResultType.Success;
                outPath = outputPath;
            }
            catch (ArgumentNullException)
            {
                messages.Append("The image file is empty.");
            }
            catch (ArgumentException)
            {
                messages.Append("The image has the wrong file format.");
            }
            catch (Exception)
            {
                messages.Append("Failed to save image.");
            }

            return new ImageSaveResult(resultType, outPath, messages.ToArray());
        }

        /// <summary>
        /// Возвращает результат операции удаления файла.
        /// </summary>
        /// <param name="imgFileName">Полное имя файла без директории расположения.</param>
        public override OperationResult Delete(string imgFileName)
        {
            if (string.IsNullOrWhiteSpace(imgFileName))
            {
                return new OperationResult(ResultType.Success);
            }

            imgFileName = Path.GetFileName(imgFileName);
            string deleteDirectory = _contentEnvironment.Path + ImagesDirectory;

            if (!Directory.Exists(deleteDirectory))
            {
                return new OperationResult(ResultType.Success);
            }

            string deleteFilePath = deleteDirectory + "\\" + imgFileName;

            if (!File.Exists(deleteFilePath))
            {
                return new OperationResult(ResultType.Success);
            }

            try
            {
                File.Delete(deleteFilePath);
            }
            catch (Exception)
            {
                return new OperationResult(ResultType.Error, "Could not delete image.");
            }

            return new OperationResult(ResultType.Success);
        }
    }
}