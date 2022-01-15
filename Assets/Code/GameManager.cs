        using System;
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;

        
        
        public class GameManager : MonoBehaviour {
            private static GameManager _instance;

            public static GameManager Instance =>
                _instance ? _instance : new GameObject("Game Manager").AddComponent<GameManager>();

            private Dictionary<string, HitBox> _fields = new Dictionary<string, HitBox>();
            private List<HitBox> _matchedPattern = new List<HitBox>();

            private int _rowsX;
            private int _rowsY;
            private int _match = 3;
            private int _turn = 0;
            private PolarGeneration _board;

            private bool _gameEnd;
            private int _maxMoves => _rowsX * _rowsY;

            public int Turn => _turn % 2;
            public int Match => _match;
            public int RowsX => _rowsX;
            public int RowsY => _rowsY;
            public List<HitBox> Pattern => _matchedPattern;

            public event Action<bool, int> OnGameEnd;

            void Awake() {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }

            public void Set(int rowsX, int rowsY, int match = 3) {
                _rowsX = rowsX;
                _rowsY = rowsY;
                _match = match;
            }

            public void AddHitBox(HitBox hitbox, int x, int y) {
                _fields.Add($"{x},{y}", hitbox);
            }

            public void MoveMade() {
                _turn++;

                _matchedPattern = PatternFinder.CheckWin(_fields);
                if (_matchedPattern != null) {
                    // WINNER
                    _gameEnd = true;
                    OnGameEnd?.Invoke(_gameEnd, _matchedPattern[0].Type);
                }
                else if (_turn >= _maxMoves) {
                    // TIE
                    _gameEnd = true;
                    OnGameEnd?.Invoke(_gameEnd, -1);
                }
            }

            public void Clear() {
                _fields.Clear();
                _matchedPattern?.Clear();
                _gameEnd = false;
                _turn = 0;
                OnGameEnd?.Invoke(_gameEnd, -1);
            }

            public void Reset() {
                _board.Generate();
            }

            public void SetBoard(PolarGeneration board) {
                _board = board;
            }
        }