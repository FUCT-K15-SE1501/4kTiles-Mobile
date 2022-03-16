using Client;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongHandler : MonoBehaviour
{
    //Song Bar 1
    public GameObject songBar1;
    public Text songName1;
    public Button leaderBoard1;
    public Button playButton1;
    //Song Bar 2
    public GameObject songBar2;
    public Text songName2;
    public Button leaderBoard2;
    public Button playButton2;
    //Song Bar 3
    public GameObject songBar3;
    public Text songName3;
    public Button leaderBoard3;
    public Button playButton3;
    //Song Bar 4
    public GameObject songBar4;
    public Text songName4;
    public Button leaderBoard4;
    public Button playButton4;

    public void Awake()
    {
        StartCoroutine(
        ClientConstants.API.Get("Library", HttpClientRequest.ConvertToResponseAction<SongListResponse>(result =>
        {
            if (!result.IsParseSuccess)
            {
                return;
            }
            Debug.Log(result.Result.data);
            List<SongInfo> list = result.Result.data;
            //Song Bar 1
            if (list.Count >= 1)
            {
                var songInfo = list[0];
                songBar1.SetActive(true);

                songName1.text = songInfo.songName;
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
            } else
            {
                songBar1.SetActive(false);
            }
            //Song Bar 2
            if (list.Count >= 2)
            {
                var songInfo = list[1];
                songBar2.SetActive(true);

                songName2.text = songInfo.songName;
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
        }))
        );
    }
}
