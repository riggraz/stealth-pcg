using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private int M, N;

    private void Start()
    {
        AlignCameraToMap();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 velocity = new Vector3(h, 0, v).normalized;

        Vector3 newPosition = transform.position + velocity * speed * Time.fixedDeltaTime;

        float x = Mathf.Clamp(newPosition.x, 0, N - 1);
        float z = Mathf.Clamp(newPosition.z, 0, M - 1);

        transform.position = new Vector3(x, newPosition.y, z);

        AlignCameraToPlayer();
    }

    private void AlignCameraToMap()
    {
        Vector3 mcPosition = Camera.main.transform.position;
        mcPosition = new Vector3(N / 2 - 0.5f, mcPosition.y, mcPosition.z);
        Camera.main.transform.position = mcPosition;
    }

    private void AlignCameraToPlayer()
    {
        Vector3 mcPosition = Camera.main.transform.position;
        mcPosition = new Vector3(mcPosition.x, mcPosition.y, transform.position.z - 6);
        Camera.main.transform.position = mcPosition;
    }

    public void SetMapSize(int M, int N)
    {
        this.M = M;
        this.N = N;
    }
}
