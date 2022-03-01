using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    [SerializeField]
    float percent = 1;
    Vector3 vector3;
    // Start is called before the first frame update
    void Start()
    {
        float width = ScreenSize.GetScreenToWorldWidth;
        vector3 = transform.localScale;
        vector3.x = 1 * width * percent;
        transform.localScale = vector3;
    }
    // Update is called once per frame
    void Update()
    {
        float width = ScreenSize.GetScreenToWorldWidth;
        vector3 = transform.localScale;
        vector3.x = 1 * width * percent;
        transform.localScale = vector3;
    }
}
