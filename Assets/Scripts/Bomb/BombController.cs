using System;
using UnityEngine;

namespace Framework.Bomb
{
    
    public class BombController : MonoBehaviour
    {
        [SerializeField] private GameObject _bombPrefab;

        [SerializeField] private float _bombRechargeTime;
        
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
                _activeRechargeTime = Math.Clamp(_activeRechargeTime + Time.deltaTime, 0 , _bombRechargeTime);

                if (_activeRechargeTime >= _bombRechargeTime)
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

            Instantiate(_bombPrefab, dropPosition, Quaternion.identity);
        }
    }
}
