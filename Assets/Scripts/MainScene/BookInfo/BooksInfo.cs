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

public class BooksInfo : MonoBehaviour
{
    public GameObject BookItemPrefab;
    public ScrollRect ScrollObj;
    public GameObject ContentView;
    public GameObject DetailedView;
    private string dbName = "URI=file:Assets/SQLDatabase/Database.db";
    private bool onRefresh = false;
    Dictionary<string, GameObject> books = new Dictionary<string, GameObject>(); 
    
    void Awake()
    {
        dbName = Database.dbName;
        FetchBooksData();    
    }

    private void FetchBooksData()
    {
        foreach(Transform child in ContentView.transform)
        {
            Destroy(child.gameObject);
        }

        books.Clear();
        
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT p.*, a.Name FROM Books AS p JOIN Authors AS a ON p.AuthorID = a.ID";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var isbn = reader["ISBN"].ToString();
                        var title = reader["Title"].ToString();
                        var price = reader["Prices"].ToString();
                        var genre = reader["Genre"].ToString();
                        var publisher = reader["Publisher"].ToString();
                        var year = reader["Year"].ToString();
                        var authorID = reader["AuthorID"].ToString();
                        var authorName = reader["Name"].ToString();

                        var bookInstance = GameObject.Instantiate(BookItemPrefab, Vector3.zero, Quaternion.identity);
                        bookInstance.transform.SetParent(ContentView.transform);
                        bookInstance.GetComponent<BookComponent>().DisplayData(isbn, title, price, genre, publisher, year, authorID, authorName);
                        books.Add(isbn, bookInstance);
                    }
                }
            }
            connection.CloseAsync();
        }
        Debug.Log("Fetch book data complete");
        FetchTransaction();
    }

    private void FetchTransaction()
    {
        int count = 0;
        foreach (var item in books)
        {
            count = 0;
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
                        item.Value.GetComponent<BookComponent>().DisplayTrans(count);
                    }
                }
                connection.CloseAsync();
            }
        }
        onRefresh = false;
        Debug.Log("Fetch transaction for book data complete");
    }

    public void OnCLickRefreshBookData()
    {
        FetchBooksData();
    }

    void Update()
    {
        if(ScrollObj.verticalNormalizedPosition >= 1.1f && !onRefresh)
        {
            Debug.Log("Reload");
            onRefresh = true;
            FetchBooksData();
        }
    }

}
