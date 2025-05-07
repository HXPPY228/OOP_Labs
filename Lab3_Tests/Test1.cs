using System.Net;
using Domain;
using DataAccess;
using Application;

namespace Lab3_Tests
{
    [TestClass]
    public class StudentServiceTests
    {
        private StudentService _studentService;
        private JsonStudentRepository _repository;
        private BreakingBadQuoteClient _quoteService;
        private StudentFactory _studentFactory;
        private readonly string _testFilePath = "test_students.json";

        [TestInitialize]
        public void Setup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }

            _repository = new JsonStudentRepository(_testFilePath);
            _quoteService = new BreakingBadQuoteClient(new HttpClient(), new QuoteFactory());
            _studentFactory = new StudentFactory();
            _studentService = new StudentService(_repository, _quoteService, _studentFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void AddStudent_ValidStudent_AddsStudentAndReturnsQuote()
        {
            // Arrange
            var studentDto = new StudentDTO { Name = "John Doe", Grade = 85 };

            // Act
            var quote = _studentService.AddStudent(studentDto);
            var students = _repository.GetAllStudents();

            // Assert
            Assert.AreEqual(1, students.Count, "Должен быть добавлен один студент.");
            Assert.AreEqual("John Doe", students[0].Name, "Имя студента должно совпадать.");
            Assert.AreEqual(85, students[0].Grade, "Оценка студента должна совпадать.");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Content), "Цитата не должна быть пустой.");
        }

        [TestMethod]
        public void GetRandomQuote_SuccessfulResponse_ReturnsQuote()
        {
            // Act
            var quote = _quoteService.GetRandomQuote();

            // Assert
            Assert.IsNotNull(quote, "Цитата не должна быть null.");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Content), "Содержимое цитаты не должно быть пустым.");
            Assert.IsFalse(string.IsNullOrEmpty(quote.Author), "Автор цитаты не должен быть пустым.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddStudent_InvalidGrade_ThrowsArgumentException()
        {
            var studentDto = new StudentDTO { Name = "John Doe", Grade = -1 };

            _studentService.AddStudent(studentDto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddStudent_InvalidName_ThrowsArgumentException()
        {
            var studentDto = new StudentDTO { Name = "", Grade = 85 };
            _studentService.AddStudent(studentDto);
        }
    }
}
