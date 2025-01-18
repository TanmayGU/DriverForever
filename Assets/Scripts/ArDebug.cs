using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARDebug : MonoBehaviour
{
    public ARSession arSession;

    void Start()
    {
        if (arSession == null)
        {
            Debug.LogError("AR Session is missing!");
            return;
        }

        Debug.Log("AR Session initialized: " + (ARSession.state == ARSessionState.Ready));
    }
}

