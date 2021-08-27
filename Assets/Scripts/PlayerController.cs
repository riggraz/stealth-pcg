using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public GameController gameController;

    private Map map;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 velocity = new Vector3(h, 0, v).normalized;

        Vector3 newPosition = rb.position + velocity * speed * Time.fixedDeltaTime;

        float x = Mathf.Clamp(newPosition.x, 0, map.N - 1);
        float z = Mathf.Clamp(newPosition.z, 0, map.M - 1);

        rb.position = new Vector3(x, newPosition.y, z);

        AlignCameraToPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !gameController.IsLevelCompleted())
        {
            rb.MovePosition(new Vector3(map.StartPoint.x, 0f, map.StartPoint.y));
        }
        else if (other.CompareTag("Finish"))
        {
            gameController.OnLevelCompleted();
        }
    }

    private void AlignCameraToPlayer()
    {
        Vector3 cPosition = Camera.main.transform.position;
        cPosition = new Vector3(map.N / 2 - 0.5f, cPosition.y, transform.position.z - 6);

        Quaternion cRotation = Quaternion.Euler(45f, 0f, 0f);
        

        if (Input.GetKey(KeyCode.Space))
        {
            cPosition = new Vector3(transform.position.x, cPosition.y, transform.position.z);
            cRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        Camera.main.transform.position = cPosition;
        Camera.main.transform.rotation = cRotation;
    }

    public void SetMap(Map map)
    {
        this.map = map;
    }
}
