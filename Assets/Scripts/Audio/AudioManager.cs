using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    public AudioSource musicSource; // Dùng để phát nhạc nền
    public AudioSource sfxSource;   // Dùng để phát tiếng súng, nổ...

    [Header("Music Clips")]
    public AudioClip menuTheme;     // Nhạc menu
    public AudioClip battleTheme;   // Nhạc lúc bắn nhau bình thường
    public AudioClip bossTheme;     // Nhạc trùm cuối (Căng thẳng!)
    public AudioClip victoryTheme;  // Nhạc thắng trận

    private void Awake()
    {
        // Singleton: Đảm bảo chỉ có 1 AudioManager duy nhất tồn tại
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // KHÔNG HỦY khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject); // Nếu lỡ tạo ra cái thứ 2 thì hủy nó đi
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        // Nếu nhạc này đang phát rồi thì thôi, không phát lại (tránh bị reset bài hát)
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot cho phép phát chồng nhiều tiếng (ví dụ tiếng súng bắn liên thanh)
        sfxSource.PlayOneShot(clip);
    }

    // Các hàm tiện ích để gọi cho nhanh
    public void PlayMenuMusic() => PlayMusic(menuTheme);
    public void PlayBattleMusic() => PlayMusic(battleTheme);
    public void PlayBossMusic() => PlayMusic(bossTheme);
    public void PlayVictoryMusic() => PlayMusic(victoryTheme);
}