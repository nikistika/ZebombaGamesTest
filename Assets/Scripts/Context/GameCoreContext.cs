using Services;
using UnityEngine;
using Zenject;

namespace Context
{
    public class GameCoreContext : MonoInstaller
    {
        [SerializeField] private GameCoreManager _gameCoreManager;
        [SerializeField] private SoundService _soundServicePrefab;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameCoreManager>().FromInstance(_gameCoreManager).AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreService>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<BallServices>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<SoundService>().FromComponentInNewPrefab(_soundServicePrefab).AsSingle().NonLazy();
        }
    }
}