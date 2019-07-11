using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Market.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.EF
{
    public class EFEntityDescription<TEntity> : IEntityDescription<TEntity> where TEntity : class, new()
    {
        public EFEntityDescription()
        {
            Type entityType = typeof(TEntity);

            // Словарь: имя сущности - SQL идентификатор сущности извлеченные из контекста приложения.
            Dictionary<string, string> entitiesIds = typeof(ApplicationDbContext).GetProperties().Where(p =>
            {
                return p.PropertyType.IsGenericType
                       && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>);
            }).ToDictionary(p => p.PropertyType.GenericTypeArguments.First().Name, p => GetEntityId(p.Name));

            // Получаем идентификатор SQL указанной сущности из словаря tableNames или атрибута Table.
            TableAttribute tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
            EntityId = tableAttribute == null || string.IsNullOrWhiteSpace(tableAttribute.Name)
                ? entitiesIds[entityType.Name]
                : GetEntityId(tableAttribute.Name);

            // Массив вложенных и невложенных свойств текущей сущности.
            PropertyInfo[] allowedProperties = entityType.GetProperties().Where(p =>
            {
                return !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                       && p.GetMethod.IsPublic
                       && !p.GetMethod.IsAbstract
                       && !p.GetMethod.IsStatic;
            }).ToArray();

            // Список ключей: невложенное свойство - SQL идентификатор свойства текущей сущности.
            EntityProperties = allowedProperties.Where(p =>
            {
                return (p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
                       && !p.GetMethod.IsVirtual;
            })
            .ToDictionary(p => p, p =>
            {
                var columnAttribute = p.GetCustomAttribute<ColumnAttribute>();
                string propertyName = columnAttribute == null || string.IsNullOrWhiteSpace(columnAttribute.Name)
                    ? p.Name
                    : columnAttribute.Name;

                return GetPropertyId(EntityId, propertyName);
            });

            // Подключаемые сущности.
            Type[] joinedEntities = allowedProperties.Where(p =>
            {
                return p.GetMethod.IsVirtual && entitiesIds.ContainsKey(p.PropertyType.Name)
                       && p.PropertyType.IsClass
                       && !typeof(IEnumerable).IsAssignableFrom(p.PropertyType)
                       && p.PropertyType.GetProperties().Any(np =>
                       {
                           return !Attribute.IsDefined(np, typeof(NotMappedAttribute))
                                  && (np.PropertyType.IsValueType || np.PropertyType == typeof(string))
                                  && np.GetMethod.IsPublic
                                  && !np.GetMethod.IsAbstract
                                  && !np.GetMethod.IsStatic;
                       });
            })
            .Select(p => p.PropertyType).ToArray();

            // Список ключей: вложенное свойство - SQL идентификатор свойства текущей сущности.
            var joinedEntitiesProperties = new List<KeyValuePair<PropertyInfo, string>>();

            foreach (Type joinedEntity in joinedEntities)
            {
                // Получаем идентификатор SQL подключаемой сущности из словаря tableNames или атрибута Table.
                TableAttribute joinedTableAttribute = joinedEntity.GetCustomAttribute<TableAttribute>();
                string joinedEntityId =
                    joinedTableAttribute == null || string.IsNullOrWhiteSpace(joinedTableAttribute.Name)
                        ? entitiesIds[joinedEntity.Name]
                        : GetEntityId(joinedTableAttribute.Name);

                // Список ключей: невложенное свойство - идентификатор SQL свойства подключаемой сущности.
                IEnumerable<KeyValuePair<PropertyInfo, string>> joinedEntityProperties = joinedEntity.GetProperties()
                .Where(p =>
                {
                    return !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                           && p.GetMethod.IsPublic
                           && (p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
                           && !p.GetMethod.IsAbstract
                           && !p.GetMethod.IsStatic
                           && !p.GetMethod.IsVirtual;
                })
                .Select(p =>
                {
                    var columnAttribute = p.GetCustomAttribute<ColumnAttribute>();
                    var propertyName = columnAttribute == null || string.IsNullOrWhiteSpace(columnAttribute.Name)
                        ? p.Name
                        : columnAttribute.Name;
                    propertyName = GetPropertyId(joinedEntityId, propertyName);

                    return new KeyValuePair<PropertyInfo, string>(p, propertyName);
                });

                // Добавляем пары ключ - значение невложенных свойств подключаемой сущности
                // к вложенным свойствам указанной в параметре типе сущности.
                joinedEntitiesProperties.AddRange(joinedEntityProperties);
            }

            JoinedEntitiesProperties = joinedEntitiesProperties.ToDictionary(p => p.Key, p => p.Value);

            // Список кортежей описания подключаемой SQL сущности.
            JoinedEntitiesInfo = joinedEntities.Select(e =>
            {
                string PKey = JoinedEntitiesProperties[e.GetProperties().Single(p =>
                {
                    return Attribute.IsDefined(p, typeof(KeyAttribute)) ||
                           p.Name.Contains($"{e.Name}id", StringComparison.OrdinalIgnoreCase);
                })];

                string FKey = EntityProperties.Single(p =>
                    p.Key.Name.Contains($"{e.Name}Id", StringComparison.OrdinalIgnoreCase)
                ).Value;

                return (entitiesIds[e.Name], PKey, FKey);
            }).ToList();
        }

        public string EntityId { get; }

        public IReadOnlyDictionary<PropertyInfo, string> EntityProperties { get; }

        public IReadOnlyList<(string EntityId, string PKey, string FKey)> JoinedEntitiesInfo { get; }

        public IReadOnlyDictionary<PropertyInfo, string> JoinedEntitiesProperties { get; }

        private static string GetPropertyId(string entityId, string propertyName)
        {
            Span<char> propertyId = stackalloc char[entityId.Length + propertyName.Length + 3];
            int currentIndex = 0;

            while (currentIndex < entityId.Length)
            {
                propertyId[currentIndex] = entityId[currentIndex];
                currentIndex++;
            }

            propertyId[currentIndex++] = '.';
            propertyId[currentIndex++] = '[';
            int lastIndex = currentIndex;

            while (currentIndex < lastIndex + propertyName.Length)
            {
                propertyId[currentIndex] = propertyName[currentIndex - lastIndex];
                currentIndex++;
            }

            propertyId[currentIndex] = ']';

            return new string(propertyId);
        }

        private static string GetEntityId(string entityName)
        {
            Span<char> tableId = stackalloc char[entityName.Length + 2];
            tableId[0] = '[';
            int currentIndex = 1;


            while (currentIndex < entityName.Length + 1)
            {
                tableId[currentIndex] = entityName[currentIndex - 1];
                currentIndex++;
            }

            tableId[currentIndex] = ']';

            return new string(tableId);
        }
    }
}