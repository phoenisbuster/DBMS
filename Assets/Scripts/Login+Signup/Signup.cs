using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using System.Threading;

public class Signup : MonoBehaviour
{
    private string dbName = "URI=file:Assets/SQLDatabase/Database.db";

    public TMP_InputField nameField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public GameObject RegBtn;
    public GameObject CancleBtn;
    
    public static Action onClickBack;
    public static Action<string> announce;

    private void Awake() 
    {
        dbName = Database.dbName;
    }

    public void SetDBName()
    {
        dbName = Database.dbName;
    }

    public void OnClickSignUp()
    {
        var isSignUp = true;
        var lastId = 0;

        if(passwordField.text == "")
        {
            announce?.Invoke("Please enter password");
            isSignUp = false;
        }

        using (var connection = new SqliteConnection(Database.dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "BEGIN;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT * FROM Customers";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        try
                        {
                            if(usernameField.text == reader["username"].ToString() || usernameField.text == "admin")
                            {    
                                isSignUp = false;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                        lastId++;
                    }
                }
                SignUp(connection , lastId, nameField.text, usernameField.text, passwordField.text);
            }
            if(isSignUp)
            {
                Database.PerformTransaction(Transaction.TransactionTypes.COMMIT, connection);
                Debug.Log("Sign-up succesfully");
                announce?.Invoke("Sign-up succesfully");
            }
            else
            {
                Database.PerformTransaction(Transaction.TransactionTypes.ROLLBACK, connection);
                Debug.LogWarning("Username is already taken");
                announce?.Invoke("Username is already taken");
            }  
            Database.DisplayWithConnection(connection, Database.TableName.Customers);
            connection.CloseAsync();
        }
        Database.Display();
    }

    public void OnClickCancle()
    {        
        onClickBack?.Invoke();
        nameField.text = "";
        usernameField.text = "";
        passwordField.text = "";
    }

    private void SignUp(SqliteConnection connection, int id, string name, string username, string password)
    {
        // using (var connection = new SqliteConnection(dbName))
        // {
        //     connection.OpenAsync(CancellationToken.None);

        //     using (var command = connection.CreateCommand())
        //     {
        //         command.CommandText = "INSERT INTO Customers VALUES('" + id + "','" + name + "','" + username + "','" + password + "');";
        //         command.ExecuteNonQuery();
        //         Debug.Log("Add new user success");
        //     }
        //     connection.CloseAsync();
        // }

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO Customers VALUES('" + id + "','" + name + "','" + username + "','" + password + "');";
            command.ExecuteNonQuery();
        }
        Database.DisplayWithConnection(connection, Database.TableName.Customers);
        OnClickCancle();
    }
}
