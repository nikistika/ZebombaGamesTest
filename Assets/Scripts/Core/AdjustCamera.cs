using UnityEngine;

namespace Core
{
    public class AdjustCamera : MonoBehaviour
    {
        private void Start()
        {
            var targetAspect = 9f / 16f;
            var windowAspect = Screen.width / (float)Screen.height;
            var scaleHeight = windowAspect / targetAspect;
            var cameraMain = Camera.main;
 
            if (scaleHeight < 1.0f)
            {
                var rect = cameraMain.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                cameraMain.rect = rect;
            }
        }
    }
}