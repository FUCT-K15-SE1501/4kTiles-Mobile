using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNotesTrigger : MonoBehaviour
{
    public float offset = 1;
    void Awake()
    {
        var halfWidth = Screen.width / 2f;
        var screenLocation = new Vector3(halfWidth, Screen.height);
        var worldLocation = Camera.main.ScreenToWorldPoint(screenLocation);
        var position = transform.position;
        position.x = worldLocation.x;
        position.y = worldLocation.y + offset;
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameController.Instance.LastSpawnedNote.gameObject)
        {
            GameController.Instance.SpawnNotes();
        }
    }
}
