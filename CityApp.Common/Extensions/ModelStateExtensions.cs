using CityApp.Common.Enums;
using CityApp.Common.Models;
using CityApp.Common.Serialization;
using CityApp.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Extensions
{
    public static class ModelStateExtensions
    {

        public static List<Error> ToErrors(this ModelStateDictionary modelState)
        {

            var result = new List<Error>();

            result.AddRange(modelState.Values.SelectMany(e => e.Errors.Select(m => new Error { Code = ErrorCode.None, Message = m.ErrorMessage })).ToList());

            return result;
        }

        public static Error ToError(this string error)
        {
            return new Error { Code = ErrorCode.None, Message = error };
        }
    }

    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Creates a 400 Bad Request ContentResult whose body is the JSON-serialized ModelStateDictionary.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static ContentResult ToJsonErrorResult(this ModelStateDictionary modelState)
        {
            Check.NotNull(modelState, nameof(modelState));

            return CreateContentResult(modelState);
        }

        /// <summary>
        /// Creates a 400 Bad Request ContentResult whose body is the JSON-serialized ModelStateDictionary.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="additionalErrors"></param>
        /// <returns></returns>
        public static ContentResult ToJsonErrorResult(this ModelStateDictionary modelState, ICollection<string> additionalErrors)
        {
            Check.NotNull(modelState, nameof(modelState));
            Check.NotNull(additionalErrors, nameof(additionalErrors));

            if (additionalErrors != null)
            {
                foreach (var error in additionalErrors)
                {
                    modelState.AddModelError(string.Empty, error);
                }
            }

            return CreateContentResult(modelState);
        }

        /// <summary>
        /// Creates a 400 Bad Request ContentResult whose body is the JSON-serialized ModelStateDictionary.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="additionalErrors"></param>
        /// <returns></returns>
        public static ContentResult ToJsonErrorResult(this ModelStateDictionary modelState, ICollection<KeyValuePair<string, string>> additionalErrors)
        {
            Check.NotNull(modelState, nameof(modelState));
            Check.NotNull(additionalErrors, nameof(additionalErrors));

            if (additionalErrors != null)
            {
                foreach (var kvp in additionalErrors)
                {
                    modelState.AddModelError(kvp.Key, kvp.Value);
                }
            }

            return CreateContentResult(modelState);
        }

        /// <summary>
        /// Creates a 400 Bad Request ContentResult whose body is the JSON-serialized ModelStateDictionary.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="additionalErrors"></param>
        /// <returns></returns>
        public static ContentResult ToJsonErrorResult(this ModelStateDictionary modelState, ICollection<(string key, string errorMessage)> additionalErrors)
        {
            Check.NotNull(modelState, nameof(modelState));
            Check.NotNull(additionalErrors, nameof(additionalErrors));

            if (additionalErrors != null)
            {
                foreach (var ae in additionalErrors)
                {
                    modelState.AddModelError(ae.key, ae.errorMessage);
                }
            }

            return CreateContentResult(modelState);
        }

        /// <summary>
        /// Only keep the Errors property, and of the errors, only keep the ErrorMessage.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static Dictionary<string, SlimModelStateEntry> ToSlimModelStateDictionary(this ModelStateDictionary modelState)
        {
            Check.NotNull(modelState, nameof(modelState));

            return modelState
                .ToDictionary
                (
                    kvp => kvp.Key,
                    kvp =>
                    {
                        return new SlimModelStateEntry
                        {
                            Errors = kvp.Value.Errors
                                .Select(e => new SlimError { ErrorMessage = e.ErrorMessage })
                                .ToArray()
                        };
                    }
                );
        }

        private static ContentResult CreateContentResult(ModelStateDictionary modelState)
        {
            // Since the vast majority of forms on this site use Pascal casing to match variable names, serialize
            //   model state in Pascal case so that we can work with the client side variable names.
            var content = JsonNet.Serialize(ToSlimModelStateDictionary(modelState), camelCase: false, ignoreReferenceLoop: true);

            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = 400
            };
        }
    }

}
