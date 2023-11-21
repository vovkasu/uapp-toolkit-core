using UnityEngine;

namespace UAppToolKit.Core.Input
{
    public class Pointer
    {
        private readonly PointerActionEnum _action;
        private readonly int _id;
        private readonly Vector3 _position;

        public Pointer(int id, PointerActionEnum action, Vector3 position)
        {
            _id = id;
            _action = action;
            _position = position;
        }

        public int Id
        {
            get { return _id; }
        }

        public PointerActionEnum Action
        {
            get { return _action; }
        }

        public Vector3 Position
        {
            get { return _position; }
        }
    }
}