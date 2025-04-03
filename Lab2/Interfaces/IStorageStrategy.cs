using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Interfaces
{
    public interface IStorageStrategy
    {
        Task SaveDocument(string content, string fileName);
        Task<string> LoadDocument(string fileName);
    }
}
