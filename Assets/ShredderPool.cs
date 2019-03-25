using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShredderPool : MonoBehaviour {
    private GameController gameController;
    // Start is called before the first frame update
    void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Respawn")) {
            gameController.RecycleEnemy(other.gameObject);
        }
    }
}