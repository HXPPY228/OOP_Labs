using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain
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
