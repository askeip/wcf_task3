using System;
using LibraryClient.ServiceReference1;

namespace LibraryClient
{
    public class LibraryServiceCallback : ILibraryServiceCallback
    {
        public LibraryServiceClient Client { get; set; }
        public void AnswerToAdminCallback(string question)
        {
            Console.WriteLine(question);
            Client.Talk("Завтра принесу");
        }
    }
}