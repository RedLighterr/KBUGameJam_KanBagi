using Cysharp.Threading.Tasks;
using UnityEngine;

public class SoundEffectScript : MonoBehaviour
{
	[SerializeField] AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.Play();
		DestroyOnEnd();
	}

	async void DestroyOnEnd()
	{
		await UniTask.WaitForSeconds(audioSource.clip.length);

		try
		{
			if (this.gameObject != null)
			{
				Destroy(this.gameObject);
			}
		}
		catch (System.Exception)
		{

		}
	}
}
