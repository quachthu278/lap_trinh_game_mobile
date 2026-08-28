using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blade : MonoBehaviour
{
    [Header("Blade Settings")]
    public float minSliceVelocity = 0.01f;
    public float minDistanceToSlice = 0.1f;
    
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Collider2D bladeCollider;
    private TrailRenderer trailRenderer;

    private bool isSlicing = false;
    private Vector2 previousPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        bladeCollider = GetComponent<Collider2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (trailRenderer != null)
        {
            trailRenderer.sortingOrder = 100; // Đưa tia cắt lên trên cùng
        }
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (Pointer.current == null) return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            StartSlice();
        }
        else if (Pointer.current.press.wasReleasedThisFrame)
        {
            StopSlice();
        }
        else if (isSlicing)
        {
            ContinueSlice();
        }
    }

    private void StartSlice()
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        newPosition.z = 0f;

        transform.position = newPosition;

        isSlicing = true;
        bladeCollider.enabled = true;
        trailRenderer.enabled = true;
        trailRenderer.Clear(); // Xóa vết cắt cũ

        previousPosition = newPosition;
    }

    private void StopSlice()
    {
        isSlicing = false;
        bladeCollider.enabled = false;
        trailRenderer.enabled = false;
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        newPosition.z = 0f;

        // Nếu vuốt đủ dài
        if (Vector2.Distance(previousPosition, newPosition) > minDistanceToSlice)
        {
            float velocity = (newPosition - (Vector3)previousPosition).magnitude / Time.deltaTime;
            
            // Nếu vuốt đủ nhanh
            if (velocity > minSliceVelocity)
            {
                // Cập nhật vị trí RigidBody2D thay vì Transform trực tiếp để xử lý va chạm tốt hơn
                rb.position = newPosition;
            }
            previousPosition = newPosition;
        }
    }
}
