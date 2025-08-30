using System;
using UnityEngine;
using Zenject;

namespace Services
{
    public class InputService: ITickable
    {
        public event Action Tap;
        public event Action Escape;
        private bool _touched;
        
        public void Tick()
        {
            if (Input.GetMouseButtonDown(0) || (!_touched && Input.touchCount > 0 && Input.GetTouch(0).tapCount == 1))
            {
                _touched = true;
                Tap?.Invoke();
            }

            if (Input.touchCount == 0)
            {
                _touched = false;
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                Escape?.Invoke();
            }
        }
    }
}