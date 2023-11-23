using System.Collections.Generic;
using RescueMatch.Core.Audio;

namespace UAppToolKit.Core.Sample
{
    public class MediaPlayerSample : MediaPlayerBase
    {
        public override IEnumerator<float> Initialize()
        {
            yield return 1f;
        }
    }
}