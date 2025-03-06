using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lab1;

[TestClass]
public class CanvasTests
{
    [TestMethod]
    public void DrawRectangle_ShouldDrawCorrectly()
    {
        Canvas canvas = new Canvas(10, 10);
        Rectangle rect = new Rectangle(2, 2, 4, 3, '#');

        canvas.DrawShape(rect);

        char[,] expected = new char[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                expected[i, j] = ' ';

        for (int i = 2; i < 5; i++)
            for (int j = 2; j < 6; j++)
                expected[i, j] = '#';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
    }

    [TestMethod]
    public void DrawCircle_ShouldDrawCorrectly()
    {
        Canvas canvas = new Canvas(10, 10);
        Circle circle = new Circle(5, 5, 3, '*');

        circle.Draw(canvas.CanvasArray);

        char[,] expected = new char[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                expected[i, j] = ' ';

        for (int i = 2; i <= 8; i++)
            for (int j = 2; j <= 8; j++)
                if ((i - 5) * (i - 5) + (j - 5) * (j - 5) <= 9)
                    expected[i, j] = '*';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
    }

    [TestMethod]
    public void DrawTriangle_ShouldDrawCorrectly()
    {
        Canvas canvas = new Canvas(10, 10);
        Triangle triangle = new Triangle(5, 2, 3, '&');

        triangle.Draw(canvas.CanvasArray);

        char[,] expected = new char[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                expected[i, j] = ' ';

        for (int i = 2; i < 5; i++)
        {
            int widthAtRow = 2 * (i - 2 + 1) - 1;
            int startX = 5 - (widthAtRow / 2);
            for (int j = startX; j < startX + widthAtRow; j++)
                if (j >= 0 && j < 10)
                    expected[i, j] = '&';
        }

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
    }

    [TestMethod]
    public void ClearCanvas_ShouldClearCanvasAndShapes()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);

        canvas.ClearCanvas();

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(0, canvas.Shapes.Count);
        Assert.AreEqual(' ', canvas.CurrentBackgroundColor);
    }

    [TestMethod]
    public void ClearCanvas_OnEmptyCanvas_ShouldRemainEmpty()
    {
        Canvas canvas = new Canvas(5, 5);

        canvas.ClearCanvas();

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(0, canvas.Shapes.Count);
        Assert.AreEqual(' ', canvas.CurrentBackgroundColor);
    }

    [TestMethod]
    public void AddBackground_ShouldApplyNewBackground()
    {
        Canvas canvas = new Canvas(5, 5);
        Background bg = new Background('.');

        canvas.AddBackground(bg);

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = '.';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual('.', canvas.CurrentBackgroundColor);
    }

    [TestMethod]
    public void AddBackground_WithShapes_ShouldPreserveShapes()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);
        Background bg = new Background('.');

        canvas.AddBackground(bg);

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = (i >= 1 && i <= 2 && j >= 1 && j <= 2) ? '#' : '.';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual('.', canvas.CurrentBackgroundColor);
    }

    [TestMethod]
    public void Erase_ShouldRemoveShape()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);

        canvas.Erase(1, 1);

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(0, canvas.Shapes.Count);
    }

    [TestMethod]
    public void Move_ShouldMoveShape()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);

        canvas.Move(1, 1, 2, 2);

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = (i >= 2 && i <= 3 && j >= 2 && j <= 3) ? '#' : ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(2, canvas.Shapes[0].X);
        Assert.AreEqual(2, canvas.Shapes[0].Y);
    }

    [TestMethod]
    public void ClearCanvasWithoutShapeReset_ShouldClearCanvasButKeepShapes()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);

        canvas.ClearCanvasWithoutShapeReset();

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(1, canvas.Shapes.Count);
    }

    [TestMethod]
    public void Undo_ShouldRevertLastAction()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);

        canvas.Undo();

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
    }

    [TestMethod]
    public void UpdateHistory_AfterMultipleActions_ShouldUndoCorrectly()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect1 = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect1);
        Rectangle rect2 = new Rectangle(2, 2, 2, 2, '*');
        canvas.DrawShape(rect2);

        canvas.Undo();
        canvas.Undo();

        char[,] expected = new char[5, 5];
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                expected[i, j] = ' ';

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(0, canvas.Shapes.Count);
    }

    [TestMethod]
    public void Redo_ShouldRestoreNextState()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect1 = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect1);
        Rectangle rect2 = new Rectangle(2, 2, 2, 2, '*');
        canvas.DrawShape(rect2);
        canvas.Undo();

        canvas.Redo();

        char[,] expected = new char[5, 5] { { ' ', ' ', ' ', ' ', ' ' },
                                            { ' ', '#', '#', ' ', ' ' },
                                            { ' ', '#', '*', '*', ' ' },
                                            { ' ', ' ', '*', '*', ' ' },
                                            { ' ', ' ', ' ', ' ', ' ' },};

        CollectionAssert.AreEqual(expected, canvas.CanvasArray);
        Assert.AreEqual(2, canvas.Shapes.Count);
    }

    [TestMethod]
    public void Redo_AtLastState_ShouldDoNothing()
    {
        Canvas canvas = new Canvas(5, 5);
        Rectangle rect = new Rectangle(1, 1, 2, 2, '#');
        canvas.DrawShape(rect);
        char[,] before = (char[,])canvas.CanvasArray.Clone();

        canvas.Redo();

        CollectionAssert.AreEqual(before, canvas.CanvasArray);
        Assert.AreEqual(1, canvas.Shapes.Count);
    }

    [TestMethod]
    public void Save_ShouldSaveCanvasToFile()
    {
        Canvas canvas = new Canvas(3, 3);
        Rectangle rect = new Rectangle(0, 0, 2, 2, '#');
        canvas.DrawShape(rect);
        string filename = "test_save.txt";

        canvas.Save(filename);

        string[] lines = File.ReadAllLines(filename);
        Assert.AreEqual(3, lines.Length);
        Assert.AreEqual("## ", lines[0]);
        Assert.AreEqual("## ", lines[1]);
        Assert.AreEqual("   ", lines[2]);

        File.Delete(filename);
    }

    [TestMethod]
    public void Save_EmptyCanvas_ShouldSaveEmptyLines()
    {
        Canvas canvas = new Canvas(2, 2);
        string filename = "test_save_empty.txt";

        canvas.Save(filename);

        string[] lines = File.ReadAllLines(filename);
        Assert.AreEqual(2, lines.Length);
        Assert.AreEqual("  ", lines[0]);
        Assert.AreEqual("  ", lines[1]);

        File.Delete(filename);
    }

    [TestMethod]
    public void Load_ShouldLoadCanvasFromFile()
    {
        Canvas canvas = new Canvas(3, 3);
        string filename = "test_load.txt";
        File.WriteAllLines(filename, ["## ", "## ", "   "]);

        canvas.Load(filename);

        char[,] expected = new char[3, 3]
        {
        { '#', '#', ' ' },
        { '#', '#', ' ' },
        { ' ', ' ', ' ' }
        };
        CollectionAssert.AreEqual(expected, canvas.CanvasArray);

        File.Delete(filename);
    }
}