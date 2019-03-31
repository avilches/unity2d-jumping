using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update

    private GameController.State state;
    private Rigidbody2D rb;

    public float speed = 2.5F;
    public float gravityFalling = 2.5F;
    public float gravityOnReleaseJumping = 2F;
    public float gravity = 3F;
    private float timeJumping = 0;
    public float maxTimeJumping = 0.3F;

    private TextMeshProUGUI velText;
    private TextMeshProUGUI graText;


    public delegate void OnDeath(GameObject o);

    public OnDeath deaths;


    [SerializeField] private float jumpForce = 2F;

    public GameController.State State {
        set => state = value;
    }

    void Start() {
        velText = GameObject.Find("Velocity").GetComponent<TextMeshProUGUI>();
        graText = GameObject.Find("Gravity").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
    }


    private bool IsPlaying() {
        return state == GameController.State.playing;
    }

    private bool jumpRequest = false;

    // Update is called once per frame
    void Update() {
        if (IsPlaying()) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (isGround) {
                    jumps = 2;
                    jumpRequest = true;
                } else if (--jumps > 0) {
                    jumpRequest = true;
                }
            }
        }

        velText.text = "" + rb.velocity.y;
        graText.text = "" + rb.gravityScale;
    }

    [SerializeField]
    private bool isGround = false;

    private int jumps = 2;


    void OnCollisionExit2D(Collision2D other) {
        if (isGround && other.gameObject.CompareTag("ground")) {
            Debug.Log("Exist grounded");
            isGround = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.CompareTag("Respawn")) {
            if (deaths != null) {
                deaths(other.gameObject);
            }
        } else if (!isGround && other.gameObject.CompareTag("ground")) {
            Debug.Log("enter grounded");
            isGround = true;
            jumpRequest = false;
            jumps = 2;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        if (!isGround && other.gameObject.CompareTag("ground")) {
            Debug.Log("stay grounded");
            isGround = true;
            jumpRequest = false;
        }
    }

    void FixedUpdate() {
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        if (right ^ left) {
            float move = right ? 1 : -1;
            rb.velocity = new Vector2(move * speed, rb.velocity.y);
        } else {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        if (jumpRequest) {
            timeJumping = maxTimeJumping;
            rb.gravityScale = gravity;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRequest = false;
            isGround = false;
        }


        if (rb.velocity.y > 0) { // going up
            timeJumping -= Time.fixedDeltaTime;
            if (timeJumping > 0 && Input.GetKey(KeyCode.Space)) {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
//                rb.gravityScale = gravity;
            } else {
//                rb.gravityScale = gravityOnReleaseJumping;
            }
        } else { // going down
//            rb.gravityScale = gravityFalling;
        }
//        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravity);
    }
}