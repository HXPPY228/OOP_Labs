using System.Text.Json;
using Domain;

namespace DataAccess
{
    public interface IStudentRepository
    {
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        Student GetStudentById(int id);
        List<Student> GetAllStudents();
    }

    public class JsonStudentRepository : IStudentRepository
    {
        private readonly string _filePath;
        private List<Student> _students;
        private int _nextId;

        public JsonStudentRepository(string filePath)
        {
            _filePath = filePath;
            LoadStudents();
        }

        private void LoadStudents()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _students = JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
                _nextId = _students.Any() ? _students.Max(s => s.Id) + 1 : 1;
            }
            else
            {
                _students = new List<Student>();
                _nextId = 1;
            }
        }

        private void SaveStudents()
        {
            var json = JsonSerializer.Serialize(_students);
            File.WriteAllText(_filePath, json);
        }

        public void AddStudent(Student student)
        {
            student.Id = _nextId++;
            _students.Add(student);
            SaveStudents();
        }

        public void UpdateStudent(Student student)
        {
            var existing = _students.FirstOrDefault(s => s.Id == student.Id);
            if (existing != null)
            {
                existing.Name = student.Name;
                existing.Grade = student.Grade;
                SaveStudents();
            }
        }

        public Student GetStudentById(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public List<Student> GetAllStudents()
        {
            return _students;
        }
    }
}
