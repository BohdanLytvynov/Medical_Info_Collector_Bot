using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelBaseLib.Commands;
using ViewModelBaseLib.VM;
using Collector_Bot_Api;
using System.Windows;

namespace Medical_Info_Collector_Telegramm_Bot.ViewModels
{
    public class MainWindowViewModel : ViewModelBaseClass
    {
        #region Fields
        CollectorBot m_colBot;

        Window m_window;
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public MainWindowViewModel(Window window)
        {
            

            m_window = window;

            m_colBot = new CollectorBot();

            var str = m_colBot.TestBotAsync();

            MessageBox.Show($"{str}");

            m_colBot.Start();
            m_window = window;

             
        }
        #endregion

        #region Methods
        public void StopBot()
        {
            MessageBox.Show($"Зупиняю роботу!");

            m_colBot.Stop();
        }
        #endregion
    }
}
