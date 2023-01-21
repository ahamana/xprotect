//
// MIT License
//
// (c) 2020 Akihiro Hamana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using System.Linq;

using VideoOS.Platform;
using VideoOS.Platform.ConfigurationItems;

namespace ScratchFilter.Common.ConfigurationItems;

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
