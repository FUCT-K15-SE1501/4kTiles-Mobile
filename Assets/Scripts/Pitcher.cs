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

using System.Collections;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    [Tooltip("Add a sound clip of a single tone in C4 (at 440Hz)")]
    public AudioClip clip;

    // [Tooltip("Will play a chord (4 tones) of the key pressed")]
    // public bool playChord;
    // public enum Chord { Major, Minor };
    // [Tooltip("Chose which chord to play")]
    // public Chord chord;
    // [Tooltip("Plays a chord in ascending sequence")]
    // public bool arpeggiate;
    // [Tooltip("Time between tones when arpeggiating")]
    // public float arpeggioTime = 0.1f;

    public float KeyToPitch(int midiKey)
    {
        int c4Key = midiKey - 72;

        float pitch = Mathf.Pow(2, c4Key / 12f);

        return pitch;
    }

    // void Play(int midiKey)
    // {
    //     if (playChord)
    //     {
    //         PlayChord(midiKey);
    //         return;
    //     }

    //     PlayNote(midiKey, 1);
    // }

    // void PlayChord(int midiKey)
    // {
    //     if (arpeggiate)
    //     {
    //         StartCoroutine(Arpeggio(midiKey));
    //         return;
    //     }

    //     PlayNote(midiKey, 0.25f);

    //     if (chord == Chord.Major)
    //     {
    //         PlayNote(midiKey + 4, 0.25f);

    //     }
    //     else if (chord == Chord.Minor)
    //     {
    //         PlayNote(midiKey + 3, 0.25f);
    //     }

    //     PlayNote(midiKey + 7, 0.25f);
    //     PlayNote(midiKey + 12, 0.25f);
    // }

    // IEnumerator Arpeggio(int midiKey)
    // {
    //     PlayNote(midiKey, 0.8f);
    //     yield return new WaitForSeconds(arpeggioTime);

    //     if (chord == Chord.Major)
    //     {
    //         PlayNote(midiKey + 4, 0.8f);
    //         yield return new WaitForSeconds(arpeggioTime);

    //     }
    //     else if (chord == Chord.Minor)
    //     {
    //         PlayNote(midiKey + 3, 0.8f);
    //         yield return new WaitForSeconds(arpeggioTime);
    //     }

    //     PlayNote(midiKey + 7, 0.8f);
    //     yield return new WaitForSeconds(arpeggioTime);

    //     PlayNote(midiKey + 12, 0.8f);
    //     yield return new WaitForSeconds(arpeggioTime);
    // }

    void PlayNote(int midiKey, float? volume = null, float? length = null, float delay = 0)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0;
        source.volume = volume ?? 1;
        source.pitch = KeyToPitch(midiKey);
        source.PlayDelayed(delay);
        Destroy(source, length ?? source.clip.length);
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown("m"))
    //         chord = Chord.Major;

    //     if (Input.GetKeyDown("n"))
    //         chord = Chord.Minor;

    //     if (Input.GetKeyDown("c"))
    //         playChord = !playChord;

    //     if (Input.GetKeyDown("a"))
    //         arpeggiate = !arpeggiate;

    //     // Keyboard keys > QUERTY.. row for white keys, and 12345.. row for black keys

    //     if (Input.GetKeyDown("q"))
    //         Play(72);

    //     if (Input.GetKeyDown("2"))
    //         Play(73);

    //     if (Input.GetKeyDown("w"))
    //         Play(74);

    //     if (Input.GetKeyDown("3"))
    //         Play(75);

    //     if (Input.GetKeyDown("e"))
    //         Play(76);

    //     if (Input.GetKeyDown("r"))
    //         Play(77);

    //     if (Input.GetKeyDown("5"))
    //         Play(78);

    //     if (Input.GetKeyDown("t"))
    //         Play(79);

    //     if (Input.GetKeyDown("6"))
    //         Play(80);

    //     if (Input.GetKeyDown("y"))
    //         Play(81);

    //     if (Input.GetKeyDown("7"))
    //         Play(82);

    //     if (Input.GetKeyDown("u"))
    //         Play(83);

    //     if (Input.GetKeyDown("i"))
    //         Play(84);

    //     if (Input.GetKeyDown("9"))
    //         Play(85);

    //     if (Input.GetKeyDown("o"))
    //         Play(86);

    //     if (Input.GetKeyDown("0"))
    //         Play(87);

    //     if (Input.GetKeyDown("p"))
    //         Play(88);
    // }
}
