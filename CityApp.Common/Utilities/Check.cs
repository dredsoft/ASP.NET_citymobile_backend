using System;
using System.Collections.Generic;
using System.Linq;

namespace CityApp.Common.Utilities
{
    /// <summary>
    /// Argument checker so that we don't have to keep writing the same guard clauses all over the place.
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if the value is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void NotNull<T>(T value, string parameterName)
        {
            if (ReferenceEquals(value, null))
            {
                NotEmpty(parameterName, nameof(value));

                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the string is null, or an <see cref="ArgumentException"/> if the
        /// string is null or white space.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void NotEmpty(string value, string parameterName)
        {
            Exception ex = null;

            if (ReferenceEquals(value, null))
            {
                ex = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                ex = new ArgumentException($"{parameterName} is empty or white space");
            }

            if (ex != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw ex;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the collection is null, or an <see cref="ArgumentException"/> if
        /// the collection is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException($"Collection argument is empty", parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the collection is null, or an <see cref="ArgumentException"/> if
        /// the collection has one or more null values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void HasNoNulls<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Any(v => v == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException("Collection has one or more null values", parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the collection is null, or an <see cref="ArgumentException"/> if
        /// the collection has one or more null or white space strings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        public static void HasNoEmpties(IReadOnlyList<string> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Any(v => string.IsNullOrWhiteSpace(v)))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException("Collection of strings has one or more null or white space values", parameterName);
            }
        }
    }
}
