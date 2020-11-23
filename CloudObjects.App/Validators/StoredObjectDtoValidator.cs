using System;
using System.Text.RegularExpressions;
using CloudObjects.App.Interfaces;
using CloudObjects.Models;
using FluentValidation;

namespace CloudObjects.App.Validators
{
    public class StoredObjectDtoValidator : AbstractValidator<StoredObject>, IDtoValidator
    {
        private readonly Regex _notAllowedCharacters = new Regex("[?!@#$%^'|&*()+=;\"]+");

        public StoredObjectDtoValidator()
        {
            RuleFor(e => e.Json)
                .NotEmpty();

            RuleFor(e => e.Name)
                .MaximumLength(512)
                .Must(name =>
                {
                    if (!Uri.IsWellFormedUriString(name, UriKind.Relative)) return false;
                    if (_notAllowedCharacters.IsMatch(name)) return false;

                    return true;
                })
                    .WithMessage($"Object Name must be well-formed URI string, and may not contain these characters: {string.Join(", ", _notAllowedCharacters)}.");
        }
    }
}
