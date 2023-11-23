using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace RescueMatch.Core.Audio
{
    public abstract class MediaPlayerBase : MonoBehaviour
    {
        public string SoundRootPath = "Sounds/";

        protected readonly Dictionary<string, AudioSource> NameSoundMap = new Dictionary<string, AudioSource>();

        protected TimeSpan OffsetTime = TimeSpan.FromMilliseconds(500);

        protected readonly List<List<string>> _soundLines = new List<List<string>>();

        public event Action<bool> OnMutedChanged;

        private bool _isMuted;
        public virtual bool IsMuted 
        {
            get
            {
                return _isMuted; 
            }
            set
            {
                if (_isMuted != value)
                {
                    if (value)
                    {
                        StopAllSounds();
                    }

                    _isMuted = value;
                    if (OnMutedChanged != null) OnMutedChanged(value);
                }
            }
        }

        public abstract IEnumerator<float> Initialize();

        public virtual AudioSource LoadSound(string soundFileName)
        {
            return LoadSound(soundFileName, soundFileName);
        }

        public virtual AudioSource LoadSound(string soundFileName, string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                alias = soundFileName;
            }

            if (NameSoundMap.ContainsKey(alias))
            {
                return NameSoundMap[alias];
            }
            var path = GetSoundPath(soundFileName);
            var clip = LoadAudioClip(path);
            var audioSource = CacheAudioClip(clip, alias);

            return audioSource;
        }

        public string GetSoundPath(string soundFileName)
        {
            return SoundRootPath + soundFileName;
        }

        public virtual AudioSource LoadSound(AudioClip audioClip)
        {
            if (NameSoundMap.ContainsKey(audioClip.name))
            {
                return NameSoundMap[audioClip.name];
            }
            var audioSource = CacheAudioClip(audioClip, audioClip.name);

            return audioSource;
        }

        protected virtual AudioClip LoadAudioClip(string path)
        {
            return Resources.Load<AudioClip>(path);
        }

        protected virtual AudioSource CacheAudioClip(AudioClip audioClip, string soundFileName = "")
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            var soundName = string.IsNullOrEmpty(soundFileName) ? audioClip.name : soundFileName;

            if (NameSoundMap.ContainsKey(soundName))
            {
                NameSoundMap[soundName] = audioSource;
            }
            else
            {
                NameSoundMap.Add(soundName, audioSource);
            }
            return audioSource;
        }

        public virtual void StopSound(string soundFileName)
        {
            if (NameSoundMap.ContainsKey(soundFileName))
            {
                NameSoundMap[soundFileName].Stop();
            }
        }

        public virtual void StopSound(AudioClip audioClip)
        {
            StopSound(audioClip.name);
        }

        public void PlaySound(AudioClip audioClip)
        {
            PlaySound(audioClip, true, false);
        }

        public virtual void PlaySound(AudioClip audioClip, bool isStopAllSounds, bool isLooped)
        {
            if (!NameSoundMap.ContainsKey(audioClip.name))
            {
                LoadSound(audioClip);
            }
            PlaySound(audioClip.name, isStopAllSounds, isLooped);
        }

        public void PlaySound(string soundFileName)
        {
            PlaySound(soundFileName, true);
        }

        public void PlaySound(string soundFileName, bool isStopAllSounds)
        {
            PlaySound(soundFileName, isStopAllSounds, false);
        }

        public virtual void PlaySound(string soundFileName, bool isStopAllSounds, bool isLooped, AudioMixerGroup audioMixerGroup = null)
        {
            if (!NameSoundMap.ContainsKey(soundFileName))
            {
                LoadSound(soundFileName);
            }
            if (NameSoundMap.ContainsKey(soundFileName))
            {
                if (isStopAllSounds)
                {
                    StopAllSounds();
                }
                var audioSource = NameSoundMap[soundFileName];
                audioSource.loop = isLooped;
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                PlaySound(audioSource);
            }
        }

        protected virtual void PlaySound(AudioSource soundFileName)
        {
            if (soundFileName == null)
            {
                return;
            }
            soundFileName.time = 0f;
            InnerPlay(soundFileName);
        }

        protected virtual void InnerPlay(AudioSource soundFileName)
        {
            if (!IsMuted)
            {
                soundFileName.Play();
            }
        }

        public bool IsSoundPlaying(string soundFileName)
        {
            if (!NameSoundMap.ContainsKey(soundFileName)) return false;
            return NameSoundMap[soundFileName].isPlaying;
        }

        public List<string> PlaySoundsLine(params string[] sounds)
        {
            return PlaySoundsLine(TimeSpan.Zero, sounds);
        }

        public List<string> PlaySoundsLine(TimeSpan startTimeOffset, TimeSpan offset, params string[] sounds)
        {
            var soundLine = new List<string>(sounds);
            _soundLines.Add(soundLine);

            var timeOffset = (float) startTimeOffset.TotalSeconds;
            foreach (string soundFileName in soundLine)
            {
                AudioSource audioSource = !NameSoundMap.ContainsKey(soundFileName) ? LoadSound(soundFileName) : NameSoundMap[soundFileName];
                SoundPlayDelayed(audioSource, TimeSpan.FromSeconds(timeOffset), false);
                timeOffset += audioSource.clip.length + (float)offset.TotalSeconds;
            }
            return soundLine;
        }

        public List<string> PlaySoundsLine(TimeSpan startTimeOffset, params string[] sounds)
        {
            return PlaySoundsLine(startTimeOffset, OffsetTime, sounds);
        }

        public void StopSoundsLine(List<string> soundsLine)
        {
            if (_soundLines.Contains(soundsLine))
            {
                foreach (var sound in soundsLine)
                {
                    StopSound(sound);
                }
                _soundLines.Remove(soundsLine);
            }
        }

        public TimeSpan GetSoundDuration(string soundFileName)
        {
            if (!NameSoundMap.ContainsKey(soundFileName))
            {
                LoadSound(soundFileName);
            }
            if (NameSoundMap.ContainsKey(soundFileName))
            {
                return GetSoundDuration(NameSoundMap[soundFileName]);
            }
            return TimeSpan.Zero;
        }

        public TimeSpan GetSoundDuration(AudioSource sound)
        {
            return TimeSpan.FromSeconds(sound.clip.length);
        }

        private void SoundPlayDelayed(string soundFileName, TimeSpan delay)
        {
            SoundPlayDelayed(soundFileName, delay, true);
        }

        private void SoundPlayDelayed(string soundFileName, TimeSpan delay, bool isStopAllSounds)
        {
            if (!NameSoundMap.ContainsKey(soundFileName))
            {
                LoadSound(soundFileName);
            }
            if (NameSoundMap.ContainsKey(soundFileName))
            {
                SoundPlayDelayed(NameSoundMap[soundFileName], delay, isStopAllSounds);
            }
        }

        protected virtual void SoundPlayDelayed(AudioSource sound, TimeSpan delay, bool isStopAllSounds)
        {
            if (sound == null)
            {
                return;
            }
            if (isStopAllSounds)
            {
                StopAllSounds();
            }
            if (!IsMuted)
            {
                sound.PlayDelayed((float) delay.TotalSeconds);
            }
        }

        public void SoundPlayDelayed(string soundFileName, int milliseconds)
        {
            SoundPlayDelayed(soundFileName, milliseconds, true);
        }

        public void SoundPlayDelayed(string soundFileName, int milliseconds, bool isStopAllSounds)
        {
            SoundPlayDelayed(soundFileName, TimeSpan.FromMilliseconds(milliseconds), isStopAllSounds);
        }


        public void StopAllSounds()
        {
            foreach (var sound in NameSoundMap)
            {
                sound.Value.Stop();
            }
            _soundLines.Clear();
        }

        public void Clear()
        {
            foreach (var audioSource in NameSoundMap)
            {
                Object.Destroy(audioSource.Value);
            }
            _soundLines.Clear();
            NameSoundMap.Clear();
            Resources.UnloadUnusedAssets();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}