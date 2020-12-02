//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

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
        /// <returns>ジェネリックイベントのデータソース</returns>
        internal static GenericEventDataSource GetGenericEventDataSource(this GenericEventDataSourceType genericEventDataSourceType)
        {
            var managementServer = new ManagementServer(EnvironmentManager.Instance.MasterSite);

            var genericEventDataSources = managementServer.GenericEventDataSourceFolder.GenericEventDataSources;

            return genericEventDataSources.First(genericEventDataSource => genericEventDataSourceType.ToString() == genericEventDataSource.Name);
        }

        #endregion Methods
    }
}
