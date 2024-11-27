using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fallDamageMultiplier = 0.5f;
    [SerializeField] private float fallDamageThreshold = 20f;
    [SerializeField] private float forwardBoost = 5f;

    [Header("Health Settings")]
    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    [Header("Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    private int remainingJumps;

    [Header("Checkpoint")]
    [SerializeField] private Transform checkPoint;

    private Rigidbody rb;
    private float windSpeed = 0f;
    private bool isGrounded = false;
    private bool hasUsedBoost = false;
    private bool hasStarted = false;
    private float fallSpeed = 0f;

    public delegate void WinAction();
    public static event WinAction OnWin; 
    public delegate void DefeatAction();
    public static event DefeatAction OnDefeat; 
    public delegate void StartAction();
    public static event StartAction OnStart;

    private void Start()
    {
        this.transform.position = checkPoint.position;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentHP = maxHP;
        remainingJumps = maxJumps;

        Mine[] mines = FindObjectsOfType<Mine>();
        foreach (Mine mine in mines)
        {
            mine.OnExplosion += TakeDamage;
        }
    }

    private void Update()
    {
        MovePlayer();

        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
        {
            Jump();
        }

        if (!isGrounded && !hasUsedBoost && Input.GetMouseButtonDown(1))
        {
            rb.AddForce(transform.forward * forwardBoost, ForceMode.Impulse);
            hasUsedBoost = true;
        }

        CheckGrounded();

        if (!isGrounded)
        {
            fallSpeed = Mathf.Abs(rb.velocity.y);
        }

        //CheckForCannonShell(); 
    }

    private void CheckGrounded()
    {
        float rayLength = 0.1f;
        RaycastHit hit;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength) ||
                     Physics.Raycast(transform.position + Vector3.left * 0.5f, Vector3.down, out hit, rayLength) ||
                     Physics.Raycast(transform.position + Vector3.right * 0.5f, Vector3.down, out hit, rayLength);

        if (isGrounded)
        {
            remainingJumps = maxJumps;
        }
    }

    private void MovePlayer()
    {
        // Получаем ввод по осям WASD (Horizontal: A/D, Vertical: W/S)
        float moveInputX = Input.GetAxis("Horizontal"); // A/D или Стрелки влево/вправо
        float moveInputZ = Input.GetAxis("Vertical");   // W/S или Стрелки вверх/вниз

        // Вектор инверсного движения в локальной системе координат
        Vector3 moveDirection = new Vector3(-moveInputX, 0f, -moveInputZ).normalized;

        // Если есть ввод движения
        if (moveDirection.magnitude >= 0.1f)
        {
            // Рассчитываем угол поворота в сторону инверсного движения (в радианах -> в градусы)
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // Плавный поворот персонажа в сторону инверсного движения
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);

            // Применяем поворот
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // Рассчитываем вектор движения относительно текущей ориентации объекта
            Vector3 move = transform.forward * moveSpeed;

            // Учитываем ветер по горизонтальной оси
            Vector3 velocity = move + Vector3.right * windSpeed;

            // Сохраняем текущую вертикальную скорость
            velocity.y = rb.velocity.y;

            // Применяем скорость к Rigidbody
            rb.velocity = velocity;
        }
        else
        {
            // Если нет ввода с клавиш, останавливаем движение, исключая вертикальную скорость
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }


    private void UpdateWindSpeed(float newWindSpeed)
    {
        windSpeed = newWindSpeed;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        hasUsedBoost = false;
        remainingJumps--;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            TeleportToCheckPoint();
        }
    }

    private void TeleportToCheckPoint()
    {
        if (checkPoint != null)
        {
            float randomXOffset = Random.Range(-0.5f, 0.5f) * checkPoint.localScale.x;
            Vector3 teleportPosition = new Vector3(
                checkPoint.position.x + randomXOffset,
                checkPoint.position.y,
                checkPoint.position.z
            );
            transform.position = teleportPosition;
        }
    }

    private void CheckForCannonShell()
    {
        float checkDistance = 1f; 

        Vector3[] directions = new Vector3[]
        {
            transform.forward,
            transform.right,
            -transform.right,
            transform.up,
            -transform.up,
            transform.forward + transform.right,
            transform.forward - transform.right,
            transform.forward + -transform.right,
            transform.forward - -transform.right,
            transform.forward + transform.up,
            transform.forward - transform.up,
            transform.right + transform.up,
            transform.right - transform.up,
            -transform.right + transform.up,
            -transform.right - transform.up,
        };

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction.normalized, out hit, checkDistance))
            {
                if (hit.collider.CompareTag("CannonShell"))
                {
                    Rigidbody shellRb = hit.collider.GetComponent<Rigidbody>();
                    if (shellRb != null)
                    {
                        Debug.Log(".");
                        TakeDamage(10f);
                        Destroy(hit.collider.gameObject);
                    }
                }
                else if (hit.collider.CompareTag("Hammer"))
                {
                    Debug.Log("Hit by Hammer!");
                    TakeDamage(50f); 
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (fallSpeed > fallDamageThreshold)
            {
                float fallDamage = (fallSpeed - fallDamageThreshold) * fallDamageMultiplier;
                TakeDamage(fallDamage);
            }

            isGrounded = true;
            hasUsedBoost = false;
            fallSpeed = 0f;
        }

        if (collision.gameObject.CompareTag("Border"))
        {
            TeleportToCheckPoint();
            OnDefeat?.Invoke();
        }

        if (collision.gameObject.CompareTag("CannonShell"))
        {
            Rigidbody shellRb = collision.gameObject.GetComponent<Rigidbody>();
            if (shellRb != null)
            {
                float cannonShellSpeed = shellRb.velocity.magnitude; 
                windSpeed += cannonShellSpeed * 10f; 
                TakeDamage(10f);
            }
        }

        if (collision.gameObject.CompareTag("Hammer"))
        {
            TakeDamage(50f);  
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Windy31 windBlock = other.GetComponent<Windy31>();
        if (windBlock != null)
        {
            windBlock.OnChangeSpeed += UpdateWindSpeed;
            UpdateWindSpeed(windBlock.GetCurrentWindSpeed());
        }

        if (other.CompareTag("StartZone") && !hasStarted)
        {
            hasStarted = true;
            OnStart?.Invoke();  
        }

        if (other.CompareTag("CheckPoint"))
        {
            checkPoint = other.transform;
        }

        if (other.CompareTag("WinZone"))
        {
            OnWin?.Invoke();  
        }

        if (other.CompareTag("CannonShell"))
        {
            TakeDamage(10f);
        }

        if (other.CompareTag("Hammer"))
        {
            TakeDamage(50f);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Windy31 windBlock = other.GetComponent<Windy31>();
        if (windBlock != null)
        {
            windBlock.OnChangeSpeed -= UpdateWindSpeed;
            UpdateWindSpeed(0f);
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }
}
