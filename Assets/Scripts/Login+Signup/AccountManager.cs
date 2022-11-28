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
    
    private void OnEnable() 
    {
        Login.announce += DisplayAnnounce;
        Login.onClickSignup += DisplaySignup;
        Signup.announce += DisplayAnnounce;
        Signup.onClickBack += DisplayLogin;
    }

    private void OnDisable() 
    {
        Login.announce -= DisplayAnnounce;
        Login.onClickSignup -= DisplaySignup;
        Signup.announce -= DisplayAnnounce;
        Signup.onClickBack -= DisplayLogin;
    }

    public void DisplayAnnounce(string text)
    {
        Announce.text = text;
        LoadScene(text);
        StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        yield return new WaitForSeconds(TimeDisplay);
        Announce.text = "";
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

    private void LoadScene(string announce)
    {
        if(announce == UserLogSuccess)
        {
            SceneManager.LoadScene(0);
        }
        if(announce == AdminLogSuccess)
        {
            SceneManager.LoadScene(2);
        }
    }
}
