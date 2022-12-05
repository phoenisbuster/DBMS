using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class PopupController : MonoBehaviour
{
    public Database DB;

    public TMP_Dropdown dropdown;

    public TMP_InputField inputDeleteID;

    public PopulateGrid populateGridScript;

    public GameObject[] arrayObjectForm;

    public TMP_InputField[] arrayInputField;
    // Start is called before the first frame update
    void Start()
    {
        dropdown.value = 0;
        foreach (var form in arrayObjectForm)
        {
            form.SetActive(false);
        }
        arrayObjectForm[0].SetActive(true);
        int i = 0;
        foreach (var input in arrayInputField)
        {
            if (i <= 1)
                input.gameObject.SetActive(true);
            else
                input.gameObject.SetActive(false);

            i++;
        }
        this.gameObject.SetActive(false);
    }

    public void TogglePopup()
    {
        if(this.gameObject.activeInHierarchy)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    public void ToggleForm()
    {
        foreach (var form in arrayObjectForm)
        {
            form.SetActive(false);
        }

        int i = 0;

        switch (dropdown.value) 
        {
            case 0:
                arrayObjectForm[0].SetActive(true);
                foreach (var input in arrayInputField)
                {
                    if(i <= 1)
                        input.gameObject.SetActive(true);
                    else
                        input.gameObject.SetActive(false);

                    i++;
                }
                break;
            case 1:
                arrayObjectForm[1].SetActive(true);
                foreach (var input in arrayInputField)
                {
                    if (i <= 3)
                        input.gameObject.SetActive(true);
                    else
                        input.gameObject.SetActive(false);

                    i++;
                }
                break;
            case 2:
                arrayObjectForm[2].SetActive(true);
                foreach (var input in arrayInputField)
                {
                    input.gameObject.SetActive(true);
                }
                break;
            case 3:
                arrayObjectForm[3].SetActive(true);
                foreach (var input in arrayInputField)
                {
                    if (i <= 2)
                        input.gameObject.SetActive(true);
                    else
                        input.gameObject.SetActive(false);

                    i++;
                }
                break;
        }

    }

    public void PopulateForm()
    {
        switch (dropdown.value)
        {
            case 0:
                DB.authorID = Int32.Parse(arrayInputField[0].text);
                DB.authorName = arrayInputField[1].text;
                DB.InsertDB(Database.TableName.Authors);
                break;
            case 1:
                DB.customerID = Int32.Parse(arrayInputField[0].text);
                DB.customerName = arrayInputField[1].text;
                DB.username = arrayInputField[2].text;
                DB.password = arrayInputField[3].text;
                DB.InsertDB(Database.TableName.Customers);
                break;
            case 2:
                DB.ISBN = Int32.Parse(arrayInputField[0].text);
                DB.title = arrayInputField[1].text;
                DB.genre = arrayInputField[2].text;
                DB.publisher = arrayInputField[3].text;
                DB.prices = Int32.Parse(arrayInputField[4].text);
                DB.year = Int32.Parse(arrayInputField[5].text);
                DB.author_ID = Int32.Parse(arrayInputField[6].text);
                DB.InsertDB(Database.TableName.Books);
                break;
            case 3:
                DB.ID = Int32.Parse(arrayInputField[0].text);
                DB.customer_ID = Int32.Parse(arrayInputField[1].text);
                DB.book_ISBN = Int32.Parse(arrayInputField[2].text);
                DB.InsertDB(Database.TableName.Transactions);
                break;
        }

        TogglePopup();

        populateGridScript.Refresh();
    }

    public void OnDeleteClick()
    {
        switch (dropdown.value)
        {
            case 0:
                DB.DeleteRow(Database.TableName.Authors, Int32.Parse(inputDeleteID.text));
                break;
            case 1:
                DB.DeleteRow(Database.TableName.Customers, Int32.Parse(inputDeleteID.text));
                break;
            case 2:
                DB.DeleteRow(Database.TableName.Books, Int32.Parse(inputDeleteID.text));
                break;
            case 3:
                DB.DeleteRow(Database.TableName.Transactions, Int32.Parse(inputDeleteID.text));
                break;
        }

        TogglePopup();

        populateGridScript.Refresh();
    }
}
