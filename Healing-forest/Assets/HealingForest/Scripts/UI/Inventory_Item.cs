using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Inventory_Item : MonoBehaviour
    {

        [SerializeField] public string ItemID { get; private set; } // 아이템 ID
        [SerializeField] private Image itemImage;
        [SerializeField] private Image eqquippedImage;
        [SerializeField] private Sprite defaultImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemCount;


        public void Initialize(string itemId, int count, Sprite itemSprite)
        {
            itemImage.sprite = itemSprite;
            itemCount.text = count.ToString();
            ItemID = itemId;
        }

        public void UpdateEqquippedImage(string itemId)
        {
            // 현재 장착된 아이템에만 equippedImage를 활성화
            if (UserDataModel.Singleton.CurrentEquipItemID == itemId)
            {
                eqquippedImage.gameObject.SetActive(true);
            }
            else
            {
                eqquippedImage.gameObject.SetActive(false);
            }
        }

        public void UpdateItemCount(int count)
        {
            itemCount.text = count.ToString();
        }
    }
}
