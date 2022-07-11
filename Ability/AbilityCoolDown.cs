using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCoolDown : MonoBehaviour {

    public string abilityButtonAxisName;
    public Image darkMask;
    public Text coolDownTextDisplay;

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject weaponHolder;
    private Image myButtonImage;
    private AudioSource abilitySource;
    private float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;
    Coroutine coolDown;

    void Start () 
    {
        Initialize (ability, weaponHolder);    
    }

    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
        myButtonImage = GetComponent<Image> ();
        abilitySource = GetComponent<AudioSource> ();
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        coolDownDuration = ability.aBaseCoolDown;
        ability.Initialize (weaponHolder);
        AbilityReady ();
    }

    // Update is called once per frame
    void Update () 
    {
        bool coolDownComplete = (Time.time > nextReadyTime);
        if (coolDownComplete) 
        {
            if(coolDown != null)
            {
                StopCoroutine(coolDown);
                coolDown = null;
            }
            AbilityReady ();
            if (Input.GetButtonDown (abilityButtonAxisName)) 
            {
                ButtonTriggered ();
            }
        } else if(coolDown == null)
        {
            coolDown = StartCoroutine(CoolDown());
        }
    }

    private void AbilityReady()
    {
        coolDownTextDisplay.text = abilityButtonAxisName;
        darkMask.enabled = false;
    }

    private IEnumerator CoolDown()
    {
        while(coolDownTimeLeft > 0)
        {
            coolDownTimeLeft -= Time.deltaTime;
            float roundedCd = Mathf.Round (coolDownTimeLeft);
            coolDownTextDisplay.text = roundedCd.ToString ();
            darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
            yield return null;
        }
    }

    private void ButtonTriggered()
    {
        nextReadyTime = coolDownDuration + Time.time;
        coolDownTimeLeft = coolDownDuration;
        darkMask.enabled = true;

        AudioManager.instance.PlaySound(ability.aSound, weaponHolder.transform.root.position);
        ability.TriggerAbility ();
    }
}