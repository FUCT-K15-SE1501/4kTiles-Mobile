///
/// Changes pitch of any audio clip to match a pressed key on a (piano) keyboard
/// if a sound is in C4 (440hz) then it will perfectly match the scale
/// by Nothke
/// 
/// QWERTY.. row for white keys
/// 12345... row for black keys
/// press C to toggle chord mode in game
/// press N or M for minor or major
/// press A to toggle arpeggiate
/// 
/// Licence: CC0
///

using System;
using System.Collections;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    public AudioClip Clip { get; set; }

    public float KeyToPitch(int midiKey)
    {
        var c4Key = midiKey - 72;
        var pitch = Mathf.Pow(2, c4Key / 12f);
        return pitch;
    }

    public void PlayNote(int midiKey, float? volume = null, float? length = null, float delay = 0)
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.clip = Clip;
        source.spatialBlend = 0;
        source.volume = volume ?? 1;
        source.pitch = KeyToPitch(midiKey);
        source.PlayDelayed(delay);
        Destroy(source, length ?? source.clip.length);
    }
}
