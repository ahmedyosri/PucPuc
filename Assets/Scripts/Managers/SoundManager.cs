using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource collisionSound;
    public AudioSource perfectSound;
    public AudioSource explosionSound;
    public AudioSource fireballSound;
    public AudioSource gamestartSound;

    public static SoundManager Instance;

    public void OnEnable()
    {
        if(Instance == null)
            Instance = this;
    }
}
