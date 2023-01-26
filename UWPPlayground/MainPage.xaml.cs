using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPPlayground.Extensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPPlayground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OpenPopupBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isShown = TestPopup.TryShowNear(targetElement: OpenPopupBtn, preferenceOrder: new Side[] { Side.Bottom, Side.Right }, margin: 8);
            if (!isShown)
            {
                // Extension couldn't find a suitable position matching given preferences. You can handle this case and set a default behaviour.
            }
        }

        private void TestTb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void TestPopup_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }
    }

}
