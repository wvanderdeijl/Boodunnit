using System;
using System.Collections;
using DefaultNamespace.Enums;
using Interfaces;
using UnityEngine;

namespace DefaultNamespace
{
    public class TerrifiedObject : MonoBehaviour, IFearable
    {
        public float Duration { get; set; }
        public TerrifyState TerrifyState { get; set; }
        
        [SerializeField] private Transform _player;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _duration = 5f;

        private float _speed;
        private Vector3 _velocity;

        private void Awake()
        {
            TerrifyState = TerrifyState.Calm;
            _speed = 5f;
            Duration = _duration;
            _velocity = Vector3.zero;
        }

        private void Update()
        {
            if (TerrifyState == TerrifyState.Terrified) AdjustVelocity();
        }

        public void Fear()
        {
            if (TerrifyState == TerrifyState.Terrified) return;
            
            TerrifyState = TerrifyState.Terrified;
            
            Vector3 offset = _player.position - transform.position;
            Vector3 direction = (offset * -1).normalized;
            _velocity = direction * _speed;

            StartCoroutine(CalmDown());
        }

        private IEnumerator CalmDown()
        {
            yield return new WaitForSeconds(Duration);
            TerrifyState = TerrifyState.Calm;
            _velocity = Vector3.zero;
            AdjustVelocity();
        }

        private void AdjustVelocity()
        {
            _rb.velocity = new Vector3(_velocity.x, _rb.velocity.y, _velocity.z);
        }
    }
}