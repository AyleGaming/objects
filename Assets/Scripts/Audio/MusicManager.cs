using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip track1;
    [SerializeField] private AudioClip track2;

    private bool isPlayingTrack1 = true;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        PlayNextTrack();
    }

    private void Update()
    {
        // Check if the audio has finished playing
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        if (isPlayingTrack1)
        {
            audioSource.clip = track1;
        }
        else
        {
            audioSource.clip = track2;
        }

        isPlayingTrack1 = !isPlayingTrack1;
        audioSource.Play();
    }
}
