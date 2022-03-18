using Client;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongHandler : MonoBehaviour
{
    public static string RequestPath { get; set; } = "Library";
    public static Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

    private int _page = 1;
    private int Page
    {
        get
        {
            _page = Mathf.Max(1, _page);
            return _page;
        }
        set => _page = Mathf.Max(1, value);
    }

    private const int PageSize = 4;
    private bool _loading = false;

    //Song Bar 1
    public GameObject songBar1;
    public Text songName1;
    public Text authorName1;
    public Button leaderBoard1;
    public Button playButton1;
    //Song Bar 2
    public GameObject songBar2;
    public Text songName2;
    public Text authorName2;
    public Button leaderBoard2;
    public Button playButton2;
    //Song Bar 3
    public GameObject songBar3;
    public Text songName3;
    public Text authorName3;
    public Button leaderBoard3;
    public Button playButton3;
    //Song Bar 4
    public GameObject songBar4;
    public Text songName4;
    public Text authorName4;
    public Button leaderBoard4;
    public Button playButton4;

    public Slider noteBeatsSlider;

    public void Awake()
    {
        Load();
    }

    private void Start()
    {
        noteBeatsSlider.value = SongLoader.NotesPerBeat;
        noteBeatsSlider.onValueChanged.RemoveAllListeners();
        noteBeatsSlider.onValueChanged.AddListener(value =>
        {
            var intValue = Mathf.FloorToInt(value);
            SongLoader.NotesPerBeat = intValue;
        });
    }

    private void Load()
    {
        var finalDict = Parameters.ToDictionary(item => item.Key, item => item.Value);
        finalDict.Add("PageNumber", Page.ToString());
        finalDict.Add("PageSize", PageSize.ToString());
        _loading = true;
        StartCoroutine(
        ClientConstants.API.Get(path: RequestPath, query: finalDict, result: HttpClientRequest.ConvertToResponseAction<SongListResponse>(result =>
        {
            if (!result.IsParseSuccess)
            {
                _loading = false;
                songBar1.SetActive(false);
                songBar2.SetActive(false);
                songBar3.SetActive(false);
                songBar4.SetActive(false);
                return;
            }

            var list = result.Result.data;
            if (!list.Any())
            {
                if (Page > 1) Page -= 1;
            }
            else
            {

                //Song Bar 1
                if (list.Count >= 1)
                {
                    var songInfo = list[0];
                    songBar1.SetActive(true);

                    songName1.text = songInfo.songName;
                    authorName1.text = $"{songInfo.author} | {songInfo.bpm} BPM";

                    playButton1.onClick.RemoveAllListeners();
                    playButton1.onClick.AddListener(() =>
                    {
                        SongLoader.CurrentSongId = songInfo.songId;
                        SceneManager.LoadScene("Play");
                    });
                    leaderBoard1.onClick.RemoveAllListeners();
                    leaderBoard1.onClick.AddListener(() =>
                    {
                        LeaderboardHandler.SongId = songInfo.songId;
                        SceneManager.LoadScene("LeaderBoard");
                    });
                }
                else
                {
                    songBar1.SetActive(false);
                }

                //Song Bar 2
                if (list.Count >= 2)
                {
                    var songInfo = list[1];
                    songBar2.SetActive(true);

                    songName2.text = songInfo.songName;
                    authorName2.text = $"{songInfo.author} | {songInfo.bpm} BPM";

                    playButton2.onClick.RemoveAllListeners();
                    playButton2.onClick.AddListener(() =>
                    {
                        SongLoader.CurrentSongId = songInfo.songId;
                        SceneManager.LoadScene("Play");
                    });
                    leaderBoard2.onClick.RemoveAllListeners();
                    leaderBoard2.onClick.AddListener(() =>
                    {
                        LeaderboardHandler.SongId = songInfo.songId;
                        SceneManager.LoadScene("LeaderBoard");
                    });
                }
                else
                {
                    songBar2.SetActive(false);
                }

                //Song Bar 3
                if (list.Count >= 3)
                {
                    var songInfo = list[2];
                    songBar3.SetActive(true);

                    songName3.text = songInfo.songName;
                    authorName3.text = $"{songInfo.author} | {songInfo.bpm} BPM";

                    playButton3.onClick.RemoveAllListeners();
                    playButton3.onClick.AddListener(() =>
                    {
                        SongLoader.CurrentSongId = songInfo.songId;
                        SceneManager.LoadScene("Play");
                    });
                    leaderBoard3.onClick.RemoveAllListeners();
                    leaderBoard3.onClick.AddListener(() =>
                    {
                        LeaderboardHandler.SongId = songInfo.songId;
                        SceneManager.LoadScene("LeaderBoard");
                    });
                }
                else
                {
                    songBar3.SetActive(false);
                }

                //Song Bar 4
                if (list.Count >= 4)
                {
                    var songInfo = list[3];
                    songBar4.SetActive(true);

                    songName4.text = songInfo.songName;
                    authorName4.text =$"{songInfo.author} | {songInfo.bpm} BPM";

                    playButton4.onClick.RemoveAllListeners();
                    playButton4.onClick.AddListener(() =>
                    {
                        SongLoader.CurrentSongId = songInfo.songId;
                        SceneManager.LoadScene("Play");
                    });
                    leaderBoard4.onClick.RemoveAllListeners();
                    leaderBoard4.onClick.AddListener(() =>
                    {
                        LeaderboardHandler.SongId = songInfo.songId;
                        SceneManager.LoadScene("LeaderBoard");
                    });
                }
                else
                {
                    songBar4.SetActive(false);
                }
            }

            _loading = false;
        }))
        );
    }

    public void NextPage()
    {
        if (_loading) return;
        Page += 1;
        Load();
    }

    public void PrevPage()
    {
        if (Page == 1) return;
        if (_loading) return;
        Page -= 1;
        Load();
    }

    public void SetDefaultValue()
    {
        if (_loading) return;
        if (RequestPath == "Library" && Page == 1) return;
        RequestPath = "Library";
        Page = 1;
        Parameters = new Dictionary<string, string>();
        Load();
    }

    public void GoToMusicCategory()
    {
        SceneManager.LoadScene("MusicCategory");
    }

    public void GoToProfile()
    {
        SceneManager.LoadScene("User");
    }

    public void GoToDefault()
    {
        ClientConstants.API.Headers.Remove("Authorization");
        SceneManager.LoadScene("Defaults");
    }
}
