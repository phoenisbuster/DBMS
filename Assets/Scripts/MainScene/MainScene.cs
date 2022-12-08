using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using DG.Tweening;
using TMPro;

public class MainScene : MonoBehaviour
{
    public GameObject UserInfoObj;
    public GameObject BooksInfoObj;
    public GameObject BooksDetailObj;
    public GameObject UserInfoBtn;
    public TMP_Text Announce;
    private string dbName = "URI=file:Database.db";
    
    private void OnEnable() 
    {
        UserInfo.OnClickCancel += userHideClick;
        UserInfo.OnClickLogout += LoadLogInSignUpScene;

        BookComponent.OnClickDetail += bookRevealClick;
        BookDetail.OnClickExit += bookHideClick;

        BookComponent.OnClickBuy += CreateTransaction;
        BookDetail.OnClickBuy += CreateTransaction;
    }

    private void OnDisable() 
    {
        UserInfo.OnClickCancel -= userHideClick;
        UserInfo.OnClickLogout -= LoadLogInSignUpScene;

        BookComponent.OnClickDetail -= bookRevealClick;
        BookDetail.OnClickExit -= bookHideClick;

        BookComponent.OnClickBuy -= CreateTransaction;
        BookDetail.OnClickBuy -= CreateTransaction;
    }

    public void OnClickUserInfo()
    {
        if(UserInfoObj.activeSelf)
        {
            userHideClick();
        }
        else
        {
            userRevealClick();
        }
    }

    private void userHideClick()
    {
        UserInfoObj.transform.DOScale(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            UserInfoObj.SetActive(false);
        }).SetLink(UserInfoObj);
       
    }

    private void userRevealClick()
    {
        UserInfoObj.SetActive(true);
        UserInfoObj.transform.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            
        }).SetLink(UserInfoObj);   
    }

    private void bookHideClick()
    {
        BooksDetailObj.transform.DOScale(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            BooksDetailObj.SetActive(false);
        }).SetLink(UserInfoObj);
       
    }

    private void bookRevealClick(Book book, int count)
    {
        BooksDetailObj.SetActive(true);
        BooksDetailObj.GetComponent<BookDetail>().SetBookDetail(book, count);
        BooksDetailObj.transform.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
        {
            
        }).SetLink(UserInfoObj);   
    }

    private void LoadLogInSignUpScene()
    {
        SceneManager.LoadScene(0);
    }

    private void CreateTransaction(int bookID)
    {
        int userID = UserInfoObj.GetComponent<UserInfo>().GetUserID();
        int curIDTras = 0;
        bool allowBuy = true;
        if(userID < 0)
        {
            DisplayAnnounce("User not found, please log-out and re log-in");
            return;
        }

        Debug.Log("Ready to buy " + bookID + " For User: " + userID);

        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Transactions";         
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        curIDTras++;
                        if(userID.ToString() == reader["CustomerID"].ToString())
                        {
                            if(bookID.ToString() == reader["BookISBN"].ToString())
                            {
                                Debug.LogWarning("You already bought this book");
                                DisplayAnnounce("You already bought this book");
                                curIDTras--;
                                allowBuy = false;
                            }
                        }
                    }
                }
            }
            connection.CloseAsync();
        }

        if(!allowBuy)
        {
            return;
        }

        using (var connection = new SqliteConnection(dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                Debug.Log("Success: INSERT INTO Transactions VALUES('" + curIDTras + "','" + userID + "','" + bookID + "','" + DateTime.Now + "');");
                command.CommandText = "INSERT INTO Transactions VALUES('" + curIDTras + "','" + userID + "','" + bookID + "','" + DateTime.Now + "');";         
                command.ExecuteNonQuery();
                DisplayAnnounce("Transaction success");
            }
            connection.CloseAsync();
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
}
