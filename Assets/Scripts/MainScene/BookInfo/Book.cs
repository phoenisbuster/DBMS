using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book
{
    public int ISBN;
    public string Title;
    public string Genre;
    public string Publisher;
    public string AuthorName;
    public int price;
    public int year;
    public int AuthorID;

    public Book(int _ISBN, string _Title, int _price)
    {
        this.ISBN = _ISBN;
        this.Title = _Title;
        this.price = _price;
    }

    public Book(int _ISBN, string _Title, int _price, string _Genre = "", string _Publisher = "", int _year = 2000, int _AuthorID = -1, string _AuthorName = "")
    {
        this.ISBN = _ISBN;
        this.Title = _Title;
        this.price = _price;
        this.Genre = _Genre;
        this.Publisher = _Publisher;
        this.year = _year;
        this.AuthorID = _AuthorID;
        this.AuthorName = _AuthorName;
    }
}
