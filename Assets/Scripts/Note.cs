using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private Sprite normalNoteSprite;
    [SerializeField] private Sprite optionalNoteSprite;

    private Animator animator;
    private new SpriteRenderer renderer;

    private bool visible;
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            var color = renderer.color;
            color.a = visible ? 1 : 0;
            renderer.color = color;
        }
    }

    private bool touchOptional = false;

    public bool TouchOptional
    {
        get => touchOptional;
        set
        {
            touchOptional = value;
            renderer.sprite = touchOptional ? optionalNoteSprite : normalNoteSprite;
        }
    }

    public bool Played { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector3.down * (GameController.Instance.noteSpeed * Time.deltaTime));
        }
    }

    private void Play()
    {
        if (Played) return;
        Played = true;
        GameController.Instance.Score.Value++;
        var color = renderer.color;
        color.a = 0.5f;
        renderer.color = color;
        // TODO: Audio Source
    }

    private void Miss()
    {
        StartCoroutine(GameController.Instance.EndGame());
        animator.Play("Missed");
        renderer.color = Color.red;
    }

    public void PlayTouch(bool isHold)
    {
        if (!GameController.Instance.GameStarted.Value || GameController.Instance.GameOver.Value) return;
        if (TouchOptional)
        {
            if (Visible) Play();
        }
        else if (!isHold)
        {
            if (Visible) Play();
            else Miss();
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played && !TouchOptional) Miss();
    }
}
