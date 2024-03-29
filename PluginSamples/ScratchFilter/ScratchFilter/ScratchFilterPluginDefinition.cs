﻿/*
 * MIT License
 *
 * (c) 2020 Akihiro Hamana
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

using NLog;

using ScratchFilter.Client.Data;
using ScratchFilter.Client.OptionsDialog;
using ScratchFilter.Client.Toolbar;

using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter;

/// <summary>
/// The PluginDefinition is the 'entry' point to any plugin.
/// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.
/// Several PluginDefinitions are allowed to be available within one DLL.
/// Here the references to all other plugin known objects and classes are defined.
/// The class is an abstract class where all implemented methods and properties need to be declared with override.
/// The class is constructed when the environment is loading the DLL.
/// </summary>
/// <seealso cref="PluginDefinition" />
/// <seealso cref="IDisposable" />
[ToString]
public sealed class ScratchFilterPluginDefinition : PluginDefinition, IDisposable
{
    #region Fields

    /// <summary>
    /// ロガーです。
    /// </summary>
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// アセンブリです。
    /// </summary>
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// バージョン情報です。
    /// </summary>
    private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.Location);

    /// <summary>
    /// プラグインの ID です。
    /// </summary>
    private static readonly Guid PluginId = Guid.Parse(Assembly.GetCustomAttribute<GuidAttribute>().Value);

    /// <summary>
    /// アンマネージリソースが解放されたかどうかです。
    /// </summary>
    private bool isDisposed;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// 静的コンストラクタです。
    /// </summary>
    static ScratchFilterPluginDefinition()
    {
        var productName = FileVersionInfo.ProductName;
        var environmentType = Enum.GetName(typeof(EnvironmentType), EnvironmentManager.Instance.EnvironmentType);

        LogManager.Configuration.Variables.Add(nameof(productName), productName);
        LogManager.Configuration.Variables.Add(nameof(environmentType), environmentType);
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// 著作権です。
    /// </summary>
    /// <value>著作権</value>
    internal string Copyright { get; } = FileVersionInfo.LegalCopyright;

    /// <inheritdoc />
    public sealed override Guid Id { get; } = PluginId;

    /// <inheritdoc />
    public sealed override string Name { get; } = FileVersionInfo.ProductName;

    /// <inheritdoc />
    public sealed override string Manufacturer { get; } = FileVersionInfo.CompanyName;

    /// <inheritdoc />
    public sealed override string VersionString { get; } = FileVersionInfo.ProductVersion;

    /// <inheritdoc />
    public sealed override Image? Icon { get; }

    /// <inheritdoc />
    public sealed override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins { get; } = new();

    /// <inheritdoc />
    public sealed override List<OptionsDialogPlugin> OptionsDialogPlugins { get; } = new();

    #endregion Properties

    #region Methods

    /// <summary>
    /// アンマネージリソースを解放し、必要に応じてマネージリソースも解放します。
    /// </summary>
    /// <param name="disposing">マネージリソースとアンマネージリソースの両方を解放する場合は <c>true</c>。アンマネージリソースだけを解放する場合は <c>false</c>。</param>
    private void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            Icon?.Dispose();
        }

        isDisposed = true;
    }

    /// <inheritdoc />
    public sealed override void Init()
    {
        Logger.ForInfoEvent()
              .Message("Start plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
              .Log();

        if (EnvironmentManager.Instance.EnvironmentType == EnvironmentType.SmartClient)
        {
            ViewItemToolbarPlugins.Add(new ToolbarSeparator());
            ViewItemToolbarPlugins.Add(new ToolbarScratchFilter());
            ViewItemToolbarPlugins.Add(new ToolbarScratchFilterSetting());
            ViewItemToolbarPlugins.Add(new ToolbarSeparator());

            OptionsDialogPlugins.Add(new OptionScratchFilter(this));

            ScratchFilterSettingManager.Instance.Load();
        }
    }

    /// <inheritdoc />
    public sealed override void Close()
    {
        ViewItemToolbarPlugins.Clear();
        OptionsDialogPlugins.Clear();

        Dispose();

        Logger.ForInfoEvent()
              .Message("End plug-in on {0}", EnvironmentManager.Instance.EnvironmentProduct)
              .Log();
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Methods
}
