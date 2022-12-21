using PasswordManager;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace PasswordManager
{
internal class Program
    {
        static void Main(string[] args)
        {
            string input;

            FileManager.CreateFiles(); //Initializes JSON file
            FileManager.Users = FileManager.GetFiles(); //gets data from JSON file

            startMenu(); //start of application

            void startMenu(){
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("START MENU");
                    Console.WriteLine("[1] Login");
                    Console.WriteLine("[2] Register");
                    Console.WriteLine("[X] Exit");
                    input = Console.ReadLine()
                        .ToLower()
                        .Trim();

                    if (input == "x" || input == "exit" || string.IsNullOrWhiteSpace(input))
                        Tool.Exit();
                    else if (input == "1")
                        userMenu(User.Login());
                    else if (input == "2")
                        User.CreateNewUser();
                    else
                    {
                        Console.WriteLine("Invalid input: " + input + ". Try Again");
                        Tool.Proceed();
                    }
                }
            }

            void userMenu(int userIndex)
            {
                if (userIndex < 0) //Checks for failed login. A failed login returns -1 as userIndex.
                    return;

                var currentUser = FileManager.Users[userIndex];
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Current user {userIndex} : {currentUser.Name}");
                    if (currentUser.IsMod)
                        Console.WriteLine("Moderator Priviliges");
                    if (currentUser.IsMod && currentUser.IsAdmin)
                        Console.WriteLine("Admin Priviliges");
                    Console.WriteLine(); //line break
                    Console.WriteLine("Select action to perform:");
                    Console.WriteLine("[1] Edit my user information");
                    Console.WriteLine("[2] Change password");
                    Console.WriteLine("[3] Delete my user");
                    if (currentUser.IsMod)
                        Console.WriteLine("[4] Display all users");
                    if (currentUser.IsMod && currentUser.IsAdmin)
                        Console.WriteLine("[5] Create new user");
                    Console.WriteLine("[L] Log out");
                    Console.WriteLine("[X] Exit");
                    input = Console.ReadLine()
                        .ToLower()
                        .Trim();

                    if (input == "x" || input == "exit" || string.IsNullOrWhiteSpace(input))
                    {
                        Tool.Exit();
                    }
                    else if (input == "l")
                    {
                        Console.WriteLine("logging out");
                        Console.WriteLine("3..");
                        Thread.Sleep(1000);
                        Console.WriteLine("2..");
                        Thread.Sleep(1000);
                        Console.WriteLine("1..");
                        Thread.Sleep(1000);
                        startMenu();
                        return; //failsafe
                    }
                    else if (input == "1")
                        User.EditUserInformation(userIndex);
                    else if (input == "2")
                        User.EditUserPassword(userIndex);
                    else if (input == "3")
                    {
                        if (User.DeleteUser(userIndex))
                        {
                            startMenu();
                            return;
                        }
                        else
                        {
                            userMenu(userIndex);
                            return;
                        }
                    }
                    else if (input == "4" && currentUser.IsMod)
                        userView(userIndex);
                    else if (input == "5" && currentUser.IsMod && currentUser.IsAdmin)
                        User.CreateNewUser();
                    else
                    {
                        Console.WriteLine("Invalid input: " + input + ". Try Again");
                        Tool.Proceed();
                    }
                }
            }

            void userView(int userIndex)
            {
                var currentUser = FileManager.Users[userIndex];
                while (currentUser.IsMod) //only users with at least moderator privelage should be able to see other users.
                {
                    Console.Clear();
                    Console.WriteLine("USER VIEW");
                    Console.WriteLine($"Current user {userIndex} : {currentUser.Name}");
                    Console.WriteLine(); // line break
                    User.View();
                    Console.WriteLine(); //line break
                    Console.WriteLine("Select a user by entering its index number");
                    Console.WriteLine("[B] Go back");
                    Console.WriteLine("[L] Log out");
                    Console.WriteLine("[X] Exit");
                    input = Console.ReadLine()
                        .ToLower()
                        .Trim();

                    for (int i = 0; i < FileManager.Users.Count; i++)
                    {
                        if (input == i.ToString())
                        {
                            userManagment(userIndex, i);
                            userView(userIndex);
                            return;
                        }
                    }

                    if (input == "x" || input == "exit" || string.IsNullOrWhiteSpace(input))
                        Tool.Exit();
                    else if (input == "b")
                        return;
                    else if (input == "l")
                    {
                        Console.WriteLine("logging out");
                        Console.WriteLine("3..");
                        Thread.Sleep(1000);
                        Console.WriteLine("2..");
                        Thread.Sleep(1000);
                        Console.WriteLine("1..");
                        Thread.Sleep(1000);
                        startMenu();
                        return; //failsafe
                    }
                    else
                    {
                        Console.WriteLine("Invalid input: " + input + ". Try Again");
                        Tool.Proceed();
                    }
                }
            }

            void userManagment(int currentUserIndex, int selectedUserIndex)
            {
                var currentUser = FileManager.Users[currentUserIndex];
                var selectedUser = FileManager.Users[selectedUserIndex];

                if (currentUserIndex == selectedUserIndex)
                {
                    Console.WriteLine("You cannot manage your own user using the managment menu. ");
                    Console.WriteLine("To edit your own information, use the options provided in the user menu.");
                    Tool.Proceed();
                    return;
                }

                while (currentUser.IsMod) //only users with at least moderator privelage should be able to see other users.
                {
                    Console.Clear();
                    Console.WriteLine("USER MANAGMENT");
                    Console.WriteLine($"Current user {currentUserIndex} : {currentUser.Name}");
                    Console.WriteLine($"Selected user {selectedUserIndex} : {selectedUser.Name}");
                    Console.WriteLine($"Email: {selectedUser.Email}");
                    if (currentUser.IsMod)
                        Console.WriteLine($"Privilege level: {User.Privilege(selectedUserIndex)}");
                    if (currentUser.IsAdmin)
                        Console.WriteLine($"Password: {selectedUser.Password}");
                    Console.WriteLine(); // line break
                    Console.WriteLine("Select action");
                    Console.WriteLine("[1] Edit user information");
                    Console.WriteLine("[2] Delete User");
                    if (currentUser.IsAdmin)
                    {
                        Console.WriteLine("[3] Set or revoke privileges");
                        Console.WriteLine("[4] Change user password");
                    }
                    Console.WriteLine("[B] Go back");
                    Console.WriteLine("[L] Log out");
                    Console.WriteLine("[X] Exit");
                    input = Console.ReadLine()
                        .ToLower()
                        .Trim();

                    if (input == "x" || input == "exit" || string.IsNullOrWhiteSpace(input))
                        Tool.Exit();
                    else if (input == "b")
                        return;
                    else if (input == "l")
                    {
                        Console.WriteLine("logging out");
                        Console.WriteLine("3..");
                        Thread.Sleep(1000);
                        Console.WriteLine("2..");
                        Thread.Sleep(1000);
                        Console.WriteLine("1..");
                        Thread.Sleep(1000);
                        startMenu();
                        return; //failsafe
                    }
                    else if (input == "1")
                        User.EditUserInformation(selectedUserIndex);
                    else if (input == "2")
                    {
                        if (User.DeleteUser(selectedUserIndex)) //user is deleted
                        {
                            startMenu();
                            return;
                        }
                        else //user is not deleted
                        {
                            userManagment(currentUserIndex, selectedUserIndex);
                            return;
                        }
                    }
                    else if (input == "3" && currentUser.IsAdmin)
                        User.SetUserPrivilege(selectedUserIndex);
                    else if (input == "4" && currentUser.IsAdmin)
                        User.EditUserPassword(selectedUserIndex);
                    else
                    {
                        Console.WriteLine("Invalid input: " + input + ". Try Again");
                        Tool.Proceed();
                    }
                }
            }
        }
    }
}