﻿/*
The functions in this file are based on the work by Viktor Gustavsson (https://github.com/villor/TibiaHUD), which has the following license.

The MIT License (MIT)

Copyright (c) 2016 Viktor Gustavsson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tibialyzer {
    public class CurveBar : Control {
        private Timer updateTimer;

        private float health = 1;
        private float mana = 1;

        public CurveBar() {
            updateTimer = new Timer();
            updateTimer.Interval = 100;
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        private void updateTimer_Tick(object sender, EventArgs e) {
            health = (float)MemoryReader.Health / MemoryReader.MaxHealth;
            mana = (float)MemoryReader.Mana / MemoryReader.MaxMana;
            try {
                bool visible = ProcessManager.IsTibiaActive();
                this.Invoke((MethodInvoker)delegate {
                    this.Visible = visible;
                });
            } catch {
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            float thickness = (float)SettingsManager.getSettingDouble("CurvedBarsFontSize");

            Rectangle rect = this.DisplayRectangle;
            rect.Y -= (rect.Width - rect.Height) / 2;
            rect.Height = rect.Width;
            rect.Width -= (int) (thickness + 2);
            rect.X += (int) (thickness / 2 + 1);

            using (Pen pen = new Pen(Color.Black)) {
                pen.Width = thickness + 2;
                e.Graphics.DrawArc(pen, rect, 135, 90);
                e.Graphics.DrawArc(pen, rect, 45, -90);

                pen.Width = thickness;

                health = Math.Min(1, Math.Max(0, health));
                mana = Math.Min(1, Math.Max(0, mana));
                
                pen.Color = StyleManager.GetHealthColor(health);
                e.Graphics.DrawArc(pen, rect, 135, 90 * health);

                pen.Color = StyleManager.ManaColor;
                e.Graphics.DrawArc(pen, rect, 45, -90 * mana);
            }
        }
    }
}