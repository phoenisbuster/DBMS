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
    private string dbName = "URI=file:Assets/SQLDatabase/Database.db";

    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public GameObject LoginBtn;
    public GameObject SignupBtn;
    
    public static Action onClickSignup;
    public static Action<string> announce;
    public static Action<bool, int> LoginSuccess;

    private void Awake() 
    {
        dbName = Database.dbName;
    }

    public void SetDBName()
    {
        dbName = Database.dbName;
    }
    
    public void OnClickLogin()
    {
        if(usernameField.text == "admin")
        {
            LoginAdmin();
            return;
        }

        try
        {
            Debug.LogWarning("Check Database path before login: " + Database.dbName);
            using (var connection = new SqliteConnection(Database.dbName))
            {
                connection.OpenAsync(CancellationToken.None);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Customers";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        var isLogin = false;
                        var id = -1;
                        while(reader.Read())
                        {
                            try
                            {
                                if(usernameField.text == reader["username"].ToString() && passwordField.text == reader["password"].ToString())
                                {    
                                    isLogin = true;
                                    try
                                    {
                                        id = Int32.Parse(reader["ID"].ToString());
                                    }   
                                    catch(FormatException)
                                    {
                                        Debug.LogError($"Unable to parse '{reader["ID"].ToString()}'");
                                    }
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
                            LoginSuccess?.Invoke(true, id);
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
        catch(Exception e)
        {
            Debug.LogWarning("Database not found " + dbName + " With message " + e.Message);
            announce?.Invoke("Database not found");
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
            LoginSuccess?.Invoke(false, -1);
        }
        else
        {
            Debug.LogWarning("Username or Password are not correct");
            announce?.Invoke("Username or Password are not correct");
        }
    }
}
