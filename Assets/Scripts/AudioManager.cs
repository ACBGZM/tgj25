using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource ambienceSource;
    public AudioSource npcBgmSource;
    public AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip cafeAmbience;
    public AudioClip buttonClickClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayAmbience();
    }

    public void PlayAmbience()
    {
        ambienceSource.clip = cafeAmbience;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void PlayNPCBGM(AudioClip npcBGM)
    {
        npcBgmSource.clip = npcBGM;
        npcBgmSource.loop = true;
        npcBgmSource.Play();
    }

    public void StopNPCBGM()
    {
        npcBgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClickClip()
    {
        sfxSource.PlayOneShot(buttonClickClip);
    }
}