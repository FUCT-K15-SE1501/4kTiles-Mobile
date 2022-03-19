using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client;
using Models;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public static bool BlackBackground { get; set; } = false;
    public static bool AutoPlay { get; set; } = false;

    public Text tapToStartText;
    public Note notePrefab;
    public float noteSpeed = 5f;
    public GameObject noteTriggerPrefab;
    public AudioClip sampleClip;
    public int notesToSpawn = 5;

    private Dictionary<int, Models.Note> noteModelDict;
    private bool loadSuccess = false;
    private bool spawnStarted = false;
    
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
        if (SongLoader.CurrentSong == null)
        {
            tapToStartText.text = "LOADING";
            StartCoroutine(SongLoader.LoadSong(success =>
            {
                loadSuccess = success;
                if (!success)
                {
                    SceneManager.LoadScene("Category");
                    tapToStartText.text = "FAILED TO LOAD";
                    GameOver.Value = true;
                    ShowGameOverScreen.Value = true;
                }
                else
                {
                    tapToStartText.text = "TAP TO START";
                }
            }));
        }
        else
        {
            loadSuccess = true;
            tapToStartText.text = "TAP TO START";
        }

        Instance = this;
        GameStarted = new ReactiveProperty<bool>();
        GameOver = new ReactiveProperty<bool>();
        Score = new ReactiveProperty<int>();
        ShowGameOverScreen = new ReactiveProperty<bool>();
        noteContainer = new GameObject("NoteContainer");
        _camera = Camera.main!;
        if (BlackBackground)
        {
            _camera.backgroundColor = Color.black;
        }

        var destroyNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        destroyNoteTrigger.name = "DestroyNoteTrigger";
        destroyNoteTrigger.AddComponent<DestroyNoteTrigger>();
        var spawnNoteTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        spawnNoteTrigger.name = "SpawnNoteTrigger";
        spawnNoteTrigger.AddComponent<SpawnNotesTrigger>();
        var outOfScreenTrigger = Instantiate<GameObject>(noteTriggerPrefab);
        outOfScreenTrigger.name = "OutOfScreenTrigger";
        outOfScreenTrigger.AddComponent<OutOfScreenTrigger>();
        if (AutoPlay)
        {
            var autoPlayTrigger = Instantiate<GameObject>(noteTriggerPrefab);
            autoPlayTrigger.name = "AutoPlayTrigger";
            autoPlayTrigger.AddComponent<AutoPlayTrigger>();
        }

        var pitchSoundContainer = new GameObject("SoundContainer");
        Pitcher = pitchSoundContainer.AddComponent<Pitcher>();
        Pitcher.Clip = sampleClip;

        LastSpawnedNote = new GameObject("LastSpawnedNote").transform;
        var worldSpawnLocation = _camera.ScreenToWorldPoint(Vector3.zero);
        worldSpawnLocation.x = 0;
        worldSpawnLocation.y += 2.5f;
        worldSpawnLocation.z = 0;
        LastSpawnedNote.position = worldSpawnLocation;
        ShowGameOverScreen.Subscribe(_ => UploadBestScore());
    }

    void Start()
    {
        //fps
        QualitySettings.vSyncCount = 1;
        SetDataForNoteGeneration();
    }

    private void Update()
    {
        if (!loadSuccess) return;
        if (spawnStarted)
        {
            DetectNoteClicks();
        }
        else
        {
            noteModelDict = SongLoader.CurrentSong.ToDictionary();
            MaxNoteId = Mathf.Max(noteModelDict.Keys.ToArray());
            noteSpeed = SongLoader.CurrentNoteSpeed;
            spawnStarted = true;
            SpawnNotes();
        }
    }

    private void CheckTouch(IEnumerable<Vector2> touched, bool isHold = false, bool failIfMiss = true)
    {
        foreach (var touch in touched)
        {
            var origin = _camera.ScreenToWorldPoint(touch);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (!hit) continue;
            var hitGameObject = hit.collider.gameObject;
            StartCoroutine(TouchNote(hitGameObject, isHold, failIfMiss));
        }
    }

    private static IEnumerator TouchNote(GameObject hitGameObject, bool isHold = false, bool failIfMiss = true)
    {
        if (!hitGameObject.CompareTag("Note")) yield break;
        var note = hitGameObject.GetComponent<Note>();
        note.PlayTouch(isHold, failIfMiss);
        yield return null;
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

        var failIfMiss = true;
        if ((touched.Any() || hold.Any()) && !GameController.Instance.GameStarted.Value)
        {
            GameController.Instance.GameStarted.Value = true;
            failIfMiss = false;
        }

        if (AutoPlay) return;

        if (touched.Any())
        {
            CheckTouch(touched, false, failIfMiss);
        }

        if (hold.Any())
        {
            CheckTouch(hold, true, failIfMiss);
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

    public void UploadBestScore()
    {
        if (!AutoPlay)
        {
            StartCoroutine(
                ClientConstants.API.Put($"Leaderboard/User?songId={SongLoader.CurrentSongId}&score={Score.Value}", "{}",
                    r => { })
            );
        }
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene("Category");
    }
    
}
