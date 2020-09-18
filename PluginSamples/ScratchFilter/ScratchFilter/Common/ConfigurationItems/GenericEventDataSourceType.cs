using System.Collections.Generic;
using System.Linq;
using VideoOS.Platform;
using VideoOS.Platform.ConfigurationItems;

namespace ScratchFilter.Common.ConfigurationItems
{
    /// <summary>
    /// ジェネリックイベントのデータソースの種類です。
    /// </summary>
    internal enum GenericEventDataSourceType
    {
        /// <summary>
        /// 互換です。
        /// </summary>
        Compatible,

        /// <summary>
        /// インターナショナルです。
        /// </summary>
        International
    }

    /// <summary>
    /// <see cref="GenericEventDataSourceType" /> の拡張メソッドです。
    /// </summary>
    internal static class GenericEventDataSourceTypeExtensions
    {
        #region Methods

        /// <summary>
        /// ジェネリックイベントのデータソースを取得します。
        /// </summary>
        /// <param name="genericEventDataSourceType">ジェネリックイベントのデータソースの種類</param>
        /// <returns>
        /// ジェネリックイベントのデータソース
        /// </returns>
        internal static GenericEventDataSource GetGenericEventDataSource(this GenericEventDataSourceType genericEventDataSourceType)
        {
            ManagementServer managementServer = new ManagementServer(EnvironmentManager.Instance.MasterSite);

            ICollection<GenericEventDataSource> genericEventDataSources = managementServer.GenericEventDataSourceFolder.GenericEventDataSources;

            return genericEventDataSources.First(genericEventDataSource => genericEventDataSourceType.ToString() == genericEventDataSource.Name);
        }

        #endregion Methods
    }
}
