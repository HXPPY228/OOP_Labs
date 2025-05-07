using System.Text.Json.Serialization;

namespace DTO
{
    public class StudentDTO
    {
        public string Name { get; set; }
        public int Grade { get; set; }
    }

    public class QuoteDTO
    {
        [JsonPropertyName("quote")]
        public string Content { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }
    }
}
