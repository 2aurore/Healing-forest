using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public abstract class SceneBase : MonoBehaviour
    {
        public abstract IEnumerator OnStart();
        public abstract IEnumerator OnEnd();

    }
}
