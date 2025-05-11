using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HF
{
    public class Inventory_Item : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Image eqquippedImage;
        [SerializeField] private Sprite defaultImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemCount;

        public void Initialize(Sprite itemSprite, int count)
        {
            itemImage.sprite = itemSprite;
            itemCount.text = count.ToString();
        }
    }
}
