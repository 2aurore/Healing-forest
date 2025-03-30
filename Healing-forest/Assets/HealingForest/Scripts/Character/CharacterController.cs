using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF
{
    public class CharacterController : MonoBehaviour
    {
        public CharacterBase linkedCharacter;

        private void Awake()
        {
            linkedCharacter = GetComponent<CharacterBase>();
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            // float mouseX = Input.GetAxis("Mouse X");
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // linkedCharacter.IsRunning = Input.GetKey(KeyCode.LeftShift);

            Vector2 input = new Vector2(horizontal, vertical);
            linkedCharacter.Move(input);
            // linkedCharacter.Rotate(mouseX);


        }
    }
}
