using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syn.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioInputStream : MonoBehaviour
    {
        AudioSource aud;
        AudioClip orgClip;

        public bool UseAudioClip;
        public bool UseMic;

        // Use this for initialization
        void Start()
        {
            aud = GetComponent<AudioSource>();
            orgClip = aud.clip;
            SetupMicInput();
        }

        // Update is called once per frame
        void Update()
        {
            if (UseAudioClip)
            {
                UseAudioClip = false;
                aud.clip = orgClip;
                aud.Play();
            }

            if (UseMic)
            {
                UseMic = false;
                SetupMicInput();
            }

        }

        void SetupMicInput()
        {
            foreach (string device in Microphone.devices)
            {
                Debug.Log("Name: " + device);
            }
            aud.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
            aud.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { }
            aud.Play();
        }
    }
}
