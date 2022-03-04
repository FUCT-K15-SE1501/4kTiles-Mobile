using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetInput : MonoBehaviour
{
    string name;
    string email;
    string pass;
    string cPass;



    [SerializeField]
    InputField InputFieldName;
    [SerializeField]
    InputField InputFieldEmail;
    [SerializeField]
    InputField InputFieldPass;
    [SerializeField]
    InputField InputFieldCPass;
    void Start()
    {
        if (InputFieldName != null)
        {
            InputFieldName.onEndEdit.AddListener(SubmitName);
        }
       
        if (InputFieldEmail != null)
        {
            InputFieldEmail.onEndEdit.AddListener(SubmitEmail);
        }
        if (InputFieldPass != null)
        {
            InputFieldPass.onEndEdit.AddListener(SubmitPass);
        }

        if (InputFieldCPass != null)
        {
            InputFieldCPass.onEndEdit.AddListener(SubmitCPass);
        }

    }

    private void SubmitName(string arg0)
    {
        name = arg0;
        print("name: " + name);
    }
    private void SubmitEmail(string arg0)
    {
        email = arg0;
        print("email: " + email);
    }
    private void SubmitPass(string arg0)
    {
        pass = arg0;
        print("pass: " + pass);
    }
    private void SubmitCPass(string arg0)
    {
        cPass = arg0;
        print("cPass: " + cPass);
    }
}
