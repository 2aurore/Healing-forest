using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // 이 속성은 Unity 에디터에서 해당 필드를 읽기 전용으로 표시합니다.
        // 실제로는 아무런 동작을 하지 않지만, 에디터에서 시각적으로 구분할 수 있게 합니다.
    }
}
