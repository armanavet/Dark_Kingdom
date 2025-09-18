using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CamerManager : MonoBehaviour
{
    Camera mainCamera;
    float fieldOfView;
    float initialCursorPosition;
    Vector2 initialMousePosition;
    [Header("Movement Settings")]
    [Tooltip("Based movement speed of the camera")]
    [SerializeField] float speed;

    [Tooltip("Reference to the game board (Used as the game bounds).")]
    [SerializeField] GameBoard Map;

    [Header("Zoom Settings")]
    [Tooltip("Minimum allowed zoom (closest to the board).")]
    [SerializeField] float minZoom;

    [Tooltip("Maximum allowed zoom (farthest from the board).")]
    [SerializeField] float maxZoom;

    [Tooltip("Minimum allowed field of view for the perspective camera.")]
    [SerializeField] float minFieldOfView;

    [Tooltip("Maximum allowed field of view for the perspective camera.")]
    [SerializeField] float maxFieldOfView;

    [Tooltip("How quickly the camera zoomes in/out.")]
    [SerializeField] float zoomSpeed;

    [Header("Rotation Settings")]
    [Tooltip("How quickly the caera rotates around it's pivot.")]
    [SerializeField] float rotationSpeed;

    [Header("Mouse Settings")]
    [Tooltip("How fast the camera moves when dragging with the left mouse button pressed.")]
    [SerializeField] float mouseDragSpeed;

    private void Start()
    {
        mainCamera = Camera.main;
        fieldOfView = mainCamera.fieldOfView;
    }
    private void Update()
    {
        CameraMove();
        CameraZoom();
        CameraRotate();
    }
    void CameraMove()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementZ = Input.GetAxisRaw("Vertical");
        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        right.y = 0;
        forward.y = 0;
        if (Input.GetMouseButtonDown(0))
        {
            initialMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 newMausePos = Input.mousePosition;
            Vector2 finalMousePos = newMausePos - initialMousePosition;

            movementX = -finalMousePos.x / mouseDragSpeed;
            movementZ = -finalMousePos.y / mouseDragSpeed;
            initialMousePosition = Input.mousePosition;
        }
        Vector3 newPosition = transform.position + (right * movementX + forward * movementZ) * speed * Time.deltaTime;//Get A D and W S
        newPosition.x = Mathf.Clamp(newPosition.x, (Map.transform.position.x - Map.Length * 0.25f), (Map.transform.position.x + Map.Length * 0.25f));
        newPosition.z = Mathf.Clamp(newPosition.z, (Map.transform.position.z - Map.Width * 0.5f), (Map.transform.position.z + Map.Width * 0.25f));
        transform.position = newPosition;
    }
    void CameraZoom()
    {
        Vector3 zoom = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        if (zoom.y > minZoom && zoom.y < maxZoom)
        {
            transform.position = zoom;
            fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
            fieldOfView = Mathf.Clamp(fieldOfView, minFieldOfView, maxFieldOfView);
            mainCamera.fieldOfView = fieldOfView;
        }

    }
   
    void CameraRotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            initialCursorPosition = Input.mousePosition.x;
        }
        if (Input.GetMouseButton(1))
        {
            float rotation = Input.mousePosition.x - initialCursorPosition;
            rotation = Mathf.Clamp(rotation, -1, 1);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + rotation * rotationSpeed * Time.deltaTime, transform.localEulerAngles.z);
            initialCursorPosition = Input.mousePosition.x;
        }

    }
}
