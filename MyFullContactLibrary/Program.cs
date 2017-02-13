using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFullContactLibrary;
using Nito.AsyncEx;

namespace ConsoleAppMailEnter
{
    class Program
    {

        static int Main(string[] args)
        {
            try
            {
                return AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        static async Task<int> MainAsync(string[] args)
        {
            bool ProgramRunning = true;
            FullContactAPIClass yourClass = new FullContactAPIClass();

            while (ProgramRunning) {

                Console.WriteLine("skriv in en mail.");

                string input = Console.ReadLine();

                FullContactPerson FP = await yourClass.LookupPersonByEmailAsync(input);

                if(FP != null)
                    printInfo(FP);
            }

            return 1;
        }

        //prints all the info collected from the API
        static void printInfo(FullContactPerson P)
        {
            Console.WriteLine("");
            Console.WriteLine("likelihood:");
            Console.WriteLine(P.likelihood);
            Console.WriteLine("");

            Console.WriteLine("Contact Info:");

            foreach (string infoString in P.contactInfo)
                Console.WriteLine(infoString);

            Console.WriteLine("");

            Console.WriteLine("Social Media:");

            foreach (string infoString in P.socialProfiles)
                Console.WriteLine(infoString);

            Console.WriteLine("");

        }
    }
}
