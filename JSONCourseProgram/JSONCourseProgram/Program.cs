/*
 * Name: Brodie Dunley
 * Description: This will be the main method to house the logic for deserializing a JSON File. In this program it will display information on courses and evaluations of those courses. It will show individual evaluations with the students marks that they achieved on the evaluation. This program will allow editing courses and evaluations, as well as adding and deleting courses or evaluations. 
 * Current Date: May 19,2025
 * Due Date: June 6, 2025
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NJsonSchema;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace JSONCourseProgram
{

    internal class Program
    {

        //JSON file to take input from or output to
        private const string jsonFile = "grades.json";

        private const string jsonSchema = "JsonSchema.json";
        static void Main(string[] args)
        {


            List<Course>? courses = new();

            bool newFile;
            if (File.Exists(jsonFile))
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonFile);
                    courses = System.Text.Json.JsonSerializer.Deserialize<List<Course>>(jsonString);
                    Console.WriteLine("Loaded courses from file.");


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file: {ex.Message}");
                    courses = new List<Course>();
                }
            }
            else
            {

                Console.WriteLine("Grades data file grades.json not fond. Create new file? (y/n):  ");
                newFile = "Yy".Contains(Console.ReadKey().KeyChar);
                if (newFile == true)
                {

                    FileStream stream = File.Create(jsonFile);
                    Console.WriteLine("\nNew data set created. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("No new file will be created, exiting...");
                    Environment.Exit(0);
                }

            }

            string userInput = "";
            string jsonSchemaString = File.ReadAllText(jsonSchema);
            do
            {
                Console.Clear();
                displayInformation(courses!);
                userInput = DisplayMenu();

                if (int.TryParse(userInput, out int number))
                {//Call the edit function
                    int userInputNum = int.Parse(userInput) - 1;
                    EditCourseMenu(courses!, userInputNum, jsonSchemaString);
                }
                else if (userInput == "A" || userInput == "a")
                {
                    //Add new Course

                    AddNewCourse(courses!, jsonSchemaString);
                }
                else if (userInput == "X" || userInput == "x")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please Try Again");

                }

            } while (userInput.ToUpper() != "X");

            //Call the method to write to the JSON file
            WriteJSONToFile(courses!, jsonFile);
        }

        //A Method to write all the Course objects back to the JSON File
        static void WriteJSONToFile(List<Course> allCourses, string filePath)
        {
            string json_str = "";
            foreach (Course course in allCourses)
            {
                json_str += JsonConvert.SerializeObject(course);
            }
            File.WriteAllText(filePath, json_str);
        }

        //Method to format the text to the middle of the screen
        static void CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;
            int leftPadding = (windowWidth - text.Length) / 2;
            if (leftPadding < 0)
                leftPadding = 0;

            Console.WriteLine(new String(' ', leftPadding) + text);
        }

        //Method to output the Course, Marks earned, out of and percent
        static void printCourses(List<Course> course)
        {
            //Headers
            Console.WriteLine("{0,-5}{1,-20}{2,-20}{3,-10}{4,-10}", "#.", "Course", "Marks Earned", "Out Of", "Percent");
            Console.WriteLine();

            for (var i = 0; i < course.Count; i++)
            {
                Console.WriteLine("{0,-5}{1,-20}{2,-20:F2}{3,-10:F2}{4,-10:F2}", (i + 1), course[i].Code, course[i].TotalMarks, course[i].MaxMarks, course[i].GradePercentage);
                Console.WriteLine();
            }
        }

        //Method to display the menu
        static string DisplayMenu()
        {

            Console.WriteLine("Press a # from the above list to view/edit.delete a specific course.");
            Console.WriteLine("Press A to add a new course.");
            Console.WriteLine("Press X to quit.");
            Console.WriteLine(new string('-', Console.WindowWidth - 1));
            Console.WriteLine("Enter a command: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        //Method that runs the Edit Course Menu. This will handle if you want to delete the course, edit or add an evaluation.
        static void EditCourseMenu(List<Course> chosenCourse, int Index, string jsonSchema)
        {
            Course course = chosenCourse[Index];
            bool continueMenu = true;
            string userChoice = "";
            do
            {
                //---------------------------------------------------------------------------------------Output formatting---------------------------------------------------------------------------------------
                Console.Clear();
                string headerTitle = "~ GRADES TRACKING SYSTEM ~";
                CenterText(headerTitle);
                Console.WriteLine(new string('-', Console.WindowWidth - 1));

                string currentCourse = course.Code + " Evaluations";
                int windowWidth = Console.WindowWidth;
                int leftPadding = (windowWidth - currentCourse.Length) / 2;
                if (leftPadding < 0)
                    leftPadding = 0;

                Console.WriteLine(new String(' ', leftPadding) + currentCourse);
                Console.WriteLine(new string('-', Console.WindowWidth - 1));


                if (course.Evaluations.Count == 0)
                {
                    Console.WriteLine("There are current no evaluation for " + chosenCourse[Index].Code);
                    Console.WriteLine(new string('-', Console.WindowWidth - 1));
                }
                else
                {

                    Console.WriteLine("{0,-5}{1,-20}{2,-20}{3,-10}{4,-10}{5,-17}{6,-10}", "#.", "Evaluation", "Marks Earned", "Out Of", "Percent", "Course Marks", "Weight/100");

                    Console.WriteLine();

                    for (var i = 0; i < course.Evaluations.Count; i++)
                    {

                        Console.WriteLine("{0,-5}{1,-20}{2,-20:F2}{3,-10:F2}{4,-10:F2}{5,-17:F2}{6,-10:F2}", (i + 1), course.Evaluations[i].Description, course.Evaluations[i].EarnedMarks, course.Evaluations[i].OutOf, course.Evaluations[i].EvalPercentage(), course.Evaluations[i].GetCourseMarks(), course.Evaluations[i].Weight);
                        Console.WriteLine();
                    }
                    Console.WriteLine(new string('-', Console.WindowWidth - 1));
                }


                //----------------------------------End of Output Formatting-------------------------------------------------------------------------------

                //Menu Option output
                Console.WriteLine("Press D to delete this course");
                Console.WriteLine("Press A to add an evaluation");
                Console.WriteLine("Press a # from the above list to edit/delete a specfic evaluation.");
                Console.WriteLine("Press X to return to the main menu.");
                Console.WriteLine(new string('-', Console.WindowWidth - 1));
                Console.Write("Enter a command: ");

                userChoice = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(userChoice, out int number))
                {
                    //Converting the input to an int and then adjusting for the index position
                    int evalIndexNumber = int.Parse(userChoice);
                    evalIndexNumber -= 1;

                    //Call a function that edits a specific evaluation
                    EditEvaluation(chosenCourse[Index].Evaluations, evalIndexNumber, jsonSchema);

                }
                //Else if add an evaluation
                else if (userChoice.ToUpper() == "A")
                {
                    bool continueEval = true;
                    do
                    {
                        Evaluation evaluation = new Evaluation();
                        Console.WriteLine("Enter a description: ");
                        evaluation.Description = Console.ReadLine() ?? "";
                        Console.WriteLine("Enter the 'out of' mark: ");
                        try
                        {
                            //Try catch block 
                            evaluation.OutOf = double.Parse(Console.ReadLine() ?? "");
                            Console.WriteLine("Enter the % weight: ");
                            //Try catch block
                            evaluation.Weight = double.Parse(Console.ReadLine() ?? "");
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Please Re-enter valid information");
                            continue;
                        }


                        Console.WriteLine("Enter marks earned or press ENTER to skip: ");
                        string userInput = Console.ReadLine() ?? "";
                        double doubleInput = 0.0;

                        //Ensuring the input is a double before assigning it to the value. Then adding it to the Earned Marks attribute
                        if (!string.IsNullOrEmpty(userInput))
                        {
                            if (double.TryParse(userInput, out double parsedValue))
                            {
                                doubleInput = parsedValue;
                                evaluation.EarnedMarks = doubleInput;
                            }
                            else
                                break;
                        }
                        course.AddEvaluation(evaluation);

                        int evaluationCount = course.Evaluations.Count;
                        int addedEvalIndex = evaluationCount - 1;

                        string jsonToCheck = JsonConvert.SerializeObject(course);
                        IList<string> messages;

                        if (ValidateCourseData(jsonToCheck, jsonSchema, out messages))
                        {
                            Console.WriteLine("Evaluation added succesfully");
                            continueEval = false;
                        }
                        else
                        {
                            //If it wasn't valid, remove it from the Evaluations
                            Console.WriteLine($"\nERROR:\tInvalid Evaluation.\n");
                            course.Evaluations.RemoveAt(addedEvalIndex);
                        }
                    } while (continueEval);


                }
                //Else if delete option
                else if (userChoice.ToUpper() == "D")
                {
                    Console.WriteLine("Delete " + course.Code + "? (y/n):");

                    string deleteChoice = Console.ReadLine() ?? "";
                    if (deleteChoice.ToUpper() == "Y")
                    {
                        chosenCourse.RemoveAt(Index);
                        Console.WriteLine("Returning to main menu");
                        continueMenu = false;
                    }
                    else
                        Console.WriteLine("Won't delete course");

                }
                else if (userChoice.ToUpper() == "X")
                {
                    Console.WriteLine("Returning to main menu");
                    continueMenu = false;
                }
                else
                {
                    Console.WriteLine("Invalid choice, please try again");
                }
            } while (continueMenu);
        }

        //Method to check the JSON is valid 
        //Check if it meets the schema. If it doesn't it will produce errors. It will return the amount of errors encountered. If it is 0 then it is valid and can be added. Otherwise no try again
        public static bool ValidateCourseData(string json_data, string json_schema, out IList<string> messages)
        {
            JSchema schema = JSchema.Parse(json_schema);
            JObject course = JObject.Parse(json_data);
            return course.IsValid(schema, out messages);
        }


        //Method to take in an evaluation perform 1 of 2 things. Either delete the evaluation or edit it. If neither of these two things, exit. 
        public static void EditEvaluation(List<Evaluation> eval, int index, string json_Schema)
        {

            bool continueEditMenu = true;

            do
            {  //Header of the output
                Console.Clear();
                Console.WriteLine(new string('-', Console.WindowWidth - 1));
                string header = eval[index].Description;
                CenterText(header);
                Console.WriteLine(new string('-', Console.WindowWidth - 1));

                //Display the contents of the current evaluation

                Console.WriteLine("{0,-15}{1,-8}{2,-8}{3,-15}{4,-10}", "Marks Earned", "Out of", "Percent", "CourseMarks", "Weight/100");

                Console.WriteLine("{0,-15:F2}{1,-8:F2}{2,-8:F2}{3,-15:F2}{4,-10:F2}", eval[index].EarnedMarks, eval[index].OutOf, eval[index].EvalPercentage(), eval[index].GetCourseMarks(), eval[index].Weight);
                Console.WriteLine(new string('-', Console.WindowWidth - 1));

                //Display options
                Console.WriteLine("Press D to delete this evaluation");
                Console.WriteLine("Press E to edit this evaluation");
                Console.WriteLine("Press X to return to the previous menu.");
                Console.WriteLine(new string('-', Console.WindowWidth - 1));

                Console.WriteLine("Enter a command: ");
                string userInput = Console.ReadLine() ?? "".ToUpper();
                switch (userInput)
                {
                    case "D":
                        bool continueLoop = true;
                        do
                        {
                            Console.WriteLine("Do you want to delete this evaluation? (Y/N)");
                            string deleteChoice = Console.ReadLine() ?? "".ToUpper();


                            if (deleteChoice == "Y")
                            {
                                eval.RemoveAt(index);
                                continueLoop = false;
                            }
                            else if (deleteChoice == "N")
                            {
                                Console.WriteLine("Exiting back to Evaluation Menu");
                                continueLoop = false;
                            }
                            else
                            {
                                Console.WriteLine("Not a valid choice, please try again");
                            }
                        } while (continueLoop);
                        break;
                    case "E":
                        bool continueLoopE = true;
                        do
                        {
                            Console.WriteLine("Enter marks earned out of " + eval[index].OutOf + ", press Enter to leave unassigned: ");
                            string userMarksInput = Console.ReadLine() ?? "";
                            if (string.IsNullOrWhiteSpace(userMarksInput))
                                continueLoopE = false;
                            else if (!string.IsNullOrEmpty(userMarksInput))
                            {
                                if (int.TryParse(userMarksInput, out int parsedValue))
                                {
                                    int intInput = parsedValue;
                                    eval[index].EarnedMarks = intInput;
                                    string jsonToCheck = JsonConvert.SerializeObject(eval[index]);
                                    IList<string> messages;

                                    if (ValidateCourseData(jsonToCheck, json_Schema, out messages) == true)
                                    {
                                        Console.WriteLine("Evaluation edited sucessfully.");
                                        continueLoopE = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\nERROR:\tInvalid Marks Earned Entered. Please enter a value >= 0 AND <= {eval[index].OutOf}\n");
                                    }

                                }
                            }
                        } while (continueLoopE);
                        break;
                    case "X":
                        continueEditMenu = false;
                        break;
                }
            } while (continueEditMenu);

        }

        //Method to add a new Course
        public static void AddNewCourse(List<Course> coursesList, string jsonSchema)
        {
            bool continueInput = true;
            do
            {

                Console.WriteLine("Enter a Course Code: ");
                string newCourseCode = Console.ReadLine() ?? "";

                Course newCourse = new Course { Code = newCourseCode };
                string jsonToCheck = JsonConvert.SerializeObject(newCourse);
                IList<string> messages;


                if (ValidateCourseData(jsonToCheck, jsonSchema, out messages) == true)
                {
                    coursesList.Add(newCourse);
                    continueInput = false;
                }
                else
                {
                    Console.WriteLine($"\nERROR:\tInvalid Course Code.\n");

                }
            } while (continueInput);


        }

        //Method to display the header title and information
        public static void displayInformation(List<Course> courses)
        {
            string headerTitle = "~ GRADES TRACKING SYSTEM ~";
            CenterText(headerTitle);
            Console.WriteLine(new string('-', Console.WindowWidth - 1));

            string GradesSummaryTitle = "Grades Summary";
            CenterText(GradesSummaryTitle);

            Console.WriteLine(new string('-', Console.WindowWidth - 1));

            if (courses == null || courses.Count == 0)
                Console.WriteLine("There are currently no saved courses.");
            else
            {
                foreach (Course course in courses)
                {
                    course.CalculateTotalMarks(course);
                    course.CalculateOutOfTotal(course);
                    course.GetStudentPercentage(course);
                }
                printCourses(courses);
                Console.WriteLine(new string('-', Console.WindowWidth - 1));
            }
        }
    }


}
