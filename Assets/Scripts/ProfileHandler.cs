using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Client;
using Assets.Scripts.Models;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ProfileHandler : MonoBehaviour
{
    [SerializeField]
    Text userName;
    [SerializeField]
    RawImage userAvatar;

    [SerializeField] private Toggle blackBackgroundToggle;
    [SerializeField] private Toggle autoPlayToggle;
    [SerializeField] private Slider touchOffsetSlider;

    private void Awake()
    {
        StartCoroutine(
            ClientConstants.API.Get("account/account", HttpClientRequest.ConvertToResponseAction<ProfileResponse>(result =>
            {
                if (!result.IsSuccess)
                {
                    userName.text = "Guest";
                    return;
                }
                userName.text = result.Result.data.userName;
                Dictionary<string, string> parameter=new Dictionary<string, string>();
                parameter.Add("radius", "50");
                StartCoroutine(
                    ClientConstants.AvatarAPI.GetTexture($"big-ears-neutral/{result.Result.data.userName}.png", request =>
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(request);
                        userAvatar.texture = texture;
                    },parameter)
                    );
            })
            ));
        blackBackgroundToggle.isOn = GameController.BlackBackground;
        blackBackgroundToggle.onValueChanged.AddListener(toggle => GameController.BlackBackground = toggle);
        autoPlayToggle.isOn = GameController.AutoPlay;
        autoPlayToggle.onValueChanged.AddListener(toggle => GameController.AutoPlay = toggle);
        touchOffsetSlider.value = GameController.TouchOffset;
        touchOffsetSlider.onValueChanged.AddListener(value => GameController.TouchOffset = value);
    }
    public void GoToMusicCategory()
    {
        SceneManager.LoadScene("MusicCategory");
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Category");
    }

    public void GoToDefault()
    {
        ClientConstants.API.Headers.Remove("Authorization");
        SceneManager.LoadScene("Defaults");
    }
}
