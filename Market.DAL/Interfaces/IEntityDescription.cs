using System.Collections.Generic;
using System.Reflection;

namespace Market.DAL.Interfaces
{
    public interface IEntityDescription<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Словарь: невложенное свойство - полный SQL идентификатор свойства.
        /// </summary>
        IReadOnlyDictionary<PropertyInfo, string> EntityProperties { get; }

        /// <summary>
        /// Идентификатор SQL указанной сущности.
        /// </summary>
        string EntityId { get; }

        /// <summary>
        /// Словарь: имя сущности - кортеж описания подключаемых SQL сущностей.
        /// </summary>
        IReadOnlyList<(string EntityId, string PKey, string FKey)> JoinedEntitiesInfo { get; }

        /// <summary>
        /// Словарь: вложенное свойство - полный SQL идентификатор свойства.
        /// </summary>
        IReadOnlyDictionary<PropertyInfo, string> JoinedEntitiesProperties { get; }
    }
}