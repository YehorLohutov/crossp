using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MyScript : MonoBehaviour
{
    public Image image;
    void Start()
    {
        StartCoroutine(GetText());
        testc();
    }

    async void testc()
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://crossp.azurewebsites.net/ads/projectid-2");
        string json = await httpResponseMessage.Content.ReadAsStringAsync();

        //BinaryFormatter binaryFormatter = new BinaryFormatter();

        //Debug.Log(json) ;
  
    }
    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://crossp.azurewebsites.net/ads/projectid-2");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            
            // Show results as text
            string text = www.downloadHandler.text;
            Debug.Log(text);




            var ad = JsonUtility.FromJson<Ad>(www.downloadHandler.text);
            
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            //using (MemoryStream ms = new MemoryStream(results))
            //{
            //    BinaryFormatter binaryFormatter = new BinaryFormatter();
            //    Ad ad = (Ad)binaryFormatter.Deserialize(ms);
            //}

            //
        }
    }
}
