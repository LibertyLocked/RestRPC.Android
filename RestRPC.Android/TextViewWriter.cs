using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.IO;

namespace RestRPC.Android
{
    delegate void RunOnUiThreadDelegate(Action action);

    class TextViewWriter : TextWriter
    {
        private TextView textView;
        private RunOnUiThreadDelegate runOnUiThread;

        public TextViewWriter(TextView textView, RunOnUiThreadDelegate runOnUiThread)
        {
            this.textView = textView;
            this.runOnUiThread = runOnUiThread;
            
        }

        public override void Write(char value)
        {
            runOnUiThread(() =>
            {
                textView.Text += value;
                ScrollTextViewToBottom();
            });
        }

        public override void Write(string value)
        {
            runOnUiThread(() =>
            {
                textView.Text += value;
                ScrollTextViewToBottom();
            });
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }

        private void ScrollTextViewToBottom()
        {
            int scrollAmount = textView.Layout.GetLineTop(textView.LineCount) - textView.Height;
            if (scrollAmount > 0)
                textView.ScrollTo(0, scrollAmount);
            else
                textView.ScrollTo(0, 0);
        }
    }
}