// Copyright (c) 2024 Samuel Zwijsen (swzwij)
// This work is licensed under a varient of the MIT License Agreement.
// To view a copy of this license, visit the License URL (https://swzwij.notion.site/Tool-License-4b6f56a8be234a9dbf6ee3da31e71a92).
// 
// NOTICE: You must provide appropriate credit to the author 
// (see license for details).

using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

namespace Swzwij.APIManager
{
    /// <summary>
    /// Manager class for handling API requests and responses.
    /// </summary>
    public class APIManager : MonoBehaviour
    {
        #region Singleton Behaviour

        /// <summary>
        /// Singleton instance of the APIManager.
        /// </summary>
        private static APIManager _instance;

        /// <summary>
        /// Accessor for the Singleton instance of the APIManager.
        /// If the instance does not exist, it creates one and ensures it persists across scenes.
        /// </summary>
        public static APIManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<APIManager>();

                if (_instance != null)
                    return _instance;

                GameObject singletonObject = new(typeof(APIManager).Name); ;
                _instance = singletonObject.AddComponent<APIManager>();
                DontDestroyOnLoad(singletonObject);

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Indicates whether the application is running in development mode.
        /// </summary>
        [Tooltip("Indicates whether the application is running in development mode.")]
        [SerializeField]
        private bool _development = false;

        /// <summary>
        /// The domain name or base URL of the API.
        /// </summary>
        [Tooltip("The domain name or base URL of the API.")]
        [SerializeField]
        private string _domain;

        /// <summary>
        /// The domain name or base URL of the API for development purposes.
        /// </summary>
        [Tooltip("The domain name or base URL of the API for development purposes.")]
        [SerializeField]
        private string _developmentDomain;

        /// <summary>
        /// Sends a GET request to the specified API endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        public void GetCall<T>(APIRequest request, Action<T> onComplete = null, Action<APIStatus> onFailure = null) =>
            StartCoroutine(GetRequest(request, onComplete, onFailure));

        /// <summary>
        /// Coroutine for sending a web request and handling the response.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        private IEnumerator GetRequest<T>(APIRequest request, Action<T> onComplete, Action<APIStatus> onFailure)
        {
            string url = _development ? $"{_developmentDomain}{request.URL}" : $"{_domain}{request.URL}";

            UnityWebRequest webRequest = new(url)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onFailure?.Invoke(new(webRequest));
                yield break;
            }

            T response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
            onComplete?.Invoke(response);
        }

        /// <summary>
        /// Sends a POST request to the specified API endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="data">Data to be sent in the POST request body.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        public void PostCall<T>(APIRequest request, object data, Action<T> onComplete = null, Action<APIStatus> onFailure = null) =>
            StartCoroutine(PostRequest(request, data, onComplete, onFailure));

        /// <summary>
        /// Coroutine for sending a POST request and handling the response.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="data">Data to be sent in the POST request body.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        private IEnumerator PostRequest<T>(APIRequest request, object data, Action<T> onComplete, Action<APIStatus> onFailure)
        {
            string jsonData = JsonUtility.ToJson(data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            string url = _development ? $"{_developmentDomain}{request.URL}" : $"{_domain}{request.URL}";

            UnityWebRequest webRequest = new(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(dataBytes),
                downloadHandler = new DownloadHandlerBuffer()
            };
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onFailure?.Invoke(new(webRequest));
                yield break;
            }

            T response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
            onComplete?.Invoke(response);
        }
    }
}