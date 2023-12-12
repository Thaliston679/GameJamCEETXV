using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    float camDutch;
    float camZoom;
    float hMove;
    float vMove;

    void Update()
    {
        float tempo = Time.time;

        camDutch = 6 * Mathf.Sin(0.5f * tempo);
        camZoom = 1.675f + 0.145f * Mathf.Sin(1 * tempo);
        vMove = 2.35f + 0.125f * Mathf.Sin(1 * tempo);
        hMove = 0.25f * Mathf.Sin(0.25f * tempo);

        transform.position = new(hMove, vMove, 0);
        transform.rotation = Quaternion.Euler(0, 0, camDutch);
        Camera.main.orthographicSize = camZoom;
    }
}
