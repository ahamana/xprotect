//
// (C) 2020 Canon Inc.
//
// This file contains trade secrets of Canon Inc. No part may be reproduced
// or transmitted in any form by any means or for any purpose without the
// express written permission of Canon Inc.
//

using System;
using System.Collections.Generic;

using VideoOS.Platform;

namespace ScratchFilter.Extensions;

/// <summary>
/// <see cref="Configuration" /> の拡張メソッドです。
/// </summary>
internal static class ConfigurationExtensions
{
    #region Methods

    /// <summary>
    /// 指定された種類の項目の一覧を取得します。
    /// </summary>
    /// <param name="items">項目の一覧</param>
    /// <param name="kind">項目の種類</param>
    /// <returns>指定された種類の項目の一覧</returns>
    private static List<Item> GetItemsOfKind(List<Item> items, Guid kind)
    {
        var kindItems = new List<Item>();

        foreach (var item in items)
        {
            if (item.FQID.Kind == kind && item.FQID.FolderType == FolderType.No)
            {
                kindItems.Add(item);
            }
            else if (item.HasChildren != HasChildren.No)
            {
                kindItems.AddRange(GetItemsOfKind(item.GetChildren(), kind));
            }
        }

        return kindItems;
    }

    /// <summary>
    /// 指定された種類の項目の一覧を取得します。
    /// </summary>
    /// <param name="configuration"><see cref="Configuration" /></param>
    /// <param name="kind">項目の種類</param>
    /// <returns>指定された種類の項目の一覧</returns>
    internal static List<Item> GetItemsOfKind(this Configuration configuration, Guid kind)
    {
        return GetItemsOfKind(configuration.GetItemsByKind(kind), kind);
    }

    #endregion Methods
}
