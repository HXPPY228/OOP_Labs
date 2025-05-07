using Application;
using Domain;


namespace Presentation
{
    public interface IConsoleCommand
    {
        void Execute();
    }

    public class AddStudentCommand : IConsoleCommand
    {
        private readonly StudentService _studentService;
        private readonly StudentDTO _studentDto;

        public AddStudentCommand(StudentService studentService, StudentDTO studentDto)
        {
            _studentService = studentService;
            _studentDto = studentDto;
        }

        public void Execute()
        {
            try
            {
                var quote = _studentService.AddStudent(_studentDto);
                Console.WriteLine("Student added successfully.");
                Console.WriteLine($"Quote: {quote.Content} - {quote.Author}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class EditStudentCommand : IConsoleCommand
    {
        private readonly StudentService _studentService;
        private readonly int _id;
        private readonly StudentDTO _studentDto;

        public EditStudentCommand(StudentService studentService, int id, StudentDTO studentDto)
        {
            _studentService = studentService;
            _id = id;
            _studentDto = studentDto;
        }

        public void Execute()
        {
            try
            {
                _studentService.EditStudent(_id, _studentDto);
                Console.WriteLine("Student updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class ViewStudentsCommand : IConsoleCommand
    {
        private readonly StudentService _studentService;

        public ViewStudentsCommand(StudentService studentService)
        {
            _studentService = studentService;
        }

        public void Execute()
        {
            var students = _studentService.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("No students found.");
            }
            else
            {
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}, Grade: {student.Grade}");
                }
            }
        }
    }
}