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

namespace Backgammon_2_4_20.Networking
{
    public interface ISender
    {
        void SendMessage(string message);
        byte[] GetMessageBytes(string message);
    }
}