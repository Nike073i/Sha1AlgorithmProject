using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
using Microsoft.Win32;
using Sha1AlgorithmProject.Math;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Sha1AlgorithmProject.WPF.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Хеширование текста";

        ///<summary>Заголовок окна</summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        #endregion

        #region Path : string - Путь файла

        ///<summary>Путь файла</summary>
        private string _path;

        ///<summary>Путь файла</summary>
        public string Path
        {
            get => _path;
            set => Set(ref _path, value);
        }

        #endregion

        #region Message : string - Сообщение для получения хэш-кода

        ///<summary>Сообщение для получения хэш-кода</summary>
        private string _message = "Введите строку для получения хеша";
        
        ///<summary>Сообщение для получения хэш-кода</summary>
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        #endregion

        #region Команды вызова функционала

        public ICommand ShowAboutAuthorCommand { get; }
        public ICommand ShowAboutAlgorithmCommand { get; }
        public ICommand GetFileHashCommand { get; }
        public ICommand GetStringHashCommand { get; }
        public ICommand ReviewCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            ShowAboutAuthorCommand = new LambdaCommand(OnExecutedShowAboutAuthorCommand);
            ShowAboutAlgorithmCommand = new LambdaCommand(OnExecutedShowAboutAlgorithmCommand);
            GetFileHashCommand = new LambdaCommand(OnExecutedGetFileHashCommand, CanExecuteGetFileHashCommand);
            GetStringHashCommand = new LambdaCommand(OnExecutedGetStringHashCommand, CanExecuteGetStringHashCommand);
            ReviewCommand = new LambdaCommand(OnExecutedReviewCommand);
        }

        private void OnExecutedShowAboutAuthorCommand()
        {
            MessageBox.Show("Работа студента группы ПИбд-42 - Филиппова Н.А. \n" +
                           "Вариант - 12. \n" +
                           "Хеш-функция - SHA-1",
                           "Автор - Филиппов Н.А.",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnExecutedShowAboutAlgorithmCommand()
        {
            MessageBox.Show("SHA-1 - алгоритм хеширования. + \n" +
                "Принимает на вход сообщение произвольной длины до 2^64 бит. \n" +
                "На выходе получается 160битное хеш значение. \n" +
                "Исходное сообщение разбивается на блоки длиной 512 бит и дополняется информацией о его длине. \n" +
                "Каждый блок проходит 80 итераций обработки с помощью сложения по модулю, XOR, &, |, отрицания и циклического сдвига. \n" +
                "На вход главного цикла приходит блок сообщения и предыдущее значение регистров (5-ти 32битных слов).", "SHA-1",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanExecuteGetFileHashCommand() => !string.IsNullOrEmpty(Path);

        private void OnExecutedGetFileHashCommand()
        {
            if (!File.Exists(Path))
            {
                MessageBox.Show("Файл по указанному пути не найден", "Ошибка получения хеша", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var textBytes = File.ReadAllBytes(Path);
            var sha1 = new Sha1();
            string hashCode = sha1.GetHash(textBytes);
            MessageBox.Show("Хеш-значение файла : \n" + hashCode, "Результат хеширования", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnExecutedReviewCommand()
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                Path = dlg.FileName;
            }
        }

        private bool CanExecuteGetStringHashCommand() => !string.IsNullOrEmpty(Message);

        private void OnExecutedGetStringHashCommand()
        {
            var sha1 = new Sha1();
            string hashCode = sha1.GetHash(Message);
            MessageBox.Show("Хеш-значение строки : \n" + hashCode, "Результат хеширования", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
