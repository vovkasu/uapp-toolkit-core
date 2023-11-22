using UAppToolKit.Core.Application;
using UAppToolKit.Core.Pages;
using UnityEngine;

namespace UAppToolKit.Core.Sample
{
    public class MenuPageSample : PageBase
    {
        public PageBaseLink GamePlayPageLink;
        public override void OnNavigatedTo(object arg)
        {
            Debug.Log("MenuPageSample.OnNavigatedTo");
            base.OnNavigatedTo(arg);
        }

        public override void OnNavigatedToCompleted()
        {
            Debug.Log("MenuPageSample.OnNavigatedToCompleted");
            base.OnNavigatedToCompleted();
        }

        public override void OnNavigatedFrom()
        {
            Debug.Log("MenuPageSample.OnNavigatedFrom");
            base.OnNavigatedFrom();
        }

        public override void OnNavigatedFromCompleted()
        {
            Debug.Log("MenuPageSample.OnNavigatedFromCompleted");
            base.OnNavigatedFromCompleted();
        }

        public void StartGamePlay()
        {
            EntryPointBase.Current.NavigationControllerBase.NavigateTo(GamePlayPageLink);
        }
    }
}