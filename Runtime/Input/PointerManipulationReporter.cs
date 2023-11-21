using System.Collections.Generic;
using UnityEngine;

namespace UAppToolKit.Core.Input
{
    public delegate void ManipulatedEventHandler(object sender, List<Pointer> args);
    public class PointerManipulationReporter : MonoBehaviour
    {
        protected bool WasLmbDown;
        public event ManipulatedEventHandler Manipulated;

        protected virtual void OnManipulated(List<Pointer> args)
        {
            ManipulatedEventHandler handler = Manipulated;
            if (handler != null) handler(this, args);
        }

        protected List<Pointer> GetTouchList()
        {
            var pointers = new List<Pointer>();
            foreach (Touch touch in UnityEngine.Input.touches)
            {
                var pointerActionEnum = PointerActionEnum.Move;

                if (touch.phase == TouchPhase.Moved)
                {
                    pointerActionEnum = PointerActionEnum.Move;
                }
                else if (touch.phase == TouchPhase.Began)
                {
                    pointerActionEnum = PointerActionEnum.Down;
                }
                else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {
                    pointerActionEnum = PointerActionEnum.Up;
                }
                pointers.Add(new Pointer(touch.fingerId, pointerActionEnum, touch.position));
            }
            return pointers;
        }

        protected List<Pointer> GetMouseList()
        {
            var pointers = new List<Pointer>();
            bool isLmbDown = UnityEngine.Input.GetMouseButton(0);

            if (!WasLmbDown && !isLmbDown)
            {
                return new List<Pointer>();
            }

            Vector3 mousePosition = UnityEngine.Input.mousePosition;

            if (WasLmbDown && !isLmbDown)
            {
                pointers.Add(new Pointer(-1, PointerActionEnum.Up, mousePosition));
                WasLmbDown = false;
                return pointers;
            }
            
            if (!WasLmbDown && isLmbDown)
            {
                pointers.Add(new Pointer(-1, PointerActionEnum.Down, mousePosition));
                WasLmbDown = true;
            }
            else if (WasLmbDown && isLmbDown)
            {
                pointers.Add(new Pointer(-1, PointerActionEnum.Move, mousePosition));
            }
            return pointers;
        }

        protected void RaiseEvent()
        {
            var touchList = GetTouchList();
            var points = touchList.Count > 0 ? touchList : GetMouseList();
            if (points.Count > 0)
            {
                OnManipulated(points);
            }
        }

        private void Update()
        {
            RaiseEvent();
        }
    }
}