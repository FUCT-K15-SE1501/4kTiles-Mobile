using Client;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    private string email;
    private string password;

    [SerializeField]
    InputField InputFieldEmail;
    [SerializeField]
    InputField InputFieldPass;
    [SerializeField]
    Text ErrorText;

    void Start()
    {
        if (InputFieldEmail != null)
        {
            InputFieldEmail.onEndEdit.AddListener(s =>
            {
                email = s;
            });
        }
        if (InputFieldPass != null)
        {
            InputFieldPass.onEndEdit.AddListener(s =>
            {
                password = s;
            });
        }
    }

    public void Submit()
    {
        var login = new Login()
        {
            email = email,
            password = password
        };

        var postData = JsonUtility.ToJson(login);
        StartCoroutine(
        ClientConstants.API.Post("Account/Login", postData, HttpClientRequest.ConvertToResponseAction<LoginResponse>(result =>
        {
            if(result.Result.errorCode == -1990)
            {
                ErrorText.text = "Email or Password does not exist!";
                return;
            }
            if (result.Result.errorCode == -1)
            {
                ErrorText.text = "Unexpected Error!";
                return;
            }
            if (result.Result.errorCode == 1)
            {
                ErrorText.text = "Login Failed!";
                return;
            }
            if (!result.IsSuccess)
            {
                ErrorText.text = "Login failed!";
                return;
            }
            Debug.Log(result.Result.data);

            ClientConstants.API.Headers.Add("Authorization", $"Bearer {result.Result.data}");
        }))
        );

    }
}
