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

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        CanvasArray = new char[height, width];
        History = new List<char[,]> { (char[,])CanvasArray.Clone() };
        CurrentStep = 0;
        Shapes = new List<Shape>(); // Гарантированная инициализация
    }

    public void ClearCanvas()
    {
        if (CanvasArray == null) return; // Проверка на случай, если массив не инициализирован
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                CanvasArray[i, j] = ' ';
            }
        }
        if (Shapes != null) Shapes.Clear(); // Проверка на null перед очисткой
    }

    public void DrawShape(Shape shape)
    {
        if (shape == null || CanvasArray == null) return; // Проверка на null
        shape.Draw(CanvasArray);
        if (Shapes != null) Shapes.Add(shape);
        UpdateHistory();
    }

    public void AddBackground(Background background)
    {
        if (background == null || CanvasArray == null) return; // Проверка на null
        background.Apply(CanvasArray);
        UpdateHistory();
    }

    public void Erase(int x, int y)
    {
        if (CanvasArray == null || !IsWithinCanvas(x, y)) return;
        CanvasArray[y, x] = ' ';
        if (Shapes != null) Shapes.RemoveAll(s => s.X == x && s.Y == y);
        UpdateHistory();
    }

    public void Move(int oldX, int oldY, int newX, int newY)
    {
        if (CanvasArray == null) return;
        Shape shapeToMove = Shapes?.Find(s => s.X == oldX && s.Y == oldY);
        if (shapeToMove != null)
        {
            Erase(oldX, oldY);
            shapeToMove.X = newX;
            shapeToMove.Y = newY;
            shapeToMove.Draw(CanvasArray);
            UpdateHistory();
        }
        else
        {
            Console.WriteLine("Фигура с указанными координатами не найдена. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    private void UpdateHistory()
    {
        if (CanvasArray == null || History == null) return;
        History.RemoveRange(CurrentStep + 1, History.Count - CurrentStep - 1);
        History.Add((char[,])CanvasArray.Clone());
        CurrentStep = History.Count - 1;
    }

    public void Undo()
    {
        if (CurrentStep > 0 && History != null && CanvasArray != null)
        {
            CurrentStep--;
            CanvasArray = (char[,])History[CurrentStep].Clone();
            if (Shapes != null) Shapes.Clear();
        }
    }

    public void Redo()
    {
        if (CurrentStep < History.Count - 1 && History != null && CanvasArray != null)
        {
            CurrentStep++;
            CanvasArray = (char[,])History[CurrentStep].Clone();
            if (Shapes != null) Shapes.Clear();
        }
    }

    public void Save(string filename)
    {
        if (CanvasArray == null) return;
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
        if (CanvasArray == null || !File.Exists(filename)) return;
        string[] lines = File.ReadAllLines(filename);
        for (int i = 0; i < Math.Min(Height, lines.Length); i++)
        {
            for (int j = 0; j < Math.Min(Width, lines[i].Length); j++)
            {
                CanvasArray[i, j] = lines[i][j];
            }
        }
        UpdateHistory();
        if (Shapes != null) Shapes.Clear();
    }

    public void Display()
    {
        if (CanvasArray == null) return;
        Console.Clear();
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Console.Write(CanvasArray[i, j]);
            }
            Console.WriteLine();
        }
    }

    private bool IsWithinCanvas(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}