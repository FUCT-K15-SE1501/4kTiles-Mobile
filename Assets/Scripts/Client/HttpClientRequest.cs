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
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public HttpClientRequest(string baseUrl)
        {
            _baseUrl = NormalizeUrl(baseUrl);
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

        private void AddHeaders(UnityWebRequest www)
        {
            foreach (var header in Headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }
        }

        public static UnityAction<UnityWebRequest> ConvertToResponseAction<T>(UnityAction<RequestResult<T>> resultAction)
        {
            return delegate(UnityWebRequest webRequest)
            {
                var result = new RequestResult<T>
                {
                    StatusCode = webRequest.responseCode,
                    IsSuccess = webRequest.result == UnityWebRequest.Result.Success,
                    RawData = webRequest.downloadHandler.text
                };
                try
                {
                    result.Result = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    result.IsParseSuccess = true;
                }
                catch
                {
                    result.IsParseSuccess = false;
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
            AddHeaders(www);
            yield return www.SendWebRequest();

            result.Invoke(www);
        }

        public IEnumerator Put(string path, string putData, UnityAction<UnityWebRequest> result)
        {
            var bytes = Encoding.UTF8.GetBytes(putData);
            var www = UnityWebRequest.Put(GetFullPath(path), bytes);
            AddHeaders(www);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            
            result.Invoke(www);
        }

        public IEnumerator Post(string path, string postData, UnityAction<UnityWebRequest> result)
        {
            var bytes = Encoding.UTF8.GetBytes(postData);
            var uploadHandler = new UploadHandlerRaw(bytes);
            var www = UnityWebRequest.Post(GetFullPath(path), postData);
            AddHeaders(www);
            www.uploadHandler = uploadHandler;
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            result.Invoke(www);
        }
    }

    public class RequestResult<T>
    {
        public T Result { get; set; } = default;
        public bool IsParseSuccess { get; set; } = false;
        public string RawData { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;
        public long StatusCode { get; set; }
    }
}
