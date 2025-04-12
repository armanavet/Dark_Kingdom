using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CamerController : MonoBehaviour
{
    Camera mainCamera;
    float fieldOfView;
    [SerializeField] float speed;
    [SerializeField] BoxCollider Map;
    Vector3 sizeOfMap;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] float zoomSpeed;

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
    }

    void CameraMove()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementZ = Input.GetAxisRaw("Vertical");
        if (Input.mousePosition.x >= Screen.width)
        {
            movementX += 1;
        }else if (Input.mousePosition.x <= 0)
        {
            movementX -= 1;
        }
        if (Input.mousePosition.y >= Screen.height)
        {
            movementZ += 1;
        }
        else if (Input.mousePosition.y <= 0)
        {
            movementZ -= 1;
        }
        Vector3 newPosition = transform.position + new Vector3(movementX, 0, movementZ) * speed * Time.deltaTime;//Get A D and W S
        newPosition.x = Mathf.Clamp(newPosition.x, (Map.transform.position.x - sizeOfMap.x * 0.25f), (Map.transform.position.x + sizeOfMap.x * 0.25f));
        newPosition.z = Mathf.Clamp(newPosition.z, (Map.transform.position.z - sizeOfMap.z * 0.5f), (Map.transform.position.z + sizeOfMap.z * 0.25f));
        transform.position = newPosition;
    }
    void CameraZoom()
    {
        //Vector3 zoom = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        Vector3 zoom = transform.position + new Vector3(0, transform.localPosition.z * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, 0);
        zoom.y = Mathf.Clamp(zoom.y, minZoom,maxZoom);
        transform.position = zoom;
    }
    void CameraRotate()
    {

    }
}
