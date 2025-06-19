using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public interface IState
    {
        void Enter(AIBrain brain);
        void Exit(AIBrain brain);
        void Update(AIBrain brain);
    }
}
