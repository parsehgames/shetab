using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SeganX
{
    public class Http : MonoBehaviour
    {
        public enum Status { Ready, Downloading, Done, NetworkError, ServerError }

        public class Request
        {
            public Status status = Status.Ready;
            public int timeout = 0;
            public int efforts = 1;
            public bool queued = false;
            public string url = string.Empty;
            public string postData = null;
            public Dictionary<string, string> header = null;
            public Action<Status, string> onCompleted = null;
            public Action<float> onProgress = null;
        }

        private float requestTime = 0;

        private void Awake()
        {
            instance = this;
        }

        private void DownloadText(Request request)
        {
            StartCoroutine(DoDownloadText(request));
        }

        private IEnumerator DoDownloadText(Request request)
        {
            // handle reqest delay time
            if (request.queued)
            {
                var delta = Time.time - requestTime;
                while (delta < 1f) yield return null;
                requestTime = Time.time;
            }

            while (request.efforts-- > 0)
            {
                var web = new UnityWebRequest(request.url);
                web.downloadHandler = new DownloadHandlerBuffer();

                if (request.postData != null)
                {
                    web.method = UnityWebRequest.kHttpVerbPOST;
                    web.uploadHandler = new UploadHandlerRaw(request.postData.GetBytes());
                    web.uploadHandler.contentType = "application/json";
                }

                web.SetRequestHeader("Accept", "*/*");
                web.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                if (request.header != null)
                    foreach (var item in request.header)
                        web.SetRequestHeader(item.Key, item.Value);

                //  print the package
                {
                    string debug = "\n" + web.method + " " + request.url;
#if SX_EXDBG
                    if (request.header != null) debug += "\nHeader: " + request.header.GetStringDebug();
#endif
                    if (request.postData != null) debug += "\nPostData:" + request.postData;
                    Debug.Log(debug + "\n");
                }

                var startTime = Time.time;
                request.status = Status.Downloading;
                var operation = web.SendWebRequest();
                while (operation.isDone == false)
                {
                    request.onProgress?.Invoke(operation.progress);

                    if (request.timeout > 0)
                    {
                        var delta = Time.time - startTime;
                        if (delta > request.timeout) break;
                    }

                    yield return null;
                }

                //  print the result
                Debug.Log(
                    "\nDownloaded " + web.downloadedBytes + " Bytes from " + web.method + " " + request.url +
#if SX_EXDBG
                    "\nHeader: " + web.GetResponseHeaders().GetStringDebug() +
#endif
                    "\nError: " + (web.error.HasContent() ? web.error : "No error") +
                    "\n" + web.downloadHandler.text + "\n");

                if (web.isDone && web.responseCode == 200)
                {
                    requests.Remove(request);

                    request.efforts = 0;
                    request.status = Status.Done;
                    request.onCompleted?.Invoke(request.status, web.downloadHandler.text.GetWithoutBOM());
                }
                else
                {
                    request.status = web.isHttpError ? Status.ServerError : Status.NetworkError;
                    if (request.efforts-- > 0)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        requests.Remove(request);
                        request.onCompleted?.Invoke(request.status, null);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static string userheader = string.Empty;
        public static List<Request> requests = new List<Request>(16);

        private static Http instance = null;

        internal static Http Instance
        {
            get
            {
                if (instance == null && GameManager.Game)
                {
                    DontDestroyOnLoad(instance = GameManager.Game.gameObject.AddComponent<Http>());
                }
                return instance;
            }
        }

        public static void DownloadText(string url, string postdata, Dictionary<string, string> header, Action<Status, string> onCompleted, Action<float> onProgress = null, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                header = header,
                onCompleted = onCompleted,
                onProgress = onProgress
            };
            requests.Add(request);
            Instance.DownloadText(request);
        }

        public static void DownloadText(string url, string postdata, Dictionary<string, string> header, Action<string> onCompleted, Action<float> onProgress = null, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                header = header,
                onCompleted = (st, txt) => onCompleted(txt),
                onProgress = onProgress
            };
            requests.Add(request);
            Instance.DownloadText(request);
        }

        public static void DownloadText(string url, Action<string> onCompleted, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                onCompleted = (st, txt) => onCompleted(txt),
            };
            requests.Add(request);
            Instance.DownloadText(request);
        }

        public static void DownloadText(string url, string postdata, Action<string> onCompleted, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                onCompleted = (st, txt) => onCompleted(txt),
            };
            requests.Add(request);
            Instance.DownloadText(request);
        }
    }
}