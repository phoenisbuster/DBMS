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
    private string dbName = "URI=file:Database.db";
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
        userID = PlayerPrefs.GetInt(AccountManager.KEY_USER_ID, -1);
        if(userID >= 0)
        {
            Debug.Log("Fetch user data success");
            FetchUserData();
        }
        else
        {
            Debug.LogError("Fetch user data fail");
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
    }

    private void ChangUserData(int target, string value) 
    {
        if(value == "")
        {
            DisplayAnnounce("Incorrect input format");
            return;
        }        
        var fieldName = target == ((int)TargetChange.Name)? "Name" : "password";
        var NewName = target == ((int)TargetChange.Name)? value : "";
        var NewPass = target == ((int)TargetChange.Password)? value : "";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Customers SET " + fieldName + " = '" + value + "' WHERE ID = " + userID + ";";
                command.ExecuteNonQuery();
                DisplayAnnounce("Update " + fieldName + " success");
            }
            connection.CloseAsync();
        }
        SaveToLocalData(NewName, "", NewPass, false);
        Database.Display();
    }

    private void SaveToLocalData(string name = "", string username = "", string passowrd = "", bool isAnnounce = true)
    {
        if(name != "")
        {
            PlayerPrefs.SetString(KEY_NAME, name);
            SetNameDisplay();
        }           

        if(username != "")
        {
            PlayerPrefs.SetString(KEY_USERNAME, username);
            SetUserNameDisplay();
        }
            
        if(passowrd != "")
        {
            PlayerPrefs.SetString(KEY_PASSWORD, passowrd);
            SetPasswordDisplay();
        }   
        Debug.Log("Fetch data " + "ID: " + userID + " name: " + name + " username: " + username + " password " + passowrd);
        if(isAnnounce)
            DisplayAnnounce("Fetch/Update Data success");
    }

    private void SetNameDisplay()
    {
        NameDisplay.text = "Your Name: " + PlayerPrefs.GetString(KEY_NAME, "You have't set a name yet");
    }

    private void SetUserNameDisplay()
    {
        UsernameDisplay.text = "User Name: " + PlayerPrefs.GetString(KEY_USERNAME, "");
    }

    private void SetPasswordDisplay()
    {
        if(ToogleHidePassword.isOn)
        {
            PasswordDisplay.text = "Your Password: ******";
        }
        else
        {
            PasswordDisplay.text = "Your Password: " + PlayerPrefs.GetString(KEY_PASSWORD, "000000");
        }
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
        PlayerPrefs.DeleteKey(KEY_NAME);
        PlayerPrefs.DeleteKey(KEY_USERNAME);
        PlayerPrefs.DeleteKey(KEY_PASSWORD);
        PlayerPrefs.SetInt(AccountManager.KEY_USER_ID, -1);
        OnClickLogout?.Invoke();
    }

}
