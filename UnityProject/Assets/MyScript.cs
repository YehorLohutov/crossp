using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft;
using Newtonsoft.Json;

public class MyScript : MonoBehaviour
{
    public Image image;
    public Button Button;
    public Text buttonText;
    public Ad Ad;

    void Start()
    {
        try
        {
            buttonText = Button.GetComponentInChildren<Text>();
            StartCoroutine(GetText());
        }
        catch(Exception ex)
        {
            Button.GetComponentInChildren<Text>().text = ex.Message;
        }
    }

    //async void test1()
    //{
    //    HttpClient httpClient = new HttpClient();
    //    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://crossp.azurewebsites.net/ads/projectid-2");
    //    string json = await httpResponseMessage.Content.ReadAsStringAsync();

  
    //}

    //void test2()
    //{
    //    HttpWebRequest request = WebRequest.Create("https://crossp.azurewebsites.net/ads/projectid-1") as HttpWebRequest;
    //    request.Method = "GET";
    //    request.Accept = "application/json";

    //    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    StringBuilder stringBuilder = new StringBuilder();
    //    stringBuilder.Append(reader.ReadToEnd());
    //    response.Close();
    //    string result = stringBuilder.ToString();

    //}
    IEnumerator GetText()
    {
        buttonText.text += "1";
        UnityWebRequest www = UnityWebRequest.Get("https://crossp.azurewebsites.net/ads/projectid-1");
        buttonText.text += "2";
        yield return www.SendWebRequest();
        buttonText.text += "3";
        if (www.isNetworkError || www.isHttpError)
        {
            buttonText.text += "4";
            buttonText.text += www.error;
            Debug.Log(www.error);
        }
        else
        {
            buttonText.text += "5";
            // Show results as text
            string text = www.downloadHandler.text;
            Debug.Log(text);



            List<Ad> deserializedProduct = JsonConvert.DeserializeObject<List<Ad>>(text);
            buttonText.text += "6";
            Ad = deserializedProduct[0];
            buttonText.text += "7";
            Button.GetComponentInChildren<Text>().text += Ad.Url;
            StartCoroutine(GetImage(Ad));

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
            //using (MemoryStream ms = new MemoryStream(results))
            //{
            //    BinaryFormatter binaryFormatter = new BinaryFormatter();
            //    Ad ad = (Ad)binaryFormatter.Deserialize(ms);
            //}

            //
        }
    }
    public void OpenUrl()
    {
        UnityEngine.Application.OpenURL(Ad.Url);
    }

    IEnumerator GetImage(Ad ad)
    {
        buttonText.text += "8";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://crossp.azurewebsites.net" + ad.Img);
        buttonText.text += "9";
        yield return request.SendWebRequest();
        buttonText.text += "10";
        if (request.isNetworkError || request.isHttpError)
        {
            buttonText.text += "11"; 
            buttonText.text += request.error;
            Debug.Log(request.error);
        }
        else
        {
            buttonText.text += "12";
            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero);
            image.sprite = sprite;
            buttonText.text += "13";
        }
    }
}
