using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LibraryService
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ILibraryService
    {
        [OperationContract]
        void LogIn(string userName);

        [OperationContract(IsInitiating = false)]
        void AddBook(Book value);

        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(LibraryFaultModel))]
        Book GetBook(int id);

        [OperationContract(IsInitiating = false)]
        [FaultContract(typeof(LibraryFaultModel))]
        List<Book> GetAllBooks(string author);

        [OperationContract(IsInitiating = false)]
        Book TakeBook(Book book);

        [OperationContract(IsInitiating = false)]
        void ReturnBook(Book book);

        [OperationContract(IsInitiating = false)]
        string ConfirmChoice();

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void LeaveLibrary();
    }

    [DataContract]
    public class LibraryFaultModel
    {
        [DataMember]
        public string Text { get; set; }
    }

    [DataContract]
    public enum BookType
    {
        [EnumMember]
        FictionBook,
        [EnumMember]
        Magazine,
        [EnumMember]
        Other
    }
    
    [DataContract]
    public class Book
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public int PublishingYear { get; set; }
        [DataMember]
        public BookType BookType { get; set; }

        public override int GetHashCode()
        {
            return Id * PublishingYear * Name.Length;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;

            Book otherBook = (Book)obj;
            return Id == otherBook.Id && Name == otherBook.Name && Author == otherBook.Author &&
                   PublishingYear == otherBook.PublishingYear && BookType == otherBook.BookType;
        }
    }
}
