using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MultiItemShellExtension
{
    // Create checksum file
    [ComVisible(true)]
    [Guid("00619cf0-6860-4b62-ba68-7952483a745f")]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class LazyCrcContextMenu : SharpContextMenu
    {
        protected override bool CanShowMenu() => true;

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var item = new ToolStripMenuItem("Create Checksum File");
            item.Click += (s, e) => Launch();
            menu.Items.Add(item);
            return menu;
        }

        private void Launch()
        {
            using (var dlg = new SaveFileDialog())
            {
                // Setup supported file extensions for the dialog
                dlg.Title = "Save Checksum File";
                dlg.Filter =
                    "CRC32 checksum (*.sfv)|*.sfv|" +
                    "MD5 checksum (*.md5)|*.md5|" +
                    "SHA256 checksum (*.sha256)|*.sha256|" +
                    "SHA512 checksum (*.sha512)|*.sha512|" +
                    "Blake3 checksum (*.b3sum)|*.b3sum";
                dlg.FilterIndex = 1;
                dlg.AddExtension = true;

                // Use the first item's parent directory as the dialog's default
                dlg.InitialDirectory = Path.GetDirectoryName(SelectedItemPaths.First());

                // Set default dialog name depending on how many items are selected
                var paths = SelectedItemPaths.ToList();
                if (paths.Count == 1)
                {
                    dlg.FileName = Path.GetFileNameWithoutExtension(paths[0]);
                }
                else
                {
                    dlg.FileName = new DirectoryInfo(Path.GetDirectoryName(paths[0])).Name;
                }

                // Exit gracefully if user closed the dialog
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                // Select algo based on selected extension
                string flag;
                switch (dlg.FilterIndex)
                {
                    case 2: flag = "--md5"; break;
                    case 3: flag = "--sha256"; break;
                    case 4: flag = "--sha512"; break;
                    case 5: flag = "--blake3"; break;
                    default: flag = null; break;
                }

                // Build arguments string
                var args = new StringBuilder();
                if (!string.IsNullOrEmpty(flag))
                {
                    args.Append(flag + " ");
                }

                args.Append($"--output \"{dlg.FileName}\" --files");
                foreach (var file in SelectedItemPaths)
                {
                    args.Append($" \"{file}\"");
                }

                // Run LazyCRC
                LazyCrcHelpers.RunLazyCRC(args.ToString());
            }
        }
    }

    // Verify checksum files (sfv, md5, sha256, sha512, b3sum)
    [ComVisible(true)]
    [Guid("d4865576-e2ca-4715-9e9f-314d4ece7397")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".sfv")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".md5")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".sha256")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".sha512")]
    [COMServerAssociation(AssociationType.ClassOfExtension, ".b3sum")]
    public class LazyCrcCheckMenu : SharpContextMenu
    {
        // Show "Verify Checksum" when exactly one checksum file is selected
        protected override bool CanShowMenu() => SelectedItemPaths.Count() == 1;

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var item = new ToolStripMenuItem("Verify Checksum");
            item.Click += (s, e) => RunCheck();
            menu.Items.Add(item);
            return menu;
        }

        private void RunCheck()
        {
            string checksumFile = SelectedItemPaths.First();
            LazyCrcHelpers.RunLazyCRC($"--check \"{checksumFile}\"");
        }
    }


    // Internal helper functions
    internal static class LazyCrcHelpers
    {
        // Get lazy_crc.exe's path
        internal static string GetExePath()
        {
            return Path.Combine(
                Path.GetDirectoryName(typeof(LazyCrcContextMenu).Assembly.Location),
                "lazy_crc.exe");
        }

        // Run LazyCRC with arguments
        internal static void RunLazyCRC(string arguments)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GetExePath(),
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false
            });
        }
    }
}