using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using LibraryClient.ServiceReference1;

namespace LibraryClient
{
    class Program
    {
        #region
        static readonly List<Book> Books = new List<Book>
            {
                new Book
                {
                    Name = "name",
                    Author = "author",
                    PublishingYear = 2017,
                    BookType = BookType.FictionBook
                },
                new Book
                {
                    Name = "name2",
                    Author = "author2",
                    PublishingYear = 2017,
                    BookType = BookType.Magazine
                },
                new Book
                {
                    Name = "name2",
                    Author = "author2",
                    PublishingYear = 2017,
                    BookType = BookType.Magazine
                },
                new Book
                {
                    Name = "name3",
                    Author = "author3",
                    PublishingYear = 2017,
                    BookType = BookType.Other
                },
                new Book
                {
                    Name = "name3",
                    Author = "author3",
                    PublishingYear = 2017,
                    BookType = BookType.Other
                },
            };
        #endregion
        static void Main(string[] args)
        {
            var cb = new LibraryServiceCallback();
            var instanceContext = new InstanceContext(cb);
            var client = new LibraryServiceClient(instanceContext);
            cb.Client = client;

            client.LogIn("user1");
            AddSomeBooks(client);
            var books = new List<Book>();
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    books.Add(client.TakeBook(client.GetBook(i)));
                    PrintBook(books[i]);
                }

            }
            catch (FaultException<LibraryFaultModel> ex)
            {
                Console.WriteLine(ex.Detail.Text);
            }
            Console.WriteLine(client.ConfirmChoice());
            for (int i = 0; i < 5; i++)
            {
                client.ReturnBook(books[i]);
                books.Add(client.TakeBook(client.GetBook(i)));
                PrintBook(books[i]);
                client.ReturnBook(books[i]);
            }
            try
            {
                var authorBooks = client.GetAllBooks("author2");
                foreach (var e in authorBooks)
                {
                    PrintBook(e);
                }

            }
            catch (FaultException<LibraryFaultModel> ex)
            {
                Console.WriteLine(ex.Detail.Text);
            }
            client.LeaveLibrary();
        }

        static void AddSomeBooks(LibraryServiceClient client)
        {
            foreach (var book in Books)
            {
                client.AddBook(book);
            }
        }

        static void PrintBook(Book book)
        {
            if (book == null)
                Console.WriteLine("It is null");
            else
                Console.WriteLine(book.Id + ", author: " + book.Author);
        }
    }
}
