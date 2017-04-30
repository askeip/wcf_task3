using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LibraryService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class LibraryService : ILibraryService
    {
        private static readonly List<Book> AvailableBooks = new List<Book>();
        private static readonly Dictionary<int, Book> TakenBooks = new Dictionary<int, Book>();

        private static readonly Dictionary<string, List<Book>> UnnamedDictionary = new Dictionary<string, List<Book>>();

        private string userName;
        private List<Book> chosenBooks;

        public void LogIn(string userName)
        {
            this.userName = userName;
            chosenBooks = new List<Book>();
            if (!UnnamedDictionary.ContainsKey(userName))
                UnnamedDictionary.Add(userName, new List<Book>());
            Console.WriteLine(userName + " came to library");
        }

        public void AddBook(Book value)
        {
            value.Id = AvailableBooks.Count;
            AvailableBooks.Add(value);
        }

        public Book GetBook(int id)
        {
            if (AvailableBooks.Count > id)
            {
                var book = AvailableBooks[id];
                if (book == null)
                    throw new FaultException<LibraryFaultModel>(new LibraryFaultModel
                    {
                        Text = "You cant get this book, because it's already taken"
                    });
                return book;
            }
            throw new FaultException<LibraryFaultModel>(new LibraryFaultModel
                {
                    Text = "This book is not exist"
                });
        }

        public List<Book> GetAllBooks(string author)
        {
            var books = AvailableBooks.Where(z => z != null && z.Author == author)
                .ToList();
            if (books.Count == 0)
                throw new FaultException<LibraryFaultModel>(new LibraryFaultModel { Text = "Books of this " +
                                                                                           "author were not found" });
            return books;
        }

        public Book TakeBook(Book book)
        {
            var bookToTake = AvailableBooks.FirstOrDefault(z => z != null && z.Equals(book));
            if (bookToTake != null)
            {
                AvailableBooks[bookToTake.Id] = null;
                TakenBooks.Add(bookToTake.Id, bookToTake);
                UnnamedDictionary[userName].Add(bookToTake);
            }
            return bookToTake;
        }

        public void ReturnBook(Book book)
        {
            if (!UnnamedDictionary[userName].Any(z => z != null && z.Equals(book))) return;
            TakenBooks.Remove(book.Id);
            UnnamedDictionary[userName]
                .Remove(book);
            AvailableBooks[book.Id] = book;
        }

        public string ConfirmChoice()
        {
            if (UnnamedDictionary[userName].Count + chosenBooks.Count <= 5)
            {
                UnnamedDictionary[userName] = UnnamedDictionary[userName].Concat(chosenBooks)
                    .ToList();
                chosenBooks = new List<Book>();
                return "Books taken";
            }
            return "You cant take more than 5 books";
        }

        public void LeaveLibrary()
        {
            Console.WriteLine(userName + " left library");
        }
    }
}
