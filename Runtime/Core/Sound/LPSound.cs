using System;
using UnityEngine;

namespace LazyPanClean {
    public class LPSound : LPSingletonMonoBehaviour<LPSound> {
        public GameObject SoundPlay(string soundSign, Vector3 targetPos, bool loop, float destroyDelay) {
            GameObject soundGo = new GameObject(soundSign);
            AudioSource source = soundGo.AddComponent<AudioSource>();
            soundGo.transform.position = targetPos;
            source.clip = LPLoader.LoadAsset<AudioClip>(AssetType.SOUND, soundSign);
            source.loop = loop;
            source.Play();
            if (Math.Abs(destroyDelay - (-1)) > 0.001f) {
                Destroy(soundGo, destroyDelay);
            }
            return soundGo;
        }

        public void SoundRecycle(GameObject soundGo) {
            Destroy(soundGo);
        }
    }
}