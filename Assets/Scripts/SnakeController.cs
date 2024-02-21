using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float bodySpeed;
    [SerializeField] private float steerSpeed;
    [SerializeField] private GameObject bodyPrefab;
    public GameManager gm;

    private int gap = 10;
    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    public StickController MoveStick;
    public AudioSource audioSource;
    public AudioClip eatFood;
    void Start()
    {

        //positionHistory.Insert(0, transform.position);
        InvokeRepeating("UpdatePositionHistory", 0f, 0.01f);
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();

    }
    void Awake()
    {
        if (MoveStick != null)
        {
            MoveStick.StickChanged += MoveStick_StickChanged;
        }
    }

    private Vector2 MoveStickPos = Vector2.zero;

    private void MoveStick_StickChanged(object sender, StickEventArgs e)
    {
        MoveStickPos = e.Position;
    }
    void Update()
    {
        // Get Input for axis
        float h = Mathf.Abs(MoveStickPos.x) > Mathf.Abs(Input.GetAxis("Horizontal")) ? MoveStickPos.x : Input.GetAxis("Horizontal");
        float v = Mathf.Abs(MoveStickPos.y) > Mathf.Abs(Input.GetAxis("Vertical")) ? MoveStickPos.y : Input.GetAxis("Vertical");

        //move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        //steer
        transform.Rotate(Vector3.up * h * steerSpeed * Time.deltaTime);

        // Update body parts
        UpdateBodyParts();

        // Grow snake on space press (for debugging purposes, you might want to remove this later)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GrowSnake();
        }
    }

    private void UpdateBodyParts()
    {
        int index = 0;
        foreach (GameObject body in bodyParts)
        {
            Vector3 point = positionHistory[Math.Min(index * gap, positionHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * bodySpeed * Time.deltaTime;

            body.transform.LookAt(point);

            index++;
        }
    }


    void UpdatePositionHistory()
    {
        Debug.Log("UpdatePositionHistory");
        // Añadir la posición actual al inicio de la lista
        positionHistory.Insert(0, transform.position);

        // Si la lista excede el número máximo de posiciones, elimina la última
        if (positionHistory.Count > 500)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.name);
        if (other.CompareTag("Wall") || other.CompareTag("SnakeBody"))
        {
            Debug.Log("Trigger with Wall or SnakeBody detected, calling EndGame.");
            gm.EndGame();
        }
        if (other.CompareTag("food"))
        {
            GrowSnake();
            Destroy(other.gameObject);
            gm.AddScore(1);
            gm.GenerateFood();
        }
    }

    private void GrowSnake()
    {
        GameObject body = Instantiate(bodyPrefab);
        bodyParts.Add(body);
        audioSource.PlayOneShot(eatFood);
    }

}
