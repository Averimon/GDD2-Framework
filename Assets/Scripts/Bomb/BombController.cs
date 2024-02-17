using System;
using UnityEngine;

namespace Framework.Bomb
{
    public class BombController : MonoBehaviour
    {
        // TODO: Remove bombPrefab and bombRechargeTime since they are now in PlayerRole
        public GameObject bombPrefab;
        public float bombRechargeTime;
        
        private float _activeRechargeTime;
        private bool _activeRecharge;

        public void OnEnable()
        {
            ResetCharge();
        }

        public void Update()
        {

            if (_activeRecharge)
            {
                _activeRechargeTime = Math.Clamp(_activeRechargeTime + Time.deltaTime, 0 , bombRechargeTime);

                if (_activeRechargeTime >= bombRechargeTime)
                {
                    ResetCharge();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DropBomb(transform.position);
                }
            }
        }

        private void ResetCharge()
        {
            _activeRechargeTime = 0f;
            _activeRecharge = false;
        }

        public void DropBomb(Vector3 dropPosition)
        {
            if (_activeRecharge) return;

            _activeRecharge = true;

            // Check if enough space

            Instantiate(bombPrefab, dropPosition, Quaternion.identity);
        }
    }
}
