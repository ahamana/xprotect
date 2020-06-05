using System;
using System.Collections.Generic;

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
        #region Fields

        /// <summary>
        /// ジェネリックイベントのデータソースの ID の一覧です。
        /// </summary>
        private static readonly IReadOnlyDictionary<GenericEventDataSourceType, Guid> GenericEventDataSourceIds = new Dictionary<GenericEventDataSourceType, Guid>()
        {
            // ジェネリックイベントのデータソース「互換」の ID
            { GenericEventDataSourceType.Compatible, Guid.Parse("b867db0c-be9e-422b-b934-6fc7fa98c5d8") },
            // ジェネリックイベントのデータソース「インターナショナル」の ID
            { GenericEventDataSourceType.International, Guid.Parse("8607bccc-2bb5-4b47-a7de-8225d14c4213") }
        };

        #endregion

        #region Methods

        /// <summary>
        /// ID を取得します。
        /// </summary>
        /// <param name="genericEventDataSourceType">ジェネリックイベントのデータソースのタイプ</param>
        /// <returns>ID</returns>
        internal static Guid GetId(this GenericEventDataSourceType genericEventDataSourceType)
        {
            return GenericEventDataSourceIds[genericEventDataSourceType];
        }

        #endregion
    }
}
