using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CityApp.Web.Infrastructure.DataAnnotations
{
    /// <summary>
    /// <para>Validate that the <see cref="DateTime"/>'s year is in the range [DateTime.UtcNow.Year - YearsAgo, DateTime.UtcNow.Year + YearsAhead].</para>
    /// <para>ErrorMessage format string parameters:</para>
    /// <list type="bullet">
    /// <item><description>{0}: the validated <see cref="DateTime"/> property's Year.</description></item>
    /// <item><description>{1}: DateTime.UtcNow.Year - YearsAgo</description></item>
    /// <item><description>{2}: DateTime.UtcNow.Year + YearsAhead</description></item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class YearRangeAttribute : ValidationAttribute
    {
        private const string DEFAULT_ERROR_MESSAGE = "Invalid Year '{0}'. Year must be >= {1} and =< {2}.";

        /// <summary>
        /// Year must be &gt;= <see cref="DateTime.UtcNow.Year"/> - YearsAgo.
        /// </summary>
        public int YearsAgo { get; private set; }

        /// <summary>
        /// Year must be &lt;= <see cref="DateTime.UtcNow.Year" /> + YearsAhead.
        /// </summary>
        public int YearsAhead { get; private set; }

        public YearRangeAttribute(int yearsAgo, int yearsAhead)
        {
            if (yearsAgo < 0) { throw new ArgumentException($"{nameof(yearsAgo)} must be >= 0", nameof(yearsAgo)); }
            if (yearsAhead < 0) { throw new ArgumentException($"{nameof(yearsAhead)} must be >= 0", nameof(yearsAhead)); }

            YearsAgo = yearsAgo;
            YearsAhead = yearsAhead;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expires = (DateTime?)value;
            if (expires == null)
            {
                // This is not a required field validator.
                return ValidationResult.Success;
            }

            // Azure's servers are set to UTC, so just use UTC.
            var minYear = DateTime.UtcNow.Year - YearsAgo;
            var maxYear = DateTime.UtcNow.Year + YearsAhead;

            if (expires.Value.Year < minYear || expires.Value.Year > maxYear)
            {
                var errMsg = ErrorMessageString ?? DEFAULT_ERROR_MESSAGE;
                var formattedErrMsg = string.Format(CultureInfo.CurrentCulture, errMsg, expires.Value.Year, minYear, maxYear);

                return new ValidationResult(formattedErrMsg, new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
