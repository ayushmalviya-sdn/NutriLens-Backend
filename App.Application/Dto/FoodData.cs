using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Dto
{
    public class FoodData
    {
        public string FoodName { get; set; }
        public string FoodCategory { get; set; }
        public string ServingSize { get; set; }
        public Macros Macros { get; set; }
        public double CaloriesPerServing { get; set; }
    }

    public class Macros
    {
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Fiber { get; set; }
        public double Sugar { get; set; }
    }

}
