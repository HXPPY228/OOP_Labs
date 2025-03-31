using Lab2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Classes
{
    public class TextParser
    {
        public static List<ITextFragment> Parse(string text)
        {
            var fragments = new List<ITextFragment>();
            int index = 0;

            while (index < text.Length)
            {
                // Проверка на ** (жирный текст)
                if (index + 1 < text.Length && text[index] == '*' && text[index + 1] == '*')
                {
                    int endIndex = text.IndexOf("**", index + 2);
                    if (endIndex != -1)
                    {
                        string boldText = text.Substring(index + 2, endIndex - index - 2);
                        fragments.Add(new BoldDecorator(new PlainTextFragment(boldText)));
                        index = endIndex + 2;
                    }
                    else
                    {
                        fragments.Add(new PlainTextFragment("**"));
                        index += 2;
                    }
                }
                // Проверка на __ (подчёркнутый текст)
                else if (index + 1 < text.Length && text[index] == '_' && text[index + 1] == '_')
                {
                    int endIndex = text.IndexOf("__", index + 2);
                    if (endIndex != -1)
                    {
                        string underlineText = text.Substring(index + 2, endIndex - index - 2);
                        fragments.Add(new UnderlineDecorator(new PlainTextFragment(underlineText)));
                        index = endIndex + 2;
                    }
                    else
                    {
                        fragments.Add(new PlainTextFragment("__"));
                        index += 2;
                    }
                }
                // Проверка на * (курсив)
                else if (text[index] == '*')
                {
                    int endIndex = text.IndexOf("*", index + 1);
                    if (endIndex != -1)
                    {
                        string italicText = text.Substring(index + 1, endIndex - index - 1);
                        fragments.Add(new ItalicDecorator(new PlainTextFragment(italicText)));
                        index = endIndex + 1;
                    }
                    else
                    {
                        fragments.Add(new PlainTextFragment("*"));
                        index += 1;
                    }
                }
                // Обычный текст
                else
                {
                    int nextBold = text.IndexOf("**", index);
                    int nextUnderline = text.IndexOf("__", index);
                    int nextItalic = text.IndexOf("*", index);
                    int nextMarker = Math.Min(nextBold == -1 ? text.Length : nextBold,
                        Math.Min(nextUnderline == -1 ? text.Length : nextUnderline,
                                 nextItalic == -1 ? text.Length : nextItalic));
                    string plainText = text.Substring(index, nextMarker - index);
                    fragments.Add(new PlainTextFragment(plainText));
                    index = nextMarker;
                }
            }

            return fragments;
        }
    }

}
