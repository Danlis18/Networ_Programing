namespace _001_PortScaner
{

    // Сокет - однонаправлений канал двосторонього зв'язку.
    //Сокети бувають:
    //1. Потокові (TCP) - забезпечують надійну передачу даних, гарантують доставку та порядок доставки.
    //2. Датаграмні (UDP) - не гарантують доставку, можуть втрачати пакети, але швидші за TCP.
    //3. Сирові (Raw) - дозволяють працювати з мережевими протоколами на низькому рівні, використовуються для створення власних протоколів або для роботи з існуючими на рівні IP.

   //Сокети реалізують мережевий та транспортний рівні моделі OSI.
   //end point - IP + PORT.
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}