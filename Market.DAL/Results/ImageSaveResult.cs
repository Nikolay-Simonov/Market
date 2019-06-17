using Market.DAL.Enums;

namespace Market.DAL.Results
{
    public class ImageSaveResult : OperationResult
    {
        public ImageSaveResult(ResultType resultType,
            string outputPath, params string[] messages) : base(resultType, messages)
        {
            OutputPath = outputPath;
        }

        public string OutputPath { get; }
    }
}