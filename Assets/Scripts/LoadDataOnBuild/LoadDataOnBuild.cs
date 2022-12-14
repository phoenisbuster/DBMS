using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class LoadDataOnBuild : MonoBehaviour
{
    public AccountManager targetOnFinish;
    public bool forceNewData = false;
    private static bool finishLoad = false;

    private enum DeviceType
    {
        Android,
        IOS,
        Window,
        Mac,
        WebGL,
        Editor
    }

    private DeviceType deviceType;
    
    private async void Awake() 
    {
        if(finishLoad)
        {
            return;
        }
        
        if(targetOnFinish)
        {
            targetOnFinish.enabled = false;
        }

        deviceType = GetDeviceType();
        Debug.Log(deviceType);

        var dataPath = System.IO.Path.Combine(Application.persistentDataPath, "Database.db");
        Debug.Log("Database path " + dataPath);
        try
        {
            if(forceNewData)
            {
                throw new Exception("Force new database is ON");
            }

            var load = await LoadDatbaseFile(dataPath);
            Debug.Log("TRY CLAUSE: " + load);
        }
        catch(Exception e)
        {
            Debug.Log("CATCH_0: " + e.Message);
            var db = Resources.Load<TextAsset>("SQLDatabase/Database");
            Debug.Log("CATCH_1: " + db);
            byte[] data = db.bytes;

            Debug.Log("CATCH_2: " + data);
            System.IO.File.WriteAllBytes(dataPath, data);
            Debug.Log("CATCH_3: " + dataPath);    
        }
        finally
        {
            Database.dbName = "URI=file:" + dataPath;
            Debug.Log("FINAL: " + Database.dbName);
            finishLoad = true;
            targetOnFinish.enabled = true;
        }    
    }

    private Task<byte[]> LoadDatbaseFile(string dataPath)
    {
        return System.IO.File.ReadAllBytesAsync(dataPath, CancellationToken.None);
    }

    private DeviceType GetDeviceType()
    {
        if(SystemInfo.deviceType == UnityEngine.DeviceType.Desktop)
        {
            var isEditor = false;
            var isWebGL = false;
            #if UNITY_EDITOR
                isEditor = true;
            #endif

            #if UNITY_WEBGL
                isWebGL = true;
            #endif            
            
            return isEditor? DeviceType.Editor : isWebGL? DeviceType.WebGL : DeviceType.Window;
        }
        else if(SystemInfo.deviceType == UnityEngine.DeviceType.Handheld)
        {
            var isAndroid = false;
            #if UNITY_ANDROID
                isAndroid = true;
            #endif

            #if UNITY_IOS
                isAndroid = false;
            #endif

            return isAndroid? DeviceType.Android : DeviceType.IOS;
        }
        else
        {
            var isWebGL = false;
            #if UNITY_WEBGL
                isWebGL = true;
            #endif

            return isWebGL? DeviceType.WebGL : DeviceType.Mac;
        }
    }

    IEnumerator ReadFromStreamingAssets()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Database.bytes");
        byte[] result = new byte[]{};
        Debug.Log(filePath);
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            result = www.downloadHandler.data;
        }
        else
        {   
            result = System.IO.File.ReadAllBytes(filePath);
            Debug.Log("HERE:   ");
        }
        Debug.Log(result);    
    }
}
