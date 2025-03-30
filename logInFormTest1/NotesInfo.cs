using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logInFormTest1
{
    public class NotesInfo
    {
        public string Owner { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        public string ToCsv()
        {
            return $"{Owner},{Date},{Note}";
        }

        public static NotesInfo FromCsv(string csv)
        {
            var values = csv.Split(',');
            return new NotesInfo()
            {
                Owner = values[0],
                Date = DateTime.Parse(values[1]),
                Note = values[2]
            };
        }
    }
}


