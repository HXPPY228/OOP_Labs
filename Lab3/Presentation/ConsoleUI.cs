using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class ConsoleUI
    {
        private readonly StudentService _studentService;

        public ConsoleUI(StudentService studentService)
        {
            _studentService = studentService;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\nStudent Record Management System");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Edit Student");
                Console.WriteLine("3. View Students");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        EditStudent();
                        break;
                    case "3":
                        var viewCommand = new ViewStudentsCommand(_studentService);
                        viewCommand.Execute();
                        break;
                    case "4":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void AddStudent()
        {
            Console.Write("Enter name: ");
            var name = Console.ReadLine();
            Console.Write("Enter grade (0-100): ");
            if (int.TryParse(Console.ReadLine(), out int grade))
            {
                var studentDto = new StudentDTO { Name = name, Grade = grade };
                var command = new AddStudentCommand(_studentService, studentDto);
                command.Execute();
            }
            else
            {
                Console.WriteLine("Invalid grade. Please enter a number between 0 and 100.");
            }
        }

        private void EditStudent()
        {
            Console.Write("Enter student ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Enter new name: ");
                var name = Console.ReadLine();
                Console.Write("Enter new grade (0-100): ");
                if (int.TryParse(Console.ReadLine(), out int grade))
                {
                    var studentDto = new StudentDTO { Name = name, Grade = grade };
                    var command = new EditStudentCommand(_studentService, id, studentDto);
                    command.Execute();
                }
                else
                {
                    Console.WriteLine("Invalid grade. Please enter a number between 0 and 100.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid number.");
            }
        }
    }
}
