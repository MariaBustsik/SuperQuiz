using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace SuperQuiz
{

    public partial class MainPage : ContentPage
    {

        public class Question
        {

            public string Text { get; set; }
            public string Answer { get; set; }
        }
        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;

        public MainPage()
        {
            InitializeComponent();

            // Считывание вопросов с файла
            
            var questionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Questions1.txt");
            
            if (File.Exists(questionsFilePath))
            {
                var lines = File.ReadAllLines(questionsFilePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        questions.Add(new Question { Text = parts[0], Answer = parts[1] });
                    }
                }
            }

            // Показать первый вопрос, если он есть
            if (questions.Count > 0)
            {
                currentQuestionIndex = 0;
                questionLabel.Text = questions[currentQuestionIndex].Text;
            }
            else
            {
                questionLabel.Text = "Нет доступных вопросов.";
            }
        }

        private void SubmitButton_Clicked(object sender, EventArgs e)
        {
            // Проверка правильности ответа
            var userAnswer = answerEntry.Text;
            var correctAnswer = questions[currentQuestionIndex].Answer;
            if (userAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
            {
                DisplayAlert("Верно", "Поздравляю, ты ответил верно!", "OK");
            }
            else
            {
                DisplayAlert("Неверно", $"Прости, верный вариант ответа {correctAnswer}", "OK");
            }

            // Переход к следующему вопросу
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
            {
                questionLabel.Text = questions[currentQuestionIndex].Text;
                answerEntry.Text = "";
            }
            else
            {
                DisplayAlert("Quiz завершён", "Ты закончил quiz!", "OK");
                currentQuestionIndex = 0; // Переходу к новому подходу
                questionLabel.Text = questions[currentQuestionIndex].Text;
                answerEntry.Text = "";
            }
        }


        private async void AddQuestionButton_Clicked(object sender, EventArgs e)
        {
            var newQuestion = await DisplayPromptAsync("Новый вопрос", "Введи новый вопрос:");
            if (!string.IsNullOrWhiteSpace(newQuestion))
            {
                var newAnswer = await DisplayPromptAsync("Новый ответ", "Введи новый ответ:");
                if (!string.IsNullOrWhiteSpace(newAnswer))
                {
                    var questionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Questions1.txt");
                    File.AppendAllText(questionsFilePath, $"{newQuestion},{newAnswer}\n");
                }
            }
        }
        private void DeleteQuestionButton_Clicked(object sender, EventArgs e)
        {
            if (questions.Count == 0)
            {
                DisplayAlert("Нет вопросов", "Нет доступных вопросов для удаления.", "OK");
                return;
            }

            var questionToDelete = questions[currentQuestionIndex];

            var confirmDelete = DisplayAlert("Удалить вопрос", $"Вы уверены, что хотите удалить вопрос:\n\n{questionToDelete.Text}", "Да", "Отмена");

            confirmDelete.ContinueWith((task) =>
            {
                if (task.Result)
                {
                    questions.Remove(questionToDelete);
                    var questionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Questions1.txt");
                    File.WriteAllLines(questionsFilePath, questions.Select(q => $"{q.Text},{q.Answer}"));
                    if (questions.Count > 0)
                    {
                        currentQuestionIndex = 0;
                        questionLabel.Text = questions[currentQuestionIndex].Text;
                    }
                    else
                    {
                        questionLabel.Text = "Нет доступных вопросов.";
                    }
                }
            });
        }   }
}
