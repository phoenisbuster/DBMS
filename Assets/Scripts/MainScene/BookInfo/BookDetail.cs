using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;

public class BookDetail : MonoBehaviour
{
    public TMP_Text bookName;
    public TMP_Text bookISBN;
    public TMP_Text bookPrice;
    public TMP_Text Genre;
    public TMP_Text Publisher;
    public TMP_Text Year;
    public TMP_Text AuthorName;
    public TMP_Text numOfUser;
    public Button ExitBtn;
    public Button BuyBtn;
    public static Action OnClickExit;
    public static Action<int> OnClickBuy;
    private int bookID = -1;
    private int AuthorID;

    public void SetBookDetail(Book book, int count)
    {
        numOfUser.text = count.ToString();
        bookID = book.ISBN;
        AuthorID = book.AuthorID;

        bookName.text = book.Title;
        bookISBN.text = bookID.ToString();
        bookPrice.text = book.price.ToString();
        Genre.text = book.Genre;
        Publisher.text = book.Publisher;
        Year.text = book.year.ToString();
        AuthorName.text = book.AuthorName;
    }

    public void OnCLickCancel()
    {
        OnClickExit?.Invoke();
    }

    public void OnCLickBuyItem()
    {
        OnClickBuy?.Invoke(bookID);
    }
}
