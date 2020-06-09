using System;
using System.Collections.Generic;
using System.Linq;
using VideoOS.Platform;

namespace ScratchFilter.Common
{
    /// <summary>
    /// カメラのマネージャです。
    /// </summary>
    [ToString]
    internal sealed class CameraManager
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
        /// <returns>
        /// カメラの一覧
        /// </returns>
        private IReadOnlyCollection<Item> GetCameras(List<Item> items)
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
        /// カメラを取得します。
        /// </summary>
        /// <param name="cameraId">カメラの ID</param>
        /// <returns>
        /// カメラ
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="cameraId" /> が <see cref="Guid.Empty" /> の場合にスローされます。
        /// </exception>
        internal Item GetCamera(Guid cameraId)
        {
            if (cameraId == Guid.Empty)
            {
                throw new ArgumentException(nameof(cameraId));
            }

            return GetCameras().FirstOrDefault(camera => cameraId == camera.FQID.ObjectId);
        }

        /// <summary>
        /// カメラの一覧を取得します。
        /// </summary>
        /// <returns>
        /// カメラの一覧
        /// </returns>
        internal IReadOnlyCollection<Item> GetCameras()
        {
            return GetCameras(Configuration.Instance.GetItemsByKind(Kind.Camera));
        }

        #endregion
    }
}
