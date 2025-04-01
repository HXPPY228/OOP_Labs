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
    static void Main(string[] args)
    {
        Document currentDocument = null;
        UndoRedoManager undoRedoManager = new UndoRedoManager();
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("Document Management System");
            Console.WriteLine("------------------------");
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
            Console.Write("\nEnter your choice (1-11): ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
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
                        Console.WriteLine($"New {docType} document created.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("Enter file path to open (e.g., document.txt/json/xml): ");
                        string openPath = Console.ReadLine();
                        if (File.Exists(openPath))
                        {
                            currentDocument = DocumentManager.OpenDocument(openPath);
                            Console.WriteLine($"Document loaded. Type: {currentDocument.Type}, Content:\n{currentDocument.GetDisplayText()}");
                        }
                        else
                        {
                            Console.WriteLine("File does not exist.");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
                            Console.WriteLine("Text appended.");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
                            Console.WriteLine("Text inserted.");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
                            Console.WriteLine("Text deleted.");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
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
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "7":
                        if (currentDocument == null)
                        {
                            Console.WriteLine("No document loaded. Create or open a document first.");
                        }
                        else
                        {
                            string savePath;
                            if (string.IsNullOrEmpty(currentDocument.FilePath))
                            {
                                Console.Write("Enter file path to save (e.g., document.txt/json/xml): ");
                                savePath = Console.ReadLine();
                            }
                            else
                            {
                                savePath = currentDocument.FilePath;
                            }
                            DocumentManager.SaveDocument(currentDocument, savePath);
                            if (string.IsNullOrEmpty(currentDocument.FilePath))
                            {
                                currentDocument.FilePath = savePath;
                            }
                            Console.WriteLine($"Document saved to: {savePath}");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "8":
                        Console.Write("Enter file path to delete: ");
                        string deletePath = Console.ReadLine();
                        if (File.Exists(deletePath))
                        {
                            File.Delete(deletePath);
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
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "9":
                        undoRedoManager.Undo();
                        Console.WriteLine("Undo performed.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "10":
                        undoRedoManager.Redo();
                        Console.WriteLine("Redo performed.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    case "11":
                        running = false;
                        Console.WriteLine("Exiting program.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 11.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}