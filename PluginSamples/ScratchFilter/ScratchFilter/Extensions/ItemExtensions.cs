//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
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
