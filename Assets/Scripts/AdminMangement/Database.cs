using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Threading;

public class Database : MonoBehaviour
{
    public static string dbName = "URI=file:Assets/SQLDatabase/Database.db";
    public static string DateFormat = "yyyy-MM-dd HH:mm:ss";

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
    public string username;
    public string password;


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
        //DeleteRow(TableName.Transactions, 2);
    }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Authors (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100));" +

                    "CREATE TABLE IF NOT EXISTS Customers (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR(100), username VARCHAR(100) NOT NULL UNIQUE, passowrd VARCHAR(100) NOT NULL);" +

                    "CREATE TABLE IF NOT EXISTS Transactions (ID INTEGER PRIMARY KEY AUTOINCREMENT, CustomerID INT, BookISBN INT, CreatedAt DATETIME," +
                    "FOREIGN KEY (CustomerID) REFERENCES Customers(ID), FOREIGN KEY (BookISBN) REFERENCES Books(ISBN) );" +

                    "CREATE TABLE IF NOT EXISTS Books (ISBN INTEGER PRIMARY KEY AUTOINCREMENT, Title VARCHAR(100), Genre VARCHAR(50), Publisher VARCHAR(100), Prices INT, Year INT, AuthorID INT, FOREIGN KEY (AuthorID) REFERENCES Authors(ID))";
                command.ExecuteNonQuery();
            }

            connection.CloseAsync();
        }
    }

    public void InsertDB(TableName table)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                Debug.Log("Check table type " + tableName);
                switch (table)
                {
                    case TableName.Authors:
                        command.CommandText = "INSERT INTO Authors VALUES('"+ authorID + "','" +  authorName + "');";
                        break;
                    case TableName.Books:
                        command.CommandText = "INSERT INTO Books VALUES('" + ISBN + "','" + title + "','" + genre + "','" + publisher + "','" + prices + "','" + year + "','" + author_ID + "'); ";
                        break;
                    case TableName.Customers:
                        command.CommandText = "INSERT INTO Customers VALUES('" + customerID + "','" + customerName + "','" + username + "','" + password + "');";
                        break;
                    case TableName.Transactions:
                        command.CommandText = "INSERT INTO Transactions VALUES('" + ID + "','" + customerID + "','" + book_ISBN + "','" + DateTime.Now.ToString(DateFormat) + "');";
                        break;
                    case TableName.All:
                        command.CommandText = "INSERT INTO Customers VALUES('" + customerID + "','" + customerName + "');" +
                            "INSERT INTO Books VALUES('" + ISBN + "','" + title + "','" + genre + "','" + publisher + "','" + prices + "','" + year + "','" + author_ID + "'); " +
                            "INSERT INTO Authors VALUES('" + authorID + "','" + authorName + "');" +
                            "INSERT INTO Transactions VALUES('" + ID + "','" + customerID + "','" + book_ISBN + "','" + DateTime.Now.ToString(DateFormat) + "');";
                        break;
                }
                command.ExecuteNonQuery();

                Debug.LogError("Success");
            }

            connection.CloseAsync();
        }
    }

    public static void Display()
    {
        Debug.Log("Print With NEW CONNECTION");

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
                        Debug.Log("ID: " + reader["ID"] + " Name " + reader["Name"] + " username " + reader["username"] + " password " + reader["password"]);
                    }
                }

                command.CommandText = "SELECT * FROM Books";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" ISBN: " + reader["ISBN"] + " Title " + reader["Title"] + " Genre: " + reader["Genre"] + " Publisher " + reader["Publisher"] + " Prices: " + reader["Prices"] + " Year: " + reader["Year"] + "Author ID: " + reader["AuthorID"]);
                    }
                }

                command.CommandText = "SELECT * FROM Transactions";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Transaction number: " + reader["ID"] + " customerID: " + reader["CustomerID"] + " BookISBN " + reader["BookISBN"] + " Date: " + reader["CreatedAt"].ToString());
                    }
                }
            }
            connection.CloseAsync();
        }
    }

    public static void DisplayWithConnection(SqliteConnection connection, TableName table)
    {
        Debug.Log("Print With Connection to " + table);
        using (var command = connection.CreateCommand())
        {
            switch(table)
            {    
                case TableName.Authors:
                    command.CommandText = "SELECT * FROM Authors";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log(" ID: " + reader["ID"] + " Name " + reader["Name"]);
                        }
                    }
                    break;
                case TableName.Customers:
                    command.CommandText = "SELECT * FROM Customers";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log("ID: " + reader["ID"] + " Name " + reader["Name"] + " username " + reader["username"] + " password " + reader["password"]);
                        }
                    }
                    break;
                case TableName.Books:
                    command.CommandText = "SELECT * FROM Books";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log(" ISBN: " + reader["ISBN"] + " Title " + reader["Title"] + " Genre: " + reader["Genre"] + " Publisher " + reader["Publisher"] + " Prices: " + reader["Prices"] + " Year: " + reader["Year"] + "Author ID: " + reader["AuthorID"]);
                        }
                    }
                    break;
                case TableName.Transactions:
                    command.CommandText = "SELECT * FROM Transactions";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.Log("Transaction number: " + reader["ID"] + " customerID: " + reader["CustomerID"] + " BookISBN " + reader["BookISBN"] + " Date: " + reader["CreatedAt"].ToString());
                        }
                    }
                    break;    
            }
        }
    }

    public static List<string> ReturnDB()
    {
        List<string> lst = new List<string>();

        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                lst.Add("AUTHORS:");
                command.CommandText = "SELECT * FROM Authors";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" ID: " + reader["ID"] + " Name " + reader["Name"]);
                        lst.Add(" ID: " + reader["ID"] + ", Name: " + reader["Name"]);
                    }
                }

                lst.Add("CUSTOMERS:");
                command.CommandText = "SELECT * FROM Customers";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("ID: " + reader["ID"] + " Name " + reader["Name"] + " username " + reader["username"] + " password " + reader["password"]);
                        lst.Add("ID: " + reader["ID"] + ", Name: " + reader["Name"] + ", username: " + reader["username"] + ", password: " + reader["password"]);
                    }
                }

                lst.Add("BOOKS:");
                command.CommandText = "SELECT * FROM Books";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" ISBN: " + reader["ISBN"] + " Title " + reader["Title"] + " Genre: " + reader["Genre"] + " Publisher " + reader["Publisher"] + " Prices: " + reader["Prices"] + " Year: " + reader["Year"] + "Author ID: " + reader["AuthorID"]);
                        lst.Add(" ISBN: " + reader["ISBN"] + ", Title: " + reader["Title"] + ", Genre: " + reader["Genre"] + ", Publisher: " + reader["Publisher"] + ", Prices: " + reader["Prices"] + ", Year: " + reader["Year"] + ", Author ID: " + reader["AuthorID"]);
                    }
                }

                lst.Add("TRANSACTIONS:");
                command.CommandText = "SELECT * FROM Transactions";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log(" customerID: " + reader["CustomerID"] + " BookISBN " + reader["BookISBN"]);
                        lst.Add(" customerID: " + reader["CustomerID"] + ", BookISBN: " + reader["BookISBN"]);
                    }
                }
            }
            connection.CloseAsync();
        }

        return lst;
    }

    public static void DeleteRow(TableName table, int id)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                Debug.Log("Delete from table name " + table + " at id = " + id);
                switch(table)
                {
                    case TableName.Authors:
                        command.CommandText = "DELETE FROM Authors WHERE ID = " + id + ";";
                        break;
                    case TableName.Customers:
                        command.CommandText = "DELETE FROM Customers WHERE ID = " + id + ";";
                        break;
                    case TableName.Books:
                        command.CommandText = "DELETE FROM Books WHERE ISBN = " + id + ";";
                        break;
                    case TableName.Transactions:
                        command.CommandText = "DELETE FROM Transactions WHERE ID = " + id + ";";
                        break;
                }
                command.ExecuteNonQuery();
            }
            connection.CloseAsync();
        }
        Display();
    }

    public static void PerformTransaction(Transaction.TransactionTypes TranType, SqliteConnection SQLconnection = null)
    {
        string CommandText = "BEGIN;";
        switch(TranType)
        {
            case Transaction.TransactionTypes.BEGIN:
                CommandText = "BEGIN;";
                break;
            case Transaction.TransactionTypes.COMMIT:
                CommandText = "COMMIT;";
                break;
            case Transaction.TransactionTypes.ROLLBACK:
                CommandText = "ROLLBACK;";
                break;
            default:
                CommandText = "ROLLBACK;";
                break;
        }

        if(SQLconnection == null)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.OpenAsync(CancellationToken.None);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = CommandText;
                    command.ExecuteNonQuery();
                }
                connection.CloseAsync();
            }
        }
        else
        {
            using (var command = SQLconnection.CreateCommand())
            {
                command.CommandText = CommandText;
                command.ExecuteNonQuery();
            }
        }
    }
}