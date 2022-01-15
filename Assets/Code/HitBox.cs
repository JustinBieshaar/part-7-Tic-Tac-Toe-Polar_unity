        using System;
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;


        public class HitBox : MonoBehaviour {
            [SerializeField] private MeshRenderer _renderer;
            [SerializeField] private GameObject _x;
            [SerializeField] private GameObject _o;

            private Vector3 _cacheSize;
            private int _type = -1;

            public int Type => _type;

            private void Start() {
                _cacheSize = transform.localScale;
            }

            private void OnMouseEnter() {
                if (_type >= 0) {
                    return;
                }

                transform.localScale = _cacheSize * 2;
            }

            private void OnMouseExit() {
                if (_type >= 0) {
                    return;
                }

                transform.localScale = _cacheSize;
            }

            private void OnMouseUpAsButton() {
                if (_type >= 0) {
                    return;
                }

                transform.localScale = _cacheSize;
                _type = GameManager.Instance.Turn;
                _renderer.enabled = false;
                var markerToSpawn = _type == 0 ? _x : _o;
                Instantiate(markerToSpawn, transform);

                GameManager.Instance.MoveMade();
            }
        }