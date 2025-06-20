namespace Wijoyo_fdtest.Domain.Events;

public class BookDeletedEvent : BaseEvent
{
    public BookDeletedEvent(Book book)
    {
        Book = book;
    }

    public Book Book { get; }
}
