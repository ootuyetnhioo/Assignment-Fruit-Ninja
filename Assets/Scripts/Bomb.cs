using UnityEngine;

public class Bomb : MonoBehaviour
{
    // Phương thức được gọi khi Collider của đối tượng này chạm vào Collider của đối tượng khác
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng khác có tag là "Player" hay không
        if (other.CompareTag("Player"))
        {
            // Tắt Collider của Bomb để tránh việc xử lý va chạm tiếp theo
            GetComponent<Collider>().enabled = false;

            // Gọi phương thức Explode() từ đối tượng GameManager
            GameManager.Instance.Explode();
        }
    }
}
