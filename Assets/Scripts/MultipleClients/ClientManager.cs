using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public static int noOfID = -1;
    public Dictionary<int, int> UserList = new Dictionary<int, int>();
    public int curOder = -1;

    private void Awake() 
    {
        UserList = new Dictionary<int, int>();
    }

    private void Update() 
    {
        curOder = noOfID;

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.LogWarning("Print LIST");
            foreach (var item in UserList)
            {
                Debug.LogWarning(item.Key + " " + item.Value);
            }
            Debug.LogWarning("End Print LIST");
        }
    }
}
