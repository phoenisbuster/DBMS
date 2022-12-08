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

public class BookInfomation : MonoBehaviour
{
    public GameObject item;
    private string dbName = "URI=file:Database.db";
    Dictionary<string, GameObject> books = new Dictionary<string, GameObject>(); 
    
    void Awake()
    {
        var title = "";
        var price = "";
        var isbn = "";
        var count = 0;        

        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Books";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        title = reader["Title"].ToString();
                        price = reader["Prices"].ToString();
                        isbn = reader["ISBN"].ToString();

                        var bookInstance = GameObject.Instantiate(item, Vector3.zero, Quaternion.identity);
                        bookInstance.transform.SetParent(gameObject.transform.GetChild(0).GetChild(0));
                        bookInstance.GetComponent<BookComponent>().bookName.text = title;
                        bookInstance.GetComponent<BookComponent>().bookPrice.text = price;
                        bookInstance.GetComponent<BookComponent>().bookID = Int32.Parse(isbn);
                        books.Add(isbn, bookInstance);
                    }
                }
            }
            connection.CloseAsync();
        }

        foreach (var item in books)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.OpenAsync(CancellationToken.None);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Transactions WHERE BookISBN = " + item.Key + ";";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            count++;    
                        }
                        item.Value.GetComponent<BookComponent>().numOfUser.text = count.ToString();
                    }
                }
                connection.CloseAsync();
            }
        }
    }

    public void Count(int isbn)
    {

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
