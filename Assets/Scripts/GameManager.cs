using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreTxt;
    public Image[] lifeImg;
    public Image[] boomImg;
    public GameObject gameOverSet;
    public ObjManager objManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[]{ "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        //초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;
        //리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            if(line == null)
                break;

            //리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }
        //텍스트 파일 닫기
        stringReader.Close();
        //첫번째 스폰 딜레이 적용
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > nextSpawnDelay && !spawnEnd) {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        //UI score update
        Player playerLogic = player.GetComponent<Player>();
        scoreTxt.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type) {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objManager = objManager;

        if (enemyPoint == 5 || enemyPoint == 6) { //right spawn
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8) {//left spawn
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * 1, -1);
        }
        else {//center spawn
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }
        //리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count) {
            spawnEnd = true;
            return;
        }
        //다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void UpdateLifeIcon(int life)
    {
        //UI life init disable
        for (int i = 0; i < 3; i++) {
            lifeImg[i].color = new Color(1, 1, 1, 0);
        }
        //UI life active
        for (int i = 0; i < life; i++) {
            lifeImg[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        //UI life init disable
        for (int i = 0; i < 3; i++) {
            boomImg[i].color = new Color(1, 1, 1, 0);
        }
        //UI life active
        for (int i = 0; i < boom; i++) {
            boomImg[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}