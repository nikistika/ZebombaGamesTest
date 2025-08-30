using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Core
{
    public class BallPartsView  : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D[] _parts;
        [SerializeField] private float _lifeTime = 1.5f;
        
        private float _initLifeTime;

        public void Init(Color color)
        {
            foreach (var part in _parts)
            {
                part.GetComponent<SpriteRenderer>().color = color;
            }
        }
        
        private void Awake()
        {
            _initLifeTime = _lifeTime;
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private async UniTask StartAsync()
        {
            await UniTask.NextFrame(); //For explosion
            
            foreach (Rigidbody2D part in _parts)
            {
                part.transform.DOScale(Vector2.zero, _initLifeTime);
                part.AddExplosionForce(10f, transform.position, 2f, 1.5f, ForceMode2D.Impulse);
            }
        }

        private void Update()
        {
            _lifeTime -= Time.deltaTime;
            
            if (_lifeTime <= 0f)
                Destroy(gameObject);
        }
    }
}