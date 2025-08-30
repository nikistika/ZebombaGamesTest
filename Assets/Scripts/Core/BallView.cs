using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core
{
    public class BallView  : MonoBehaviour
    {
        [Inject] private AppSettings _appSettings;

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private BallPartsView _partsViewPrefab;
        
        private Rigidbody2D _rigidbody;
        private bool _isRealized;

        public bool IsSleeping { get; private set; }
        public bool IsIntoColumn { get; private set; }
        public Color Color { get; private set; }

        public event Action<BallView, Collider2D> TriggerEntered; 
        public event Action<BallView> Stopped; 
        
        public void Init(Vector2 ballPosition)
        {
            transform.position = ballPosition;
            _rigidbody = GetComponent<Rigidbody2D>();
            Color = _appSettings.Colors[Random.Range(0, _appSettings.Colors.Length)];
            _sprite.color = Color;
        }

        public void Destroy()
        {
            var partsView = Instantiate(_partsViewPrefab);
            partsView.Init(Color);
            partsView.transform.position = transform.position;
            Destroy(gameObject);
        }

        public void Sleep()
        {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        
        public void Unsleep()
        {
            Debug.Log($"Unsleep ball [{gameObject.name}]");
            _isRealized = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TriggerEntered?.Invoke(this, other);
            IsIntoColumn = true;
        }


        private void Update()
        {
            if (_isRealized) return;
            
            if (IsSleeping != _rigidbody.IsSleeping())
            {
                IsSleeping = _rigidbody.IsSleeping();

                if (IsSleeping)
                {
                    _isRealized = true;
                    _rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    Stopped?.Invoke(this);
                    Debug.Log("Stopped: ", this);
                }
            }
        }
    }
}