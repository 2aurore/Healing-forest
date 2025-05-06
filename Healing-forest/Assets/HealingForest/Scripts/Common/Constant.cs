using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class Constant
    {

    }

    // 아이템 카테고리를 위한 enum
    public enum ItemCategory
    {
        Material,   // 재료 아이템
        Crafting,   // 제작 아이템
        Equipment   // 장비 아이템
    }

    public enum ToolType
    {
        None = 0,
        Axe,
        Shovel,
        FishingRod,
        Net,

        End,
    }

    public enum InteractionType
    {
        Chop,
        Hit,
        Dig,
        Fish
    }
}
