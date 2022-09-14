using AutoMapper;
using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountViolationListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public string CategoryName { get; set; }
        public ViolationCategory Category { get; set; }
        public string TypeName { get; set; }
        public string HelpUrl { get; set; }
        public string CustomHelpUrl { get; set; }
        public ViolationActions Actions { get; set; }
        public ViolationActions CustomActions { get; set; }

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
        public ViolationActions DisplayActions
        {
            get
            {
                if (CustomActions!=0)
                {
                    return CustomActions;
                }
                else
                {
                    return Actions;
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
    }
}
