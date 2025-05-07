using Application;
using DataAccess;
using Presentation;

internal class Program
{
    static void Main(string[] args)
    {
        var httpClient = new HttpClient();
        var quoteFactory = new QuoteFactory();
        var quoteService = new BreakingBadQuoteClient(httpClient, quoteFactory);
        var repository = new JsonStudentRepository("students.json");
        var studentFactory = new StudentFactory();
        var studentService = new StudentService(repository, quoteService, studentFactory);
        var consoleUI = new ConsoleUI(studentService);

        try
        {
            consoleUI.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}