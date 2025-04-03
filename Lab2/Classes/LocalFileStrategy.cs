using Lab2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Classes
{
    public class LocalFileStrategy : IStorageStrategy
    {
        public async Task SaveDocument(string content, string fileName)
        {
            await File.WriteAllTextAsync(fileName, content);
        }

        public async Task<string> LoadDocument(string fileName)
        {
            return await File.ReadAllTextAsync(fileName);
        }
    }
}
