using UnityEngine;

public class Blade : MonoBehaviour
{
    // Các thuộc tính quy định sức mạnh và tốc độ tối thiểu của lưỡi dao
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;

    // Tham chiếu đến Camera chính, Collider và TrailRenderer của lưỡi dao
    private Camera mainCamera;
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    // Hướng của lưỡi dao và các thuộc tính chỉ đọc liên quan
    private Vector3 direction;
    public Vector3 Direction => direction;

    // Biến đánh dấu trạng thái của việc chia đối tượng
    private bool slicing;
    public bool Slicing => slicing;

    // Phương thức Awake() được gọi khi đối tượng được tạo
    private void Awake()
    {
        // Gán tham chiếu đến các thành phần
        mainCamera = Camera.main;
        sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();
    }

    // Phương thức OnEnable() được gọi khi đối tượng được bật
    private void OnEnable()
    {
        // Dừng quá trình chia khi đối tượng được bật
        StopSlice();
    }

    // Phương thức OnDisable() được gọi khi đối tượng bị tắt
    private void OnDisable()
    {
        // Dừng quá trình chia khi đối tượng bị tắt
        StopSlice();
    }

    // Phương thức Update() được gọi mỗi frame
    private void Update()
    {
        // Bắt đầu quá trình chia khi người dùng nhấn nút chuột trái
        if (Input.GetMouseButtonDown(0)) {
            StartSlice();
        } 
        // Dừng quá trình chia khi người dùng nhả nút chuột trái
        else if (Input.GetMouseButtonUp(0)) {
            StopSlice();
        } 
        // Nếu đang trong quá trình chia, tiếp tục quá trình chia
        else if (slicing) {
            ContinueSlice();
        }
    }

    // Phương thức bắt đầu quá trình chia
    private void StartSlice()
    {
        // Lấy vị trí của chuột và chuyển đổi thành vị trí trong không gian thế giới
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        transform.position = position;

        // Thiết lập các trạng thái và thuộc tính liên quan đến quá trình chia
        slicing = true;             // Đặt trạng thái chia là đang chia
        sliceCollider.enabled = true;  // Bật Collider để phát hiện va chạm trong quá trình chia
        sliceTrail.enabled = true;     // Bật hiển thị vết dầu sau vật thể đang chia
        sliceTrail.Clear();            // Xóa dữ liệu vết dầu để chuẩn bị cho vết mới
    }

    // Phương thức dừng quá trình chia
    private void StopSlice()
    {
        // Thiết lập các trạng thái và thuộc tính liên quan đến dừng quá trình chia
        slicing = false;            // Đặt trạng thái chia thành không chia
        sliceCollider.enabled = false;  // Tắt Collider để ngừng phát hiện va chạm trong quá trình chia
        sliceTrail.enabled = false;     // Tắt hiển thị vết dầu sau vật thể đang chia
    }

    // Phương thức tiếp tục quá trình chia
    private void ContinueSlice()
    {
        // Lấy vị trí mới của chuột và chuyển đổi thành vị trí trong không gian thế giới
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f;
        
        // Tính hướng di chuyển từ vị trí cũ đến vị trí mới
        direction = newPosition - transform.position;

        // Tính toán vận tốc dựa trên độ lớn của hướng di chuyển
        float velocity = direction.magnitude / Time.deltaTime;
        
        // Bật hoặc tắt Collider tùy thuộc vào vận tốc
        sliceCollider.enabled = velocity > minSliceVelocity;

        // Cập nhật vị trí của lưỡi dao
        transform.position = newPosition;
    }
}
