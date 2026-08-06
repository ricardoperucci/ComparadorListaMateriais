using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais.UI
{
    /// <summary>
    /// Seletor de pasta no diálogo moderno do Explorer — o mesmo visual do <see cref="OpenFileDialog"/>
    /// usado para escolher as listas, com barra de locais, caminho digitável e busca.
    /// <para>
    /// O WinForms só ganhou <c>OpenFolderDialog</c> no .NET 9; este projeto é .NET 8 e subir o alvo
    /// obrigaria todas as máquinas a instalar outro runtime. Então aqui está o mesmo caminho que o
    /// <c>OpenFolderDialog</c> percorre por dentro: <c>IFileOpenDialog</c> com <c>FOS_PICKFOLDERS</c>.
    /// Se o COM falhar por qualquer motivo, cai no <see cref="FolderBrowserDialog"/> de sempre.
    /// </para>
    /// </summary>
    public static class SeletorPasta
    {
        /// <summary>Abre o seletor de pasta. Devolve null se o usuário cancelar.</summary>
        public static string Selecionar(IWin32Window pDono = null, string pTitulo = null)
        {
            try
            {
                return SelecionarPeloShell(pDono, pTitulo);
            }
            catch (Exception)
            {
                // Sem o diálogo do shell (Windows antigo, política de sistema, COM indisponível),
                // o programa continua funcionando com o seletor clássico.
                using (var dialogo = new FolderBrowserDialog())
                {
                    return dialogo.ShowDialog() == DialogResult.OK ? dialogo.SelectedPath : null;
                }
            }
        }

        private static string SelecionarPeloShell(IWin32Window pDono, string pTitulo)
        {
            var dialogo = (IFileDialog)new FileOpenDialogRCW();

            try
            {
                dialogo.GetOptions(out uint opcoes);
                dialogo.SetOptions(opcoes | FosPickFolders | FosForceFileSystem | FosPathMustExist);

                if (!string.IsNullOrWhiteSpace(pTitulo))
                    dialogo.SetTitle(pTitulo);

                IntPtr dono = pDono?.Handle ?? IntPtr.Zero;

                int resultado = dialogo.Show(dono);

                if (resultado == CanceladoPeloUsuario)
                    return null;

                if (resultado != 0)
                    Marshal.ThrowExceptionForHR(resultado);

                dialogo.GetResult(out IShellItem item);

                if (item == null)
                    return null;

                try
                {
                    item.GetDisplayName(SigdnFileSysPath, out IntPtr ponteiroNome);

                    try
                    {
                        return Marshal.PtrToStringUni(ponteiroNome);
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(ponteiroNome);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(item);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(dialogo);
            }
        }

        // =====================================================================
        //  Interop — IFileOpenDialog / IShellItem
        // =====================================================================

        private const uint FosPickFolders = 0x00000020;
        private const uint FosForceFileSystem = 0x00000040;
        private const uint FosPathMustExist = 0x00000800;

        private const uint SigdnFileSysPath = 0x80058000;

        /// <summary>HRESULT_FROM_WIN32(ERROR_CANCELLED) — o usuário fechou o diálogo.</summary>
        private const int CanceladoPeloUsuario = unchecked((int)0x800704C7);

        [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        private class FileOpenDialogRCW
        {
        }

        /// <summary>
        /// Declaração mínima de <c>IFileDialog</c>. A ordem dos membros É a interface (é o vtable do
        /// COM), por isso os métodos não usados aparecem aqui mesmo assim — remover qualquer um
        /// deslocaria todos os seguintes.
        /// </summary>
        [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileDialog
        {
            [PreserveSig] int Show(IntPtr pHwndOwner);

            [PreserveSig] int SetFileTypes(uint pCFileTypes, IntPtr pFilterSpec);
            [PreserveSig] int SetFileTypeIndex(uint pFileType);
            [PreserveSig] int GetFileTypeIndex(out uint pFileType);

            [PreserveSig] int Advise(IntPtr pFde, out uint pCookie);
            [PreserveSig] int Unadvise(uint pCookie);

            [PreserveSig] int SetOptions(uint pOpcoes);
            [PreserveSig] int GetOptions(out uint pOpcoes);

            [PreserveSig] int SetDefaultFolder(IShellItem pItem);
            [PreserveSig] int SetFolder(IShellItem pItem);
            [PreserveSig] int GetFolder(out IShellItem pItem);
            [PreserveSig] int GetCurrentSelection(out IShellItem pItem);

            [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pNome);
            [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pNome);

            [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pTitulo);
            [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pTexto);
            [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pRotulo);

            [PreserveSig] int GetResult(out IShellItem pItem);

            [PreserveSig] int AddPlace(IShellItem pItem, int pOnde);
            [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pExtensao);
            [PreserveSig] int Close(int pResultado);
            [PreserveSig] int SetClientGuid(ref Guid pGuid);
            [PreserveSig] int ClearClientData();
            [PreserveSig] int SetFilter(IntPtr pFiltro);
        }

        [ComImport, Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            [PreserveSig] int BindToHandler(IntPtr pBc, ref Guid pBhid, ref Guid pRiid, out IntPtr pPv);
            [PreserveSig] int GetParent(out IShellItem pItem);
            [PreserveSig] int GetDisplayName(uint pSigdnName, out IntPtr pNome);
            [PreserveSig] int GetAttributes(uint pMascara, out uint pAtributos);
            [PreserveSig] int Compare(IShellItem pItem, uint pDica, out int pOrdem);
        }
    }
}
