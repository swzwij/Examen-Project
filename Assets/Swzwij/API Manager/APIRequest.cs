// Copyright (c) 2024 Samuel Zwijsen (swzwij)
// This work is licensed under a varient of the MIT License Agreement.
// To view a copy of this license, visit the License URL (https://swzwij.notion.site/Tool-License-4b6f56a8be234a9dbf6ee3da31e71a92).
// 
// NOTICE: You must provide appropriate credit to the author 
// (see license for details).

namespace Swzwij.APIManager
{
    /// <summary>
    /// Abstract base class for defining API request configurations.
    /// </summary>
    public abstract class APIRequest
    {
        /// <summary>
        /// Gets the URL of the API endpoint to be requested.
        /// </summary>
        public abstract string URL { get; }
    }
}