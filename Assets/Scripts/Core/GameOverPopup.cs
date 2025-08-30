using Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Core
{
    public class GameOverPopup : MonoBehaviour
    {
        [Inject] private ScoreService _scoreService;
        
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _againButton;

        private void Awake()
        {
            _againButton.onClick.AddListener(OnAgainClicked);
        }

        private void OnAgainClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _scoreText.text = string.Format(_scoreText.text, _scoreService.CurrentScores);
        }
    }
}