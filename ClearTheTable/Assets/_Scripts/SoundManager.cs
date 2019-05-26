using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    [SerializeField]
    AudioSource audioSource;

    // Card sounds
    [SerializeField]
    AudioClip CardShuffle;
    [SerializeField]
    AudioClip CardSelect;
    [SerializeField]
    AudioClip CardDeselect;

    public void PlayClip(int i)
    {
        switch (i)
        {
            case 0:
                audioSource.clip = CardShuffle;
                audioSource.Play();
                break;
            case 1:
                audioSource.clip = CardSelect;
                audioSource.Play();
                break;
            case 2:
                audioSource.clip = CardDeselect;
                audioSource.Play();
                break;
        }
    }

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }
}
