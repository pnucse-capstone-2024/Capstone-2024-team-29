using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectLabeler : MonoBehaviour
{
    public Camera segmentationCamera;  // Segmentation 마스크를 위한 별도의 카메라
    public string outputDirectory = "Assets/labeledScreenshot/";
    public int imageWidth = 640;
    public int imageHeight = 640;

    public float preRenderDelay = 0.1f;  // 색상 변경 후 대기 시간
    public float postRenderDelay = 0.1f;  // 스크린샷 후 대기 시간

    public List<GameObject> objectsToLabel = new List<GameObject>(); // 라벨링할 객체들을 담는 리스트

    private Dictionary<string, Color> objectColorMapping = new Dictionary<string, Color>
    {
        { "longiBox", new Color(1.0f, 0.0f, 0.0f) },    // 빨간색 (R = 1.0, G = 0.0, B = 0.0)
        { "floorBox", new Color(0.0f, 0.0f, 1.0f) },    // 파란색 (R = 0.0, G = 0.0, B = 1.0)
        { "r_hole", new Color(1.0f, 0.5f, 0.0f) },      // 오렌지색 (R = 1.0, G = 0.5, B = 0.0)
        { "plate_1", new Color(0.0f, 1.0f, 0.0f) },     // 초록색 (R = 0.0, G = 1.0, B = 0.0)
        { "plate_2", new Color(1.0f, 1.0f, 0.0f) },     // 노란색 (R = 1.0, G = 1.0, B = 0.0)
        { "plate_3", new Color(1.0f, 0.0f, 1.0f) },     // 마젠타색 (R = 1.0, G = 0.0, B = 1.0)
        { "plate_back", new Color(0.0f, 1.0f, 1.0f) },  // 시안색 (R = 0.0, G = 1.0, B = 1.0)
    };

    public LightController lightController; // LightController를 참조하는 변수 추가

    public void LabelObjects(string screenshotFilePath)
    {
        // 원본 이미지 촬영 후 텍스처와 조명을 제거한 상태에서 마스크 이미지를 촬영하도록 설정
        string directory = outputDirectory;
        string fileName = Path.GetFileNameWithoutExtension(screenshotFilePath) + ".png";
        string maskFilePath = Path.Combine(directory, fileName);

        // 텍스처 제거 및 조명 재설정
        RemoveTexturesAndAdjustLighting();

        // slotHole과 r_hole의 위치 조정
        AdjustObjectPositions();

        // 마스크 생성
        StartCoroutine(RenderFullMask(maskFilePath));
    }

    void GenerateSegmentationMasksAndLabels()
    {
        foreach (KeyValuePair<string, Color> entry in objectColorMapping)
        {
            string tag = entry.Key;
            Color color = entry.Value;

            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag); // 태그로 객체를 식별

            if (objects.Length == 0)
            {
                Debug.LogWarning($"No objects found with tag: {tag}");
                continue;
            }

            SetObjectColors(objects, color);

            // 라벨링할 객체 리스트에 추가
            objectsToLabel.AddRange(objects);
        }
    }

    void SetObjectColors(GameObject[] objects, Color color)
    {
        foreach (GameObject obj in objects)
        {

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
                renderer.material = new Material(Shader.Find("Unlit/Color")); // 새로운 단일 색상 머티리얼 설정
                renderer.material.color = color; // 색상 설정
            }
        }
    }

    void RemoveTexturesAndAdjustLighting()
    {
        // 조명 설정을 변경하여 그림자를 제거하고 밝기를 높임
        if (lightController != null)
        {
            lightController.light1.DisableShadows();
            lightController.light1.SetIntensity(2.5f);  // 세그멘테이션 마스크를 위한 밝기 조정
        }
    }

    public void AdjustObjectPositions()
    {
        bool hasPlateBack = false;

        // plate_back 태그가 있는지 먼저 확인
        for (int i = objectsToLabel.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsToLabel[i];

            if (obj != null && obj.CompareTag("plate_back"))
            {
                hasPlateBack = true;
                break;
            }
        }

        // slotHole과 r_hole의 위치를 약간 앞으로 이동 (plate_back 존재 시 r_hole 음수 이동)
        for (int i = objectsToLabel.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsToLabel[i];

            if (obj == null)
            {
                // null 참조를 방지하기 위해 null인 오브젝트는 리스트에서 제거
                objectsToLabel.RemoveAt(i);
                continue;
            }

            if (obj.CompareTag("slotHole"))
            {
                obj.transform.position += new Vector3(1.6f, 0, 0); // x축으로 1.6만큼 이동
            }

            if (obj.CompareTag("r_hole"))
            {
                // plate_back이 존재하면 음수 이동, 그렇지 않으면 양수 이동
                if (hasPlateBack)
                {
                    obj.transform.position += new Vector3(-1f, 0, 0); // x축으로 -1.6만큼 이동
                }
                else
                {
                    obj.transform.position += new Vector3(1.6f, 0, 0); // x축으로 1.6만큼 이동
                }
            }
        }
    }


    IEnumerator RenderFullMask(string maskFilePath)
    {
        // 색상 변경 및 조명 조정 후 일정 시간 대기
        GenerateSegmentationMasksAndLabels();
        yield return new WaitForSeconds(preRenderDelay);

        // // RenderTexture 생성 및 카메라에 할당
        // RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        // segmentationCamera.targetTexture = renderTexture;
        // segmentationCamera.Render();

        // // 카메라 렌더링이 완료되었는지 확인한 후 스크린샷 촬영
        // yield return new WaitForEndOfFrame();

        // // RenderTexture를 텍스처로 변환
        // RenderTexture.active = renderTexture;
        // Texture2D maskTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        // maskTexture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        // maskTexture.Apply();
        // RenderTexture.active = null;

        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        //screenShot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
        //screenShot.Apply();
        //Post-Processing을 적용시킨 상태로 스크린샷 촬영(유니티포럼에서 긁어옴)
        RenderTexture transformedRenderTexture = null;
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            Screen.width,
            Screen.height,
            24,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Default,
            1);

        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
        transformedRenderTexture = RenderTexture.GetTemporary(
            screenShot.width,
            screenShot.height,
            24,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Default,
            1);
        Graphics.Blit(
            renderTexture,
            transformedRenderTexture,
            new Vector2(1.0f, -1.0f),
            new Vector2(0.0f, 1.0f));
        RenderTexture.active = transformedRenderTexture;
        screenShot.ReadPixels(
            new Rect(0, 0, screenShot.width, screenShot.height),
            0, 0);
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        if (transformedRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(transformedRenderTexture);
        }

        screenShot.Apply();

        // 마스크 이미지 저장 (스크린샷 파일 이름과 동일하게 저장)
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(maskFilePath, bytes);
        Debug.Log($"Saved segmentation mask to {maskFilePath}");

        // Clean up
        segmentationCamera.targetTexture = null;
        RenderTexture.active = null;

        yield return new WaitForSeconds(postRenderDelay); // 스크린샷 후 대기
    }
}

