using Microsoft.EntityFrameworkCore;
using System;

namespace EFCore
{
    public class Program
    {
        // Annotation (Attribute)
        [DbFunction()]
        public static double? GetAverageReviewScore(int itemId)
        {
            throw new NotImplementedException("c#에서 사용 금지!");
        }
        
        static void Main(string[] args)
        {
            DbCommands.InitializeDB(forceReset: false);

            // CRUD
            Console.WriteLine("명령어를 입력하세요");
            Console.WriteLine("[0] Force Reset");
            Console.WriteLine("[1] ShowItems");

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.ShowItems();
                        break;
                    case "2":

                        break;
                    case "3":
                        break;
                        
                }
            }

        }
    }
}

