using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgrade : MonoBehaviour
{
    GameObject _player;
    ShootingController _shootingController;
    HPController _hpController;

    public Button[] button;
    public Text[] text;

    int upgradeOptions = 6; // *** Always update when adding new upgrade ***

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _shootingController = _player.GetComponent<ShootingController>();
        _hpController = _player.GetComponent<HPController>();
        
        FindObjectOfType<GameUI>().Shuffle += Shuffle;
        Shuffle();
    }

    public void Shuffle()
    {
        int[] selected = new int[button.Length];
        int n = selected.Length;
        System.Random rand = new System.Random();
        for(int i=0; i<selected.Length; i++)
        {
            selected[i] = rand.Next(0, upgradeOptions);
            for(int j=0; j<i; j++)
            {
                if(selected[i] == selected[j])
                {
                    i--;
                    break;
                }
            }
        }

        for(int i=0; i<button.Length; i++)
        {
            button[i].onClick.RemoveAllListeners();
            switch(selected[i])
            {
                case 0:
                    text[i].text = "Rate of fire\n" + _shootingController.fireRate + "s -> " + (_shootingController.fireRate - 0.01f) + "s";
                    button[i].onClick.AddListener(FireRate);
                    break;
                case 1:
                    text[i].text = "Max ammo capacity\n" + _shootingController.maxAmmo + " -> " + (_shootingController.maxAmmo + 2);
                    button[i].onClick.AddListener(MaxAmmo);
                    break;
                case 2:
                    text[i].text = "Bullet damage\n" + _shootingController.bulletDmg + " -> " + (_shootingController.bulletDmg + 2);
                    button[i].onClick.AddListener(BulletDmg);
                    break;
                case 3:
                    text[i].text = "Max health\n" + _hpController.maxHealth + " -> " + (_hpController.maxHealth + 10);
                    button[i].onClick.AddListener(MaxHealth);
                    break;
                case 4:
                    text[i].text = "Block chance\n" + _hpController.blockChance * 100 + "% -> " + ((_hpController.blockChance + 0.02f) * 100) + "%";
                    button[i].onClick.AddListener(BlockChance);
                    break;
                case 5:
                    text[i].text = "Heal when switching cover\n +" + _hpController.healAmount + " -> +" + (_hpController.healAmount + 2);
                    button[i].onClick.AddListener(HealAmount);
                    break;
                /*
                case :
                    text[i].text = "\n" +  + " -> " + ;
                    button[i].onClick.AddListener();
                    break;
                */
            }
        }
    }

    void FireRate()
    {
        _shootingController.fireRate -= 0.01f;
    }

    void MaxAmmo()
    {
        _shootingController.maxAmmo += 2;
    }

    void BulletDmg()
    {
        _shootingController.bulletDmg += 2;
    }

    void MaxHealth()
    {
        _hpController.maxHealth += 10;
    }

    void BlockChance()
    {
        _hpController.blockChance += 0.02f;
    }

    void HealAmount()
    {
        _hpController.healAmount += 2;
    }
}
