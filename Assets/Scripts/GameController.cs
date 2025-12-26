using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class CountryData
    {
        public string countryName;
        public string flagName;
        public string slotName;
        public GameObject landmarkPrefab;
    }
    
    public List<CountryData> countries = new List<CountryData>();
    public GameObject landmarkSpawnPoint;
    public TextMeshProUGUI feedbackText;
    public GameObject restartButton;
    
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip winSound;
    
    public GameObject sights_Jordan;
    public GameObject sights_Egypt;
    public GameObject sights_Germany;
    public GameObject sights_Peru;
    
    private int currentLandmarkIndex = 0;
    private CountryData currentCountry;
    private GameObject currentLandmarkInstance;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        
        countries.Add(new CountryData { 
            countryName = "Jordan", 
            flagName = "Flag_JO", 
            slotName = "Slot_JO",
            landmarkPrefab = sights_Jordan
        });
        
        countries.Add(new CountryData { 
            countryName = "Egypt", 
            flagName = "Flag_EG", 
            slotName = "Slot_EG",
            landmarkPrefab = sights_Egypt
        });
        
        countries.Add(new CountryData { 
            countryName = "Germany", 
            flagName = "Flag_DE", 
            slotName = "Slot_DE",
            landmarkPrefab = sights_Germany
        });
        
        countries.Add(new CountryData { 
            countryName = "Peru", 
            flagName = "Flag_PE", 
            slotName = "Slot_PE",
            landmarkPrefab = sights_Peru
        });
        
        SpawnNextLandmark();
    }
    
    void SpawnNextLandmark()
    {
        if (currentLandmarkInstance != null)
        {
            Destroy(currentLandmarkInstance);
        }
        
        if (currentLandmarkIndex >= countries.Count)
        {
            Debug.Log("Oyun bitti! Tüm eşleşmeler tamamlandı!");
            ShowFeedback("OYUN BITTI! TEBRIKLER!", Color.yellow);
            
            if (restartButton != null)
                restartButton.SetActive(true);
            
            return;
        }
        
        currentCountry = countries[currentLandmarkIndex];
        
        if (currentCountry.landmarkPrefab != null && landmarkSpawnPoint != null)
        {
            currentLandmarkInstance = Instantiate(
                currentCountry.landmarkPrefab, 
                landmarkSpawnPoint.transform.position, 
                landmarkSpawnPoint.transform.rotation
            );
            currentLandmarkInstance.AddComponent<LandmarkController>();

            Debug.Log("Şu ülke için bayrak bul: " + currentCountry.countryName);
        }
    }
    
    public bool CheckMatch(string flagName, string slotName)
    {
        Debug.Log($"Gelen Flag: '{flagName}' | Beklenen: '{currentCountry.flagName}'");
        Debug.Log($"Gelen Slot: '{slotName}' | Beklenen: '{currentCountry.slotName}'");
        
        bool isCorrect = (flagName == currentCountry.flagName && slotName == currentCountry.slotName);
        
        if (isCorrect)
        {
            Debug.Log("DOĞRU! " + currentCountry.countryName + " bayrağı doğru slota yerleşti!");
            ShowFeedback("DOĞRU!", Color.green);
            currentLandmarkIndex++;
            Invoke("SpawnNextLandmark", 1.5f); 
        }
        else
        {
            Debug.Log("YANLIŞ! Tekrar dene.");
            ShowFeedback("YANLIŞ!", Color.red);
        }
        
        return isCorrect;
    }
    
    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);
            
            if (audioSource != null)
            {
                if (message.Contains("DOĞRU"))
                    audioSource.PlayOneShot(correctSound);
                else if (message.Contains("YANLIŞ"))
                    audioSource.PlayOneShot(wrongSound);
                else if (message.Contains("TEBRIKLER"))
                    audioSource.PlayOneShot(winSound);
            }
            
            Invoke("HideFeedback", 1f);
        }
    }
    
    void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }
}