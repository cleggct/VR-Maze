using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollider : MonoBehaviour {

    private float duration = 1f;

	// Use this for initialization
	void Start () {
        SteamVR_Fade.Start(Color.black, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        SteamVR_Fade.Start(Color.black, duration);
    }

    private void OnTriggerExit(Collider other)
    {
        SteamVR_Fade.Start(Color.clear, duration);
    }
}
