using Client;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool _pending = false;

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
    // Success and load Login Scene
    public void Submit()
    {
        if (_pending) return;
        _pending = true;

        var login = new Login()
        {
            email = email,
            password = password
        };

        var postData = JsonUtility.ToJson(login);
        StartCoroutine(
        ClientConstants.API.Post("Account/Login", postData, HttpClientRequest.ConvertToResponseAction<LoginResponse>(result =>
        {
            _pending = false;
            if (!result.IsParseSuccess)
            {
                ErrorText.text = "Login failed!";
                return;
            }
            if (result.Result.errorCode == -1990)
            {
                ErrorText.text = "Invalid Email or Password!";
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
            ClientConstants.API.Headers.Add("Authorization", $"Bearer {result.Result.data}");
            SceneManager.LoadScene("Category");            
        })));
    }
    // Load SignUp Scene
    public void SignUp()
    {
        SceneManager.LoadScene("SignUp");
    }
}
