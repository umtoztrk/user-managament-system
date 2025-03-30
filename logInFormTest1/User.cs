using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Information
{
    public class User
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }

        public string Salary { get; set; }

        public string Photo { get; set; }

        public string UserType { get; set; }

        public string ToCsv()
        {
            return $"{Name},{Surname},{Phone},{Email},{Address},{Password},{Salary},{UserType},{Photo}";
        }

        public static User FromCsv(string csvLine)
        {
            var values = csvLine.Split(',');
            return new User
            {
                Name = values[0],
                Surname = values[1],
                Phone = values[2],
                Email = values[3],
                Address = values[4],
                Password = values[5],
                Salary = values[6],
                UserType = values[7],
                Photo = values[8],
            };


        }
    }
}
