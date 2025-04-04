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
        private const string TestFilePath = "test.txt";

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
        }

        [TestMethod]
        public void SaveDocument_PlainText_ShouldCreateFile()
        {
            var doc = new Document(DocumentType.PlainText);
            doc.AppendText("Test");
            DocumentManager.SaveDocument(doc, TestFilePath);
            Assert.IsTrue(File.Exists(TestFilePath));
        }

        [TestMethod]
        public void OpenDocument_ValidFile_ShouldLoadContent()
        {
            File.WriteAllText(TestFilePath, "Test");
            var doc = DocumentManager.OpenDocument(TestFilePath);
            Assert.AreEqual("Test", doc.GetOriginalText());
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
