using System.Xml.Linq;
using Lab2.Classes;
using Lab2.Documentn;
using Lab2.Enums;
using Lab2.Interfaces;

namespace Lab2_UnitTests
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public void CreateDocument_PlainText_ShouldInitializeEmpty()
        {
            var doc = new Document(DocumentType.PlainText);
            Assert.AreEqual(string.Empty, doc.GetOriginalText());
        }

        [TestMethod]
        public void AppendText_ShouldAddContent()
        {
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("Hello");
            Assert.AreEqual("Hello", doc.GetOriginalText());
        }

        [TestMethod]
        public void InsertText_AtBeginning_ShouldModifyContent()
        {
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("World");
            doc.InsertText(0, "Hello ");
            Assert.AreEqual("Hello World", doc.GetOriginalText());
        }

        [TestMethod]
        public void DeleteText_ShouldRemoveFragments()
        {
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("Hello World");
            doc.DeleteText(0, 1); // Удалить первый фрагмент
            Assert.AreEqual(string.Empty, doc.GetOriginalText());
        }

        [TestMethod]
        public void SearchWord_ExistingWord_ShouldReturnPositions()
        {
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("Hello Hello");
            var positions = doc.SearchWord("Hello");
            Assert.AreEqual(2, positions.Count);
        }
    }
    [TestClass]
    public class DocumentManagerTests
    {

        [TestMethod]
        public async Task SaveDocument_PlainText_ShouldCreateFile()
        {
            // Устанавливаем стратегию хранения для локальных файлов
            DocumentManager.SetStorageStrategy(new LocalFileStrategy());

            // Генерируем уникальный путь к файлу с расширением .txt
            string testFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");

            // Создаем документ и добавляем текст
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("Test\nEND");

            // Асинхронно сохраняем документ
            await DocumentManager.SaveDocument(doc, testFilePath);

            // Проверяем, что файл создан
            Assert.IsTrue(File.Exists(testFilePath));

            // Проверяем содержимое файла
            string content = await File.ReadAllTextAsync(testFilePath);
            Assert.AreEqual("Test\nEND", content);

            // Удаляем временный файл
            File.Delete(testFilePath);
        }

        [TestMethod]
        public async Task OpenDocument_ValidFile_ShouldLoadContent()
        {
            // Устанавливаем стратегию хранения для локальных файлов
            DocumentManager.SetStorageStrategy(new LocalFileStrategy());

            // Генерируем уникальный путь к файлу с расширением .txt
            string testFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");

            // Асинхронно записываем тестовое содержимое в файл
            await File.WriteAllTextAsync(testFilePath, "Test");

            // Асинхронно открываем документ
            var doc = await DocumentManager.OpenDocument(testFilePath);

            // Проверяем содержимое документа
            Assert.AreEqual("Test", doc.GetOriginalText());

            // Проверяем, что тип документа — PlainText
            Assert.AreEqual(DocumentType.PlainText, doc.Type);

            // Удаляем временный файл
            File.Delete(testFilePath);
        }
    }
    [TestClass]
    public class DocumentHistoryTests
    {
        [TestMethod]
        public void AddEntry_ShouldStoreSnapshot()
        {
            var history = new DocumentHistory();
            history.AddEntry("TEST", "Content");
            Assert.AreEqual(1, history.GetHistory().Count());
        }

        [TestMethod]
        public void History_ShouldStoreMaxEntries()
        {
            var history = new DocumentHistory();
            for (int i = 0; i < 60; i++)
                history.AddEntry("ACTION", $"Content {i}");
            Assert.AreEqual(50, history.GetHistory().Count());
        }
    }
    [TestClass]
    public class TerminalSettingsTests
    {
        [TestMethod]
        public void UpdateSettings_ShouldChangeFontSize()
        {
            TerminalSettingsManager.Instance.UpdateSettings(12, "CGA");
            Assert.AreEqual(12, TerminalSettingsManager.Instance.GetCurrentFontSize());
        }
    }
}
