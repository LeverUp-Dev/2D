using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;
    public float speed;
    public int maxPower;
    public int power;
    public int maxBoom;
    public int boom;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject boomEffect;

    public GameManager manager;
    public ObjManager objManager;
    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;
    public bool isRespawnTime;

    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;

        if (isRespawnTime) {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

            for (int i = 0; i < followers.Length; i++) {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else {
            spriteRenderer.color = new Color(1, 1, 1, 1);

            for (int i = 0; i < followers.Length; i++) {
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    public void JoyPanel(int type)
    {
        for (int i = 0;i < 9; i++) {
            joyControl[i] = i == type;
        }
    }

    public void joyDown()
    {
        isControl = true;
    }

    public void joyUp()
    {
        isControl = false;
    }

    void Move()
    {
        //keyboard control value
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //joy control value
        if (joyControl[0]) { h = -1; v = 1; }
        if (joyControl[1]) { h = 0; v = 1; }
        if (joyControl[2]) { h = 1; v = 1; }
        if (joyControl[3]) { h = -1; v = 0; }
        if (joyControl[4]) { h = 0; v = 0; }
        if (joyControl[5]) { h = 1; v = 0; }
        if (joyControl[6]) { h = -1; v = -1; }
        if (joyControl[7]) { h = 0; v = -1; }
        if (joyControl[8]) { h = 1; v = -1; }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)
            h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0).normalized * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")) {
            anim.SetInteger("input", (int)h);
        }
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }

    void Fire()
    {
        //if (!Input.GetButton("Fire1"))
        //    return;

        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power) {
            case 1:
                GameObject bullet = objManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.15f;
                GameObject bulletL = objManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.15f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletC = objManager.MakeObj("BulletPlayerB");
                bulletC.transform.position = transform.position;
                GameObject bulletLL = objManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidC = bulletC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        //if (!Input.GetButton("Fire2"))
        //    return;

        if (!isButtonB)
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        manager.UpdateBoomIcon(boom);
        //effect visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);
        //remove enemy
        GameObject[] enemiesL = objManager.GetPool("EnemyL");
        GameObject[] enemiesM = objManager.GetPool("EnemyM");
        GameObject[] enemiesS = objManager.GetPool("EnemyS");

        for (int i = 0; i < enemiesL.Length; i++) {
            if (enemiesL[i].activeSelf) {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesM.Length; i++) {
            if (enemiesM[i].activeSelf) {
                Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int i = 0; i < enemiesS.Length; i++) {
            if (enemiesS[i].activeSelf) {
                Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        //remove enemy bullet
        GameObject[] bulletsA = objManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objManager.GetPool("BulletEnemyB");
        GameObject[] bulletsC = objManager.GetPool("BulletBossA");
        GameObject[] bulletsD = objManager.GetPool("BulletBossB");
        for (int i = 0; i < bulletsA.Length; i++) {
            if (bulletsA[i].activeSelf)
                bulletsA[i].SetActive(false);
        }
        for (int i = 0; i < bulletsB.Length; i++) {
            if (bulletsB[i].activeSelf)
                bulletsB[i].SetActive(false);
        }
        for (int i = 0; i < bulletsC.Length; i++) {
            if (bulletsC[i].activeSelf)
                bulletsC[i].SetActive(false);
        }
        for (int i = 0; i < bulletsD.Length; i++) {
            if (bulletsD[i].activeSelf)
                bulletsD[i].SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet") {

            if (isRespawnTime)
                return;

            if (isHit)
                return;

            isHit = true;
            life--;
            manager.UpdateLifeIcon(life);
            manager.CallExplosion(transform.position, "P");

            if (life == 0) {
                manager.GameOver();
            }
            else {
                manager.RespawnPlayer();
            }

            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Item") {
            Item item = collision.gameObject.GetComponent<Item>();

            switch (item.type) {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else {
                        boom++;
                        manager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        if (power == 5)
            followers[1].SetActive(true);
        if (power == 6)
            followers[2].SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border") {
            switch (collision.gameObject.name) {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
