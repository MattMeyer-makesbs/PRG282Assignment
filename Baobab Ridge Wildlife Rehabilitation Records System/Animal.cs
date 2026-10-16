using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public class Animal
    {
        public Animal(string iD, string name, string species, int age, int recoveryScore, string status, string housingUnit)
        {
            ID = iD;
            Name = name;
            Species = species;
            Age = age;
            RecoveryScore = recoveryScore;
            Status = status;
            HousingUnit = housingUnit;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
        public string Status { get; set; }
        public string HousingUnit { get; set; }
    }
}
