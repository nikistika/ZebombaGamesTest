using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(menuName = "Core/AppSettings", fileName = "AppSettings")]
    public class AppSettings : ScriptableObject
    {
        [field: SerializeField] public List<GameColor> GameColors { get; private set; }
        [field: SerializeField] public BallView BallPrefab { get; private set; }

        public Color[] Colors
        {
            get
            {
                _colors ??= GameColors.Select(x => x.Color).ToArray();
                return _colors;
            }
        }

        private Color[] _colors;

        private void OnValidate()
        {
            _colors = GameColors.Select(x => x.Color).ToArray();
        }

        public int GetScoresByColor(Color color)
        {
            var gameColor = GameColors.FirstOrDefault(x => x.Color == color);

            if (gameColor.Equals(default))
                Debug.LogError("GameColor not found");

            return gameColor.ScoreForLine;
        }

        [Serializable]
        public struct GameColor
        {
            [field: SerializeField] public Color Color { get; private set; }
            [field: SerializeField] public int ScoreForLine { get; private set; }
        }
    }
}