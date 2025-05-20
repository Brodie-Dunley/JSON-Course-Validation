/*
 * Name: Brodie Dunley
 * Description: This will house the Course Class that will be used in the main program. The only required attribute is the Code and Evaluations. Code represents the course code. While the Evaluations list will house the array of evaluations for each course.
 * Current Date: May 19,2025
 * Due Date: June 6, 2025
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JSONCourseProgram
{
    
    
    internal class Course
    {
       
        public string Code { get; set; } = "";

        public double TotalMarks { get; set; } = 0.0;

        public double MaxMarks { get; set; } = 0.0;

        public double GradePercentage { get; set; } = 0.0;

        public List<Evaluation> Evaluations { get; set;} = new List<Evaluation>();

     
        public void AddEvaluation(Evaluation evaluations)
        {
            Evaluations.Add(evaluations);
        }

        public void CalculateTotalMarks(Course course)
        {
            double sum = 0.0;
            foreach (var eval in course.Evaluations)
            {
                sum += eval.EarnedMarks;
            }
            course.TotalMarks = sum;
        }
        public void CalculateOutOfTotal(Course course)
        {
            double sum = 0.0;
            foreach (var eval in course.Evaluations)
            {
                sum += eval.OutOf;
            }
            course.MaxMarks = sum;
        }
        public void GetStudentPercentage(Course course) 
        {
            course.GradePercentage = Math.Round(((course.TotalMarks / course.MaxMarks) * 100),2);
        }
    }
}
