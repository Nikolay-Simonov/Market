using Market.DAL.Enums;
using Market.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Market.DAL.Infrastructure
{
    public class FacetsBuilder<TEntity> : IFacetsBuilder<TEntity> where TEntity : class, new()
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        private readonly StringBuilder _queryBuilder = new StringBuilder();
        private readonly Random _rand = new Random();

        public FacetsBuilder(IEntityDescription<TEntity> entityDescription)
        {
            if (entityDescription == null)
            {
                throw new ArgumentNullException(nameof(entityDescription));
            }

            if (string.IsNullOrWhiteSpace(entityDescription.EntityId))
            {
                throw new ArgumentException(
                    nameof(entityDescription.EntityId) + " can't be null, empty or whitespace.",
                    nameof(entityDescription)
                );
            }

            if (entityDescription.EntityProperties == null || !entityDescription.EntityProperties.Any())
            {
                throw new ArgumentException(
                    $"The \"{nameof(entityDescription.EntityProperties)}\" dictionary "
                        + "contains no elements or is null.",
                    nameof(entityDescription)
                );
            }

            if (entityDescription.JoinedEntitiesInfo != null)
            {
                if (!entityDescription.JoinedEntitiesInfo.Any())
                {
                    throw new ArgumentException(
                        $"The \"{nameof(entityDescription.JoinedEntitiesInfo)}\" list "
                            + "contains no elements.",
                        nameof(entityDescription)
                    );
                }

                if (entityDescription.JoinedEntitiesProperties == null ||
                    !entityDescription.JoinedEntitiesProperties.Any())
                {
                    throw new ArgumentException(
                        $"The \"{nameof(entityDescription.JoinedEntitiesProperties)}\" dictionary "
                            + "contains no elements or is null.",
                        nameof(entityDescription)
                    );
                }
            }

            EntityDescription = entityDescription;
        }

        public IEntityDescription<TEntity> EntityDescription { get; }

        /// <summary>
        /// Параметры параметризированного запроса.
        /// </summary>
        public IReadOnlyDictionary<string, object> Parameters =>
            new ReadOnlyDictionary<string, object>(_parameters);

        /// <summary>
        /// Восстанавливает текс запроса.
        /// </summary>
        public void Clear() => _queryBuilder.Clear();

        public IFacetsBuilder<TEntity> AppendText(string text)
        {
            _queryBuilder.Append(text);

            return this;
        }

        public IFacetsBuilder<TEntity> AddParameter(string paramName, object value)
        {
            _parameters.Add(paramName, value);

            return this;
        }

        public IFacetsBuilder<TEntity> Condition(Expression<Func<TEntity, string>> property,
            string value, Op op = Op.Equal)
        {
            return ValidateLogical(property, value, ConditionType.NoCondition, op);
        }

        public IFacetsBuilder<TEntity> Condition<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty? value, Op op = Op.Equal) where TProperty : struct
        {
            return ValidateLogical(property, value, ConditionType.NoCondition, op);
        }

        public IFacetsBuilder<TEntity> Or(Expression<Func<TEntity, string>> property,
            string value, Op op = Op.Equal)
        {
            return ValidateLogical(property, value, ConditionType.OR, op);
        }

        public IFacetsBuilder<TEntity> Or<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty? value, Op op = Op.Equal) where TProperty : struct
        {
            return ValidateLogical(property, value, ConditionType.OR, op);
        }

        public IFacetsBuilder<TEntity> And(Expression<Func<TEntity, string>> property,
            string value, Op op = Op.Equal)
        {
            return ValidateLogical(property, value, ConditionType.AND, op);
        }

        public IFacetsBuilder<TEntity> And<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty? value, Op op = Op.Equal) where TProperty : struct
        {
            return ValidateLogical(property, value, ConditionType.AND, op);
        }

        public IFacetsBuilder<TEntity> ConditionIn(Expression<Func<TEntity, string>> property,
            ISet<string> values)
        {
            return ValidateIn(property, values, ConditionType.NoCondition);
        }

        public IFacetsBuilder<TEntity> ConditionIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values) where TProperty : struct
        {
            return ValidateIn(property, values, ConditionType.NoCondition);
        }

        public IFacetsBuilder<TEntity> OrIn(Expression<Func<TEntity, string>> property,
            ISet<string> values)
        {
            return ValidateIn(property, values, ConditionType.OR);
        }

        public IFacetsBuilder<TEntity> OrIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values) where TProperty : struct
        {
            return ValidateIn(property, values, ConditionType.OR);
        }

        public IFacetsBuilder<TEntity> AndIn(Expression<Func<TEntity, string>> property,
            ISet<string> values)
        {
            return ValidateIn(property, values, ConditionType.AND);
        }

        public IFacetsBuilder<TEntity> AndIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values) where TProperty : struct
        {
            return ValidateIn(property, values, ConditionType.AND);
        }

        public string GetParamName()
        {
            const int alphabetLength = 26;
            const int utfDigitsStart = 48;
            const int uftDigitsEnd = 58;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Span<char> paramSpan = stackalloc char[5];
            string result;

            do
            {
                paramSpan[0] = '@';
                paramSpan[1] = chars[_rand.Next(0, alphabetLength)];
                paramSpan[2] = chars[_rand.Next(0, alphabetLength)];
                paramSpan[3] = chars[_rand.Next(0, alphabetLength)];
                paramSpan[4] = (char) _rand.Next(utfDigitsStart, uftDigitsEnd);
                result = new string(paramSpan);
            } while (Parameters.Keys.Contains(result));

            return result;
        }

        public override string ToString()
        {
            var selectBuilder = new StringBuilder();

            selectBuilder.Append("SELECT ");

            int lastIndex = EntityDescription.EntityProperties.Count - 1;
            int currentIndex = 0;

            foreach (var (_, value) in EntityDescription.EntityProperties)
            {
                if (currentIndex < lastIndex)
                {
                    selectBuilder.Append(value);
                    selectBuilder.Append(", ");
                    currentIndex++;

                    continue;
                }

                selectBuilder.Append(value);
            }

            selectBuilder.Append(" FROM ");
            selectBuilder.Append(EntityDescription.EntityId);
            selectBuilder.Append(' ');

            if (EntityDescription.JoinedEntitiesInfo != null)
            {
                foreach (var (entityId, pKey, fKey) in EntityDescription.JoinedEntitiesInfo)
                {
                    selectBuilder.Append("LEFT JOIN ");
                    selectBuilder.Append(entityId);
                    selectBuilder.Append(" ON ");
                    selectBuilder.Append(fKey);
                    selectBuilder.Append(" = ");
                    selectBuilder.Append(pKey);
                    selectBuilder.Append(' ');
                }
            }

            if (StringBuilderIsNullOrWhiteSpace(_queryBuilder))
            {
                return selectBuilder.ToString();
            }

            selectBuilder.Append("WHERE ");
            selectBuilder.Append(_queryBuilder);

            return selectBuilder.ToString();
        }

        private IFacetsBuilder<TEntity> ValidateLogical(Expression<Func<TEntity, string>> property, string value,
            ConditionType conditionType, Op op)
        {
            return string.IsNullOrWhiteSpace(value)
                ? this
                : AddLogical(property, value, conditionType, op);
        }

        private IFacetsBuilder<TEntity> ValidateLogical<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty? value, ConditionType conditionType, Op op) where TProperty : struct
        {
            return !value.HasValue
                ? this
                : AddLogical(property, value.Value, conditionType, op);
        }

        private IFacetsBuilder<TEntity> ValidateIn(Expression<Func<TEntity, string>> property, ISet<string> values,
            ConditionType conditionType)
        {
            if (values == null)
            {
                return this;
            }

            string[] tempValues = values.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            return tempValues.Length < 1
                ? this
                : AddIn(property, tempValues, conditionType);
        }

        private IFacetsBuilder<TEntity> ValidateIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            ISet<TProperty> values, ConditionType conditionType) where TProperty : struct
        {
            return values == null || !values.Any()
                ? this
                : AddIn(property, values.ToArray(), conditionType);
        }

        private FacetsBuilder<TEntity> AddIn<TProperty>(Expression<Func<TEntity, TProperty>> property,
            IReadOnlyList<TProperty> values, ConditionType conditionType)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (!(property.Body is MemberExpression memberExpression)
                || !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                throw new ArgumentException
                (
                    "The expression must be a property.",
                     nameof(property)
                );
            }

            string propertyId = null;

            if (EntityDescription.EntityProperties.ContainsKey(propertyInfo))
            {
                propertyId = EntityDescription.EntityProperties[propertyInfo];
            }
            else if (EntityDescription.JoinedEntitiesProperties.ContainsKey(propertyInfo))
            {
                propertyId = EntityDescription.JoinedEntitiesProperties[propertyInfo];
            }

            if (string.IsNullOrWhiteSpace(propertyId))
            {
                throw new ArgumentException
                (
                    "The expression must be a public non-virtual instance property " +
                    $"of the \"{EntityDescription.EntityId}\" or nested navigation property.",
                    nameof(property)
                );
            }

            if (conditionType == ConditionType.OR)
            {
                _queryBuilder.Append(" OR ");
            }
            else if (conditionType == ConditionType.AND)
            {
                _queryBuilder.Append(" AND ");
            }
            else if (conditionType == ConditionType.NoCondition)
            {
                _queryBuilder.Append(' ');
            }

            _queryBuilder.Append(propertyId);
            _queryBuilder.Append(" IN (");
            int lastIndex = values.Count - 1;

            for (var i = 0; i < values.Count; i++)
            {
                string paramName = GetParamName();

                if (i < lastIndex)
                {
                    _queryBuilder.Append(paramName);
                    _queryBuilder.Append(',');
                    _parameters.Add(paramName, values[i]);

                    continue;
                }

                _queryBuilder.Append(paramName);
                _parameters.Add(paramName, values[i]);
            }

            _queryBuilder.Append(')');

            return this;
        }

        private FacetsBuilder<TEntity> AddLogical<TProperty>(Expression<Func<TEntity, TProperty>> property,
            TProperty value, ConditionType conditionType, Op op)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (!(property.Body is MemberExpression memberExpression)
                || !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                throw new ArgumentException
                (
                    "The expression must be a property.",
                    nameof(property)
                );
            }

            string propertyId = null;

            if (EntityDescription.EntityProperties.ContainsKey(propertyInfo))
            {
                propertyId = EntityDescription.EntityProperties[propertyInfo];
            }
            else if (EntityDescription.JoinedEntitiesProperties.ContainsKey(propertyInfo))
            {
                propertyId = EntityDescription.JoinedEntitiesProperties[propertyInfo];
            }

            if (string.IsNullOrWhiteSpace(propertyId))
            {
                throw new ArgumentException
                (
                    "The expression must be a public non-virtual instance property " +
                    $"of the \"{EntityDescription.EntityId}\" or nested navigation property.",
                    nameof(property)
                );
            }

            if (conditionType == ConditionType.OR)
            {
                _queryBuilder.Append(" OR ");
            }
            else if (conditionType == ConditionType.AND)
            {
                _queryBuilder.Append(" AND ");
            }
            else if (conditionType == ConditionType.NoCondition)
            {
                _queryBuilder.Append(' ');
            }

            string paramName = GetParamName();
            _queryBuilder.Append(propertyId);
            _queryBuilder.Append(GetOperator(op));
            _queryBuilder.Append(paramName);
            _parameters.Add(paramName, value);

            return this;
        }

        private static bool StringBuilderIsNullOrWhiteSpace(StringBuilder builder)
        {
            if (builder == null)
            {
                return true;
            }

            for (int index = 0; index < builder.Length; ++index)
            {
                if (!char.IsWhiteSpace(builder[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetOperator(Op op)
        {
            switch (op)
            {
                case Op.Equal: return " = ";
                case Op.Greater: return " > ";
                case Op.Less: return " < ";
                case Op.GreaterEqual: return " >= ";
                case Op.LessEqual: return " <= ";
                case Op.NotEqual: return " <> ";
                default: throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }

        private enum ConditionType
        {
            OR,
            AND,
            NoCondition
        }
    }
}