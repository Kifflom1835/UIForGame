﻿using System;
using Common.Helpers.MonoBehaviourExtenders;
using JetBrains.Annotations;
using UnityEngine;

namespace Common.Helpers.UI.BaseUiElements
{
    public class BaseWindowController<T> : GenericSingleton<T> where T: Component
    {
        [Header("= BaseWindowController Fields =")]
        [SerializeField] protected BaseUiElement uiContainer;
        
        [SerializeField] [CanBeNull] 
        protected BaseInteractableUiElement closeButton;

        public Action OnClosed;
        public Action OnOpened;
        
        public bool IsOpened => uiContainer.IsEnabled;

        protected override void InheritAwake()
        {
            base.InheritAwake();
            if (!(closeButton is null)) closeButton.OnClick += OnCloseButtonClick;
        }

        public virtual Coroutine Open([CanBeNull] Action onOpened = null)
        {
            return uiContainer.Enable(() =>
            {
                OnOpened?.Invoke();
                onOpened?.Invoke();
            });
        }

        public virtual Coroutine Close([CanBeNull] Action onClosed = null)
        {
            return uiContainer.Disable(() =>
            {
                OnClosed?.Invoke();
                onClosed?.Invoke();
            });
        }

        public virtual void ForceClose()
        {
            uiContainer.ForceDisable();
        }
        
        public virtual void ForceOpen()
        {
            uiContainer.ForceEnable();
        }
        
        public void RunClose()
        {
            Close();
        }
        
        protected virtual void OnCloseButtonClick()
        {
            Close();
        }
    }
}