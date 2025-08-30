using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootEntryPoints : MonoBehaviour
{
    [SerializeField] private RectTransform _part1;
    [SerializeField] private RectTransform _part2;
    [SerializeField] private RectTransform _part3;
    
    private Vector2 _targetPos1;
    private Vector2 _targetPos2;
    private Vector2 _targetPos3;

    private void Start()
    {
        _targetPos1 = _part1.anchoredPosition;
        _part1.anchoredPosition = _targetPos1 + new Vector2(-Screen.width, 0);
        _targetPos2 = _part2.anchoredPosition;
        _part2.anchoredPosition = _targetPos2 + new Vector2(Screen.width, 0);
        _targetPos3 = _part3.anchoredPosition;
        _part3.anchoredPosition = _targetPos3 + new Vector2(-Screen.width, 0);
        PlayIntroAsync().Forget();
    }

    private async UniTask PlayIntroAsync()
    {
        await _part1.DOAnchorPos(_targetPos1, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        await _part2.DOAnchorPos(_targetPos2, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        await _part3.DOAnchorPos(_targetPos3, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        SceneManager.LoadScene(SceneNames.GameScene);
    }
}
