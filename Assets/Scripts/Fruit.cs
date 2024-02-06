using UnityEngine;

public class Fruit : MonoBehaviour
{
    // Các đối tượng thể hiện trạng thái của quả cả và quả đã bị chia
    public GameObject whole;
    public GameObject sliced;

    // Các thành phần của quả
    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;
    private ParticleSystem juiceEffect;

    // Điểm số khi quả bị chia
    public int points = 1;

    // Phương thức Awake() được gọi khi đối tượng được tạo
    private void Awake()
    {
        // Lấy tham chiếu đến các thành phần của quả
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        juiceEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Phương thức chia quả với các tham số như hướng, vị trí, và lực
    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        // Tăng điểm số trong GameManager khi quả bị chia
        GameManager.Instance.IncreaseScore(points);

        // Tắt Collider của quả và kích hoạt hiệu ứng nước ép
        fruitCollider.enabled = false;    // Tắt Collider của quả trái để ngừng phát hiện va chạm
        whole.SetActive(false);           // Tắt hiển thị đối tượng nguyên vẹn
        sliced.SetActive(true);           // Bật hiển thị đối tượng đã bị chia
        juiceEffect.Play();               // Kích hoạt hiệu ứng nước ép (chảy nước ép)

        // Tính góc quay cho phần đã bị chia dựa trên hướng chia
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Đặt quay của đối tượng đã chia thành góc được tính
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Lấy danh sách các Rigidbody trong phần đã bị chia
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        // Gán vận tốc và lực cho từng phần đã bị chia
        foreach (Rigidbody slice in slices)
        {
            slice.velocity = fruitRigidbody.velocity;
            // Áp dụng lực nhấn tại vị trí 'position' với hướng và độ lớn của lực là 'force'
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    // Phương thức được gọi khi Collider của đối tượng này va chạm với Collider của đối tượng khác
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng khác có tag là "Player" hay không
        if (other.CompareTag("Player"))
        {
            // Lấy tham chiếu đến Blade từ đối tượng Player
            Blade blade = other.GetComponent<Blade>();
            
            // Gọi phương thức chia quả với các thông số từ Blade
            Slice(blade.Direction, blade.transform.position, blade.sliceForce);
        }
    }
}
