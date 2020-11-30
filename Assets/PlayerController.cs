using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour {
    // Start is called before the first frame update

    [Header("Horizontal move: config")]
    [SerializeField] public float force = 2.5F;
    [SerializeField] public float maxSpeed = 7f;
    [SerializeField] public float drag = 4f;
    [Header("Horizontal move: state")]
    public float currentDrag = 0;
    public Vector2 velocity = Vector2.zero;
    public Vector2 direction = Vector2.zero;

    [Header("Vertical move: config")]
    [SerializeField] public float jumpForce = 2F;
    public float gravityFalling = 2.5F;
    public float gravityOnReleaseJumping = 2F;
    public float gravity = 3F;
    public float maxTimeJumping = 0.3F;
    public Vector3 colliderOffset;
    public float groundLength = 0.5f;

    [Header("Vertical move: state")]
    public bool isGround = false;
    public float timeJumping = 0;
    public int jumps = 2;


    private bool jumpRequest = false;

    [Header("Components")]
    public LayerMask groundLayer;
    public Rigidbody2D rb;
    public GameController gameController;
    public delegate void OnDeath(GameObject o);
    public OnDeath deaths;

    void Start() {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
    }


    private bool IsPlaying() {
        return gameController.state == GameController.State.playing;
    }

    private void OnDrawGizmos() {
        Gizmos.color = isGround ? Color.red : Color.blue;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    // Update is called once per frame
    void Update() {
        if (!IsPlaying()) return;

        // digital (-1,0,1)
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        if (right ^ left) {
            direction = right ? Vector2.right : Vector2.left;
        } else {
            direction = Vector2.zero;
        }
        // analogic
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool wasOnGround = isGround;
        isGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        if (isGround) {
            jumps = 2;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isGround) {
                jumpRequest = true;
                Debug.Log("Jump request ground");
            } else if (--jumps > 0) {
                Debug.Log("Jump request "+jumps);
                jumpRequest = true;
            }
        }
    }


    void OnCollisionExit2D(Collision2D other) {
        // if (isGround && other.gameObject.CompareTag("ground")) {
            // Debug.Log("no grounded");
            // isGround = false;
        // }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.CompareTag("Respawn")) {
            if (deaths != null) {
                deaths(other.gameObject);
            }
        } else if (!isGround && other.gameObject.CompareTag("ground")) {
            Debug.Log("enter grounded");
            //isGround = true;
            //jumpRequest = false;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
/*        if (!isGround && other.gameObject.CompareTag("ground")) {
            Debug.Log("stay grounded");
            isGround = true;
            jumpRequest = false;
        }*/
    }

    public void StopGame() {
        GetComponent<Animator>().Play("Player_idle");
    }

    public void StartGame() {
    }

    private bool isFacingRight = true;
    void FixedUpdate() {
        var horizontal = direction.x;
        Move(direction.x);

        if (jumpRequest) {
            jumpRequest = false;
            timeJumping = maxTimeJumping;
            rb.gravityScale = gravity;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        } else {
            if (rb.velocity.y > 0) {
                // going up
                timeJumping -= Time.fixedDeltaTime;
                if (timeJumping > 0 && Input.GetKey(KeyCode.Space)) {
                    rb.gravityScale = gravity;
                } else {
                    rb.gravityScale = gravityOnReleaseJumping;
                }
            } else {
                // going down
                rb.gravityScale = gravityFalling;
            }

            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - gravity);
        }

        // Reset de la direccion  (por si pasa dos veces por FixedUpdate sin pasar por Update, que es donde se lee el mando
        direction = Vector2.zero;
    }

    void Move(float horizontal) {
        if (Math.Sign(horizontal) == 0) {
            // No hay movimiento lateral del mando, se pone el drag original para que frene poco a poco
            currentDrag = rb.drag = drag;
            GetComponent<Animator>().Play("Player_run", -1, 0);
        } else {
            // Hay movimiento lateral del mando
            HorizontalMovement(horizontal);
            FlipSprite(horizontal);
            GetComponent<Animator>().Play("Player_run");            
        }

        // Limit the maximum speed
        if (Math.Abs(rb.velocity.x) > maxSpeed) {
            Debug.Log("Limitng to "+maxSpeed);
            var maxCurrentDirectionSpeed = Math.Sign(rb.velocity.x) * maxSpeed;
            rb.velocity = new Vector2(maxCurrentDirectionSpeed, rb.velocity.y);
        }
        velocity = rb.velocity; // update the editor UI
    }
    void FlipSprite(float horizontal) {
        // La animacion mira por defecto a la derecha. Girar la animación cuando la dirección sea contraria
        var isDirectionRight = horizontal > 0;
        var changeFacing = isDirectionRight ^ isFacingRight;
        if (changeFacing) {
            isFacingRight = !isFacingRight;
            transform.rotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
        }
    }
    void HorizontalMovement(float horizontal) {
        // Si el jugador cambia de dirección de repente (tenia velocidad en el sentido contrarior de la direccion nueva...
        var changeDirection = Math.Sign(horizontal * rb.velocity.x) == -1;
        if (changeDirection) {
            // ... se resetea la velocidad lateral (x = 0) y la y se deja la que tenia  
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        // Se añade la fuerza lateral y se pone el drag a 0
        rb.AddForce(Vector2.right * horizontal * force, ForceMode2D.Impulse);
        currentDrag = rb.drag = 0;
    }

}