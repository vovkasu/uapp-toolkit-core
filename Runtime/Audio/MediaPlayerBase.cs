using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RescueMatch.Core.Audio
{
    public abstract class MediaPlayerBase
    {
        protected readonly Dictionary<string, AudioSource> NameSoundMap = new Dictionary<string, AudioSource>();

        protected TimeSpan OffsetTime = TimeSpan.FromMilliseconds(500);
        protected readonly GameObject _root;
        protected float DefaultVolume;

        private bool _isMuted;
        public virtual bool IsMuted 
        {
            get
            {
                return _isMuted; 
            }
            set
            {
                if (value)
                {
                    StopAllSounds();
                }
                _isMuted = value;
            }
        }
        
        protected readonly List<List<string>> _soundLines = new List<List<string>>();
        protected readonly string _soundsPath;


        protected MediaPlayerBase(GameObject root, string soundsPath, float defaultVolume)
        {
            _root = root;
            _soundsPath = soundsPath;
            DefaultVolume = defaultVolume;
        }


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
            TuneAudioSource(audioSource, DefaultVolume);

            return audioSource;
        }

        public string GetSoundPath(string soundFileName)
        {
            return _soundsPath + soundFileName;
        }

        public virtual AudioSource LoadSound(AudioClip audioClip)
        {
            if (NameSoundMap.ContainsKey(audioClip.name))
            {
                return NameSoundMap[audioClip.name];
            }
            var audioSource = CacheAudioClip(audioClip, audioClip.name);
            TuneAudioSource(audioSource);

            return audioSource;
        }

        protected virtual AudioClip LoadAudioClip(string path)
        {
            return Resources.Load<AudioClip>(path);
        }

        protected virtual AudioSource CacheAudioClip(AudioClip audioClip, string soundFileName = "")
        {
            var audioSource = _root.AddComponent<AudioSource>();
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

        protected virtual void TuneAudioSource(AudioSource audioSource, float volume = 1, bool playOnAwake = false, bool isLooped = false, float panLevel = 0)
        {
            audioSource.volume = volume;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = isLooped;
#if UNITY_5 || UNITY_5_3_OR_NEWER
            audioSource.spatialBlend = panLevel;
#else
            audioSource.panLevel = panLevel;
#endif
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

        public virtual void PlaySound(string soundFileName, bool isStopAllSounds, bool isLooped)
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
                DeferredSoundPlay(audioSource, TimeSpan.FromSeconds(timeOffset), false);
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

        private void DeferredSoundPlay(string soundFileName, TimeSpan timeSpan)
        {
            DeferredSoundPlay(soundFileName, timeSpan, true);
        }

        private void DeferredSoundPlay(string soundFileName, TimeSpan timeSpan, bool isStopAllSounds)
        {
            if (!NameSoundMap.ContainsKey(soundFileName))
            {
                LoadSound(soundFileName);
            }
            if (NameSoundMap.ContainsKey(soundFileName))
            {
                DeferredSoundPlay(NameSoundMap[soundFileName], timeSpan, isStopAllSounds);
            }
        }

        protected virtual void DeferredSoundPlay(AudioSource sound, TimeSpan timeSpan, bool isStopAllSounds)
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
                sound.PlayDelayed((float) timeSpan.TotalSeconds);
            }
        }

        public void DeferredSoundPlay(string soundFileName, int milliseconds)
        {
            DeferredSoundPlay(soundFileName, milliseconds, true);
        }

        public void DeferredSoundPlay(string soundFileName, int milliseconds, bool isStopAllSounds)
        {
            DeferredSoundPlay(soundFileName, TimeSpan.FromMilliseconds(milliseconds), isStopAllSounds);
        }


        public void StopAllSounds()
        {
            foreach (var sound in NameSoundMap)
            {
                sound.Value.Stop();
            }
            _soundLines.Clear();
        }
        
        [Obsolete("Obsolete: use Clear")]
        public void Finalize()
        {
            Clear();
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
    }
}