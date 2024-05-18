using System;
using UnityEngine;

namespace Music
{
	[AddComponentMenu("Base/Music Clip")]
	public class MusicClipManager : MonoBehaviour {

		[Header("Main")]
		[SerializeField] private string gamePrefsName= "DefaultGame";
		[SerializeField] private AudioClip music;
		[SerializeField] private bool loopMusic;
		[SerializeField] private float fadeTime = 5f;
	
		private AudioSource source;
		private GameObject sourceGo;

		private float volumePrefs;
		private float targetVolume;

		private bool playClip;

		[Header("Start game")]
		public bool playAtStart;

		[Header("Information")]
		[SerializeField] [Range(0, 1)]  private float volume;
		[SerializeField] [Range(0, 1)]  private float defaultVolume;

		private void Awake ()
		{
			string stKey = $"{gamePrefsName}_MusicVol";
			volumePrefs = PlayerPrefs.HasKey (stKey) ? PlayerPrefs.GetFloat (stKey) : defaultVolume;
			
			sourceGo = new GameObject ("Music_" + music.name);
			source = sourceGo.AddComponent<AudioSource> ();
			source.name = "Music_" + music.name;
			source.playOnAwake = playAtStart;
			source.clip = music;
			source.volume = volume;
			DontDestroyOnLoad (sourceGo);
			
			playClip = playAtStart;
			
			FadeIn();
		}

		private void Update ()
		{
			if (playClip)
			{
				if (loopMusic)
				{
					if (!source.isPlaying) source.Play();
				}
				else
				{
					playClip = false;
				}
			}
			
			if (fadeTime > 0.0f)
			{
				if (Math.Abs(volume - targetVolume) > 0.01f)
				{
					volume = Mathf.Lerp(volume, targetVolume, Time.deltaTime * fadeTime);
					source.volume = volume;
				}
			}
		}
	
		public void UpdateVolume () {
			if (source)
			{
				volumePrefs = PlayerPrefs.GetFloat ($"{gamePrefsName}_MusicVol");
				
				volume = source.volume;
				targetVolume = volumePrefs;
			}
		}

		public void PlayMusic()
		{
			if (!playClip)
			{
				playClip = true;
				
				FadeIn();
			}
		}

		public void StopMusic()
		{
			if (playClip)
			{
				playClip = false;
				
				FadeOut();
			}
		}

		private void FadeIn ()
		{
			volume = fadeTime > 0.0f ? 0.0f : volumePrefs;

			targetVolume = volumePrefs;
			source.volume = volume;
		}

		private void FadeOut ()
		{
			volume = fadeTime > 0.0f ? source.volume : 0.0f;

			targetVolume = 0.0f;
			source.volume = volume;
		}

		public bool IsPlaying () {
			return source.isPlaying;
		}
	}
}
