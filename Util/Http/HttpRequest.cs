using System.Collections;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

class Result<T>
{
    public bool complete;
    public bool succ;
    public T value;
}

public class HttpRequest
{
    Result<JsonData> result = new Result<JsonData>();

    public bool isComplete => result.complete;
    public bool isSucc => result.succ;
    public JsonData value => isSucc && isComplete ? result.value : new JsonData();
    public byte[] rawData;

    public IEnumerator Get(string url)
    {
        result.complete = false;
        if (string.IsNullOrEmpty(url))
        {
            result.complete = true;
            result.succ = false;
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            result.complete = true;
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                result.succ = false;
            }
            else
            {
                result.value = JsonMapper.ToObject(webRequest.downloadHandler.text);
                rawData = webRequest.downloadHandler.data;
                result.succ = true;
            }
        }
    }


    public IEnumerator Post(string url, WWWForm form)
    {
        result.complete = false;
        if (string.IsNullOrEmpty(url))
        {
            result.complete = true;
            result.succ = false;
            yield break;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();
            result.complete = true;
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                result.succ = false;
            }
            else
            {
                result.value = JsonMapper.ToObject(webRequest.downloadHandler.text);
                result.succ = true;
            }
        }
    }
}