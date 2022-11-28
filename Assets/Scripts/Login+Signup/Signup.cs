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
    private string dbName = "URI=file:Database.db";

    public TMP_InputField nameField;
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public GameObject RegBtn;
    public GameObject CancleBtn;
    
    public static Action onClickBack;
    public static Action<string> announce;

    public void OnClickSignUp()
    {
        if(passwordField.text == "")
        {
            announce?.Invoke("Please enter password");
            return;
        }
        
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Customers";
                using (IDataReader reader = command.ExecuteReader())
                {
                    var isSignUp = true;
                    var lastId = 0;
                    while(reader.Read())
                    {
                        try
                        {
                            if(usernameField.text == reader["username"].ToString() || usernameField.text == "admin")
                            {    
                                isSignUp = false;    
                                break;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                        lastId++;
                    }

                    if(isSignUp)
                    {
                        Debug.Log("Sign-up succesfully");
                        announce?.Invoke("Sign-up succesfully");
                        SignUp(lastId, nameField.text, usernameField.text, passwordField.text);
                    }
                    else
                    {
                        Debug.LogWarning("Username is already taken");
                        announce?.Invoke("Username is already taken");
                    }
                }

            }
            connection.CloseAsync();
        }
    }

    public void OnClickCancle()
    {
        onClickBack?.Invoke();
        nameField.text = "";
        usernameField.text = "";
        passwordField.text = "";
    }

    private void SignUp(int id, string name, string username, string password)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Customers VALUES('" + id + "','" + name + "','" + username + "','" + password + "');";
                command.ExecuteNonQuery();
                Debug.Log("Add new user success");
            }
            connection.CloseAsync();
        }
        OnClickCancle();
        Database.Display();
    }
}
