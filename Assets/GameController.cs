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
    private GameObject player;


    public enum State {
        idle,
        playing
    }

    public State state;

    // Start is called before the first frame update
    void Start() {
        state = State.idle;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update() {
        if (state == State.idle) {
            startTitle.gameObject.SetActive(true);

            if (Input.GetKey(KeyCode.Space)) {
                StartGame();
            }
        } else {
            startTitle.gameObject.SetActive(false);
            Parallax();

            if (Input.GetKey(KeyCode.Escape)) {
                StopGame();
            }
        }
    }

    private void StopGame() {
        state = State.idle;
        ground.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ground2.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Animator>().Play("Player_idle");
    }

    private void StartGame() {
        state = State.playing;
        ground.GetComponent<Rigidbody2D>().velocity = new Vector2(-4F, 0);
        ground2.GetComponent<Rigidbody2D>().velocity = new Vector2(-4F, 0);
        player.GetComponent<Animator>().Play("Player_run");

    }

    private void Parallax() {
        var move = speed * Time.deltaTime;
        background.uvRect = new Rect(background.uvRect.x + move, 0f, 1f, 1f);

        var p = ground.transform.position;
//        ground.transform.position = new Vector3(p.x - 2F* Time.deltaTime, p.y, p.z);
//        ground.transform.position += new Vector3(- 2F* Time.deltaTime, 0, 0);
//        ground.transform.position = new Vector3(p.x - 2F* Time.deltaTime, p.y, p.z);

        repositionIfOut(ground);
        repositionIfOut(ground2);
    }

    private void repositionIfOut(GameObject objectWithCollider) {
        float groundLength = objectWithCollider.GetComponent<BoxCollider2D>().size.x;
        if (objectWithCollider.transform.position.x < -groundLength) {
            objectWithCollider.transform.position += new Vector3(groundLength * 2f, 0, 0);
        }
    }
}