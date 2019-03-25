using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update

    private GameController.State state;
    private Rigidbody2D rb;

    public delegate void OnDeath(GameObject o);

    public OnDeath deaths;


    [SerializeField] private float jumpForce = 2F;

    public GameController.State State {
        set => state = value;
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool IsPlaying() {
        return state == GameController.State.playing;
    }

    // Update is called once per frame
    void Update() {
        if (IsPlaying()) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//                rb.velocity = Vector2.up * jumpForce;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.CompareTag("Respawn")) {
            if (deaths != null) {
                deaths(other.gameObject);
            }
        }
    }

    
}