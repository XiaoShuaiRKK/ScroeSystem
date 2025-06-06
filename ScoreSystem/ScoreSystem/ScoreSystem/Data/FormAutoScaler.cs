using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem.Data
{
    public class FormAutoScaler
    {
        private class ControlInfo
        {
            public Rectangle Bounds;
            public float FontSize;
        }

        private readonly Form form;
        private readonly Dictionary<Control, ControlInfo> originalControlInfo = new Dictionary<Control, ControlInfo>();
        private Size originalFormSize;
        private Timer resizeTimer;

        public FormAutoScaler(Form form)
        {
            this.form = form;
            form.Load += (_, __) =>
            {
                originalFormSize = form.ClientSize;
                CaptureOriginalControlInfo(form.Controls);
            };

            resizeTimer = new Timer();
            resizeTimer.Interval = 200; // 用户停止调整窗口 200ms 后才执行 Resize
            resizeTimer.Tick += (_, __) =>
            {
                resizeTimer.Stop();
                ResizeControls();
            };

            form.Resize += (_, __) =>
            {
                if (resizeTimer.Enabled)
                    resizeTimer.Stop();
                resizeTimer.Start();
            };
        }

        private void CaptureOriginalControlInfo(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (!originalControlInfo.ContainsKey(control))
                {
                    originalControlInfo[control] = new ControlInfo
                    {
                        Bounds = control.Bounds,
                        FontSize = control.Font.Size
                    };
                }

                if (control.HasChildren)
                    CaptureOriginalControlInfo(control.Controls);
            }
        }

        private void ResizeControls()
        {
            if (originalFormSize.Width == 0 || originalFormSize.Height == 0)
                return;

            float scaleX = (float)form.ClientSize.Width / originalFormSize.Width;
            float scaleY = (float)form.ClientSize.Height / originalFormSize.Height;

            foreach (var kvp in originalControlInfo)
            {
                Control control = kvp.Key;
                ControlInfo info = kvp.Value;

                control.Left = (int)(info.Bounds.Left * scaleX);
                control.Top = (int)(info.Bounds.Top * scaleY);
                control.Width = (int)(info.Bounds.Width * scaleX);
                control.Height = (int)(info.Bounds.Height * scaleY);

                float fontScale = Math.Min(scaleX, scaleY);
                control.Font = new Font(control.Font.FontFamily, Math.Max(info.FontSize * fontScale, 8f), control.Font.Style);
            }
        }
    }
}
