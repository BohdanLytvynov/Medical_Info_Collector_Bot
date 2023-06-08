using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelBaseLib.Commands;
using ViewModelBaseLib.VM;
using Collector_Bot_Api;
using System.Windows;
using DBController;
using MedInfoDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Options;

namespace Medical_Info_Collector_Telegramm_Bot.ViewModels
{
    public class MainWindowViewModel : ViewModelBaseClass
    {
        #region Fields
        CollectorBot m_colBot;

        Window m_window;

        DB_Controller m_dbController;

        MedInfoDbContext m_medInfoDB;
        #endregion

        #region Properties

        #endregion

        #region Ctor
        public MainWindowViewModel(Window window)
        {            
            DbContextOptionsBuilder<MedInfoDbContext> optBuilder = new DbContextOptionsBuilder<MedInfoDbContext>();

            optBuilder.UseSqlServer("Data Source=ws1\\SQL2019Express;Initial Catalog=MedInfoColBot;Integrated Security=True");

            m_medInfoDB = new MedInfoDbContext(optBuilder);

            m_dbController = new DB_Controller(m_medInfoDB);

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
