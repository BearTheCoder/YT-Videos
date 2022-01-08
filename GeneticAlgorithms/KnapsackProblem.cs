/*
    To use this code (C#): 
        1.) Create a new Console Application (Frameworks) name it "KnapSackProblem"
        2.) Copy everything in this file.
        3.) Replace everything in your application with what you copied.
        4.) If you named it something other than "KnapSackProblem", 
                Replace line 16 where it says "KnapSackProblem" to the name you choose.
 */

using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace KnapSackProblem {

    // Agent or Bot Class -------------------------------------------
    class Bot {
        internal List<bool> Chromosome { get; set; }
        internal double FitnessValue;
        internal double ItemsWeight;
        internal Bot() {
            Chromosome = new List<bool>();
        }
    }

    // Genetic Algorithm ------------------------------------------
    class Program {

        // Functional Variables --------------------------------------------------
        private static List<double> Items;
        private static List<Bot> Population;
        private static Random RandNum;
        private static Stopwatch Watch;
        private static int BagWeight;

        // Statistical Variables --------------------------------------------------
        private static int GenerationCount, FirstPerformanceGeneration, BestGeneration;
        private static double BestPerformance, FirstPerformance;

        // Functional "Settings" Variables ----------------------------------------
        private static readonly int PopulationSize = 100;   // More = Better but SLOWER
        private static readonly int ItemCount = 100;        // More = Harder
        private static readonly int MaxGenerations = 50000; // More = Better Optimization
        private static readonly int BagWeightMin = 30;      // Less = Harder
        private static readonly int BagWeightMax = 50;      // More = Easier
        private static readonly double ItemWeightMax = 10f;  // More = Harder

        static void Main(string[] args) {
            InitializeVariables();
            CreateItems();
            CreateFirstGeneration();
            for (int i = 0; i < MaxGenerations; i++) {
                FitnessTesting();
                DisplayStats();
                Selection();
            }
            DisplayFinalStats();
        }
        private static void InitializeVariables() {
            RandNum = new Random();
            Watch = new Stopwatch();
            Items = new List<double>();
            Population = new List<Bot>();
            BagWeight = RandNum.Next(BagWeightMin, BagWeightMax);
            Watch.Start();
        }
        private static void CreateItems() {
            for (int i = 0; i < ItemCount; i++) {
                Items.Add(RandNum.NextDouble() * ItemWeightMax);
            }
        }
        private static void CreateFirstGeneration() {
            for (int i = 0; i < PopulationSize; i++) {
                Bot Bot = new Bot();
                for (int j = 0; j < ItemCount; j++) {
                    Bot.Chromosome.Add(GetBool());
                }
                Population.Add(Bot);
            }
        }
        private static bool GetBool() {
            if (RandNum.Next(0, 2) == 0) { return true; }
            else { return false; }
        }
        private static void FitnessTesting() {
            GenerationCount++;
            foreach(Bot A in Population) {
                double TotalWeight = 0;
                for (int i = 0; i < ItemCount; i++) {
                    if (A.Chromosome[i] == true) { TotalWeight += Items[i]; }
                }
                A.ItemsWeight = TotalWeight;
                if (TotalWeight < BagWeight) { A.FitnessValue = TotalWeight / (double)BagWeight; }
                else { A.FitnessValue = 0; }
            }
        }
        private static void Selection() {
            // Sorting ---------------------------------------------------------------------------
            List<Bot> SortedAgents = Population.OrderByDescending(o => o.FitnessValue).ToList();
            if (SortedAgents[0].FitnessValue == 0) {
                SortedAgents = Population.OrderBy(o => o.ItemsWeight).ToList();
            }
            Population.Clear();
            //Selection --------------------------------------------------------------------------
            for (int i = 0; i < (SortedAgents.Count / 10); i++) {
                Population.Add(SortedAgents[i]);
            }
            for (int i = 0; i < ((SortedAgents.Count / 2) - (PopulationSize / 20)); i++) {
                Population.Add(Crossover(SortedAgents[i], SortedAgents[i + 1]));
                Population.Add(Crossover(SortedAgents[i + 1], SortedAgents[i]));
            }
        }
        private static Bot Crossover(Bot Parent1, Bot Parent2) {
            Bot Bot = new Bot();
            for (int i = 0; i < ItemCount; i++) {
                int CrossoverPoint = RandNum.Next(1, Parent1.Chromosome.Count);
                // New Chromosome Creation and Mutation ---------------------------------------------
                if (RandNum.Next(0, ItemCount) != 1) {
                    if (i < CrossoverPoint) { Bot.Chromosome.Add(Parent1.Chromosome[i]); }
                    else { Bot.Chromosome.Add(Parent2.Chromosome[i]); }
                }
                else {
                    if (i < CrossoverPoint) { Bot.Chromosome.Add(!Parent1.Chromosome[i]); }
                    else { Bot.Chromosome.Add(!Parent2.Chromosome[i]); }
                }
            }
            return Bot;
        }

        // Non-Functional Statistic "UI" -----------------------------------------------------------
        private static void DisplayStats() {
            Bot TopAgent = Population.OrderByDescending(o => o.FitnessValue).ToList()[0];
            if (FirstPerformanceGeneration == 0 && TopAgent.FitnessValue != 0) {
                FirstPerformanceGeneration = GenerationCount;
                FirstPerformance = TopAgent.FitnessValue;
            }
            if (TopAgent.FitnessValue > BestPerformance) {
                BestPerformance = TopAgent.FitnessValue;
                BestGeneration = GenerationCount;
            }
            NumberFormatInfo NFI = new CultureInfo("en-US", false).NumberFormat;
            NFI.PercentDecimalDigits = 8;
            Console.WriteLine("Generation: " + GenerationCount);
            Console.WriteLine("Population Count: " + Population.Count);
            Console.WriteLine("Top Performer Usage: " + TopAgent.FitnessValue.ToString("P", NFI));
            Console.WriteLine("\n");
        }
        private static void DisplayFinalStats() {
            double TotalSum = 0;
            foreach (double D in Items) {
                TotalSum += D;
            }
            NumberFormatInfo NFI = new CultureInfo("en-US", false).NumberFormat;
            NFI.PercentDecimalDigits = 8;
            TimeSpan T = new TimeSpan(Watch.ElapsedTicks);
            Console.WriteLine("FINAL STATS");
            Console.WriteLine("BagWeight: " + BagWeight);
            Console.WriteLine("Total Sum of Item Weights: " + TotalSum);
            Console.WriteLine("Max Weight of Each Item: " + ItemWeightMax);
            Console.WriteLine("Max Generations: " + MaxGenerations);
            Console.WriteLine("Population Count: " + PopulationSize);
            Console.WriteLine("First Perfomance: " + FirstPerformance.ToString("P", NFI));
            Console.WriteLine("First Perfomance Generation: " + FirstPerformanceGeneration);
            Console.WriteLine("Best Perfomance: " + BestPerformance.ToString("P", NFI));
            Console.WriteLine("Best Perfomance Generation: " + BestGeneration);
            Console.WriteLine("Total Time Elapsed in Seconds: " + T.TotalSeconds);
            Watch.Stop();
            Console.ReadLine();
        }
    }
}
