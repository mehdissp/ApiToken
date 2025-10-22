using System;
using System.Globalization;

public class ShamsiToMiladiConverter
{
    public static DateTime ConvertShamsiToMiladi(string shamsiDate)
    {
        // جدا کردن قسمت‌های تاریخ
        string[] parts = shamsiDate.Split('/');

        if (parts.Length != 3)
            throw new ArgumentException("فرمت تاریخ نامعتبر است");

        int year = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int day = int.Parse(parts[2]);

        // ایجاد شیء PersianCalendar
        PersianCalendar pc = new PersianCalendar();

        // تبدیل به میلادی
        DateTime miladiDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);

        return miladiDate;
    }
}

// نمونه استفاده
class Program
{
    static void Main()
    {
        string shamsiDate = "1404/05/01";
        DateTime miladiDate = ShamsiToMiladiConverter.ConvertShamsiToMiladi(shamsiDate);

        Console.WriteLine($"تاریخ شمسی: {shamsiDate}");
        Console.WriteLine($"تاریخ میلادی: {miladiDate:yyyy/MM/dd}");
        // خروجی: 2025/07/23
    }
}