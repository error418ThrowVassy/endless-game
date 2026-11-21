using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] AudioClip music;
    [SerializeField] AudioClip jumpClip;
    [SerializeField] AudioClip coinClip;
    [SerializeField] AudioClip deathClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (music != null)
        {
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public static void PlayJump() => PlaySfx(Instance?.jumpClip);
    public static void PlayCoin() => PlaySfx(Instance?.coinClip);
    public static void PlayDeath() => PlaySfx(Instance?.deathClip);

    static void PlaySfx(AudioClip clip)
    {
        if (Instance == null || clip == null) return;
        Instance.sfxSource.PlayOneShot(clip);
    }
}