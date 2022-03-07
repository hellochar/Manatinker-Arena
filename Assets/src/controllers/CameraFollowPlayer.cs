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
        var targetPosition = Player.transform.position + diffCurrent;
        // var playerVelocity = Player.GetComponent<Rigidbody2D>().velocity;
        // // predict where player will be in one second
        // targetPosition += new Vector3(playerVelocity.x, playerVelocity.y, 0) * 0.2f;

        if (!nearMode) {
            var s = new Vector2(Screen.width, Screen.height) / 2f;
            // -1 -> 1
            var mouseOffset = (Input.mousePosition.xy() - s) / s;
            targetPosition += new Vector3(mouseOffset.x, mouseOffset.y, 0) * 2f;
        }

        diffTarget = nearMode ? diffOriginal : diffClose;
        if (Vector3.Distance(diffCurrent, diffTarget) > 0.01f) {
            diffCurrent = Vector3.Lerp(diffCurrent, diffTarget, 0.2f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
        }
        var dist = Vector3.Distance(Player.transform.position.xy(), transform.position.xy());
        var lerpRate = Util.MapLinear(Mathf.Clamp(dist, 1f, 5f), 1f, 5f, 1f, 5f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpRate * Time.deltaTime);
    }
}

