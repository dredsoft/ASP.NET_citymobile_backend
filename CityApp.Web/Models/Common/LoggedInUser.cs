using System;

namespace CityApp.Web.Models.Common
{
    public class LoggedInUser
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public override string ToString() => $"{FullName} ({Email})";

        public string LastSession { get; set; }
    }
}
