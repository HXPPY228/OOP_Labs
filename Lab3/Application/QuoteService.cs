using System.Text.Json;
using Domain;
using DataAccess;

namespace Application
{
    public interface IQuoteService
    {
        QuoteDTO GetRandomQuote();
    }
    public class QuoteFactory
    {
        public QuoteDTO CreateQuote(string content, string author)
        {
            return new QuoteDTO { Content = content, Author = author };
        }

        public QuoteDTO CreateDefaultQuote()
        {
            return new QuoteDTO { Content = "Цитата недоступна", Author = "Неизвестно" };
        }
    }

    public class BreakingBadQuoteClient : IQuoteService
    {
        private readonly HttpClient _httpClient;
        private readonly QuoteFactory _quoteFactory;

        public BreakingBadQuoteClient(HttpClient httpClient, QuoteFactory quoteFactory)
        {
            _httpClient = httpClient;
            _quoteFactory = quoteFactory;
        }

        public QuoteDTO GetRandomQuote()
        {
            var response = _httpClient.GetAsync("https://api.breakingbadquotes.xyz/v1/quotes").Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            var quotes = JsonSerializer.Deserialize<List<QuoteDTO>>(json);
            var quote = quotes?.FirstOrDefault();
            return quote != null ? _quoteFactory.CreateQuote(quote.Content, quote.Author) : _quoteFactory.CreateDefaultQuote();
        }
    }
}
