using Lab1;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.SetWindowSize(120, 50);
        Canvas canvas = new Canvas(80, 30);

        while (true)
        {
            Console.Clear();
            canvas.Display();
            Console.WriteLine("\nКонтекстное меню:");
            Console.WriteLine("1. Нарисовать прямоугольник");
            Console.WriteLine("2. Нарисовать круг");
            Console.WriteLine("3. Нарисовать равнобедренный треугольник");
            Console.WriteLine("4. Стереть фигуру");
            Console.WriteLine("5. Переместить фигуру");
            Console.WriteLine("6. Добавить фон");
            Console.WriteLine("7. Undo (отмена)");
            Console.WriteLine("8. Redo (вернуться)");
            Console.WriteLine("9. Сохранить холст в файл");
            Console.WriteLine("10. Загрузить холст из файла");
            Console.WriteLine("11. Очистить холст");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие (0-11): ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    int rectX = ReadInt("Введите X координату (верхняя левая точка): ");
                    int rectY = ReadInt("Введите Y координату (верхняя левая точка): ");
                    int width = ReadInt("Введите ширину: ");
                    int height = ReadInt("Введите высоту: ");
                    char rectColor = ReadChar("Введите символ для цвета фигуры (например, #): ");
                    canvas.DrawShape(new Rectangle(rectX, rectY, width, height, rectColor));
                    break;

                case "2":
                    int circleX = ReadInt("Введите X координату (центр): ");
                    int circleY = ReadInt("Введите Y координату (центр): ");
                    int radius = ReadInt("Введите радиус: ");
                    char circleColor = ReadChar("Введите символ для цвета фигуры (например, *): ");
                    canvas.DrawShape(new Circle(circleX, circleY, radius, circleColor));
                    break;

                case "3":
                    int triX = ReadInt("Введите X координату (верхняя точка): ");
                    int triY = ReadInt("Введите Y координату (верхняя точка): ");
                    int triHeight = ReadInt("Введите высоту: ");
                    char triColor = ReadChar("Введите символ для цвета фигуры (например, &): ");
                    canvas.DrawShape(new Triangle(triX, triY, triHeight, triColor));
                    break;

                case "4":
                    int eraseX = ReadInt("Введите X начальную координату: ");
                    int eraseY = ReadInt("Введите Y начальную координату: ");
                    canvas.Erase(eraseX, eraseY);
                    break;

                case "5":
                    int oldX = ReadInt("Введите текущий X координаты фигуры: ");
                    int oldY = ReadInt("Введите текущий Y координаты фигуры: ");
                    int newX = ReadInt("Введите новую X координату: ");
                    int newY = ReadInt("Введите новую Y координату: ");
                    canvas.Move(oldX, oldY, newX, newY);
                    break;

                case "6":
                    char bgColor = ReadChar("Введите символ для фона (например, .): ");
                    canvas.AddBackground(new Background(bgColor));
                    break;

                case "7":
                    canvas.Undo();
                    break;

                case "8":
                    canvas.Redo();
                    break;

                case "9":
                    Console.Write("Введите имя файла (например, canvas): ");
                    string saveFile = Console.ReadLine();
                    canvas.Save(saveFile);
                    break;

                case "10":
                    Console.Write("Введите имя файла (например, canvas): ");
                    string loadFile = Console.ReadLine();
                    canvas.Load(loadFile);
                    break;

                case "11":
                    canvas.ClearCanvas();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, введите целое число.");
            }
        }
    }
    static char ReadChar(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (input.Length == 1)
            {
                return input[0];
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, введите ровно один символ.");
            }
        }
    }
}