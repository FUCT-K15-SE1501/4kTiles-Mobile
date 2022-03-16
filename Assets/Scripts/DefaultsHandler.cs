using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultsHandler : MonoBehaviour
{
    public void Login()
    {
        SceneManager.LoadScene("Login");
    }
    public void Guest()
    {
        SceneManager.LoadScene("Category");
    }
}
