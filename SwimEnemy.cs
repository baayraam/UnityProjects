using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwimEnemy : MonoBehaviour
{
    public float speed = 5f; // Geschwindigkeit des Gegners
    public float distance = 10f; // Entfernung, die der Gegner laufen soll
    private float leftBound; // Linke Grenze, bis zu der der Gegner sich bewegen kann
    private float rightBound; // Rechte Grenze, bis zu der der Gegner sich bewegen kann
    private Vector2 startingPosition; // Ausgangsposition des Gegners
    private bool movingRight = true; // Gibt an, ob der Gegner sich nach rechts bewegt
    private SpriteRenderer spriteRenderer; // Referenz auf den SpriteRenderer des Gegners
    public GameObject player;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer-Komponente abrufen
        leftBound = transform.position.x - distance / 2f;
        rightBound = transform.position.x + distance / 2f;
        startingPosition = transform.position;
    }
    void Update()
    {
        // Bewegt den Gegner in die aktuelle Richtung
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * speed * Time.deltaTime * direction);

        // Klemmt die X-Koordinate des Gegners auf der linken und rechten Seite der Szene
        float clampedX = Mathf.Clamp(transform.position.x, leftBound, rightBound);
        transform.position = new Vector2(clampedX, transform.position.y);

        // Wenn der Gegner eine der Grenzen erreicht hat, ändert er die Richtung und dreht den sprite
        if (transform.position.x <= leftBound || transform.position.x >= rightBound)
        {
            movingRight = !movingRight;
            // Sprite wird umgedreht, wenn der Enemy nach links läuft
            spriteRenderer.flipX = !movingRight;
        }
    }

}
