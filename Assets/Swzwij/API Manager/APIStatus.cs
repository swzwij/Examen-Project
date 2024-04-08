// Copyright (c) 2024 Samuel Zwijsen (swzwij)
// This work is licensed under a varient of the MIT License Agreement.
// To view a copy of this license, visit the License URL (https://swzwij.notion.site/Tool-License-4b6f56a8be234a9dbf6ee3da31e71a92).
// 
// NOTICE: You must provide appropriate credit to the author 
// (see license for details).

using UnityEngine.Networking;

namespace Swzwij.APIManager
{
    /// <summary>
    /// Represents the status of an API request, including success, failure, or specific error conditions.
    /// </summary>
    public class APIStatus
    {
        /// <summary>
        /// The UnityWebRequest associated with the API request.
        /// </summary>
        private readonly UnityWebRequest _request;

        /// <summary>
        /// Gets a description of the request's status, providing information about its success or failure.
        /// </summary>
        public string Status
        {
            get
            {
                return _request.result switch
                {
                    UnityWebRequest.Result.InProgress => "The request is currently in progress and hasn't finished yet.",
                    UnityWebRequest.Result.Success => "The request was successful, and data was received as expected.",
                    UnityWebRequest.Result.ConnectionError => "The request failed due to a problem in communicating with the server.",
                    UnityWebRequest.Result.ProtocolError => "The server returned an error response. The request successfully communicated with the server, but the server's response indicated an error as defined by the connection protocol.",
                    UnityWebRequest.Result.DataProcessingError => "Error occurred while processing data received from the server. The request successfully communicated with the server, but an error was encountered when handling the received data.",
                    _ => "The request status is unknown or could not be determined."
                };
            }
        }

        /// <summary>
        /// Initializes a new instance of the APIStatus class with the provided UnityWebRequest result.
        /// </summary>
        /// <param name="result">The web request.</param>
        public APIStatus(UnityWebRequest result) => _request = result;
    }
}