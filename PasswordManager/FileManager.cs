using System.Text.Json;

namespace PasswordManager
{
    public class FileManager
    {
        private static string _currentDir = Environment.CurrentDirectory;
        private static string _path = Directory.GetParent(_currentDir).Parent.Parent.FullName + @"\Users.json";
        public static List<User> Users { get; set; }

        public static void UpdateJson()
        {
            FileManager.CreateFiles();
            FileManager.Users = FileManager.GetFiles();
        }

        public static void CreateFiles()
        {
            if (!File.Exists(_path) || String.IsNullOrEmpty(File.ReadAllText(_path)))
            {
                using (FileStream fs = File.Create(_path)) { }
                File.WriteAllText(_path, "[]");
            }
        }

        public static void WriteUpdatedJson() //Writes the current state of Users (List of Users objects) to Jsonfile
        {
            string JsonData = JsonSerializer.Serialize(Users);
            File.WriteAllText(_path, JsonData);
        }

        public static List<User> GetFiles()
        {
            string JsonData = File.ReadAllText(_path);
            var Users = JsonSerializer.Deserialize<List<User>>(JsonData,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true });
            return Users;
        }
    }
}
