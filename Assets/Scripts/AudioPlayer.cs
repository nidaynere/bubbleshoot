using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    private void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}