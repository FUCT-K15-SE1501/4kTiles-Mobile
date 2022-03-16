using Client;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenreHandler : MonoBehaviour
{
    [SerializeField]
    Button[] GenreBtn;

    void Awake()
    {
        StartCoroutine(
            ClientConstants.API.Get("Library/Genre", HttpClientRequest.ConvertToResponseAction<Genres>(result =>
            {
                if (!result.IsParseSuccess)
                {
                    return;
                }
                var genres = result.Result.data;
                for (int i = 0; i < GenreBtn.Length; i++)
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
                            Debug.Log(genre);
                        });
                    }
                }
            }))
            );
    }
}
