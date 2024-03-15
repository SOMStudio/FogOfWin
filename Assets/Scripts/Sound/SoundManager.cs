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
			if (PlayerPrefs.HasKey(stKey))
			{
				volume = PlayerPrefs.GetFloat(stKey);
			}
			else
			{
				volume = 0.5f;
			}

			soundObjectList = new List<SoundObject>();

			foreach (var theSound in gameSounds)
			{
				tempSoundObj = new SoundObject(theSound, theSound.name, volume);
				soundObjectList.Add(tempSoundObj);

				DontDestroyOnLoad(tempSoundObj.sourceGo);
			}
		}

		public float GetVolume()
		{
			return volume;
		}

		public void UpdateVolume()
		{
			if (soundObjectList == null)
			{
				Init();
			}

			string stKey = $"{gamePrefsName}_SFXVol";
			volume = PlayerPrefs.GetFloat(stKey);

			foreach (var sound in soundObjectList)
			{
				tempSoundObj = sound;
				tempSoundObj.source.volume = volume;
			}
		}

		public void PlaySoundByIndex(int indexSound, Vector3 playPosition)
		{
			if (indexSound > soundObjectList.Count)
			{
				indexSound = soundObjectList.Count - 1;
			}

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
		public readonly AudioSource source;
		public readonly GameObject sourceGo;
		
		private readonly Transform sourceTR;
		private readonly AudioClip clip;
		private string name;

		public SoundObject(AudioClip setClip, string setName, float setVolume)
		{
			sourceGo = new GameObject("AudioSource_" + setName);
			sourceTR = sourceGo.transform;
			source = sourceGo.AddComponent<AudioSource>();
			source.name = "AudioSource_" + setName;
			source.playOnAwake = false;
			source.clip = setClip;
			source.volume = setVolume;
			clip = setClip;
			name = setName;
		}

		public void PlaySound(Vector3 atPosition)
		{
			sourceTR.position = atPosition;
			source.PlayOneShot(clip);
		}
	}
}