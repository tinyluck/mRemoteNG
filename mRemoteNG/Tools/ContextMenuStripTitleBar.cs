using System;
using System.Linq;
using System.Windows.Forms;
using mRemoteNG.App;
using mRemoteNG.Connection;
using mRemoteNG.Container;
using mRemoteNG.Properties;
using mRemoteNG.UI.Forms;
using mRemoteNG.Resources.Language;
using System.Runtime.Versioning;


namespace mRemoteNG.Tools
{
    [SupportedOSPlatform("windows")]
    public class ContextMenuStripTitleBar
    {
        private readonly ContextMenuStrip _cMen;
        private static readonly FrmMain FrmMain = FrmMain.Default;

        public bool Disposed { get; private set; }

        public ContextMenuStripTitleBar()
        {
            try
            {
                _cMen = new ContextMenuStrip
                {
                    Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular,
                                                   System.Drawing.GraphicsUnit.Point, Convert.ToByte(0)),
                    RenderMode = ToolStripRenderMode.Professional
                };
            }
            catch (Exception ex)
            {
                Runtime.MessageCollector.AddExceptionStackTrace("Creating ContextMenuStripTitleBar failed", ex);
            }
        }

        public void Show(int x, int y)
        {
            _cMen.Items.Clear();
            ConnectionsTreeToMenuItemsConverter menuItemsConverter = new()
            {
                MouseUpEventHandler = ConMenItem_MouseUp
            };

            // ReSharper disable once CoVariantArrayConversion
            ToolStripItem[] rootMenuItems = menuItemsConverter
                                            .CreateToolStripDropDownItems(Runtime.ConnectionsService
                                                                                 .ConnectionTreeModel).ToArray();
            _cMen.Items.AddRange(rootMenuItems);

            _cMen.Show(x, y);
        }

        public void Dispose()
        {
            try
            {
                _cMen.Dispose();
                Disposed = true;
            }
            catch (Exception ex)
            {
                Runtime.MessageCollector.AddExceptionStackTrace("Disposing SysTrayIcon failed", ex);
            }
        }

        private void nI_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private static void nI_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FrmMain.Visible)
            {
                HideForm();
                FrmMain.ShowInTaskbar = false;
            }
            else
            {
                ShowForm();
                FrmMain.ShowInTaskbar = true;
            }
        }

        private static void ShowForm()
        {
            FrmMain.Show();
            FrmMain.WindowState = FrmMain.PreviousWindowState;

            if (Properties.OptionsAppearancePage.Default.ShowSystemTrayIcon) return;
            Runtime.NotificationAreaIcon.Dispose();
            Runtime.NotificationAreaIcon = null;
        }

        private static void HideForm()
        {
            FrmMain.Hide();
            FrmMain.PreviousWindowState = FrmMain.WindowState;
        }

        private void ConMenItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (((ToolStripMenuItem)sender).Tag is ContainerInfo) return;
            if (FrmMain.Visible == false)
                ShowForm();
            Runtime.ConnectionInitiator.OpenConnection((ConnectionInfo)((ToolStripMenuItem)sender).Tag);
        }

        private static void cMenExit_Click(object sender, EventArgs e)
        {
            Shutdown.Quit();
        }
    }
}