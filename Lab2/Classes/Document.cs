using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2.Interfaces;
using Lab2.Classes;
using Lab2.Enums;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Lab2.Documentn
{
    public class Document
    {
        public List<ITextFragment> _fragments;
        public string FilePath { get; set; }
        public DocumentType Type { get; set; }

        public Document(DocumentType type)
        {
            _fragments = new List<ITextFragment>();
            FilePath = string.Empty;
            Type = type;
        }

        public void AppendText(string text)
        {
            var newFragments = TextParser.Parse(text, Type);
            _fragments.AddRange(newFragments);
        }

        public void InsertText(int charPosition, string text)
        {
            if (charPosition < 0 || charPosition > GetTextWithoutMarkersLength())
            {
                throw new ArgumentOutOfRangeException("Character position is out of range.");
            }

            var newFragments = TextParser.Parse(text, Type);
            if (charPosition == GetTextWithoutMarkersLength())
            {
                _fragments.AddRange(newFragments);
                return;
            }

            int currentPos = 0;
            for (int i = 0; i < _fragments.Count; i++)
            {
                string fragmentTextWithoutMarkers = RemoveMarkers(_fragments[i].GetOriginalText());
                int fragmentLength = fragmentTextWithoutMarkers.Length;

                if (currentPos + fragmentLength > charPosition)
                {
                    // Позиция внутри текущего фрагмента
                    int splitPos = charPosition - currentPos;
                    string originalText = _fragments[i].GetOriginalText();
                    string textWithoutMarkers = RemoveMarkers(originalText);

                    // Определяем, сколько символов нужно взять из originalText, чтобы получить splitPos символов без маркеров
                    int plainTextPos = 0;
                    int splitIndex = 0;
                    for (int j = 0; j < originalText.Length && plainTextPos < splitPos; j++)
                    {
                        if (j + 1 < originalText.Length && (originalText.Substring(j, 2) == "**" || originalText.Substring(j, 2) == "__"))
                        {
                            j++;
                        }
                        else if (originalText[j] == '*')
                        {
                            
                        }
                        else
                        {
                            plainTextPos++;
                        }
                        splitIndex = j + 1;
                    }

                    string leftText = textWithoutMarkers.Substring(0, splitPos);
                    string rightText = textWithoutMarkers.Substring(splitPos);

                    string decoratorPrefix = "", decoratorSuffix = "";
                    if (_fragments[i] is BoldDecorator)
                    {
                        decoratorPrefix = "**";
                        decoratorSuffix = "**";
                    }
                    else if (_fragments[i] is UnderlineDecorator)
                    {
                        decoratorPrefix = "__";
                        decoratorSuffix = "__";
                    }
                    else if (_fragments[i] is ItalicDecorator)
                    {
                        decoratorPrefix = "*";
                        decoratorSuffix = "*";
                    }

                    ITextFragment leftFragment = TextParser.Parse(decoratorPrefix + leftText + decoratorSuffix, Type)[0];
                    ITextFragment rightFragment = TextParser.Parse(decoratorPrefix + rightText + decoratorSuffix, Type)[0];

                    _fragments[i] = leftFragment;

                    _fragments.InsertRange(i + 1, newFragments);

                    if (!string.IsNullOrEmpty(rightText))
                    {
                        _fragments.Insert(i + 1 + newFragments.Count, rightFragment);
                    }
                    return;
                }
                currentPos += fragmentLength;
            }
        }

        public void DeleteText(int fragmentStart, int fragmentCount)
        {
            if (fragmentStart < 0 || fragmentStart >= _fragments.Count || fragmentCount < 0 || fragmentStart + fragmentCount > _fragments.Count)
            {
                throw new ArgumentOutOfRangeException("Invalid start or count.");
            }
            _fragments.RemoveRange(fragmentStart, fragmentCount);
        }

        public List<int> SearchWord(string word)
        {
            string textWithoutMarkers = GetTextWithoutMarkers();
            List<int> positions = new List<int>();
            int index = textWithoutMarkers.IndexOf(word, 0);
            while (index != -1)
            {
                positions.Add(index);
                index = textWithoutMarkers.IndexOf(word, index + 1);
            }
            return positions;
        }

        public string GetDisplayText()
        {
            if (Type == DocumentType.PlainText)
            {
                return string.Join("", _fragments.Select(f => f.GetOriginalText()));
            }
            return string.Join("", _fragments.Select(f => f.GetText()));
        }

        public string GetOriginalText()
        {
            if (Type == DocumentType.RichText)
            {
                return string.Join("", _fragments.Select(f => f.GetText()));
            }
            return string.Join("", _fragments.Select(f => f.GetOriginalText()));
        }

        private string GetTextWithoutMarkers()
        {
            string originalText = GetOriginalText();
            return RemoveMarkers(originalText);
        }

        private int GetTextWithoutMarkersLength()
        {
            return GetTextWithoutMarkers().Length;
        }

        private string RemoveMarkers(string text)
        {
            // Удаляем маркеры **, __, * из текста
            return text.Replace("**", "").Replace("__", "").Replace("*", "");
        }
    }
    public class DocumentData
    {
        public DocumentType Type { get; set; }
        public string Content { get; set; }
    }

    public static class DocumentManager
    {
        public static Document CreateNewDocument(DocumentType type)
        {
            return new Document(type);
        }

        public static Document OpenDocument(string path)
        {
            string format = Path.GetExtension(path).ToLower().TrimStart('.');
            if (format == "txt")
            {
                string content = File.ReadAllText(path);
                Document doc = new Document(DocumentType.PlainText);
                doc.AppendText(content);
                doc.FilePath = path;
                return doc;
            }
            else if (format == "json")
            {
                string json = File.ReadAllText(path);
                DocumentData data = JsonConvert.DeserializeObject<DocumentData>(json);
                Document doc = new Document(data.Type);
                doc.AppendText(data.Content);
                doc.FilePath = path;
                return doc;
            }
            else if (format == "xml")
            {
                using (var reader = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(DocumentData));
                    DocumentData data = (DocumentData)serializer.Deserialize(reader);
                    Document doc = new Document(data.Type);
                    doc.AppendText(data.Content);
                    doc.FilePath = path;
                    return doc;
                }
            }
            throw new ArgumentException("Unsupported file format");
        }

        public static void SaveDocument(Document document, string path)
        {
            string format = Path.GetExtension(path).ToLower().TrimStart('.');
            if (format == "txt")
            {
                File.WriteAllText(path, document.GetOriginalText());
            }
            else if (format == "json")
            {
                DocumentData data = new DocumentData { Type = document.Type, Content = document.GetOriginalText() };
                string json = JsonConvert.SerializeObject(data);
                File.WriteAllText(path, json);
            }
            else if (format == "xml")
            {
                DocumentData data = new DocumentData { Type = document.Type, Content = document.GetOriginalText() };
                var serializer = new XmlSerializer(typeof(DocumentData));
                using (var writer = new StreamWriter(path))
                {
                    serializer.Serialize(writer, data);
                }
            }
            else
            {
                throw new ArgumentException("Unsupported file format");
            }
            document.FilePath = path;
        }

        public static void DeleteDocument(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new FileNotFoundException("File does not exist", path);
            }
        }

        private static string GetFormatFromPath(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".txt":
                    return "txt";
                case ".json":
                    return "json";
                case ".xml":
                    return "xml";
                default:
                    throw new ArgumentException("Unsupported file extension");
            }
        }
    }
}
