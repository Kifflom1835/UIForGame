﻿using System.Collections;
using System.Reflection;
using Common.Helpers.Coroutines;
using Common.Helpers.Tweeks.CurveAnimationHelper;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.AudioManager
{
    public class ManagedAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioManager.EAudioSource sourceType;
        [SerializeField] private AudioSource audioSource;
        
        /// <summary>
        /// For smooth clips changing. If null <see cref="PlaySmooth"/> can't be used.
        /// </summary>
        [SerializeField] [CanBeNull] private AudioSource secondAudioSource = null;

        private Coroutine _smoothPlayCoroutine;
        private Coroutine _decreasingVolumeCoroutine;
        private Coroutine _stopPlayingCoroutine;
        private AudioSource _activeSource;

        public AudioManager.EAudioSource SourceType => sourceType;

        private void Awake()
        {
            _activeSource = audioSource;
        }

        public bool IsPlaying(AudioClip clip)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
            return _activeSource.clip != null && _activeSource.clip.Equals(clip);
        }
        
        public void PlayOneShot(AudioClip clip)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
        }
        
        public void Play(AudioClip clip, bool loop = false)
        {
            if (clip == null) Debug.LogError("AudioClip is null.");
            _activeSource.Stop();
            _activeSource.loop = loop;
            _activeSource.clip = clip;
            _activeSource.Play();
        }

        /// <summary>
        /// Makes clip start playing smooth. like this:
        /// 
        /// old clip -            -- new clip full volume
        ///             -        --   
        ///                 --
        ///             --       -
        /// new clip --             - old clip stops
        ///
        /// </summary>
        public Coroutine PlaySmooth(AudioClip clip, bool loop = false, float changeTime = 1f)
        {

            return CoroutineHelper.RestartCoroutine(ref _stopPlayingCoroutine, PlaySmoothProcess(clip, loop, changeTime),
                    this);
        }

        public Coroutine StopSmooth(float stopTime = 1f)
        {
            return CoroutineHelper.RestartCoroutine(ref _stopPlayingCoroutine, 
                StopPlayingSmoothProcess(stopTime), this);
        }

        private IEnumerator PlaySmoothProcess(AudioClip clip, bool loop, float changeTime)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null.");
                yield break;
            }

            var decreasingVolumeSource = _activeSource;
            var increasingVolumeSource = GetFreeSource();
            
            _activeSource = increasingVolumeSource;
            
            if (decreasingVolumeSource.isPlaying)
            {
                if (decreasingVolumeSource.clip != null && decreasingVolumeSource.clip.Equals(clip)) yield break;
                CoroutineHelper.RestartCoroutine(ref _decreasingVolumeCoroutine,
                    CurveAnimationHelper.LerpFloatByCurve
                    (
                        result => decreasingVolumeSource.volume = result,
                        decreasingVolumeSource.volume,
                        0,
                        timeOrSpeed: changeTime,
                        onDone: () => { decreasingVolumeSource.Stop(); }
                    ),
                    this);
            }
            
            increasingVolumeSource.Stop();
            increasingVolumeSource.loop = loop;
            increasingVolumeSource.clip = clip;
            increasingVolumeSource.Play();
        }

        private IEnumerator StopPlayingSmoothProcess(float stopTime)
        {
            var decreasingVolumeSource = _activeSource;
            if (decreasingVolumeSource.isPlaying)
            {
                if (decreasingVolumeSource.clip != null) yield break;
                yield return CoroutineHelper.RestartCoroutine(ref _decreasingVolumeCoroutine,
                    CurveAnimationHelper.LerpFloatByCurve
                    (
                        result => decreasingVolumeSource.volume = result,
                        decreasingVolumeSource.volume,
                        0,
                        timeOrSpeed: stopTime,
                        onDone: () => { decreasingVolumeSource.Stop(); }
                    ),
                    this);
            }
            else
            {
                yield return null;
            }
        }
        
        private AudioSource GetFreeSource()
        {
            return ReferenceEquals(_activeSource, audioSource) ? secondAudioSource : audioSource;
        }
    }
}