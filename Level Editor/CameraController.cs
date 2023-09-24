using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Geschwindigkeit der Kamerabewegung
    public float zoomSpeed = 5f; // Geschwindigkeit der Zoomfunktion
    public float minZoomDistance = 5f; // Minimale Zoomdistanz
    public float maxZoomDistance = 20f; // Maximale Zoomdistanz
    public float edgeScrollThreshold = 20f; // Schwellenwert für den Bildschirmrand-Scroll

    private Camera cam; // Referenz auf die Kamera-Komponente
    private float initialZoomDistance; // Ursprüngliche Zoomdistanz
    private Vector3 initialPosition; // Ursprüngliche Kameraposition

    private void Start()
    {
        cam = GetComponent<Camera>(); // Zugriff auf die Kamera-Komponente des GameObjects
        initialZoomDistance = cam.orthographicSize; // Speichern der ursprünglichen Zoomdistanz
        initialPosition = transform.position; // Speichern der ursprünglichen Kameraposition
    }

    private void Update()
    {
        HandleCameraMovement(); // Kamerabewegung behandeln
        HandleZoom(); // Zoomfunktion behandeln

        if (Input.GetKeyDown(KeyCode.R))  // Wenn die R-Taste gedrückt wurde
        {
            ResetCamera(); // Kamera zurücksetzen
        }
    }

    private void HandleCameraMovement()
    {
        float mouseX = Input.mousePosition.x; // X-Koordinate der Mausposition
        float mouseY = Input.mousePosition.y; // Y-Koordinate der Mausposition

        float horizontalMovement = 0f; // Horizontale Bewegung
        float verticalMovement = 0f; // Vertikale Bewegung
        
        // Überprüfen, ob die Mausposition den Schwellenwert für den Bildschirmrand-Scroll überschreitet
        if (mouseX < edgeScrollThreshold)
        {
            horizontalMovement = -1f; // Negative horizontale Bewegung
        }
        else if (mouseX > Screen.width - edgeScrollThreshold)
        {
            horizontalMovement = 1f; // Positive horizontale Bewegung
        }

        if (mouseY < edgeScrollThreshold)
        {
            verticalMovement = -1f; // Negative vertikale Bewegung
        }
        else if (mouseY > Screen.height - edgeScrollThreshold)
        {
            verticalMovement = 1f; // Positive vertikale Bewegung
        }

        // Erzeugen eines Vektors für die Bewegung basierend auf horizontaler und vertikaler Bewegung
        Vector3 move = new Vector3(horizontalMovement, verticalMovement, 0f) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World); // Kamera entsprechend verschieben
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Scrollwert der Maus
        float zoomAmount = scroll * zoomSpeed; // Zoommenge basierend auf dem Scrollwert

        float newZoomDistance = cam.orthographicSize - zoomAmount; // Neue Zoomdistanz berechnen
        newZoomDistance = Mathf.Clamp(newZoomDistance, minZoomDistance, maxZoomDistance); // Zoomdistanz innerhalb des gültigen Bereichs einschränken
        cam.orthographicSize = newZoomDistance; // Zoomdistanz der Kamera setzen
    }

    public void ResetCamera()
    {
        transform.position = initialPosition; // Kameraposition zurücksetzen
        cam.orthographicSize = initialZoomDistance; // Zoomdistanz zurücksetzen
    }
}
