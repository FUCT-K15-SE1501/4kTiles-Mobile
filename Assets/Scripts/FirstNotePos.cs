using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstNotePos : MonoBehaviour
{
    public float offset = 0.1f;
    private void Awake()
    {
        var worldLocation = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var position = transform.position;
        position.y = worldLocation.y + offset;
        transform.position = position;
    }
}
