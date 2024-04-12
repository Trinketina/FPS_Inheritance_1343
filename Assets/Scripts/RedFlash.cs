using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedFlash : MonoBehaviour
{
    [SerializeField] Image redScreen;

    Color red;
    Color trans;

    float time = -1f;

    // Start is called before the first frame update
    void Start()
    {
        red = redScreen.color;
        red.a = .5f;

        trans = redScreen.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (time >= -1f)
        {
            redScreen.color = Color.Lerp(trans, red, time);
            time -= Time.deltaTime;
        }

    }

    public void Flash()
    {
        redScreen.color = red;

        time = 1;
    }
    
}
