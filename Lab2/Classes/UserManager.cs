using Lab2.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Classes
{
    public static class UserManager
    {
        private const string UsersFilePath = "D:\\BSUIR\\2_course\\ООП\\Lab2\\UserData.json";
        private static List<User> _users;

        static UserManager()
        {
            LoadUsers();
        }

        public static List<User> Users => _users;

        private static void LoadUsers()
        {
            if (!File.Exists(UsersFilePath))
            {
                _users = new List<User>();
                return;
            }

            var json = File.ReadAllText(UsersFilePath);
            _users = JsonConvert.DeserializeObject<List<User>>(json);
        }

        public static void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
            File.WriteAllText(UsersFilePath, json);
        }

        public static void UpdateUserRole(string userName, UserRole newRole)
        {
            var user = _users.FirstOrDefault(u => u.Name == userName);
            if (user != null)
            {
                user.Role = newRole;
                SaveUsers();
            }
        }
        public static IEnumerable<User> GetAdmins()
        => Users.Where(u => u.Role == UserRole.Admin);
    }
}
