using UnityEditor;
using UnityEngine;

namespace AID.Nudge
{
    public static class CommentBehGizmoDrawer
    {
        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected | GizmoType.NonSelected, typeof(CommentBeh))]
        public static void DrawGizmoForCommentBeh(CommentBeh commentBeh, GizmoType gizmoType)
        {
            var nudgeSettings = NudgeSettings.instance;

            if (!nudgeSettings.showHidden && commentBeh.comment.hidden && (gizmoType & GizmoType.Selected) == 0)
                return;

            Gizmos.DrawIcon(
                commentBeh.transform.position,
                commentBeh.comment.isTask ? nudgeSettings.commentTaskGizmoPath : nudgeSettings.sceneCommentGizmoPath,
                true);

            if ((gizmoType & GizmoType.Selected) != 0 || nudgeSettings.drawLinkedConnection)
            {
                foreach (var item in commentBeh.comment.linkedObjects)
                {
                    AttemptToDrawLine(commentBeh.transform, item, commentBeh.comment, nudgeSettings);
                }
            }

            if (!commentBeh.hidesTextInSceneViewport && !commentBeh.comment.hidden)
            {
                DrawString(commentBeh.transform.position, commentBeh.comment.body, commentBeh.textColor, new Vector2(0.5f, 0), 14f);
            }
        }

        private static void AttemptToDrawLine(Transform from, UnityEngine.Object targetObj, Comment comment, NudgeSettings nudgeSettings)
        {
            if (targetObj != null)
            {
                Transform linkedTransform = null;
                if (targetObj is GameObject)
                {
                    linkedTransform = (targetObj as GameObject).transform;
                }
                else if (targetObj is Component)
                {
                    linkedTransform = (targetObj as Component).transform;
                }

                if (linkedTransform != null)
                {
                    Gizmos.DrawIcon(
                        linkedTransform.position,
                        comment.isTask ? nudgeSettings.commentTaskLinkedGizmoPath : nudgeSettings.commentLinkedGizmoPath,
                        true);

                    var prevCol = Gizmos.color;
                    Gizmos.color = prevCol * nudgeSettings.linkedTint;
                    Gizmos.DrawLine(from.position, linkedTransform.position);
                    Gizmos.color = prevCol;
                }
            }
        }

        static public void DrawString(Vector3 worldPosition, string text, Color textColor, Vector2 anchor, float textSize)
        {
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            if (!view)
                return;
            Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
                return;
            var pixelRatio = UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.right).x - UnityEditor.HandleUtility.GUIPointToScreenPixelCoordinate(Vector2.zero).x;
            UnityEditor.Handles.BeginGUI();
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = (int)textSize,
                normal = new GUIStyleState() { textColor = textColor }
            };
            Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
            var alignedPosition =
                ((Vector2)screenPosition +
                size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
                Vector2.up * view.camera.pixelHeight;
            GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
            UnityEditor.Handles.EndGUI();
        }
    }
}
