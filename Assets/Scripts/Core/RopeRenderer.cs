namespace Core
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public class RopeRenderer : MonoBehaviour
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _ball;
        [SerializeField] private LineRenderer _lineRenderer;

        public void SetBall(Transform ball)
        {
            _lineRenderer.enabled = true;
            _ball = ball;
            Refresh();
        }

        public void Hide()
        {
            _lineRenderer.enabled = false;
        }
        
        private void OnValidate()
        {
            _lineRenderer ??= GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _lineRenderer.positionCount = 2;
        }

        private void Update()
        {
            if (_ball == null) return;

            Refresh();
        }

        private void Refresh()
        {
            _lineRenderer.SetPosition(0, _pivot.position);
            _lineRenderer.SetPosition(1, _ball.position);
        }
    }
}