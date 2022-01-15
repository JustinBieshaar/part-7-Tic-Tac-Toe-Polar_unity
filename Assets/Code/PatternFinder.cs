using System.Collections.Generic;
using UnityEngine;

public static class PatternFinder {
    public static List<HitBox> CheckWin(Dictionary<string, HitBox> fields) {
        List<HitBox> match;

        var rowsX = GameManager.Instance.RowsX;
        var rowsY = GameManager.Instance.RowsY;
        for (int x = 0; x < rowsX; x++) {
            for (int y = 0; y < rowsY; y++) {
                // diagonal bottom left, top right
                match = CheckMatch(x, y, 1, -1, fields);


                // diagonal bottom right, top left
                if (match == null) {
                    match = CheckMatch(x, y, 1, -1, fields);
                }

                // diagonal bottom left, top right
                if (match == null) {
                    match = CheckMatch(x, y, 1, 1, fields);
                }

                // horizontal
                if (match == null) {
                    match = CheckMatch(x, y, 1, 0, fields);
                }

                // vertical
                if (match == null) {
                    match = CheckMatch(x, y, 0, 1, fields);
                }

                if (match != null) {
                    return match;
                }
            }
        }

        return null;
    }


    private static List<HitBox> CheckMatch(int x, int y, int dX, int dY,
        Dictionary<string, HitBox> fields) {
        var hitMatch = new List<HitBox>();
        var type = -1;
        var checkCount = 0;

        var rowsX = GameManager.Instance.RowsX;
        var rowsY = GameManager.Instance.RowsY;
        var match = GameManager.Instance.Match;
        while (checkCount < match && y >= 0 && y < rowsY) {
            var found = false;
            var key = $"{x},{y}";
            var marker = fields.ContainsKey(key) ? fields[key] : null;
            if (marker != null && marker.Type != -1) {
                if (type == -1) {
                    type = marker.Type;
                }

                if (marker.Type == type) {
                    hitMatch.Add(marker);
                    found = true;
                }
            }

            if (!found) {
                if (hitMatch.Count >= match) {
                    return hitMatch;
                }

                hitMatch.Clear();
                return null;
            }

            x += dX;
            if (x >= rowsX) {
                x = x % rowsX;
            }
            else if (x < 0) {
                x = rowsX + x;
            }

            y += dY;
            checkCount++;
        }

        return hitMatch.Count >= match ? hitMatch : null;
    }
}