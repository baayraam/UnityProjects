using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject followObject; // Das Spielobjekt, dem die Kamera folgen soll
    public Vector2 followOffset; // Der Offset, um den die Kamera das Objekt versetzt folgen soll
    public float speed = 3f; // Die Geschwindigkeit, mit der die Kamera das Objekt verfolgt
    private Vector2 threshold; // Der Schwellenwert für den Abstand zwischen Kamera und Objekt
    private Rigidbody2D rb;

    public float smoothTime = 0.3f; // Die Dauer der sanften Kamera-Bewegung
    private Vector3 velocity = Vector3.zero; // Die Geschwindigkeit für die sanfte Kamera-Bewegung

    // Start is called before the first frame update
    void Start()
    {
        threshold = calculateThreshold(); // Berechnet den Schwellenwert für den Abstand zwischen Kamera und Objekt
        rb = followObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 follow = followObject.transform.position; // Die Position des zu folgenden Objekts
        float xDifference = Vector2.Distance(Vector2.right * transform.position.x, Vector2.right * follow.x); // Der horizontale Abstand zwischen Kamera und Objekt
        float yDifference = Vector2.Distance(Vector2.up * transform.position.y, Vector2.up * follow.y); // Der vertikale Abstand zwischen Kamera und Objekt

        Vector3 newPosition = transform.position; // Die neue Position der Kamera
        if (Mathf.Abs(xDifference) >= threshold.x) // Überprüft, ob der horizontale Abstand den Schwellenwert überschreitet
        {
            newPosition.x = follow.x; // Setzt die neue horizontale Position auf die Position des zu folgenden Objekts
        }

        if (Mathf.Abs(yDifference) >= threshold.y) // Überprüft, ob der vertikale Abstand den Schwellenwert überschreitet
        {
            newPosition.y = follow.y; // Setzt die neue vertikale Position auf die Position des zu folgenden Objekts
        }

        float moveSpeed = rb.velocity.magnitude > speed ? rb.velocity.magnitude : speed; // Die Geschwindigkeit der Kamera-Bewegung basierend auf der Geschwindigkeit des zu folgenden Objekts

        // Verwendet SmoothDamp für eine sanftere Kamera-Bewegung
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime, moveSpeed);

    }

    private Vector3 calculateThreshold()
    {
        Rect aspect = Camera.main.pixelRect;
        Vector2 t = new Vector2(Camera.main.orthographicSize * aspect.width / aspect.height, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }
}
