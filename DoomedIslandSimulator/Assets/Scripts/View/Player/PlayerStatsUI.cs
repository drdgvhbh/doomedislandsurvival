using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour {
    [SerializeField]
    private GameObject HealthBar;
    [SerializeField]
    private GameObject HealthText;
    [SerializeField]
    private GameObject StaminaBar;
    [SerializeField]
    private GameObject StaminaText;
    [SerializeField]
    private GameObject Nourishment;
    [SerializeField]
    private GameObject NourishmentText;

    [SerializeField]
    private GameObject NourishmentLevelText;



    private PlayerData Pd;

    private void Awake() {
        Pd = GetComponent<PlayerData>();
        HealthBar.GetComponent<Image>().fillAmount = Pd.Health / Pd.MaximumHealth;
        HealthText.GetComponent<TMPro.TextMeshProUGUI>().text = System.Math.Ceiling(Pd.Health) + " / " + Pd.MaximumHealth;
        StaminaBar.GetComponent<Image>().fillAmount = Pd.Stamina / Pd.MaximumStamina;
        StaminaText.GetComponent<TMPro.TextMeshProUGUI>().text = System.Math.Ceiling(Pd.Stamina) + " / " + Pd.MaximumStamina;
        Nourishment.GetComponent<Image>().fillAmount = Pd.Nourishment / Pd.NourishmentThres;
        NourishmentText.GetComponent<TMPro.TextMeshProUGUI>().text = System.Math.Ceiling(Pd.Nourishment) + " / " + Pd.NourishmentThres;
        NourishmentLevelText.GetComponent<TMPro.TextMeshProUGUI>().text = Pd.NourishmentLevel.ToString();
    }



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
