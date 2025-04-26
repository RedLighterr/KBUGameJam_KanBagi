using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectScript : MonoBehaviour
{
	[SerializeField] AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		DestroyOnEnd();
	}

	async void DestroyOnEnd()
	{
		audioSource.Play();
		await UniTask.WaitForSeconds(audioSource.clip.length);
		Destroy(this.gameObject);
	}
}
