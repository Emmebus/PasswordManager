using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Transactions;

namespace PasswordManager
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsMod { get; set; }
        public bool IsAdmin { get; set; }
        public User(string name, string email, string password) { 
            Name= name;
            Email= email;
            Password= password;
        }

        public static void CreateNewUser()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("CREATING NEW USER");
                Console.WriteLine("Enter nothing to cancel");
                Console.WriteLine("Enter you username:");
                string nameInput = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(nameInput))
                {
                    Console.WriteLine("User creation Cancelled");
                    Tool.Proceed();
                    return;
                }

                if (UsernameVerification(nameInput))
                {
                    Console.WriteLine("Username is approved");
                    Tool.Proceed();
                }
                else
                {
                    Console.WriteLine("Invalid username input");
                    Tool.Proceed();
                    continue;
                }

                Console.WriteLine("Enter you email:");
                string emailInput = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(emailInput))
                {
                    Console.WriteLine("User creation Cancelled");
                    Tool.Proceed();
                    return;
                }

                if (EmailVerification(emailInput))
                {
                    Console.WriteLine("Email is approved");
                    Tool.Proceed();
                }
                else
                {
                    Console.WriteLine("Invalid email input");
                    Tool.Proceed();
                    continue;
                }

                Console.WriteLine("Enter you password:");
                string PasswordInput  = HiddenInput();
                if (string.IsNullOrWhiteSpace(PasswordInput))
                {
                    Console.WriteLine("User creation Cancelled");
                    Tool.Proceed();
                    return;
                }

                if (PasswordVerification(PasswordInput))
                {
                    Console.WriteLine("Password is approved");
                    Tool.Proceed();
                }
                else
                    continue;

                Console.Clear();
                Console.WriteLine($"Name: {nameInput}");
                Console.WriteLine($"Email: {emailInput}");
                Console.WriteLine("Your password will not be shown.");
                if (Tool.Confirm())
                {
                    User newUser = new User(nameInput, emailInput, PasswordInput);
                    FileManager.Users.Add(newUser);
                    FileManager.WriteUpdatedJson();
                    Console.WriteLine("New user created");
                    Tool.Proceed();
                    break;
                }
                else
                {
                    Console.WriteLine("User creation cancelled");
                    Tool.Proceed();
                    break;
                }
            }
        }

        public static int Login()
        {
            Console.Clear();
            Console.WriteLine("LOGIN");
            Console.WriteLine("Enter your username:");
            string nameInput = Console.ReadLine().Trim();
            Console.WriteLine("Enter your password");
            string passwordInput = HiddenInput();

            for (int i = 0; i < FileManager.Users.Count; i++)
            {
                if (passwordInput == FileManager.Users[i].Password && nameInput == FileManager.Users[i].Name)
                {
                    Console.WriteLine("Loggin in...");
                    Thread.Sleep(2000);
                    return i;
                }
            }
            Console.WriteLine("Login failed");
            Tool.Proceed();
            return -1;
        }

        public static void EditUserInformation(int userIndex)
        {
            var currentUser = FileManager.Users[userIndex];
            while (true)
            {
                Console.Clear();
                Console.WriteLine("EDIT MY USED INFORMATION");
                Console.WriteLine($"Current username: {currentUser.Name}");
                Console.WriteLine("Enter a new Username:");
                Console.WriteLine("Enter nothing to continue without editing");
                string nameInput = Console.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(nameInput))
                {
                    Console.WriteLine("Nothing inputed");
                    Console.WriteLine("Username will not be edited");
                    Tool.Proceed();
                } else if (User.UsernameVerification(nameInput)){
                    currentUser.Name = nameInput;
                    Console.WriteLine($"Username set to {nameInput}");
                    Tool.Proceed();
                }
                else
                {
                    Console.WriteLine("Invalid name input");
                    Tool.Proceed();
                    continue;
                }

                Console.WriteLine($"Current email: {currentUser.Email}");
                Console.WriteLine("Enter a new email:");
                Console.WriteLine("Enter nothing to continue without editing");
                string emailInput = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(emailInput))
                {
                    Console.WriteLine("Nothing inputed");
                    Console.WriteLine("Email will not be edited");
                    Tool.Proceed();
                }
                else if (User.EmailVerification(emailInput))
                {
                    currentUser.Email = emailInput;
                    Console.WriteLine($"Email set to {emailInput}");
                    Tool.Proceed();
                }
                else
                {
                    Console.WriteLine("Invalid email input");
                    Tool.Proceed();
                    continue;
                }

                FileManager.WriteUpdatedJson();
                Console.Clear();

                Console.WriteLine($"User {userIndex} Updated. Current information:");
                Console.WriteLine($"Username: {currentUser.Name}");
                Console.WriteLine($"Email: {currentUser.Email}");
                Tool.Proceed();
                return;
            }
        }

        public static void EditUserPassword(int userIndex)
        {
            var currentUser = FileManager.Users[userIndex];
            Console.Clear();
            Console.WriteLine("EDIT MY PASSWORD");
            Console.WriteLine($"Current username {currentUser.Name}");
            Console.WriteLine("Note that your input will be hidden");
            Console.WriteLine("Enter your current password to proceed");
            Console.WriteLine("Enter nothing to continue without password");
            string passwordInput = HiddenInput();

            if (string.IsNullOrWhiteSpace(passwordInput))
            {
                Console.WriteLine("Password edit canceled");
                Tool.Proceed();
                return;
            }
            else if (passwordInput == currentUser.Password)
            {
                Console.WriteLine("Correct password");
                Console.WriteLine("Enter new password");
                string PasswordInput = HiddenInput();

                if (PasswordVerification(PasswordInput))
                {
                    Console.WriteLine("New Password is approved");
                    currentUser.Password = PasswordInput;
                    FileManager.WriteUpdatedJson();
                    Console.WriteLine("Your password has been updated");
                    Tool.Proceed();
                    return;
                }
                else
                {
                    EditUserPassword(userIndex);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Wrong password");
                Tool.Proceed();
                EditUserPassword(userIndex);
                return;
            }
        }

        public static bool DeleteUser(int userIndex)
        {
            Console.WriteLine($"Are you sure you wanna delete your user {FileManager.Users[userIndex].Name}?");
            if (Tool.Confirm())
            {
                Console.WriteLine("Your user will now be deleted");
                Tool.Proceed();
                FileManager.Users.RemoveAt(userIndex);
                FileManager.WriteUpdatedJson();
                return true;
            }
            else
            {
                Console.WriteLine("User was not deleted");
                Tool.Proceed();
                return false;
            }
        }

        public static void SetUserPrivilege(int userIndex)
        {
            var selectedUser = FileManager.Users[userIndex];

            Console.WriteLine($"Set privilage level for {selectedUser.Name}");
            Console.WriteLine("[1] Basic user");
            Console.WriteLine("[2] Mod user");
            Console.WriteLine("[3] Adming user");
            string input = Console.ReadLine()
                .ToLower()
                .Trim();

            if (input == "1")
            {
                selectedUser.IsMod = false;
                selectedUser.IsAdmin = false;
                Console.WriteLine("User privilage set to basic");
                Tool.Proceed();
                return;
            }
            else if (input == "2")
            {
                selectedUser.IsMod = true;
                selectedUser.IsAdmin = false;
                FileManager.WriteUpdatedJson();
                Console.WriteLine("User privilage set to moderator");
            }
            else if (input == "3")
            {
                selectedUser.IsMod = true;
                selectedUser.IsAdmin = true;
                Console.WriteLine("User privilage set to Administrator");
            }
            else
            {
                Console.WriteLine("Invalid input: " + input + ". Try Again");
                Tool.Proceed();
                SetUserPrivilege(userIndex);
                return;
            }
            FileManager.WriteUpdatedJson();
            Tool.Proceed();
            return;
        }


        public static void View()
        {
            if (FileManager.Users.Count == 0)
                Console.WriteLine("No users currently avalible");
            else
            {
                for (int i = 0; i < FileManager.Users.Count; i++)
                {
                    Console.WriteLine($"[{i}] {FileManager.Users[i].Name}");
                }
            }
        }

        public static string Privilege(int selectedUserIndex)
        {
            var selectedUser = FileManager.Users[selectedUserIndex];
            if (selectedUser.IsAdmin && selectedUser.IsMod)
                return "Admin";
            else if (selectedUser.IsMod)
                return "Moderator";
            else
                return "Basic";
        }

        private static bool UsernameVerification(string username)
        {
            if (!string.IsNullOrWhiteSpace(username) && username.Length > 3)
            {
                for (int i = 0; i < FileManager.Users.Count; i++)
                {
                    if (username == FileManager.Users[i].Name)
                    {
                        Console.WriteLine("Username already exists");
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }
        
        private static bool EmailVerification(string email)
        {
            if (!string.IsNullOrWhiteSpace(email) && email.Length > 3 && email.Contains("@") && email.Contains("."))
            {
                for (int i = 0; i < FileManager.Users.Count; i++)
                {
                    if (email.ToLower() == FileManager.Users[i].Email.ToLower())
                    {
                        Console.WriteLine("Email already exists");
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        private static bool PasswordVerification(string password)
        {
            if (!string.IsNullOrWhiteSpace(password) && !password.Contains('\u0000') && password.Length > 8 && password.Any(c => char.IsLower(c)) && password.Any(c => char.IsUpper(c)) && password.Any(ch => !char.IsLetterOrDigit(ch)) && password.Any(c => char.IsNumber(c)))
            {
                Console.WriteLine("Repeat password:");
                string passwordCompare = HiddenInput();
                if (password == passwordCompare)
                    return true;
                else
                {
                    Console.WriteLine("Passwords do no match");
                    Tool.Proceed();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Invalid password input");
                Console.WriteLine("Password must be at least 8 characters long");
                Console.WriteLine("Password must contain both upper- and lowercase letters");
                Console.WriteLine("Password must contain at least one letter, one number and one special character");
                Tool.Proceed();
                return false;
            }
        }

        private static string HiddenInput() {
            string hiddenInput = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    hiddenInput += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(hiddenInput))
                    {
                        hiddenInput = hiddenInput.Substring(0, hiddenInput.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return hiddenInput;
        }
    }
}