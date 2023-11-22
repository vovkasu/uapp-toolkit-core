using System;
using UAppToolKit.Core.Application;
using UnityEngine.UI;

namespace UAppToolKit.Core.Popup
{
    public class PopUpBase : PopUpAnimationController
    {
        public Button CloseButton;
        public event Action ClosedEvent;
        
        private Action _onPopupDestroy;

        protected virtual void OnClosedEvent()
        {
            Action handler = ClosedEvent;
            if (handler != null) handler();
        }

        protected virtual void Awake()
        {
            if (CloseButton != null) CloseButton.onClick.AddListener(EntryPointBase.Current.NavigationControllerBase.GoBack);
            Show();
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
        }

        protected virtual void OnDestroy()
        {
            OnClosedEvent();
        }

        private void OnDisable()
        {
            if (_onPopupDestroy != null)
            {
                _onPopupDestroy();
            }
            _onPopupDestroy = null;
        }

        public virtual void Close(Action onComplete)
        {
            _onPopupDestroy = onComplete;
            if (onComplete != null) onComplete();
        }
    }
}