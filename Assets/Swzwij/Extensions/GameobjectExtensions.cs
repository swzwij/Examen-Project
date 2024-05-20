// Copyright (c) 2024 Samuel Zwijsen (swzwij)
// This work is licensed under a varient of the MIT License Agreement.
// To view a copy of this license, visit the License URL (https://swzwij.notion.site/Tool-License-4b6f56a8be234a9dbf6ee3da31e71a92).
// 
// NOTICE: You must provide appropriate credit to the author 
// (see license for details).

using System.Collections.Generic;
using UnityEngine;

namespace Swzwij.Extensions
{
    public static class GameobjectExtensions
    {
        /// <summary>
        /// Dictionary storing cached components associated with GameObjects for fast retrieval using TryGetCachedComponent method.
        /// The outer dictionary uses GameObject instances as keys, 
        /// while the inner dictionary stores components with their type names as keys.
        /// </summary>
        private static readonly Dictionary<GameObject, Dictionary<string, Component>> _tryGetCachedComponents = new();

        /// <summary>
        /// Tries to retrieve a cached component of type T associated with the specified GameObject. 
        /// If the component is cached, returns it; otherwise, fetches the component, caches it, and returns it.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="gameObject">The GameObject to retrieve the component from.</param>
        /// <returns>The cached or newly fetched component of type T.</returns>
        public static T TryGetCachedComponent<T>(this GameObject gameObject)
            where T : Component
        {
            if (!_tryGetCachedComponents.TryGetValue(gameObject, out Dictionary<string, Component> dict))
            {
                dict = new Dictionary<string, Component>();
                _tryGetCachedComponents.Add(gameObject, dict);
            }

            string typeName = typeof(T).Name;

            if (dict.TryGetValue(typeName, out Component cachedComponent))
                return (T)cachedComponent;

            T component = gameObject.GetComponent<T>();
            dict[typeName] = component;
            return component;
        }

        /// <summary>
        /// Gets the specified component of type T from the GameObject. 
        /// If the component doesn't exist, adds it and returns it.
        /// </summary>
        /// <typeparam name="T">The type of component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to get or add the component to.</param>
        /// <returns>The component of type T.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
            => gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();

        /// <summary>
        /// Checks if a specified component is attached to the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to check for.</typeparam>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <returns>Whether the component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject)
            where T : Component
            => gameObject.TryGetComponent<T>(out _);
    }
}