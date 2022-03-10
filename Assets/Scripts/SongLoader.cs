using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using JetBrains.Annotations;
using Models;
using UnityEngine;

public static class SongLoader
{
    public static int CurrentSongId { get; set; } = 0;
    [CanBeNull] public static Song CurrentSong { get; set; }

    public static IEnumerator LoadSong(Action<bool> onResult)
    {
        return ClientConstants.API.Get($"Song/{CurrentSongId}", HttpClientRequest.ConvertToResponseAction<SongResponse>(
            result =>
            {
                if (!result.IsParseSuccess)
                {
                    onResult.Invoke(false);
                }
                else
                {
                    try
                    {
                        var song = JsonUtility.FromJson<Song>(result.Result.data.notes);
                        CurrentSong = song;
                        onResult.Invoke(true);
                    }
                    catch
                    {
                        onResult.Invoke(false);
                    }
                }
            }));
    }
}