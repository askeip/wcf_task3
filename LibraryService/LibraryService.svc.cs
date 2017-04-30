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
        private class TakingInfo
        {
            public DateTime DateOfTaking;
            public Book Book;

            public TakingInfo(Book book)
            {
                DateOfTaking = DateTime.UtcNow;
                Book = book;
            }
        }

        private ILibraryCallback callback = null;

        private static readonly List<Book> AvailableBooks = new List<Book>();
        private static readonly Dictionary<int, Book> TakenBooks = new Dictionary<int, Book>();

        private static readonly Dictionary<string, List<TakingInfo>> UserActivity = new Dictionary<string, List<TakingInfo>>();

        private string userName;
        private List<Book> chosenBooks;

        public void LogIn(string userName)
        {
            callback = OperationContext.Current.GetCallbackChannel<ILibraryCallback>();
            this.userName = userName;
            chosenBooks = new List<Book>();
            if (!UserActivity.ContainsKey(userName))
                UserActivity.Add(userName, new List<TakingInfo>());
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
                UserActivity[userName].Add(new TakingInfo(bookToTake));
            }
            return bookToTake;
        }

        public void ReturnBook(Book book)
        {
            if (!UserActivity[userName].Any(z => z != null && z.Book.Equals(book))) return;
            TakenBooks.Remove(book.Id);
            UserActivity[userName].Remove(UserActivity[userName].First(z => z.Book.Equals(book)));
            AvailableBooks[book.Id] = book;
        }

        public string ConfirmChoice()
        {
            if (UserActivity[userName].Any(z => DateTime.UtcNow.Subtract(z.DateOfTaking) > TimeSpan.FromDays(30)))
            {
                callback.AnswerToAdminCallback("Где книги и когда принесешь?");
                return null;
            }
            if (UserActivity[userName].Count + chosenBooks.Count <= 5)
            {
                UserActivity[userName] = UserActivity[userName].Concat(chosenBooks.Select(z => new TakingInfo(z)))
                    .ToList();
                chosenBooks = new List<Book>();
                return "Books taken";
            }
            return "You cant take more than 5 books";
        }

        public void Talk(string speech)
        {
            Console.WriteLine(speech);
        }

        public void LeaveLibrary()
        {
            Console.WriteLine(userName + " left library");
        }
    }
}
