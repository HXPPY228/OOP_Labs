using Lab1;
using System;
using System.Collections.Generic;
using System.IO;

public class Canvas
{
    private readonly int Width;
    private readonly int Height;
    private char[,] CanvasArray;
    private List<char[,]> History;
    private int CurrentStep;
    private List<Shape> Shapes;
    private char CurrentBackgroundColor = ' ';
    private List<List<Shape>> ShapesHistory;
    private List<char> BackgroundHistory;

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        CanvasArray = new char[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CanvasArray[i, j] = ' ';
            }
        }
        History = new List<char[,]> { (char[,])CanvasArray.Clone() };
        CurrentStep = 0;
        Shapes = new List<Shape>();
        ShapesHistory = new List<List<Shape>> { new List<Shape>() };
        BackgroundHistory = new List<char> { ' ' };
    }

    public void ClearCanvas()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                CanvasArray[i, j] = ' ';
            }
        }
        CurrentBackgroundColor = ' ';
        if (Shapes != null) Shapes.Clear();
    }

    public void DrawShape(Shape shape)
    {
        if (shape == null) return;
        shape.Draw(CanvasArray);
        if (Shapes != null) Shapes.Add(shape);
        UpdateHistory();
    }

    public void AddBackground(Background background)
    {
        if (background == null) return;
        background.Apply(CanvasArray, CurrentBackgroundColor);
        CurrentBackgroundColor = background.Color;
        UpdateHistory();
    }

    public void Erase(int x, int y)
    {
        if (!IsWithinCanvas(x, y)) return;

        Shape shapeToErase = Shapes?.Find(s => s.X == x && s.Y == y);
        if (shapeToErase != null)
        {
            if (shapeToErase is Rectangle rect)
            {
                for (int i = rect.Y; i < rect.Y + rect.Height && i < CanvasArray.GetLength(0); i++)
                {
                    for (int j = rect.X; j < rect.X + rect.Width && j < CanvasArray.GetLength(1); j++)
                    {
                        if (IsWithinCanvas(j, i))
                            CanvasArray[i, j] = CurrentBackgroundColor;
                    }
                }
            }
            else if (shapeToErase is Circle circle)
            {
                for (int i = circle.Y - circle.Radius; i <= circle.Y + circle.Radius && i < CanvasArray.GetLength(0); i++)
                {
                    for (int j = circle.X - circle.Radius; j <= circle.X + circle.Radius && j < CanvasArray.GetLength(1); j++)
                    {
                        if (IsWithinCanvas(j, i) && (i - circle.Y) * (i - circle.Y) + (j - circle.X) * (j - circle.X) <= circle.Radius * circle.Radius)
                            CanvasArray[i, j] = CurrentBackgroundColor;
                    }
                }
            }
            else if (shapeToErase is Triangle triangle)
            {
                for (int i = triangle.Y; i < triangle.Y + triangle.Height && i < CanvasArray.GetLength(0); i++)
                {
                    int widthAtRow = 2 * (i - triangle.Y + 1) - 1;
                    int startX = triangle.X - (widthAtRow / 2);
                    for (int j = startX; j < startX + widthAtRow && j < CanvasArray.GetLength(1); j++)
                    {
                        if (IsWithinCanvas(j, i))
                            CanvasArray[i, j] = CurrentBackgroundColor;
                    }
                }
            }

            if (Shapes != null) Shapes.Remove(shapeToErase);
            UpdateHistory();
        }
        else
        {
            if (CanvasArray != null)
            {
                CanvasArray[y, x] = CurrentBackgroundColor;
                UpdateHistory();
            }
        }
    }

    public void Move(int oldX, int oldY, int newX, int newY)
    {
        Shape shapeToMove = Shapes?.Find(s => s.X == oldX && s.Y == oldY);
        if (shapeToMove != null)
        {

            shapeToMove.X = newX;
            shapeToMove.Y = newY;

            ClearCanvasWithoutShapeReset();
            foreach (Shape shape in Shapes)
            {
                shape.Draw(CanvasArray);
            }

            UpdateHistory();
        }
        else
        {
            Console.WriteLine("Фигура с указанными координатами не найдена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    private void ClearCanvasWithoutShapeReset()
    {
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                CanvasArray[i, j] = CurrentBackgroundColor;
            }
        }
    }

    private void UpdateHistory()
    {
        if (History == null || ShapesHistory == null) return;
        History.RemoveRange(CurrentStep + 1, History.Count - CurrentStep - 1);
        History.Add((char[,])CanvasArray.Clone());
        ShapesHistory.RemoveRange(CurrentStep + 1, ShapesHistory.Count - CurrentStep - 1);
        ShapesHistory.Add(Shapes.Select(s => s.Clone()).ToList());
        BackgroundHistory.RemoveRange(CurrentStep + 1, BackgroundHistory.Count - CurrentStep - 1);
        BackgroundHistory.Add(CurrentBackgroundColor);
        CurrentStep = History.Count - 1;
    }

    public void Undo()
    {
        if (CurrentStep > 0 && History != null && ShapesHistory != null)
        {
            CurrentStep--;
            CanvasArray = (char[,])History[CurrentStep].Clone();
            Shapes = ShapesHistory[CurrentStep].Select(s => s.Clone()).ToList(); // Восстанавливаем список фигур
            CurrentBackgroundColor = BackgroundHistory[CurrentStep];
        }
    }

    public void Redo()
    {
        if (CurrentStep < History.Count - 1 && History != null && ShapesHistory != null)
        {
            CurrentStep++;
            CanvasArray = (char[,])History[CurrentStep].Clone();
            Shapes = ShapesHistory[CurrentStep].Select(s => s.Clone()).ToList(); // Восстанавливаем список фигур
            CurrentBackgroundColor = BackgroundHistory[CurrentStep];
        }
    }

    public void Save(string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            for (int i = 0; i < Height; i++)
            {
                string line = "";
                for (int j = 0; j < Width; j++)
                {
                    line += CanvasArray[i, j];
                }
                writer.WriteLine(line);
            }
        }
    }

    public void Load(string filename)
    {
        if (!File.Exists(filename)) return;
        string[] lines = File.ReadAllLines(filename);
        for (int i = 0; i < Math.Min(Height, lines.Length); i++)
        {
            for (int j = 0; j < Math.Min(Width, lines[i].Length); j++)
            {
                CanvasArray[i, j] = lines[i][j];
            }
        }
        CurrentBackgroundColor = FindBackgroundColor();
        UpdateHistory();
        if (Shapes != null) Shapes.Clear();
    }

    private char FindBackgroundColor()
    {
        Dictionary<char, int> charCount = new Dictionary<char, int>();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                char c = CanvasArray[i, j];
                if (!charCount.ContainsKey(c)) charCount[c] = 0;
                charCount[c]++;
            }
        }

        char[] shapeChars = { '#', '*', '&' };
        char mostCommonNonShape = ' ';
        int maxCount = 0;

        foreach (var pair in charCount)
        {
            bool isShapeChar = Array.Exists(shapeChars, sc => sc == pair.Key);
            if (!isShapeChar && pair.Value > maxCount)
            {
                mostCommonNonShape = pair.Key;
                maxCount = pair.Value;
            }
        }

        return mostCommonNonShape;
    }

    public void Display()
    {
        Console.Clear();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                char currentChar = CanvasArray[i, j];
                Console.Write(currentChar == CurrentBackgroundColor ? CurrentBackgroundColor : currentChar);
            }
            Console.WriteLine();
        }
    }

    private bool IsWithinCanvas(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}