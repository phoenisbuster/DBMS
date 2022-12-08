using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookComponent : MonoBehaviour
{
    public TMP_Text bookName;
    public TMP_Text bookPrice;
    public TMP_Text numOfUser;
    public int bookID = -1;
    public static Action<Book, int> OnClickDetail;
    public static Action<int> OnClickBuy;
    private Book bookInstance;

    public void DisplayData(string id, string name, string price, string genre, string publisher, string year, string authorID, string authorName)
    {
        bookID = Int32.Parse(id);
        bookName.text = name;
        bookPrice.text = price;

        CreateBookInstance(Int32.Parse(price), genre, publisher, Int32.Parse(year), Int32.Parse(authorID), authorName);
    }

    public void DisplayTrans(int count)
    {
        numOfUser.text = count.ToString();
    }

    public void CreateBookInstance(int price, string genre, string publisher, int year, int authorID, string AuthorName)
    {
        bookInstance = new Book(bookID, bookName.text, price, genre, publisher, year, authorID, AuthorName);
    }

    public void OnCLickDetail()
    {
        OnClickDetail?.Invoke(bookInstance, Int32.Parse(numOfUser.text));
    }
    public void OnCLickBuyItem()
    {
        OnClickBuy?.Invoke(bookID);
    }
}
