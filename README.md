# Lazy CRC Shell Extension

A Windows Explorer shell extension for LazyCRC, adding a context menu entry for generating checksum files directly from Explorer.

## Usage

Right-click any file or folder and select **Create Checksum File**. A save dialog will appear where you can choose the output location, filename, and checksum format:

- CRC32 (`.sfv`)
- MD5 (`.md5`)
- SHA256 (`.sha256`)
- SHA512 (`.sha512`)
- Blake3 (`.blake3`)

## Installation

1. Get the latest `lazy_crc_shell.zip` [from the releases section](https://github.com/mpaterakis/LazyCRC-Shell-Extension/releases)
2. Extract all files in the same folder as `lazy_crc.exe`
3. Run `register_lazy_crc_shell.bat` as Administrator

## Uninstall

Run `unregister_lazy_crc_shell.bat` as Administrator

## License
This project is covered by the [MIT License](LICENSE).

## Credits
* [SharpShell](https://github.com/Nirmal4G/SharpShell)
* [ILRepack](https://github.com/gluck/il-repack)
