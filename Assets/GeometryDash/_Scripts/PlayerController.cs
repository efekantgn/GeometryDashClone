using System.Collections;
using UnityEngine;


public enum LevelType
{
    Jump,
    Fly
}

public class PlayerController : MonoBehaviour
{
    private const string _tObstacle = "Obstacle";
    private const string _tPortal = "Portal";
    private Vector2 _startPos;
    private Vector2 _boxSize;
    private float _castDistance;
    private Rigidbody2D _rb2d;
    private BoxCollider2D _bc2d;
    private float _gravityScale;
    private LevelType _levelTypeOnStart;
    private AudioSource _deathSound;
    private bool _isDead;

    [SerializeField] private Transform _playerSprite;
    [SerializeField] private Transform _planeSprite;
    [SerializeField] private LevelType _levelType;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _flyingRotSpeed=2;
    [SerializeField] private ParticleSystem _onGroundParticle;
    [SerializeField] private ParticleSystem _onDeathParticle;



    #region Encapsulation
    public Rigidbody2D Rb2d { get => _rb2d; set => _rb2d = value; }
    public LevelType LevelType { get => _levelType; set => _levelType = value; }
    public BoxCollider2D Bc2d { get => _bc2d; set => _bc2d = value; }
    public Vector2 BoxSize { get => _boxSize; set => _boxSize = value; }
    public float CastDistance { get => _castDistance; set => _castDistance = value; }
    public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }
    public float JumpForce { get => _jumpForce; set => _jumpForce = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public Vector2 StartPos { get => _startPos; set => _startPos = value; }
    public Transform PlayerSprite { get => _playerSprite; set => _playerSprite = value; }
    public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
    public float GravityScale { get => _gravityScale; set => _gravityScale = value; }
    public float FlyingRotSpeed { get => _flyingRotSpeed; set => _flyingRotSpeed = value; }
    public LevelType LevelTypeOnStart { get => _levelTypeOnStart; set => _levelTypeOnStart = value; }
    public ParticleSystem OnGroundParticle { get => _onGroundParticle; set => _onGroundParticle = value; }
    public Transform PlaneSprite { get => _planeSprite; set => _planeSprite = value; }
    public AudioSource DeathSound { get => _deathSound; set => _deathSound = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public ParticleSystem OnDeathParticle { get => _onDeathParticle; set => _onDeathParticle = value; }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DeathSound=GetComponent<AudioSource>();
        StartPos = transform.position;
        Rb2d = GetComponent<Rigidbody2D>();
        Bc2d = GetComponent<BoxCollider2D>();
        BoxSize = new Vector2(Bc2d.size.x, Bc2d.size.y*.1f);
        CastDistance = Bc2d.size.y / 2;
        GravityScale = Rb2d.gravityScale;
        LevelTypeOnStart = LevelType;
        OnBirth();
    }

    private void Update()
    {
        if (!Rb2d) return;
        if (IsDead) return;

        Move();

        if (LevelType == LevelType.Jump)
        {
            if (IsGrounded())
            {
                if (Input.GetMouseButton(0))
                {
                    Jump();

                }


                if (OnGroundParticle.isStopped)
                    OnGroundParticle.Play();

                CharRotationController();

            }
            else
            {
                if (OnGroundParticle.isPlaying)
                    OnGroundParticle.Stop();
                    
                PlayerSprite.Rotate(Vector3.back * RotationSpeed * Time.deltaTime);
            }

        }
        else if (LevelType == LevelType.Fly)
        {

            Fly();
            PlayerSprite.transform.rotation = Quaternion.Euler(0, 0, Rb2d.velocity.y*FlyingRotSpeed);
           
        }
        
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(Bc2d.bounds.center, BoxSize,0,-transform.up, CastDistance,GroundLayer);
    }


    private void Move()
    {
        Rb2d.velocity = new Vector2(MoveSpeed, Rb2d.velocity.y);
    }
    
    /// <summary>
    /// Karakter yere dokunduðu an rotasyonunun nasýl olacaðýnýayarlýyor.
    /// </summary>
    private void CharRotationController()
    {
        Vector3 spriteRot = PlayerSprite.transform.eulerAngles;
        spriteRot.z = Mathf.Round(spriteRot.z / 90) * 90;
        PlayerSprite.rotation = Quaternion.Euler(spriteRot);
    }
    private void Jump()
    {
         Rb2d.velocity = new Vector2(Rb2d.velocity.x, JumpForce);
    }

    private void Fly()
    {
        if (Input.GetMouseButton(0))
            Rb2d.gravityScale = GravityScale * -1;
        else
            Rb2d.gravityScale = GravityScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_tPortal))
        {
            PlayerSprite.rotation = Quaternion.Euler(Vector3.zero);
            switch (LevelType)
            {
                case LevelType.Jump:
                    LevelType = LevelType.Fly;
                    PlaneSprite.gameObject.SetActive(true);
                    OnGroundParticle.Play();
                    break;
                case LevelType.Fly:
                    LevelType = LevelType.Jump;
                    PlaneSprite.gameObject.SetActive(false);
                    break;
                default: break;
            }



        }
    }
    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        


        if (collision.transform.CompareTag(_tObstacle))
        {
            OnDeath();

            //WaitforSecond
            yield return new WaitForSeconds(1);
            RestartGame();
        }

    }

    private void OnDeath()
    {
        PlayerSprite.gameObject.SetActive(false);   
        DeathSound.Play();
        OnDeathParticle.Play();
        Rb2d.velocity = Vector3.zero;
        Rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        IsDead = true;

    }

    public void OnBirth()
    {
        PlayerSprite.gameObject.SetActive(true);   
        OnDeathParticle.Stop();
        Rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        IsDead = false;
    }


    private void RestartGame()
    {
        OnBirth();

        LevelType = LevelTypeOnStart;
        Rb2d.gravityScale = GravityScale;
        transform.position = StartPos;
        PlayerSprite.rotation = Quaternion.Euler(Vector3.zero);
        PlaneSprite.gameObject.SetActive(false);
    }
}
