using System;
using System.Collections.Generic;
using System.IO;

// Абстрактный базовый класс для фигур (Абстракция)
public abstract class Shape
{
    public int X { get; set; } // Координата X
    public int Y { get; set; } // Координата Y
    public char Color { get; set; } // Символ для рисования

    protected Shape(int x, int y, char color)
    {
        X = x;
        Y = y;
        Color = color;
    }

    public abstract void Draw(char[,] canvas);
}

// Конкретная фигура — Круг (Наследование, Полиморфизм)
public class Circle : Shape
{
    public int Radius { get; set; }

    public Circle(int x, int y, int radius, char color = '*') : base(x, y, color)
    {
        Radius = radius;
    }

    public override void Draw(char[,] canvas)
    {
        for (int i = Y - Radius; i <= Y + Radius; i++)
        {
            for (int j = X - Radius; j <= X + Radius; j++)
            {
                if (IsInsideCircle(i, j) && IsWithinCanvas(i, j, canvas))
                {
                    canvas[i, j] = Color;
                }
            }
        }
    }

    private bool IsInsideCircle(int i, int j)
    {
        return (i - Y) * (i - Y) + (j - X) * (j - X) <= Radius * Radius;
    }

    private bool IsWithinCanvas(int i, int j, char[,] canvas)
    {
        return i >= 0 && i < canvas.GetLength(0) && j >= 0 && j < canvas.GetLength(1);
    }
}

// Конкретная фигура — Прямоугольник
public class Rectangle : Shape
{
    public int Width { get; set; }
    public int Height { get; set; }

    public Rectangle(int x, int y, int width, int height, char color = '#') : base(x, y, color)
    {
        Width = width;
        Height = height;
    }

    public override void Draw(char[,] canvas)
    {
        for (int i = Y; i < Y + Height; i++)
        {
            for (int j = X; j < X + Width; j++)
            {
                if (IsWithinCanvas(i, j, canvas))
                {
                    canvas[i, j] = Color;
                }
            }
        }
    }

    private bool IsWithinCanvas(int i, int j, char[,] canvas)
    {
        return i >= 0 && i < canvas.GetLength(0) && j >= 0 && j < canvas.GetLength(1);
    }
}

// Класс для фона (Инкапсуляция)
public class Background
{
    public char Color { get; set; }

    public Background(char color = ' ')
    {
        Color = color;
    }

    public void Apply(char[,] canvas)
    {
        for (int i = 0; i < canvas.GetLength(0); i++)
        {
            for (int j = 0; j < canvas.GetLength(1); j++)
            {
                canvas[i, j] = Color;
            }
        }
    }
}

// Класс для управления холстом (Канвас)
public class Canvas
{
    private readonly int Width;
    private readonly int Height;
    private char[,] CanvasArray { get; set; }
    private List<char[,]> History { get; set; }
    private int CurrentStep { get; set; }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        CanvasArray = new char[height, width];
        ClearCanvas();
        History = new List<char[,]> { (char[,])CanvasArray.Clone() };
        CurrentStep = 0;
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
    }

    public void DrawShape(Shape shape)
    {
        shape.Draw(CanvasArray);
        UpdateHistory();
    }

    public void AddBackground(Background background)
    {
        background.Apply(CanvasArray);
        UpdateHistory();
    }

    public void Erase(int x, int y)
    {
        if (IsWithinCanvas(x, y))
        {
            CanvasArray[y, x] = ' ';
            UpdateHistory();
        }
    }

    public void Move(Shape shape, int newX, int newY)
    {
        Erase(shape.X, shape.Y);
        shape.X = newX;
        shape.Y = newY;
        shape.Draw(CanvasArray);
        UpdateHistory();
    }

    private void UpdateHistory()
    {
        History.RemoveRange(CurrentStep + 1, History.Count - CurrentStep - 1);
        History.Add((char[,])CanvasArray.Clone());
        CurrentStep = History.Count - 1;
    }

    public void Undo()
    {
        if (CurrentStep > 0)
        {
            CurrentStep--;
            CanvasArray = (char[,])History[CurrentStep].Clone();
        }
    }

    public void Redo()
    {
        if (CurrentStep < History.Count - 1)
        {
            CurrentStep++;
            CanvasArray = (char[,])History[CurrentStep].Clone();
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
        if (File.Exists(filename))
        {
            string[] lines = File.ReadAllLines(filename);
            for (int i = 0; i < Math.Min(Height, lines.Length); i++)
            {
                for (int j = 0; j < Math.Min(Width, lines[i].Length); j++)
                {
                    CanvasArray[i, j] = lines[i][j];
                }
            }
            UpdateHistory();
        }
    }

    public void Display()
    {
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

class Program
{
    static void Main(string[] args)
    {
        Canvas canvas = new Canvas(50, 20);

        while (true)
        {
            // Показываем меню
            Console.Clear();
            canvas.Display();
            Console.WriteLine("\nКонтекстное меню:");
            Console.WriteLine("1. Нарисовать прямоугольник");
            Console.WriteLine("2. Нарисовать круг");
            Console.WriteLine("3. Стереть объект (по координатам)");
            Console.WriteLine("4. Переместить фигуру");
            Console.WriteLine("5. Добавить фон");
            Console.WriteLine("6. Undo (отмена)");
            Console.WriteLine("7. Redo (повтор)");
            Console.WriteLine("8. Сохранить холст в файл");
            Console.WriteLine("9. Загрузить холст из файла");
            Console.WriteLine("10. Очистить холст");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие (0-10): ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": // Нарисовать прямоугольник
                    Console.Write("Введите X координату: ");
                    int rectX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату: ");
                    int rectY = int.Parse(Console.ReadLine());
                    Console.Write("Введите ширину: ");
                    int width = int.Parse(Console.ReadLine());
                    Console.Write("Введите высоту: ");
                    int height = int.Parse(Console.ReadLine());
                    Console.Write("Введите символ (например, #): ");
                    char rectColor = Console.ReadLine()[0];
                    canvas.DrawShape(new Rectangle(rectX, rectY, width, height, rectColor));
                    break;

                case "2": // Нарисовать круг
                    Console.Write("Введите X координату: ");
                    int circleX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату: ");
                    int circleY = int.Parse(Console.ReadLine());
                    Console.Write("Введите радиус: ");
                    int radius = int.Parse(Console.ReadLine());
                    Console.Write("Введите символ (например, *): ");
                    char circleColor = Console.ReadLine()[0];
                    canvas.DrawShape(new Circle(circleX, circleY, radius, circleColor));
                    break;

                case "3": // Стереть объект
                    Console.Write("Введите X координату: ");
                    int eraseX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату: ");
                    int eraseY = int.Parse(Console.ReadLine());
                    canvas.Erase(eraseX, eraseY);
                    break;

                case "4": // Переместить фигуру
                    Console.Write("Введите текущий X координаты фигуры: ");
                    int oldX = int.Parse(Console.ReadLine());
                    Console.Write("Введите текущий Y координаты фигуры: ");
                    int oldY = int.Parse(Console.ReadLine());
                    Console.Write("Введите новую X координату: ");
                    int newX = int.Parse(Console.ReadLine());
                    Console.Write("Введите новую Y координату: ");
                    int newY = int.Parse(Console.ReadLine());
                    // Здесь упрощённо предполагаем, что фигура — это круг или прямоугольник на определённых координатах
                    // В реальном приложении нужно бы хранить список фигур и выбирать конкретную
                    Shape shapeToMove = new Circle(oldX, oldY, 2, '*'); // Пример, можно заменить на Rectangle
                    canvas.Move(shapeToMove, newX, newY);
                    break;

                case "5": // Добавить фон
                    Console.Write("Введите символ для фона (например, .): ");
                    char bgColor = Console.ReadLine()[0];
                    canvas.AddBackground(new Background(bgColor));
                    break;

                case "6": // Undo
                    canvas.Undo();
                    break;

                case "7": // Redo
                    canvas.Redo();
                    break;

                case "8": // Сохранить
                    Console.Write("Введите имя файла (например, canvas.txt): ");
                    string saveFile = Console.ReadLine();
                    canvas.Save(saveFile);
                    break;

                case "9": // Загрузить
                    Console.Write("Введите имя файла (например, canvas.txt): ");
                    string loadFile = Console.ReadLine();
                    canvas.Load(loadFile);
                    break;

                case "10": // Очистить холст
                    canvas.ClearCanvas();
                    break;

                case "0": // Выход
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}