using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 4f;

    private Rigidbody2D rb2D;
    private Animator animator;
    private float move;
    private bool isGrounded;
    private int coins;

    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    public TMP_Text textCoins;

    public AudioSource audioSource;
    public AudioClip coinClip;
    public AudioClip barrelClip;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (textCoins != null)
    {   
            textCoins.text = coins.ToString();
    }
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

        rb2D.linearVelocity = new Vector2(
            move * speed,
            rb2D.linearVelocity.y
        );

        if (move != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(move),
                1f,
                1f
            );
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.linearVelocity = new Vector2(
                rb2D.linearVelocity.x,
                jumpForce
            );
        }

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            if (audioSource != null && coinClip != null)
            {
                audioSource.PlayOneShot(coinClip);
            }

            coins++;

            if (textCoins != null)
            {
                textCoins.text = coins.ToString();
            }

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Barrel"))
        {
            if (audioSource != null && barrelClip != null)
            {
                audioSource.PlayOneShot(barrelClip);
            }

            Vector2 knockbackDir =
                (rb2D.position - (Vector2)collision.transform.position).normalized;

            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3f, ForceMode2D.Impulse);

            BoxCollider2D[] colliders =
                collision.gameObject.GetComponents<BoxCollider2D>();

            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            Animator barrelAnimator = collision.GetComponent<Animator>();

            if (barrelAnimator != null)
            {
                barrelAnimator.enabled = true;
            }

            Destroy(collision.gameObject, 0.5f);
        }

        if (collision.CompareTag("Spikes"))
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
