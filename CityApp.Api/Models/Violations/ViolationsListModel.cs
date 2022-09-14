using AutoMapper;
using CityApp.Data.Enums;
using CityApp.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class ViolationsListModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public string CategoryName { get; set; }
        public string TypeName { get; set; }
        public string HelpUrl { get; set; }
        public string CustomHelpUrl { get; set; }
        public string Code { get; set; }
        public double? Fee { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationActions Actions { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationActions CustomActions { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationRequiredFields RequiredFields { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationRequiredFields CustomRequiredFields { get; set; }

        public string Description { get; set; }
        public string CustomDescription { get; set; }

        public int? ReminderMinutes { get; set; }

        public string ReminderMessage { get; set; }

        public string DisplayDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomDescription))
                {
                    return Description;
                }
                else
                {
                    return CustomDescription;
                }
            }
        }

        [IgnoreMap]
        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationRequiredFields DisplayRequiredFields
        {
            get
            {
                if (CustomRequiredFields == 0)
                {
                    return RequiredFields;
                }
                else
                {
                    return CustomRequiredFields;
                }
            }
        }


        /// <summary>
        /// Display Name instead of CustomName if it is null or empty
        /// </summary>
        [IgnoreMap]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomName))
                {
                    return Name;
                }
                else
                {
                    return CustomName;
                }
            }
        }

        /// <summary>
        /// Display Actions instead of CustomActions if it is null or empty
        /// </summary>
        [IgnoreMap]
        [JsonConverter(typeof(StringEnumConverter))]
        public ViolationActions DisplayActions
        {
            get
            {
                if (CustomActions == 0)
                {
                    return Actions;
                }
                else
                {
                    return CustomActions;
                }
            }
        }

        /// <summary>
        /// Display HelpUrl instead of CustomHelpUrl if it is null or empty
        /// </summary>
        [IgnoreMap]
        public string DisplayHelpUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomHelpUrl))
                {
                    return HelpUrl;
                }
                else
                {
                    return CustomHelpUrl;
                }
            }
        }

        public List<ViolationQuestionsListItem> ViolationQuestion { get; set; }


    }
}
