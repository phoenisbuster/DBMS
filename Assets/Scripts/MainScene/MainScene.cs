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
    public GameObject LoginScene;
    public bool multipleUser = false;
    public int fixID = -1;
    public ClientManager clientManager;
    public int thisID = -1;
    private string dbName = "URI=file:Assets/SQLDatabase/Database.db";
    
    private void Awake() 
    {
        dbName = Database.dbName;       
        BooksInfoObj.GetComponent<BooksInfo>().SetMultiple(multipleUser);
        
    }

    private void Start() {
        thisID = UserInfoObj.GetComponent<UserInfo>().GetUserID();
    }

    private void OnEnable() 
    {
        UserInfo.OnClickCancel += OnClickUserInfo;
        UserInfo.OnClickLogout += LoadLogInSignUpScene;

        BookComponent.OnClickDetail += bookRevealClick;
        BookDetail.OnClickExit += bookHideClick;

        BookComponent.OnClickBuy += CreateTransaction;
        BookDetail.OnClickBuy += CreateTransaction;
    }

    private void OnDisable() 
    {
        UserInfo.OnClickCancel -= OnClickUserInfo;
        UserInfo.OnClickLogout -= LoadLogInSignUpScene;

        BookComponent.OnClickDetail -= bookRevealClick;
        BookDetail.OnClickExit -= bookHideClick;

        BookComponent.OnClickBuy -= CreateTransaction;
        BookDetail.OnClickBuy -= CreateTransaction;
    }

    public void OnClickUserInfo(int id)
    { 
        if(CheckAllowAction(id < 0? thisID : id))
        {    
            if(UserInfoObj.activeSelf)
            {
                userHideClick(thisID);
            }
            else
            {
                userRevealClick();
            } 
        }   
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

    private void userHideClick(int userID)
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
            UserInfoObj.GetComponent<UserInfo>().SetBooksHistory();
        }).SetLink(UserInfoObj);   
    }

    private void bookHideClick()
    {
        if(CheckAllowAction(thisID))
        {     
            BooksDetailObj.transform.DOScale(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
            {
                BooksDetailObj.SetActive(false);
            }).SetLink(UserInfoObj);
        }
    }

    private void bookRevealClick(Book book, int count)
    {
        if(CheckAllowAction(thisID))
        {    
            BooksDetailObj.SetActive(true);
            BooksDetailObj.GetComponent<BookDetail>().SetBookDetail(book, count);
            BooksDetailObj.transform.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.InOutElastic).OnComplete(()=>
            {
                
            }).SetLink(UserInfoObj);
        }
    }

    private void LoadLogInSignUpScene(int userID)
    {
        if(CheckAllowAction(userID))
        {
            if(!multipleUser)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                LoginScene.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    private void CreateTransaction(int bookID)
    {
        if(!CheckAllowAction(thisID))
        {
            return;
        }
        
        var userID = thisID;

        int curIDTras = 0;
        bool allowBuy = true;
        if(userID < 0)
        {
            DisplayAnnounce("User not found, please log-out and re log-in");
            allowBuy = false;
        }

        Debug.Log("Performming to buy " + bookID + " For User: " + userID);

        using (var connection = new SqliteConnection(Database.dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Transactions";         
                using (IDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        if(userID.ToString() == reader["CustomerID"].ToString())
                        {
                            if(bookID.ToString() == reader["BookISBN"].ToString())
                            {
                                Debug.LogWarning("You already bought this book");
                                DisplayAnnounce("You already bought this book");
                                allowBuy = false;
                            }
                        }
                        curIDTras++;
                    }
                }
            }
            connection.CloseAsync();
        }

        using (var connection = new SqliteConnection(Database.dbName))
        {
            connection.OpenAsync(CancellationToken.None);

            Database.PerformTransaction(Transaction.TransactionTypes.BEGIN, connection);

            using (var command = connection.CreateCommand())
            {
                Debug.Log("INSERT INTO Transactions VALUES('" + curIDTras + "','" + userID + "','" + bookID + "','" + DateTime.Now.ToString(Database.DateFormat) + "');");
                command.CommandText = "INSERT INTO Transactions VALUES('" + curIDTras + "','" + userID + "','" + bookID + "','" + DateTime.Now.ToString(Database.DateFormat) + "');";         
                command.ExecuteNonQuery();
                Database.DisplayWithConnection(connection, Database.TableName.Transactions);
            }

            if(allowBuy)
            {
                Database.PerformTransaction(Transaction.TransactionTypes.COMMIT, connection);
            }
            else
            {
                Database.PerformTransaction(Transaction.TransactionTypes.ROLLBACK, connection);
            }
            Database.DisplayWithConnection(connection, Database.TableName.Transactions);
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
