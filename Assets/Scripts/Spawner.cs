using System.Collections;
using UnityEngine;

// RequireComponent đảm bảo rằng Collider sẽ được thêm vào đối tượng nếu chưa có
[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    // Collider xác định khu vực sinh các đối tượng
    private Collider spawnArea;

    // Mảng chứa các đối tượng Fruit và đối tượng Bomb
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;

    // Xác suất sinh ra đối tượng Bomb thay vì đối tượng Fruit
    [Range(0f, 1f)] public float bombChance = 0.05f;

    // Thời gian giữa các lần sinh đối tượng
    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    // Góc xoay của đối tượng khi sinh ra
    public float minAngle = -15f;
    public float maxAngle = 15f;

    // Lực tác động lên đối tượng khi sinh ra
    public float minForce = 18f;
    public float maxForce = 22f;

    // Thời gian tồn tại tối đa của đối tượng
    public float maxLifetime = 5f;

    // Phương thức Awake() được gọi khi đối tượng được tạo
    private void Awake()
    {
        // Lấy tham chiếu đến Collider của đối tượng
        spawnArea = GetComponent<Collider>();
    }

    // Phương thức được gọi khi đối tượng được bật
    private void OnEnable()
    {
        // Bắt đầu Coroutine để thực hiện quá trình sinh các đối tượng
        StartCoroutine(Spawn());
    }

    // Phương thức được gọi khi đối tượng bị tắt
    private void OnDisable()
    {
        // Dừng tất cả các coroutine đang chạy
        StopAllCoroutines();
    }

    // Coroutine thực hiện quá trình sinh các đối tượng
    private IEnumerator Spawn()
    {
        // Đợi 2 giây trước khi bắt đầu sinh các đối tượng
        yield return new WaitForSeconds(2f);

        // Lặp vô hạn trong khi đối tượng Spawner đang được bật
        while (enabled)
        {
            // Chọn ngẫu nhiên một đối tượng từ mảng fruitPrefabs
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            // Kiểm tra xem có nên sinh ra một đối tượng Bomb thay vì Fruit dựa trên xác suất
            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            // Tạo vị trí ngẫu nhiên trong khu vực sinh
            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            // Tạo đối tượng với vị trí, xoay và prefab đã chọn
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));
            GameObject fruit = Instantiate(prefab, position, rotation);

            // Hủy đối tượng sau khoảng thời gian maxLifetime
            Destroy(fruit, maxLifetime);

            // Áp dụng lực tác động lên đối tượng để di chuyển nó lên trên
            float force = Random.Range(minForce, maxForce);
            // Lấy tham chiếu đến thành phần Rigidbody của đối tượng fruit
            // Áp dụng lực lên đối tượng theo hướng lên trên (trục y) với giá trị lực là force
            // ForceMode.Impulse được sử dụng để áp dụng lực như là một lực một lần
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            // Chờ một khoảng thời gian ngẫu nhiên trước khi sinh tiếp đối tượng mới
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}
