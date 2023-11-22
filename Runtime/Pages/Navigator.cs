using System;
using System.Collections.Generic;
using UAppToolKit.Core.Application;
using UAppToolKit.Core.Popup;
using UnityEngine;

namespace UAppToolKit.Core.Pages
{
    public class Navigator : MonoBehaviour
    {
        public readonly List<PopUpBase> PopUps = new List<PopUpBase>();
        [HideInInspector]
        public bool IsActive;
        public float WaitTimeLoadScreen = 1.0f;
        public event EventHandler OnPageLoaded;
        
        public virtual void OnNavigatedTo(object arg)
        {
            IsActive = true;
            if (WaitTimeLoadScreen < 0)
            {
                WaitTimeLoadScreen = 1.0f;
            }
        }

        public virtual void OnNavigatedToCompleted()
        {
        }

        public virtual void OnNavigatedFrom()
        {
            IsActive = false;
        }

        public virtual void OnNavigatedFromCompleted()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (EntryPointBase.Current.NavigationControllerBase.BackNavigationByEscape)
                {
                    GoBackLoadingType();
                }
            }
        }

        protected virtual void GoBackLoadingType()
        {
            EntryPointBase.Current.NavigationControllerBase.GoBack();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}