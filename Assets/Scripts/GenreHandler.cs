using Client;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenreHandler : MonoBehaviour
{
    [SerializeField]
    Button[] GenreBtn;
    [SerializeField]
    InputField SearchField;

    private void Awake()
    {
        StartCoroutine(
            ClientConstants.API.Get("Library/Genre", HttpClientRequest.ConvertToResponseAction<Genres>(result =>
            {
                if (!result.IsParseSuccess)
                {
                    return;
                }
                var genres = result.Result.data;
                for (var i = 0; i < GenreBtn.Length; i++)
                {
                    var btn = GenreBtn[i];
                    if (i >= genres.Count)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.enabled = false;
                    } else
                    {
                        btn.enabled = true;
                        var genre = genres[i];
                        btn.GetComponentInChildren<Text>().text = genre;
                        btn.onClick.AddListener(() =>
                        {
                            SongHandler.RequestPath = "Library/genre/song";
                            SongHandler.Parameters = new Dictionary<string, string> {{"Name", genre}};
                            SceneManager.LoadScene("Category");
                        });
                    }
                }
            }))
            );
    }

    public void GoToCategory()
    {
        SceneManager.LoadScene("Category");
    }

    public void GoToProfile()
    {
        SceneManager.LoadScene("User");
    }

    public void SearchByName()
    {
        var name = SearchField.text;
        SongHandler.RequestPath = "Library/search";
        SongHandler.Parameters = new Dictionary<string, string> { { "searchString", name } };
        SceneManager.LoadScene("Category");
    }
}
