using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text levelStatusText;
    public GameController gameController;

    private Map map;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();

        AlignCameraToMap();
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

    private void AlignCameraToMap()
    {
        Vector3 mcPosition = Camera.main.transform.position;
        mcPosition = new Vector3(map.N / 2 - 0.5f, mcPosition.y, mcPosition.z);
        Camera.main.transform.position = mcPosition;
    }

    private void AlignCameraToPlayer()
    {
        Vector3 mcPosition = Camera.main.transform.position;
        mcPosition = new Vector3(mcPosition.x, mcPosition.y, transform.position.z - 6);
        Camera.main.transform.position = mcPosition;
    }

    public void SetMap(Map map)
    {
        this.map = map;
    }
}
