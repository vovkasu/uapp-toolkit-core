using UAppToolKit.Core.Application;
using UAppToolKit.Core.Pages;
using UnityEngine;

namespace UAppToolKit.Core.Sample
{
    public class GamePlayPageSample : PageBase
    {
        public override void OnNavigatedTo(object arg)
        {
            Debug.Log("GamePlayPageSample.OnNavigatedTo");
            base.OnNavigatedTo(arg);
        }

        public override void OnNavigatedToCompleted()
        {
            Debug.Log("GamePlayPageSample.OnNavigatedToCompleted");
            base.OnNavigatedToCompleted();
        }

        public override void OnNavigatedFrom()
        {
            Debug.Log("GamePlayPageSample.OnNavigatedFrom");
            base.OnNavigatedFrom();
        }

        public override void OnNavigatedFromCompleted()
        {
            Debug.Log("GamePlayPageSample.OnNavigatedFromCompleted");
            base.OnNavigatedFromCompleted();
        }

        public void BackToPrevPage()
        {
            EntryPointBase.Current.NavigationControllerBase.GoBack();
        }
    }
}