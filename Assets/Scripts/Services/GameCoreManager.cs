using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Services
{
    public class GameCoreManager : MonoBehaviour
    {
        [Inject] private BallServices _ballServices;
        [Inject] private ScoreService _scoreService;
        [Inject] private InputService _inputService;
        [Inject] private SoundService _soundService;

        [SerializeField] private SpringJoint2D _springJoint;
        [SerializeField] private RopeRenderer _ropeRenderer;
        [SerializeField] private List<Collider2D> _columnTriggers;
        [SerializeField] private GameOverPopup _gameOverPopup;
        
        private BallView[,] _pinnedBalls = new BallView[Dimension, Dimension];
        private BallView _currentBall;
        private BallView _lastBall;

        private const int Dimension = 3;

        private void Start()
        {
            Time.timeScale = 2f;
            CreateBall();
            _inputService.Tap += CutRope;
        }

        private void OnDestroy()
        {
            _inputService.Tap -= CutRope;
        }

        private void CutRope()
        {
            _springJoint.connectedBody = null;
            _soundService.PlayCutRope();
            _ropeRenderer.Hide();
        }

        private void CreateBall()
        {
            _ballServices.BallTriggerEntered += BallServices_OnTriggerEntered;
            _ballServices.BallStopped += BallServices_OnBallStopped;
            
            _currentBall = _ballServices.CreateBall();
            _lastBall = _currentBall;
            _springJoint.connectedBody = _currentBall.GetComponent<Rigidbody2D>();
            _ropeRenderer.SetBall(_currentBall.transform);
        }

        private void RemoveBall(BallView ball)
        {
            _soundService.PlayRemove();
            var ballCoords = FindBallIndex(ball);
            
            _ballServices.RemoveBall(ball);

            if (ballCoords == null)
            {
                Debug.LogError($"pinnedBall not found in coors: {ballCoords}");
                return;
            }

            _pinnedBalls[ballCoords.Value.x, ballCoords.Value.y] = null;
        } 

        private void BallServices_OnBallStopped(BallView ball)
        {
            Refresh();
        }

        private void BallServices_OnTriggerEntered(BallView ballView, Collider2D columnTrigger)
        {
            if (_currentBall == null) return;

            var columnIndex = _columnTriggers.IndexOf(columnTrigger);

            if (columnIndex < 0)
            {
                Debug.LogError("Wrong index");
                return;
            }

            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                if (_pinnedBalls[columnIndex, rowIndex] == null)
                {
                    _pinnedBalls[columnIndex, rowIndex] = ballView;
                    Debug.Log($"Ball placed at [{columnIndex}, {rowIndex}]");
                    _currentBall = null;
                    return;
                }
            }

            Debug.LogError($"Column {columnIndex} is full! Ball cannot be placed.");
            RemoveBall(ballView);
            GameOver();
        }

        private void Refresh()
        {
            if (_currentBall == null)
            {
                CreateBall();
            }

            if (_lastBall != null && _lastBall.IsIntoColumn == false && _lastBall.IsSleeping)
            {
                GameOver();
            }

            FindLines();
            TryGameOver();
        }

        private void TryGameOver()
        {
            var isFull = IsFullGrid();
            
            Debug.Log($"Is full grid: {isFull}");
            
            if (isFull)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            if (_currentBall != null) 
                _currentBall.Sleep();
            
            _gameOverPopup.Show();
        }

        private void FindLines()
        {
            for (int i = 0; i < Dimension; i++)
            {
                FindLine(_pinnedBalls, i, 0, 0, 1, Dimension); // Горизонтальная линия
                FindLine(_pinnedBalls, 0, i, 1, 0, Dimension); // Вертикальная линия
            }

            FindLine(_pinnedBalls, 0, 0, 1, 1, Dimension); // Главная диагональ вниз
            FindLine(_pinnedBalls, Dimension - 1, 0, -1, 1, Dimension); // Побочная диагональ вверх
        }

        private  void FindLine(BallView[,] ballView, int startX, int startY, int dx, int dy, int length)
        {
            var currentLine = new List<Vector2Int>();
            var prevColor = ballView[startX, startY]?.Color;
        
            for (var i = 0; i < length; i++)
            {
                var x = startX + i * dx;
                var y = startY + i * dy;
                var color = ballView[x, y]?.Color;
            
                if (color != null && color == prevColor)
                {
                    currentLine.Add(new Vector2Int(x, y));
                }
                else
                {
                    return;
                }
            }

            if (currentLine.Count == length)
            {
                ReleaseLine(currentLine);
            }
        }

        private async UniTask ReleaseLine(List<Vector2Int> lineCoords)
        {
            Debug.Log($"ReleaseLine {lineCoords.ToString()}");

            var firstBallCoords = lineCoords.First();
            var color = _pinnedBalls[firstBallCoords.x, firstBallCoords.y].Color;
            _scoreService.AddScoreByLine(color);
            
            foreach (var coord in lineCoords)
            {
                var ball = _pinnedBalls[coord.x, coord.y];

                if (ball == null)
                {
                    Debug.LogError($"Ball[{coord.x},{coord.y}] not found");
                    return;
                }
                
                RemoveBall(ball);

                await UniTask.WaitForSeconds(0.2f);
            }

            UnsleepBalls();
        }

        private void UnsleepBalls()
        {
            var ballsEnumerator = _pinnedBalls.GetEnumerator();
            
            while (ballsEnumerator.MoveNext())
            {
                if (ballsEnumerator.Current is BallView ballView)
                {
                    ballView.Unsleep();
                }
            }
        }
        
        private Vector2Int? FindBallIndex(BallView targetBall)
        {
            for (var i = 0; i < _pinnedBalls.GetLength(0); i++)
            {
                for (var j = 0; j < _pinnedBalls.GetLength(1); j++)
                {
                    if (_pinnedBalls[i, j] == targetBall)
                    {
                        return new(i, j);
                    }
                }
            }
            
            return null;
        }
        
        private bool IsFullGrid()
        {
            var enumerator = _pinnedBalls.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    return false;
            }
            
            return true;
        }
    }
}