using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    // Identifies which input field a validation problem belongs to.
    public enum AnimalField
    {
        AnimalId,
        Name,
        Species,
        Age,
        RecoveryScore
    }

    // The outcome of validating the user's input.
    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public AnimalField Field { get; private set; }
        public string Message { get; private set; }
        public Animal Animal { get; private set; }

        // Creates a successful result that carries the fully built animal.
        public static ValidationResult Success(Animal animal)
        {
            return new ValidationResult { IsValid = true, Animal = animal };
        }

        // Creates a failed result naming the problem field and describing the problem.
        public static ValidationResult Failure(AnimalField field, string message)
        {
            return new ValidationResult { IsValid = false, Field = field, Message = message };
        }
    }

    // Validates animal input.
    public static class AnimalValidator
    {
        private const int MaxTextLength = 30;
        private const int MinNumber = 0;
        private const int MaxNumber = 100;

        // Animal ID must be WR- followed by exactly four digits.
        private static readonly Regex IdPattern = new Regex("^WR-[0-9]{4}$");

        // Validates every field in order (ID, Name, Species, Age, Score) and stops at the first problem.
        // Set checkUniqueId to false when updating, because the ID already exists and cannot change.
        public static ValidationResult Validate(string idText, string nameText, string speciesText,
            string ageText, string scoreText, IEnumerable<Animal> existingAnimals, bool checkUniqueId)
        {
            // Trim everything first, as required, treating null as empty.
            string id = (idText ?? "").Trim().ToUpperInvariant();
            string name = (nameText ?? "").Trim();
            string species = (speciesText ?? "").Trim();
            string ageInput = (ageText ?? "").Trim();
            string scoreInput = (scoreText ?? "").Trim();

            // Animal ID.
            if (id.Length == 0)
            {
                return ValidationResult.Failure(AnimalField.AnimalId, "Animal ID is required.");
            }
            if (!IdPattern.IsMatch(id))
            {
                return ValidationResult.Failure(AnimalField.AnimalId,
                    "Animal ID must be in the format WR-0000 (WR- followed by exactly 4 digits).");
            }
            if (checkUniqueId && IdAlreadyExists(id, existingAnimals))
            {
                return ValidationResult.Failure(AnimalField.AnimalId,
                    "Animal ID " + id + " already exists. Each Animal ID must be unique.");
            }

            // Name and Species share the same rules.
            ValidationResult textProblem = CheckText(name, "Name", AnimalField.Name);
            if (textProblem != null) return textProblem;

            textProblem = CheckText(species, "Species", AnimalField.Species);
            if (textProblem != null) return textProblem;

            // Age and Recovery Score share the same rules.
            int age;
            ValidationResult numberProblem = CheckWholeNumber(ageInput, "Age", AnimalField.Age, out age);
            if (numberProblem != null) return numberProblem;

            int score;
            numberProblem = CheckWholeNumber(scoreInput, "Recovery Score", AnimalField.RecoveryScore, out score);
            if (numberProblem != null) return numberProblem;

            string status;
            string housingUnit;
            AnimalClassifier.Classify(score, out status, out housingUnit);

            return ValidationResult.Success(new Animal(id, name, species, age, score, status, housingUnit));
        }

        // Checks a Name or Species value: required, at most 30 characters, no pipe character.
        // Returns null when the text is fine, otherwise a failure result.
        private static ValidationResult CheckText(string text, string label, AnimalField field)
        {
            if (text.Length == 0)
            {
                return ValidationResult.Failure(field, label + " is required.");
            }
            if (text.Length > MaxTextLength)
            {
                return ValidationResult.Failure(field,
                    label + " must be " + MaxTextLength + " characters or fewer.");
            }
            if (text.Contains("|"))
            {
                return ValidationResult.Failure(field, label + " cannot contain the pipe character (|).");
            }
            return null;
        }

        // Checks an Age or Recovery Score value: required and a whole number from 0 to 100.
        private static ValidationResult CheckWholeNumber(string text, string label, AnimalField field, out int value)
        {
            value = 0;

            if (text.Length == 0)
            {
                return ValidationResult.Failure(field, label + " is required.");
            }

            if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value)
                || value < MinNumber || value > MaxNumber)
            {
                return ValidationResult.Failure(field,
                    label + " must be a whole number between " + MinNumber + " and " + MaxNumber + ".");
            }
            return null;
        }

        // Returns true if an animal with this (already uppercase) ID is in the list.
        private static bool IdAlreadyExists(string id, IEnumerable<Animal> existingAnimals)
        {
            if (existingAnimals == null) return false;

            foreach (Animal existing in existingAnimals)
            {
                if (string.Equals(existing.ID, id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
