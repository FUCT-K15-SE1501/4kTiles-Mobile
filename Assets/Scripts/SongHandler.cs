using Client;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongHandler : MonoBehaviour
{

    [SerializeField]
    Text ErrorText;

    public void Load()
    {
        StartCoroutine(
        ClientConstants.API.Get("Library", HttpClientRequest.ConvertToResponseAction<SongListResponse>(result =>
        {
            if (!result.IsParseSuccess)
            {
                ErrorText.text = "Load Failed!";
                return;
            }
            if (result.Result.errorCode == -1990)
            {
                ErrorText.text = "Song does not exist!";
                return;
            }
            if (result.Result.errorCode == 0)
            {
                ErrorText.text = "No Error!";
                return;
            }
            Debug.Log(result.Result.data);
        }))
        );
    }
}
