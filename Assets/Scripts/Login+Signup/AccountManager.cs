using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject SignupPanel;

    public TMP_Text Announce;
    public float TimeDisplay = 2;

    public static string AdminLogSuccess = "Admin log-in succesfully";
    public static string UserLogSuccess = "Log-in succesfully";

    public static string KEY_USER_ID = "UserId";
    public static int noOfID = -1;
    public GameObject newScene;
    public GameObject AdminScene;
    public ClientManager clientManager;
    public bool multipleUser = false;
    public int fixID = 0;
    
    public void SetDBName()
    {
        LoginPanel.GetComponent<Login>().SetDBName();
        SignupPanel.GetComponent<Signup>().SetDBName();
    }

    private void OnEnable() 
    {
        Login.announce += DisplayAnnounce;
        Login.onClickSignup += DisplaySignup;
        Login.LoginSuccess += LoadScene;
        Signup.announce += DisplayAnnounce;
        Signup.onClickBack += DisplayLogin;
    }

    private void OnDisable() 
    {
        Login.announce -= DisplayAnnounce;
        Login.onClickSignup -= DisplaySignup;
        Login.LoginSuccess -= LoadScene;
        Signup.announce -= DisplayAnnounce;
        Signup.onClickBack -= DisplayLogin;
    }

    public void DisplayAnnounce(string text)
    {
        if(!multipleUser)
        {
            Announce.text = text;
            StartCoroutine(Display());
        }
        else
        {
            if(ClientManager.noOfID == fixID - 1)
            {
                Announce.text = text;
                StartCoroutine(Display());
            }
        }
    }

    IEnumerator Display()
    {
        yield return new WaitForSeconds(TimeDisplay);
        Announce.text = "Announce display here";
    }

    public void DisplaySignup()
    {
        LoginPanel.SetActive(false);
        SignupPanel.SetActive(true);
    }

    public void DisplayLogin()
    {
        LoginPanel.SetActive(true);
        SignupPanel.SetActive(false);
    }

    public bool CheckAllowAction(int userID)
    {
        if(!multipleUser)
        {
            return true;
        }
        else
        {
            if(clientManager.UserList.ContainsKey(fixID))
            {
                if(clientManager.UserList[fixID] == userID)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void LoadScene(bool isUser = true, int idUser = -1)
    {
        if(!multipleUser)
        {    
            if(isUser)
            {
                if(idUser >= 0)
                {
                    ClientManager.noOfID++;
                    PlayerPrefs.SetInt(KEY_USER_ID + ClientManager.noOfID, idUser);
                    SceneManager.LoadScene(1);
                }
                else
                {
                    Debug.LogError("User Not found");
                    DisplayAnnounce("User Not found");
                }
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            if(isUser && (ClientManager.noOfID == fixID-1))
            {
                ClientManager.noOfID++;
                PlayerPrefs.SetInt(KEY_USER_ID + ClientManager.noOfID, idUser);
            }

            Debug.LogWarning("CHECK cur user Order: " + ClientManager.noOfID + " Mathch userID: " + idUser);

            if(!isUser && fixID == -1)
            {
                AdminScene.SetActive(true);
                gameObject.SetActive(false);
            }
            else if(ClientManager.noOfID == fixID)
            {
                clientManager.UserList.Add(fixID, idUser);
                
                newScene.SetActive(true);
                gameObject.SetActive(false);
            }    
        }
    }

    private void Awake() 
    {
        //ClientManager.noOfID = -1;
        SetDBName();
    }
}
