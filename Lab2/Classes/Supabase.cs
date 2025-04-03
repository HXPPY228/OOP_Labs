using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase;
using Postgrest.Attributes;
using Postgrest.Models;
using Lab2.Interfaces;
using Postgrest;

namespace Lab2.Classes
{

    [Table("documents")]
    public class DocumentRecord : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("content")]
        public string Content { get; set; }

        [Column("file_name")]
        public string FileName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SupabaseStorageStrategy : IStorageStrategy
    {
        private readonly Supabase.Client _supabase;

        public SupabaseStorageStrategy(string supabaseUrl, string supabaseKey)
        {
            _supabase = new Supabase.Client(supabaseUrl, supabaseKey);
            _supabase.InitializeAsync().Wait();
        }

        public async Task SaveDocument(string content, string fileName)
        {
            var record = new DocumentRecord
            {
                Content = content,
                FileName = fileName,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<DocumentRecord>().Insert(record);
        }

        public async Task<string> LoadDocument(string fileName)
        {
            var response = await _supabase.From<DocumentRecord>()
                .Where(x => x.FileName == fileName)
                .Get();

            return response.Models.FirstOrDefault()?.Content;
        }
    }
}
