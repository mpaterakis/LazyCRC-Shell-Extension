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
                    "Blake3 checksum (*.blake3)|*.blake3";
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

                // Get lazy_crc.exe's path
                string exePath = Path.Combine(
                    Path.GetDirectoryName(typeof(LazyCrcContextMenu).Assembly.Location),
                    "lazy_crc.exe"
                );

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
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args.ToString(),
                    UseShellExecute = false,
                    CreateNoWindow = false
                });
            }
        }
    }
}