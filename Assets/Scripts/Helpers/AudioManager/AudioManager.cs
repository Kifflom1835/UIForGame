﻿using System;
using System.Collections.Generic;
using Common.Helpers.MonoBehaviourExtenders;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Helpers.AudioManager
{
    public sealed class AudioManager : GenericSingleton<AudioManager>
    {
        [SerializeField] private AudioMixer audioMixer = null;
        [SerializeField] private ManagedAudioSource uiElementsResponseSource = null;
        [SerializeField] private List<ManagedAudioSource> audioSources = new List<ManagedAudioSource>();

        #region Constants

        public enum EAudioSource
        {
            None,
            UiElementResponse,
            GlobalSoundtrack,
            EffectsSource
        }

        #endregion
        
        #region Private Inner Logic

        private static readonly Dictionary<EAudioSource, ManagedAudioSource> AudioSources =
            new Dictionary<EAudioSource, ManagedAudioSource>();

        protected override void InheritAwake()
        {
            if (uiElementsResponseSource == null)
            {
                throw new NullReferenceException("[AudioManager] You must add source for UI elements feedback!");
            }

            if (audioMixer == null)
            {
                throw new NullReferenceException("[AudioManager] You must add persistent reference for audioMixer!");
            }

            AudioSources.Add(EAudioSource.UiElementResponse, uiElementsResponseSource);

            try
            {
                foreach (var audioSource in audioSources)
                {
                    AudioSources.Add(audioSource.SourceType, audioSource);
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError(
                    $"[AudioManager.InheritAwake] You added 2 audio sources with the same key in AudioManager. This is not allowed! Key: {e.ParamName}");
                throw;
            }
        }

        #endregion

        #region Public API

        public void ChangeVolume(string sourceName, float value)
        {
           // Debug.Log("Music Volume: "+Mathf.Log(value) * 20);
            audioMixer.SetFloat(sourceName, value); //Mathf.Log(value) * 20);
        }

        public ManagedAudioSource GetSource(EAudioSource sourceType)
        {
            if (AudioSources.TryGetValue(sourceType, out var source))
            {
                return source;
            }
            else
            {
                Debug.LogError($"There is no managed audio source with type \"{sourceType}\" registered.");
                return null;
            }
        }

        #endregion
    }
}
