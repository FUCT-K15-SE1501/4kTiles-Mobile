using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public Note notePrefab;
    public float noteSpeed = 5f;
    public GameObject noteTriggerPrefab;
    public AudioClip sampleClip;
    public int notesToSpawn = 5;

    private Dictionary<int, Models.Note> noteModelDict = NoteConverter.TestRow.ToDictionary();

    private Camera _camera;
    private GameObject noteContainer;
    private float noteHeight;
    private float noteWidth;
    private Vector3 noteLocalScale;
    private float noteSpawnStartPosX;

    private bool lastSpawn = false;
    private int lastNoteId = -1;

    public int MaxNoteId { get; private set; }
    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; }
    public ReactiveProperty<bool> GameStarted { get; set; }
    public ReactiveProperty<bool> GameOver { get; set; }
    public ReactiveProperty<int> Score { get; set; }
    public Pitcher Pitcher { get; private set; }
    public Transform LastSpawnedNote { get; private set; }

    private void Awake()
    {
        MaxNoteId = Mathf.Max(noteModelDict.Keys.ToArray());

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
        if (GameController.Instance.GameStarted.Value || !Input.GetMouseButtonDown(0)) return;
        GameController.Instance.GameStarted.Value = true;
        StartCoroutine(CheckTouch(new List<Vector2>() { Input.mousePosition }, failIfMiss: false));
    }

    // TODO: Check if this is the last note
    private IEnumerator CheckTouch(IEnumerable<Vector2> touched, bool isHold = false, bool failIfMiss = true)
    {
        foreach (var touch in touched)
        {
            var origin = _camera.ScreenToWorldPoint(touch);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (!hit) continue;
            var hitGameObject = hit.collider.gameObject;
            if (!hitGameObject.CompareTag("Note")) continue;
            var note = hitGameObject.GetComponent<Note>();
            note.PlayTouch(isHold, failIfMiss);
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
        var noteSpawnStartPosY = LastSpawnedNote.position.y + noteHeight;
        Note note = null;
        for (var i = 0; i < notesToSpawn; i++)
        {
            if (lastSpawn) break;
            for (var j = 0; j < 4; j++)
            {
                var index = ++lastNoteId;
                note = Instantiate(notePrefab, noteContainer.transform);
                note.Id = index;
                note.transform.localScale = noteLocalScale;
                note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * j, noteSpawnStartPosY);
                note.Visible = noteModelDict.ContainsKey(index);
                if (note.Visible)
                {
                    note.NoteModel = noteModelDict[index];
                }

                if (lastNoteId == MaxNoteId)
                {
                    lastSpawn = true;
                }
            }
            noteSpawnStartPosY += noteHeight;
            if (i == notesToSpawn - 1) LastSpawnedNote = note.transform;
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
