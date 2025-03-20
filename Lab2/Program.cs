using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Абстрактный класс документа (Abstract Factory)
abstract class Document
{
    public string Title { get; set; }
    public string Content { get; set; }
    public abstract void Display();
}

// Конкретные типы документов (Concrete Products)
class PlainTextDocument : Document
{
    public PlainTextDocument(string title)
    {
        Title = title;
        Content = "";
    }

    public override void Display()
    {
        Console.WriteLine($"Обычный текст: {Content}");
    }
}

class MarkdownDocument : Document
{
    public MarkdownDocument(string title)
    {
        Title = title;
        Content = "";
    }

    public override void Display()
    {
        Console.WriteLine($"Markdown: {Content}");
    }
}

// Интерфейс форматирования (Decorator)
interface ITextFormatter
{
    string Format(string text);
}

// Базовый текст (Component)
class BasicText : ITextFormatter
{
    public string Format(string text) => text;
}

// Декораторы форматирования
class BoldFormatter : ITextFormatter
{
    private ITextFormatter _formatter;
    public BoldFormatter(ITextFormatter formatter) => _formatter = formatter;
    public string Format(string text) => $"**{_formatter.Format(text)}**";
}

class ItalicFormatter : ITextFormatter
{
    private ITextFormatter _formatter;
    public ItalicFormatter(ITextFormatter formatter) => _formatter = formatter;
    public string Format(string text) => $"*{_formatter.Format(text)}*";
}

// Команда (Command pattern)
interface ICommand
{
    void Execute();
    void Undo();
}

class TextEditCommand : ICommand
{
    private Document _document;
    private string _oldContent;
    private string _newContent;

    public TextEditCommand(Document document, string newContent)
    {
        _document = document;
        _oldContent = document.Content;
        _newContent = newContent;
    }

    public void Execute() => _document.Content = _newContent;
    public void Undo() => _document.Content = _oldContent;
}

// Система Undo/Redo (Memento + Command)
class History
{
    private Stack<ICommand> _undoStack = new Stack<ICommand>();
    private Stack<ICommand> _redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
        }
    }
}

// Хранилище (Strategy)
interface IStorage
{
    void Save(Document document);
    Document Load(string title);
}

class FileStorage : IStorage
{
    public void Save(Document document)
    {
        File.WriteAllText($"{document.Title}.txt", document.Content);
    }

    public Document Load(string title)
    {
        var content = File.ReadAllText($"{title}.txt");
        return new PlainTextDocument(title) { Content = content };
    }
}

// Фабрика документов (Factory)
class DocumentFactory
{
    public Document CreateDocument(string type, string title)
    {
        return type.ToLower() switch
        {
            "plaintext" => new PlainTextDocument(title),
            "markdown" => new MarkdownDocument(title),
            _ => throw new ArgumentException("Неподдерживаемый тип документа")
        };
    }
}

// Наблюдатель (Observer)
interface IDocumentObserver
{
    void Update(string message);
}

class User : IDocumentObserver
{
    public string Name { get; set; }
    public string Role { get; set; }

    public void Update(string message)
    {
        Console.WriteLine($"{Name} ({Role}) получил уведомление: {message}");
    }
}

// Главный класс редактора (Facade)
class DocumentEditor
{
    private Document _currentDocument;
    private History _history = new History();
    private IStorage _storage;
    private List<IDocumentObserver> _observers = new List<IDocumentObserver>();
    private DocumentFactory _factory = new DocumentFactory();

    public DocumentEditor(IStorage storage)
    {
        _storage = storage;
    }

    public void CreateDocument(string type, string title)
    {
        _currentDocument = _factory.CreateDocument(type, title);
        NotifyObservers($"Создан документ: {title}");
    }

    public void EditText(string newContent)
    {
        var command = new TextEditCommand(_currentDocument, newContent);
        _history.ExecuteCommand(command);
        NotifyObservers("Текст отредактирован");
    }

    public void Undo() => _history.Undo();
    public void Redo() => _history.Redo();

    public void Save() => _storage.Save(_currentDocument);
    public void Load(string title) => _currentDocument = _storage.Load(title);

    public void AddObserver(IDocumentObserver observer) => _observers.Add(observer);
    private void NotifyObservers(string message)
    {
        foreach (var observer in _observers)
            observer.Update(message);
    }

    public void Display() => _currentDocument?.Display();
}

class Program
{
    static DocumentEditor editor = new DocumentEditor(new FileStorage());
    static ITextFormatter formatter = new BasicText();

    static void Main(string[] args)
    {
        // Инициализация пользователей
        var admin = new User { Name = "Алексей", Role = "Admin" };
        var editorUser = new User { Name = "Мария", Role = "Editor" };

        editor.AddObserver(admin);
        editor.AddObserver(editorUser);

        ShowMenu();
    }

    static void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Текстовый редактор ===");
            Console.WriteLine("1. Создать новый документ");
            Console.WriteLine("2. Редактировать текст");
            Console.WriteLine("3. Применить форматирование");
            Console.WriteLine("4. Показать документ");
            Console.WriteLine("5. Отменить действие (Undo)");
            Console.WriteLine("6. Повторить действие (Redo)");
            Console.WriteLine("7. Сохранить документ");
            Console.WriteLine("8. Загрузить документ");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        CreateDocument();
                        break;
                    case "2":
                        EditText();
                        break;
                    case "3":
                        ApplyFormatting();
                        break;
                    case "4":
                        editor.Display();
                        break;
                    case "5":
                        editor.Undo();
                        Console.WriteLine("Действие отменено");
                        break;
                    case "6":
                        editor.Redo();
                        Console.WriteLine("Действие повторено");
                        break;
                    case "7":
                        editor.Save();
                        Console.WriteLine("Документ сохранен");
                        break;
                    case "8":
                        LoadDocument();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    static void CreateDocument()
    {
        Console.WriteLine("Выберите тип документа:");
        Console.WriteLine("1. PlainText");
        Console.WriteLine("2. Markdown");
        string typeChoice = Console.ReadLine();

        Console.Write("Введите название документа: ");
        string title = Console.ReadLine();

        string type = typeChoice switch
        {
            "1" => "plaintext",
            "2" => "markdown",
            _ => throw new Exception("Неверный тип документа")
        };

        editor.CreateDocument(type, title);
        Console.WriteLine("Документ создан успешно!");
    }

    static void EditText()
    {
        Console.Write("Введите новый текст: ");
        string text = Console.ReadLine();
        editor.EditText(text);
        Console.WriteLine("Текст обновлен!");
    }

    static void ApplyFormatting()
    {
        Console.WriteLine("Выберите форматирование:");
        Console.WriteLine("1. Обычный текст");
        Console.WriteLine("2. Жирный");
        Console.WriteLine("3. Курсив");
        Console.WriteLine("4. Жирный + Курсив");
        string formatChoice = Console.ReadLine();

        formatter = formatChoice switch
        {
            "1" => new BasicText(),
            "2" => new BoldFormatter(new BasicText()),
            "3" => new ItalicFormatter(new BasicText()),
            "4" => new BoldFormatter(new ItalicFormatter(new BasicText())),
            _ => throw new Exception("Неверный выбор форматирования")
        };

        Console.WriteLine("Форматирование применено!");
    }

    static void LoadDocument()
    {
        Console.Write("Введите название документа для загрузки: ");
        string title = Console.ReadLine();
        editor.Load(title);
        Console.WriteLine("Документ загружен!");
    }
}