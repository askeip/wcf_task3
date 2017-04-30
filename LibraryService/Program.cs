using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace LibraryService
{
    public class Program
    {
        public static void Main()
        {
            var host = new ServiceHost(typeof(LibraryService));

            host.Open();

            Console.WriteLine("Press enter...");
            Console.ReadLine();

            host.Close();
        }
    }
}