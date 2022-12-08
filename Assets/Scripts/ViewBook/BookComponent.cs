using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookComponent : MonoBehaviour
{
    public TextMeshProUGUI bookName;
    public TextMeshProUGUI authorName;
    public TextMeshProUGUI bookPrice;
    public TextMeshProUGUI numOfUser;
    // Start is called before the first frame update
    void Start()
    {
        ChangeText();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void ChangeText()
    {
        bookName.text = "BookShop";
        authorName.text = "Mika";
        bookPrice.text = "300$";
        numOfUser.text = "100";
    }
}
