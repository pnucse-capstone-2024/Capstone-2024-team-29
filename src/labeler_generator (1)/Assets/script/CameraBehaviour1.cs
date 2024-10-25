using System.IO;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float _zoomSpeed = 300f; // 100배 증가
    [SerializeField]
    float _zoomMax = 2f;
    [SerializeField]
    float _zoomMin = 10f;

    [SerializeField]
    float _RotateSpeed = -1f; // 100배 증가
    [SerializeField]
    float _dragSpeed = 10f; // 100배 증가
    [SerializeField]
    float _inputSpeed = 2000f; // 100배 증가

    private void LateUpdate()
    {
        CameraZoom();
        CameraDrag();
        CameraRotate();
        CameraRotateWithKey();
        CameraInput();
        TakeScreenshot();
    }

    void CameraRotate()
    {
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            Vector3 rotateValue = new Vector3(y, x * -1, 0);
            transform.eulerAngles = transform.eulerAngles - rotateValue;
            transform.eulerAngles += rotateValue * _RotateSpeed;
        }
    }

    void CameraRotateWithKey()
    {
        Vector3 rotateValue = new Vector3();
        if (Input.GetKey(KeyCode.UpArrow))
            rotateValue += new Vector3(-20f, 0, 0); // 100배 증가
        if (Input.GetKey(KeyCode.DownArrow))
            rotateValue += new Vector3(20f, 0, 0); // 100배 증가
        if (Input.GetKey(KeyCode.LeftArrow))
            rotateValue += new Vector3(0, -20f, 0); // 100배 증가
        if (Input.GetKey(KeyCode.RightArrow))
            rotateValue += new Vector3(0, 20f, 0); // 100배 증가

        Vector3 p = rotateValue;
        if (p.sqrMagnitude > 0)
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * 0.01f;

            p.x = Mathf.Clamp(p.x, -_inputSpeed, _inputSpeed);
            p.y = Mathf.Clamp(p.y, -_inputSpeed, _inputSpeed);
            p.z = Mathf.Clamp(p.z, -_inputSpeed, _inputSpeed);

            transform.Rotate(p);
        }
    }

    void CameraZoom()
    {
        float _zoomDirection = Input.GetAxis("Mouse ScrollWheel");

        if (transform.position.y <= _zoomMax && _zoomDirection > 0)
            return;

        if (transform.position.y >= _zoomMin && _zoomDirection < 0)
            return;

        transform.position += transform.forward * _zoomDirection * _zoomSpeed;
    }

    void CameraDrag()
    {
        if (Input.GetMouseButton(0))
        {
            float posX = Input.GetAxis("Mouse X");
            float posZ = Input.GetAxis("Mouse Y");

            Quaternion v3Rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            transform.position += v3Rotation * new Vector3(posX * -_dragSpeed, 0, posZ * -_dragSpeed);
        }
    }

    float totalRun = 1.0f;
    private void CameraInput()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
            p_Velocity += new Vector3(0, 5f, 0); // 100배 증가
        if (Input.GetKey(KeyCode.S))
            p_Velocity += new Vector3(0, -5f, 0); // 100배 증가
        if (Input.GetKey(KeyCode.Alpha1))
            p_Velocity += new Vector3(0, 0, 5f); // 100배 증가
        if (Input.GetKey(KeyCode.Alpha2))
            p_Velocity += new Vector3(0, 0, -5f); // 100배 증가
        if (Input.GetKey(KeyCode.A))
            p_Velocity += new Vector3(-5f, 0, 0); // 100배 증가
        if (Input.GetKey(KeyCode.D))
            p_Velocity += new Vector3(5f, 0, 0); // 100배 증가

        Vector3 p = p_Velocity;
        if (p.sqrMagnitude > 0)
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * 0.01f;

            p.x = Mathf.Clamp(p.x, -_inputSpeed, _inputSpeed);
            p.y = Mathf.Clamp(p.y, -_inputSpeed, _inputSpeed);
            p.z = Mathf.Clamp(p.z, -_inputSpeed, _inputSpeed);

            transform.Translate(p);
        }
    }

    private void TakeScreenshot()
    {
        string directory = "Assets/screenshot2/";
        string baseFileName = "screenshot";
        string extension = ".png";

        int fileIndex = 1;
        string filePath = Path.Combine(directory, baseFileName + fileIndex + extension);

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = Path.Combine(directory, baseFileName + fileIndex + extension);
        }

        // 스크린샷을 파일로 저장
        if (Input.GetKeyDown(KeyCode.F1))
            ScreenCapture.CaptureScreenshot(filePath);
        // Debug.Log($"Screenshot saved to: {filePath}");
    }
}
