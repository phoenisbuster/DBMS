using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Threading;

public class Test : MonoBehaviour
{
    public string dbName = "URI=file:test.db";

    // Start is called before the first frame update
    void Start()
    {
        CreateDB();
        Display();
    }

    public void CreateDB()
    {
        using(var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS weapons (name VARCHAR(20), damage INT);" +
                    "CREATE TABLE IF NOT EXISTS sowrds (name VARCHAR(20), dmg INT);";
                command.ExecuteNonQuery();
            }

            connection.CloseAsync();
        }
    }

    public void Display()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM weapons";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Debug.Log("Name:" + reader["name"] + "Damage" + reader["damage"]);
                    }
                }
            }

            connection.CloseAsync();
        }
    }
}
