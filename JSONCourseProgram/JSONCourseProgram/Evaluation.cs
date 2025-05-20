/*
 * Name: Brodie Dunley
 * Description: This will house the Evaluation Class that will be used in the main program. This class will represent the actuall evaluation from the JSON file and has attributes to show the students marks (EarnedMarks), the weight of each assignment, what the assignment is out of (So that we can calculate the percentage as well weighted course marks the student got) and the description of what the evaluation is
 * Current Date: May 19,2025
 * Due Date: June 6, 2025
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONCourseProgram
{
    public class Evaluation
    {

        public string Description { get; set; } = "";

        public double Weight { get; set; } = 0.0;

        public double OutOf { get; set; } = 0.0;

        public double EarnedMarks { get; set; } = 0.0;

        public double GetCourseMarks()
        {
            double percent = this.EarnedMarks / this.OutOf;
            return Math.Ceiling(this.Weight * percent);
        }

        public double EvalPercentage()
        {
            return Math.Round(((this.EarnedMarks / this.OutOf) * 100), 2);
        }
    }
}
