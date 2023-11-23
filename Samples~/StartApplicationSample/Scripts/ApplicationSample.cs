using UAppToolKit.Core.Application;
using UAppToolKit.Core.Options;
using UAppToolKit.Core.Pages;
using UnityEngine;

namespace UAppToolKit.Core.Sample
{
    public class ApplicationSample : EntryPointBase
    {
        public static ApplicationSample Instance;
        
        public OptionsProviderSample OptionsProviderSample;

        [HideInInspector]
        public MediaPlayerSample MediaPlayer;

        [HideInInspector] 
        public NavigationController NavigationController;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            GetOptionsProviderBase();

            MediaPlayer = MediaPlayerBase as MediaPlayerSample;
            NavigationController = NavigationControllerBase as NavigationController;
        }

        public override OptionsProviderBase GetOptionsProviderBase()
        {
            if (OptionsProviderSample == null)
            {
                OptionsProviderSample = new OptionsProviderSample();
            }
            return OptionsProviderSample;
        }
    }
}
