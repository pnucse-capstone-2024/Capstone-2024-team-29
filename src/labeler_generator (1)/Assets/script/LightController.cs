using UnityEngine;

public class LightController : MonoBehaviour
{
    public CustomLight light1; // 필드를 public으로 변경

    public void CreateLight(float intensity, int clarity)
    {
        // Instantiate one directional light
        light1 = new CustomLight("light/DirectionalLight");

        // Set initial positions, rotations, intensities
        light1.SetPosition(0f, 150f, -100f);
        float randomXRotation = Random.Range(30f, 70f);  // Random X rotation between 30 and 70 degrees
        float randomYRotation = Random.Range(-40f, 0f);  // Random Y rotation between 0 and -40 degrees
        light1.SetRotation(randomXRotation, randomYRotation, 0f);  // Adjust rotation to direct the light
        light1.SetIntensity(intensity);  // 기본 밝기로 설정
        light1.EnableShadows();  // 그림자 활성화

        // light1.SetShadowStrength(0.8f); // 그림자 세기 조절(투명~완전 진하게)

        // UmbraSoftShadows의 SampleCount를 32로 설정
        light1.SetSampleCount(clarity);
    }

    public void RemoveLight()
    {
        if (light1 != null && light1.LightGameObject != null)
        {
            Object.Destroy(light1.LightGameObject);  // Destroy the light GameObject
            light1 = null;  // Clear the reference
        }
    }

    public class CustomLight
    {
        public GameObject LightGameObject { get; private set; }
        public UnityEngine.Light SceneLight { get; private set; }
        public Umbra.UmbraSoftShadows UmbraShadows { get; private set; }

        public CustomLight(string prefabPath)
        {
            // Load the light prefab from Resources
            GameObject lightPrefab = Resources.Load<GameObject>(prefabPath);

            if (lightPrefab == null)
            {
                Debug.LogError("Failed to load light prefab from Resources.");
                return;
            }

            // Instantiate the light game object
            LightGameObject = Object.Instantiate(lightPrefab, Vector3.zero, Quaternion.identity);

            // Get the Light component
            SceneLight = LightGameObject.GetComponent<UnityEngine.Light>();
            if (SceneLight == null)
            {
                Debug.LogError("Light component not found on the instantiated prefab.");
                return;
            }

            // Set the light type to Directional
            SceneLight.type = LightType.Directional;

            UmbraShadows = LightGameObject.AddComponent<Umbra.UmbraSoftShadows>();
            if (UmbraShadows != null)
            {
                UmbraShadows.profile = ScriptableObject.CreateInstance<Umbra.UmbraProfile>();
                UmbraShadows.profile.name = "New Umbra Profile";
                UmbraShadows.profile.ApplyPreset(Umbra.UmbraPreset.Blurred);

            }
        }

        // 그림자 강도 조절
        public void SetShadowStrength(float strength)
        {
            if (SceneLight != null)
            {
                SceneLight.shadowStrength = Mathf.Clamp01(strength);  // 그림자 강도를 0에서 1 사이로 설정
            }
        }

        public void SetSampleCount(int count)
        {
            if (UmbraShadows != null && UmbraShadows.profile != null)
            {
                UmbraShadows.profile.sampleCount = Mathf.Clamp(count, 1, 64); // 샘플 수를 1에서 64 사이로 제한
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            LightGameObject.transform.position = new Vector3(x, y, z);
        }

        public void SetRotation(float x, float y, float z)
        {
            LightGameObject.transform.rotation = Quaternion.Euler(x, y, z);
        }

        public void SetIntensity(float intensity)
        {
            if (SceneLight != null)
            {
                SceneLight.intensity = intensity;
            }
        }

        public void EnableShadows()
        {
            if (SceneLight != null)
            {
                SceneLight.shadows = LightShadows.Soft; // 그림자를 활성화 (Soft 선택)
            }
        }

        public void DisableShadows()
        {
            if (SceneLight != null)
            {
                SceneLight.shadows = LightShadows.None; // 그림자를 비활성화
            }
        }
    }
}
