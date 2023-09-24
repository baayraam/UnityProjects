using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private RespawnScript respawnRespawn; // Referenz auf das RespawnScript für Respawn-Punkte
    private RespawnScript respawnLava; // Referenz auf das RespawnScript für Lava-Respawn-Punkte
    public Sprite newSprite; // Der neue Sprite, der verwendet werden soll

    void Awake()
    {
        // Das RespawnScript-Objekt mit dem Tag "Respawn" finden und die Referenz speichern
        respawnRespawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnScript>();
        // Das RespawnScript-Objekt mit dem Tag "Lava" finden und die Referenz speichern
        respawnLava = GameObject.FindGameObjectWithTag("Lava").GetComponent<RespawnScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Diese Methode wird nicht verwendet und ist leer
    }

    // Update is called once per frame
    void Update()
    {
        // Diese Methode wird nicht verwendet und ist leer
    }

    // Wird aufgerufen, wenn ein Collider2D mit diesem Collider2D kollidiert
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Überprüfen, ob der kollidierende Collider den Tag "Player" hat
        if(other.gameObject.CompareTag("Player"))
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>(); // Den SpriteRenderer des GameObjects abrufen
            spriteRenderer.sprite = newSprite; // Den neuen Sprite setzen
            respawnRespawn.respawnpoint = this.gameObject; // Das aktuelle GameObject als Respawn-Punkt für respawnRespawn setzen
            respawnLava.respawnpoint = this.gameObject; // Das aktuelle GameObject als Respawn-Punkt für respawnLava setzen
        }
    }
}
