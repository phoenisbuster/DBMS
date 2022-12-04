using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopulateGrid : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Populate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Populate()
    {
        GameObject newObj;

        List<string> lstDb = Database.ReturnDB();


        foreach (var record in lstDb)
        {
            newObj = (GameObject)Instantiate(prefab, transform);
            newObj.GetComponent<TextMeshProUGUI>().text = record;
        }
    }
}
