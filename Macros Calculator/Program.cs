using System;

namespace Macros_Calculator
{
    class Program
    {
        public enum Gender
        {
            Male = 1,
            Female
        }

        public enum ActivityLevel
        {
            Sedentary = 1,
            LightlyActive,
            Moderate,
            Active,
            VeryActive
        }

        static void Main(string[] args)
        {
            const double kgToLbMultiplyer = 2.2046226218;

            double weight;
            double bodyFatPercentage;
            double BMRUsingRegWeight;
            double BMRusingLBM;
            double lbm;
            double tdee;
            double tdeeLBM;
            Gender userGender;
            int height;
            int age;

            Console.Clear();

            Console.WriteLine("We will now be determining your Basal Metabolic Rate.\nPlease fill out the following questions as honestly as possible!\n");

            Console.WriteLine("What is your gender?");
            foreach (int val in Enum.GetValues(typeof(Gender)))
            {
                Console.WriteLine("{0}) {1}", val, Enum.GetName(typeof(Gender), val));
            }
            userGender = (Gender)Enum.Parse(typeof(Gender), Console.ReadLine());

            Console.WriteLine("\nWhat is your weight in kg? (round to 4 decimal points for accuracy)");
            weight = Double.Parse(Console.ReadLine());

            Console.WriteLine("\nWhat is your body fat percentage? (round to 1 decimal point)");
            bodyFatPercentage = Double.Parse(Console.ReadLine());

            lbm = weight - (weight * bodyFatPercentage / 100);

            Console.WriteLine("\nWhat is your height in cm?");
            height = Int16.Parse(Console.ReadLine());

            Console.WriteLine("\nWhat is your age in years?");
            age = Int16.Parse(Console.ReadLine());

            BMRUsingRegWeight = calculateBMR(userGender, weight, height, age);
            BMRusingLBM = calculateBMR(userGender, lbm, height, age);

            Console.Clear();

            displayStats(userGender, weight, bodyFatPercentage, height, age);
            Console.WriteLine("\nWe will now be determining your Total Daily Energy Expenditure (TDEE)\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.Clear();

            Console.WriteLine("What is your activity level?\n");

            foreach (int val in Enum.GetValues(typeof(ActivityLevel)))
            {
                Console.WriteLine("{0}) {1}", val, Enum.GetName(typeof(ActivityLevel), val));
            }

            ActivityLevel activitySelection = (ActivityLevel)Enum.Parse(typeof(ActivityLevel), Console.ReadLine());
            tdee = calculateTDEE(activitySelection, BMRUsingRegWeight);
            tdeeLBM = calculateTDEE(activitySelection, BMRusingLBM);

            Console.Clear();

            displayStats(userGender, weight, bodyFatPercentage, height, age);
            Console.WriteLine("TDEE using your regular weight: " + tdee);
            Console.WriteLine("TDEE using your LBM: " + tdeeLBM);
            Console.WriteLine("\nWe will now be determining your Macros (Fat, Carbs, and Protein)\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Console.WriteLine("\nIs your goal to lose weight? (Y/N)");
            bool isCutting = Console.ReadLine().ToUpper() == "Y" ? true : false;
            if (isCutting)
            {
                calculateMacros(tdeeLBM - 500, lbm * kgToLbMultiplyer);
            }
            else
            {
                calculateMacros(tdeeLBM, lbm);
            }

            Console.WriteLine("Thank you!");
            Console.WriteLine("Press any key to terminate program...");
            Console.ReadKey();

        }

        /// <summary>
        /// uses the Harris-Benedict equations revised by Roza & Shizgal to determine the Basal Metabolic Rate (BMR)
        /// BMR: Minimum number of calories required for basic functions at rest. i.e. How many calories you need to maintain your weight while doing absolutely no physical activity.
        /// </summary>
        /// <param name="gender">Only considers Male or Female</param>
        /// <param name="weight">Weight in KG</param>
        /// <param name="height">Height in CM</param>
        /// <param name="age">Age in years</param>
        /// <returns>Basal Metabolic Rate</returns>
        private static double calculateBMR(Gender gender, double weight, int height, int age)
        {

            double basicModifier;
            double weightModifier;
            double heightModifier;
            double ageModifier;
            
            if (gender == Gender.Male)
            {
                basicModifier = 88.362;
                weightModifier = 13.397;
                heightModifier = 4.799;
                ageModifier = 5.677;
            }
            else if (gender == Gender.Female)
            {
                basicModifier = 447.593;
                weightModifier = 9.247;
                heightModifier = 3.098;
                ageModifier = 4.330;
            }
            else
            {
                throw new ArgumentException("Gender inputted has not been implemented in this calculation");
            }

            return Math.Round(basicModifier + (weight * weightModifier) + (height * heightModifier) - (ageModifier * age));
        }

        /// <summary>
        /// Displays to console user statistics based on previous inputs
        /// </summary>
        /// <param name="gender">Only considers Male or Female</param>
        /// <param name="weight">Weight in KG</param>
        /// <param name="bodyFat">Body fat percentage rounded to 1 decimal place</param>
        /// <param name="height">Height in CM</param>
        /// <param name="age">Age in years</param>
        private static void displayStats(Gender gender, double weight, double bodyFat, int height, int age)
        {
            Console.WriteLine("Here are your stats:\n");
            Console.WriteLine("Gender: " + Gender.Male.ToString());
            Console.WriteLine("Height (in cm): " + height);
            Console.WriteLine("Body Fat Percentage: " + bodyFat);
            Console.WriteLine("Weight (in kg): " + weight);
            Console.WriteLine("Lean Body Mass (LBM): {0}", Math.Round(weight - (weight * bodyFat / 100),4));
        }

        /// <summary>
        /// Calculates Total Daily Energy Expenditure (TDEE) using selected activity level of user.
        /// TDEE: Minimum amount of calories required to maintain weight while considering activity level.
        /// </summary>
        /// <param name="activityLevel">User's current average level of activity</param>
        /// <param name="BMR">Basal Metabolic Rate</param>
        /// <returns>Rounded TDEE</returns>
        private static double calculateTDEE(ActivityLevel activityLevel, double BMR)
        {

            double activityFactor = 0;

            switch (activityLevel)
            {
                case ActivityLevel.Sedentary:
                    activityFactor = 1.2;
                    break;
                case ActivityLevel.LightlyActive:
                    activityFactor = 1.375;
                    break;
                case ActivityLevel.Moderate:
                    activityFactor = 1.55;
                    break;
                case ActivityLevel.Active:
                    activityFactor = 1.725;
                    break;
                case ActivityLevel.VeryActive:
                    activityFactor = 1.9;
                    break;
            }

            return Math.Round(BMR * activityFactor);
        }

        /// <summary>
        /// Calculates macros by considering a 1:1 ratio for Proteins:Weight, 30% Fat, rest is Carbs
        /// </summary>
        /// <param name="tdee">Total Daily Energy Expenditure</param>
        /// <param name="weight">Weight in LBS</param>
        private static void calculateMacros(double tdee, double weight)
        {
            int carbMultiplyer = 4;
            int proteinMultiplyer = 4;
            int fatMultiplyer = 9;

            int carbCals;
            int proteinCals;
            int fatCals;

            int carbs;
            int proteins;
            int fats;

            proteins = (int) Math.Ceiling(weight);
            fats = (int) Math.Ceiling(0.3 * weight);

            proteinCals = proteinMultiplyer * proteins;
            fatCals = fatMultiplyer * fats;
            carbCals = (int) (Math.Ceiling(tdee) - proteinCals - fatCals);

            carbs = carbCals / carbMultiplyer;

            Console.WriteLine("TDEE : " + tdee);
            Console.WriteLine("F: {0} ({1} cals)", fats, fatCals);
            Console.WriteLine("C: {0} ({1} cals)", carbs, carbCals);
            Console.WriteLine("P: {0} ({1} cals)", proteins, proteinCals);
        }
    }
}
