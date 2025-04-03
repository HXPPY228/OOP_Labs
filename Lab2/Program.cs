using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Lab2.Interfaces;
using Lab2.Classes;
using Lab2.Documentn;
using Lab2.Enums;

class Program
{
    static private void PressAnyButton()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    static async Task Main(string[] args)
    {
        Document currentDocument = null;
        UndoRedoManager undoRedoManager = new UndoRedoManager();
        bool running = true, entry = true;

        while (entry)
        {
            Console.WriteLine("Select user to login:");
            for (int i = 0; i < UserManager.Users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {UserManager.Users[i].Name} ({UserManager.Users[i].Role})");
            }
            Console.Write("Enter choice (1-4): ");

            if (int.TryParse(Console.ReadLine(), out int userChoice) && userChoice >= 1 && userChoice <= 4)
            {
                Session.Login(UserManager.Users[userChoice - 1]);
                Console.WriteLine($"Logged in as: {Session.CurrentUser.Name}");
                PressAnyButton();
                entry = false;
            }
            else
            {
                Console.WriteLine("Invalid choice. Exiting.");
                PressAnyButton();
                Console.Clear();
            }
        }

        while (running)
        {
            Console.Clear();
            Console.WriteLine("Document Management System");
            Console.WriteLine("------------------------");
            Console.WriteLine($"Current User: {Session.CurrentUser.Name} | Role: {Session.CurrentUser.Role}");
            Console.WriteLine("Current Document: " + (currentDocument?.FilePath ?? "None"));
            Console.WriteLine("Current Document type: " + (currentDocument != null ? currentDocument.Type.ToString() : "None"));
            Console.WriteLine("Content: " + (currentDocument?.GetDisplayText() ?? "No content"));
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Create New Document");
            Console.WriteLine("2. Open Document");
            Console.WriteLine("3. Append Text");
            Console.WriteLine("4. Insert Text");
            Console.WriteLine("5. Delete Text");
            Console.WriteLine("6. Search Word");
            Console.WriteLine("7. Save Document");
            Console.WriteLine("8. Delete Document");
            Console.WriteLine("9. Undo");
            Console.WriteLine("10. Redo");
            Console.WriteLine("11. Exit");
            Console.WriteLine("=============================");
            Console.WriteLine("12. Manage Users (Admin only)");
            Console.WriteLine("13. Switch User");
            Console.Write("\nEnter your choice (1-13): ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        if (!Session.PermissionStrategy.CanEdit())
                        {
                            Console.WriteLine("You cant do this action with your role!");
                            PressAnyButton();
                            break;
                        }
                        Console.WriteLine("Select document type:");
                        Console.WriteLine("1. PlainText");
                        Console.WriteLine("2. Markdown");
                        Console.WriteLine("3. RichText");
                        string typeChoice = Console.ReadLine();
                        DocumentType docType;
                        switch (typeChoice)
                        {
                            case "1":
                                docType = DocumentType.PlainText;
                                break;
                            case "2":
                                docType = DocumentType.Markdown;
                                break;
                            case "3":
                                docType = DocumentType.RichText;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Defaulting to PlainText.");
                                docType = DocumentType.PlainText;
                                break;
                        }
                        currentDocument = DocumentManager.CreateNewDocument(docType);
                        currentDocument.Subscribe(Session.CurrentUser);
                        Console.WriteLine($"New {docType} document created.");
                        PressAnyButton();
                        break;

                    case "2":
                        Console.WriteLine("Select storage type:");
                        Console.WriteLine("1. Local File");
                        Console.WriteLine("2. Supabase Cloud");
                        var storageChoice = Console.ReadLine();

                        try
                        {
                            if (storageChoice == "2")
                            {
                                // Устанавливаем облачную стратегию
                                DocumentManager.SetStorageStrategy(new SupabaseStorageStrategy(
                                    Lab2.DB.GetUrl() ,
                                    Lab2.DB.GetKey()
                                ));

                                Console.Write("Enter cloud file name (e.g., document.json): ");
                                string cloudFileName = Console.ReadLine();

                                currentDocument = await DocumentManager.OpenDocumentDB(cloudFileName);
                                currentDocument.Subscribe(Session.CurrentUser);
                                Console.WriteLine($"Loaded from cloud: {cloudFileName}");
                            }
                            else
                            {
                                Console.Write("Enter local file path (e.g., doc.txt): ");
                                string localPath = Console.ReadLine();

                                if (!File.Exists(localPath))
                                    throw new FileNotFoundException("File not found");

                                currentDocument = DocumentManager.OpenDocument(localPath);
                                currentDocument.Subscribe(Session.CurrentUser);
                                Console.WriteLine($"Loaded from local storage: {localPath}");
                            }

                            Console.WriteLine($"Type: {currentDocument.Type}");
                            Console.WriteLine($"Content:\n{currentDocument.GetDisplayText()}");
                        }
                        catch (FileNotFoundException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (Postgrest.Exceptions.PostgrestException ex)
                        {
                            Console.WriteLine($"Supabase error: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }

                        PressAnyButton();
                        break;

                    case "3":
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            Console.Write("Enter text to append (use **bold**, __underline__, *italic*): ");
                            string appendText = Console.ReadLine();
                            ICommand appendCommand = new AppendTextCommand(currentDocument, appendText);
                            undoRedoManager.ExecuteCommand(appendCommand);
                        }
                        PressAnyButton();
                        break;

                    case "4":
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            Console.Write("Enter character position to insert at (ignoring **, __, *): ");
                            int insertPos = int.Parse(Console.ReadLine());
                            Console.Write("Enter text to insert (use **bold**, __underline__, *italic*): ");
                            string insertText = Console.ReadLine();
                            ICommand insertCommand = new InsertTextCommand(currentDocument, insertPos, insertText);
                            undoRedoManager.ExecuteCommand(insertCommand);
                        }
                        PressAnyButton();
                        break;

                    case "5":
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            Console.Write("Enter start fragment index to delete: ");
                            int deleteStart = int.Parse(Console.ReadLine());
                            Console.Write("Enter number of fragments to delete: ");
                            int deleteCount = int.Parse(Console.ReadLine());
                            ICommand deleteCommand = new DeleteTextCommand(currentDocument, deleteStart, deleteCount);
                            undoRedoManager.ExecuteCommand(deleteCommand);
                        }
                        PressAnyButton();
                        break;

                    case "6":
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            Console.Write("Enter word to search (positions ignore **, __, *): ");
                            string searchWord = Console.ReadLine();
                            List<int> positions = currentDocument.SearchWord(searchWord);
                            if (positions.Count > 0)
                            {
                                Console.WriteLine($"Found '{searchWord}' at positions: {string.Join(", ", positions)}");
                            }
                            else
                            {
                                Console.WriteLine($"'{searchWord}' not found.");
                            }
                        }
                        PressAnyButton();
                        break;

                    case "7":
                        if (!Session.PermissionStrategy.CanEdit())
                        {
                            Console.WriteLine("You cant do this action with your role!");
                            PressAnyButton();
                            break;
                        }
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            Console.WriteLine("Select storage type:");
                            Console.WriteLine("1. Local File");
                            Console.WriteLine("2. Supabase Cloud");
                            var storageChoice1 = Console.ReadLine();

                            IStorageStrategy strategy = storageChoice1 switch
                            {
                                "1" => new LocalFileStrategy(),
                                "2" => new SupabaseStorageStrategy(
                                    Lab2.DB.GetUrl(),
                                    Lab2.DB.GetKey()),
                                _ => throw new ArgumentException("Invalid storage type")
                            };

                            DocumentManager.SetStorageStrategy(strategy);

                            Console.Write("Enter file name to save: ");
                            string fileName = Console.ReadLine();

                            currentDocument.FilePath = fileName;

                            await DocumentManager.SaveDocumentDB(currentDocument, fileName);
                            PressAnyButton();
                            break;
                        }
                        PressAnyButton();
                        break;

                    case "8":
                        if (!Session.PermissionStrategy.CanEdit())
                        {
                            Console.WriteLine("You cant do this action with your role!");
                            PressAnyButton();
                            break;
                        }
                        Console.Write("Enter file path to delete: ");
                        string deletePath = Console.ReadLine();
                        if (File.Exists(deletePath))
                        {
                            DocumentManager.DeleteDocument(deletePath);
                            if (currentDocument != null && currentDocument.FilePath == deletePath)
                            {
                                currentDocument = null;
                            }
                            Console.WriteLine("Document deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("File does not exist.");
                        }
                        PressAnyButton();
                        break;

                    case "9":
                        if (!Session.PermissionStrategy.CanEdit())
                        {
                            Console.WriteLine("You cant do this action with your role!");
                            PressAnyButton();
                            break;
                        }
                        undoRedoManager.Undo();
                        Console.WriteLine("Undo performed.");
                        PressAnyButton();
                        break;

                    case "10":
                        if (!Session.PermissionStrategy.CanEdit())
                        {
                            Console.WriteLine("You cant do this action with your role!");
                            PressAnyButton();
                            break;
                        }
                        undoRedoManager.Redo();
                        Console.WriteLine("Redo performed.");
                        PressAnyButton();
                        break;

                    case "11":
                        running = false;
                        Console.WriteLine("Exiting program.");
                        PressAnyButton();
                        break;
                    case "12":
                        if (!Session.PermissionStrategy.CanManageUsers())
                        {
                            Console.WriteLine("Access denied!");
                            PressAnyButton();
                            break;
                        }
                        Console.WriteLine("Access granted!");
                        Console.WriteLine("Список пользователей:");
                        for (int i = 0; i < UserManager.Users.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {UserManager.Users[i].Name} ({UserManager.Users[i].Role})");
                        }

                        Console.Write("\nВведите номер пользователя для изменения: ");
                        if (!int.TryParse(Console.ReadLine(), out int userNumber) ||
                            userNumber < 1 ||
                            userNumber > UserManager.Users.Count)
                        {
                            Console.WriteLine("Некорректный номер пользователя!");
                            PressAnyButton();
                            break;
                        }

                        var selectedUser = UserManager.Users[userNumber - 1];

                        Console.WriteLine("Доступные роли:");
                        var roles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();
                        for (int i = 0; i < roles.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {roles[i]}");
                        }

                        Console.Write("Выберите номер новой роли: ");
                        if (!int.TryParse(Console.ReadLine(), out int roleNumber) ||
                            roleNumber < 1 ||
                            roleNumber > roles.Count)
                        {
                            Console.WriteLine("Некорректный номер роли!");
                            PressAnyButton();
                            break;
                        }

                        UserManager.UpdateUserRole(selectedUser.Name, roles[roleNumber - 1]);
                        Console.WriteLine($"Роль пользователя {selectedUser.Name} успешно изменена на {roles[roleNumber - 1]}!");
                        PressAnyButton();
                        break;
                    case "13":
                        currentDocument = null;
                        undoRedoManager = new UndoRedoManager();
                        entry = true;
                        running = true;

                        while (entry)
                        {
                            Console.Clear();
                            Console.WriteLine("Select user to login:");
                            for (int i = 0; i < UserManager.Users.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {UserManager.Users[i].Name} ({UserManager.Users[i].Role})");
                            }
                            Console.Write("Enter choice (1-4): ");

                            if (int.TryParse(Console.ReadLine(), out int newUserChoice) &&
                                newUserChoice >= 1 &&
                                newUserChoice <= UserManager.Users.Count)
                            {
                                Session.Login(UserManager.Users[newUserChoice - 1]);
                                Console.WriteLine($"Logged in as: {Session.CurrentUser.Name}");
                                PressAnyButton();
                                entry = false;
                            }
                            else
                            {
                                Console.WriteLine("Invalid choice. Try again.");
                                PressAnyButton();
                            }
                        }
                        break;


                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 13.");
                        PressAnyButton();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                PressAnyButton();
            }
        }
    }
}