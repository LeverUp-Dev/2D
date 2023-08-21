using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject BulletObjA;
    public GameObject BulletObjB;
    public GameObject player;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S") {
            GameObject bullet = Instantiate(BulletObjA, transform.position, Quaternion.identity);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        else if (enemyName == "L") {
            GameObject bulletR = Instantiate(BulletObjB, transform.position + Vector3.right * 0.3f, Quaternion.identity);
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            GameObject bulletL = Instantiate(BulletObjB, transform.position + Vector3.left * 0.3f, Quaternion.identity);
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
            rigidR.AddForce(dirVecR.normalized * 5, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 5, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnHit(int dmg)
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);

        if(health <= 0) {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;
            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")
            Destroy(gameObject);
        else if(collision.gameObject.tag == "PlayerBullet") {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);
            Destroy(collision.gameObject);
        }
    }
}
