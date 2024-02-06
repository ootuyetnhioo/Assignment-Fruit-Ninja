using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Đối tượng GameManager sẽ được thiết lập là duy nhất thông qua mô hình singleton
    public static GameManager Instance { get; private set; }

    // Các thành phần và đối tượng được kết nối trong trình chỉnh sửa Unity
    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private Text hightCoreScoreText;
    [SerializeField] private Text gameOverscoreText;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject gameOverPaint;

    // Biến điểm số và các biến liên quan
    private int score = 0;
    private int bestScore;
    private int hightCore;
    public int Score => score;

    // Biến kiểm soát trạng thái game over và danh sách các coroutine đang chạy
    private bool isGameOver;
    private List<Coroutine> runningCoroutines = new List<Coroutine>();

    // Phương thức Awake() được gọi khi đối tượng được tạo
    private void Awake()
    {
        // Kiểm tra xem có GameManager nào khác đang tồn tại không, nếu có, hủy bản sao hiện tại
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            // Nếu chưa có GameManager nào tồn tại, đặt Instance thành bản thân và không hủy khi chuyển scene
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Phương thức Start() được gọi khi game bắt đầu
    private void Start()
    {
        // Khởi tạo điểm số cao nhất từ PlayerPrefs
        bestScore = PlayerPrefs.GetInt("bestScore", 0);
        hightCore = bestScore;

        // Cập nhật điểm số cao nhất lên giao diện
        UpdateBestScoreText();

        // Bắt đầu một game mới
        NewGame();
    }

    // Phương thức bắt đầu một game mới
    private void NewGame()
    {
        // Dừng tất cả các coroutine đang chạy và làm sạch danh sách
        StopAllCoroutines();
        runningCoroutines.Clear();

        // Ẩn giao diện game over
        gameOverPaint.SetActive(false);

        // Đặt lại thời gian chạy của game và trạng thái game over
        Time.timeScale = 1f;
        isGameOver = false;

        // Làm sạch scene từ các quả và bom
        ClearScene();

        // Kích hoạt Blade và Spawner để bắt đầu trò chơi mới
        blade.enabled = true;
        spawner.enabled = true;

        // Đặt lại điểm số
        score = 0;
        scoreText.text = score.ToString();
    }

    // Phương thức làm sạch scene bằng cách hủy các đối tượng Fruit và Bomb
    private void ClearScene()
    {
        MonoBehaviour[] objectsToDestroy = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour obj in objectsToDestroy)
        {
            if (obj is Fruit || obj is Bomb)
            {
                DestroyImmediate(obj.gameObject);
            }
        }
    }

    // Phương thức tăng điểm số và cập nhật điểm số cao nhất nếu cần
    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScore = score;
            hightCore = bestScore;
            PlayerPrefs.SetInt("bestScore", bestScore);
            UpdateBestScoreText();
        }
    }

    // Phương thức xử lý vụ nổ khi quả hoặc bom bị chia
    public void Explode()
    {
        if (!isGameOver)
        {
            // Đặt trạng thái game over và tắt Blade và Spawner
            isGameOver = true;
            blade.enabled = false;
            spawner.enabled = false;

            // Dừng tất cả các coroutine đang chạy
            foreach (var coroutine in runningCoroutines)
            {
                StopCoroutine(coroutine);
            }

            // Bắt đầu quá trình vụ nổ
            StartCoroutine(ExplodeSequence());
        }
    }

    // Phương thức chạy chuỗi các coroutine liên quan đến vụ nổ
    private IEnumerator ExplodeSequence()
    {
        // Bắt đầu quá trình vụ nổ và thêm coroutine vào danh sách đang chạy
        Coroutine explodeCoroutine = StartCoroutine(ExplodeAnimation());
        runningCoroutines.Add(explodeCoroutine);

        // Chờ 1 giây sau đó kết thúc game
        yield return new WaitForSecondsRealtime(1f);
        GameOver();

        // Loại bỏ coroutine khỏi danh sách đang chạy
        runningCoroutines.Remove(explodeCoroutine);
    }

    // Phương thức thực hiện quá trình vụ nổ
    private IEnumerator ExplodeAnimation()
    {
        // Thời gian và màu sắc sử dụng trong quá trình vụ nổ
        float elapsed = 0f;
        float duration = 0.5f;

        // Thực hiện quá trình vụ nổ
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            // Giảm tốc độ thời gian khi quá trình vụ nổ diễn ra
            Time.timeScale = 1f - t;

            // Cập nhật thời gian đã trôi qua
            elapsed += Time.unscaledDeltaTime;

            // Đợi một frame
            yield return null;
        }

        // Thiết lập lại thời gian và thực hiện quá trình giảm màu
        elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            // Cập nhật thời gian đã trôi qua
            elapsed += Time.unscaledDeltaTime;

            // Đợi một frame
            yield return null;
        }
    }

    // Phương thức xử lý khi game over
    private void GameOver()
    {
        // Hiển thị giao diện và điểm số cuối cùng
        gameOverPaint.SetActive(true);
        gameOverscoreText.text = score.ToString();

        // Làm sạch scene
        ClearScene();
    }

    // Phương thức cập nhật giao diện với điểm số cao nhất
    private void UpdateBestScoreText()
    {
        bestScoreText.text = bestScore.ToString();
        hightCoreScoreText.text = hightCore.ToString();
    }

    // Phương thức khởi động lại game khi nút Restart được nhấn
    public void GameRestart()
    {
        gameOverPaint.SetActive(false);
        NewGame();
    }
}
