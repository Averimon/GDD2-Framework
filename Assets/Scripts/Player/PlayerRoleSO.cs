using System;
using UnityEngine;

namespace Framework.Player
{
    
    [CreateAssetMenu(fileName = "PlayerRoleSO", menuName = "PlayerRoleSO", order = 0)]
    public class PlayerRoleSO : ScriptableObject
    {
        [SerializeField] private Bomb.BombSO _bomb;

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

            Instantiate(_bomb.prefab, dropPosition, Quaternion.identity);
        }
    }
}
