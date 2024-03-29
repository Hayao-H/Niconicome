﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Niconicome.ViewModels.Setting.Pages;

namespace Niconicome.Views.Setting.Pages
{
    /// <summary>
    /// DebugSettingsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class DebugSettingsPage : Page
    {
        public DebugSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new DebugSettingsPageViewModel();
        }
    }
}
