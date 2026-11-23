using System;

namespace TodoListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили: Лукьяненко Даниил, Мироненко Данил, гр. 3831.9");

            Console.Write("Введите имя: ");
            string? firstName = Console.ReadLine();

            Console.Write("Введите фамилию: ");
            string? lastName = Console.ReadLine();

            Console.Write("Введите год рождения: ");
            string? birthYearInput = Console.ReadLine();

            if (int.TryParse(birthYearInput, out int birthYear))
            {
                int currentYear = DateTime.Now.Year;
                int age = currentYear - birthYear;
                Console.WriteLine($"Добавлен пользователь {firstName} {lastName}, возраст - {age}");
            }
            else
            {
                Console.WriteLine("Некорректный ввод года рождения.");
            }
        }
    }
}
