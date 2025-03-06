using Lab1;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
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
            Console.WriteLine("8. Redo (повтор)");
            Console.WriteLine("9. Сохранить холст в файл");
            Console.WriteLine("10. Загрузить холст из файла");
            Console.WriteLine("11. Очистить холст");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие (0-11): ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите X координату (верхняя левая точка): ");
                    int rectX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату (верхняя левая точка): ");
                    int rectY = int.Parse(Console.ReadLine());
                    Console.Write("Введите ширину: ");
                    int width = int.Parse(Console.ReadLine());
                    Console.Write("Введите высоту: ");
                    int height = int.Parse(Console.ReadLine());
                    Console.Write("Введите символ для цвета фигуры (например, #): ");
                    char rectColor = Console.ReadLine()[0];
                    canvas.DrawShape(new Rectangle(rectX, rectY, width, height, rectColor));
                    break;

                case "2":
                    Console.Write("Введите X координату (центр): ");
                    int circleX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату (центр): ");
                    int circleY = int.Parse(Console.ReadLine());
                    Console.Write("Введите радиус: ");
                    int radius = int.Parse(Console.ReadLine());
                    Console.Write("Введите символ для цвета фигуры (например, *): ");
                    char circleColor = Console.ReadLine()[0];
                    canvas.DrawShape(new Circle(circleX, circleY, radius, circleColor));
                    break;

                case "3":
                    Console.Write("Введите X координату (верхняя точка): ");
                    int triX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y координату (верхняя точка): ");
                    int triY = int.Parse(Console.ReadLine());
                    Console.Write("Введите высоту: ");
                    int triHeight = int.Parse(Console.ReadLine());
                    Console.Write("Введите символ для цвета фигуры (например, &): ");
                    char triColor = Console.ReadLine()[0];
                    canvas.DrawShape(new Triangle(triX, triY, triHeight, triColor));
                    break;

                case "4":
                    Console.Write("Введите X начальную координату: ");
                    int eraseX = int.Parse(Console.ReadLine());
                    Console.Write("Введите Y начальную координату: ");
                    int eraseY = int.Parse(Console.ReadLine());
                    canvas.Erase(eraseX, eraseY);
                    break;

                case "5":
                    Console.Write("Введите текущий X координаты фигуры: ");
                    int oldX = int.Parse(Console.ReadLine());
                    Console.Write("Введите текущий Y координаты фигуры: ");
                    int oldY = int.Parse(Console.ReadLine());
                    Console.Write("Введите новую X координату: ");
                    int newX = int.Parse(Console.ReadLine());
                    Console.Write("Введите новую Y координату: ");
                    int newY = int.Parse(Console.ReadLine());
                    canvas.Move(oldX, oldY, newX, newY);
                    break;

                case "6":
                    Console.Write("Введите символ для фона (например, .): ");
                    char bgColor = Console.ReadLine()[0];
                    canvas.AddBackground(new Background(bgColor));
                    break;

                case "7":
                    canvas.Undo();
                    break;

                case "8":
                    canvas.Redo();
                    break;

                case "9":
                    Console.Write("Введите имя файла (например, canvas.txt): ");
                    string saveFile = Console.ReadLine();
                    canvas.Save(saveFile);
                    break;

                case "10":
                    Console.Write("Введите имя файла (например, canvas.txt): ");
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
}