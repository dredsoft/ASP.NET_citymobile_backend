using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CityApp.Common.Utilities
{
    public static class SimpleCopy
    {
        /// <summary>
        /// <para>Copy source's public, writable instance property values into a new instance of T.</para>
        /// <para>Intended to be used to copy non-navigation properties of EF entities (e.g., cloning a contact).</para>
        /// </summary>
        /// <typeparam name="T">Assumed to have a public, parameterless constructor.</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyPublicInstanceProperties<T>(T source)
            where T : class
        {
            // Get the public, writable instance property names and values from source.
            var publicInstancePropertyNamesAndValues = new Dictionary<string, object>();

            foreach (var pi in typeof(T).GetProperties())
            {
                // Can't do anything with read-only properties.
                if (!pi.CanWrite) { continue; }

                // We're only interested in simple types, such as strings, ints, enums, Guids, etc.
                if (!IsSimpleType(pi.PropertyType)) { continue; }

                publicInstancePropertyNamesAndValues[pi.Name] = pi.GetValue(source, null);
            }

            // Create a new instance of T.
            var dest = Activator.CreateInstance<T>();
            
            var destinationProperties = GetPublicInstanceProperties(dest);
            var destinationPropertyNames = destinationProperties.Select(pi => pi.Name).ToArray();

            // If there are any properties on the source object that do not exist on the destination object, throw
            //   an exception. Note that since we're using strictly C# objects and not translating from JSON, we 
            //   want a case-sensitive string comparison.
            var unknownSourcePropertyNames = publicInstancePropertyNamesAndValues.Keys
                .Except(destinationPropertyNames, StringComparer.Ordinal)
                .ToArray();

            if (unknownSourcePropertyNames.Length > 0)
            {
                throw new InvalidOperationException($"Source object has properties that are not present on the destination object of type {typeof(T).FullName}. Unknown source property names: {string.Join(", ", unknownSourcePropertyNames)}");
            }

            foreach (var nameValue in publicInstancePropertyNamesAndValues)
            {
                // We want a case sensitive search since we're dealing with C#, and not, e.g., translating from JSON.
                var destinationProperty = destinationProperties.SingleOrDefault(pi => pi.Name == nameValue.Key);
                if (destinationProperty == null)
                {
                    throw new InvalidOperationException($"Trying to set property {nameValue.Key} on destination type {dest.GetType().FullName}, but property does not exist on destination object.");
                }

                if (!destinationProperty.CanWrite)
                {
                    throw new InvalidOperationException($"Cannot write to destination property {destinationProperty.Name} of type {dest.GetType().FullName}");
                }

                if (!IsSimpleType(destinationProperty.PropertyType))
                {
                    throw new NotSupportedException($"Destination properties implementing non-primitive, non-simple types are not supported. Property: {destinationProperty.Name}; Type: {destinationProperty.PropertyType.FullName}.");
                }

                // Update the destination property value.
                destinationProperty.SetValue(dest, publicInstancePropertyNamesAndValues[nameValue.Key]);
            }

            return dest;
        }

        private static bool IsSimpleType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            if (t.IsPrimitive)
            {
                // The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
                //   Web API will URI-bind primitive types by default.
                //   See: https://msdn.microsoft.com/en-us/library/system.type.isprimitive(v=vs.110).aspx
                return true;
            }

            if (t == typeof(TimeSpan) || t == typeof(DateTime) || t == typeof(decimal) || t == typeof(Guid) || t == typeof(string) || t.IsEnum)
            {
                // Web API will also URI-bind these "simple" types.
                //   See: http://www.asp.net/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api
                return true;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // It's a nullable value type (e.g., int?, decimal?, DateTime?). Web API will URI-bind these.
                return true;
            }

            return false;
        }

        private static PropertyInfo[] GetPublicInstanceProperties<T>(T value)
        {
            return value
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToArray();
        }
    }
}
