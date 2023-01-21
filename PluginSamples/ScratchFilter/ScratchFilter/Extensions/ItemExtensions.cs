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

using System.Collections.Generic;

using VideoOS.Platform;

namespace ScratchFilter.Extensions;

/// <summary>
/// <see cref="Item" /> の拡張メソッドです。
/// </summary>
internal static class ItemExtensions
{
    #region Methods

    /// <summary>
    /// 項目の一覧をフォルダ階層のない形式にします。
    /// </summary>
    /// <param name="items">項目の一覧</param>
    /// <returns>フォルダ階層のない形式にした項目の一覧</returns>
    private static List<Item> FlattenItems(List<Item> items)
    {
        var flatItems = new List<Item>();

        items.ForEach(item =>
        {
            if (item.FQID.FolderType == FolderType.No)
            {
                flatItems.Add(item);
            }
            else if (item.HasChildren != HasChildren.No)
            {
                flatItems.AddRange(FlattenItems(item.GetChildren()));
            }
        });

        return flatItems;
    }

    /// <summary>
    /// 項目をフォルダ階層のない形式にします。
    /// </summary>
    /// <param name="item"><see cref="Item" /></param>
    /// <returns>フォルダ階層のない形式にした項目</returns>
    internal static List<Item> Flatten(this Item item) =>
        FlattenItems(new() { item });

    #endregion Methods
}
