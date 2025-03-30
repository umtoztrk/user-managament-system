using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop2_proje_testProject2
{
    public class Record
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }

        public string toCsv()
        {
            return $"{Owner},{Name},{Surname},{Phone},{Address},{Description},{Email}";
        }

        public static Record fromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            return new Record
            {
                Owner = values[0],
                Name = values[1],
                Surname = values[2],
                Phone = values[3],
                Address = values[4],
                Description = values[5],
                Email = values[6]
            };
        }
    }
}
