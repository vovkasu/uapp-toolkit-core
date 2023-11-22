using UAppToolKit.Core.Application;
using UAppToolKit.Core.Options;
using UnityEngine;

namespace UAppToolKit.Core.Sample
{
    public class ApplicationSample : EntryPointBase
    {
        public OptionsProviderSample OptionsProviderSample;

        public override OptionsProviderBase GetOptionsProviderBase()
        {
            if (OptionsProviderSample == null)
            {
                OptionsProviderSample = new OptionsProviderSample();
            }
            return OptionsProviderSample;
        }

        public override void SetStartPage(GameObject startPage)
        {
            throw new System.NotImplementedException();
        }
    }
}
