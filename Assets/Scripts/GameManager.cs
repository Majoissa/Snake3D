using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static int score = 0;
    public static int bestScore = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public GameObject gameOverScreen;

    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Transform floorTransform;
    private float areaWidth;
    private float areaLength;
    private AudioSource audioSource;
    public AudioClip music;
    public AudioClip gameOverClip;


    void Start()
    {
        // Configurar el AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.loop = true; // Repetir la música de fondo
        audioSource.playOnAwake = false;
        audioSource.Play(); // Iniciar la música de fondo

        areaWidth = floorTransform.localScale.x * 0.8f;
        areaLength = floorTransform.localScale.z * 0.8f;

        SpawnFood();
        score = 0;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateScoreUI();
    }

    private void SpawnFood()
    {
        float randomX = UnityEngine.Random.Range(-areaWidth / 2, areaWidth / 2);
        float randomZ = UnityEngine.Random.Range(-areaLength / 2, areaLength / 2);
        Vector3 randomPosition = new Vector3(randomX, 0.5f, randomZ) + floorTransform.position;

        Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    public void GenerateFood()
    {
        SpawnFood();
    }
    public void AddScore(int pointsToAdd)
    {
        score += pointsToAdd;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }
        UpdateScoreUI();
    }


    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
    public void EndGame()
    {
        Debug.Log("EndGame called");
        Time.timeScale = 0;
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverScreen not set in the Inspector");
        }
        audioSource.Stop();
        audioSource.PlayOneShot(gameOverClip);
    }
    public void RestartGame()
    {
        // Llamada para reiniciar el juego
        ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarga la escena actual
        Time.timeScale = 1; // Restablece el tiempo si se detuvo durante EndGame
    }
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score: " + bestScore;
        }
    }

}
