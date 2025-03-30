using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logInFormTest1
{
    public class ReminderInfo
    {
        public string Owner { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string ReminderType { get; set; }
        public string Event { get; set; }
        public string datetime { get; set; }

        public string toCsv()
        {
            return $"{Owner},{Description},{Summary},{Event},{datetime}";
        }

        public static ReminderInfo fromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            return new ReminderInfo
            {
                Owner = values[0],
                Description = values[1],
                Summary = values[2],
                Event = values[3],
                datetime = values[4]
            };
        }
    }
}
