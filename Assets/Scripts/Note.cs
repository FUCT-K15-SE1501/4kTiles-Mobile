using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private Sprite normalNoteSprite;
    [SerializeField] private Sprite normalFailNoteSprite;
    [SerializeField] private Sprite normalPlayNoteSprite;
    [SerializeField] private Sprite optionalNoteSprite;
    [SerializeField] private Sprite optionalPlayNoteSprite;

    private new SpriteRenderer renderer;
    private bool _visible;
    private Models.Note _noteModel;

    public int Id { get; set; }
    public Models.Note NoteModel
    {
        get => _noteModel;
        set
        {
            _noteModel = value;
            renderer.sprite = _noteModel.TouchOptional ? optionalNoteSprite : normalNoteSprite;
        }
    }
    public bool Visible
    {
        get => _visible;
        set
        {
            _visible = value;
            var color = renderer.color;
            color.a = _visible ? 1 : 0;
            if (!_visible)
            {
                renderer.sprite = normalFailNoteSprite;
            }
            renderer.color = color;
        }
    }
    public bool TouchOptional => _noteModel?.TouchOptional ?? false;
    public bool Played { get; set; }

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector3.down * (GameController.Instance.noteSpeed * Time.deltaTime));
        }
    }

    private void OnDestroy()
    {
        if ((!Played && !TouchOptional) || GameController.Instance.MaxNoteId != Id) return;
        GameController.Instance.PlayerWon = true;
        GameController.Instance.GameOver.Value = true;
        GameController.Instance.ShowGameOverScreen.Value = true;
    }

    private void Play()
    {
        if (Played) return;
        Played = true;

        GameController.Instance.Score.Value++;
        renderer.sprite = TouchOptional ? optionalPlayNoteSprite : normalPlayNoteSprite;

        if (NoteModel.NoteType == null) return;
        foreach (var noteType in NoteModel.NoteType)
        {
            GameController.Instance.Pitcher.PlayNote(noteType.MidiKey, noteType.Volume, noteType.Length, noteType.Delay);
        }
    }

    private void Miss()
    {
        StartCoroutine(GameController.Instance.EndGame());
        GameController.Instance.Pitcher.PlayNote(60);
        renderer.sprite = normalFailNoteSprite;
        renderer.color = Color.white;
    }

    public void PlayTouch(bool isHold, bool failIfMiss)
    {
        if (!GameController.Instance.GameStarted.Value || GameController.Instance.GameOver.Value) return;
        if (TouchOptional)
        {
            if (Visible) Play();
        }
        else if (!isHold)
        {
            if (Visible) Play();
            else if (failIfMiss) Miss();
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played && !TouchOptional) Miss();
    }
}
