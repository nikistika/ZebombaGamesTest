using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Services
{
    public class BallServices
    {
        [Inject] private AppSettings _appSettings;
        [Inject] private DiContainer _diContainer;
        
        private List<BallView> _balls = new();
        private BallView _boundBall;
        private int _ballIndex;
        
        private readonly Vector2 _ballPositionLeft = new(-1.88f, 3.52f);
        private readonly Vector2 _ballPositionRight = new(1.88f, 3.52f);

        public event Action<BallView, Collider2D> BallTriggerEntered;
        public event Action<BallView> BallStopped;
        
        private int BallIndex() => _ballIndex++; 

        public BallView CreateBall()
        {
            // Debug.Log($"AppSetting: {JsonConvert.SerializeObject(_appSettings, new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore})}");
            var newBall = _diContainer.InstantiatePrefabForComponent<BallView>(_appSettings.BallPrefab);
            var isLeft = Random.Range(0, 2) == 0;
            var ballPosition = isLeft ? _ballPositionLeft : _ballPositionRight;
            newBall.gameObject.name = $"Ball{BallIndex()}";
            Debug.Log($"ballPosition: {ballPosition}");
            newBall.Init(ballPosition);
            newBall.TriggerEntered += OnBallTriggerEntered;
            newBall.Stopped += OnBallStopped;
            _balls.Add(newBall);
            
            return newBall;
        }

        public void RemoveBall(BallView ballView)
        {
            ballView.TriggerEntered -= OnBallTriggerEntered;
            ballView.Stopped -= OnBallStopped;
            ballView.Destroy();
            _balls.Remove(ballView);
        }

        private void OnBallStopped(BallView ball)
        {
            if (ball.IsIntoColumn == false)
                RemoveBall(ball);
            
            BallStopped?.Invoke(ball);
        }

        private void OnBallTriggerEntered(BallView ballView, Collider2D columnTrigger)
        {
            BallTriggerEntered?.Invoke(ballView, columnTrigger);
        }
    }
}