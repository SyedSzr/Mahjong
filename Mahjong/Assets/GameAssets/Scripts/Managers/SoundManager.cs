using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Settings;
using Game.Components;

namespace Game.Managers
{
    using UnityEngine;

    public class SoundManager : MonoBehaviour
    {

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        public List<AudioSource> SfxSources;

        [Header("Settings")]
        public bool isBgmOn = true;
        public bool isSfxOn = true;
        public bool isVibrationOn = true;

        private void Start()
        {
            PlayBGM(DependencyManager.Instance.GameManager.GetClipByID("Bg"));
        }

        #region Background Music
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (bgmSource == null) return;

            if (isBgmOn && clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.Play();
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null)
                bgmSource.Stop();
        }

        public void ToggleBGM(bool enable)
        {
            isBgmOn = enable;
            if (!isBgmOn)
                StopBGM();
            else if (bgmSource.clip != null)
                PlayBGM(bgmSource.clip, bgmSource.loop);
        }
        #endregion

        #region Sound Effects
        public int sfxIndex = 0;

        public void PlaySFX(AudioClip clip)
        {
            if (!isSfxOn || clip == null) return;
            SfxSources[sfxIndex].Stop();
            SfxSources[sfxIndex].PlayOneShot(clip);
            sfxIndex = (sfxIndex + 1) % SfxSources.Count;
        }

        public void ToggleSFX(bool enable)
        {
            isSfxOn = enable;
        }
        #endregion

        #region Vibration
        public void Vibrate()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (isVibrationOn)
                Handheld.Vibrate();
#endif
        }

        public void ToggleVibration(bool enable)
        {
            isVibrationOn = enable;
        }
        #endregion
    }

}