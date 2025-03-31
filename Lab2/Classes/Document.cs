using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2.Interfaces;
using Lab2.Classes;

namespace Lab2.Documentn
{
    public class Document
    {
        private List<ITextFragment> _fragments;
        public string FilePath { get; set; }

        public Document()
        {
            _fragments = new List<ITextFragment>();
            FilePath = string.Empty;
        }

        public void AppendText(string text)
        {
            var newFragments = TextParser.Parse(text);
            _fragments.AddRange(newFragments);
        }

        public void InsertText(int charPosition, string text)
        {
            if (charPosition < 0 || charPosition > GetTextWithoutMarkersLength())
            {
                throw new ArgumentOutOfRangeException("Character position is out of range.");
            }

            var newFragments = TextParser.Parse(text);
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

                    ITextFragment leftFragment = TextParser.Parse(decoratorPrefix + leftText + decoratorSuffix)[0];
                    ITextFragment rightFragment = TextParser.Parse(decoratorPrefix + rightText + decoratorSuffix)[0];

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
            return string.Join("", _fragments.Select(f => f.GetText()));
        }

        public string GetOriginalText()
        {
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
        public string Content { get; set; }
    }

    public static class DocumentManager
    {
        public static Document CreateNewDocument()
        {
            return new Document();
        }

        public static Document OpenDocument(string path)
        {
            string format = GetFormatFromPath(path);
            IDocumentLoader loader = DocumentFormatFactory.GetLoader(format);
            return loader.Load(path);
        }

        public static void SaveDocument(Document document, string path)
        {
            string format = GetFormatFromPath(path);
            IDocumentSaver saver = DocumentFormatFactory.GetSaver(format);
            saver.Save(path, document);
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
