﻿using UnityEngine;

namespace AID.Nudge
{
    //This is what we want but more, so we use custom menu items in NudgeMenuItems
    //[CreateAssetMenu( menuName = "CommentSO %$&c", fileName = "New CommentSO")]
    public class CommentSO : ScriptableObject, ICommentHolder
    {
        public Comment comment;

        public Comment Comment => comment;

        public string Name => name;

        public Object UnityObject => this;

#if UNITY_2020_1_OR_NEWER
        public virtual void OnValidate()
#else

        public virtual void OnEnable()
#endif
        {
            if (comment == null)
                comment = new Comment();

            comment.ValidateInternalData();
        }

#if UNITY_EDITOR

        [ContextMenu("Copy GUID")]
        public void CopyGUID()
        {
            UnityEditor.EditorGUIUtility.systemCopyBuffer = comment.GUIDString;
        }

#endif
    }
}
