using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Proj.Pages
{
    public class GameModel : PageModel
    {
        // Route params from @page "{operation}/{difficulty}"
        [BindProperty(SupportsGet = true)]
        public string Operation { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string Difficulty { get; set; } = "";

        // Selected answer from the buttons
        [BindProperty]
        public string? SelectedAnswer { get; set; }

        // Set to true by the JS timer in Game.cshtml when time runs out
        [BindProperty]
        public bool TimeUp { get; set; }

        // Values displayed in the UI
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Symbol { get; set; } = "";

        public List<double> Choices { get; set; } = new();

        public int Score { get; set; }
        public int QuestionNumber { get; set; }

        public string? Feedback { get; set; }
        public bool GameOver { get; set; }

        // For cat meme popup
        public bool IsCorrectAnswer { get; set; }

        // Timer setting (seconds per question) – works with Game.cshtml JS
        public int TimeLimitSeconds { get; } = 10;

        // Extra classes for OOP (can be mentioned in your diagram/defense)
        public Player CurrentPlayer { get; private set; } = new Player();
        public ScoreTracker Tracker { get; private set; } = new ScoreTracker();
        public Question CurrentQuestion { get; private set; } = new Question();

        // Simple static state for this project
        private static readonly Random _rand = new Random();
        private static int _num1, _num2;
        private static int _score = 0;
        private static int _questionCount = 1;

        public void OnGet()
        {
            // Start a new game
            _score = 0;
            _questionCount = 1;

            GameOver = false;
            Feedback = null;
            IsCorrectAnswer = false;

            Tracker = new ScoreTracker();
            CurrentPlayer = new Player();

            GenerateQuestion();
        }

        public void OnPost()
        {
            // User clicked "Next Question"
            if (Request.Form.ContainsKey("Next"))
            {
                if (_questionCount >= 10)
                {
                    // Finished all questions
                    GameOver = true;
                    Score = _score;
                    QuestionNumber = 10;

                    Number1 = _num1;
                    Number2 = _num2;
                    Symbol = OperationToSymbol(Operation);
                    return;
                }

                _questionCount++;
                Feedback = null;
                IsCorrectAnswer = false;
                GenerateQuestion();
                return;
            }

            // Timer expired, no answer selected
            if (TimeUp && !Request.Form.ContainsKey("SelectedAnswer"))
            {
                double correctAnswer = Calculate(_num1, _num2, Operation);

                Feedback = $"⏰ Time's up! The correct answer is {correctAnswer}";
                IsCorrectAnswer = false;

                Tracker.RegisterAnswer(false);

                Number1 = _num1;
                Number2 = _num2;
                Symbol = OperationToSymbol(Operation);
                Score = _score;
                QuestionNumber = _questionCount;

                if (_questionCount >= 10)
                {
                    GameOver = true;
                }

                return;
            }

            // User clicked an answer button
            if (Request.Form.ContainsKey("SelectedAnswer") &&
                double.TryParse(Request.Form["SelectedAnswer"], out double userAnswer))
            {
                double correctAnswer = Calculate(_num1, _num2, Operation);

                if (Math.Abs(userAnswer - correctAnswer) < 0.01)
                {
                    _score++;
                    Feedback = "✅ Correct!";
                    IsCorrectAnswer = true;
                    Tracker.RegisterAnswer(true);
                }
                else
                {
                    Feedback = $"❌ Wrong! The correct answer is {correctAnswer}";
                    IsCorrectAnswer = false;
                    Tracker.RegisterAnswer(false);
                }

                // Keep showing the same question while feedback is displayed
                Number1 = _num1;
                Number2 = _num2;
                Symbol = OperationToSymbol(Operation);
                Score = _score;
                QuestionNumber = _questionCount;

                if (_questionCount >= 10)
                {
                    GameOver = true;
                }
                else
                {
                    GameOver = false;
                }
            }
            else
            {
                // No answer selected and timer not marked TimeUp (should be rare)
                Feedback = "⚠️ Please select an answer first.";
                IsCorrectAnswer = false;

                Number1 = _num1;
                Number2 = _num2;
                Symbol = OperationToSymbol(Operation);
                Score = _score;
                QuestionNumber = _questionCount;
                GameOver = false;
            }
        }

        // ================== Helpers ==================

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
                    max = 4000;
                    break;
                default:
                    min = 1;
                    max = 20;
                    break;
            }

            // Integer-only division logic
            if (Operation == "divide")
            {
                // choose divisor
                _num2 = _rand.Next(min, max + 1);
                if (_num2 == 0) _num2 = 1;

                // choose integer quotient
                int quotient = _rand.Next(min, max + 1);

                // dividend = divisor * quotient (guaranteed integer result)
                _num1 = quotient * _num2;
            }
            else
            {
                _num1 = _rand.Next(min, max + 1);
                _num2 = _rand.Next(min, max + 1);
            }

            Number1 = _num1;
            Number2 = _num2;
            Symbol = OperationToSymbol(Operation);

            QuestionNumber = _questionCount;
            Score = _score;
            GameOver = false;
            Feedback = null;
            IsCorrectAnswer = false;

            double correct = Calculate(_num1, _num2, Operation);
            Choices = GenerateChoices(correct);

            // Update Question object (for OOP structure)
            CurrentQuestion = new Question
            {
                Number1 = Number1,
                Number2 = Number2,
                Symbol = Symbol,
                CorrectAnswer = correct,
                Choices = new List<double>(Choices)
            };
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
                // integer division: we already ensure a is multiple of b
                "divide" => b == 0 ? 0 : a / b,
                _ => 0
            };
        }

        private List<double> GenerateChoices(double correct)
        {
            var set = new HashSet<double> { correct };

            while (set.Count < 4)
            {
                double offset = _rand.Next(-10, 11);
                double choice = correct + offset;

                if (choice >= 0)
                {
                    set.Add(choice);
                }
            }

            // Shuffle
            return set.OrderBy(x => _rand.Next()).ToList();
        }
    }

    // -----------------------------
    // EXTRA OOP CLASSES FOR PROJECT
    // -----------------------------

    public class Player
    {
        public string Name { get; set; } = "Guest";
        public int GamesPlayed { get; private set; }
        public int BestScore { get; private set; }

        public void FinishGame(int score)
        {
            GamesPlayed++;
            if (score > BestScore)
            {
                BestScore = score;
            }
        }
    }

    public class Question
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Symbol { get; set; } = "";
        public double CorrectAnswer { get; set; }
        public List<double> Choices { get; set; } = new();

        public bool IsCorrect(double value)
        {
            return Math.Abs(value - CorrectAnswer) < 0.01;
        }
    }

    public class ScoreTracker
    {
        public int TotalQuestions { get; private set; }
        public int CorrectAnswers { get; private set; }

        public void RegisterAnswer(bool isCorrect)
        {
            TotalQuestions++;
            if (isCorrect)
            {
                CorrectAnswers++;
            }
        }

        public int GetScore() => CorrectAnswers;

        public double GetAccuracy()
        {
            if (TotalQuestions == 0) return 0;
            return (double)CorrectAnswers / TotalQuestions;
        }
    }
}
