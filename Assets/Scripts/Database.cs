using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Threading;

public class Database : MonoBehaviour
{
    private string dbName = "URI=file:Database.db";

    public enum TableName
    {
        Authors,
        Customers,
        Books,
        Transactions,
        All
    }

    public TableName tableName;

    [Header("Author")]
    public int authorID;
    public string authorName;

    [Header("Customer")]
    public int customerID;
    public string customerName;

    [Header("Book")]
    public int ISBN;
    public string title;
    public string genre;
    public string publisher;
    public int prices;
    public int year;
    public int author_ID;

    [Header("Transaction")]
    public int ID;
    public int customer_ID;
    public int book_ISBN;



    // Start is called before the first frame update
    void Start()
    {
        CreateDB();
    }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Authors (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100));" +

                    "CREATE TABLE IF NOT EXISTS Customers (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100));" +

                    "CREATE TABLE IF NOT EXISTS Transactions (ID INTEGER PRIMARY KEY AUTOINCREMENT, CustomerID INT, BookISBN INT, CreatedAt DATETIME," +
                    "FOREIGN KEY (CustomerID) REFERENCES Customers(ID), FOREIGN KEY (BookISBN) REFERENCES Books(ISBN) );" +

                    "CREATE TABLE IF NOT EXISTS Books (ISBN INTEGER PRIMARY KEY AUTOINCREMENT, Title VARCHAR(100), Genre VARCHAR(50), Publisher VARCHAR(100), Prices INT, Year INT, AuthorID INT, FOREIGN KEY (AuthorID) REFERENCES Authors(ID))";
                command.ExecuteNonQuery();
            }

            connection.CloseAsync();
        }
    }

    public void InsertDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                switch (tableName)
                {
                    case TableName.Authors:
                        command.CommandText = "INSERT INTO Authors VALUES('"+ authorID + "','" +  authorName + "');";
                        break;
                    case TableName.Books:
                        command.CommandText = "INSERT INTO Books VALUES('" + ISBN + "','" + title + "','" + genre + "','" + publisher + "','" + prices + "','" + year + "','" + author_ID + "'); ";
                        break;
                    case TableName.Customers:
                        command.CommandText = "INSERT INTO Customers VALUES('" + customerID + "','" + customerName + "');";
                        break;
                    case TableName.Transactions:
                        command.CommandText = "INSERT INTO Transactions VALUES('" + ID + "','" + customer_ID + "','" + book_ISBN + "');";
                        break;
                    case TableName.All:
                        command.CommandText = "INSERT INTO Customers VALUES('" + customerID + "','" + customerName + "');" +
                            "INSERT INTO Books VALUES('" + ISBN + "','" + title + "','" + genre + "','" + publisher + "','" + prices + "','" + year + "','" + author_ID + "'); " +
                            "INSERT INTO Authors VALUES('" + authorID + "','" + authorName + "');" +
                            "INSERT INTO Transactions VALUES('" + ID + "','" + customerID + "','" + customerName + "');";
                        break;
                }
                command.ExecuteNonQuery();
                Debug.LogError("Success");
            }

            connection.CloseAsync();
        }

        Display();
    }

    public void Display()
    {
        Debug.Log("Print");

        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Authors";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" ID: " + reader["ID"] + " Name " + reader["Name"]);
                    }
                }

                command.CommandText = "SELECT * FROM Customers";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("ID: " + reader["ID"] + " Name " + reader["Name"]);
                    }
                }

                command.CommandText = "SELECT * FROM Books";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" ISBN: " + reader["ISBN"] + " Title " + reader["Title"] + " Genre: " + reader["Genre"] + " Publisher " + reader["Publisher"] + " Prices: " + reader["Prices"] + " Year: " + reader["Year"] + "Author ID: " + reader[""]);
                    }
                }

                command.CommandText = "SELECT * FROM Transactions";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" customerID: " + reader["Customer_ID"] + " BookISBN " + reader["BookISBN"]);
                    }
                }
            }

            connection.CloseAsync();
        }
    }
}