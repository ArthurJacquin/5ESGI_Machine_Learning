#if UNITY_STANDALONE_OSX || UNITY_EDITOR
using System;
using UnityEngine;

namespace Crosstales.FB.Wrapper
{
    /// <summary>File browser implementation for macOS.</summary>
    public class FileBrowserMac : FileBrowserBase
    {
        #region Variables

        private static Action<string[]> _openFileCb;
        private static Action<string[]> _openFolderCb;
        private static Action<string> _saveFileCb;

        private const char splitChar = (char)28;

        #endregion


        #region Implemented methods

        public override bool canOpenMultipleFiles
        {
            get
            {
                return true;
            }
        }

        public override bool canOpenMultipleFolders
        {
            get
            {
                return true;
            }
        }

        public override bool isPlatformSupported
        {
            get
            {
                return Util.Helper.isMacOSPlatform; // || Util.Helper.isLinuxEditor;
            }
        }

        public override string[] OpenFiles(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogOpenFilePanel(title, directory, getFilterFromFileExtensionList(extensions), multiselect));
            return paths != null ? paths.Split(splitChar) : null;
        }

        public override string[] OpenFolders(string title, string directory, bool multiselect)
        {
            string paths = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogOpenFolderPanel(title, directory, multiselect));
            return paths != null ? paths.Split(splitChar) : null;
        }

        public override string SaveFile(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Mac.NativeMethods.DialogSaveFilePanel(title, directory, defaultName, getFilterFromFileExtensionList(extensions)));
        }

        #endregion


        #region Private methods

        private static string getFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            if (extensions != null && extensions.Length > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                for (int xx = 0; xx < extensions.Length; xx++)
                {
                    var filter = extensions[xx];

                    sb.Append(filter.Name);
                    sb.Append(";");

                    for (int ii = 0; ii < filter.Extensions.Length; ii++)
                    {
                        sb.Append(filter.Extensions[ii]);

                        if (ii + 1 < filter.Extensions.Length)
                            sb.Append(",");
                    }

                    if (xx + 1 < extensions.Length)
                        sb.Append("|");
                }

                if (Util.Config.DEBUG)
                    Debug.Log("getFilterFromFileExtensionList: " + sb);

                return sb.ToString();
            }

            return string.Empty;
        }

        #endregion
    }
}

namespace Crosstales.FB.Wrapper.Mac
{
    /// <summary>Native methods (bridge to macOS).</summary>
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        public delegate void AsyncCallback(string path);

        [System.Runtime.InteropServices.DllImport("FileBrowser")]
        internal static extern IntPtr DialogOpenFilePanel(string title, string directory, string extension, bool multiselect);

        [System.Runtime.InteropServices.DllImport("FileBrowser")]
        internal static extern IntPtr DialogOpenFolderPanel(string title, string directory, bool multiselect);

        [System.Runtime.InteropServices.DllImport("FileBrowser")]
        internal static extern IntPtr DialogSaveFilePanel(string title, string directory, string defaultName, string extension);

    }
}
#endif
// © 2017-2019 crosstales LLC (https://www.crosstales.com)