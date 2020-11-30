using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public RawImage background;
    public GameObject ground;
    public GameObject ground2;
    public float speed = 0.2F;
    public Canvas startTitle;
    private PlayerController player;
    [SerializeField] private GameObject enemy;
    [SerializeField] float minSpawn;
    [SerializeField] float maxSpawn;
    [SerializeField] private bool spawner = true;

    private List<GameObject> pool = new List<GameObject>();

    public enum State {
        idle,
        playing
    }

    public State state;

    // Start is called before the first frame update
    void Start() {
        state = State.idle;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.deaths += OnDeath;
        startTitle.gameObject.SetActive(true);

        for (int i = 0; i < 2; i++) {
            pool.Add(CreateEnemy());
        }
    }

    // Update is called once per frame
    void Update() {
        if (state == State.idle) {

            if (Input.GetKey(KeyCode.Space)) {
                StartGame();
            }
        } else {
            Parallax();

            if (Input.GetKey(KeyCode.Escape)) {
                StopGame();
            }
        }
    }

    private void Spawner() {
        if (IsPlaying() && spawner) {
            GameObject enemyI = GetEnemy();
            enemyI.SetActive(true);
            enemyI.transform.position = new Vector3(10f, -3.57F, 0F);
            enemyI.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            enemyI.GetComponent<Rigidbody2D>().velocity = Vector2.left * 7f;
            float nextShootTime = Random.Range(minSpawn, maxSpawn);
            Invoke("Spawner", nextShootTime);
        }
    }

    private GameObject GetEnemy() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                return pool[i];
            }
        }
        Debug.Log("OPS");

        var e = CreateEnemy();
        pool.Add(e);
        return e;
    }

    private int created = 0;
    private int outs = 0;
    private GameObject CreateEnemy() {
        Debug.Log("Create enemy");
        var e = Instantiate(enemy, transform.position, Quaternion.identity);
        e.transform.parent = gameObject.transform;
        e.SetActive(false);
        return e;
    }

    public void RecycleEnemy(GameObject o) {
        o.SetActive(false);
    }

    private void OnDeath(GameObject culprit) {
        StopGame();
        foreach (Transform t in transform) {
            if (t.gameObject == culprit) {
                t.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                t.GetComponent<Rigidbody2D>().velocity = new Vector2(12F, 2F);
                StartCoroutine(DestroyEnemyForever(t.gameObject));
            } else {
                RecycleEnemy(t.gameObject);
            }
        }
    }

    private IEnumerator DestroyEnemyForever(GameObject tGameObject) {
        yield return new WaitForSeconds(1F);
        RecycleEnemy(tGameObject);
    }

    private bool IsPlaying() {
        return state == State.playing;
    }

    private void StopGame() {
        state = State.idle;
        startTitle.gameObject.SetActive(true);
        //ground.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //ground2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.StopGame();
    }

    private void StartGame() {
        state = State.playing;
        startTitle.gameObject.SetActive(false);
        //ground.GetComponent<Rigidbody2D>().velocity = new Vector2(-4F, 0);
        //ground2.GetComponent<Rigidbody2D>().velocity = new Vector2(-4F, 0);

        player.StartGame();
        Spawner();

    }

    private void Parallax() {
        var move = speed * Time.deltaTime;
        background.uvRect = new Rect(background.uvRect.x + move, 0f, 1f, 1f);

//        var p = ground.transform.position;
//        ground.transform.position = new Vector3(p.x - 2F* Time.deltaTime, p.y, p.z);
//        ground.transform.position += new Vector3(- 2F* Time.deltaTime, 0, 0);
//        ground.transform.position = new Vector3(p.x - 2F* Time.deltaTime, p.y, p.z);

        if (spawner) {
            repositionIfOut(ground);
            repositionIfOut(ground2);
        }
    }

    private void repositionIfOut(GameObject objectWithCollider) {
        float groundLength = objectWithCollider.GetComponent<BoxCollider2D>().size.x;
        if (objectWithCollider.transform.position.x < -groundLength) {
            objectWithCollider.transform.position += new Vector3(groundLength * 2f, 0, 0);
        }
    }
}