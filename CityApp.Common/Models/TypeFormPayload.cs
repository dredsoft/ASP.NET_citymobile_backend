using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{

    public class TypeFormPayload
    {
        public string event_id { get; set; }
        public string event_type { get; set; }
        public Guid CitatoinId { get; set; }
        public Guid AccountId { get; set; }
        public Form_Response form_response { get; set; }
    }

    public class Form_Response
    {
        public string form_id { get; set; }
        public string token { get; set; }
        public DateTime submitted_at { get; set; }
        public DateTime landed_at { get; set; }
        public Calculated calculated { get; set; }
        public Definition definition { get; set; }
        public Answer[] answers { get; set; }
    }

    public class Calculated
    {
        public int score { get; set; }
    }

    public class Definition
    {
        public string id { get; set; }
        public string title { get; set; }
        public Field[] fields { get; set; }
    }

    public class Field
    {
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string _ref { get; set; }
        public bool allow_multiple_selections { get; set; }
        public bool allow_other_choice { get; set; }
    }

    public class Answer
    {
        public string type { get; set; }
        public string text { get; set; }
        public Field1 field { get; set; }
        public string email { get; set; }
        public string date { get; set; }
        public Choices choices { get; set; }
        public int number { get; set; }
        public bool boolean { get; set; }
        public Choice choice { get; set; }
    }

    public class Field1
    {
        public string id { get; set; }
        public string type { get; set; }
    }

    public class Choices
    {
        public string[] labels { get; set; }
    }

    public class Choice
    {
        public string label { get; set; }
    }

}
