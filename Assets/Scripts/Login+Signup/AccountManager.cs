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
        Announce.text = text;
        StartCoroutine(Display());
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

    private void LoadScene(bool isUser = true, int idUser = -1)
    {
        if(isUser)
        {
            if(idUser >= 0)
            {
                PlayerPrefs.SetInt(KEY_USER_ID, idUser);
                SceneManager.LoadScene(0);
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
}
