using UnityEngine;

namespace BetterWagons.Features
{
    /// <summary>
    /// Lightweight on-screen message. Rendered from BetterWagonsMod.OnGUI.
    /// Call Show() to display a line for a few seconds.
    /// </summary>
    public static class Toast
    {
        private static string _message;
        private static float _expiresAt;
        private static GUIStyle _labelStyle;
        private static Texture2D _bgTex;

        public static void Show(string message, float seconds = 2.5f)
        {
            _message = message;
            _expiresAt = Time.realtimeSinceStartup + seconds;
        }

        public static void OnGUI()
        {
            if (string.IsNullOrEmpty(_message)) return;
            float remaining = _expiresAt - Time.realtimeSinceStartup;
            if (remaining <= 0) return;

            EnsureStyles();

            // Fade out over last 0.5s.
            float alpha = Mathf.Clamp01(remaining / 0.5f);
            var prevColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, alpha);

            float w = 560f;
            float h = 44f;
            float x = (Screen.width - w) * 0.5f;
            float y = 60f;
            var outer = new Rect(x, y, w, h);
            var inner = new Rect(x + 2, y + 2, w - 4, h - 4);

            GUI.Box(outer, GUIContent.none, _borderStyle);
            GUI.Box(inner, GUIContent.none, _bgStyle);
            GUI.Label(inner, _message, _labelStyle);

            GUI.color = prevColor;
        }

        private static void EnsureStyles()
        {
            if (_labelStyle != null) return;

            // Dark parchment brown (FF uses warm earthy tones for UI panels).
            _bgTex = Solid(new Color32(45, 30, 20, 230));
            var borderTex = Solid(new Color32(192, 150, 90, 255));   // amber/tan

            _bgStyle = new GUIStyle { normal = { background = _bgTex } };
            _borderStyle = new GUIStyle { normal = { background = borderTex } };

            _labelStyle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color32(240, 220, 180, 255) }  // cream
            };
        }

        private static Texture2D Solid(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            t.hideFlags = HideFlags.HideAndDontSave;
            return t;
        }

        private static GUIStyle _bgStyle;
        private static GUIStyle _borderStyle;
    }
}
