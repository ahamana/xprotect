using NLog;
using ScratchFilter.Client.Data;
using ScratchFilter.Client.Toolbar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ScratchFilter
{
    /// <summary>
    /// The PluginDefinition is the 'entry' point to any plugin.
    /// This is the starting point for any plugin development and the class MUST be available for a plugin to be loaded.
    /// Several PluginDefinitions are allowed to be available within one DLL.
    /// Here the references to all other plugin known objects and classes are defined.
    /// The class is an abstract class where all implemented methods and properties need to be declared with override.
    /// The class is constructed when the environment is loading the DLL.
    /// </summary>
    [ToString]
    public sealed class ScratchFilterPluginDefinition : PluginDefinition
    {
        #region Fields

        /// <summary>
        /// �v���O�C���� ID �ł��B
        /// </summary>
        private static readonly Guid PluginId = Guid.Parse("8c9a02d0-dcb0-4460-8779-23ad282677a0");

        /// <summary>
        /// ���K�[�ł��B
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// �o�[�W�������ł��B
        /// </summary>
        private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        #endregion

        #region Properties

        /// <summary>
        /// ID �ł��B
        /// </summary>
        /// <value>
        /// ID
        /// </value>
        public override Guid Id
        {
            get;
        } = PluginId;

        /// <summary>
        /// ���i���ł��B
        /// </summary>
        /// <value>
        /// ���i��
        /// </value>
        public override string Name
        {
            get;
        } = FileVersionInfo.ProductName;

        /// <summary>
        /// ��Ж��ł��B
        /// </summary>
        /// <value>
        /// ��Ж�
        /// </value>
        public override string Manufacturer
        {
            get;
        } = FileVersionInfo.CompanyName;

        /// <summary>
        /// �v���O�C���̃o�[�W�����ł��B
        /// </summary>
        /// <value>
        /// �v���O�C���̃o�[�W����
        /// </value>
        public override string VersionString
        {
            get;
        } = FileVersionInfo.ProductVersion;

        /// <summary>
        /// �g�b�v���x���Ŏg�p����A�C�R���ł��B
        /// </summary>
        /// <value>
        /// �g�b�v���x���Ŏg�p����A�C�R��
        /// </value>
        public override Image Icon
        {
            get;
        }

        /// <summary>
        /// Smart Client �̃c�[���o�[�p�v���O�C���̈ꗗ�ł��B
        /// </summary>
        /// <value>
        /// Smart Client �̃c�[���o�[�p�v���O�C���̈ꗗ
        /// </value>
        public override List<ViewItemToolbarPlugin> ViewItemToolbarPlugins
        {
            get;
        } = new List<ViewItemToolbarPlugin>();

        #endregion

        #region Methods

        /// <summary>
        /// �������������s���܂��B
        /// </summary>
        public override void Init()
        {
            Logger.Info("Start plug-in");

            ViewItemToolbarPlugins.Add(new ToolbarSeparator());
            ViewItemToolbarPlugins.Add(new ToolbarScratchFilter());
            ViewItemToolbarPlugins.Add(new ToolbarScratchFilterSetting());
            ViewItemToolbarPlugins.Add(new ToolbarSeparator());

            ScratchFilterSettingManager.Instance.Load();
        }

        /// <summary>
        /// �I���������s���܂��B
        /// </summary>
        public override void Close()
        {
            ViewItemToolbarPlugins.Clear();

            Logger.Info("End plug-in");
        }

        #endregion
    }
}
