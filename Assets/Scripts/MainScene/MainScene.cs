using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainScene : MonoBehaviour
{
    public GameObject UserInfoObj;
    public GameObject BooksInfoObj;
    public GameObject UserInfoBtn;
    
    private void OnEnable() 
    {
        UserInfo.OnClickCancel += OnClickHideUserInfo;
        UserInfo.OnClickLogout += LoadLogInSignUpScene;
    }

    private void OnDisable() 
    {
        UserInfo.OnClickCancel -= OnClickHideUserInfo;
        UserInfo.OnClickLogout -= LoadLogInSignUpScene;
    }

    public void OnClickUserInfo()
    {
        if(UserInfoObj.activeSelf)
        {
            OnClickHideUserInfo();
        }
        else
        {
            OnClickRevealUserInfo();
        }
    }

    private void OnClickHideUserInfo()
    {
        // UserInfoObj.transform.DOMove(UserInfoBtn.transform.position, 0.25f).SetEase(Ease.Linear).SetLink(UserInfoObj);
        UserInfoObj.transform.DOScale(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            UserInfoObj.SetActive(false);
        }).SetLink(UserInfoObj);
       
    }

    private void OnClickRevealUserInfo()
    {
        UserInfoObj.SetActive(true);
        // UserInfoObj.transform.DOMove(transform.position, 0.25f).SetEase(Ease.Linear).SetLink(UserInfoObj);
        UserInfoObj.transform.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            
        }).SetLink(UserInfoObj);   
    }

    private void LoadLogInSignUpScene()
    {
        SceneManager.LoadScene(1);
    }
}
