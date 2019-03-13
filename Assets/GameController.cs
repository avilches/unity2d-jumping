using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public RawImage background;
    public float speed = 0.2F;
    public Canvas startTitle;
    

    public enum State {
        idle,
        playing
    }

    public State state;

    // Start is called before the first frame update
    void Start() {
        state = State.idle;
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
                state = State.idle;
            }

        }
    }

    private void StartGame() {
        state = State.playing;
    }

    private void Parallax() {
        background.uvRect = new Rect(background.uvRect.x + speed * Time.deltaTime, 0f, 1f, 1f);
    }
}