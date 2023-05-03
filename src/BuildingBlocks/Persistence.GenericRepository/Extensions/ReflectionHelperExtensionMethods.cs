using Persistence.GenericRepository.Base;

namespace Persistence.GenericRepository.Extensions
{
    public static class ReflectionHelperExtensionMethods
    {
        public static void SetPrivatePropertyValue<TEntity, TId, TValue>(this TEntity entity, string propertyName, TValue value) where TEntity : BaseEntity<TId>
        {
            var propertyInfo = typeof(TEntity).GetProperty(propertyName);
            if (propertyInfo != null) return;
            propertyInfo?.SetValue(entity, value);
        }
    }
}
