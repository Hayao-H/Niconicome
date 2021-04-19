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
using System.Windows.Shapes;
using Niconicome.ViewModels.Mainpage.Subwindows;

namespace Niconicome.Views.Mainpage
{
    /// <summary>
    /// SearchPage.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchPage : Window
    {
        public SearchPage()
        {
            this.InitializeComponent();
            this.DataContext = new SearchViewModel();
        }
    }
}
