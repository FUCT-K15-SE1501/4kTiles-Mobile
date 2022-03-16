using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using JetBrains.Annotations;
using Models;
using UnityEngine;

public static class SongLoader
{
    private static int _currentSongId = 1;

    public static int CurrentSongId
    {
        get => _currentSongId;
        set
        {
            CurrentSong = null;
            _currentSongId = value;
        }
    }

    public static float CurrentNoteSpeed { get; set; } = 5;
    [CanBeNull] public static Song CurrentSong { get; private set; }

    public static IEnumerator LoadSong(int id, Action<bool, SongResponse> onResult)
    {
        return ClientConstants.API.Get($"Song/{id}", HttpClientRequest.ConvertToResponseAction<SongResponse>(
                result => onResult.Invoke(result.IsParseSuccess, result.Result)
            )
        );
    }

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
                        CurrentNoteSpeed = result.Result.data.bpm / 60f;
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