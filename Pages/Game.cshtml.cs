using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class GameModel : PageModel
    {
        // Query-string parameters from Operations / Difficulty pages
        [BindProperty(SupportsGet = true)]
        public string Operation { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string Difficulty { get; set; } = "";

        // Selected answer from the form (radio/button)
        [BindProperty]
        public string? SelectedAnswer { get; set; }

        // Values displayed on the page
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Symbol { get; set; } = "";

        public List<double> Choices { get; set; } = new();

        public int Score { get; set; }
        public int QuestionNumber { get; set; }

        public string? Feedback { get; set; }
        public bool GameOver { get; set; }

        // Shared game state (simple for a school project)
        private static readonly Random _rand = new Random();
        private static int _num1, _num2;
        private static int _score = 0;
        private static int _questionCount = 1;

        public void OnGet()
        {
            // New game starts when coming from Operations / Difficulty page
            _score = 0;
            _questionCount = 1;
            GameOver = false;
            Feedback = null;

            GenerateQuestion();
        }

        public void OnPost()
        {
            // 1. Next Question button was clicked
            if (Request.Form.ContainsKey("Next"))
            {
                if (_questionCount >= 10)
                {
                    // Finished all questions
                    GameOver = true;
                    Score = _score;
                    QuestionNumber = 10;

                    // Keep showing last question numbers
                    Number1 = _num1;
                    Number2 = _num2;
                    Symbol = OperationToSymbol(Operation);
                    return;
                }

                _questionCount++;
                GenerateQuestion();
                return;
            }

            // 2. User submitted an answer
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

                // Make sure the page does NOT show 0 × 0
                Number1 = _num1;
                Number2 = _num2;
                Symbol = OperationToSymbol(Operation);

                Score = _score;
                QuestionNumber = _questionCount;

                // If this was the last question, end the game
                if (_questionCount >= 10)
                {
                    GameOver = true;
                }
                else
                {
                    GameOver = false;
                }

                // We don't regenerate choices here, because after answering
                // your UI only shows feedback + "Next Question" button.
            }
            else
            {
                // No answer selected – show warning and keep same question
                Feedback = "⚠️ Please select an answer first.";

                Number1 = _num1;
                Number2 = _num2;
                Symbol = OperationToSymbol(Operation);
                Score = _score;
                QuestionNumber = _questionCount;
                GameOver = false;
            }
        }

        // ===== Helper methods =====

        private void GenerateQuestion()
        {
            // Decide range based on difficulty
            int min, max;
            switch (Difficulty?.ToLower())
            {
                case "easy":
                    min = 1;
                    max = 10;
                    break;
                case "medium":
                    min = 5;
                    max = 50;
                    break;
                case "hard":
                    min = 100;
                    max = 4000; // gives numbers like 3760, 2368
                    break;
                default:
                    min = 1;
                    max = 20;
                    break;
            }

            _num1 = _rand.Next(min, max + 1);
            _num2 = _rand.Next(min, max + 1);

            // Avoid division by zero
            if (Operation == "divide" && _num2 == 0)
                _num2 = 1;

            Number1 = _num1;
            Number2 = _num2;
            Symbol = OperationToSymbol(Operation);

            QuestionNumber = _questionCount;
            Score = _score;
            GameOver = false;
            Feedback = null;

            double correct = Calculate(_num1, _num2, Operation);
            Choices = GenerateChoices(correct);
        }

        private string OperationToSymbol(string op)
        {
            return op switch
            {
                "add" => "+",
                "subtract" => "-",
                "multiply" => "×",
                "divide" => "÷",
                _ => "?"
            };
        }

        private double Calculate(int a, int b, string op)
        {
            return op switch
            {
                "add" => a + b,
                "subtract" => a - b,
                "multiply" => a * 1.0 * b,
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

                if (choice >= 0)
                {
                    list.Add(choice);
                }
            }

            return list.OrderBy(x => _rand.Next()).ToList();
        }
    }
}
