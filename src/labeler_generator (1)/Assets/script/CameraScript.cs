using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public ObjectLabeler objectLabeler;  // ObjectLabeler를 참조할 변수 추가
    public ObjectSpawner objectSpawner;  // ObjectSpawner 참조 추가
    public LightController lightController;
    public Camera segmentationCamera;  // Segmentation 카메라를 참조할 변수 추가
    public float delayBeforeScreenshot = 1.0f; // 스크린샷 전 대기 시간
    public float delayAfterScreenshot = 1.0f; // 스크린샷 후 대기 시간
    public int screenshotWidth = 640;  // 원하는 스크린샷 너비
    public int screenshotHeight = 640;  // 원하는 스크린샷 높이

    void Start()
    {
        // 스크린샷 디렉토리가 존재하지 않으면 생성
        string directory = "Assets/screenshot/";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Button button = GameObject.Find("Create").GetComponent<Button>();
        button.onClick.AddListener(StartMyCoroutine);
    }

    void StartMyCoroutine()
    {
        switchCanvas("Canvas", true);
        StartCoroutine(ProcessCombinations()); // 코루틴 시작
        switchCanvas("Canvas", false);
    }

    IEnumerator ProcessCombinations()
    {
        // Longi
        TMP_InputField longiNumLeft = GameObject.Find("Longi_Number_Left").GetComponent<TMP_InputField>(); // ok
        int longiLeftValue = ExtractValueForLongi(longiNumLeft);
        TMP_InputField longiNumRIght = GameObject.Find("Longi_Number_Right").GetComponent<TMP_InputField>(); // ok
        int longiRightValue = ExtractValueForLongi(longiNumRIght);
        TMP_Dropdown longiTypeLeft = GameObject.Find("Longi_Type_Left").GetComponent<TMP_Dropdown>(); // ok
        string longiTypeLeftValue = GetSelectedDropdownValue(longiTypeLeft);
        TMP_Dropdown longiTypeRight = GameObject.Find("Longi_Type_Right").GetComponent<TMP_Dropdown>(); // ok
        string longiTypeRightValue = GetSelectedDropdownValue(longiTypeRight);
        TMP_InputField longiDis = GameObject.Find("Longi_Distance_Value").GetComponent<TMP_InputField>(); // ok
        float longiDisValue = ExtractValueForLongiDis(longiDis);

        // Slot Hole
        TMP_Dropdown slotHoleType = GameObject.Find("Slot_Hole_Type_Value").GetComponent<TMP_Dropdown>(); // ok
        string slotHoleTypeValue = GetSelectedDropdownValue(slotHoleType);
        // TMP_Dropdown rHoleLocationInput = GameObject.Find("Slot_Hole_R_Hole_Value").GetComponent<TMP_Dropdown>(); // 
        // TMP_Dropdown rHoleRight = GameObject.Find("Slot_Hole_R_Hole_Right").GetComponent<TMP_Dropdown>(); 

        // Plate
        TMP_Dropdown plateType = GameObject.Find("Plate_Type_Value").GetComponent<TMP_Dropdown>(); // ok
        string plateTypeValue = GetSelectedDropdownValue(slotHoleType);
        //TMP_Dropdown plateLocation = GameObject.Find("Plate_Location_Value").GetComponent<TMP_Dropdown>(); // 
        //string plateStatusValue = GetDropdownValueAsInt(plateLocation);

        // Texture
        TMP_InputField longiNoiseInput = GameObject.Find("Longi_Noise_Value").GetComponent<TMP_InputField>();
        int longiNoise = ExtractValueForNoise(longiNoiseInput); // ok
        TMP_InputField plateNoiseInput = GameObject.Find("Plate_Noise_Value").GetComponent<TMP_InputField>();
        int plateNoise = ExtractValueForNoise(plateNoiseInput); // ok
        TMP_InputField floorNoiseInput = GameObject.Find("Floor_Noise_Value").GetComponent<TMP_InputField>();
        int floorNoise = ExtractValueForNoise(floorNoiseInput); // ok
        TMP_InputField bottomNoiseInput = GameObject.Find("Bottom_Noise_Value").GetComponent<TMP_InputField>();
        int bottomNoise = ExtractValueForNoise(bottomNoiseInput); // ok

        // Lighting
        TMP_InputField intensityInput = GameObject.Find("Intensity_Value").GetComponent<TMP_InputField>(); //
        float intensity = ExtractValueForIntensity(intensityInput); // ok

        // Shadow
        TMP_InputField clarityInput = GameObject.Find("Shadow_Clarity_Value").GetComponent<TMP_InputField>(); //
        int clarity = ExtractValueForClarity(clarityInput);

        // Repeat
        TMP_InputField repeatInput = GameObject.Find("Repeat_Value").GetComponent<TMP_InputField>(); // 
        int repeat = ConvertInputToInt(repeatInput);

        // particle
        TMP_InputField particleBrightnessInput = GameObject.Find("Brightness_Value").GetComponent<TMP_InputField>();
        int particleBrightness = ExtractValueForNoise(particleBrightnessInput);

        // scratch
        TMP_InputField scratchNoiseInput = GameObject.Find("Scratch_Strength_Value").GetComponent<TMP_InputField>();
        float scratchNoise = ExtractValueForScratch(scratchNoiseInput);

        // albedo
        TMP_InputField albedoNoiseInput = GameObject.Find("Albedo_Strength_Value").GetComponent<TMP_InputField>();
        float albedoNoise = ExtractValueForScratch(albedoNoiseInput);

        UnityEngine.Debug.LogError($"intensity{intensity} clarity{clarity} particleBrightness{particleBrightness} scratchNoise{scratchNoise} albedoNoise{albedoNoise}.");

        string[] longiSuffixes = { "LF", "LA", "LT" };
        string[] plateSuffixes = { "CP01", "CP02", "CP03", "CP04", "CP05", "CP06" };
        string[] slotHoleSuffixesA = { "AH", "AA", "AG", "AJ" };
        string[] slotHoleSuffixesT = { "TE", "TG" };
        float[] possibleCameraDisValues = { 65f, 70f, 75f };

        while(repeat > 0)
        {
            // longi1과 longi2 설정, 10% 확률로 다르게 설정
            // int Longi2 = Random.Range(0f, 1f) <= 0.1f ? Random.Range(8, 19) : Longi1;
            int Longi1 = longiLeftValue;
            int Longi2 = longiRightValue;

            // longiSuffix1과 longiSuffix2 설정, 10% 확률로 다르게 설정
            string longiSuffix1 = longiTypeLeftValue;
            // string longiSuffix2 = Random.Range(0f, 1f) <= 0.3f ? longiSuffixes[Random.Range(0, longiSuffixes.Length)] : longiSuffix1;
            string longiSuffix2 = longiTypeRightValue;

            foreach (float cameraDis in possibleCameraDisValues)
            {
                lightController.CreateLight(intensity, clarity);
                            
                string plateSuffix = plateTypeValue;
                string slotHoleSuffix = slotHoleTypeValue;


                // float longiDistance = Random.Range(600, 851) * 0.05F;
                float longiDistance = longiDisValue * 0.05F;

                // public void SpawnObjects(int longi1, int longi2, float longiDistance, string longiSuffix1, string longiSuffix2,
                // int slotHoleStatus, string slotHoleSuffix1, string slotHoleSuffix2,
                // int plateStatus, string plateSuffix, Quaternion plateRotation,
                // int r_holeStatus)

                // ObjectSpawner에 파라미터 전달하여 오브젝트 생성

                int plateStatus;
                int randomValue = Random.Range(0, 10); // 0부터 9까지의 난수 생성

                if (!(slotHoleSuffix == "AA" || slotHoleSuffix == "TE")) // 0~7 (80% 확률)
                {
                    plateStatus = Random.Range(1, 3); // 1 또는 2
                }
                else // 8~9 (20% 확률)
                {
                    plateStatus = Random.Range(3, 5); // 3 또는 4
                }

                int slotHoleStatus = 1;
                int r_holeStatus = 0;

                int particleStatus = Random.Range(0, 5);

                Quaternion plateRotation = Quaternion.identity;

                if (plateStatus == 1)
                {
                    if (plateSuffix == "CP01" || plateSuffix == "CP03" || plateSuffix == "CP05")
                    {
                        r_holeStatus = 0;
                    }
                    else if (slotHoleSuffix == "AJ" || slotHoleSuffix == "TG")
                    {
                        r_holeStatus = 3;
                    }
                    else
                    {
                        r_holeStatus = 1;
                    }

                    plateRotation = Quaternion.identity;
                }

                if (plateStatus == 2)
                {

                    if (plateSuffix == "CP01" || plateSuffix == "CP03" || plateSuffix == "CP05")
                    {
                        r_holeStatus = 0;
                    }
                    else if (slotHoleSuffix == "AJ" || slotHoleSuffix == "TG")
                    {
                        r_holeStatus = 3;
                    }
                    else
                    {
                        r_holeStatus = 2;
                    }

                    plateRotation = Quaternion.Euler(0, 180, 0);
                }

                if (plateStatus == 3)
                {
                    if (plateSuffix == "CP02" || plateSuffix == "CP04" || plateSuffix == "CP06")
                        r_holeStatus = 1;

                    plateRotation = Quaternion.identity;
                }

                if (plateStatus == 4)
                {
                    if (plateSuffix == "CP02" || plateSuffix == "CP04" || plateSuffix == "CP06")
                        r_holeStatus = 2;

                    plateRotation = Quaternion.Euler(0, 180, 0);
                }


                // 카메라 위치와 회전 설정
                // 카메라의 기본 높이 설정
                float cameraHeight = 44.5f;

                // 론지 값에 따른 카메라 높이 조정
                if ((Longi1 >= 1 && Longi1 <= 7) || (Longi1 >= 20 && Longi1 <= 26))
                {
                    cameraHeight = 22.5f;
                }

                if (Longi1 >= 8 && Longi1 <= 14)
                {
                    cameraHeight = 25.0f;
                }

                Vector3 cameraPosition = new Vector3(cameraDis, cameraHeight, 0);
                Vector3 cameraRotation = new Vector3(10, -90, 0);

                objectSpawner.SpawnObjects(Longi1, Longi2, longiDistance, longiSuffix1, longiSuffix2,
                slotHoleStatus, slotHoleSuffix, slotHoleSuffix,
                plateStatus, plateSuffix, plateRotation,
                r_holeStatus, particleStatus, cameraDis, cameraHeight, cameraPosition, cameraRotation,
                longiNoise, plateNoise, floorNoise, bottomNoise, particleBrightness, scratchNoise, albedoNoise);

                // 카메라 위치 설정
                SetCameraPosition(cameraDis, cameraHeight, cameraPosition, cameraRotation);

                // 스크린샷 찍기
                yield return new WaitForSeconds(delayBeforeScreenshot);
                string screenshotFilePath = TakeScreenshot(Longi1, Longi2, longiSuffix1, longiSuffix2, slotHoleSuffix, plateSuffix, cameraDis,
                    longiNoise, plateNoise, floorNoise, bottomNoise, particleBrightness, scratchNoise, albedoNoise);
                Debug.Log($"Screenshot saved to: {screenshotFilePath}");

                // 오브젝트 라벨링
                if (objectLabeler != null)
                {
                    objectLabeler.LabelObjects(screenshotFilePath);
                }
                else
                {
                    Debug.LogError("ObjectLabeler reference not set in CameraScript!");
                }

                // 텍스트 파일 생성 및 데이터 저장
                string textFilePath = Path.ChangeExtension(Path.Combine("Assets/parameter/", Path.GetFileName(screenshotFilePath)), ".txt");
                SaveDataToTxt(textFilePath, Longi1, Longi2, longiDistance, plateStatus, plateSuffix,
                    cameraDis, cameraHeight, cameraPosition, cameraRotation);

                // 마스크 생성 완료 후 삭제 대기
                yield return new WaitForSeconds(delayAfterScreenshot);

                // 생성된 오브젝트 삭제
                objectSpawner.DeleteSpawnedObjects();

                lightController.RemoveLight();

            }


            repeat = repeat - 1;    
        }

        //for (int Longi1 = 12; Longi1 <= 16; Longi1++)
        //{
        //    // longi1과 longi2 설정, 10% 확률로 다르게 설정
        //    // int Longi2 = Random.Range(0f, 1f) <= 0.1f ? Random.Range(8, 19) : Longi1;
        //    int Longi2 = Longi1;

        //    // longiSuffix1과 longiSuffix2 설정, 10% 확률로 다르게 설정
        //    string longiSuffix1 = longiSuffixes[Random.Range(0, longiSuffixes.Length)];
        //    // string longiSuffix2 = Random.Range(0f, 1f) <= 0.3f ? longiSuffixes[Random.Range(0, longiSuffixes.Length)] : longiSuffix1;
        //    string longiSuffix2 = longiSuffix1;


        //    // slotHoleSuffix를 longiSuffix에 따라 결정
        //    string[] selectedSlotHoleSuffixes = (longiSuffix1 == "LF" || longiSuffix1 == "LA") ? slotHoleSuffixesA : slotHoleSuffixesT;

        //    // plateSuffix와 slotHoleSuffix에서 두 개씩 무작위로 선택
        //    List<string> selectedPlateSuffixes = new List<string>(plateSuffixes);
        //    List<string> selectedSlotHoleSuffixesList = new List<string>(selectedSlotHoleSuffixes);
        //    ShuffleList(selectedPlateSuffixes);
        //    ShuffleList(selectedSlotHoleSuffixesList);

        //    string[] chosenPlateSuffixes = selectedPlateSuffixes.ToArray();
        //    string[] chosenSlotHoleSuffixes = selectedSlotHoleSuffixesList.ToArray();

        //    foreach (string plateSuffix in chosenPlateSuffixes)
        //    {
        //        foreach (string slotHoleSuffix in chosenSlotHoleSuffixes)
        //        {
        //            foreach (float cameraDis in possibleCameraDisValues)
        //            {
        //                lightController.CreateLight(intensity, clarity);

        //                float longiDistance = Random.Range(600, 851) * 0.05F;

        //                // public void SpawnObjects(int longi1, int longi2, float longiDistance, string longiSuffix1, string longiSuffix2,
        //                // int slotHoleStatus, string slotHoleSuffix1, string slotHoleSuffix2,
        //                // int plateStatus, string plateSuffix, Quaternion plateRotation,
        //                // int r_holeStatus)

        //                // ObjectSpawner에 파라미터 전달하여 오브젝트 생성

        //                int plateStatus;
        //                int randomValue = Random.Range(0, 10); // 0부터 9까지의 난수 생성

        //                if (!(slotHoleSuffix == "AA" || slotHoleSuffix == "TE")) // 0~7 (80% 확률)
        //                {
        //                    plateStatus = Random.Range(1, 3); // 1 또는 2
        //                }
        //                else // 8~9 (20% 확률)
        //                {
        //                    plateStatus = Random.Range(3, 5); // 3 또는 4
        //                }

        //                int slotHoleStatus = 1;
        //                int r_holeStatus = 0;

        //                int particleStatus = 0;

        //                Quaternion plateRotation = Quaternion.identity;

        //                if (plateStatus == 1)
        //                {
        //                    if (plateSuffix == "CP01" || plateSuffix == "CP03" || plateSuffix == "CP05")
        //                    {
        //                        r_holeStatus = 0;
        //                    }
        //                    else if (slotHoleSuffix == "AJ" || slotHoleSuffix == "TG")
        //                    {
        //                        r_holeStatus = 3;
        //                    }
        //                    else
        //                    {
        //                        r_holeStatus = 1;
        //                    }

        //                    plateRotation = Quaternion.identity;
        //                }

        //                if (plateStatus == 2)
        //                {

        //                    if (plateSuffix == "CP01" || plateSuffix == "CP03" || plateSuffix == "CP05")
        //                    {
        //                        r_holeStatus = 0;
        //                    }
        //                    else if (slotHoleSuffix == "AJ" || slotHoleSuffix == "TG")
        //                    {
        //                        r_holeStatus = 3;
        //                    }
        //                    else
        //                    {
        //                        r_holeStatus = 2;
        //                    }

        //                    plateRotation = Quaternion.Euler(0, 180, 0);
        //                }

        //                if (plateStatus == 3)
        //                {
        //                    if (plateSuffix == "CP02" || plateSuffix == "CP04" || plateSuffix == "CP06")
        //                        r_holeStatus = 1;

        //                    plateRotation = Quaternion.identity;
        //                }

        //                if (plateStatus == 4)
        //                {
        //                    if (plateSuffix == "CP02" || plateSuffix == "CP04" || plateSuffix == "CP06")
        //                        r_holeStatus = 2;

        //                    plateRotation = Quaternion.Euler(0, 180, 0);
        //                }


        //                // 카메라 위치와 회전 설정
        //                // 카메라의 기본 높이 설정
        //                float cameraHeight = 44.5f;

        //                // 론지 값에 따른 카메라 높이 조정
        //                if ((Longi1 >= 1 && Longi1 <= 7) || (Longi1 >= 20 && Longi1 <= 26))
        //                {
        //                    cameraHeight = 22.5f;
        //                }

        //                if (Longi1 >= 8 && Longi1 <= 14)
        //                {
        //                    cameraHeight = 25.0f;
        //                }

        //                Vector3 cameraPosition = new Vector3(cameraDis, cameraHeight, 0);
        //                Vector3 cameraRotation = new Vector3(10, -90, 0);

        //                objectSpawner.SpawnObjects(Longi1, Longi2, longiDistance, longiSuffix1, longiSuffix2,
        //                slotHoleStatus, slotHoleSuffix, slotHoleSuffix,
        //                plateStatus, plateSuffix, plateRotation,
        //                r_holeStatus, particleStatus, cameraDis, cameraHeight, cameraPosition, cameraRotation,
        //                longiNoise, plateNoise, floorNoise, bottomNoise, particleBrightness, scratchNoise, albedoNoise);

        //                // 카메라 위치 설정
        //                SetCameraPosition(cameraDis, cameraHeight, cameraPosition, cameraRotation);

        //                // 스크린샷 찍기
        //                yield return new WaitForSeconds(delayBeforeScreenshot);
        //                string screenshotFilePath = TakeScreenshot(Longi1, Longi2, longiSuffix1, longiSuffix2, slotHoleSuffix, plateSuffix, cameraDis, 
        //                    longiNoise, plateNoise, floorNoise, bottomNoise, particleBrightness, scratchNoise, albedoNoise);
        //                Debug.Log($"Screenshot saved to: {screenshotFilePath}");

        //                // 오브젝트 라벨링
        //                if (objectLabeler != null)
        //                {
        //                    objectLabeler.LabelObjects(screenshotFilePath);
        //                }
        //                else
        //                {
        //                    Debug.LogError("ObjectLabeler reference not set in CameraScript!");
        //                }

        //                // 텍스트 파일 생성 및 데이터 저장
        //                string textFilePath = Path.ChangeExtension(Path.Combine("Assets/parameter/", Path.GetFileName(screenshotFilePath)), ".txt");
        //                SaveDataToTxt(textFilePath, Longi1, Longi2, longiDistance, plateStatus, plateSuffix,
        //                    cameraDis, cameraHeight, cameraPosition, cameraRotation);

        //                // 마스크 생성 완료 후 삭제 대기
        //                yield return new WaitForSeconds(delayAfterScreenshot);

        //                // 생성된 오브젝트 삭제
        //                objectSpawner.DeleteSpawnedObjects();

        //                lightController.RemoveLight();

        //            }
        //        }
        //    }
        //}

        Debug.Log("All combinations processed.");
    }

    void SetCameraPosition(float cameraDis, float cameraHeight, Vector3 cameraPosition, Vector3 cameraRotation)
    {

        Camera.main.transform.position = cameraPosition;
        Camera.main.transform.eulerAngles = cameraRotation;
        Camera.main.nearClipPlane = 0.1f;

        // Segmentation 카메라가 설정되어 있다면 메인 카메라와 동일한 위치와 회전으로 설정
        if (segmentationCamera != null)
        {
            segmentationCamera.transform.position = cameraPosition;
            segmentationCamera.transform.eulerAngles = cameraRotation;
        }
        else
        {
            Debug.LogError("Segmentation Camera reference not set in CameraScript!");
        }
    }

    string TakeScreenshot(int longi1, int longi2, string longiSuffix1, string longiSuffix2, string slotHoleSuffix, string plateSuffix, float cameraDis, 
        int longiNoise, int plateNoise, int floorNoise, int bottomNoise, float particleBrightness, float scratchNoise, float albedoNoise)
    {
        string directory = "Assets/screenshot/";
        string baseFileName = $"screenshot_{longi1}_{longi2}_{longiSuffix1}_{longiSuffix2}_{slotHoleSuffix}_{plateSuffix}_{cameraDis}_{longiNoise}_{plateNoise}_{floorNoise}_{bottomNoise}_{particleBrightness}_{scratchNoise}_{albedoNoise}";
        string extension = ".png";

        string filePath = Path.Combine(directory, baseFileName + extension);

        int fileIndex = 1;
        while (File.Exists(filePath))
        {
            filePath = Path.Combine(directory, baseFileName + "_" + fileIndex + extension);
            fileIndex++;
        }

        // 원하는 크기의 RenderTexture 생성
        //RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        //Camera.main.targetTexture = rt;
        //Camera.main.Render();

        //RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);
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
            new Rect(0, 0, screenshotWidth, screenshotHeight),
            0, 0);
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        if (transformedRenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(transformedRenderTexture);
        }

        screenShot.Apply();
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        //Destroy(rt);

        // 파일로 저장
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        return filePath;
    }

    void SaveDataToTxt(string filePath, int longi1, int longi2, float longiDistance, int plateStatus, string plateSuffix,
        float cameraDis, float cameraHeight, Vector3 cameraPosition, Vector3 cameraRotation)
    {
        // 파일의 디렉터리 경로를 추출
        string directory = Path.GetDirectoryName(filePath);

        // 디렉터리가 존재하지 않으면 생성
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // ObjectSpawner의 MakeTxt 메서드를 호출하여 데이터 작성
            // MakeTxt(StreamWriter writer, int longi1, int longi2, float longiDistance, int plateStatus, string plateSuffix)
            objectSpawner.MakeTxt(writer, longi1, longi2, longiDistance, plateStatus, plateSuffix,
                cameraDis, cameraHeight, cameraPosition, cameraRotation);
        }
    }

    // 리스트 섞기 함수 추가
    void ShuffleList<T>(List<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            int k = Random.Range(0, count);
            T value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
    }

    private void switchCanvas(string canvasName, bool enabled)
    {
        // 지정한 이름으로 Canvas를 찾습니다.
        GameObject canvasObject = GameObject.Find(canvasName);

        if (canvasObject != null)
        {
            // Canvas 컴포넌트를 가져옵니다.
            Canvas canvas = canvasObject.GetComponent<Canvas>();

            if (canvas != null)
            {
                // Canvas를 비활성화합니다.
                canvas.enabled = enabled;
                UnityEngine.Debug.Log($"{canvasName}.");
            }
            else
            {
                UnityEngine.Debug.LogError($"No Canvas component found on {canvasName}.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"No GameObject found with the name {canvasName}.");
        }
    }

    private int ConvertInputToInt(TMP_InputField inputField)
    {
        int result;
        if (int.TryParse(inputField.text, out result))
        {
            return result;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Input is not a valid int: " + inputField.text);
            return -1;
        }
    }

    private float ConvertInputToFloat(TMP_InputField inputField)
    {
        float result;
        if (float.TryParse(inputField.text, out result))
        {
            return result;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Input is not a valid float: " + inputField.text);
            return 50f;
        }
    }

    private string GetSelectedDropdownValue(TMP_Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }

    private int GetDropdownValueAsInt(TMP_Dropdown dropdown)
    {
        string selectedOptionText = dropdown.options[dropdown.value].text;
        if (int.TryParse(selectedOptionText, out int result))
        {
            return result; // 변환 가능하면 값 반환
        }
        return 0; // 변환 불가능하면 기본값 0 반환
    }

    private float ExtractValueForLongiDis(TMP_InputField inputField)
    {
        float value;
        value = ConvertInputToFloat(inputField);

        if (value >= 600f && value <= 850f)
        {
            return value;
        }

        return 600f;
    }

    private int ExtractValueForLongi(TMP_InputField inputField)
    {
        int value;
        value = ConvertInputToInt(inputField);

        if (value >= 1 && value <= 26)
        {
            return value;
        }

        return 1;
    }

    private int ExtractValueForScratch(TMP_InputField inputField)
    {
        int value;
        value = ConvertInputToInt(inputField);

        if (value >= 0 && value <= 100)
        {
            return value;
        }

        return 0;
    }

    private int ExtractValueForNoise(TMP_InputField inputField)
    {
        int value;
        value = ConvertInputToInt(inputField);

        if (value >= 0 && value <=100)
        {
            return value;
        }

        return 0;
    }

    private float ExtractValueForIntensity(TMP_InputField inputField)
    {
        float value;
        value = ConvertInputToFloat(inputField);

        float minValue = 0.3f;
        float maxValue = 3f;

        // 입력값이 0~100 범위 내에 있는지 검사하고 벗어나면 50으로 설정
        if (value < 0f || value > 100f)
        {
            value = 50f; // 범위를 벗어나면 50으로 맞춤
        }

        // 0~100 사이의 값을 0.3~3 범위로 변환
        float mappedValue = Mathf.Lerp(minValue, maxValue, value / 100f);

        return mappedValue; ;
    }

    private int ExtractValueForClarity(TMP_InputField inputField)
    {
        int value;
        value = ConvertInputToInt(inputField);

        int minValue = 1;
        int maxValue = 64;

        // 입력값이 0~100 범위 내에 있는지 검사하고, 벗어나면 50으로 설정
        if (value < 0 || value > 100)
        {
            value = 50; // 범위를 벗어나면 50으로 설정
        }

        // 0~100 사이의 값을 1~64 범위로 변환 (반올림하여 정수로 반환)
        float t = value / 100f; // 0과 1 사이의 비율로 변환
        int mappedValue = Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, t));

        return mappedValue;
    }
}