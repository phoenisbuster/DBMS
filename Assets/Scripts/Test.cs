using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Threading;

public class Test : MonoBehaviour
{
    public string dbUrl = "abc";

    // Start is called before the first frame update
    void Start()
    {
        CreateDB();
    }

    public void CreateDB()
    {
        using(var connect = new SqliteConnection(dbUrl))
        {
            connect.OpenAsync(CancellationToken.None);

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
