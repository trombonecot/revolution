using UnityEngine;
using Unity.Cinemachine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float edgeThreshold = 10f;
    public float keyboardSpeed = 15f;
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private CinemachineCamera cinemachineCam;

    private void Start()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
    }

    private void Update()
    {
        Vector3 move = Vector3.zero;

        // Mouse movement at screen edges
        if (Input.mousePosition.x >= Screen.width - edgeThreshold)
        {
            move += Vector3.right;
        }
        else if (Input.mousePosition.x <= edgeThreshold)
        {
            move += Vector3.left;
        }

        if (Input.mousePosition.y >= Screen.height - edgeThreshold)
        {
            move += Vector3.forward;
        }
        else if (Input.mousePosition.y <= edgeThreshold)
        {
            move += Vector3.back;
        }

        // Keyboard arrow movement
        move += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move the camera
        transform.position += move * moveSpeed * Time.deltaTime;

        /*// Zoom by moving the camera along the Y-axis
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 zoomDirection = Vector3.up * scroll * zoomSpeed;
            transform.position += zoomDirection;
        }*/
    }
}
