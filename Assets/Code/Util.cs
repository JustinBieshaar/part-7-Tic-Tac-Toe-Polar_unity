using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    public static Transform SetPosition(this Transform transform, float x, float y, float z) {
        var pos = transform.position;
        pos.x = x;
        pos.y = y;
        pos.z = z;
        transform.position = pos;
        return transform;
    }

    public static Transform SetPositionLocal(this Transform transform, float x, float y, float z) {
        var pos = transform.localPosition;
        pos.x = x;
        pos.y = y;
        pos.z = z;
        transform.localPosition = pos;
        return transform;
    }
}