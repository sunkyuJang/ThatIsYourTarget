using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
    public static GizmosDrawer instanse { private set; get; }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else
            Destroy(this);
    }

    private List<GizmoInfo> gizmosToDraw = new List<GizmoInfo>();

    public void DrawLine(Vector3 startPosition, Vector3 endPosition, float duration, Color color)
    {
        var gizmoInfo = new GizmoInfo { StartPosition = startPosition, EndPosition = endPosition, Duration = duration, Color = color };
        AddGizmoInfo(gizmoInfo);
    }

    public void DrawSphere(Vector3 startPosition, float size, float duration, Color color)
    {
        var gizmoInfo = new SphereGizmoInfo { StartPosition = startPosition, Radius = size, Duration = duration, Color = color, GizmoType = GizmoInfo.GizmosType.Sphere };
        AddGizmoInfo(gizmoInfo);
    }

    public void DrawChangePoint(Vector3 startPoint, Vector3 endPoint, float duration, Color color)
    {
        DrawSphere(endPoint, 0.5f, duration, color);
        DrawLine(startPoint, endPoint, duration, color);
    }

    void AddGizmoInfo(GizmoInfo info)
    {
        gizmosToDraw.Add(info);
        TimeCounter.Instance.SetTimeCounting(info.Duration, () => { info.NeedRemove = true; });
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < gizmosToDraw.Count; i++)
        {
            var info = gizmosToDraw[i];
            if (info.NeedRemove)
            {
                gizmosToDraw.RemoveAt(i--);
                continue;
            }
            else
            {
                Gizmos.color = info.Color;
                switch (info.GizmoType)
                {
                    case GizmoInfo.GizmosType.Line:
                        Gizmos.DrawLine(info.StartPosition, info.EndPosition);
                        break;
                    case GizmoInfo.GizmosType.Sphere:
                        var sphereInfo = info as SphereGizmoInfo;
                        Gizmos.DrawSphere(sphereInfo.StartPosition, sphereInfo.Radius);
                        break;
                }
            }
        }
    }

    public class GizmoInfo
    {
        public enum GizmosType { Line, Sphere }
        public GizmosType GizmoType { set; get; } = GizmosType.Line;
        public Vector3 StartPosition { get; set; } = Vector3.zero;
        public Vector3 EndPosition { get; set; } = Vector3.zero;
        public Color Color { set; get; } = Color.magenta;
        public float Duration { get; set; } = 0f;
        public bool NeedRemove { set; get; } = false;
    }

    public class SphereGizmoInfo : GizmoInfo
    {
        public float Radius { set; get; }
    }
}
