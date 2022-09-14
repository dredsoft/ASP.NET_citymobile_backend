using CityApp.Data.Enums;
using System;

namespace CityApp.Common.Models
{
    public class LoggedInUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                {
                    return Email;
                }
                else
                {
                    return $"{FirstName} {LastName}";
                }
            }
        }

        public override string ToString() => $"{FullName} ({Email})";

        public SystemPermissions Permission { get; set; }

        public string LastSession { get; set; }
    }
}
