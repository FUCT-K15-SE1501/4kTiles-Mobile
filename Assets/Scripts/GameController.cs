using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public Note notePrefab;
    public AudioSource audioSource;
    public float noteSpeed = 5f;
    public GameObject noteTriggerPrefab;

    public Transform LastSpawnedNote { get; private set; }
    private GameObject noteContainer;
    private static float noteHeight;
    private static float noteWidth;
    private Vector3 noteLocalScale;
    private float noteSpawnStartPosX;
    public const int NotesToSpawn = 20;
    private int prevRandomIndex = -1;
    private int lastNoteId = 1;
    private Coroutine playSongSegmentCoroutine;
    private float songSegmentLength = 0.8f;
    private bool lastNote = false;
    private bool lastSpawn = false;

    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; } = false;
    public int LastPlayedNoteId { get; set; } = 0;
    public ReactiveProperty<bool> GameStarted { get; set; }
    public ReactiveProperty<bool> GameOver { get; set; }
    public ReactiveProperty<int> Score { get; set; }

    private void Awake()
    {
        Instance = this;
        GameStarted = new ReactiveProperty<bool>();
        GameOver = new ReactiveProperty<bool>();
        Score = new ReactiveProperty<int>();
        ShowGameOverScreen = new ReactiveProperty<bool>();
        noteContainer = new GameObject("NoteContainer");

        var destroyNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        destroyNoteTrigger.name = "DestroyNoteTrigger";
        destroyNoteTrigger.AddComponent<DestroyNoteTrigger>();
        var spawnNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        spawnNoteTrigger.name = "SpawnNoteTrigger";
        spawnNoteTrigger.AddComponent<SpawnNotesTrigger>();
        var outOfScreenTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        outOfScreenTrigger.name = "OutOfScreenTrigger";
        outOfScreenTrigger.AddComponent<OutOfScreenTrigger>();

        LastSpawnedNote = new GameObject("LastSpawnedNote").transform;
        var worldSpawnLocation = Camera.main.ScreenToWorldPoint(Vector3.zero);
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

    private void DetectNoteClicks()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Began) continue;
            var origin = Camera.main.ScreenToWorldPoint(touch.position);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (!hit) continue;
            var hitGameObject = hit.collider.gameObject;
            if (!hitGameObject.CompareTag("Note")) continue;
            var note = hitGameObject.GetComponent<Note>();
            note.Play();
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

    public void SpawnNotes()
    {
        if (lastSpawn) return;

        var noteSpawnStartPosY = LastSpawnedNote.position.y + noteHeight;
        Note note = null;
        var timeTillEnd = audioSource.clip.length - audioSource.time;
        int notesToSpawn = NotesToSpawn;
        if (timeTillEnd < NotesToSpawn)
        {
            notesToSpawn = Mathf.CeilToInt(timeTillEnd);
            lastSpawn = true;
        }
        for (int i = 0; i < notesToSpawn; i++)
        {
            var randomIndex = GetRandomIndex();
            for (int j = 0; j < 4; j++)
            {
                note = Instantiate(notePrefab, noteContainer.transform);
                note.transform.localScale = noteLocalScale;
                note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * j, noteSpawnStartPosY);
                note.Visible = (j == randomIndex);
                if (note.Visible)
                {
                    note.Id = lastNoteId;
                    lastNoteId++;
                }
            }
            noteSpawnStartPosY += noteHeight;
            if (i == NotesToSpawn - 1) LastSpawnedNote = note.transform;
        }
    }

    private int GetRandomIndex()
    {
        var randomIndex = Random.Range(0, 4);
        while (randomIndex == prevRandomIndex) randomIndex = Random.Range(0, 4);
        prevRandomIndex = randomIndex;
        return randomIndex;
    }

    public void PlaySomeOfSong()
    {
        if (!audioSource.isPlaying && !lastNote)
        {
            audioSource.Play();
        }
        if (audioSource.clip.length - audioSource.time <= songSegmentLength)
        {
            lastNote = true;
        }
        if (playSongSegmentCoroutine != null) StopCoroutine(playSongSegmentCoroutine);
        playSongSegmentCoroutine = StartCoroutine(PlaySomeOfSongCoroutine());
    }

    private IEnumerator PlaySomeOfSongCoroutine()
    {
        yield return new WaitForSeconds(songSegmentLength);
        audioSource.Pause();
        if (lastNote)
        {
            PlayerWon = true;
            StartCoroutine(EndGame());
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
