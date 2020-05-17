﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using VideoOS.Platform;

namespace ScratchFilter.Common
{
    /// <summary>
    /// カメラのマネージャです。
    /// </summary>
    internal class CameraManager
    {
        #region Properties

        /// <summary>
        /// インスタンスです。
        /// </summary>
        /// <value>
        /// インスタンス
        /// </value>
        internal static CameraManager Instance
        {
            get;
        } = new CameraManager();

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        private CameraManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// カメラの一覧を取得します。
        /// </summary>
        /// <param name="items">項目の一覧</param>
        /// <returns>カメラの一覧</returns>
        private ReadOnlyCollection<Item> GetCameras(List<Item> items)
        {
            List<Item> cameras = new List<Item>();

            items.ForEach(item =>
            {
                if (item.FQID.Kind == Kind.Camera && item.FQID.FolderType == FolderType.No)
                {
                    cameras.Add(item);
                }
                else if (item.HasChildren != HasChildren.No)
                {
                    cameras.AddRange(GetCameras(item.GetChildren()));
                }
            });

            return cameras.AsReadOnly();
        }

        /// <summary>
        /// カメラの一覧を取得します。
        /// </summary>
        /// <returns>カメラの一覧</returns>
        internal ReadOnlyCollection<Item> GetCameras()
        {
            return GetCameras(Configuration.Instance.GetItemsByKind(Kind.Camera));
        }

        #endregion
    }
}