using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rigidbody;

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
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector3.down * (GameController.Instance.noteSpeed * Time.deltaTime));
        }
    }

    public void Play()
    {
        if (!GameController.Instance.GameStarted.Value || GameController.Instance.GameOver.Value) return;
        if (Visible)
        {
            if (Played) return;
            Played = true;
            GameController.Instance.Score.Value++;
            animator.Play("Played");
            // TODO: Audio Source
        }
        else
        {
            StartCoroutine(GameController.Instance.EndGame());
            animator.Play("Missed");
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played && !TouchOptional)
        {
            StartCoroutine(GameController.Instance.EndGame());
            animator.Play("Missed");
        }
    }
}
