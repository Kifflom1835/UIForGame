﻿using System;
using UnityEngine;

namespace Common.Helpers.MonoBehaviourExtenders
{
    /// <summary>
    /// Inherit from this class if you need component that has instance you want to refer later.
    /// </summary>
    public abstract class GenericSingleton<T> : CachedMonoBehaviour where T : Component
    {
        public static bool HasInstance => !ReferenceEquals(instance, null);
        
        public static T Instance {
        
            get {
                if (instance != null) return instance;
            
                var objectsOfType = (T[]) FindObjectsOfType (typeof(T));
                
                if (objectsOfType.Length == 1)
                {
                    instance = objectsOfType[0];
                    return instance;
                }

                Debug.LogError ("There is no any " + typeof(T).Name + " GameObject in the scene. Returned null.");
                return null;
            }
            protected set => instance = value;
        }
    
        // ReSharper disable once InconsistentNaming
        private static T instance;

        private void Awake()
        {
            if (ReferenceEquals(instance, null))
            {
                Instance = this as T;
            }
            else if(Instance != this as T)
            {
                throw new InvalidOperationException($"There is already exist instance of this singleton! " +
                                                    $"Instance object name: {instance.name} " +
                                                    $"This object name: {name}");
            }

            InheritAwake();
        }

        private void OnDestroy()
        {
            Instance = null;
            InheritOnDestroy();
        }

        protected virtual void InheritAwake()
        {
            
        }
        
        protected virtual void InheritOnDestroy()
        {
            
        }
    }
}
