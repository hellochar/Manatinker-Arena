using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject Player;
    Vector3 diffOriginal, diffClose;
    Vector3 diffTarget;
    Vector3 diffCurrent;
    public bool nearMode = false;
    // Start is called before the first frame update
    void Start()
    {
        diffOriginal = diffCurrent = diffTarget = transform.position - Player.transform.position;
        diffClose = diffOriginal / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            nearMode = !nearMode;
        }
        diffTarget = nearMode ? diffOriginal : diffClose;
        if (Vector3.Distance(diffCurrent, diffTarget) > 0.01f) {
            diffCurrent = Vector3.Lerp(diffCurrent, diffTarget, 0.2f);
            transform.position = Vector3.Lerp(transform.position, Player.transform.position + diffCurrent, 0.2f);
        }
        var dist = Vector3.Distance(Player.transform.position.xy(), transform.position.xy());
        var lerpRate = Util.MapLinear(Mathf.Clamp(dist, 1f, 5f), 1f, 5f, 1f, 5f);
        transform.position = Vector3.Lerp(transform.position, Player.transform.position + diffCurrent, lerpRate * Time.deltaTime);
    }
}

