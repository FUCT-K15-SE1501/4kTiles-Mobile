using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public Note notePrefab;
    public float noteSpeed = 5f;
    public GameObject noteTriggerPrefab;
    public AudioClip sampleClip;

    public Transform LastSpawnedNote { get; private set; }
    private GameObject noteContainer;
    private static float noteHeight;
    private static float noteWidth;
    private Vector3 noteLocalScale;
    private float noteSpawnStartPosX;
    public const int NotesToSpawn = 20;
    private bool lastNote = false;
    private bool lastSpawn = false;
    private Camera _camera;

    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; } = false;
    public ReactiveProperty<bool> GameStarted { get; set; }
    public ReactiveProperty<bool> GameOver { get; set; }
    public ReactiveProperty<int> Score { get; set; }
    public Pitcher Pitcher { get; set; }

    private void Awake()
    {
        Instance = this;
        GameStarted = new ReactiveProperty<bool>();
        GameOver = new ReactiveProperty<bool>();
        Score = new ReactiveProperty<int>();
        ShowGameOverScreen = new ReactiveProperty<bool>();
        noteContainer = new GameObject("NoteContainer");
        _camera = Camera.main!;

        var destroyNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        destroyNoteTrigger.name = "DestroyNoteTrigger";
        destroyNoteTrigger.AddComponent<DestroyNoteTrigger>();
        var spawnNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        spawnNoteTrigger.name = "SpawnNoteTrigger";
        spawnNoteTrigger.AddComponent<SpawnNotesTrigger>();
        var outOfScreenTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        outOfScreenTrigger.name = "OutOfScreenTrigger";
        outOfScreenTrigger.AddComponent<OutOfScreenTrigger>();

        var pitchSoundContainer = new GameObject("SoundContainer");
        Pitcher = pitchSoundContainer.AddComponent<Pitcher>();
        Pitcher.Clip = sampleClip;

        LastSpawnedNote = new GameObject("LastSpawnedNote").transform;
        var worldSpawnLocation = _camera.ScreenToWorldPoint(Vector3.zero);
        worldSpawnLocation.x = 0;
        worldSpawnLocation.y += 1f;
        worldSpawnLocation.z = 0;
        LastSpawnedNote.position = worldSpawnLocation;
    }

    void Start()
    {
        SetDataForNoteGeneration();
        SpawnNotes();
    }

    private void Update()
    {
        DetectNoteClicks();
        DetectStart();
    }

    private void DetectStart()
    {
        if (!GameController.Instance.GameStarted.Value && Input.GetMouseButtonDown(0))
        {
            GameController.Instance.GameStarted.Value = true;
        }
    }

    // TODO: Check if this is the last note
    private IEnumerator CheckTouch(IEnumerable<Vector2> touched, bool isHold = false)
    {
        foreach (var touch in touched)
        {
            var origin = _camera.ScreenToWorldPoint(touch);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (!hit) continue;
            var hitGameObject = hit.collider.gameObject;
            if (!hitGameObject.CompareTag("Note")) continue;
            var note = hitGameObject.GetComponent<Note>();
            note.PlayTouch(isHold);
            yield return null;
        }
    }

    private void DetectNoteClicks()
    {
        var touched = new List<Vector2>();
        var hold = new List<Vector2>();
        foreach (var touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touched.Add(touch.position);
                    break;
                case TouchPhase.Stationary:
                    hold.Add(touch.position);
                    break;
                default:
                    break;
            }
        }
        
        if (!touched.Any() && Input.GetMouseButtonDown(0))
        {
            touched.Add(Input.mousePosition);
        }

        if (touched.Any())
        {
            StartCoroutine(CheckTouch(touched, false));
        }

        if (hold.Any())
        {
            StartCoroutine(CheckTouch(hold, true));
        }
    }


    private void SetDataForNoteGeneration()
    {
        var topRight = new Vector3(Screen.width, Screen.height, 0);
        var topRightWorldPoint = Camera.main.ScreenToWorldPoint(topRight);
        var bottomLeftWorldPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var screenWidth = topRightWorldPoint.x - bottomLeftWorldPoint.x;
        var screenHeight = topRightWorldPoint.y - bottomLeftWorldPoint.y;
        noteHeight = screenHeight / 4;
        noteWidth = screenWidth / 4;
        var noteSpriteRenderer = notePrefab.GetComponent<SpriteRenderer>();
        noteLocalScale = new Vector3(
               noteWidth / noteSpriteRenderer.bounds.size.x * noteSpriteRenderer.transform.localScale.x,
               noteHeight / noteSpriteRenderer.bounds.size.y * noteSpriteRenderer.transform.localScale.y, 1);
        var leftmostPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2));
        var leftmostPointPivot = leftmostPoint.x + noteWidth / 2;
        noteSpawnStartPosX = leftmostPointPivot;
    }

    // TODO: Check if this is the last spawn of notes and assign to lastSpawn
    public void SpawnNotes()
    {
        if (lastSpawn) return;

        var noteSpawnStartPosY = LastSpawnedNote.position.y + noteHeight;
        Note note = null;
        for (var i = 0; i < NotesToSpawn; i++)
        {
            var randomIndex = Random.Range(0, 4);
            for (var j = 0; j < 4; j++)
            {
                note = Instantiate(notePrefab, noteContainer.transform);
                note.transform.localScale = noteLocalScale;
                note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * j, noteSpawnStartPosY);
                note.Visible = j == randomIndex;
                if (note.Visible)
                {
                    note.TouchOptional = Random.Range(0, 2) == 1;
                    note.MidiKey = Random.Range(72, 89);
                }
            }
            noteSpawnStartPosY += noteHeight;
            if (i == NotesToSpawn - 1) LastSpawnedNote = note.transform;
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator EndGame()
    {
        GameOver.Value = true;
        yield return new WaitForSeconds(1);
        ShowGameOverScreen.Value = true;
    }
}
