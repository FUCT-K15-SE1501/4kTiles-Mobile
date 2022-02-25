using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Animator animator;

    private bool visible;
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (!visible) animator.Play("Invisible");
        }
    }

    private bool touchOptional = false;

    public bool TouchOptional
    {
        get => touchOptional;
        set
        {
            touchOptional = value;
            if (touchOptional) animator.Play("TouchOptional");
        }
    }

    public bool Played { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        animator.Play("Played");
        // TODO: Audio Source
    }

    private void Miss()
    {
        StartCoroutine(GameController.Instance.EndGame());
        animator.Play("Missed");
    }

    public void PlayTouch(bool isHold)
    {
        if (!GameController.Instance.GameStarted.Value || GameController.Instance.GameOver.Value) return;
        if (isHold && TouchOptional)
        {
            if (Visible) Play();
        }
        else
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
