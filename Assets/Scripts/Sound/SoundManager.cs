using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Sound
{
	[AddComponentMenu("Base/Sound Manager")]
	public class SoundManager : MonoSingleton<SoundManager>
	{
		[Header("Main")]
		[SerializeField] private string gamePrefsName = "DefaultGame";
		[SerializeField] protected AudioClip[] gameSounds;
		
		private List<SoundObject> soundObjectList;
		private SoundObject tempSoundObj;

		[Header("Information")]
		[SerializeField] [Range(0, 1)] private float volume = 0.5f;

		private void Start()
		{
			if (soundObjectList == null)
			{
				Init();
			}
		}

		private void Init()
		{
			DontDestroyOnLoad(gameObject);
			
			string stKey = $"{gamePrefsName}_SFXVol";
			volume = PlayerPrefs.HasKey(stKey) ? PlayerPrefs.GetFloat(stKey) : 0.5f;

			soundObjectList = new List<SoundObject>();

			foreach (var theSound in gameSounds)
			{
				tempSoundObj = new SoundObject(theSound, theSound.name, volume);
				soundObjectList.Add(tempSoundObj);

				DontDestroyOnLoad(tempSoundObj.SourceGo);
			}
		}

		public float GetVolume()
		{
			return volume;
		}

		public void UpdateVolume()
		{
			if (soundObjectList == null) Init();

			string stKey = $"{gamePrefsName}_SFXVol";
			volume = PlayerPrefs.GetFloat(stKey);

			if (soundObjectList != null)
				foreach (var sound in soundObjectList)
				{
					tempSoundObj = sound;
					tempSoundObj.Source.volume = volume;
				}
		}

		public void PlaySoundByIndex(int indexSound, Vector3 playPosition)
		{
			if (indexSound > soundObjectList.Count) indexSound = soundObjectList.Count - 1;

			tempSoundObj = soundObjectList[indexSound];
			tempSoundObj.PlaySound(playPosition);
		}
		
		public void PlaySoundByIndex(int indexSound)
		{
			PlaySoundByIndex(indexSound, Vector3.zero);
		}
	}

	public class SoundObject
	{
		public readonly AudioSource Source;
		public readonly GameObject SourceGo;
		
		private readonly Transform sourceTR;
		private readonly AudioClip clip;

		public SoundObject(AudioClip setClip, string setName, float setVolume)
		{
			SourceGo = new GameObject("AudioSource_" + setName);
			sourceTR = SourceGo.transform;
			Source = SourceGo.AddComponent<AudioSource>();
			Source.name = "AudioSource_" + setName;
			Source.playOnAwake = false;
			Source.clip = setClip;
			Source.volume = setVolume;
			clip = setClip;
		}

		public void PlaySound(Vector3 atPosition)
		{
			sourceTR.position = atPosition;
			Source.PlayOneShot(clip);
		}
	}
}