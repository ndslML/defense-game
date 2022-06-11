using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SE;
    [SerializeField] AudioClip[] buttonClips;
    [SerializeField] AudioClip[] shotBoom;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }
        Destroy(gameObject);
    }

    public void ButtonSound(int i)
    {
        SE.PlayOneShot(buttonClips[i]);
    }
    public void ShotBoomSound(int i)
    {
        SE.PlayOneShot(shotBoom[i]);
    }
    public void ShotBoomSoundLow(int i)
    {
        SE.PlayOneShot(shotBoom[i],0.7f);
    }
}
