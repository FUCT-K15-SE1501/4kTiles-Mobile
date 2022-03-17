using Client;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardHandler : MonoBehaviour
{
    public Text SongName;
    public Button PlayButton;

    public GameObject Top1Object;
    public GameObject Top2Object;
    public GameObject Top3Object;

    public Text Top1Name;
    public Text Top2Name;
    public Text Top3Name;

    public Text Top1Value;
    public Text Top2Value;
    public Text Top3Value;
    public static int SongId { get; set; } = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SongLoader.LoadSong(SongId, (success, response) =>
        {
            if (!success)
            {
                SceneManager.LoadScene("Category");
            }
            else
            {
                SongName.text = response.data.songName;
                PlayButton.onClick.RemoveAllListeners();
                PlayButton.onClick.AddListener(() =>
                {
                    SongLoader.CurrentSongId = SongId;
                    SceneManager.LoadScene("Play");
                });
            }
        }));
        StartCoroutine(
             ClientConstants.API.Get($"Leaderboard/Song/{SongId}",
             HttpClientRequest.ConvertToResponseAction<LeaderboardResponse>(
                 result =>
                 {
                     if (!result.IsParseSuccess)
                     {
                         Top1Object.SetActive(false);
                         Top2Object.SetActive(false);
                         Top3Object.SetActive(false);
                         return;
                     }
                     var list = result.Result.data;

                     if (list.Count > 0)
                     {
                         Top1Object.SetActive(true);
                         Top1Name.text = list[0].userName;
                         Top1Value.text = list[0].bestScore.ToString() + "pt";
                     }
                     else
                     {
                         Top1Object.SetActive(false);
                     }
                     if (list.Count > 1)
                     {
                         Top2Object.SetActive(true);
                         Top2Name.text = list[1].userName;
                         Top2Value.text = list[1].bestScore.ToString() + "pt";
                     }
                     else
                     {
                         Top2Object.SetActive(false);
                     }
                     if (list.Count > 2)
                     {
                         Top3Object.SetActive(true);
                         Top3Name.text = list[2].userName;
                         Top3Value.text = list[2].bestScore.ToString() + "pt";
                     }
                     else
                     {
                         Top3Object.SetActive(false);
                     }
                 }
               )
             )
        );
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Category");
    }
}
