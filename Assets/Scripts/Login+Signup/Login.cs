using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using TMPro;
using System.Threading;

public class Login : MonoBehaviour
{
    private string dbName = "URI=file:Database.db";

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public GameObject LoginBtn;
    public GameObject SignupBtn;
    
    public static Action onClickSignup;
    public static Action<string> announce;

    public void OnClickLogin()
    {
        if(usernameField.text == "admin")
        {
            LoginAdmin();
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
                    var isLogin = false;
                    while(reader.Read())
                    {
                        try
                        {
                            if(usernameField.text == reader["username"].ToString() && passwordField.text == reader["password"].ToString())
                            {    
                                isLogin = true;    
                                break;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                    }

                    if(isLogin)
                    {
                        Debug.Log("Log-in succesfully");
                        announce?.Invoke(AccountManager.UserLogSuccess);
                    }
                    else
                    {
                        Debug.LogWarning("Username or Password are not correct");
                        announce?.Invoke("Username or Password are not correct");
                    }
                }

            }
            connection.CloseAsync();
        }
    }

    public void OnClickSignUp()
    {
        onClickSignup?.Invoke();
        usernameField.text = "";
        passwordField.text = "";
    }

    private void LoginAdmin()
    {
        if(passwordField.text == "admin")
        {
            Debug.Log("Admin log-in succesfully");
            announce?.Invoke(AccountManager.AdminLogSuccess);
        }
        else
        {
            Debug.LogWarning("Username or Password are not correct");
            announce?.Invoke("Username or Password are not correct");
        }
    }
}
