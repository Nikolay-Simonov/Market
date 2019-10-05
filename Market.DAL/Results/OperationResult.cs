using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Market.DAL.Enums;

namespace Market.DAL.Results
{
    public class OperationResult : IEnumerable<string>
    {
        public OperationResult(ResultType resultType, params string[] messages)
        {
            Type = resultType;
            Messages = messages;
        }

        public ResultType Type { get; }

        public IEnumerable<string> Messages { get; }

        public IEnumerator<string> GetEnumerator() => Messages.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Messages.GetEnumerator();

        /// <summary>
        /// Объединяет все <see cref="Messages"/> в одно сообщение.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildMessage()
        {
            if (Messages == null || !Messages.Any())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            foreach (var msg in Messages)
            {
                stringBuilder.AppendLine(msg);
            }

            return stringBuilder.ToString();
        }
    }
}