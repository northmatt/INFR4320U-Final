using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public int playerID;
    public float movementSpeed;
    public float rotationSpeed;
    public GameObject bullet;
    public float fireRate;
    public int maxHealth;
    public GameObject explosion;
    public GameObject damageExplosion;

    private Rigidbody2D rb2d;
    private HealthBar hb;
    private Vector3 movement;
    private float lastFired = 0;
    private float health;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        hb = GetComponent<HealthBar>();

        //firerate is origonally waitTime for each bullet when you initiate the value. this makes it how many bullets can be shot per second
        fireRate = 1 / fireRate;

        health = maxHealth;

        hb.SetVars(maxHealth, health);
    }

    private void Update() {
        //movement input
        movement += transform.up * Input.GetAxis("Vertical P" + playerID) * movementSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, 0, -Input.GetAxis("Horizontal P" + playerID) * rotationSpeed * Time.deltaTime));

        //shooting input
        if (Input.GetButton("Fire P" + playerID) && lastFired + fireRate <= Time.time) {
            Instantiate(bullet, transform.position + (transform.rotation * Vector3.up * 0.7f), transform.rotation, GameController.instance.bulletsMainParent);
            lastFired = Time.time;
        }
    }

    private void FixedUpdate() {
        //applying movement force
        rb2d.velocity = movement * Time.fixedDeltaTime;
        movement = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //if a bullet colides with the tank then destroy the bullet, remove one hp, set the hb (HealthBar) variables, and destroy the tank if it is equal to or less than 0
        //if the bullet is only being destroyed then instantiate just the "damageExplosion" and if not then instantiate the eplosion before destroying the player
        if (!collision.gameObject.CompareTag("Bullet"))
            return;

        Destroy(collision.gameObject);
        health -= 1;
        hb.SetVars(maxHealth, health);

        if (health <= 0) {
            //Instantiate(explosion, transform.position + Vector3.back, Quaternion.identity);
            Destroy(gameObject);
        }
        else {
            //Instantiate(damageExplosion, transform.position + Vector3.back, Quaternion.identity);
        }
    }
}
