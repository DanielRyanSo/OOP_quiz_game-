using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class GameModel : PageModel
    {
        [BindProperty(SupportsGet = true)] public string Operation { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string Difficulty { get; set; } = "";
        [BindProperty] public string? SelectedAnswer { get; set; }

        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Symbol { get; set; } = "";
        public List<double> Choices { get; set; } = new();
        public string? Feedback { get; set; }
        public int Score { get; set; }
        public int QuestionNumber { get; set; }
        public bool GameOver { get; set; }

        private static int _score = 0;
        private static int _questionCount = 1;
        private static Random _rand = new();
        private static int _num1, _num2;

        public void OnGet()
        {
            _score = 0;
            _questionCount = 1;

            if (!string.IsNullOrEmpty(Operation) && !string.IsNullOrEmpty(Difficulty))
            {
                GenerateQuestion();
            }
        }

        public void OnPost()
        {
            if (Request.Form.ContainsKey("Next"))
            {
                if (_questionCount > 10)
                {
                    GameOver = true;
                    Score = _score;
                    return;
                }
                GenerateQuestion();
                return;
            }

            if (Request.Form.ContainsKey("SelectedAnswer") &&
                double.TryParse(Request.Form["SelectedAnswer"], out double userAnswer))
            {
                double correctAnswer = Calculate(_num1, _num2, Operation);

                if (Math.Abs(userAnswer - correctAnswer) < 0.01)
                {
                    _score++;
                    Feedback = "✅ Correct!";
                }
                else
                {
                    Feedback = $"❌ Wrong! The correct answer is {correctAnswer}";
                }

                Score = _score;
                _questionCount++;

                if (_questionCount > 10)
                {
                    GameOver = true;
                    Score = _score;
                }
            }
            else
            {
                Feedback = "⚠️ Please select an answer first.";
            }
        }

        private void GenerateQuestion()
        {
            (int min, int max) range = Difficulty switch
            {
                "easy" => (1, 99),      
                "medium" => (100, 999),
                "hard" => (1000, 9999), 
                _ => (1, 99)
            };

            _num1 = _rand.Next(range.min, range.max + 1);
            _num2 = _rand.Next(range.min, range.max + 1);
            if (Operation == "divide" && _num2 == 0) _num2 = 1;

            Number1 = _num1;
            Number2 = _num2;

            Symbol = Operation switch
            {
                "add" => "+",
                "subtract" => "-",
                "multiply" => "×",
                "divide" => "÷",
                _ => "?"
            };

            double correct = Calculate(_num1, _num2, Operation);
            Choices = GenerateChoices(correct);

            Feedback = null;
            Score = _score;
            QuestionNumber = _questionCount;
            GameOver = false;
        }

        private double Calculate(int a, int b, string op)
        {
            return op switch
            {
                "add" => a + b,
                "subtract" => a - b,
                "multiply" => a * b,
                "divide" => Math.Round((double)a / b, 2),
                _ => 0
            };
        }

        private List<double> GenerateChoices(double correct)
        {
            var list = new HashSet<double> { correct };

            while (list.Count < 4)
            {
                double offset = _rand.Next(-20, 21);
                double choice = Math.Round(correct + offset, 2);
                if (choice != correct && choice >= 0)
                    list.Add(choice);
            }

            return list.OrderBy(x => _rand.Next()).ToList();
        }
    }
}
