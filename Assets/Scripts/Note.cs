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

    public bool Played { get; set; }
    public int Id { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector2.down * (GameController.Instance.noteSpeed * Time.deltaTime));
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
            GameController.Instance.PlaySomeOfSong();
            animator.Play("Played");
        }
        else
        {
            StartCoroutine(GameController.Instance.EndGame());
            animator.Play("Missed");
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played)
        {
            StartCoroutine(GameController.Instance.EndGame());
            animator.Play("Missed");
        }
    }
}
