using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Client
{
    public static class ClientConstants
    {
        public static readonly HttpClientRequest API = new HttpClientRequest("https://fktiles-api.herokuapp.com/api/");
        public static readonly HttpClientRequest AvatarAPI = new HttpClientRequest("https://avatars.dicebear.com/api/");
    }

    public class HttpClientRequest
    {
        private readonly string _baseUrl;
        public Dictionary<string, string> Headers { get; private set; }

        public HttpClientRequest(string baseUrl)
        {
            _baseUrl = NormalizeUrl(baseUrl);
            Headers = new Dictionary<string, string>();
        }

        private static string NormalizeUrl(string url)
        {
            if (!url.EndsWith("/"))
                url += "/";
            return url;
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (path.StartsWith("/"))
                path = path.Substring(1);
            if (!path.EndsWith("/"))
                path += "/";
            return path;
        }

        private string GetFullPath(string path) => _baseUrl + NormalizePath(path);

        public static UnityAction<UnityWebRequest> ConvertToResponseAction<T>(UnityAction<RequestResult<T>> resultAction)
        {
            return delegate(UnityWebRequest webRequest)
            {
                var result = new RequestResult<T>();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    result.StatusCode = webRequest.responseCode;
                    result.IsSuccess = true;
                    result.Result = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                }
                else
                {
                    result.StatusCode = webRequest.responseCode;
                    result.IsSuccess = false;
                }

                resultAction.Invoke(result);
            };
        } 

        public IEnumerator Get(string path, UnityAction<UnityWebRequest> result, Dictionary<string, string> query = null)
        {
            var queryString = string.Empty;
            if (query != null)
            {
                foreach (var keyValuePair in query)
                {
                    if (queryString.Length > 0)
                        queryString += "&";

                    queryString += keyValuePair.Key + "=" + UnityWebRequest.EscapeURL(keyValuePair.Value);
                }
            }

            var www = UnityWebRequest.Get(GetFullPath(path) + (queryString.Length > 0 ? "?" + queryString : ""));
            yield return www.SendWebRequest();

            result.Invoke(www);
        }

        public IEnumerator Put(string path, string putData, UnityAction<UnityWebRequest> result)
        {
            var bytes = Encoding.UTF8.GetBytes(putData);
            var www = UnityWebRequest.Put(GetFullPath(path), bytes);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            
            result.Invoke(www);
        }

        public IEnumerator Post(string path, string postData, UnityAction<UnityWebRequest> result)
        {
            var bytes = Encoding.UTF8.GetBytes(postData);
            var uploadHandler = new UploadHandlerRaw(bytes);
            var www = UnityWebRequest.Post(GetFullPath(path), postData);
            www.uploadHandler = uploadHandler;
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            result.Invoke(www);
        }
    }

    public class RequestResult<T>
    {
        public T Result { get; set; } = default;
        public bool IsSuccess { get; set; } = false;
        public long StatusCode { get; set; }
    }
}
