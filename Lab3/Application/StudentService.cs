using DataAccess;
using Domain;

namespace Application
{
    public class StudentFactory
    {
        public Student CreateStudent(string name, int grade)
        {
            return new Student(name, grade);
        }
    }

    public class StudentService
    {
        private readonly IStudentRepository _repository;
        private readonly IQuoteService _quoteService;
        private readonly StudentFactory _studentFactory;

        public StudentService(IStudentRepository repository, IQuoteService quoteService, StudentFactory studentFactory)
        {
            _repository = repository;
            _quoteService = quoteService;
            _studentFactory = studentFactory;
        }

        public QuoteDTO AddStudent(StudentDTO studentDto)
        {
            if (string.IsNullOrWhiteSpace(studentDto.Name))
                throw new ArgumentException("Name cannot be empty");
            if (studentDto.Grade < 0 || studentDto.Grade > 100)
                throw new ArgumentException("Grade must be between 0 and 100");

            var student = _studentFactory.CreateStudent(studentDto.Name, studentDto.Grade);
            _repository.AddStudent(student);
            return _quoteService.GetRandomQuote();
        }

        public void EditStudent(int id, StudentDTO studentDto)
        {
            var student = _repository.GetStudentById(id);
            if (student == null)
                throw new Exception("Student not found");

            if (string.IsNullOrWhiteSpace(studentDto.Name))
                throw new ArgumentException("Name cannot be empty");
            if (studentDto.Grade < 0 || studentDto.Grade > 100)
                throw new ArgumentException("Grade must be between 0 and 100");

            student.Name = studentDto.Name;
            student.Grade = studentDto.Grade;
            _repository.UpdateStudent(student);
        }

        public List<Student> GetAllStudents()
        {
            return _repository.GetAllStudents();
        }
    }
}
