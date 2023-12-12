using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip click;

    public void Click()
    {
        audioSource.PlayOneShot(click);
    }
}
