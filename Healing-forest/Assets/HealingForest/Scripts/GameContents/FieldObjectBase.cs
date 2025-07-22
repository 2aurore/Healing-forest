using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class FieldObjectBase : MonoBehaviour
    {
        protected virtual void Start()
        {
            TileMapManager.Instance.RegistObejctToObjectMap(this.gameObject, transform.position);
        }
    }
}
