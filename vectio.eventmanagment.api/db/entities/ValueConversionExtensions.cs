using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace vectio.eventmanagement.api.db.entities
{
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class, new()
        {
            var settings= new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ValueConverter<T, string> converter = new ValueConverter<T, string>
            (
                v => JsonConvert.SerializeObject(v,settings),
                v => JsonConvert.DeserializeObject<T>(v,settings) ?? new T()
            );

            ValueComparer<T> comparer = new ValueComparer<T>
            (
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r,settings),
                v => v == null ? 0 : JsonConvert.SerializeObject(v,settings).GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v,settings),settings)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            

            return propertyBuilder;
        }
    }
}
