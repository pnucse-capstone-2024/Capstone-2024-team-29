using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelScreenShot : MonoBehaviour
{
    // Suffixes
    string[] longiSuffixes = { "LF", "LA", "LT" };
    string[] plateSuffixes = { "CP01", "CP02", "CP03", "CP04", "CP05", "CP06" };
    string[] slotHoleSuffixesA = { "AH", "AA", "AG", "AJ" };
    string[] slotHoleSuffixesT = { "TE", "TG" };

    private Camera mainCamera;

    void Start()
    {
        float randomDis = Random.Range(700, 1201) * 0.1F;
        Vector3 cameraPosition = new Vector3(randomDis, 44.5f, 0);
        Vector3 cameraRotation = new Vector3(18, -90, 0);

        // 메인 카메라 위치와 회전 설정
        Camera.main.transform.position = cameraPosition;
        Camera.main.transform.eulerAngles = cameraRotation;
        Camera.main.nearClipPlane = 0.1f;

        StartCoroutine(SpawnAndCapture());
    }

    IEnumerator SpawnAndCapture()
    {
        for (int currentIndex = 1; currentIndex <= 26; currentIndex++)
        {
            yield return StartCoroutine(SpawnAndCaptureModel(currentIndex, "longi"));
            yield return StartCoroutine(SpawnAndCaptureModel(currentIndex, "slot_hole"));
            yield return StartCoroutine(SpawnAndCaptureModel(currentIndex, "plate"));
        }
    }

    IEnumerator SpawnAndCaptureModel(int index, string modelType)
    {
        // Suffixes 선택
        string randomSuffix = "";
        string[] selectedSuffixes = null;
        string modelPath = "";

        switch (modelType)
        {
            case "longi":
                selectedSuffixes = longiSuffixes;
                randomSuffix = longiSuffixes[Random.Range(0, longiSuffixes.Length)];
                modelPath = $"longi/longi_{index}{randomSuffix}";
                break;
            case "slot_hole":
                selectedSuffixes = (randomSuffix == "LF" || randomSuffix == "LA") ? slotHoleSuffixesA : slotHoleSuffixesT;
                randomSuffix = selectedSuffixes[Random.Range(0, selectedSuffixes.Length)];
                modelPath = $"slot_hole/slot_hole_{index}{randomSuffix}";
                break;
            case "plate":
                selectedSuffixes = plateSuffixes;
                randomSuffix = plateSuffixes[Random.Range(0, plateSuffixes.Length)];
                modelPath = $"plate/plate{index}_{randomSuffix}";
                break;
        }

        // Resources 폴더에서 모델을 로드
        GameObject model = Resources.Load<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogError($"Model not found: {modelPath}");
            yield break;
        }

        // 원하는 좌표에 오브젝트를 생성
        Vector3 spawnPosition = new Vector3(0, 0, 0);
        Quaternion spawnRotation = Quaternion.identity;
        GameObject instance = Instantiate(model, spawnPosition, spawnRotation);
        instance.transform.localScale = Vector3.one * 100; // Set the scale to 100 uniformly

        // Ensure the model does not rotate unexpectedly
        if (instance.GetComponent<Rigidbody>())
        {
            instance.GetComponent<Rigidbody>().freezeRotation = true;
        }

        // Material 및 Texture 설정
        Material material = new Material(Shader.Find("Custom/FloorShader"));
        Texture2D texture = Resources.Load<Texture2D>($"Textures/{modelType}1");
        material.mainTexture = texture;

        SetTiling(material, 2.0f, 3.6f, 0.0f, 0.07f); // 예시 값을 사용해 타일링 설정

        ApplyMaterial(instance, material);
        SetLayer(instance, 6);

        // 카메라가 스크린샷을 찍도록 신호
        yield return new WaitForSeconds(1);  // 잠시 대기 (카메라가 설정될 시간 확보)
        TakeScreenshot($"{modelType}_{index}");

        // 오브젝트 삭제
        Destroy(instance);
    }

    void ApplyMaterial(GameObject obj, Material mat)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
        }
        else
        {
            foreach (Transform child in obj.transform)
            {
                ApplyMaterial(child.gameObject, mat);
            }
        }
    }

    void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    void SetTiling(Material mat, float tilingX, float tilingY, float offsetX, float offsetY)
    {
        if (mat.shader.name.Contains("Universal Render Pipeline"))
        {
            mat.SetFloat("_BaseMap_ST_X", tilingX);
            mat.SetFloat("_BaseMap_ST_Y", tilingY);
            mat.SetFloat("_BaseMap_ST_Z", tilingX);
            mat.SetFloat("_BaseMap_ST_W", tilingY);
        }
        else
        {
            mat.mainTextureScale = new Vector2(tilingX, tilingY);
            mat.mainTextureOffset = new Vector2(offsetX, offsetY);
        }
    }

    private void TakeScreenshot(string filename)
    {
        string directory = "Assets/screenshot2/";
        string extension = ".png";
        string filePath = Path.Combine(directory, filename + extension);

        // 스크린샷을 파일로 저장
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"Screenshot saved to: {filePath}");
    }
}
