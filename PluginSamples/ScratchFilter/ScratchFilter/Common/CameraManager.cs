using System.Collections.Generic;
using VideoOS.Platform;

namespace ScratchFilter.Common
{
    /// <summary>
    /// カメラのマネージャです。
    /// </summary>
    [ToString]
    internal sealed class CameraManager
    {
        #region Constructors

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        private CameraManager() { }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// インスタンスです。
        /// </summary>
        /// <value>インスタンス</value>
        internal static CameraManager Instance { get; } = new CameraManager();

        #endregion Properties

        #region Methods

        /// <summary>
        /// カメラの一覧を取得します。
        /// </summary>
        /// <param name="items">項目の一覧</param>
        /// <returns>カメラの一覧</returns>
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
        /// カメラの一覧を取得します。
        /// </summary>
        /// <returns>カメラの一覧</returns>
        internal IReadOnlyCollection<Item> GetCameras()
        {
            return GetCameras(Configuration.Instance.GetItemsByKind(Kind.Camera));
        }

        #endregion Methods
    }
}
