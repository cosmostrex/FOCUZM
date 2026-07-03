using UnityEngine;
using System.Collections;

public class TeleportArrivalGate : MonoBehaviour
{
    public GazeWallManager gazeManager;
    public ViewSpotLock gazeLock;
    public float startDelay = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gazeLock) gazeLock.Lock();
        StartCoroutine(StartGaze());
    }

    IEnumerator StartGaze()
    {
        yield return new WaitForSeconds(startDelay);
        gazeManager?.StartGazeMode();
        Debug.Log("Gate: 응시 시작");
    }
}
