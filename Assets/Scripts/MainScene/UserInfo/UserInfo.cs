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

public class UserInfo : MonoBehaviour
{
    private string dbName = "URI=file:Assets/SQLDatabase/Database.db";
    public Image Avartar;
    public Button CancelBtn;
    public Button LogoutBtn;
    public TMP_Text NameDisplay;
    public TMP_Text UsernameDisplay;
    public TMP_Text PasswordDisplay;
    public Toggle ToogleHidePassword;
    public Button NameChangeBtn;
    public Button PasswordChangeBtn;
    public Button NameChangeCancelBtn;
    public Button PasswordChangeCancelBtn;
    public TMP_InputField NewNameInput;
    public TMP_InputField NewPasswordInput;
    public TMP_Text Announce;
    public TMP_Text BookIdsDisplay;

    public static Action OnClickCancel;
    public static Action OnClickLogout;

    private string KEY_NAME = "Name";
    private string KEY_USERNAME = "UserName";
    private string KEY_PASSWORD = "Password";
    private int userID = -1;
    private enum TargetChange
    {
        Name,
        Password
    }
    private bool isChangeName = false;
    private bool isChangePass = false;

    private void Awake() 
    {
        dbName = Database.dbName;
        userID = PlayerPrefs.GetInt(AccountManager.KEY_USER_ID + AccountManager.noOfID, -1);
        if(userID >= 0)
        {
            Debug.Log("Fetch user data success: " + AccountManager.KEY_USER_ID + AccountManager.noOfID);
            FetchUserData();
        }
        else
        {
            Debug.LogError("Fetch user data fail " + AccountManager.KEY_USER_ID + AccountManager.noOfID);
        }
    }

    private void FetchUserData() 
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Customers WHERE ID = " + userID + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var name = reader["Name"].ToString();
                        var username = reader["username"].ToString();
                        var passowrd = reader["password"].ToString();
                        SaveToLocalData(name, username, passowrd);
                    }
                }
            }
            connection.CloseAsync();
        }
        SetBooksHistory();
    }

    private void ChangUserData(int target, string value) 
    {
        var allowChanged = true;
        if(value == "")
        {
            DisplayAnnounce("Incorrect input format");
            allowChanged = false;
        }        
        var fieldName = target == ((int)TargetChange.Name)? "Name" : "password";
        var NewName = target == ((int)TargetChange.Name)? value : "";
        var NewPass = target == ((int)TargetChange.Password)? value : "";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            
            Database.PerformTransaction(Transaction.TransactionTypes.BEGIN, connection);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Customers SET " + fieldName + " = '" + value + "' WHERE ID = " + userID + ";";
                command.ExecuteNonQuery();
                Database.DisplayWithConnection(connection, Database.TableName.Customers);
            }

            if(allowChanged)
            {
                Database.PerformTransaction(Transaction.TransactionTypes.COMMIT, connection);
                DisplayAnnounce("Change " + fieldName + " success");
            }
            else
            {
                Database.PerformTransaction(Transaction.TransactionTypes.ROLLBACK, connection);
            }
            Database.DisplayWithConnection(connection, Database.TableName.Customers);
            connection.CloseAsync();
        }
        SaveToLocalData(NewName, "", NewPass, false);
        Database.Display();
    }

    private void SaveToLocalData(string name = "", string username = "", string passowrd = "", bool isAnnounce = true)
    {
        if(name != "")
        {
            PlayerPrefs.SetString(KEY_NAME+userID, name);
            SetNameDisplay();
        }           

        if(username != "")
        {
            PlayerPrefs.SetString(KEY_USERNAME+userID, username);
            SetUserNameDisplay();
        }
            
        if(passowrd != "")
        {
            PlayerPrefs.SetString(KEY_PASSWORD+userID, passowrd);
            SetPasswordDisplay();
        }   
        Debug.Log("Fetch data " + "ID: " + userID + " name: " + name + " username: " + username + " password " + passowrd);
        if(isAnnounce)
            DisplayAnnounce("Fetch/Update Data success");
    }

    private void SetNameDisplay()
    {
        NameDisplay.text = "Your Name: " + PlayerPrefs.GetString(KEY_NAME+userID, "You have't set a name yet");
    }

    private void SetUserNameDisplay()
    {
        UsernameDisplay.text = "User Name: " + PlayerPrefs.GetString(KEY_USERNAME+userID, "");
    }

    private void SetPasswordDisplay()
    {
        if(ToogleHidePassword.isOn)
        {
            PasswordDisplay.text = "Your Password: ******";
        }
        else
        {
            PasswordDisplay.text = "Your Password: " + PlayerPrefs.GetString(KEY_PASSWORD+userID, "000000");
        }
    }

    public void SetBooksHistory()
    {
        string bookids = "All bookIds you bought: ";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT bookisbn FROM Transactions WHERE customerid = " + userID + ";";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        bookids += reader["BookISBN"].ToString() + " ";
                    }
                }
            }
            connection.CloseAsync();
        }
        Debug.Log("Fetch Transaction for user data complete");
        BookIdsDisplay.text = bookids;
    }

    public void DisplayAnnounce(string text)
    {
        Announce.text = text;
        StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        yield return new WaitForSeconds(2);
        Announce.text = "Announce display here";
    }

    public void OnClickChangData(int target)
    {
        if(target == ((int)TargetChange.Name))
        {
            if(!isChangeName)
            {
                isChangeName = true;
                NameChangeBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Confirm";
                NameChangeCancelBtn.gameObject.SetActive(true);
                NewNameInput.gameObject.SetActive(true);
            }
            else
            {
                ChangUserData(target, NewNameInput.text);
                OnClickCancelChangeData(target);
            }
        }
        else
        {
            if(!isChangePass)
            {
                isChangePass = true;
                PasswordChangeBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Confirm";
                PasswordChangeCancelBtn.gameObject.SetActive(true);
                NewPasswordInput.gameObject.SetActive(true);
            }
            else
            {
                ChangUserData(target, NewPasswordInput.text);    
                OnClickCancelChangeData(target);
            }
        }
    }

    public void OnClickCancelChangeData(int target)
    {
        if(target == ((int)TargetChange.Name))
        {    
            isChangeName = false;
            NameChangeBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Change Name";
            NewNameInput.text = "";
            NameChangeCancelBtn.gameObject.SetActive(false);
            NewNameInput.gameObject.SetActive(false);
        }
        else
        {
            isChangePass = false;
            PasswordChangeBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Change Password";
            NewPasswordInput.text = "";
            PasswordChangeCancelBtn.gameObject.SetActive(false);
            NewPasswordInput.gameObject.SetActive(false);
        }
    }
    
    public int GetUserID()
    {
        return userID;
    }

    public void OnToogleHidePassword()
    {
        SetPasswordDisplay();
    }

    public void OnClickBtnX()
    {
        OnClickCancel?.Invoke();
    }

    public void OnClickLogoutBtn()
    {
        PlayerPrefs.DeleteKey(KEY_NAME + userID);
        PlayerPrefs.DeleteKey(KEY_USERNAME + userID);
        PlayerPrefs.DeleteKey(KEY_PASSWORD + userID);
        PlayerPrefs.DeleteKey(AccountManager.KEY_USER_ID + AccountManager.noOfID);
        AccountManager.noOfID--;
        OnClickLogout?.Invoke();
    }

}
