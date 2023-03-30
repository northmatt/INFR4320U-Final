using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public float speed;
    public int maxHealth;
    public float lifeTime;
    public GameObject bulletExplode;
    public AudioClip ricochet;

    private Rigidbody2D rb2d;
    private AudioSource aS;
    private float health;

    private void PlayRicochetSound() {
        aS.clip = ricochet;
        aS.Play();
    }

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        aS = GetComponent<AudioSource>();

        rb2d.velocity = transform.up * speed;

        health = maxHealth;
    }

    private void Update() {
        lifeTime -= Time.deltaTime;

        //kills the bullet if it's lifetime or health reaches 0
        if (health <= 0 || lifeTime <= 0) {
            //Instantiate(bulletExplode, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate() {
        //keeps the velocity at "speed" (sometimes bouncing can change speed)
        rb2d.velocity = rb2d.velocity.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //remove one hp if the bullet hits something that isn't the player
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayRicochetSound();
        health -= 1;
    }
}
