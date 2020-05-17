using System;

namespace ScratchFilter.Common.ConfigurationItems
{
    /// <summary>
    /// ジェネリックイベントのデータソースのタイプです。
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
        /// <summary>
        /// ジェネリックイベントのデータソース「互換」の ID です。
        /// </summary>
        private static readonly Guid GenericEventDataSourceCompatibleId = new Guid("b867db0c-be9e-422b-b934-6fc7fa98c5d8");

        /// <summary>
        /// ジェネリックイベントのデータソース「インターナショナル」の ID です。
        /// </summary>
        private static readonly Guid GenericEventDataSourceInternationalId = new Guid("8607bccc-2bb5-4b47-a7de-8225d14c4213");

        /// <summary>
        /// ID を取得します。
        /// </summary>
        /// <param name="dataSourceType">ジェネリックイベントのデータソース</param>
        /// <returns>ID</returns>
        public static Guid GetId(this GenericEventDataSourceType dataSourceType)
        {
            return dataSourceType switch
            {
                GenericEventDataSourceType.Compatible => GenericEventDataSourceCompatibleId,
                GenericEventDataSourceType.International => GenericEventDataSourceInternationalId,
                _ => default(Guid)
            };
        }
    }
}
