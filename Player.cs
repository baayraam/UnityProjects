using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ParticleSystem dust; // Partikelsystem für Staubeffekte
    public ParticleSystem lava; // Partikelsystem für Lavaboden-Effekte

    // Variablen für die programmierung der horizontalen Bewegung
    [Header("Horizontal Movement")]
    public float moveSpeed = 14f; // Geschwindigkeit der horizontalen Bewegung des Spielers
    private Vector2 direction; // Richtung der horizontalen Bewegung
    private bool facingRight = true; // Gibt an, ob der Spieler nach rechts schaut

    // Variablen für die programmierung der Vertikalen Bewegung
    [Header("Vertical Movement")]
    public float jumpSpeed = 15f; // Geschwindigkeit des Sprungs
    public float jumpDelay = 0.25f; // Verzögerung zwischen aufeinanderfolgenden Sprüngen
    private float jumpTimer; // Timer für den Sprungverzögerung
    private bool isJumping = false; // Gibt an, ob der Spieler gerade springt

    // Variablen, die in Unity angehängt werden müssen
    [Header("Components")]
    public Rigidbody2D rb; // Rigidbody2D-Komponente des Spielers
    public Animator animator; // Animator-Komponente des Spielers
    public LayerMask groundLayer; // Layer-Maske für den Boden
    public GameObject characterHolder; // Das Spielobjekt, das den Spieler-Charakter hält

    // Variablen für die programmierung weiterer Physik
    [Header("Physics")]
    public float maxSpeed = 10f; // Maximale Geschwindigkeit des Spielers
    public float linearDrag = 4f; // Linearer Drag für die Verlangsamung des Spielers
    public float gravity = 1; // Gravitation des Spielers
    public float fallMultiplier = 5f; // Multiplikator für die Geschwindigkeit des Falls

    // Variablen für die Erkennung von Kollisionen(Berührungen)
    [Header("Collision")]
    public bool onGround = false; // Gibt an, ob der Spieler den Boden berührt
    public float groundLength = 0.55f; // Länge des Raycasts zum Erkennen des Bodens
    public Vector3 colliderOffsetFront; // Offset für den vorderen Collider des Spielers
    public Vector3 colliderOffsetBack; // Offset für den hinteren Collider des Spielers

    // Variablen für die Programmierung der Wand Sprünge
    [Header("Wall Jumping")]
    public float wallSlideSpeed = 1f; // Geschwindigkeit beim Rutschen an der Wand
    public float wallJumpSpeed = 7.5f; // Geschwindigkeit des Wand-Sprungs
    public float wallJumpDelay = 0.2f; // Verzögerung zwischen Wandberührung und Wand-Sprung
    private float wallJumpTimer; // Timer für den Wand-Sprungverzögerung
    public Vector2 wallJumpDirection; // Richtung des Wand-Sprungs

    [Header("Swimming")]
    public float swimSpeed = 5f; // Schwimmgeschwindigkeit des Spielers
    public bool inWater = false; // Gibt an, ob der Spieler im Wasser ist

    void Update()
    {
        bool isWallSliding = false; // Gibt an, ob der Spieler an der Wand entlang rutscht
        bool isTouchingWall =
            Physics2D.Raycast(
                transform.position,
                Vector2.right,
                colliderOffsetFront.x + 0.1f,
                groundLayer
            )
            || Physics2D.Raycast(
                transform.position,
                Vector2.left,
                colliderOffsetFront.x + 0.1f,
                groundLayer
            );

        // Überprüfen, ob der Spieler an der Wand entlang rutscht
        if (isTouchingWall && !onGround && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }

        // Überprüfen, ob der Spieler einen Wand-Sprung ausführt
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            Flip();
            wallJumpTimer = Time.time + wallJumpDelay;
            Vector2 wallDir = Physics2D.Raycast(
                transform.position,
                Vector2.right,
                colliderOffsetFront.x + 0.1f,
                groundLayer
            )
                ? Vector2.right
                : Vector2.left;
            wallJumpDirection = new Vector2(-wallDir.x, 1);
        }

        // Durchführen des Wand-Sprungs, wenn die Bedingungen erfüllt sind
        if (wallJumpTimer > Time.time && isWallSliding)
        {
            rb.velocity = new Vector2(
                wallJumpDirection.x * wallJumpSpeed,
                wallJumpDirection.y * jumpSpeed
            );
        }

        // Überprüfen, ob die Umschalttaste gedrückt wird, um die Geschwindigkeit des Spielers zu ändern
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button2))
        {
            maxSpeed = 9f;
            CreateDust();
        }
        else
        {
            maxSpeed = 7f;
        }

        bool wasOnGround = onGround;
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Überprüfen, ob der Spieler gerade den Boden berührt hat
        if (!wasOnGround && onGround)
        {
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        }
        // Überprüfen, ob der Sprungknopf gedrückt wurde
        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
            isJumping = true;
        }

        // Überprüfen, ob der Spieler den Boden berührt
        onGround =
            Physics2D.Raycast(
                transform.position + colliderOffsetFront,
                Vector2.down,
                groundLength,
                groundLayer
            )
            || Physics2D.Raycast(
                transform.position - colliderOffsetFront,
                Vector2.down,
                groundLength,
                groundLayer
            );

        // Neue Bedingung für das Schwimmen nach unten
        if (inWater && Input.GetKey(KeyCode.S))
        {
            direction.y = -1f;
        }
    }

    void FixedUpdate()
    {
        // Wenn der Spieler im Wasser ist, Schwimmfunktion aufrufen
        if (inWater)
        {
            SwimCharacter(direction.x, direction.y);
        }
        else
        {
            // Ansonsten, Funktionen für horizontale Bewegung und Physik anwenden
            moveCharacter(direction.x);
            modifyPhysics();
        }

        // Überprüfen, ob der Spieler springen soll
        if (jumpTimer > Time.time && onGround)
        {
            Jump();
            isJumping = true;
        }
        else if (!onGround && isJumping)
        {
            // Bewegung im Sprung
            Vector2 airMove = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }
    }

    // Methode, um die Spielerfigur zu drehen
    void Flip()
    {
        CreateDust();
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    // Methode, um die horizontale Bewegung des Spielers zu steuern
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);
        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));

        // Umdrehen der Spielfigur, wenn sie in die entgegengesetzte Richtung läuft
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }

        // Begrenzen der Maximalgeschwindigkeit des Spielers
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    // Methode, um die Physik des Spielers anzupassen
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            // Anwendung des linearen Abbremsens, wenn der Spieler die Richtung ändert oder langsam läuft
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }

            // Beenden des Abbremsens, wenn der Spieler fast zum Stillstand kommt
            if (Mathf.Abs(direction.x) < 0.1f && rb.velocity.magnitude < 0.1f)
            {
                rb.drag = 0f;
            }

            // Deaktivieren der Schwerkraft, wenn der Spieler auf dem Boden ist
            rb.gravityScale = 0;
        }
        else
        {
            // Anwendung des linearen Abbremsens im Sprung
            rb.drag = linearDrag * 0.15f;

            // Anwendung der Fallmultiplikator-Gravitation, wenn der Spieler nach unten fällt
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            // Verringern der Gravitation, wenn der Spieler nach oben springt und den Sprungknopf nicht gedrückt hält
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
            // Anpassung der Gravitation basierend auf dem Abstand zum Boden
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer);
                if (hit.collider != null && hit.distance < 0.1f)
                {
                    rb.gravityScale = 1;
                }
                else
                {
                    rb.gravityScale = gravity;
                }
            }
        }
    }

    // Methode zum Ausführen des Sprungs
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        jumpTimer = 0;
        isJumping = false;
        CreateDust();
    }

    // Methode zum Erzeugen von Staubeffekten
    void CreateDust()
    {
        dust.Play();
    }

    // Methode zum Steuern der Schwimmbewegung des Spielers
    void SwimCharacter(float horizontal, float vertical)
    {
        rb.velocity = new Vector2(horizontal * swimSpeed, vertical * swimSpeed);
    }

    // Methode zum Ändern der Größe des Spielers während des Sprungs
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        characterHolder.transform.localScale = new Vector3(xSqueeze, ySqueeze, 1);
        yield return new WaitForSeconds(seconds);
        characterHolder.transform.localScale = new Vector3(1, 1, 1);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;
        }
        if (other.CompareTag("Lava"))
        {
            lava.Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = false;
        }
    }
}
