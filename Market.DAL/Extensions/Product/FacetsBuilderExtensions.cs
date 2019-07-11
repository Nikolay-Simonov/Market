using System.Collections.Generic;
using System.Linq;
using Market.DAL.Interfaces;

namespace Market.DAL.Extensions.Product
{
    public static class FacetsBuilderExtentions
    {
        public static IFacetsBuilder<Entities.Product> AndCharacteristics(this IFacetsBuilder<Entities.Product> facetsBuilder,
            IDictionary<string, HashSet<string>> characteristics)
        {
            if (characteristics == null)
            {
                return facetsBuilder;
            }

            characteristics = characteristics
                .Where(c =>
                    !string.IsNullOrWhiteSpace(c.Key) && c.Value.Any(v => !string.IsNullOrWhiteSpace(v))
                )
                .Select(c => new KeyValuePair<string, HashSet<string>>
                    (
                        c.Key,
                        c.Value.Where(v => !string.IsNullOrWhiteSpace(v)).ToHashSet()
                    )
                )
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (!characteristics.Any())
            {
                return facetsBuilder;
            }

            string propertyId = facetsBuilder.EntityDescription
                .EntityProperties[typeof(Entities.Product).GetProperty(nameof(Entities.Product.Character))];

            facetsBuilder.AppendText(" AND ")
                .AppendText(propertyId)
                .AppendText(".value('count(for $char in //characteristic where ");
            int count = 0;
            int lastIndex = characteristics.Count - 1;

            foreach (var (name, values) in characteristics)
            {
                facetsBuilder.AppendCharacteristicName(name);
                facetsBuilder.AppendCharacteristicIn(values);

                if (count >= lastIndex)
                {
                    break;
                }

                facetsBuilder.AppendText(" and ");
                count++;
            }

            facetsBuilder.AppendText(" return 0)', 'int') > 0");

            return facetsBuilder;
        }

        private static void AppendCharacteristicName(this IFacetsBuilder<Entities.Product> facetsBuilder, string name)
        {
            string paramName = facetsBuilder.GetParamName();
            facetsBuilder.AppendText("$char/name = sql:variable(\"");
            facetsBuilder.AppendText(paramName);
            facetsBuilder.AppendText("\")");
            facetsBuilder.AddParameter(paramName, name);
        }

        private static void AppendCharacteristicIn(this IFacetsBuilder<Entities.Product> facetsBuilder,
            ICollection<string> values)
        {
            facetsBuilder.AppendText(" and $char/value = (");
            int count = 0;
            int lastIndex = values.Count - 1;

            foreach (string value in values)
            {
                string paramName = facetsBuilder.GetParamName();
                facetsBuilder.AppendText("sql:variable(\"");
                facetsBuilder.AppendText(paramName);
                facetsBuilder.AppendText("\")");
                facetsBuilder.AddParameter(paramName, value);

                if (count >= lastIndex)
                {
                    break;
                }

                facetsBuilder.AppendText(",");
                count++;
            }

            facetsBuilder.AppendText(")");
        }
    }
}