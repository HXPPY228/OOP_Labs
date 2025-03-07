using Lab1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
public class Canvas
{
    private readonly int Width;
    private readonly int Height;
    private char[,] _canvasArray;
    private List<char[,]> History;
    private int CurrentStep;
    private List<Shape> _shapes;
    private char _currentBackgroundColor = ' ';
    private List<List<Shape>> ShapesHistory;
    private List<char> BackgroundHistory;

    public char[,] CanvasArray 
    {
        get { return _canvasArray; }
        set { _canvasArray = value; } 
    }
    public List<Shape> Shapes
    { 
        get { return _shapes; }
        set { _shapes = value; }
    }
    public char CurrentBackgroundColor
    {
        get { return _currentBackgroundColor; }
        set { _currentBackgroundColor = value; }
    }
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

        Shape shapeToErase = Shapes?.FindLast(s => s.X == x && s.Y == y);
        if (shapeToErase != null)
        {
            Shapes.Remove(shapeToErase);

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

    public void Move(int oldX, int oldY, int newX, int newY)
    {
        Shape shapeToMove = Shapes?.FindLast(s => s.X == oldX && s.Y == oldY);
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

    public void ClearCanvasWithoutShapeReset()
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
            Shapes = ShapesHistory[CurrentStep].Select(s => s.Clone()).ToList();
            CurrentBackgroundColor = BackgroundHistory[CurrentStep];
        }
    }

    public void Redo()
    {
        if (CurrentStep < History.Count - 1 && History != null && ShapesHistory != null)
        {
            CurrentStep++;
            CanvasArray = (char[,])History[CurrentStep].Clone();
            Shapes = ShapesHistory[CurrentStep].Select(s => s.Clone()).ToList();
            CurrentBackgroundColor = BackgroundHistory[CurrentStep];
        }
    }

    public void Save(string filename)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented
        };
        var data = new
        {
            BackgroundColor = CurrentBackgroundColor,
            Shapes = Shapes
        };
        string json = JsonConvert.SerializeObject(data, settings);
        File.WriteAllText(filename, json);
    }

    public void Load(string filename)
    {
        if (!File.Exists(filename)) return;

        string json = File.ReadAllText(filename);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        var data = JsonConvert.DeserializeAnonymousType(json, new { BackgroundColor = ' ', Shapes = new List<Shape>() }, settings);

        CurrentBackgroundColor = data.BackgroundColor;
        Shapes = data.Shapes;

        ClearCanvasWithoutShapeReset();
        foreach (Shape shape in Shapes)
        {
            shape.Draw(CanvasArray);
        }

        UpdateHistory();
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