using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Util
{
  public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
  {
    return (layerMask.value & (1 << obj.layer)) > 0;
  }

  public static async Task<T> RetryIfFail<T>(Func<Task<T>> action, int retries = 5, int waitTime = 5000)
  {
    for (int i = 1; i <= retries; i++)
    {
      try
      {
        Debug.Log($"Try {i} out of {retries}");
        return await action();
      }
      catch (Exception ex)
      {
        Debug.Log(ex);
        await UniTask.Delay(waitTime);
      }
    }
    return default;
  }

  public static async Task<UnityWebRequest> WaitForRequest(UnityWebRequest request) {
    AsyncOperation op = request.SendWebRequest();
    while (op.isDone == false)
    {
      if (!Application.isPlaying) return default;
      await UniTask.Delay(15);
    }
    return request;
  }

  public static async Task<AudioClip> GetAudioClip(string youtubeURL)
  {
    var form = new WWWForm();
    form.AddField("url", youtubeURL);
    form.AddField("converter", "ffmpeg-mp3");
    using UnityWebRequest post = UnityWebRequest.Post("https://api.onlinevideoconverter.pro/api/convert", form);
    await WaitForRequest(post);

    var response = JsonUtility.FromJson<Root>(post.downloadHandler.text);
    var taskName = response.resource.taskName;
    using UnityWebRequest get = UnityWebRequestMultimedia.GetAudioClip($"https://en.onlinevideoconverter.pro/api/storage/{taskName}", AudioType.MPEG);
    get.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
    await WaitForRequest(get);
    return DownloadHandlerAudioClip.GetContent(get);
  }
}