using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CamerController : MonoBehaviour
{
    Camera mainCamera;
    float fieldOfView;
    float initialCursorPosition;
    [SerializeField] float speed;
    [SerializeField] BoxCollider Map;
    Vector3 sizeOfMap;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] float minFieldOfView;
    [SerializeField] float maxFieldOfView;
    [SerializeField] float zoomSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] bool m;
    private void Start()
    {
        mainCamera = Camera.main;
        fieldOfView = mainCamera.fieldOfView;
        sizeOfMap = Map.size;
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
        //if (Input.mousePosition.x >= Screen.width)
        //{
        //    movementX += 1;
        //}
        //else if (Input.mousePosition.x <= 0)
        //{
        //    movementX -= 1;
        //}
        //if (Input.mousePosition.y >= Screen.height)
        //{
        //    movementZ += 1;
        //}
        //else if (Input.mousePosition.y <= 0)
        //{
        //    movementZ -= 1;
        //}
        Vector3 newPosition = transform.position + (right * movementX + forward * movementZ) * speed * Time.deltaTime;//Get A D and W S
        newPosition.x = Mathf.Clamp(newPosition.x, (Map.transform.position.x - sizeOfMap.x * 0.25f), (Map.transform.position.x + sizeOfMap.x * 0.25f));
        newPosition.z = Mathf.Clamp(newPosition.z, (Map.transform.position.z - sizeOfMap.z * 0.5f), (Map.transform.position.z + sizeOfMap.z * 0.25f));
        transform.position = newPosition;
    }
    void CameraZoom()
    {
        //Vector3 zoom = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        Vector3 zoom = transform.position - new Vector3(0,Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime,0);
        zoom.y = Mathf.Clamp(zoom.y, minZoom, maxZoom);
        transform.position = zoom;
        fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        fieldOfView = Mathf.Clamp(fieldOfView, minFieldOfView,maxFieldOfView);
        mainCamera.fieldOfView = fieldOfView;
    }

    //void CameraZoom()
    //{
    //    float scroll = Input.GetAxis("Mouse ScrollWheel");
    //    if (Mathf.Approximately(scroll, 0f)) return;
    //    Vector3 zoomTarget = transform.position + transform.forward * 100f;
    //    Vector3 direction = (transform.position - zoomTarget).normalized;
    //    float distance = Vector3.Distance(transform.position, zoomTarget);
    //    float targetDistance = Mathf.Clamp(distance - scroll * zoomSpeed * Time.deltaTime, minZoom, maxZoom);

    //    transform.position = zoomTarget + direction * targetDistance;

    //}
    void CameraRotate()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            initialCursorPosition = Input.mousePosition.x;
        }
        if (Input.GetMouseButton(1))
        {
            float rotation = Input.mousePosition.x - initialCursorPosition;
            rotation = Mathf.Clamp(rotation,-1,1);
            transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + rotation * rotationSpeed * Time.deltaTime, transform.localEulerAngles.z);
        }

    }
}
