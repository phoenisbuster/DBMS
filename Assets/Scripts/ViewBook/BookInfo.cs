using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Threading;
using TMPro;

public class BookInfo : MonoBehaviour
{
    public GameObject[] Books;
    // Start is called before the first frame update
    void Start()
    {
        Display();
    }

    // Update is called once per frame
    public static void Display()
    {
        Debug.Log("Print");

        using (var connection = new SqliteConnection("URI=file:Database.db"))
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
                        Debug.Log("Transaction number: " + reader["ID"] + " customerID: " + reader["Customer_ID"] + " BookISBN " + reader["BookISBN"]);
                    }
                }
            }
            connection.CloseAsync();
        }
    }
}
