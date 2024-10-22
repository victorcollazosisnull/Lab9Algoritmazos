using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float speed;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer; 

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (horizontal < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (horizontal > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }
    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(speed * horizontal, speed * vertical);
    }
}
