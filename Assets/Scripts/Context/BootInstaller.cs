using Core;
using Services;
using UnityEngine;
using Zenject;

namespace Context
{
    public class BootInstaller : MonoInstaller
    {
        [SerializeField] private AppSettings _appSettings;

        public override void InstallBindings()
        {
            Container.Bind<AppSettings>().FromInstance(_appSettings).AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().FromNew().AsSingle();
        }
    }
}