using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    public float speed = 5f; // Geschwindigkeit des Gegners
    public float distance = 10f; // Entfernung, die der Gegner laufen soll
    private float leftBound; // Linke Grenze, bis zu der der Gegner sich bewegen kann
    private float rightBound; // Rechte Grenze, bis zu der der Gegner sich bewegen kann
    private Vector2 startingPosition; // Ausgangsposition des Gegners
    private bool movingRight = true; // Gibt an, ob der Gegner sich nach rechts bewegt
    private SpriteRenderer spriteRenderer; // Referenz auf den SpriteRenderer des Gegners
    public GameObject player;
    public GameObject respawnpoint;

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

        // Wenn der Gegner eine der Grenzen erreicht hat, ändert er die Richtung und dreht den Sprite
        if (transform.position.x <= leftBound || transform.position.x >= rightBound)
        {
            movingRight = !movingRight;
            // Sprite wird umgedreht, wenn der Enemy nach links läuft
            spriteRenderer.flipX = !movingRight;
        }
    }
    // Kollisionserkennung mit dem Spieler
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Überprüfe, ob der Spieler mit dem Collider des Gegners kollidiert
        if (collision.CompareTag("Player"))
        {
            // Zerstört den Gegner
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
        // Überprüfe, ob die Kollision mit einem GameObject stattgefunden hat, das den Tag "Obstacle" hat
        if (collision.gameObject.CompareTag("Player"))
        {
            //Spawned den player an den letzten Checkpoint oder anfangs respawnpoint
            player.transform.position = respawnpoint.transform.position;
        }
        }
    }
}
