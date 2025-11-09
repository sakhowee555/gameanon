using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;
    public AudioSource sfx;
    public AudioSource bgm;
    public AudioClip click, correct, wrong;

    void Awake() { if (I != null && I != this) { Destroy(gameObject); return; } I = this; DontDestroyOnLoad(gameObject); }

    public void PlaySfx(AudioClip c) { if (c) sfx.PlayOneShot(c); }
    public void PlayCorrect() { if (correct) sfx.PlayOneShot(correct); }
    public void PlayWrong() { if (wrong) sfx.PlayOneShot(wrong); }
}
