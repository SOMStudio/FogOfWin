using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Music
{
	[AddComponentMenu("Base/Music Manager")]
	public class MusicManager : MonoSingleton<MusicManager> {

		[Header("Main")]
		[SerializeField] protected List<MusicClipManager> musicList;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void UpdateVolume() {
			foreach (var item in musicList) {
				item.UpdateVolume ();
			}
		}

		public void StopMusic(int indexMusic) {
			MusicClipManager temp = musicList [indexMusic];

			if (temp) temp.StopMusic();
		}

		public void PlayMusic(int indexMusic) {
			MusicClipManager temp = musicList [indexMusic];

			if (temp) temp.PlayMusic();
		}
	
		public void PlayMusicStopAnother(int indexMusic) {
			for (var i = 0; i < musicList.Count; i++)
			{
				if (i != indexMusic)
				{
					if (musicList[i].IsPlaying()) StopMusic(i);
				}
				else
				{
					PlayMusic (i);
				}
			}
		}
		
		public void PlayMenuMusic()
		{
			PlayMusicStopAnother(0);
		}
	
		public void PlayLevelMusic()
		{
			PlayMusicStopAnother(1);
		}
	}
}
